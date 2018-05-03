using System;

#region Astar code

#region AStar interfaces Definition
/// <summary>
/// Interface defines heuristical function, needed in A* path computation.
/// </summary>
/// <typeparam name="TState">Class wrapper for data.</typeparam>
public interface Heuristical<TState>
{
    /// <summary>
    /// Heuristic should return estimated distance to given graphState.
    /// </summary>
    /// <param name="state">State to compute distance to.</param>
    /// <param name="param">Optional parameters to give for heuristical function.</param>
    /// <returns>Estimated distance to given graphState as integer. Use 0 if graphState is equal to given graphState.</returns>
    int getHeuristicDistance(TState state, int param = null);
}

/// <summary>
/// Interface defines generative function, needed in A* path computation.
/// Operations need to be defined as strings. No error checking for duplicates.
/// </summary>
/// <typeparam name="TState">Class wrapper for data.</typeparam>
public interface Generative<TState>
{
    /// <summary>
    /// Function will generate a new graphState from current TState.
    /// </summary>
    /// <param name="operation">Operation to perform on current graphState.</param>
    /// <returns>New graphState, or null, if operation is invalid, or cannot be performed.</returns>
    TState generate(string operation);

    /// <summary>
    /// Function is used to provide list of possible operations.
    /// Needed for graphState generation.
    /// </summary>
    /// <returns>List of operations usable in function "generate".</returns>
    int[] getOperations();

    /// <summary>
    /// Function is used to generate hash from actual TState.
    /// Hash needs to be in uint. Beware using GetHashCode as it usually returns only reference to class.
    /// </summary>
    /// <returns>hash graphState of TState</returns>
    uint getHash();
}
#endregion

/// <summary>
/// Class is used for A* computation.
/// </summary>
/// <typeparam name="TState">Parameter is the graphState type.
/// This type need to define heuristic function used in A* algorithm.
/// Need to define generative options for generating new states, and operations list.
/// Need to define (override) Equals function that is used in graphState checking.
/// Need to define (override) GetHashCode function needed in graphState checking.</typeparam>
public class AStar<TState>
    where TState : Heuristical<TState>, Generative<TState>
{

    #region working classes definition
    /// <summary>
    /// Class, where resulting path is stored.
    /// Path of states is stored in "foundPathStates" array from 0 to pathLength-1
    /// Steps are stored in string array "foundPathOperations" where at 0 index is null, cause we really dont know,
    /// what kind of trick or operation user made to get to this state. As of others indexes,
    /// there is operation that was used to get to state on same index of TState. For example at index 1 is operation that
    /// was used to get to TState at index 1 from TState at index 0.
    /// </summary>
    public class PathResult
    {
        public TState[] foundPathStates; //states
        public int[] foundPathOperations; //operations
        public int pathLength;
        public int searchedNodes;
        public int generatedNodes;
        public int generatedUniqueNodes;
        public int maximumUsedHeapMemory;
        public int maximumUsedHashMemory;
        public int heuristicParamUsed;
        public TimeSpan totalTimeTaken;

        public PathResult(TState[] foundPath = null, string[] nextStateStep = null, int pathLength = 0, int searchedNodes = 0, int generatedNodes = 0, int generatedUniqueNodes = 0, int maximumUsedHeapMemory = 0, int maximumUsedHashMemory = 0)
        {
            this.foundPathStates = foundPath;
            this.foundPathOperations = nextStateStep;
            this.pathLength = pathLength;
            this.searchedNodes = searchedNodes;
            this.generatedNodes = generatedNodes;
            this.generatedUniqueNodes = generatedUniqueNodes;
            this.maximumUsedHeapMemory = maximumUsedHeapMemory;
            this.maximumUsedHashMemory = maximumUsedHashMemory;
        }
    }

    /// <summary>
    /// Class used for generated states/nodes in graph, where pathfinding is used.
    /// In this graphState is stored current graphState, parent in graph, lastMove/operation performed and depth in graph.
    /// </summary>
    private class GraphState
    {
        public readonly TState actualState;
        public GraphState graphParent;
        public int lastMove;
        public int graphDepth;
        public int distanceFromNeighbour;
        public int distanceToEnd;

        public GraphState(TState state, GraphState graphParent, int graphDepth, int lastMove, int distanceFromNeighbour, int distanceToEnd)
        {
            if (state == null) throw new System.Exception("Cannot create GraphState with empty TState!");
            this.actualState = state;
            this.graphParent = graphParent;
            this.graphDepth = graphDepth;
            this.lastMove = lastMove;
            this.distanceFromNeighbour = distanceFromNeighbour;
            this.distanceToEnd = distanceToEnd;
        }
    }


    /// <summary>
    /// Class used for hash informations. This is one graph node.
    /// Whole hash table is generated by memory need.
    /// In hash is stored current graphState and also next hashdata for chaining.
    /// </summary>
    private class HashData
    {
        public GraphState graphState;
        public HashData next;

        public HashData(GraphState state, HashData next)
        {
            this.graphState = state;
            this.next = next;
        }
    }

    /// <summary>
    /// Class used as wrapper for key-graphState pairs in heap structure
    /// For A* key should be F-cost (or distance from start + distance to end)
    /// </summary>
    private class KeyValuePair
    {
        public int key;
        public GraphState graphState;

        public KeyValuePair(int key, GraphState value)
        {
            this.key = key;
            this.graphState = value;
        }
    }

    /// <summary>
    /// Class used for manipulating hash used in A*.
    /// In hash there are stored already visited states (nodes).
    /// </summary>
    private class HashList
    {
        public const int DEFAULT_HASHTABLE_SIZE = 65536;
        public const int DEFAULT_MAX_ELEMENTS = 65536;

        private HashData[] hashTable; //hashes for puzzle states
        private int hashTableSize; //size of hashTable array to create
        private int maxElementsCount; //hash size in number of elements
        public int count; //actual number of elements in hash

        /// <summary>
        /// Constructor will create hash table.
        /// </summary>
        /// <param name="hashTableSize">Size of hashTable to use for storing Hashed elements.</param>
        /// <param name="maxElementsCount">Maximum number of elements in hash table.</param>
        public HashList(int hashTableSize = DEFAULT_HASHTABLE_SIZE, int maxElementsCount = DEFAULT_MAX_ELEMENTS)
        {
            this.maxElementsCount = maxElementsCount;
            this.hashTable = new HashData[this.maxElementsCount];
            if (this.hashTable == null) throw new System.Exception("hashTable not initialized!\n");
            this.count = 0;
        }

        /// <summary>
        /// Generate Hash for graphState.
        /// Hash is generated only from TState hash function.
        /// Other fields are not added to hash function for manipulating.
        /// Hash is used in this case as fast-searching function.
        /// </summary>
        /// <param name="state">GraphState for hash</param>
        /// <returns>Returns hash for graphState</returns>
        public uint getHash(GraphState state)
        {
            if (state == null) throw new System.Exception("null state not accepted for Hash.\n");
            /*uint hash = 0;
            uint seed = 101;
            int size = string.Length;
            for (int i = 0; i < size; i++) hash = hash * seed + string[i];*/
            return state.actualState.getHash() % (uint)this.maxElementsCount;
        }

        /// <summary>
        /// Function will add state to hash
        /// </summary>
        /// <param name="state">GraphState to add to hash table</param>
        public void add(GraphState state)
        {
            HashData temp; //used for chain searching
            uint hash; //hash of state

            if (state == null) throw new System.Exception("data field cannot be null.\n");

            //if already in hash, then do not add new
            if (this.isInHash(state)) return;

            hash = this.getHash(state);
            if (this.hashTable[hash] == null)
            {
                //if no element exist, add new element to hashTable
                this.hashTable[hash] = new HashData(state, null);
                if (this.hashTable[hash] == null) throw new System.Exception("Not enough space to create new hashTable element!\n");
                this.count++;
                return;
            }
            else
            {
                //if element already exist, search end of chain and add new element
                temp = this.hashTable[hash];
                while (temp.next != null) temp = temp.next; //search for the end of chain

                temp.next = new HashData(state, null); //add new element to hash
                if (temp.next == null) throw new System.Exception("Not enough space to create new hashTable element!\n");
                this.count++;
                return;
            }
        }

        /// <summary>
        /// Function will return true if state exist in hashTable, otherwise false.
        /// </summary>
        /// <param name="state">GraphState to check if it is hash table</param>
        /// <returns>true, if graphState is in hash table, otherwise false</returns>
        public bool isInHash(GraphState state)
        {
            HashData temp;
            uint hash;
            //if (this.hashTable == null) throw new System.Exception("hashTable not initialized!\n");
            if (state == null) throw new System.Exception("currentState field cannot be null.\n");

            hash = this.getHash(state);
            if (this.hashTable[hash] == null) return false;
            temp = this.hashTable[hash];
            while (temp.next != null)
            {
                if (temp.graphState.actualState.getHash() == state.actualState.getHash()) return true;
                temp = temp.next;
            }
            if (temp.graphState.actualState.getHash() == state.actualState.getHash()) return true;
            return false;
        }

        /// <summary>
        /// This will return HashData from hash for editing GraphState.
        /// </summary>
        /// <param name="state">GraphState to find</param>
        /// <returns>saved hashed GraphState</returns>
        public HashData getState(GraphState state)
        {
            HashData temp;
            uint hash;
            //if (this.hashTable == null) throw new System.Exception("hashTable not initialized!\n");
            if (state == null) throw new System.Exception("currentState field cannot be null.\n");

            hash = getHash(state);
            if (this.hashTable[hash] == null) return null;
            temp = this.hashTable[hash];
            while (temp.next != null)
            {
                if (temp.graphState.actualState.getHash() == state.actualState.getHash()) return temp;
                temp = temp.next;
            }
            if (temp.graphState.actualState.getHash() == state.actualState.getHash()) return temp;
            return null;
        }

        /// <summary>
        /// Function will remove hash node from list
        /// </summary>
        /// <param name="currentState">graphState to remove</param>
        public void removeState(GraphState currentState)
        {
            HashData temp, previous;
            uint hash;
            if (this.hashTable == null) throw new System.Exception("hashTable not initialized!\n");
            if (currentState == null) throw new System.Exception("currentState field cannot be null.\n");

            hash = getHash(currentState);
            if (this.hashTable[hash] == null) return;
            previous = this.hashTable[hash]; ;
            temp = this.hashTable[hash];

            //1. find hash element
            while (temp.next != null)
            {
                //if found, remove element
                if (temp.graphState.actualState.getHash() == currentState.actualState.getHash())
                {
                    if (temp == this.hashTable[hash])
                        this.hashTable[hash] = temp.next;
                    else previous.next = temp.next;
                    temp = null;
                    this.count--;
                    return;
                }
                previous = temp;
                temp = temp.next;
            }

            //2. check for last element
            if (temp.graphState.actualState.getHash() == currentState.actualState.getHash())
            {
                if (temp == this.hashTable[hash])
                    this.hashTable[hash] = temp.next;
                else previous.next = temp.next;
                previous.next = temp.next;
                temp = null;
                this.count--;
                return;
            }
        }
    }

    /// <summary>
    /// Heap List with basic fuunctions - min, max, removeMin,...
    /// You can specify size of heap when constructing. Default is 65536.
    /// </summary>
    private class HeapList
    {
        public const int DEFAULT_MAX_ELEMENTS = 65536;

        private int maxHeapElements;
        public int maxNodeDepth;
        public int count;
        private KeyValuePair[] root;

        /// <summary>
        /// Constructor will create heap array. You can specify number of elements in heap. Default is 65536.
        /// Size doesnt change.
        /// </summary>
        /// <param name="maxHeapElements">Number of possible elements in heap.</param>
        public HeapList(int maxHeapElements = DEFAULT_MAX_ELEMENTS)
        {
            this.maxHeapElements = maxHeapElements;
            this.root = new KeyValuePair[maxHeapElements + 1];
            this.count = 0;
            this.maxNodeDepth = 0;
        }

        /// <summary>
        /// This will add node to heap based on key
        /// </summary>
        /// <param name="node"></param>
        public void add(KeyValuePair node)
        {
            int iTemp;
            KeyValuePair kTemp;
            if (this.maxHeapElements <= this.count) throw new Exception("Not enough space to add new element to heap. heapSize: " + this.maxHeapElements);
            this.count++;
            this.root[count] = node;
            iTemp = this.count;
            while (iTemp > 1 && (this.root[iTemp].key < this.root[iTemp / 2].key))
            {
                kTemp = this.root[iTemp / 2];
                this.root[iTemp / 2] = this.root[iTemp];
                this.root[iTemp] = kTemp;
                iTemp /= 2;
            }
        }

        /// <summary>
        /// Function will return minimum node from heap, without deleting
        /// </summary>
        /// <returns>Minimum (root) node from heap</returns>
        public KeyValuePair getMin()
        {
            return this.root[1];
        }

        /// <summary>
        /// Function will return maximum node from heap, without deleting
        /// </summary>
        /// <returns>Maximum (last) node from heap</returns>
        public KeyValuePair getMax()
        {
            return this.root[count];
        }

        /// <summary>
        /// Function will return (pop) minimum node from heap, with deleting
        /// </summary>
        /// <returns>Minimum (root) node from heap</returns>
        public KeyValuePair removeMin()
        {
            int iTemp;
            KeyValuePair kTemp;
            KeyValuePair result;
            result = this.root[1];
            this.root[1] = this.root[count];
            this.root[this.count] = null;
            this.count--;
            iTemp = 1;
            while (iTemp * 2 <= this.count)
            {
                //1. check for last elements
                if (iTemp * 2 == this.count)
                {
                    if (this.root[iTemp * 2].key < this.root[iTemp].key)
                    {
                        kTemp = this.root[iTemp * 2];
                        this.root[iTemp * 2] = this.root[iTemp];
                        this.root[iTemp] = kTemp;
                    }
                    break;
                }

                //2.check which element is smaller
                if (this.root[iTemp * 2].key <= this.root[iTemp * 2 + 1].key)
                {
                    if (this.root[iTemp].key > this.root[iTemp * 2].key)
                    {
                        kTemp = this.root[iTemp * 2];
                        this.root[iTemp * 2] = this.root[iTemp];
                        this.root[iTemp] = kTemp;
                        iTemp = iTemp * 2;
                        continue;
                    }
                    else break;
                }
                else
                {
                    if (this.root[iTemp].key > this.root[iTemp * 2 + 1].key)
                    {
                        kTemp = this.root[iTemp * 2 + 1];
                        this.root[iTemp * 2 + 1] = this.root[iTemp];
                        this.root[iTemp] = kTemp;
                        iTemp = iTemp * 2 + 1;
                        continue;
                    }
                    else break;
                }
            }

            return result;
        }
    }
    #endregion

    #region AStar private fields definitions
    private int heapSize; //size of heap to use
    private int hashSize; //size of hash to use
    private string[] operationsList; //list of possible operations to perform with states

    private int maxDepth; //max depth to where to find solutions
    private int maxTime; //maximum time of searching
    private string heuristicParam; //parameter for heuristic function. Can be changed.
    private DateTime startTime; //used for time tracking

    /// <summary>
    /// Change this to increase/decrease maxDepth of searching.
    /// Use -1 for infinite.
    /// </summary>
    public int MaxDepth
    {
        get { return maxDepth; }
        set { maxDepth = value; }
    }

    public int HashSize
    {
        get { return hashSize; }
        set { hashSize = value; }
    }

    public int HeapSize
    {
        get { return heapSize; }
        set { heapSize = value; }
    }

    /// <summary>
    /// Change this to increase/decrease maximum Time spent in searching.
    /// Time is in milliseconds. Use -1 for infinite.
    /// </summary>
    public int MaxTime
    {
        get { return maxTime; }
        set { maxTime = value; }
    }

    /// <summary>
    /// Parameter passed to heuristic function. Optional.
    /// </summary>
    public string HeuristicParam
    {
        get { return heuristicParam; }
        set { heuristicParam = value; }
    }
    #endregion



    #region AStar functionality

    /// <summary>
    /// Constructor for AStar class algorithm. Here you can specifi heap and hash size, also max depth to which algorithm will be searching nodes.
    /// Max depth is not cost, or distance to start/finish, but rather depth of elements. For example if depth is 2 and each element have 2 childs after generating,
    /// then algorithm will stop at 3-rd generation. 1.st you have deth 0, then have 2 that have depth 1, 4 with depth 2. And these 4 will be last nodes checked.
    /// </summary>
    /// <param name="generator">This should be valid TState that will be used only once, for generating operations.</param>
    /// <param name="heapSize">Number of elements in heap. This number is leading as to how much memory algorithm will be using.</param>
    /// <param name="hashSize">Size of hash table. This does not determine upper limit, cause hash table can chain elements.</param>
    /// <param name="maxDepth">Maximum depth to where to generate child nodes (states). Depth is as follows: Child -> child -> child -> root - 3 -> 2 -> 1 -> 0. Use -1 for infinite.</param>
    /// <param name="maxTime">Maximum time in miliseconds for search performing. Use -1 for infinite.</param>
    public AStar(TState generator, int heapSize = 65536, int hashSize = 65536, int maxDepth = -1, int maxTime = -1, string heuristicParam = null)
    {
        this.heapSize = heapSize;
        this.hashSize = hashSize;
        this.maxDepth = maxDepth;
        this.maxTime = maxTime;
        this.HeuristicParam = heuristicParam;
        this.operationsList = generator.getOperations();
    }

    /// <summary>
    /// Function will find path (if exist) and return structured PathResult
    /// </summary>
    /// <param name="startingPuzzleState">Starting Puzzle graphState</param>
    /// <param name="finishingPuzzleState">Finishing puzzle graphState</param>
    /// <returns>Returns structured pathResult, if path does not exist, return pathResult, but path will be null</returns>
    public PathResult getAStarPath(TState startingPuzzleState, TState finishingPuzzleState)
    {
        HeapList openNodes; //list of open nodes - priority front
        HashList closedNodes; //list of generated nodes
        GraphState currentGraphState; //current graphState in graph
        GraphState[] generatedDirectionGraphStates = new GraphState[this.operationsList.Length]; //generated nodes
        HashData tempHashNode;
        PathResult pathResult;
        TState tempTState;
        int tempHeuristicResult;

        //initialization
        pathResult = new PathResult();
        startTime = DateTime.UtcNow; //start time for algorithm

        //create Hash table
        closedNodes = new HashList(this.hashSize);

        //create default heap list
        openNodes = new HeapList(this.heapSize);

        //1. add first element
        tempHeuristicResult = startingPuzzleState.getHeuristicDistance(finishingPuzzleState, this.heuristicParam);
        openNodes.add(new KeyValuePair(tempHeuristicResult, new GraphState(startingPuzzleState, null, 0, -1, 0, tempHeuristicResult)));
        closedNodes.add(openNodes.getMin().graphState);
        pathResult.generatedNodes++;
        pathResult.maximumUsedHeapMemory++;

        //2. check if graph is not empty
        while (openNodes.count != 0)
        {
            //2.1 special - add maximum used heap and hash size
            if (pathResult.maximumUsedHeapMemory < openNodes.count) pathResult.maximumUsedHeapMemory = openNodes.count;
            if (pathResult.maximumUsedHashMemory < closedNodes.count) pathResult.maximumUsedHashMemory = closedNodes.count;

            //3. select best not-processed graphState
            currentGraphState = openNodes.removeMin().graphState;
            //3.1 now check if node was actualized in hash. If yes, use node from hash.
            tempHashNode = closedNodes.getState(currentGraphState);
            if (tempHashNode != null)
                if (tempHashNode.graphState.distanceFromNeighbour + tempHashNode.graphState.distanceToEnd < currentGraphState.distanceFromNeighbour + currentGraphState.distanceToEnd)
                    currentGraphState = tempHashNode.graphState;

            pathResult.searchedNodes++;
            //Debug.Log("Graph Depth: " + currentGraphState.graphDepth + "\n");

            //4. test if graphState is finish graphState, or depth is maxDepth or bigger. For -1 maxDepth just ignore depth.
            //also check for elapsed time in miliseconds. For -1 maxtime, just ignore time.
            //If some of these apply, finish searching and return found path.
            if (currentGraphState.actualState.isEqual(finishingPuzzleState) ||
                ((this.maxDepth != -1) && (currentGraphState.graphDepth > this.maxDepth)) ||
                ((this.maxTime != -1) && ((DateTime.UtcNow).Subtract(startTime).TotalMilliseconds > this.maxTime)))
            {
                string[] foundOperationsPath = new string[currentGraphState.graphDepth + 1];
                TState[] foundStatesPath = new TState[currentGraphState.graphDepth + 1];
                for (int i = currentGraphState.graphDepth; i > 0; i--)
                {
                    foundStatesPath[i] = currentGraphState.actualState;
                    foundOperationsPath[i] = this.operationsList[currentGraphState.lastMove];
                    currentGraphState = currentGraphState.graphParent;
                }
                //first state was starting and we dont know which move user used to create such a state. So we are using null.
                //Or we can try to think a little and create some super Intelligent Intelligence that can guess with 100% precision
                //which move author used to create such a state in which this state now is. Maybe somewhere in future...
                foundStatesPath[0] = currentGraphState.actualState;
                foundOperationsPath[0] = null;

                pathResult.foundPathStates = foundStatesPath;
                pathResult.pathLength = foundOperationsPath.Length;
                pathResult.foundPathOperations = foundOperationsPath;
                pathResult.heuristicParamUsed = this.heuristicParam;
                pathResult.totalTimeTaken = (DateTime.UtcNow).Subtract(startTime);
                closedNodes = null;
                openNodes = null;
                //yes, now we should return it and... maybe go to sleep (bed)? :)
                return pathResult;
            }

            //5. add current node to hash
            closedNodes.add(currentGraphState);

            //6. create childs (neighbours) of current node
            for (int i = (generatedDirectionGraphStates.Length - 1); i >= 0; i--)
            {
                tempTState = currentGraphState.actualState.generate(this.operationsList[i]);
                if (tempTState == null) { generatedDirectionGraphStates[i] = null; continue; }

                generatedDirectionGraphStates[i] =
                    new GraphState(tempTState, currentGraphState, currentGraphState.graphDepth + 1, i,
                    currentGraphState.distanceFromNeighbour + tempTState.getHeuristicDistance(currentGraphState.actualState, this.heuristicParam),
                    tempTState.getHeuristicDistance(finishingPuzzleState, this.heuristicParam));
            }

            //7. check every child(neighbour) node and add to heap or hash and actualize hash if node score is better
            for (int i = generatedDirectionGraphStates.Length - 1; i >= 0; i--)
            {
                //check only not-null states
                if (generatedDirectionGraphStates[i] != null)
                {
                    pathResult.generatedNodes++; //increase generated nodes counter
                    tempHashNode = closedNodes.getState(generatedDirectionGraphStates[i]);
                    if (tempHashNode != null)
                    {
                        //if node is already in hash, check depth and update
                        if (generatedDirectionGraphStates[i].distanceFromNeighbour + generatedDirectionGraphStates[i].distanceToEnd <
                            tempHashNode.graphState.distanceFromNeighbour + tempHashNode.graphState.distanceToEnd)
                        {
                            //now we update node in hash, but not in heap. in heap it will be updated in getting min element
                            openNodes.add(new KeyValuePair(generatedDirectionGraphStates[i].distanceFromNeighbour + generatedDirectionGraphStates[i].distanceToEnd, generatedDirectionGraphStates[i]));
                            closedNodes.removeState(generatedDirectionGraphStates[i]);
                            closedNodes.add(generatedDirectionGraphStates[i]);
                        }
                    }
                    else
                    {
                        openNodes.add(new KeyValuePair(generatedDirectionGraphStates[i].distanceFromNeighbour + generatedDirectionGraphStates[i].distanceToEnd, generatedDirectionGraphStates[i]));
                        closedNodes.add(generatedDirectionGraphStates[i]);
                        pathResult.generatedUniqueNodes++;
                    }
                }
            }
        }
        //dereference memory
        closedNodes = null;
        openNodes = null;
        return pathResult;
    }
    #endregion
}

#endregion