using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms.Collections
{
    public class StackList<T>
    {
        public const uint DEFAULT_MAX_ELEMENTS = 65536;

        private uint maxElementsCount;
        private uint count;
        private T[] stackList;

        public uint Count { get => count; }

        /// <summary>
        /// Constructor will create stack array. You can specify number of elements for stack. Default is 65536.
        /// Size cannot change.
        /// </summary>
        /// <param name="maxHeapElements">Number of possible elements in stack.</param>
        public StackList(uint maxElementsCount = DEFAULT_MAX_ELEMENTS)
        {
            this.maxElementsCount = maxElementsCount;
            stackList = new T[maxElementsCount];
            if (stackList == null) throw new OutOfMemoryException("Cannot initialize heap table.");
            count = 0;
        }

        /// <summary>
        /// This will add new item to stack
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            if (maxElementsCount <= count) throw new OutOfMemoryException("Not enough space to add new element to heap. heapSize: " + maxElementsCount);
            if (item == null) throw new ArgumentNullException("Item cannot be null.");
            
            stackList[count] = item;
            count++;
        }

        /// <summary>
        /// This will return and remove last item from Stack
        /// </summary>
        public T Pop()
        {
            if (count == 0) return default(T);
            return stackList[--count];
        }

        /// <summary>
        /// This will only return last item from stack without removing it
        /// </summary>
        public T Peak()
        {
            if (count == 0) return default(T);
            return stackList[count];
        }
    }
}
