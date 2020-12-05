using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Individuals;

namespace ModelLearner.Algorithms.LOUGA.Policies
{
    /// <summary>
    /// Simple mutation that switches a gene in individual with given probability.
    /// </summary>
    class SimpleMutation : IPolicy<IntegerIndividual>
    {
        IntegerIndividualRepresenter Representer;
        
        /// <summary>
        /// Probability to switch a gene in an individual.
        /// </summary>
        double GeneSwitchProbability;

        /// <summary>
        /// Probability to mutate an individual.
        /// </summary>
        double MutationProbability;

        private static Random rng = new Random();

        public SimpleMutation(IntegerIndividualRepresenter representer, double geneSwitchProbability, double mutationProbability)
        {
            this.Representer = representer;
            this.GeneSwitchProbability = geneSwitchProbability;
            this.MutationProbability = mutationProbability;
        }

        /// <summary>
        /// General IPolicy's function mutating some individuals in given population.
        /// </summary>
        /// <param name="population"></param>
        public void Affect(List<IntegerIndividual> population)
        {
            foreach (var individual in population)
            {
                if (rng.NextDouble() >= MutationProbability) continue;

                Mutate(individual);
            }
        }

        /// <summary>
        /// Mutates and individual's gene
        /// </summary>
        /// <param name="individual"></param>
        public void Mutate(IntegerIndividual individual)
        {
            if (individual.Fitness == 1) return;
            individual.Fitness = double.MinValue;

            for (int i = 0; i < individual.Genome.Length; i++)
            {
                if (rng.NextDouble() < GeneSwitchProbability)
                {
                    Representer.RandomizeGene(individual, i);
                }
            }
        }
    }
}
