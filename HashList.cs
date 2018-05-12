using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class HashList<TValue>
        where TValue:IEquatable<TValue>
    {
        public const int DEFAULT_HASHTABLE_SIZE = 65536;
        public const int DEFAULT_MAX_ELEMENTS = 65536;

        private SingleLinkedList<TValue>[] hashTable; //hashes for puzzle states
        private int hashTableSize; //size of hashTable array to create
        private int maxElementsCount; //hash size in number of elements
        private int count; //actual number of elements in hash

        public int Count { get => count; set => count = value; }

        /// <summary>
        /// Constructor will create hash table.
        /// </summary>
        /// <param name="hashTableSize">Size of hashTable to use for storing Hashed elements.</param>
        /// <param name="maxElementsCount">Maximum number of elements in hash table.</param>
        public HashList(int hashTableSize = DEFAULT_HASHTABLE_SIZE, int maxElementsCount = DEFAULT_MAX_ELEMENTS)
        {
            if (hashTableSize > maxElementsCount) throw new ArgumentOutOfRangeException("Hash table size cannot be larger than max elements count.");
            this.maxElementsCount = maxElementsCount;
            this.hashTableSize = hashTableSize;
            this.hashTable = new SingleLinkedList<TValue>[this.hashTableSize];
            if (this.hashTable == null) throw new OutOfMemoryException("HashTable not initialized.");
            this.Count = 0;
        }

        /// <summary>
        /// Generate Hash for graphState.
        /// Hash is generated only from TState hash function.
        /// Other fields are not added to hash function for manipulating.
        /// Hash is used in this case as fast-searching function.
        /// </summary>
        /// <param name="state">GraphState for hash</param>
        /// <returns>Returns hash for graphState</returns>
        public uint GetHashCode(TValue value)
        {
            if (value == null) throw new ArgumentNullException("null value is not accepted for hash.");
            /*uint hash = 0;
            uint seed = 101;
            int size = string.Length;
            for (int i = 0; i < size; i++) hash = hash * seed + string[i];*/
            return value.GetHashCode() % (uint)this.maxElementsCount;
        }

        /// <summary>
        /// Function will add state to hash
        /// </summary>
        /// <param name="state">GraphState to add to hash table</param>
        public void add(TValue value)
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
                this.Count++;
                return;
            }
            else
            {
                //if element already exist, search end of chain and add new element
                temp = this.hashTable[hash];
                while (temp.next != null) temp = temp.next; //search for the end of chain

                temp.next = new HashData(state, null); //add new element to hash
                if (temp.next == null) throw new System.Exception("Not enough space to create new hashTable element!\n");
                this.Count++;
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
                if (temp.graphState.actualState.isEqual(state.actualState)) return true;
                temp = temp.next;
            }
            if (temp.graphState.actualState.isEqual(state.actualState)) return true;
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
                if (temp.graphState.actualState.isEqual(state.actualState)) return temp;
                temp = temp.next;
            }
            if (temp.graphState.actualState.isEqual(state.actualState)) return temp;
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
                if (temp.graphState.actualState.isEqual(currentState.actualState))
                {
                    if (temp == this.hashTable[hash])
                        this.hashTable[hash] = temp.next;
                    else previous.next = temp.next;
                    temp = null;
                    this.Count--;
                    return;
                }
                previous = temp;
                temp = temp.next;
            }

            //2. check for last element
            if (temp.graphState.actualState.isEqual(currentState.actualState))
            {
                if (temp == this.hashTable[hash])
                    this.hashTable[hash] = temp.next;
                else previous.next = temp.next;
                previous.next = temp.next;
                temp = null;
                this.Count--;
                return;
            }
        }
    }
}
