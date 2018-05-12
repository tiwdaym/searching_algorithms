using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    interface ISimpleCollection<TValue>
    {
        /// <summary>
        /// This should Retrun number of valid elements in Collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// This should return root element of Collection
        /// </summary>
        SingleLinkedNode<TValue> Root { get; }

        /// <summary>
        /// Function will add new element to collection
        /// </summary>
        /// <param name="item"></param>
        void Add(TValue item);

        /// <summary>
        /// Function will remove the element from collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>If element was removed successfully, returns true, otherwise false</returns>
        bool Remove(TValue item);

        /// <summary>
        /// Function will check, if given item exists in Collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(TValue item);

        /// <summary>
        /// Function will convert Collection to a List and returns
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <returns></returns>
        TValue[] ToList(uint arrayIndex = 0);

    }
}
