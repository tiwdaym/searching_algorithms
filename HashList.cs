using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class HashList<TValue> : ISimpleCollection<TValue>
        where TValue : IEquatable<TValue>, IHashable
    {
        public const int DEFAULT_HASHTABLE_SIZE = 65536;
        public const int DEFAULT_MAX_ELEMENTS = 65536;

        private SingleLinkedList<TValue>[] hashTable; //hashes for puzzle states
        private int hashTableSize; //size of hashTable array to create
        private int maxElementsCount; //hash size in number of elements
        private int count; //actual number of elements in hash

        public int Count { get => count; }

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
        public uint GetHash(TValue value)
        {
            if (value == null) throw new ArgumentNullException("null value is not accepted for hash.");
            /*uint hash = 0;
            uint seed = 101;
            int size = string.Length;
            for (int i = 0; i < size; i++) hash = hash * seed + string[i];*/
            return value.GetHash() % (uint)this.maxElementsCount;
        }

        /// <summary>
        /// Function will add state to hash
        /// </summary>
        /// <param name="state">GraphState to add to hash table</param>
        public void Add(TValue item)
        {
            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");

            //1. Get hash of item
            uint hash = GetHash(item);

            //if item is already in hash, then do not add new
            if (hashTable[hash].Contains(item)) return;

            //2. Add the item to list
            hashTable[hash].Add(item);
            this.count++;
        }

        /// <summary>
        /// Function will return true if state exist in hashTable, otherwise false.
        /// </summary>
        /// <param name="state">GraphState to check if it is hash table</param>
        /// <returns>true, if graphState is in hash table, otherwise false</returns>
        public bool Contains(TValue item)
        {
            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");

            return (hashTable[GetHash(item)].Contains(item));
        }

        /// <summary>
        /// Function will remove hash node from list
        /// </summary>
        /// <param name="currentState">graphState to remove</param>
        public bool Remove(TValue item)
        {

            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");

            //1. Get hash of item
            uint hash = GetHash(item);

            //2. Add the item to list
            bool removed = hashTable[hash].Remove(item);
            if (removed) this.count--;

            return removed;
        }

        public TValue[] ToList(uint arrayIndex = 0)
        {
            throw new NotImplementedException();
        }
    }
}
