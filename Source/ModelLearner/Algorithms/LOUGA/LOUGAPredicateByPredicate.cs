using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Policies;
using ModelLearner.Algorithms.LOUGA.FitnessFunctions;
using ModelLearner.Algorithms.LOUGA.Individuals;
using System.Windows.Forms;
using GeneralPlanningLibrary;
using GeneralPlanningLibrary.Utility;

namespace ModelLearner.Algorithms.LOUGA
{
    /// <summary>
    /// This algorithm works almost the same as LOUGA algorithm. Only difference is that it focuses only on a single predicate type at a time.
    /// Since different predicate types are independent on each other this approach doesn't make outcome worse. Only makes the algorithm faster.
    /// </summary>
    class LOUGAPredicateByPredicate : ILearningAlgorithm
    {
        public const string CurrentBestTabName = "Best individuals";

        public Model Learn(Model originalModel, List<World> worlds)
        {
            if (worlds.Count == 0) throw new Exception("No worlds to learn from.");

            Model m = originalModel;
            
            //general preparation.
            IntegerIndividualRepresenter representer = new IntegerIndividualRepresenter(m, worlds);
            IFitnessFunction<IntegerIndividual> fitness = new FitnessFunctions.IntegerPrefixTreeWrapper(new FitnessFunctions.IntegerErrorRateFunction(m, representer, worlds));
            //IFitnessFunction<IntegerIndividual> fitness = new FitnessFunctions.IntegerErrorRateFunction(m, representer, worlds);
            Form1.Manager.ITM[1] = representer.GetDecodeFunction();
            Form1.Manager.ITS[1] = representer.OutputModel;
            int[] finalGenome = new int[representer.GenomeLength];

            List<GeneticAlgorithm> algorithms = new List<GeneticAlgorithm>();
            List<IntegerIndividual>[] bestIndividualsLists = new List<IntegerIndividual>[m.Predicates.Count];
            string[] predicateNames = new string[m.Predicates.Count];
            bool[] foundSolution = new bool[m.Predicates.Count];
            int numberOfAlgorithmsRunning = m.Predicates.Count;

            Form1.Manager.WriteLine("Preparing evolutions");

            //sorting predicates
            SortedDictionary<string, Predicate> sortedPredicates = new SortedDictionary<string, Predicate>();
            foreach (var pair in m.Predicates)
            {
                sortedPredicates.Add(pair.Key, pair.Value);
            }

            //prepartions for each predicate.
            foreach (var predicate in sortedPredicates) 
            {
                var ea = PrepareEvolutionAlgorithmForPredicate(predicate.Value, m, worlds, 2 + algorithms.Count * 2);

                bestIndividualsLists[algorithms.Count] = ((BestIndividualsLogger)ea.Policies[4]).AllTimeBestIndividuals;
                predicateNames[algorithms.Count] = predicate.Key;

                algorithms.Add(ea);

                if (Form1.Halt) return null;

                Form1.Manager.WriteLine("Prepared evolution for predicate '" + predicate.Key + "'.");
            }

            Form1.Manager.WriteLine("Preparation finished");
            Form1.Manager.WriteLine("Genome length is " + representer.GenomeLength + " genes.");

            for (int generation = 0; generation < Properties.Settings.Default.LOUGA_GenerationsCount; generation++)
            {
                if (Form1.Halt) return null;

                for (int id = 0; id < algorithms.Count; id++)
                {
                    if (foundSolution[id]) continue;

                    algorithms[id].RunGeneration();

                    if (Form1.Halt) return null;

                    if (bestIndividualsLists[id][0].Fitness == 1)
                    {
                        foundSolution[id] = true;

                        numberOfAlgorithmsRunning--;
                        Form1.Manager.WriteLine("Found solution for predicate '" + predicateNames[id] + "'.");
                    }
                }

                //creating new best individual
                int[] currentBest = new int[representer.GenomeLength];
                ErrorInfo info = new ErrorInfo();

                for (int id = 0; id < algorithms.Count; id++)
                {
                    if (bestIndividualsLists[id].Count > 0)
                    {
                        int[] genome = bestIndividualsLists[id][0].Genome;

                        for (int j = 0; j < genome.Length; j++)
                        {
                            if (genome[j] != 0)
                            {
                                currentBest[j] = genome[j];

                                PredicateReference pr = representer.PairReference[representer.Names[j]];
                            }
                        }

                        info.ErrorsAdd += bestIndividualsLists[id][0].ErrorInfo.ErrorsAdd;
                        info.ErrorsDel += bestIndividualsLists[id][0].ErrorInfo.ErrorsDel;
                        info.ErrorsPre += bestIndividualsLists[id][0].ErrorInfo.ErrorsPre;
                        info.ErrorsObservation += bestIndividualsLists[id][0].ErrorInfo.ErrorsObservation;
                        info.TotalAdd += bestIndividualsLists[id][0].ErrorInfo.TotalAdd;
                        info.TotalDel += bestIndividualsLists[id][0].ErrorInfo.TotalDel;
                        info.TotalPre += bestIndividualsLists[id][0].ErrorInfo.TotalPre;
                        info.TotalObservation += bestIndividualsLists[id][0].ErrorInfo.TotalObservation;
                    }
                }

                bool bestChanged = false;

                for (int i = 0; i < currentBest.Length; i++)
                {
                    if (currentBest[i] != finalGenome[i])
                    {
                        bestChanged = true;
                        finalGenome = currentBest;
                        break;
                    }
                }

                if (bestChanged)
                {
                    RichTextBox rtb = new RichTextBox();
                    rtb.Font = OutputManager.TextboxFont;

                    rtb.Text = representer.DecodeModel(new IntegerIndividual(finalGenome)).ToString();

                    Form1.Manager.Highlight(rtb);

                    Form1.Manager.SetTabRTF(rtb.Rtf);
                    Form1.Manager.Solution = new IntegerIndividual(finalGenome);
                }

                IntegerIndividual ii = new IntegerIndividual(finalGenome);

                ii.ErrorInfo = info;

                ii.Fitness = 1;
                if (info.TotalAdd + info.TotalDel > 0) ii.Fitness *= (1 - (double)(info.ErrorsAdd + info.ErrorsDel) / (info.TotalAdd + info.TotalDel));
                if (info.TotalObservation > 0) ii.Fitness *= (1 - (double)info.ErrorsObservation / info.TotalObservation * Properties.Settings.Default.LOUGA_ObservationErrorWeight);

                if (generation % 10 == 0)
                {
                    ii.Fitness = fitness.GetValue(ii);
                }

                Form1.Manager.WriteLine("Generation " + (generation + 1) + ": best individual:" + ii.GetFitness());


                if (numberOfAlgorithmsRunning == 0) break;
            }


            //Evaluate(finalGenome, m, worlds);

            Form1.Manager.WriteLine("Algorithm finished.");
            Form1.Manager.Solution = new IntegerIndividual(finalGenome);

            Form1.Manager.WriteLine("\n" + Utility.CountErrors(representer.DecodeModel(finalGenome), worlds).ToString());

            return representer.DecodeModel(finalGenome);
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
        /// Finds perfect solution for a single predicate.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        /// <param name="outputTabId">index of tab for output</param>
        /// <returns></returns>
        private GeneticAlgorithm PrepareEvolutionAlgorithmForPredicate(Predicate p, Model m, List<World> worlds, int outputTabId)
        {
            IntegerIndividualRepresenter representer = new IntegerIndividualRepresenter(m, worlds);
            representer.RemovePossibilities();

            IFitnessFunction<IntegerIndividual> fitnessfunction = new IntegerPrefixTreeWrapper(new IntegerErrorRateOfSinglePredicate(m, representer, worlds, p));
            
            
            //logging function for algorithm
            Action<string> Log = new Action<string>
                ((string x) =>
                    {
                        Console.WriteLine(x);
                        Form1.Manager.WriteLine(x, outputTabId);
                    }
                );

            //policies
            List<IPolicy<IntegerIndividual>> policies = new List<IPolicy<IntegerIndividual>>();
            policies.Add(new SimpleMutation(representer, 0.1, 0.05));
            policies.Add(new SimpleCrossover());
            policies.Add(new LocalSearchPolicy(representer, fitnessfunction, Log));
            policies.Add(new IntegerRestartingPolicy(fitnessfunction, representer, Log));
            policies.Add(new BestIndividualsLogger(outputTabId + 1));

            //removing possibilities concerning other predicates.
            for (int i = 0; i < representer.GenomeLength; i++)
            {
                if (representer.PairReference[representer.Names[i]].Predicate != p)
                {
                    representer.Possible[i, IntegerIndividualRepresenter.Add] = false;
                    representer.Possible[i, IntegerIndividualRepresenter.Delete] = false;
                    representer.Possible[i, IntegerIndividualRepresenter.None] = true;

                    representer.MultipleOptions[i] = false;


                    PredicateReference pr = representer.PairReference[representer.Names[i]];
                    representer.PossiblePredicates[pr.Operator.Name].Remove(pr.ToString());
                }
            }

            Form1.Manager.ITM[outputTabId + 1] = representer.GetDecodeFunctionForPredicate(p);
            Form1.Manager.ITS[outputTabId + 1] = representer.OutputModel;

            GeneticAlgorithm alg = new GeneticAlgorithm(fitnessfunction, policies, Log);

            //initial population
            List<IntegerIndividual> startingPopulation = new List<IntegerIndividual>();
            for (int i = 0; i < Properties.Settings.Default.LOUGA_PopulationSize - 1; i++)
            {
                startingPopulation.Add(new IntegerIndividual(representer.GenomeLength, representer.Possible));
            }
            startingPopulation.Add(new IntegerIndividual(new int[representer.GenomeLength]));

            alg.Population = startingPopulation;

            foreach (var individual in alg.Population)
            {
                individual.Fitness = fitnessfunction.GetValue(individual);
            }

            return alg;
        }
    }
}
