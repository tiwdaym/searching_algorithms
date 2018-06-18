using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    class GeneratedPath<T>
    {
        public T[] pathStates; //states
        public string[] pathOperations; //operations
        public uint pathLength;
        public ulong searchedNodes;
        public ulong generatedNodes;
        public ulong generatedUniqueNodes;
        public uint maximumUsedHeapMemory;
        public uint maximumUsedHashMemory;
        public uint maximumUsedStackMemory;
        public int heuristicParamUsed;
        public TimeSpan totalTimeTaken;

        public GeneratedPath(T[] pathStates = null, string[] pathOperations = null, uint pathLength = 0, ulong searchedNodes = 0, ulong generatedNodes = 0, ulong generatedUniqueNodes = 0, uint maximumUsedHeapMemory = 0, uint maximumUsedHashMemory = 0, uint maximumUsedStackMemory = 0)
        {
            this.pathStates = pathStates;
            this.pathOperations = pathOperations;
            this.pathLength = pathLength;
            this.searchedNodes = searchedNodes;
            this.generatedNodes = generatedNodes;
            this.generatedUniqueNodes = generatedUniqueNodes;
            this.maximumUsedHeapMemory = maximumUsedHeapMemory;
            this.maximumUsedHashMemory = maximumUsedHashMemory;
            this.maximumUsedStackMemory = maximumUsedStackMemory;
        }
    }
}
