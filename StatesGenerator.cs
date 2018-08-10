using SearchingAlgorithms.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class StatesGenerator<T>
        where T : IEquatable<T>, IHashable, IGenerative<T>
    {
        HeapMinList<GraphNodeSimple<T>> openSet, nextOpenSet;
        HashList<GraphNodeSimple<T>> prevClosedSet, closedSet, nextClosedSet;

        DateTime startTime;
        bool isProcessingChangesDisabled = false;

        bool useSqlLiteAsStorage = false;
        /// <summary>
        /// If using SqlLite as storage, HashSize and MaxGeneratedElementsCount are not used
        /// </summary>
        bool UseSqlLiteAsStorage
        {
            get => useSqlLiteAsStorage;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change useSqlLiteAsStorage while in processing.");
                useSqlLiteAsStorage = value;
            }
        }

        string sqlLiteDatabasesPrefix = "States_Generator_";
        string SqlLiteDatabasesPrefix
        {
            get => sqlLiteDatabasesPrefix;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change sqlLiteDatabasesPrefix while in processing.");
                sqlLiteDatabasesPrefix = value;
            }
        }

        T startState;
        /// <summary>
        /// You can set or change starting state from where to find a path when pathfinding.
        /// Not allowed to change during computation - otherwise error is thrown.
        /// </summary>
        /// <param name="startState"></param>
        public T StartState
        {
            get => startState;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change start state while in processing.");
                if (value == null) throw new ArgumentNullException("Start state cannot be null.");
                startState = value;
            }
        }


        uint maxSearchingDepth = 0;
        /// <summary>
        /// You can set maximum Finding Depth to where algorithm will go.
        /// After reaching the depth, partial shortest path is returned.
        /// Usable while computing.
        /// Use 0 for unlimited depth.
        /// </summary>
        public uint MaxSearchingDepth { get => maxSearchingDepth; set => maxSearchingDepth = value; }

        uint maxSearchingTime = 0;
        /// <summary>
        /// You can set maximum Finding Time in miliseconds.
        /// After reaching the time, partial shortest path is returned.
        /// Usable while computing.
        /// Use 0 for unlimited time.
        /// </summary>
        public uint MaxSearchingTime { get => maxSearchingTime; set => maxSearchingTime = value; }

        uint maxGeneratedElementsCount = 65536;
        uint hashSize = 65536;


        public uint MaxGeneratedElementsCount
        {
            get => maxGeneratedElementsCount;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change maximum generated elements count while in processing.");
                maxGeneratedElementsCount = value;
            }
        }

        public uint HashSize
        {
            get => hashSize;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Hash size while in processing.");
                hashSize = value;
            }
        }

        public StatesGenerator(T startState, uint maxSearchingDepth = 0, uint maxSearchingTime = 0, uint maxGeneratedElementsCount = 65536, uint hashSize = 65536)
        {
            MaxSearchingDepth = maxSearchingDepth;
            MaxSearchingTime = maxSearchingTime;
            MaxGeneratedElementsCount = maxGeneratedElementsCount;
            HashSize = hashSize;
            StartState = startState;
        }

        /// <summary>
        /// Function will generate all possible states in graph sorted by depth.
        /// Count of states is defined by MaxGeneratedElementsCount
        /// Maximum depth for states is defined by MaxSearchingDepth
        /// </summary>
        /// <returns></returns>
        public GraphNodeSimple<T>[] GenerateStates()
        {
            GraphNodeSimple<T> currentGraphNode, tmpGraphNode;
            T tempTState;
            SingleLinkedList<GraphNodeSimple<T>> generatedNodes = new SingleLinkedList<GraphNodeSimple<T>>();

            //initialization
            isProcessingChangesDisabled = true;
            openSet = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount);
            closedSet = new HashList<GraphNodeSimple<T>>(hashSize, maxGeneratedElementsCount);

            startTime = DateTime.UtcNow;

            //1. add first element
            openSet.Add(new GraphNodeSimple<T>(startState, null, null, 0));
            //closedSet.Add(openSet.GetMin());

            //2. Repeat until openSet have generated nodes
            while (openSet.Count > 0)
            {

                //3. Get first min node from graph and save it to list of generatedNodes if depth applies
                currentGraphNode = openSet.RemoveMin();
                if (generatedNodes.Count < maxGeneratedElementsCount && (maxSearchingDepth == 0 || currentGraphNode.graphDepth <= maxSearchingDepth)) generatedNodes.Add(currentGraphNode);

                //4. add current node to hash
                closedSet.Add(currentGraphNode);

                //5. create childs (neighbours) of current node and add them to open and closed set for checking
                string[] operationsList = currentGraphNode.node.OperationsList();
                for (int i = (operationsList.Length - 1); i >= 0; i--)
                {
                    tempTState = currentGraphNode.node.GenerateNewState(operationsList[i]);
                    if (tempTState == null) continue;
                    tmpGraphNode = new GraphNodeSimple<T>(tempTState, currentGraphNode, operationsList[i], currentGraphNode.graphDepth + 1);
                    if (closedSet.Contains(tmpGraphNode)) continue;
                    openSet.Add(tmpGraphNode);
                    closedSet.Add(tmpGraphNode);
                }

                //6. additional check if we should end the loop
                if (generatedNodes.Count >= maxGeneratedElementsCount ||
                    ((maxSearchingDepth != 0) && (currentGraphNode.graphDepth > maxSearchingDepth)) ||
                    ((maxSearchingTime != 0) && ((DateTime.UtcNow).Subtract(startTime).TotalMilliseconds > maxSearchingTime))) break;

            }

            ResetProcessing();

            //7. Return generated ndoes as list.
            return generatedNodes.ToList();
        }


        /// <summary>
        /// Function will generate all possible states in graph in particular provided depth.
        /// Count of states is defined by MaxGeneratedElementsCount
        /// </summary>
        /// <param name="statesDepth">Specify for states in particular depth</param>
        /// <returns></returns>
        public GraphNodeSimple<T>[] GenerateStates(uint statesDepth)
        {
            GraphNodeSimple<T> currentGraphNode, tmpGraphNode;
            T tempTState;
            SingleLinkedList<GraphNodeSimple<T>> generatedNodes = new SingleLinkedList<GraphNodeSimple<T>>();
            uint currentDepth = 0;
            bool doProcessing = true;

            //initialization
            isProcessingChangesDisabled = true;
            openSet = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount);
            nextOpenSet = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount);
            closedSet = new HashList<GraphNodeSimple<T>>(hashSize, maxGeneratedElementsCount);
            prevClosedSet = new HashList<GraphNodeSimple<T>>(hashSize, maxGeneratedElementsCount);
            nextClosedSet = new HashList<GraphNodeSimple<T>>(hashSize, maxGeneratedElementsCount);

            startTime = DateTime.UtcNow;

            //1. add first element
            openSet.Add(new GraphNodeSimple<T>(startState, null, null, 0));
            closedSet.Add(openSet.GetMin());

            //2. Repeat until openSet have generated nodes
            while (currentDepth < statesDepth && openSet.Count > 0 && doProcessing)
            {
                while (openSet.Count > 0 && doProcessing)
                {

                    //3. Get first min node from graph
                    currentGraphNode = openSet.RemoveMin();

                    //4. add current node to hash
                    closedSet.Add(currentGraphNode);

                    //5. create childs (neighbours) of current node and add them to open and closed set
                    string[] operationsList = currentGraphNode.node.OperationsList();
                    for (int i = (operationsList.Length - 1); i >= 0 && doProcessing; i--)
                    {
                        tempTState = currentGraphNode.node.GenerateNewState(operationsList[i]);
                        if (tempTState == null) continue;
                        tmpGraphNode = new GraphNodeSimple<T>(tempTState, null, operationsList[i], currentGraphNode.graphDepth + 1);
                        if (closedSet.Contains(tmpGraphNode) || nextClosedSet.Contains(tmpGraphNode) || prevClosedSet.Contains(tmpGraphNode)) continue;
                        nextOpenSet.Add(tmpGraphNode);
                        nextClosedSet.Add(tmpGraphNode);
                    }

                    //6. additional check if we should end the loop
                    if (generatedNodes.Count >= maxGeneratedElementsCount ||
                        ((maxSearchingTime != 0) && ((DateTime.UtcNow).Subtract(startTime).TotalMilliseconds > maxSearchingTime)))
                        doProcessing = false;

                }

                //increase depth
                currentDepth++;
                prevClosedSet = closedSet;
                closedSet = nextClosedSet;
                nextClosedSet = new HashList<GraphNodeSimple<T>>(hashSize, maxGeneratedElementsCount);

                openSet = nextOpenSet;
                nextOpenSet = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount);
            }

            // Get found states
            while (openSet.Count > 0) generatedNodes.Add(openSet.RemoveMin());

            //7. Return generated nodes as sorted list.
            ResetProcessing();
            return generatedNodes.ToList();
        }

       

        public void ResetProcessing()
        {
            closedSet = null;
            prevClosedSet = null;
            nextClosedSet = null;

            nextOpenSet = null;
            openSet = null;

            isProcessingChangesDisabled = false;
        }
    }
}
