using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Policies;
using ModelLearner.Algorithms.LOUGA.FitnessFunctions;
using ModelLearner.Algorithms.LOUGA.Individuals;

namespace ModelLearner.Algorithms.LOUGA
{
    /// <summary>
    /// Class running general genetic algorithm.
    /// Mutations, crossovers and other functions are represented as classes inplementing IPolicy interface and are called every population.
    /// </summary>
    public class GeneticAlgorithm
    {
        public IFitnessFunction<IntegerIndividual> FitnessFunction;
        public List<IPolicy<IntegerIndividual>> Policies;

        public List<IntegerIndividual> Population;
        public int GenerationId;

        public bool FoundSolution;

        private Action<string> Log;

        public GeneticAlgorithm(IFitnessFunction<IntegerIndividual> fitnessFunciton, List<IPolicy<IntegerIndividual>> policies, Action<string> logFunction)
        {
            this.FitnessFunction = fitnessFunciton;
            this.Policies = policies;
            this.Log = logFunction;
        }

        /// <summary>
        /// Runs algorithm for preset number of generations or until solution is found (if so set up in settings)
        /// </summary>
        /// <param name="startingPopulation"></param>
        /// <returns></returns>
        public IntegerIndividual Run(List<IntegerIndividual> startingPopulation)
        {
            SetPopulation(startingPopulation);
            
            for (int i = 0; i < Properties.Settings.Default.LOUGA_GenerationsCount; i++)
            {
                RunGeneration();

                if (Population[0].Fitness == 1 && 
                    (Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound ||
                    Properties.Settings.Default.LOUGA_PredicateByPredicate)
                    )
                {
                    break;
                }

                if (Form1.Halt) return null;
            }

            if (Population[0].GetFitness() != 1)
            {
                FitnessFunction.GetValue(Population[0]);
            }

            return Population[0];
        }

        /// <summary>
        /// Runs algorithm for given number of generations and returns best individual.
        /// </summary>
        /// <param name="generationsCount"></param>
        /// <returns></returns>
        public IntegerIndividual RunFewGenerations(int generationsCount)
        {
            if (Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound && Population[0].GetFitness() == 1)
            {
                return Population[0];
            }

            for (int i = 0; i < generationsCount && GenerationId < Properties.Settings.Default.LOUGA_GenerationsCount; i++)
            {
                RunGeneration();

                if (Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound && Population[0].GetFitness() == 1)
                {
                    break;
                }
            }

            return Population[0];
        }
        
        /// <summary>
        /// Runs algorithm for a single generation and returns best individual.
        /// Lets each policy affect population. After every policy does its job, recalculates fitness value of each added/changed individual 
        /// and removes bad individuals from population if needed.
        /// </summary>
        /// <returns>Best individual in current population</returns>
        public IntegerIndividual RunGeneration()
        {
            foreach (var individual in Population) individual.IncrementAge();

            foreach (var policy in Policies)
            {
                policy.Affect(Population);

                
                Population.Sort((x, y) => y.CompareTo(x));

                if (Population.Count > Properties.Settings.Default.LOUGA_PopulationSize)
                {
                    Population.RemoveRange(Properties.Settings.Default.LOUGA_PopulationSize, Population.Count - Properties.Settings.Default.LOUGA_PopulationSize);
                }

                foreach (var individual in Population)
                {
                    if (individual.GetFitness() == double.MinValue)
                    {
                        individual.Fitness = FitnessFunction.GetValue(individual);
                        if (Form1.Halt) return null;
                    }
                }

                if (Form1.Halt) return null;
            }


            Log("Generation " + (GenerationId + 1) + ": best individual:" + Population[0].GetFitness());

            GenerationId++;

            return Population[0];
        }


        public void SetPopulation(List<IntegerIndividual> startingPopulation)
        {
            Population = new List<IntegerIndividual>();
            foreach (var individual in startingPopulation) Population.Add((IntegerIndividual)individual.Clone());

            GenerationId = 0;
        }
    }
}
