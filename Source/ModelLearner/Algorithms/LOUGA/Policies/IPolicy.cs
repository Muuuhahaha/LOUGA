using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.LOUGA.Policies
{
    /// <summary>
    /// Interface for general function that affects population in some or all generations.
    /// Can be crossover, mutation or just logging function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPolicy<T>
    {
        void Affect(List<T> population);
    }
}
