using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    /// <summary>
    /// Interface defines heuristical function, needed in path computation.
    /// </summary>
    /// <typeparam name="T">Class wrapper for data.</typeparam>
    public interface IHeuristical<T>
    {
        /// <summary>
        /// Heuristic should return estimated distance to given graphState.
        /// </summary>
        /// <param name="state">State to compute distance to.</param>
        /// <param name="param">Optional parameters to give for heuristical function.</param>
        /// <returns>Estimated distance to given state as integer. Use 0 if state is equal to given state.</returns>
        int HeuristicDistance(T state, int param);
    }
}
