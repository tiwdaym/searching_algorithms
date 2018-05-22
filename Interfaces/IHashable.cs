using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    interface IHashable
    {
        /// <summary>
        /// Function is used to generate hash of actual instance.
        /// Hash needs to be in uint. Beware using GetHashCode as it usually returns only reference to instance.
        /// </summary>
        /// <returns>hash of actual instance</returns>
        uint GetHash();
    }
}
