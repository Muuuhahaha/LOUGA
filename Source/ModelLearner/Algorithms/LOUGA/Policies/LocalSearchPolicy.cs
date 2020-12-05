using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Individuals;
using ModelLearner.Algorithms.LOUGA.FitnessFunctions;

namespace ModelLearner.Algorithms.LOUGA.Policies
{
    /// <summary>
    /// Policy that starts greedy search for better individuals when the best individual's age reaches target value.
    /// </summary>
    public class LocalSearchPolicy : IPolicy<IntegerIndividual>
    {
        private IntegerIndividualRepresenter Representer;
        private IFitnessFunction<IntegerIndividual> FitnessFunction;

        /// <summary>
        /// Method used to write algorithm's progress to correct output tab in UI.
        /// </summary>
        private Action<string> LogFunction;

        public LocalSearchPolicy(IntegerIndividualRepresenter representer, IFitnessFunction<IntegerIndividual> fitnessfunction, Action<string> logFunction)
        {
            this.Representer = representer;
            this.FitnessFunction = fitnessfunction;
            this.LogFunction = logFunction;
        }

        /// <summary>
        /// General IPolicy's method used for starting greedy search if necessary.
        /// </summary>
        /// <param name="population"></param>
        public void Affect(List<IntegerIndividual> population)
        {
            if (population[0].Age != Properties.Settings.Default.LOUGA_GreedySearchTreshold) return;

            LogFunction("local search");
            var betterIndividuals = FindBetterIndividuals(population[0]);

            if (Form1.Halt) return;

            population.AddRange(betterIndividuals);
        }

        /// <summary>
        /// Returns list of individuals that can be created from given individual by switching one gene are have better fitness than him.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        private List<IntegerIndividual> FindBetterIndividuals(IntegerIndividual individual)
        {
            List<IntegerIndividual> betterIndividuals = new List<IntegerIndividual>();

            IntegerIndividual ii = individual.Clone();

            for (int i = 0; i < ii.Genome.Length; i++)
            {
                int value = ii[i];

                for (int j = 0; j < IntegerIndividualRepresenter.MaxValue; j++)
                {
                    if (!Representer.Possible[i, j]) continue;

                    ii[i] = j;
                    double fitness = FitnessFunction.GetValue(ii);

                    if (fitness > individual.Fitness)
                    {
                        betterIndividuals.Add(ii.Clone());
                    }
                }

                ii[i] = value;

                if (Form1.Halt) return null;
            }

            return betterIndividuals;
        }

    }
}
