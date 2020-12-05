using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Individuals;
using ModelLearner;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOUGA.FitnessFunctions
{
    /// <summary>
    /// This class represents fitness function that makes fittnes value by counting add, delete and observation errors in input plans.
    /// Only focuses on one predicate.
    /// </summary>
    class IntegerErrorRateOfSinglePredicate : IFitnessFunction<IntegerIndividual>
    {
        /// <summary>
        /// Model description.
        /// </summary>
        public Model M;

        /// <summary>
        /// Input worlds.
        /// </summary>
        public List<World> Worlds;

        /// <summary>
        /// Individual representer.
        /// </summary>
        public IntegerIndividualRepresenter Representer;

        /// <summary>
        /// Predicate the fitness function focuses on.
        /// </summary>
        public Predicate P;

        public IntegerErrorRateOfSinglePredicate(Model m, IntegerIndividualRepresenter representer, List<World> worlds, Predicate p)
        {
            this.M = m;
            this.Representer = representer;
            this.Worlds = worlds;
            this.P = p;

            CutPredicatesInWorlds();
        }

        /// <summary>
        /// Creates internal copy of Worlds list and removes unnecesarry predicates from states to speed up computing.
        /// </summary>
        private void CutPredicatesInWorlds()
        {
            List<World> newWorlds = new List<World>();

            foreach (var world in Worlds)
            {
                World newWorld = new World();
                List<Plan> newPlans = new List<Plan>();

                newWorld.Objects = world.Objects;
                newWorld.Model = world.Model;
                newWorld.ModelName = world.ModelName;
                newWorld.Name = world.ModelName;

                foreach (var plan in world.Plans)
                {
                    State[] newStates = new State[plan.States.Length];

                    for (int i = 0; i < plan.States.Length; i++)
                    {
                        if (plan.States[i] == null || plan.States[i].Predicates == null)
                        {
                            newStates[i] = null;
                        }
                        else
                        {
                            List<PredicateInstance> predicates = new List<PredicateInstance>();

                            foreach (var predicate in plan.States[i].Predicates)
                            {
                                if (predicate.Type == P)
                                {
                                    predicates.Add(predicate);
                                }
                            }

                            if (predicates.Count > 0)
                            {
                                newStates[i] = new State(predicates.ToArray());
                            }
                        }
                    }

                    newPlans.Add(new Plan(plan.Actions, newStates));
                }

                newWorld.Plans = newPlans.ToArray();
                newWorlds.Add(newWorld);
            }

            Worlds = newWorlds;
        }

        /// <summary>
        /// Returns value of given individual.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public double GetValue(IntegerIndividual individual)
        {
            individual.ErrorInfo.TotalAdd = 0;
            individual.ErrorInfo.TotalDel = 0;
            individual.ErrorInfo.TotalObservation = 0;
            individual.ErrorInfo.ErrorsAdd = 0;
            individual.ErrorInfo.ErrorsDel = 0;
            individual.ErrorInfo.ErrorsObservation = 0;

            foreach (World w in Worlds)
            {
                foreach (Plan p in w.Plans)
                {
                    List<PredicateInstance> state = new List<PredicateInstance>();
                    if (p.States[0] != null)
                    {
                        foreach (var pi in p.States[0].Predicates)
                        {
                            if (pi.Type == P)
                            {
                                state.Add(new PredicateInstance(pi.Parameters, pi.Type));
                            }
                        }
                    }

                    for (int i = 0; i < p.Actions.Length; i++)
                    {
                        if (Form1.Halt) return double.MinValue;

                        GeneralPlanningLibrary.Action a = p.Actions[i];

                        List<string> possiblePredicates = Representer.PossiblePredicates[a.Operator.Name];
                        foreach (string predicate in possiblePredicates)
                        {
                            string pairName = Representer.GetName(a.Operator.Name, predicate);

                            int index = Representer.PairID[pairName];
                            PredicateReference pr = Representer.PairReference[pairName];
                            if (pr.Predicate != P) continue;
                            PredicateInstance pi = a.InstantiatePredicate(pr);

                            int predicateIndex = state.FindIndex(x => x.Equals(pi));

                            //Add or delete errors
                            if (individual.Genome[index] == IntegerIndividualRepresenter.Add)
                            {
                                individual.ErrorInfo.TotalAdd++;
                                if (predicateIndex != -1)
                                {
                                    individual.ErrorInfo.ErrorsAdd++;
                                }
                                else
                                {
                                    state.Add(pi);
                                }
                            }
                            if (individual.Genome[index] == IntegerIndividualRepresenter.Delete)
                            {
                                individual.ErrorInfo.TotalDel++;
                                if (predicateIndex == -1)
                                {
                                    individual.ErrorInfo.ErrorsDel++;
                                }
                                else
                                {
                                    state.RemoveAt(predicateIndex);
                                }
                            }
                        }

                        //observation errors
                        if (p.States[i + 1] != null)
                        {
                            foreach (var statePredicate in p.States[i + 1].Predicates)
                            {
                                if (statePredicate.Type != P) continue;

                                individual.ErrorInfo.TotalObservation++;
                                if (!state.Contains(statePredicate))
                                {
                                    individual.ErrorInfo.ErrorsObservation++;
                                }
                            }

                            if (Properties.Settings.Default.LOUGA_GoalStateComplete && i + 1 == p.Actions.Length)
                            {
                                foreach (var modelPredicate in state)
                                {
                                    if (modelPredicate.Type != P) continue;

                                    individual.ErrorInfo.TotalObservation++;

                                    bool found = false;
                                    for (int j = 0; j < p.States[i + 1].Predicates.Length; j++)
                                    {
                                        if (p.States[i + 1].Predicates[j].Equals(modelPredicate))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        individual.ErrorInfo.ErrorsObservation++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double fitness = 1;
            if (individual.ErrorInfo.TotalAdd + individual.ErrorInfo.TotalDel > 0) fitness *= (1 - (double)(individual.ErrorInfo.ErrorsAdd + individual.ErrorInfo.ErrorsDel) / (individual.ErrorInfo.TotalAdd + individual.ErrorInfo.TotalDel));
            if (individual.ErrorInfo.TotalObservation > 0) fitness *= (1 - (double)individual.ErrorInfo.ErrorsObservation / individual.ErrorInfo.TotalObservation * Properties.Settings.Default.LOUGA_ObservationErrorWeight);

            individual.Fitness = fitness;

            return fitness;
        }
    }
}
