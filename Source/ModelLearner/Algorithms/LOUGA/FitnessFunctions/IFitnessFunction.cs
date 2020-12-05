using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.LOUGA.FitnessFunctions
{
    /// <summary>
    /// Interface for general fitness function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFitnessFunction<T>
    {
        /// <summary>
        /// Returns value of given individual.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        double GetValue(T individual);
    }
}
