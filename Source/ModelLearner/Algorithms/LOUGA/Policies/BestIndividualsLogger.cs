using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Individuals;

namespace ModelLearner.Algorithms.LOUGA.Policies
{
    /// <summary>
    /// Class used to store best individuals in LOUGA's run and show them to the user.
    /// </summary>
    public class BestIndividualsLogger : IPolicy<IntegerIndividual>
    {
        /// <summary>
        /// List of best individuals.
        /// </summary>
        public List<IntegerIndividual> AllTimeBestIndividuals;

        /// <summary>
        /// Maximum number of stored individuals.
        /// </summary>
        public const int MaxBestIndividuals = 30;

        /// <summary>
        /// ID of tab the logger should output individuals to.
        /// </summary>
        public int TabId;

        public BestIndividualsLogger(int tabId)
        {
            this.TabId = tabId;
            AllTimeBestIndividuals = new List<IntegerIndividual>();
        }

        /// <summary>
        /// General IPolicy funcion used for searching for added individuals.
        /// </summary>
        /// <param name="population"></param>
        public void Affect(List<IntegerIndividual> population)
        {
            UpdateAllTimeBest(population);
        }

        /// <summary>
        /// Updates list of all-time best individuals with individuals from current population.
        /// </summary>
        /// <param name="population"></param>
        private void UpdateAllTimeBest(List<IntegerIndividual> population)
        {
            List<IntegerIndividual> newBest = new List<IntegerIndividual>();

            foreach (var individual in population)
            {
                if (AllTimeBestIndividuals.Count == 0 ||
                    (!AllTimeBestIndividuals.Contains(individual) &&
                     ((AllTimeBestIndividuals[AllTimeBestIndividuals.Count - 1].Fitness < individual.Fitness || 
                     AllTimeBestIndividuals.Count < MaxBestIndividuals))))
                {
                    bool found = false;

                    for (int i = AllTimeBestIndividuals.Count - 1; i >= 0; i--)
                    {
                        if (AllTimeBestIndividuals[i].Fitness >= individual.Fitness)
                        {
                            AllTimeBestIndividuals.Insert(i + 1, individual.Clone());
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        AllTimeBestIndividuals.Insert(0, individual.Clone());
                    }

                    newBest.Add(individual.Clone());
                }
            }

            if (AllTimeBestIndividuals.Count > MaxBestIndividuals)
            {
                AllTimeBestIndividuals.RemoveRange(MaxBestIndividuals, AllTimeBestIndividuals.Count - MaxBestIndividuals);
            }

            Form1.Manager.AddBestIndividuals(newBest, TabId);
        }
    }
}
