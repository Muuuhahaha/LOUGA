using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;


namespace ModelLearner.Algorithms.LOCM2
{
    /// <summary>
    /// Machine that learns model by performing LOCM2 algorithm on input data.
    /// LOCM2 creates multiple finite state automata for each object type describing object's behavior from different viewpoints.
    /// </summary>
    class LOCM2Machine : ILearningAlgorithm
    {
        /// <summary>
        /// Learns model description from input worlds.
        /// </summary>
        /// <param name="originalModel"></param>
        /// <param name="worlds"></param>
        /// <returns></returns>
        public Model Learn(Model originalModel, List<World> worlds)
        {
            return Learn(originalModel, worlds, true);
        }

        /// <summary>
        /// Learns model's description from input worlds.
        /// </summary>
        /// <param name="originalModel">model to complete</param>
        /// <param name="worlds">worlds to learn from</param>
        /// <param name="overridePredicates">TRUE if existing predicates should be deleted</param>
        /// <returns></returns>
        public Model Learn(Model originalModel, List<World> worlds, bool overridePredicates = true)
        {
            if (worlds.Count == 0) throw new Exception("No worlds to learn from.");

            Output("Making matrices...");
            Dictionary<string, TransitionMatrix> matrices = MakeMatrices(originalModel, worlds);
            if (Form1.Halt) return null;

            Output("\nTesting hypotheses...");
            TestHypotheses(matrices, originalModel, worlds);
            if (Form1.Halt) return null;

            Output("\nInducing parameters...");
            InduceParameters(matrices);
            if (Form1.Halt) return null;

            Output("\nFinishing operators...");
            FinishOperators(matrices, originalModel);
            if (Form1.Halt) return null;

            Output(originalModel.ToString(), OutputManager.SolutionTabName);

            return originalModel;
        }
        
        /// <summary>
        /// Makes matrices of transitions.
        /// </summary>
        /// <param name="m">input model</param>
        /// <param name="worlds">worlds to learn from</param>
        /// <returns></returns>
        private Dictionary<string, TransitionMatrix> MakeMatrices(Model m, List<World> worlds)
        {
            Dictionary<string, TransitionMatrix> matrices = new Dictionary<string,TransitionMatrix>();

            foreach(var t in m.Types)
            {
                if (Form1.Halt) return null;

                TransitionMatrix matrix = new TransitionMatrix(t.Value);

                //forming matrix
                matrix.FormMatrix(worlds, m);
                Output(matrix.ToString());

                //finding holess
                matrix.FindHoles();
                foreach (var hole in matrix.Holes)
                {
                    Output("    - hole: " + matrix.Transitions[hole.Item1].Name + " -> " + matrix.Transitions[hole.Item2].Name);
                }

                //creating transition sets
                Output("    forming transition sets");
                matrix.FormTransitionSets(worlds);

                if (Form1.Halt) return null;

                if (matrix.TransitionSets.Count == 1)
                {
                    Output("    formed 1 transition set");
                }
                else
                {
                    Output("    formed " + matrix.TransitionSets.Count + " transitions sets");
                }

                //outputing transition sets' states
                for (int j = 0; j < matrix.TransitionSets.Count; j++)
                {
                    Output("      set " + j + ": ");
                    for (int k = 0; k < matrix.TransitionSets[j].StatesCount; k++)
                    {
                        for (int i = 0; i < matrix.TransitionSets[j].Transitions.Length; i++)
                        {
                            if (!matrix.TransitionSets[j].Transitions[i]) continue;

                            if (matrix.TransitionSets[j].PrecedingState[i] == k)
                            {
                                Output("        state " + k + ": before " + matrix.Transitions[i].Name);
                            }
                            if (matrix.TransitionSets[j].NextState[i] == k)
                            {
                                Output("        state " + k + ": after  " + matrix.Transitions[i].Name);
                            }
                        }
                    }
                }

                matrices.Add(t.Key, matrix);
                Output();
            }

            return matrices;
        }

        /// <summary>
        /// Tests hypotheses about states' parameters.
        /// </summary>
        /// <param name="matrices">Matrices whose hypotheses should be tested.</param>
        /// <param name="originalModel">Input model</param>
        /// <param name="worlds">worlds to test hypotheses on</param>
        private void TestHypotheses(Dictionary<string, TransitionMatrix> matrices, Model originalModel, List<World> worlds)
        {
            foreach (World w in worlds)
            {
                if (w.Model != originalModel) continue;

                foreach (var pair in w.Objects.Concat(originalModel.Constants))
                {
                    Console.WriteLine(pair.Value.Name);
                    foreach (Plan plan in w.Plans)
                    {
                        List<GeneralPlanningLibrary.Action> subsequence = GetSubsequenceByObject(pair.Value, plan.Actions);
                        TransitionMatrix matrix = matrices[pair.Value.Type.Name];

                        matrix.TestHypotheses(subsequence, pair.Value);
                        
                        if (Form1.Halt) return;
                    }
                }
            }
        }

        /// <summary>
        /// Returns list of action from given plan that have given object as a parameter.   
        /// </summary>
        /// <param name="o">Object to look for</param>
        /// <param name="plan">Plan from which subsequence should be picked.</param>
        /// <returns></returns>
        private List<GeneralPlanningLibrary.Action> GetSubsequenceByObject(GeneralPlanningLibrary.Object o, GeneralPlanningLibrary.Action[] plan)
        {
            List<GeneralPlanningLibrary.Action> subsequence = new List<GeneralPlanningLibrary.Action>();

            for (int i = 0; i < plan.Length; i++)
            {
                for (int j = 0; j < plan[i].Parameters.Length; j++)
                {
                    if (plan[i].Parameters[j] == o)
                    {
                        subsequence.Add(plan[i]);
                        break;
                    }
                }
            }

            return subsequence;
        }

        /// <summary>
        /// Creates new predicates for each type describing object's states and updates operators' precondition and effect lists accorting to matrices' transition sets.
        /// </summary>
        /// <param name="matrices">Matrices for each type.</param>
        /// <param name="m">Model to update</param>
        /// <param name="worlds"></param>
        private void FinishOperators(Dictionary<string, TransitionMatrix> matrices, Model m)
        {
            Dictionary<string, Predicate> newPredicates = new Dictionary<string, Predicate>();

            //creating new predicates
            foreach (var matrix in matrices)
            {
                for (int i = 0; i < matrix.Value.TransitionSets.Count; i++)
                {
                    TransitionSet ts = matrix.Value.TransitionSets[i];
                    for (int j = 0; j < ts.StatesCount; j++)
                    {
                        Predicate p = new Predicate();
                        p.Name = matrix.Key + "_state_" + i + "_" + j;

                        p.ParameterTypes = new GeneralPlanningLibrary.Type[1 + ts.ParametersInfo[j].Count][];
                        p.ParameterNames = new string[1 + ts.ParametersInfo[j].Count];

                        p.ParameterTypes[0] = new GeneralPlanningLibrary.Type[] { matrix.Value.Type };
                        p.ParameterNames[0] = "?" + matrix.Key;
                        for (int k = 0; k < ts.ParametersInfo[j].Count; k++)
                        {
                            p.ParameterTypes[k + 1] = new GeneralPlanningLibrary.Type[] { ts.ParametersInfo[j][k].ParameterType };
                            p.ParameterNames[k + 1] = "?" + ts.ParametersInfo[j][k].ParameterType.Name + (k + 1);
                        }

                        newPredicates.Add(p.Name, p);
                        Output("  added predicate " + p.ToString());
                    }
                }
            }

            m.Predicates = newPredicates;

            Output();

            //updating operators' precondition and effect lists
            foreach (var oper in m.Operators)
            {
                oper.Value.Preconditions = new Tree(TreeType.And, new List<Tree>(), oper.Value);
                oper.Value.Effects = new Tree(TreeType.And, new List<Tree>(), oper.Value);

                for (int i = 0; i < oper.Value.ParameterCount; i++)
                {
                    GeneralPlanningLibrary.Type t = oper.Value.ParameterTypes[i][0];
                    TransitionMatrix tm = matrices[t.Name];
                    string transitionName = oper.Key + "." + i;
                    int transitionId = tm.TransitionIndices[transitionName];

                    //creating state change for each involved transition set
                    for (int j = 0; j < tm.TransitionSets.Count; j++)
                    {
                        if (!tm.TransitionSets[j].Transitions[transitionId]) continue;

                        int current = tm.TransitionSets[j].PrecedingState[transitionId];
                        int next = tm.TransitionSets[j].NextState[transitionId];

                        if (current == -1 || next == -1)
                        {
                            throw new Exception("Not enough usage examples for type " + tm.Type.Name);
                        }

                        //giving parameters to states
                        List<ParameterInfo>[] stateParameters = tm.TransitionSets[j].ParametersInfo;
                        int[] parametrizationCurrent = new int[1 + stateParameters[current].Count];
                        int[] parametrizationNext = new int[1 + stateParameters[next].Count];

                        parametrizationCurrent[0] = i;
                        parametrizationNext[0] = i;
                        bool parameterChange = false;
                        for (int k = 0; k < stateParameters[current].Count; k++)
                        {
                            int index = -1;
                            foreach (var item in stateParameters[current][k].AsociatedTransitions)
                            {
                                if (item.Operator == oper.Value && item.TransitionType == TransitionType.BeforeAction)
                                {
                                    index = item.ParameterId;
                                    break;
                                }
                            }
                            parametrizationCurrent[k + 1] = index;
                        }
                        for (int k = 0; k < stateParameters[next].Count; k++)
                        {
                            int index = -1;
                            foreach (var item in stateParameters[next][k].AsociatedTransitions)
                            {
                                if (item.Operator == oper.Value && item.TransitionType == TransitionType.AfterAction)
                                {
                                    index = item.ParameterId;
                                    break;
                                }
                            }
                            parametrizationNext[k + 1] = index;

                            if (current == next && parametrizationCurrent[k + 1] != parametrizationNext[k + 1])
                            {
                                parameterChange = true;
                            }
                        }

                        //creating and assigning predicate references to operator's lists
                        Predicate pcurrent = newPredicates[t.Name + "_state_" + j + "_" + current];
                        Predicate pnext = newPredicates[t.Name + "_state_" + j + "_" + next];
                        
                        PredicateReference prcurrent = new PredicateReference(pcurrent, parametrizationCurrent, null, oper.Value);
                        PredicateReference prnext = new PredicateReference(pnext, parametrizationNext, null, oper.Value);

                        oper.Value.Preconditions.Children.Add(new Tree(prcurrent, oper.Value));

                        if (current != next || parameterChange)
                        {
                            oper.Value.Effects.Children.Add(new Tree(TreeType.Not, new Tree(prcurrent, oper.Value), oper.Value));
                            oper.Value.Effects.Children.Add(new Tree(prnext, oper.Value));
                        }
                    }
                }

                //polishing tree formating
                if (oper.Value.Preconditions.Children.Count == 0) oper.Value.Preconditions = null;
                else if (oper.Value.Preconditions.Children.Count == 1) oper.Value.Preconditions = oper.Value.Preconditions.Children[0];
                if (oper.Value.Effects.Children.Count == 0) oper.Value.Effects = null;
                else if (oper.Value.Effects.Children.Count == 1) oper.Value.Effects = oper.Value.Effects.Children[0];

                Output("  finished operator '" + oper.Key + "'");
            }
        }

        /// <summary>
        /// Induces parameters from tested hypotheses
        /// </summary>
        /// <param name="matrices">Matrices whose states' paremeters should be induced</param>
        private void InduceParameters(Dictionary<string, TransitionMatrix> matrices)
        {
            foreach (var matrix in matrices)
            {
                matrix.Value.InduceParameters();
                if (Form1.Halt) return;

                for (int i = 0; i < matrix.Value.TransitionSets.Count; i++)
                {
                    if (matrix.Value.TransitionSets[i].StatesCount < 2 && matrix.Value.TransitionSets[i].ParametersInfo[0].Count == 0)
                    {
                        matrix.Value.TransitionSets.RemoveAt(i);
                        i--;
                    }

                    if (Form1.Halt) return;
                }
            }
        }

        /// <summary>
        /// Output to user
        /// </summary>
        /// <param name="text"></param>
        /// <param name="outputTab"></param>
        private void Output(string text = "", string outputTab = OutputManager.OutputTabName)
        {
            Form1.Manager.WriteLine(text, outputTab);
            Console.WriteLine(text);
        }
    }
}
