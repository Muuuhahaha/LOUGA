using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.LOUGA
{
    /// <summary>
    /// Interface for general individual.
    /// </summary>
    public interface IIndividual
    {
        /// <summary>
        /// Returns individual's fitness.
        /// </summary>
        /// <returns></returns>
        double GetFitness();

        /// <summary>
        /// Sets individual's fitness to given value.
        /// </summary>
        /// <param name="fitness"></param>
        void SetFitness(double fitness);

        /// <summary>
        /// Makes copy of individual.
        /// </summary>
        /// <returns></returns>
        IIndividual Clone();

        /// <summary>
        /// Returns length of indivual's genome.
        /// </summary>
        /// <returns></returns>
        int GetGeneLength();

        /// <summary>
        /// Returns individual's age.
        /// </summary>
        /// <returns></returns>
        int GetAge();

        /// <summary>
        /// Increments individual's age by one.
        /// </summary>
        void IncrementAge();
    }
}
