using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    /// <summary>
    /// Interface defines genertion of Random values for Type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IRandomizable<T>
    {
        /// <summary>
        /// Function Returns next random for the type
        /// </summary>
        /// <returns></returns>
        T NextRandom(); 
    }
}
