using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// This algorithm builds weighted MAX-SAT formula from input plans and tries to find the best solution of that formula
    /// in order to solve original problem.
    /// </summary>
    class ARMS : ILearningAlgorithm
    {
        /// <summary>
        /// Describes if objects can repeat in predicates/operators parameters lists. 
        /// </summary>
        public bool ParametersCanRepeat = false;
        public const string OutputDoubleFormat = "F6";

        /// <summary>
        /// Algorithm used to solve MAX-SAT formula
        /// </summary>
        public IMaxSatSolver Solver;

        private static Random rg = new Random();

        public ARMS(IMaxSatSolver solver)
        {
            Solver = solver;
        }

        /// <summary>
        /// Learns model from given example plans.
        /// </summary>
        /// <param name="worlds"></param>
        /// <returns></returns>
        public Model Learn(Model originalModel, List<World> worlds)
        {
            if (worlds.Count == 0) throw new Exception("No worlds to learn from.");

            Model m = originalModel;

            List<World> currentWorlds = CopyWorlds(worlds);
            HashSet<string> satisfiedPredicates = new HashSet<string>();

            //preparting unfinished operators list
            Dictionary<string, Operator> unfinishedOperators = new Dictionary<string, Operator>();
            foreach (var pair in m.Operators)
            {
                unfinishedOperators.Add(pair.Key, pair.Value);
            }

            Random rg = new Random();
            double maxWeight = double.MinValue;
            while (unfinishedOperators.Count > 0)
            {
                Dictionary<string, double> operatorWeights = new Dictionary<string, double>();
                maxWeight = double.MinValue;

                //assigning random values to operators
                foreach (var pair in unfinishedOperators)
                {
                    double weight = rg.NextDouble() * (Properties.Settings.Default.ARMS_MaxActionConstraintWeight - Properties.Settings.Default.ARMS_MinActionConstraintWeight) + Properties.Settings.Default.ARMS_MinActionConstraintWeight;
                    if (weight > maxWeight) maxWeight = weight;
                    operatorWeights.Add(pair.Key, weight);
                }

                Formula formula = GenerateFormula(m, currentWorlds, unfinishedOperators, operatorWeights, satisfiedPredicates);
                Log("Created formula: ");
                Log(formula.ToString(true), false);

                Log("Solving formula...");
                bool[] solution = Solver.Solve(formula);

                if (Form1.Halt) return null;

                Log("Best solution found: " + formula.GetValue(solution).ToString(OutputDoubleFormat));
                Console.WriteLine(formula.GetPrintableString(solution));

                foreach (var pair in operatorWeights)
                {
                    if (pair.Value == maxWeight)
                    {
                        FinishOperator(unfinishedOperators[pair.Key], m, solution, formula);
                        Log("----------------------------------------------------------------------");
                        Log("finished operator: " + pair.Key);
                        Log(unfinishedOperators[pair.Key].ToString());
                        Log("----------------------------------------------------------------------");

                        SaveSatisfiedVariables(formula, pair.Key, satisfiedPredicates, solution);

                        unfinishedOperators.Remove(pair.Key);
                    }
                }

                UpdateWorlds(currentWorlds, unfinishedOperators);
            }

            Log("");
            Log(GeneralPlanningLibrary.Utility.Utility.CountErrors(m, worlds).ToString());

            Log(m.ToString(), true, OutputManager.SolutionTabName);

            return m;
        }

        /// <summary>
        /// Saves which variables were satisfied in previous formulas and are not present in next runs.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="operatorName"></param>
        /// <param name="satisfiedPredicates"></param>
        /// <param name="solution"></param>
        private void SaveSatisfiedVariables(Formula formula, string operatorName, HashSet<string> satisfiedPredicates, bool[] solution)
        {
            for (int i = 0; i < formula.VariableNames.Count; i++)
            {
                string name = formula.VariableNames[i];

                if (name.Substring(0, name.Length - 4).EndsWith(operatorName))
                {
                    if (solution[i])
                    {
                        satisfiedPredicates.Add(name);
                    }
                }
            }
        }

        /// <summary>
        /// Removes actions at begginings of plans that were already learned and updates initial states.
        /// </summary>
        /// <param name="worlds"></param>
        /// <param name="unfinishedOperators"></param>
        private void UpdateWorlds(List<World> worlds, Dictionary<string, Operator> unfinishedOperators)
        {
            foreach (var world in worlds)
            {
                int emptyPlans = 0;

                foreach (var plan in world.Plans)
                {
                    State s = plan.States[0];

                    int index;

                    for (index = 0; index < plan.Actions.Length; index++)
                    {
                        if (unfinishedOperators.ContainsKey(plan.Actions[index].Operator.Name)) break;

                        s = plan.Actions[index].ApplicateToState(s);
                    }

                    if (index == 0) continue;

                    State[] states = new State[plan.States.Length - index];
                    GeneralPlanningLibrary.Action[] actions = new GeneralPlanningLibrary.Action[plan.Actions.Length - index];

                    states[0] = s;
                    for (int i = 0; i < actions.Length; i++)
                    {
                        states[i + 1] = plan.States[i + index + 1];
                        actions[i] = plan.Actions[i + index];
                    }

                    plan.States = states;
                    plan.Actions = actions;

                    if (actions.Length == 0) emptyPlans++;
                }
                
                if (emptyPlans > 0)
                {
                    Plan[] newPlans = new Plan[world.Plans.Length - emptyPlans];

                    int index = 0;
                    for (int i = 0; i < world.Plans.Length; i++)
                    {
                        if (world.Plans[i].Actions.Length == 0) continue;

                        newPlans[index++] = world.Plans[i];
                    }

                    world.Plans = newPlans;
                }
            }
        }

        /// <summary>
        /// Copies given list of worlds.
        /// </summary>
        /// <param name="worlds"></param>
        /// <returns></returns>
        private List<World> CopyWorlds(List<World> worlds)
        {
            List<World> newWorlds = new List<World>();

            foreach (var world in worlds)
            {
                World w = new World();
                w.Model = world.Model;
                w.ModelName = world.ModelName;
                w.Objects = world.Objects;
                w.Name = world.Name;
                w.Plans = new Plan[world.Plans.Length];

                for (int i = 0; i < world.Plans.Length; i++)
                {
                    GeneralPlanningLibrary.Action[] actions = new GeneralPlanningLibrary.Action[world.Plans[i].Actions.Length];

                    for (int j = 0; j < actions.Length; j++)
                    {
                        actions[j] = world.Plans[i].Actions[j];
                    }

                    State[] states = new State[world.Plans[i].States.Length];

                    for (int j = 0; j < states.Length; j++)
                    {
                        if (world.Plans[i].States[j] == null) continue;

                        PredicateInstance[] predicates = new PredicateInstance[world.Plans[i].States[j].Predicates.Length];

                        for (int k = 0; k < predicates.Length; k++)
                        {
                            predicates[k] = world.Plans[i].States[j].Predicates[k];
                        }

                        states[j] = new State(predicates);
                    }

                    w.Plans[i] = new Plan(actions, states);
                }

                newWorlds.Add(w);
            }

            return newWorlds;
        }

        /// <summary>
        /// Decodes info from solved MAX-SAT formula and creates operator in correct format.
        /// </summary>
        /// <param name="oper">Operator to decode</param>
        /// <param name="m"></param>
        /// <param name="solution">Solution of MAX-SAT problem</param>
        /// <param name="formula">Original ormula representing MAX-SAT problem</param>
        private void FinishOperator(Operator oper, Model m, bool[] solution, Formula formula)
        {
            Console.WriteLine(oper.Name);

            List<PredicateReference> predicates = oper.GeneratePossiblePredicates(m.Predicates, ParametersCanRepeat);
            oper.Preconditions = new Tree(TreeType.And, new List<Tree>(), oper);
            oper.Effects = new Tree(TreeType.And, new List<Tree>(), oper);

            foreach (var predicate in predicates)
            {
                int addIndex = formula.VariableIndices[GetName(predicate, ListType.Add)];
                int preIndex = formula.VariableIndices[GetName(predicate, ListType.Precondition)];
                int delIndex = formula.VariableIndices[GetName(predicate, ListType.Delete)];

                if (solution[addIndex])
                {
                    oper.Effects.Children.Add(new Tree(predicate, oper));
                    Console.WriteLine("add: " + GetName(predicate, ListType.Add));
                }
                if (solution[preIndex])
                {
                    oper.Preconditions.Children.Add(new Tree(predicate, oper));
                    Console.WriteLine("pre: " + GetName(predicate, ListType.Precondition));
                }
                if (solution[delIndex])
                {
                    oper.Effects.Children.Add(new Tree(TreeType.Not, new Tree(predicate, oper), oper));
                }
            }

            if (oper.Preconditions.Children.Count == 1)
            {
                oper.Preconditions = oper.Preconditions.Children[0];
            }
            else if (oper.Preconditions.Children.Count == 0)
            {
                oper.Preconditions = null;
            }

            if (oper.Effects.Children.Count == 1)
            {
                oper.Effects = oper.Effects.Children[0];
            }
            else if (oper.Effects.Children.Count == 0)
            {
                oper.Effects = null;
            }
        }

        /// <summary>
        /// Generates formula for MAX-SAT algorithm from given model and worlds.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        /// <param name="unfinishedOperators">List of currently unfinished operators.</param>
        /// <param name="operatorWeights">Weights of operators</param>
        /// <returns></returns>
        private Formula GenerateFormula(Model m, List<World> worlds, Dictionary<string, Operator> unfinishedOperators, Dictionary<string, double> operatorWeights, HashSet<string> satisfiedPredicates)
        {
            Formula formula = new Formula();
            int planCount = 0;

            //Action constraints
            Dictionary<string, List<PredicateReference>> possiblePredicates = new Dictionary<string, List<PredicateReference>>();
            int id = 0;
            foreach (var pair in unfinishedOperators)
            {
                List<PredicateReference> predicates = pair.Value.GeneratePossiblePredicates(m.Predicates, ParametersCanRepeat);
                double weight = operatorWeights[pair.Key];
                possiblePredicates.Add(pair.Value.Name, predicates);

                foreach (PredicateReference p in predicates)
                {
                    int addIndex = formula.AddParameterOrGetIndex(GetName(p, ListType.Add), id * 3);
                    int predIndex = formula.AddParameterOrGetIndex(GetName(p, ListType.Precondition), id * 3 + 1);
                    int delIndex = formula.AddParameterOrGetIndex(GetName(p, ListType.Delete), id * 3 + 2);

                    formula.TryAddClause(new Clause(new int[] { addIndex, predIndex }, new bool[] { false, false }, formula, weight));
                    formula.TryAddClause(new Clause(new int[] { delIndex, predIndex }, new bool[] { false, true }, formula, weight));
                }

                id++;
            }

            Dictionary<string, int> PredicateActionPairAmount = new Dictionary<string, int>();
            Dictionary<string, int> ActionPredicatePairAmount = new Dictionary<string, int>();

            foreach (World w in worlds)
            {
                foreach (Plan p in w.Plans)
                {
                    planCount++;
                }
            }

            //Information constraints
            foreach (World w in worlds)
            {
                foreach (Plan p in w.Plans)
                {
                    if (p.Actions.Length == 0) continue;

                    HashSet<string> PredicateActionPairsInCurrentPlan = new HashSet<string>();
                    HashSet<string> ActionPredicatePairsInCurrentPlan = new HashSet<string>();

                    for (int i = 0; i < p.States.Length; i++)
                    {
                        if (p.States[i] == null) continue;

                        foreach (var predicate in p.States[i].Predicates)
                        {
                            List<int[]> parametrizations = new List<int[]>();
                            int j, index;

                            //finding closest preceeding action that shares parameters with the predicate
                            for (j = i - 1; j >= 0 && parametrizations.Count == 0; j--)
                            {
                                parametrizations = p.Actions[j].GetParametrization(predicate);
                            }
                            j++;

                            //the last action couldn't delete the predicate
                            if (unfinishedOperators.ContainsKey(p.Actions[j].Operator.Name))
                            {
                                foreach (var parametrization in parametrizations)
                                {
                                    PredicateReference pr = new PredicateReference(predicate.Type, parametrization, null, p.Actions[j].Operator);

                                    index = formula.AddParameterOrGetIndex(GetName(pr, ListType.Delete));
                                    formula.TryAddClause(new Clause(index, false, formula, Properties.Settings.Default.ARMS_InformationConstraintWeight), CollisionHandlingPolicy.MaxWeight);

                                    string s = GetName(pr, ListType.Add);
                                    if (!ActionPredicatePairsInCurrentPlan.Contains(s)) ActionPredicatePairsInCurrentPlan.Add(s);
                                }
                            }

                            //finding if the predicate was present in any state before
                            bool found = false;
                            for (int k = 0; k < i; k++)
                            {
                                if (p.States[k] != null && p.States[k].Predicates != null && p.States[k].Predicates.Contains(predicate))
                                {
                                    found = true;
                                    break;
                                }
                            }

                            //predicate was not present in any preceeding state -> some action must have add it
                            if (!found)
                            {
                                List<int> literals = new List<int>();
                                List<bool> values = new List<bool>();

                                for (int k = 0; k < i; k++)
                                {
                                    parametrizations = p.Actions[k].GetParametrization(predicate);

                                    if (parametrizations == null) continue;

                                    foreach (var parametrization in parametrizations)
                                    {
                                        PredicateReference pr = new PredicateReference(predicate.Type, parametrization, null, p.Actions[k].Operator);

                                        int l = formula.AddParameterOrGetIndex(GetName(pr, ListType.Add));

                                        if (!literals.Contains(l))
                                        {
                                            literals.Add(l);
                                            values.Add(true);
                                        }
                                    }
                                }

                                if (literals.Count > 0)
                                {
                                    formula.TryAddClause(new Clause(literals.ToArray(), values.ToArray(), formula, Properties.Settings.Default.ARMS_InformationConstraintWeight), CollisionHandlingPolicy.MaxWeight);
                                }
                            }


                            //finding closest succeeding action that shares parameters with the predicate
                            parametrizations.Clear();
                            for (j = i; j < p.Actions.Length && parametrizations.Count == 0; j++)
                            {
                                parametrizations = p.Actions[j].GetParametrization(predicate);
                            }
                            j--;

                            if (unfinishedOperators.ContainsKey(p.Actions[j].Operator.Name))
                            {
                                foreach (var parametrization in parametrizations)
                                {
                                    PredicateReference pr = new PredicateReference(predicate.Type, parametrization, null, p.Actions[j].Operator);

                                    string s = GetName(pr, ListType.Precondition);
                                    if (!PredicateActionPairsInCurrentPlan.Contains(s)) PredicateActionPairsInCurrentPlan.Add(s);
                                }
                            }
                        }
                    }

                    //Frequent action-predicate and predicate-action pairs.
                    foreach (var pair in PredicateActionPairsInCurrentPlan)
                    {
                        if (!PredicateActionPairAmount.ContainsKey(pair)) PredicateActionPairAmount.Add(pair, 1);
                        else PredicateActionPairAmount[pair]++;
                    }

                    foreach (var pair in ActionPredicatePairsInCurrentPlan)
                    {
                        if (!ActionPredicatePairAmount.ContainsKey(pair)) ActionPredicatePairAmount.Add(pair, 1);
                        else ActionPredicatePairAmount[pair]++;
                    }
                }
            }

            foreach (var pair in PredicateActionPairAmount)
            {
                if (pair.Value >= planCount * Properties.Settings.Default.ARMS_ProbabilityTreshold)
                {
                    int index = formula.AddParameterOrGetIndex(pair.Key);
                    Clause clause = new Clause(index, true, formula, (double)pair.Value / planCount);
                    formula.TryAddClause(clause, CollisionHandlingPolicy.MaxWeight);
                }
            }

            foreach (var pair in ActionPredicatePairAmount)
            {
                if (pair.Value >= planCount * Properties.Settings.Default.ARMS_ProbabilityTreshold)
                {
                    int index = formula.AddParameterOrGetIndex(pair.Key);
                    Clause clause = new Clause(index, true, formula, (double)pair.Value / planCount);
                    formula.TryAddClause(clause, CollisionHandlingPolicy.MaxWeight);
                }
            }

            //Frequent pairs - plan constraints
            List<ActionPair> frequentPairs = FindFrequentActionPairs(m, worlds);

            GeneratePlanConstraints(formula, frequentPairs, m, planCount, unfinishedOperators, satisfiedPredicates);
            
            return formula;
        }

        /// <summary>
        /// Generates plan contraints for given frequent actions pairs.
        /// </summary>
        /// <param name="formula">Formula to add clauses to</param>
        /// <param name="frequentPairs"></param>
        /// <param name="satisfiedPredicates">Set storing which formula's variables were satisfied in previous runs.</param>
        private void GeneratePlanConstraints(Formula formula, List<ActionPair> frequentPairs, Model m, int planCount, Dictionary<string, Operator> unfinishedOperators, HashSet<string> satisfiedPredicates) 
        {
            foreach (var pair in frequentPairs)
            {
                //Both operators need to be learned
                if (unfinishedOperators.ContainsKey(pair.FirstOperator.Name) && unfinishedOperators.ContainsKey(pair.SecondOperator.Name)) 
                {
                    var list = pair.GeneratePossiblePredicates(m.Predicates);
                    if (list.Item1.Count == 0) continue;

                    MultiClause multiclause = new MultiClause(formula, (double)pair.SupportRate / planCount);

                    for (int i = 0; i < list.Item1.Count; i++)
                    {
                        int preIndexFirst = formula.AddParameterOrGetIndex(GetName(list.Item1[i], ListType.Precondition));
                        int addIndexFirst = formula.AddParameterOrGetIndex(GetName(list.Item1[i], ListType.Add));
                        int delIndexFirst = formula.AddParameterOrGetIndex(GetName(list.Item1[i], ListType.Delete));

                        int preIndexSecond = formula.AddParameterOrGetIndex(GetName(list.Item2[i], ListType.Precondition));
                        int addIndexSecond = formula.AddParameterOrGetIndex(GetName(list.Item2[i], ListType.Add));

                        multiclause.AddMultiliteral(new int[] { preIndexFirst, preIndexSecond, delIndexFirst }, new bool[] { true, true, false });
                        multiclause.AddMultiliteral(new int[] { addIndexFirst, preIndexSecond }, new bool[] { true, true });
                        multiclause.AddMultiliteral(new int[] { delIndexFirst, addIndexSecond }, new bool[] { true, true });
                    }

                    formula.Multiclauses.Add(multiclause);
                }
                //only the first operator needs to be learnt
                else if (unfinishedOperators.ContainsKey(pair.FirstOperator.Name) && !unfinishedOperators.ContainsKey(pair.SecondOperator.Name))
                {
                    var list = pair.GeneratePossiblePredicates(m.Predicates);
                    if (list.Item1.Count == 0) continue;

                    MultiClause multiclause = new MultiClause(formula, (double)pair.SupportRate / planCount);

                    for (int i = 0; i < list.Item1.Count; i++)
                    {
                        int preIndexFirst = formula.AddParameterOrGetIndex(GetName(list.Item1[i], ListType.Precondition));
                        int addIndexFirst = formula.AddParameterOrGetIndex(GetName(list.Item1[i], ListType.Add));
                        int delIndexFirst = formula.AddParameterOrGetIndex(GetName(list.Item1[i], ListType.Delete));

                        bool preSecondTrue = satisfiedPredicates.Contains(GetName(list.Item2[i], ListType.Precondition));
                        bool addSecondTrue = satisfiedPredicates.Contains(GetName(list.Item2[i], ListType.Add));

                        if (preSecondTrue) multiclause.AddMultiliteral(new int[] { preIndexFirst, delIndexFirst }, new bool[] { true, false });
                        if (preSecondTrue) multiclause.AddMultiliteral(new int[] { addIndexFirst }, new bool[] { true });
                        if (addSecondTrue) multiclause.AddMultiliteral(new int[] { delIndexFirst }, new bool[] { true });
                    }

                    if (multiclause.Literals.Count > 0)
                    {
                        formula.Multiclauses.Add(multiclause);
                    }
                }
                //only the second operator need to be learnt
                else if (!unfinishedOperators.ContainsKey(pair.FirstOperator.Name) && unfinishedOperators.ContainsKey(pair.SecondOperator.Name))
                {
                    var list = pair.GeneratePossiblePredicates(m.Predicates);
                    if (list.Item1.Count == 0) continue;

                    MultiClause multiclause = new MultiClause(formula, (double)pair.SupportRate / planCount);

                    for (int i = 0; i < list.Item1.Count; i++)
                    {
                        bool preFirstTrue = satisfiedPredicates.Contains(GetName(list.Item1[i], ListType.Precondition));
                        bool addFirstTrue = satisfiedPredicates.Contains(GetName(list.Item1[i], ListType.Add));
                        bool delFirstTrue = satisfiedPredicates.Contains(GetName(list.Item1[i], ListType.Delete));

                        int preIndexSecond = formula.AddParameterOrGetIndex(GetName(list.Item2[i], ListType.Precondition));
                        int addIndexSecond = formula.AddParameterOrGetIndex(GetName(list.Item2[i], ListType.Add));

                        if (preFirstTrue && !delFirstTrue) multiclause.AddMultiliteral(new int[] { preIndexSecond }, new bool[] { true });
                        if (addFirstTrue) multiclause.AddMultiliteral(new int[] { preIndexSecond }, new bool[] { true });
                        if (delFirstTrue) multiclause.AddMultiliteral(new int[] { addIndexSecond }, new bool[] { true });
                    }
                    if (multiclause.Literals.Count > 0)
                    {
                        formula.Multiclauses.Add(multiclause);
                    }
                }
            }


        }

        /// <summary>
        /// Generates a set of pairs of actions which occur often in given plans.
        /// </summary>
        /// <param name="m">Input model</param>
        /// <param name="worlds">List of worlds to learn from.</param>
        /// <returns></returns>
        public List<ActionPair> FindFrequentActionPairs(Model m, List<World> worlds)
        {
            Dictionary<string, ActionPair> pairs = new Dictionary<string, ActionPair>();

            List<ActionPair> currentActionPairs;

            int planCount = 0;
            foreach (var w in worlds)
            {
                foreach (var p in w.Plans)
                {
                    planCount++;

                    Dictionary<string, ActionPair> pairsInPlan = new Dictionary<string, ActionPair>();

                    for (int i = 0; i < p.Actions.Length; i++)
                    {
                        for (int j = i; j < p.Actions.Length; j++)
                        {
                            currentActionPairs = GenerateAllConnectedPairs(p.Actions[i], p.Actions[j]);
                            if (currentActionPairs == null) continue;

                            currentActionPairs = RemoveSubsets(currentActionPairs);
                            foreach (var pair in currentActionPairs)
                            {
                                string name = pair.GetName();

                                if (!pairsInPlan.ContainsKey(name))
                                {
                                    pairsInPlan.Add(name, pair);
                                }
                            }
                        }
                    }

                    foreach (var tuple in pairsInPlan)
                    {
                        ActionPair pair;

                        if (pairs.TryGetValue(tuple.Key, out pair))
                        {
                            pair.SupportRate++;
                        }
                        else
                        {
                            pairs.Add(tuple.Key, tuple.Value);
                        }
                    }
                }
            }

            List<ActionPair> frequentPairs = new List<ActionPair>();
            foreach (var tuple in pairs)
            {
                if (tuple.Value.SupportRate > planCount * Properties.Settings.Default.ARMS_ProbabilityTreshold)
                {
                    frequentPairs.Add(tuple.Value);
                    //Log(tuple.Key + "    " + tuple.Value.SupportRate);
                }
            }

            return frequentPairs;
        }

        /// <summary>
        /// Removes redundant pairs in given pair list.
        /// (removes those pairs which connectors are "subsets" of other pairs)
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        private List<ActionPair> RemoveSubsets(List<ActionPair> pairs)
        {
            if (pairs.Count == 0) return new List<ActionPair>();

            int length = pairs[0].Connector.Length;
            List<ActionPair> newPairs = new List<ActionPair>();

            for (int i = 0; i < pairs.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < pairs.Count && !found; j++)
                {
                    if (i == j) continue;

                    //trying to find out if [i].Connector is subset of [j].Connector
                    bool subset = true;
                    for (int k = 0; k < length && subset; k++)
                    {
                        if (pairs[i].Connector[k] >= 0 && pairs[i].Connector[k] != pairs[j].Connector[k]) subset = false;
                    }

                    if (subset) found = true;
                }

                if (!found) newPairs.Add(pairs[i]);
            }


            return newPairs;
        }

        /// <summary>
        /// Generates all possible connections of 2 actions.
        /// </summary>
        /// <param name="firstAction"></param>
        /// <param name="secondAction"></param>
        /// <returns></returns>
        private List<ActionPair> GenerateAllConnectedPairs(GeneralPlanningLibrary.Action firstAction, GeneralPlanningLibrary.Action secondAction)
        {
            if (firstAction.Operator == secondAction.Operator) return null;

            int[] connector = new int[firstAction.Parameters.Length];
            bool[] usedParameters = new bool[secondAction.Parameters.Length];

            for (int i = 0; i < connector.Length; i++) connector[i] = -1;

            List<ActionPair> pairs = new List<ActionPair>();

            GenerateConnectedPairsRecursively(firstAction, secondAction, 0, connector, 0, usedParameters, pairs);

            return pairs;
        }

        /// <summary>
        /// Generates all possible connections of 2 actions.
        /// </summary>
        /// <param name="firstAction"></param>
        /// <param name="secondAction"></param>
        /// <param name="index">Index of current parameter of first action</param>
        /// <param name="connector">Connector generated so far</param>
        /// <param name="usedCount">Number of generated connections</param>
        /// <param name="usedParameters">Represents which parameters of second actions are already used.</param>
        /// <param name="finishedPairs">List of already finished connections.</param>
        private void GenerateConnectedPairsRecursively(GeneralPlanningLibrary.Action firstAction, GeneralPlanningLibrary.Action secondAction, int index, int[] connector, int usedCount, bool[] usedParameters, List<ActionPair> finishedPairs)
        {
            if (index == firstAction.Parameters.Length)
            {
                if (usedCount == 0) return;

                finishedPairs.Add(new ActionPair(firstAction.Operator, secondAction.Operator, (int[])connector.Clone()));
                return;
            }

            for (int i = 0; i < secondAction.Parameters.Length; i++)
            {
                if (usedParameters[i]) continue;

                if (firstAction.Parameters[index] == secondAction.Parameters[i])
                {
                    connector[index] = i;
                    usedParameters[i] = true;

                    GenerateConnectedPairsRecursively(firstAction, secondAction, index + 1, connector, usedCount + 1, usedParameters, finishedPairs);

                    usedParameters[i] = false;
                    connector[index] = -1;
                }
            }

            GenerateConnectedPairsRecursively(firstAction, secondAction, index + 1, connector, usedCount, usedParameters, finishedPairs);
        }

        /// <summary>
        /// Generates unique name for "predicate-operator's list" pair
        /// </summary>
        /// <param name="predicate">Reference of predicate to operator</param>
        /// <param name="type">Type of operator's list the predicate is in</param>
        /// <returns></returns>
        private string GetName(PredicateReference predicate, ListType type)
        {
            string s = predicate.ToString() + "->" + predicate.Operator.Name + ".";
            if (type == ListType.Precondition) s += "pre";
            else if (type == ListType.Add) s += "add";
            else s += "del";
            return s;
        }

        enum ListType
        {
            Precondition, 
            Add, 
            Delete
        }

        public static void Log(string s, bool toConsole = true, string tabName = OutputManager.OutputTabName)
        {
            if (toConsole) Console.WriteLine(s);
            Form1.Manager.WriteLine(s, tabName);
        }
    }
}
