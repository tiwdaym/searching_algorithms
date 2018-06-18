using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms
{
    class DFS<T>
        where T : IEquatable<T>, IHashable, IGenerative<T>, IHeuristical<T>
    {
        StackList<GraphNodeComplex<T>> openSet;
        HashList<GraphNodeComplex<T>> closedSet;
        DateTime startTime;
        bool isProcessingChangesDisabled = false;
        bool useBetterPath = false;
        /// <summary>
        /// Set true, if you want graph to use better path when collisions will be found in path generation
        /// </summary>
        public bool UseBetterPath { get => useBetterPath; set => useBetterPath = value; }


        GeneratedPath<T> pathResult = null;
        public GeneratedPath<T> PathResult { get => pathResult; }

        T startState;
        T finishState;
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
        /// <summary>
        /// You can set or change starting state from where to find a path when pathfinding.
        /// Not allowed to change during computation - otherwise error is thrown.
        /// </summary>
        /// <param name="startState"></param>
        public T FinishState
        {
            get => finishState;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change finish state while in processing.");
                if (value == null) throw new ArgumentNullException("Finish state cannot be null.");
                finishState = value;
            }
        }


        uint maxSearchingDepth = 0;
        uint maxSearchingTime = 0;
        /// <summary>
        /// You can set maximum Finding Depth to where algorithm will go.
        /// After reaching the depth, partial shortest path is returned.
        /// Usable while computing.
        /// Use 0 for unlimited depth.
        /// </summary>
        public uint MaxSearchingDepth { get => maxSearchingDepth; set => maxSearchingDepth = value; }
        /// <summary>
        /// You can set maximum Finding Time in miliseconds.
        /// After reaching the time, partial shortest path is returned.
        /// Usable while computing.
        /// Use 0 for unlimited time.
        /// </summary>
        public uint MaxSearchingTime { get => maxSearchingTime; set => maxSearchingTime = value; }


        uint maxStackSize = 65536;
        uint hashSize = 65536;

        public uint MaxStackSize
        {
            get => maxStackSize;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change maximum generated elements count while in processing.");
                maxStackSize = value;
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

        int heuristicParam;
        public int HeuristicParam
        {
            get => heuristicParam;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Heuristic Param while in processing.");
                heuristicParam = value;
            }
        }



        public DFS(T startState, T finishState, uint maxSearchingDepth = 0, uint maxSearchingTime = 0, uint maxStackSize = 65536, uint hashSize = 65536, int heuristicParam = 0)
        {
            pathResult = new GeneratedPath<T>();
            MaxSearchingDepth = maxSearchingDepth;
            MaxSearchingTime = maxSearchingTime;
            MaxStackSize = maxStackSize;
            HashSize = hashSize;
            StartState = startState;
            FinishState = finishState;
            HeuristicParam = heuristicParam;
        }

        /// <summary>
        /// Function call will find Path by default set parameters
        /// </summary>
        /// <returns>Result is also stored in pathResult</returns>
        public GeneratedPath<T> findPath()
        {
            GraphNodeComplex<T> currentGraphNode, tmpGraphNode, tmpGraphNode2;
            T tempTState;

            //initialization
            isProcessingChangesDisabled = true;
            openSet = new StackList<GraphNodeComplex<T>>(maxStackSize);
            closedSet = new HashList<GraphNodeComplex<T>>(hashSize, maxStackSize);

            pathResult = new GeneratedPath<T>();
            startTime = DateTime.UtcNow;


            //1. add first element
            openSet.Push(new GraphNodeComplex<T>(startState, null, null, 0, startState.HeuristicDistance(finishState, heuristicParam)));
            pathResult.generatedNodes++;

            //2. check if graph is not empty
            while (openSet.Count > 0)
            {
                //2.1 special - add maximum used heap and hash size
                if (pathResult.maximumUsedHeapMemory < openSet.Count) pathResult.maximumUsedHeapMemory = openSet.Count;
                if (pathResult.maximumUsedHashMemory < closedSet.Count) pathResult.maximumUsedHashMemory = closedSet.Count;

                //3. select best non-processed graphState
                currentGraphNode = openSet.Pop();
                pathResult.searchedNodes++;
                if (closedSet.Contains(currentGraphNode)) continue;

                //4. test if graphNode is finish, or depth is maxDepth or bigger. For 0 maxDepth just ignore depth.
                //also check for elapsed time in miliseconds. For 0 maxtime, just ignore time.
                //If some of these apply, finish searching and return found path from current node.
                if (currentGraphNode.node.Equals(finishState) ||
                    ((maxSearchingDepth != 0) && (currentGraphNode.realGraphDepth > maxSearchingDepth)) ||
                    ((maxSearchingTime != 0) && ((DateTime.UtcNow).Subtract(startTime).TotalMilliseconds > maxSearchingTime)))
                {
                    string[] foundOperationsPath = new string[currentGraphNode.realGraphDepth + 1];
                    T[] foundStatesPath = new T[currentGraphNode.realGraphDepth + 1];
                    for (int i = currentGraphNode.realGraphDepth; i > 0; i--)
                    {
                        foundStatesPath[i] = currentGraphNode.node;
                        foundOperationsPath[i] = currentGraphNode.lastOperation;
                        currentGraphNode = currentGraphNode.graphParent;
                    }
                    //first state was starting and we dont know which move user used to create such a state. So we are using -1 as undefined.
                    //Or we can try to think a little and create some super Intelligent Intelligence that can guess with 100% precision
                    //which move author used to create such a state in which this state now is. Maybe somewhere in future...
                    foundStatesPath[0] = currentGraphNode.node;
                    foundOperationsPath[0] = null;

                    pathResult.pathStates = foundStatesPath;
                    pathResult.pathLength = (uint)foundOperationsPath.Length;
                    pathResult.pathOperations = foundOperationsPath;
                    pathResult.heuristicParamUsed = default(int);
                    pathResult.totalTimeTaken = (DateTime.UtcNow).Subtract(startTime);
                    ResetProcessing();
                    //yes, now we should return it and... maybe go to sleep (bed)? :)
                    return pathResult;
                }

                //5. add current node to hash
                closedSet.Add(currentGraphNode);

                string[] operationsList = currentGraphNode.node.OperationsList();
                HeapMaxList<GraphNodeComplex<T>> sortedGraphNodes = new HeapMaxList<GraphNodeComplex<T>>((uint)operationsList.Length);
                //6. create childs (neighbours) of current node and add them to open and closed set for checking
                for (int i = (operationsList.Length - 1); i >= 0; i--)
                {
                    tempTState = currentGraphNode.node.GenerateNewState(operationsList[i]);
                    pathResult.generatedNodes++;
                    if (tempTState == null) continue;
                    tmpGraphNode = new GraphNodeComplex<T>(tempTState, currentGraphNode, operationsList[i], currentGraphNode.realGraphDepth + 1, currentGraphNode.realGraphDepth + 1 + tempTState.HeuristicDistance(finishState, heuristicParam));

                    if (closedSet.TryGetValue(tmpGraphNode, out tmpGraphNode2))
                    {
                        if (useBetterPath)
                        {
                            if (tmpGraphNode.comparingParam < tmpGraphNode2.comparingParam)
                            {
                                closedSet.Remove(tmpGraphNode2);
                                sortedGraphNodes.Add(tmpGraphNode);
                            }
                            continue;
                        }
                        else continue;
                    }
                    
                    sortedGraphNodes.Add(tmpGraphNode);
                }
                while (sortedGraphNodes.Count > 0)
                {
                    openSet.Push(sortedGraphNodes.RemoveMax());
                }

            }

            ResetProcessing();

            return null;
        }



        /// <summary>
        /// This should be called to clean hash and heap lists and reset processing stage
        /// </summary>
        public void ResetProcessing()
        {
            closedSet = null;
            openSet = null;

            isProcessingChangesDisabled = false;
        }
    }
}
