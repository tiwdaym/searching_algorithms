using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    /// <summary>
    /// Interface defines generative functions, needed in path computation.
    /// Operations need to be defined as int and should be unique.
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
    }
}
