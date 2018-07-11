using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SearchingAlgorithms.Collections;

namespace SearchingAlgorithms
{
    class EvolutionSearch<T>
        where T : IEquatable<T>, IHashable, IGenerative<T>, IHeuristical<T>
    {
        DNA[] population;
        uint populationCount;
        uint dnaLength;
        uint generationsCount;
        HeapMaxList<GraphNodeSimple<DNA>> sorter;

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


        uint maxSearchingTime = 0;
        /// <summary>
        /// You can set maximum Finding Time in miliseconds.
        /// After reaching the time, partial shortest path is returned.
        /// Usable while computing.
        /// Use 0 for unlimited time.
        /// </summary>
        public uint MaxSearchingTime { get => maxSearchingTime; set => maxSearchingTime = value; }


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

        public uint PopulationCount
        {
            get => populationCount;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Population Count while in processing.");
                populationCount = value;
            }
        }
        public uint DnaLength
        {
            get => dnaLength;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Dna Length while in processing.");
                dnaLength = value;
            }
        }
        public uint GenerationsCount
        {
            get => generationsCount;
            set
            {
                if (isProcessingChangesDisabled) throw new InvalidOperationException("Cannot change Generations Count while in processing.");
                generationsCount = value;
            }
        }

        public EvolutionSearch(T startState,T finishState, uint maxSearchingTime = 0, uint maxGeneratedElementsCount = 65536, int heuristicParam = 0,
            uint populationCount = 1000, uint dnaLength = 500, uint generationsCount = 1000, uint crossoverPointsCount = 1, uint smallestCrossoverChromosomeUnit = 1, uint breedingStrategy = 0,
            double amountOfGenerationToSurvive = 0.50, double amountOfNewRandomIndividuals = 0.1, double mutationChance = 0.02)
        {
            pathResult = new GeneratedPath<T>();
            MaxSearchingTime = maxSearchingTime;
            StartState = startState;
            FinishState = finishState;
            PopulationCount = populationCount;
            GenerationsCount = generationsCount;
            DnaLength = dnaLength;
            HeuristicParam = heuristicParam;
        }

        /// <summary>
        /// Function call will find Path by default set parameters
        /// </summary>
        /// <returns>Result is also stored in pathResult</returns>
        /*public GeneratedPath<T> findPath()
        {
            //initialization
            isProcessingChangesDisabled = true;
            startTime = DateTime.UtcNow;

            pathResult = new GeneratedPath<T>();

            //01. Initialize Random population and compute fitness (sorted from min to max in array)
            InitializeRandomPopulation();
            PopulationFitness();

            uint currentGeneration = 0;

            while (currentGeneration < generationsCount)
            {




                PopulationFitness();
            }




            //1. add first element
            openSet.Add(new GraphNodeComplex<T>(startState, null, null, 0, startState.HeuristicDistance(finishState, heuristicParam)));
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
                //3.1 check if node is in closed set and have better comparing distance
                if (closedSet.TryGetValue(currentGraphNode, out tmpGraphNode) && tmpGraphNode.comparingParam < currentGraphNode.comparingParam) currentGraphNode = tmpGraphNode;

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

                //6. create childs (neighbours) of current node and add them to open and closed set for checking
                for (int i = (operationsList.Length - 1); i >= 0; i--)
                {
                    tempTState = currentGraphNode.node.GenerateNewState(operationsList[i]);
                    pathResult.generatedNodes++;
                    if (tempTState == null) continue;
                    tmpGraphNode = new GraphNodeComplex<T>(tempTState, currentGraphNode, operationsList[i], currentGraphNode.realGraphDepth + 1, currentGraphNode.realGraphDepth + 1 + tempTState.HeuristicDistance(finishState, heuristicParam));
                    if (closedSet.Contains(tmpGraphNode))
                    {
                        if (closedSet.TryGetValue(tmpGraphNode, out tmpGraphNode2) && tmpGraphNode.comparingParam < tmpGraphNode2.comparingParam) closedSet.Remove(tmpGraphNode2);
                        else continue;
                    };

                    openSet.Add(tmpGraphNode);
                    closedSet.Add(tmpGraphNode);
                }

            }

            ResetProcessing();

            return null;
        }*/


        void InitializeRandomPopulation()
        {
            this.population = new DNA[this.populationCount];
            int operationsCount = this.startState.OperationsList().Length;
            for (uint i = 0; i < this.populationCount; i++) this.population[i] = DNA.Random(this.dnaLength, operationsCount, 0);
        }



        void PopulationFitness()
        {
            T tempTState = startState;

            sorter = new HeapMaxList<GraphNodeSimple<DNA>>(populationCount);

            DNA currentDna;

            for (int i = 0; i < populationCount; i++)
            {
                currentDna = population[i];
                sorter.Add(new GraphNodeSimple<DNA>(currentDna, null, null, DnaFitness(currentDna)));
            }

            for (int i = 0; i < populationCount; i++)
            {
                population[i] = sorter.GetMax().node;
                pathResult.searchedNodes += (ulong)sorter.RemoveMax().graphDepth;
            }
        }

        int DnaFitness(DNA dna)
        {
            int dnaLength = dna.chromosome.Length;
            string[] operationsList = startState.OperationsList();
            int operationsCount = operationsList.Length;
            uint nextOperation = 0;
            int distance = 0;

            T currentState = startState;
            T tmpState;

            while (distance < dnaLength)
            {
                if (currentState.Equals(finishState)) break;
                nextOperation = dna.chromosome[distance];
                if (nextOperation < operationsCount)
                {
                    tmpState = currentState.GenerateNewState(operationsList[nextOperation]);
                    if (tmpState != null) currentState = tmpState;
                }
                distance++;
            }
            
            return dnaLength - distance + startState.HeuristicDistance(finishState, heuristicParam) - currentState.HeuristicDistance(finishState, heuristicParam);
        }

        /*DNA[] BreedChildrens(DNA parent1, DNA parent2)
        {
            DNA[] childrens = new DNA[2];

        }*/


        /// <summary>
        /// This should be called to clean hash and heap lists and reset processing stage
        /// </summary>
        public void ResetProcessing()
        {
            sorter = null;
            population = null;
            isProcessingChangesDisabled = false;
        }
    }
}
