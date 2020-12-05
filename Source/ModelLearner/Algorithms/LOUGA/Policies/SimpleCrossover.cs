using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Individuals;

namespace ModelLearner.Algorithms.LOUGA.Policies
{
    /// <summary>
    /// This class implements standard single-point crossover. 
    /// When crossing, generates random point in parents' genomes. 
    /// Data beyond that point is swaped and thus are created 2 new individuals.
    /// </summary>
    public class SimpleCrossover : IPolicy<IntegerIndividual>
    {
        private static Random rng = new Random();

        public SimpleCrossover()
        {
        }

        /// <summary>
        /// Breeds two individuals and creates two new with genes created by combining parent's genes.
        /// </summary>
        /// <param name="mother"></param>
        /// <param name="father"></param>
        /// <returns>Two new individuals</returns>
        public Tuple<IntegerIndividual, IntegerIndividual> Breed(IntegerIndividual mother, IntegerIndividual father)
        {
            int[] first = new int[mother.Genome.Length];
            int[] second = new int[mother.Genome.Length];

            int splitPoint = rng.Next(mother.Genome.Length + 1);

            for (int i = 0; i < mother.Genome.Length; i++)
            {
                if (i >= splitPoint)
                {
                    first[i] = mother[i];
                    second[i] = father[i];
                }
                else
                {
                    first[i] = father[i];
                    second[i] = mother[i];
                }
            }

            return new Tuple<IntegerIndividual, IntegerIndividual>(new IntegerIndividual(first), new IntegerIndividual(second));                                                            
        }

        /// <summary>
        /// General IPolicy's function shuffling the population and then crossing random individuals.
        /// </summary>
        /// <param name="population"></param>
        public void Affect(List<IntegerIndividual> population)
        {
            GeneralPlanningLibrary.Utility.Utility.Shuffle<IntegerIndividual>(population);

            int max = population.Count - 1;

            for (int i = 0; i < max ; i += 2)
            {
                var tuple = Breed(population[i], population[i + 1]);

                population.Add(tuple.Item1);
                population.Add(tuple.Item2);
            }
        }
    }
}
