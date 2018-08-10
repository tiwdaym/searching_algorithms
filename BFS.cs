using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms
{
    class BFS<T>
        where T : IEquatable<T>, IHashable, IGenerative<T>
    {
        HeapMinList<GraphNodeSimple<T>> openSet, openSetReversed;
        HashList<GraphNodeSimple<T>> closedSet, closedSetReversed;
        DateTime startTime;
        bool isProcessingChangesDisabled = false;


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

        bool useBidirectionalSearch = false;
        public bool UseBidirectionalSearch
        {
            get => useBidirectionalSearch;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change search type while in processing.");
                useBidirectionalSearch = value;
            }
        }


        public BFS(T startState, T finishState, uint maxSearchingDepth = 0, uint maxSearchingTime = 0, uint maxGeneratedElementsCount = 65536, uint hashSize = 65536)
        {
            pathResult = new GeneratedPath<T>();
            MaxSearchingDepth = maxSearchingDepth;
            MaxSearchingTime = maxSearchingTime;
            MaxGeneratedElementsCount = maxGeneratedElementsCount;
            HashSize = hashSize;
            StartState = startState;
            FinishState = finishState;
        }

        
        /// <summary>
        /// Function call will find Path by default set parameters
        /// </summary>
        /// <returns>Result is also stored in pathResult</returns>
        public GeneratedPath<T> findPath()
        {
            if (useBidirectionalSearch) return findPathBidirectional();
            else return findPathBasic();
        }


        /// <summary>
        /// Basic search from start to finish
        /// </summary>
        GeneratedPath<T> findPathBasic()
        {
            GraphNodeSimple<T> currentGraphNode;
            T tempTState;

            //initialization
            isProcessingChangesDisabled = true;
            openSet = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount);
            closedSet = new HashList<GraphNodeSimple<T>>(hashSize, maxGeneratedElementsCount);

            pathResult = new GeneratedPath<T>();
            startTime = DateTime.UtcNow;


            //1. add first element
            openSet.Add(new GraphNodeSimple<T>(startState, null, null, 0));
            pathResult.generatedNodes++;

            //2. check if graph is not empty
            while (openSet.Count > 0)
            {
                //2.1 special - add maximum used heap and hash size
                if (pathResult.maximumUsedHeapMemory < openSet.Count) pathResult.maximumUsedHeapMemory = openSet.Count;
                if (pathResult.maximumUsedHashMemory < closedSet.Count) pathResult.maximumUsedHashMemory = closedSet.Count;

                //3. select best non-processed graphState
                currentGraphNode = openSet.RemoveMin();
                pathResult.searchedNodes++;

                //4. test if graphNodee is finish, or depth is maxDepth or bigger. For 0 maxDepth just ignore depth.
                //also check for elapsed time in miliseconds. For 0 maxtime, just ignore time.
                //If some of these apply, finish searching and return found path from current node.
                if (currentGraphNode.node.Equals(finishState) ||
                    ((maxSearchingDepth != 0) && (currentGraphNode.graphDepth > maxSearchingDepth)) ||
                    ((maxSearchingTime != 0) && ((DateTime.UtcNow).Subtract(startTime).TotalMilliseconds > maxSearchingTime)))
                {
                    string[] foundOperationsPath = new string[currentGraphNode.graphDepth + 1];
                    T[] foundStatesPath = new T[currentGraphNode.graphDepth + 1];
                    for (int i = currentGraphNode.graphDepth; i > 0; i--)
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

                //6. create childs (neighbours) of current node and add them to open and closed set for checking
                for (int i = (operationsList.Length - 1); i >= 0; i--)
                {
                    tempTState = currentGraphNode.node.GenerateNewState(operationsList[i]);
                    pathResult.generatedNodes++;
                    if (tempTState == null) continue;
                    GraphNodeSimple<T> tmpGraphNode = new GraphNodeSimple<T>(tempTState, currentGraphNode, operationsList[i], currentGraphNode.graphDepth + 1);
                    if (closedSet.Contains(tmpGraphNode)) continue;
                    openSet.Add(tmpGraphNode);
                    closedSet.Add(tmpGraphNode);
                }

            }

            ResetProcessing();

            return null;
        }

        /// <summary>
        /// Bidirectional search from start and from finish
        /// </summary>
        GeneratedPath<T> findPathBidirectional()
        {
            GraphNodeSimple<T> currentGraphNode;
            T tempTState;
            HeapMinList<GraphNodeSimple<T>> currentOpenSet;
            HashList<GraphNodeSimple<T>> currentClosedSet;
            bool isProcessingReversedSet = false;

            //initialization
            isProcessingChangesDisabled = true;
            openSet = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount / 2);
            closedSet = new HashList<GraphNodeSimple<T>>(hashSize / 2, maxGeneratedElementsCount / 2);
            openSetReversed = new HeapMinList<GraphNodeSimple<T>>(maxGeneratedElementsCount / 2);
            closedSetReversed = new HashList<GraphNodeSimple<T>>(hashSize / 2, maxGeneratedElementsCount / 2);

            pathResult = new GeneratedPath<T>();
            startTime = DateTime.UtcNow;


            //1. add first and last element
            GraphNodeSimple<T> tmpGraphNode = new GraphNodeSimple<T>(startState, null, null, 0);
            openSet.Add(tmpGraphNode);
            closedSet.Add(tmpGraphNode);
            tmpGraphNode = new GraphNodeSimple<T>(finishState, null, null, 0);
            openSetReversed.Add(tmpGraphNode);
            closedSetReversed.Add(tmpGraphNode);
            pathResult.generatedNodes += 2;

            //2. check if graph is not empty
            while (openSet.Count > 0 || openSetReversed.Count > 0)
            {
                //2.1 special - add maximum used heap and hash size
                if (pathResult.maximumUsedHeapMemory < openSet.Count + openSetReversed.Count) pathResult.maximumUsedHeapMemory = openSet.Count + openSetReversed.Count;
                if (pathResult.maximumUsedHashMemory < closedSet.Count + closedSetReversed.Count) pathResult.maximumUsedHashMemory = closedSet.Count + closedSetReversed.Count;

                //3. select best non-processed graphState
                if (openSet.Count > 0 && (openSet.GetMin().graphDepth <= openSetReversed.GetMin().graphDepth))
                {
                    currentOpenSet = openSet;
                    currentClosedSet = closedSet;
                    isProcessingReversedSet = false;
                }
                else
                {
                    currentOpenSet = openSetReversed;
                    currentClosedSet = closedSetReversed;
                    isProcessingReversedSet = true;
                }

                currentGraphNode = currentOpenSet.GetMin();
                pathResult.searchedNodes++;

                //4. test if graphNode is in opposite closedSet (generated), or depth is maxDepth or bigger. For 0 maxDepth just ignore depth.
                //also check for elapsed time in miliseconds. For 0 maxtime, just ignore time.
                //If some of these apply, finish searching and return found path from current node.
                if ((isProcessingReversedSet ? closedSet : closedSetReversed).Contains(currentGraphNode) ||
                    ((maxSearchingDepth != 0) && (currentGraphNode.graphDepth > maxSearchingDepth)) ||
                    ((maxSearchingTime != 0) && ((DateTime.UtcNow).Subtract(startTime).TotalMilliseconds > maxSearchingTime)))
                {
                    GraphNodeSimple<T> graphNodeFromStart = isProcessingReversedSet ? null : currentGraphNode;
                    GraphNodeSimple<T> graphNodeFromFinish = isProcessingReversedSet ? currentGraphNode : null;

                    if (graphNodeFromStart == null) closedSet.TryGetValue(graphNodeFromFinish, out graphNodeFromStart);
                    if (graphNodeFromFinish == null) closedSetReversed.TryGetValue(graphNodeFromStart, out graphNodeFromFinish);
                    //if any graphNodeFrom is still null, create non-null null graph node
                    if (!(isProcessingReversedSet ? closedSet : closedSetReversed).Contains(currentGraphNode)) {
                        graphNodeFromFinish = new GraphNodeSimple<T>(finishState, null, null, 0);
                        if (graphNodeFromStart == null) graphNodeFromStart = openSet.GetMin();
                    }

                    //4.0 init operation paths arrays
                    string[] foundOperationsPath = new string[graphNodeFromStart.graphDepth + graphNodeFromFinish.graphDepth + 1];
                    T[] foundStatesPath = new T[graphNodeFromStart.graphDepth + graphNodeFromFinish.graphDepth + 1];

                    //4.1 save last node from start operations for processing on operations to finish
                    tmpGraphNode = graphNodeFromStart;
                    //4.2 now fill up all moves from start
                    for (int i = graphNodeFromStart.graphDepth; i > 0; i--)
                    {
                        foundStatesPath[i] = graphNodeFromStart.node;
                        foundOperationsPath[i] = graphNodeFromStart.lastOperation;
                        graphNodeFromStart = graphNodeFromStart.graphParent;
                    }

                    //4.3 initialize states to check from start to finish
                    T lastState = tmpGraphNode.node;
                    T tmpState = default(T);
                    string foundOperation = null;

                    //4.4 now fill up all moves to end and find which operation was needed to move to state
                    int totalGraphDepth = tmpGraphNode.graphDepth + graphNodeFromFinish.graphDepth;
                    for (int i_nodes = tmpGraphNode.graphDepth + 1; i_nodes <= totalGraphDepth; i_nodes++)
                    {
                        tmpState = default(T);
                        string[] tmpOperations = lastState.OperationsList();
                        //4.3.a try every operation if it is possible to create state
                        for (int i_operations = tmpOperations.Length - 1; i_operations >= 0; i_operations--)
                        {
                            foundOperation = tmpOperations[i_operations];
                            tmpState = lastState.GenerateNewState(foundOperation);
                            if (tmpState != null && tmpState.Equals(graphNodeFromFinish.graphParent.node)) break;
                        }
                        if (tmpState == null) throw new InvalidOperationException("There is no possible operation that can create state from starting state by backward moving.");
                        foundStatesPath[i_nodes] = graphNodeFromFinish.graphParent.node;
                        foundOperationsPath[i_nodes] = foundOperation;
                        lastState = tmpState;
                        graphNodeFromFinish = graphNodeFromFinish.graphParent;
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
                currentClosedSet.Add(currentGraphNode);
                //5.1 Remove currentProcessed node from OpenSet
                currentOpenSet.RemoveMin();

                string[] operationsList = currentGraphNode.node.OperationsList();

                //6. create childs (neighbours) of current node and add them to open and closed set for checking
                for (int i = operationsList.Length - 1; i >= 0; i--)
                {
                    tempTState = currentGraphNode.node.GenerateNewState(operationsList[i]);
                    pathResult.generatedNodes++;
                    if (tempTState == null) continue;
                    tmpGraphNode = new GraphNodeSimple<T>(tempTState, currentGraphNode, operationsList[i], currentGraphNode.graphDepth + 1);
                    if (currentClosedSet.Contains(tmpGraphNode)) continue;
                    currentOpenSet.Add(tmpGraphNode);
                    currentClosedSet.Add(tmpGraphNode);
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
            closedSetReversed = null;
            openSetReversed = null;

            isProcessingChangesDisabled = false;
        }
    }
}
