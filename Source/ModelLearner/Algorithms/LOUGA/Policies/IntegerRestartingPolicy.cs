using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Policies;
using ModelLearner.Algorithms.LOUGA.FitnessFunctions;
using ModelLearner.Algorithms.LOUGA.Individuals;
using GeneralPlanningLibrary.Utility;

namespace ModelLearner.Algorithms.LOUGA.Policies
{
    /// <summary>
    /// Policy that restarts population when the evolution stagnates for too long.
    /// Also can crosses current best individual with previously found best individuals if its age reaches defined value.
    /// </summary>
    public class IntegerRestartingPolicy : IPolicy<IntegerIndividual>
    {
        /// <summary>
        /// Crossover used to cross currently best individual with previously found best individuals.
        /// </summary>
        SimpleCrossover Crossover;

        IFitnessFunction<IntegerIndividual> FitnessFunction;
        IntegerIndividualRepresenter Representer;
        private static Random rng = new Random();

        /// <summary>
        /// Maximum number of individuals in old population.
        /// </summary>
        public const int MaxOldPopSize = 10;

        /// <summary>
        /// Maximum age of best individual in old population before restarting old population.
        /// </summary>
        public const int MaxOldPopAge = 15;

        /// <summary>
        /// Best individuals from previous runs.
        /// </summary>
        private List<IntegerIndividual> OldPopulation;

        /// <summary>
        /// Restarted old populations. 
        /// Don't affect algorithm run in any way. Stored just to preserve history.
        /// </summary>
        private List<List<IntegerIndividual>> OldOldPopulations;

        /// <summary>
        /// List of all individuals that had fitness 1.
        /// </summary>
        private List<IntegerIndividual> Solutions;

        /// <summary>
        /// Method used to write algorithm's progress to correct output tab in UI.
        /// </summary>
        private Action<string> Log;

        public IntegerRestartingPolicy(IFitnessFunction<IntegerIndividual> fitnessFunction, IntegerIndividualRepresenter representer, Action<string> logFunction)
        {
            this.FitnessFunction = fitnessFunction;
            this.Representer = representer;
            this.OldPopulation = new List<IntegerIndividual>();
            this.OldOldPopulations = new List<List<IntegerIndividual>>();
            this.Solutions = new List<IntegerIndividual>();
            this.Log = logFunction;

            this.Crossover = new SimpleCrossover();
        }

        /// <summary>
        /// General IPolicy's method. Checks best individual's age and crosses him with old population or restarts population.
        /// </summary>
        /// <param name="population"></param>
        public void Affect(List<IntegerIndividual> population)
        {
            //crossover with old population
            if (population[0].Age == Properties.Settings.Default.LOUGA_CrossoverWithOldPopulationTreshold)
            {
                foreach (var individual in OldPopulation)
                {
                    var tuple = Crossover.Breed(population[0], individual);

                    population.Add(tuple.Item1);
                    population.Add(tuple.Item2);

                    tuple.Item1.Fitness = FitnessFunction.GetValue(tuple.Item1);
                    tuple.Item2.Fitness = FitnessFunction.GetValue(tuple.Item2);

                    if (Form1.Halt) return;
                }
                Log("crossover with old population");
                
                population.Sort((x, y) => y.CompareTo(x));
            }

            //solution found
            if (population[0].Fitness == 1)
            {
                RestartPopulation(population);
                if (Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound)
                {
                    return;
                }
            }
            //population restart
            else if (population[0].Age >= Properties.Settings.Default.LOUGA_PopulationRestartTreshold)
            {
                RestartPopulation(population);
            }
        }
        
        /// <summary>
        /// Restarts population, saves best individual and logs everything.
        /// </summary>
        /// <param name="population"></param>
        private void RestartPopulation(List<IntegerIndividual> population)
        {
            //checking if soluton was found
            if (population[0].Fitness < 1)
            {
                Log("-----------------------------------------------------------------------");
                Log("Best individual with fitness " + population[0].Fitness + ".");
            }
            else
            {
                Log("");
                Log("-----------------------------------------------------------------------");
                Log("Found solution!");
                Log("-----------------------------------------------------------------------");

                if (Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound)
                {
                    Log(Representer.OutputModel(population[0]));
                    return;
                }
            }
            
            //adding best individual to old population list
            int popsize = Properties.Settings.Default.LOUGA_PopulationSize;
            int index = OldPopulation.FindIndex(x => x.Fitness == population[0].Fitness);
            if (index == -1)
            {
                population[0].Age = 0;

                OldPopulation.Add(population[0]);
                OldPopulation.Sort((x, y) => y.CompareTo(x));
                if (OldPopulation.Count > MaxOldPopSize)
                {
                    OldPopulation.RemoveRange(MaxOldPopSize, OldPopulation.Count - MaxOldPopSize);
                }
            }

            OldPopulation[0].Age++;

            if (OldPopulation[0].Fitness == 1)
            {
                Solutions.Add(OldPopulation[0]);
                OldPopulation = new List<IntegerIndividual>();
            }

            //restarting old population
            else if (OldPopulation[0].Age >= Properties.Settings.Default.LOUGA_OldPopulationRestartTreshold)
            {
                OldOldPopulations.Add(OldPopulation);
                OldPopulation = new List<IntegerIndividual>();
                Log("Old population cleared");
            }
            
            //generating new population
            population.Clear();
            while (population.Count < popsize)
            {
                IntegerIndividual ii = new IntegerIndividual(Representer.GenomeLength, Representer.Possible);
                population.Add(ii);
            }

            //output for user
            Log("Former old populations:");
            for (int i = 0; i < OldOldPopulations.Count; i++)
            {
                Log(" Old pop nr. " + i + ":");
                for (int j = 0; j < OldOldPopulations[i].Count; j++)
                {
                    Log(j + "): " + string.Format("{0:0.00000}", OldOldPopulations[i][j].Fitness) + " - " + OldOldPopulations[i][j].ToString());
                }
            }

            Log("Restarted population. Currently in old population:");
            for (int i = 0; i < OldPopulation.Count; i++)
            {
                Log(i + "): " + string.Format("{0:0.00000}", OldPopulation[i].Fitness) + " - " + OldPopulation[i].ToString());
            }

            Log("-----------------------------------------------------------------------");
        }
    }
}
