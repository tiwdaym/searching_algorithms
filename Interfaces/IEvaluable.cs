using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchingAlgorithms
{
    /// <summary>
    /// Interface defines Evaluation methods for Evolution checking
    /// </summary>
    /// <typeparam name="T">Type of one bit of chromosome to use for Evaluating</typeparam>
    public interface IEvaluable<T>
    {
        /// <summary>
        /// Simple Fitness method, where Fitness is computed.
        /// Higher is better.
        /// </summary>
        /// <param name="chromosome"></param>
        /// <returns></returns>
        uint Fitness(T[] chromosome);
    }
}
