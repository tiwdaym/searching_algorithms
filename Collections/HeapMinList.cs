using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    public class HeapMinList<T>
        where T : IComparable<T>, IEquatable<T>
    {
        public const uint DEFAULT_MAX_ELEMENTS = 65536;
        private const uint DATA_OFFSET = 1;

        private uint maxElementsCount;
        private uint count;
        private T[] heapTable;

        public uint Count { get => count; }

        /// <summary>
        /// Constructor will create heap array. You can specify number of elements for heap. Default is 65536.
        /// Size cannot change.
        /// </summary>
        /// <param name="maxHeapElements">Number of possible elements in heap.</param>
        public HeapMinList(uint maxElementsCount = DEFAULT_MAX_ELEMENTS)
        {
            this.maxElementsCount = maxElementsCount;
            this.heapTable = new T[maxElementsCount + DATA_OFFSET];
            if (this.heapTable == null) throw new OutOfMemoryException("Cannot initialize heap table.");
            this.count = 0;
        }

        /// <summary>
        /// This will add new item to heap
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (this.maxElementsCount <= this.count) throw new OutOfMemoryException("Not enough space to add new element to heap. heapSize: " + this.maxElementsCount);
            if (item == null) throw new ArgumentNullException("Item cannot be null.");

            this.count++;
            this.heapTable[count] = item;

            uint iTemp = this.count;

            while (iTemp > 1 && (this.heapTable[iTemp].CompareTo(this.heapTable[iTemp / 2]) < 0))
            {
                T kTemp = this.heapTable[iTemp / 2];
                this.heapTable[iTemp / 2] = this.heapTable[iTemp];
                this.heapTable[iTemp] = kTemp;
                iTemp /= 2;
            }
        }

        /// <summary>
        /// Function will return minimum node from heap, without deleting
        /// </summary>
        /// <returns>Minimum (root) node from heap</returns>
        public T GetMin()
        {
            return this.heapTable[1];
        }

        /// <summary>
        /// Function will return (pop) minimum node from heap, with deleting
        /// </summary>
        /// <returns>Minimum (root) node from heap</returns>
        public T RemoveMin()
        {
            int iTemp;
            T kTemp;
            T result = this.heapTable[1];

            this.heapTable[1] = this.heapTable[Count];
            this.heapTable[this.Count] = default(T);
            if (this.count == 0) return default(T);
            this.count--;
            iTemp = 1;

            while (iTemp * 2 <= this.count)
            {
                //1. check for last elements
                if (iTemp * 2 == this.count)
                {
                    if (this.heapTable[iTemp * 2].CompareTo(this.heapTable[iTemp]) < 0)
                    {
                        kTemp = this.heapTable[iTemp * 2];
                        this.heapTable[iTemp * 2] = this.heapTable[iTemp];
                        this.heapTable[iTemp] = kTemp;
                    }
                    break;
                }

                //2.check which element is smaller
                if (this.heapTable[iTemp * 2].CompareTo(this.heapTable[iTemp * 2 + 1]) <= 0)
                {
                    if (this.heapTable[iTemp].CompareTo(this.heapTable[iTemp * 2]) > 0)
                    {
                        kTemp = this.heapTable[iTemp * 2];
                        this.heapTable[iTemp * 2] = this.heapTable[iTemp];
                        this.heapTable[iTemp] = kTemp;
                        iTemp = iTemp * 2;
                        continue;
                    }
                    else break;
                }
                else
                {
                    if (this.heapTable[iTemp].CompareTo(this.heapTable[iTemp * 2 + 1]) > 0)
                    {
                        kTemp = this.heapTable[iTemp * 2 + 1];
                        this.heapTable[iTemp * 2 + 1] = this.heapTable[iTemp];
                        this.heapTable[iTemp] = kTemp;
                        iTemp = iTemp * 2 + 1;
                        continue;
                    }
                    else break;
                }
            }

            return result;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++) if (heapTable[i].Equals(item)) return true;
            return false;
        }
    }
}
