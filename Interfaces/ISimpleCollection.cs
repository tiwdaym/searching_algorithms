using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    public interface ISimpleCollection<T>
    {
        /// <summary>
        /// This should Retrun number of valid elements in Collection
        /// </summary>
        uint Count { get; }

        /// <summary>
        /// Function will add new element to collection
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);

        /// <summary>
        /// Function will remove the element from collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>If element was removed successfully, returns true, otherwise false</returns>
        bool Remove(T item);

        /// <summary>
        /// Function will check, if given item exists in Collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(T item);

        /// <summary>
        /// Function will convert Collection to a List and returns
        /// </summary>
        /// <returns></returns>
        T[] ToList();

    }
}
