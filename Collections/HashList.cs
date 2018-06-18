using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    class HashList<T> : ISimpleCollection<T>
        where T : IEquatable<T>, IHashable
    {
        public const uint DEFAULT_HASHTABLE_SIZE = 65536;
        public const uint DEFAULT_MAX_ELEMENTS = 65536;

        private SingleLinkedList<T>[] hashTable; //hashes for puzzle states
        private uint hashTableSize; //size of hashTable array to create
        private uint maxElementsCount; //hash size in number of elements
        private uint count; //actual number of elements in hash

        public uint Count { get => count; }

        /// <summary>
        /// Constructor will create hash table.
        /// </summary>
        /// <param name="hashTableSize">Size of hashTable to use for storing Hashed elements.</param>
        /// <param name="maxElementsCount">Maximum number of elements in hash table.</param>
        public HashList(uint hashTableSize = DEFAULT_HASHTABLE_SIZE, uint maxElementsCount = DEFAULT_MAX_ELEMENTS)
        {
            if (hashTableSize > maxElementsCount) throw new ArgumentOutOfRangeException("Hash table size cannot be larger than max elements count.");
            this.maxElementsCount = maxElementsCount;
            this.hashTableSize = hashTableSize;
            this.hashTable = new SingleLinkedList<T>[this.hashTableSize];
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
        public uint GetHash(T value)
        {
            if (value == null) throw new ArgumentNullException("null value is not accepted for hash.");
            /*uint hash = 0;
            uint seed = 101;
            int size = string.Length;
            for (int i = 0; i < size; i++) hash = hash * seed + string[i];*/
            return value.GetHash() % this.maxElementsCount;
        }

        /// <summary>
        /// Function will add state to hash
        /// </summary>
        /// <param name="state">GraphState to add to hash table</param>
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");
            if (count >= maxElementsCount) throw new OutOfMemoryException("Maximum elements count reached. Cannot add new element.");

            //1. Get hash of item
            uint hash = GetHash(item);

            //2. Create Linked list if no list existing with item as root
            if (hashTable[hash] == null)
            {
                hashTable[hash] = new SingleLinkedList<T>(item);
                this.count++;
                return;
            }

            //3. if item is already in hash, then do not add new
            if (hashTable[hash].Contains(item)) return;

            //4. Add item to linked list
            hashTable[hash].Add(item);
            this.count++;
        }

        /// <summary>
        /// Function will return true if state exist in hashTable, otherwise false.
        /// </summary>
        /// <param name="state">GraphState to check if it is hash table</param>
        /// <returns>true, if graphState is in hash table, otherwise false</returns>
        public bool Contains(T item)
        {
            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");
            uint hash = GetHash(item);
            if (hashTable[hash] == null) return false;
            return (hashTable[hash].Contains(item));
        }

        /// <summary>
        /// Function will remove hash node from list
        /// </summary>
        /// <param name="currentState">graphState to remove</param>
        public bool Remove(T item)
        {

            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");

            //1. Get hash of item
            uint hash = GetHash(item);

            if (hashTable[hash] == null) return false;

            //2. Add the item to list
            bool removed = hashTable[hash].Remove(item);
            if (removed) this.count--;

            return removed;
        }

        /// <summary>
        /// Function will return Value stored in Hash if there is the value.
        /// This can be useful when you want to reuse a previously stored reference instead of a newly constructed one (so that more sharing of references can occur)
        /// or to look up a value that has more complete data than the value you currently have, although their comparer functions indicate they are equal. 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="foundValue"></param>
        /// <returns></returns>
        public bool TryGetValue(T item, out T foundValue)
        {
            if (item == null) throw new ArgumentNullException("item field cannot be null.\n");
            foundValue = default(T);
            uint hash = GetHash(item);
            if (hashTable[hash] == null) return false;
            return hashTable[hash].TryGetValue(item, out foundValue);
        }

        public T[] ToList()
        {
            throw new NotImplementedException();
        }
    }
}
