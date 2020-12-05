using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ModelLearner.Algorithms.LOUGA.Policies;
using ModelLearner.Algorithms.LOUGA.FitnessFunctions;
using ModelLearner.Algorithms.LOUGA.Individuals;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOUGA
{
    /// <summary>
    /// This algorithm learns original model by performing evolution simulation on population representing possible models.
    /// Models are coded to individuals by generating list of possible predicates operators can possibly add to world for each operator.
    /// Each this pair represents a single gene in individual's genome.
    /// Algorithm two uses simple mutations and crossovers. When population stagnates, performes local greedy search and crossover with old population.
    /// If nothing helps to make population better, restarts population and starts new run.
    /// </summary>
    class LOUGA : ILearningAlgorithm
    {
        /// <summary>
        /// Name of tab with best individuals found.
        /// </summary>
        public const string CurrentBestTabName = "Best individuals";

        public Model Learn(Model originalModel, List<World> worlds)
        {
            if (worlds.Count == 0) throw new Exception("No worlds to learn from.");

            Model m = originalModel;

            //preparation of run
            IntegerIndividualRepresenter representer = new IntegerIndividualRepresenter(m, worlds);
            representer.RemovePossibilities();

            IntegerErrorRateFunction erFunction = new IntegerErrorRateFunction(m, representer, worlds);
            IFitnessFunction<IntegerIndividual> fitnessfunction = new IntegerPrefixTreeWrapper(erFunction);

            //policies
            BestIndividualsLogger logger = new BestIndividualsLogger(1);
            List<IPolicy<IntegerIndividual>> policies = new List<IPolicy<IntegerIndividual>>();
            policies.Add(new SimpleMutation(representer, 0.1, 0.05));
            policies.Add(new SimpleCrossover());
            policies.Add(new LocalSearchPolicy(representer, fitnessfunction, Log));
            policies.Add(logger);
            policies.Add(new IntegerRestartingPolicy(fitnessfunction, representer, Log));
            policies.Add(logger);

            GeneticAlgorithm alg = new GeneticAlgorithm(fitnessfunction, policies, Log);

            representer.RemovePossibilities();

            //output functions for output manager
            Form1.Manager.ITM[1] = representer.GetDecodeFunction();
            Form1.Manager.ITS[1] = representer.OutputModel;

            //initial population
            List<IntegerIndividual> startingPopulation = new List<IntegerIndividual>();
            for (int i = 0; i < Properties.Settings.Default.LOUGA_PopulationSize; i++)
            {
                startingPopulation.Add(new IntegerIndividual(representer.GenomeLength, representer.Possible));
            }

            Form1.Manager.WriteLine("Genome length is " + representer.GenomeLength + " genes.");

            //algorithm run
            IntegerIndividual solution = alg.Run(startingPopulation);

            if (solution == null || Form1.Halt) return null;

            m = representer.DecodeModel(solution);

            return m;
        }

        /// <summary>
        /// Method used for testing. Computes fitness of individual represented by given string.
        /// </summary>
        /// <param name="individual"></param>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        private void Evaluate(string individual, Model m, List<World> worlds)
        {
            IntegerIndividual ii = new IntegerIndividual(individual);
            IntegerIndividualRepresenter representer = new IntegerIndividualRepresenter(m, worlds);
            IFitnessFunction<IntegerIndividual> fitness = new IntegerPrefixTreeWrapper(new IntegerErrorRateFunction(m, representer, worlds));

            Console.WriteLine(representer.OutputModel(ii));

            Console.WriteLine(fitness.GetValue(ii));
        }

        /// <summary>
        /// Method used for testing. Computes fitness of individual represented by given string.
        /// </summary>
        /// <param name="individual"></param>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        /// <param name="predicate"></param>
        private void Evaluate(string individual, Model m, List<World> worlds, string predicate)
        {
            IntegerIndividual ii = new IntegerIndividual(individual);
            IntegerIndividualRepresenter representer = new IntegerIndividualRepresenter(m, worlds);
            IFitnessFunction<IntegerIndividual> fitness = new IntegerErrorRateOfSinglePredicate(m, representer, worlds, m.Predicates[predicate]);

            Console.WriteLine(representer.OutputModel(ii));

            Console.WriteLine(fitness.GetValue(ii));
        }

        /// <summary>
        /// Method used for testing. Computes fitness of individual represented by given string.
        /// </summary>
        /// <param name="genome"></param>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        private void Evaluate(int[] genome, Model m, List<World> worlds)
        {
            IntegerIndividual ii = new IntegerIndividual(genome);
            IntegerIndividualRepresenter representer = new IntegerIndividualRepresenter(m, worlds);
            IFitnessFunction<IntegerIndividual> fitness = new IntegerPrefixTreeWrapper(new IntegerErrorRateFunction(m, representer, worlds));

            Console.WriteLine(representer.OutputModel(ii));

            Console.WriteLine(fitness.GetValue(ii));
        }

        /// <summary>
        /// Method used to log text to user trough OutputManager class.
        /// </summary>
        /// <param name="text"></param>
        private void Log(string text)
        {
            Console.WriteLine(text);
            Form1.Manager.WriteLine(text);
        }
    }
}
