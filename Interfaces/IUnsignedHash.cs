using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    interface IUnsignedHash
    {
        /// <summary>
        /// Class should implement this unsigned hash.
        /// Also used to not forget about implementing own Hash Function
        /// </summary>
        /// <returns>Hash in uint</returns>
        uint GetHashCode();
    }
}
