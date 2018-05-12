using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    /// <summary>
    /// Interface defines generative functions, needed in A* path computation.
    /// Operations need to be defined as strings. No error checking for duplicates.
    /// </summary>
    /// <typeparam name="TState">Class wrapper for data.</typeparam>
    public interface IGenerative<TState>
    {
        /// <summary>
        /// Function will generate a new graphState from current TState.
        /// </summary>
        /// <param name="operation">Operation to perform on current graphState.</param>
        /// <returns>New graphState, or null, if operation is invalid, or cannot be performed.</returns>
        TState GenerateNewState(int operation);

        /// <summary>
        /// Function is used to provide list of possible operations to do on various states.
        /// </summary>
        /// <returns>List of operations available for use.</returns>
        int[] OperationsList();

        /// <summary>
        /// Function is used to generate hash from actual TState.
        /// Hash needs to be in uint. Beware using GetHashCode as it usually returns only reference to class.
        /// </summary>
        /// <returns>hash graphState of TState</returns>
        uint GetHash();

        /// <summary>
        /// Function will check for equality of current TState with given graphState.
        /// </summary>
        /// <param name="">State to compare current TState with</param>
        /// <returns>Return true, if states are equal, false otherwise.</returns>
        bool IsEqual(TState state);
    }
}
