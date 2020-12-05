using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM
{
    /// <summary>
    /// Machine performing LOCM algorithm on input data.
    /// LOCM algorithm creates 
    /// </summary>
    public class LOCMMachine : ILearningAlgorithm
    {
        public bool NeedsTyping
        {
            get { return true; }
        }

        /// <summary>
        /// Learns finite state automata for each type from input plans.
        /// </summary>
        /// <param name="originalModel">Model to complete</param>
        /// <param name="worlds">List of worlds to learn from</param>
        /// <returns></returns>
        public Model Learn(Model originalModel, List<World> worlds)
        {
            return Learn(originalModel, worlds, true);
        }

        /// <summary>
        /// Learns finite state automata for each type from input plans.
        /// </summary>
        /// <param name="originalModel">Model to complete</param>
        /// <param name="worlds">List of worlds to learn from</param>
        /// <param name="overridePredicates">describes whether the algorithm should delete existing predicate types.</param>
        /// <returns></returns>
        public Model Learn(Model originalModel, List<World> worlds, bool overridePredicates)
        {
            if (worlds.Count == 0) throw new Exception("No worlds to learn from.");

            Dictionary<string, PartialStateMachine> machines = MakeMachines(originalModel, originalModel, worlds);
            if (Form1.Halt) return null;

            Output("Making hypotheses about parameters...");
            foreach (var pair in machines)
            {
                pair.Value.MakeHypotheses();

                if (Form1.Halt) return null;

                Console.WriteLine(pair.Value.Hypotheses.Count);
            }

            Output("\nTesting hypotheses...");
            TestHypotheses(originalModel, originalModel, worlds, machines);
            if (Form1.Halt) return null;

            Output("\nInducing parameters...");
            InduceParameters(machines);
            if (Form1.Halt) return null;

            Output("\nFinishing operators...");
            FinishOperators(originalModel, machines, overridePredicates);

            Output(originalModel.ToString(), OutputManager.SolutionTabName);
            return originalModel;
        }

        /// <summary>
        /// Makes simple finite state machines for each type from input plans.
        /// </summary>
        /// <param name="m">Model to put machines in</param>
        /// <param name="originalModel">Original model</param>
        /// <param name="worlds">Worlds to learn from</param>
        /// <returns>Set of state machines for each type.</returns>
        private Dictionary<string, PartialStateMachine> MakeMachines(Model m, Model originalModel, List<World> worlds)
        {
            Dictionary<string, PartialStateMachine> machines = new Dictionary<string, PartialStateMachine>();
            foreach (var pair in m.Types)
            {
                int transitionsCount = 0;
                Dictionary<string, int> transitionIndices = new Dictionary<string, int>();

                Output("Transitions for type '" + pair.Key + "'");
                List<TransitionInfo> transitions = new List<TransitionInfo>();

                //creating transitions for each operator
                foreach (var oper in m.Operators)
                {
                    for (int i = 0; i < oper.Value.ParameterTypes.Length; i++)
                    {
                        for (int j = 0; j < oper.Value.ParameterTypes[i].Length; j++)
                        {
                            if (oper.Value.ParameterTypes[i][j].IsFatherOf(pair.Value))
                            {
                                transitionIndices.Add(oper.Value.Name + "-" + i, transitionsCount);
                                transitionsCount++;
                                
                                transitions.Add(new TransitionInfo(oper.Value, i, TransitionType.AfterAction));
                                transitions.Add(new TransitionInfo(oper.Value, i, TransitionType.BeforeAction));

                                Output("  " + oper.Value.Name + "-" + i);
                                if (Form1.Halt) return null;
                            }
                        }
                    }
                }

                machines.Add(pair.Key, new PartialStateMachine(pair.Value, transitionsCount, transitionIndices, transitions));
            }

            Output();
            Output("Making machines...");

            //parsing info from input plans
            foreach (World w in worlds)
            {
                if (w.Model != originalModel) continue;

                foreach (var pair in w.Objects.Concat(m.Constants))
                {
                    foreach (Plan plan in w.Plans)
                    {
                        List<GeneralPlanningLibrary.Action> subsequence = GetSubsequenceByObject(pair.Value, plan.Actions);
                        PartialStateMachine machine;

                        machines.TryGetValue(pair.Value.Type.Name, out machine);

                        //updating machines
                        machine.Update(subsequence, pair.Value);

                        if (Form1.Halt) return null;
                    }
                }
            }

            foreach (var pair in machines)
            {
                Output("  Machine for type '" + pair.Key + "' has " + pair.Value.StateCount + " states.");
            }
            Output();

            return machines;
        }

        /// <summary>
        /// Tests hypotheses about parameters of states of partial state machines.
        /// </summary>
        /// <param name="m">model that is being created</param>
        /// <param name="originalModel">original model</param>
        /// <param name="worlds">worlds to test hypotheses on</param>
        /// <param name="machines">partial state machines for each type.</param>
        private void TestHypotheses(Model m, Model originalModel, List<World> worlds, Dictionary<string, PartialStateMachine> machines)
        {
            //Testing hypotheses on each object in worlds.
            foreach (World w in worlds)
            {
                if (w.Model != originalModel) continue;

                foreach (var pair in w.Objects.Concat(m.Constants))
                {
                    PartialStateMachine machine;
                    machines.TryGetValue(pair.Value.Type.Name, out machine);

                    foreach (Plan plan in w.Plans)
                    {
                        List<GeneralPlanningLibrary.Action> subsequence = GetSubsequenceByObject(pair.Value, plan.Actions);

                        if (subsequence.Count == 0) continue;

                        //Testing hypotheses
                        machine.TestHypotheses(subsequence, pair.Value);

                        if (Form1.Halt) return;
                    }
                }
            }
        }

        /// <summary>
        /// Inducing parameters from tested hypotheses.
        /// </summary>
        /// <param name="machines">Machines for each type to update</param>
        private void InduceParameters(Dictionary<string, PartialStateMachine> machines)
        {
            foreach (var machinePair in machines)
            {
                PartialStateMachine machine = machinePair.Value;

                machine.InduceParameters();
            }

        }

        /// <summary>
        /// Creates new predicates for each type describing object's states and updates operators' precondition and effect lists accordingly.
        /// </summary>
        /// <param name="m">Model to update</param>
        /// <param name="machines">Machines describing objects' behavior</param>
        /// <param name="overridePredicates">Describes whether method should delete existing predicates.</param>
        private void FinishOperators(Model m, Dictionary<string, PartialStateMachine> machines, bool overridePredicates)
        {
            //create new predicates describing objects' types
            Dictionary<string, Predicate> newPredicates = new Dictionary<string, Predicate>();
            foreach (var pair in m.Types)
            {
                GeneralPlanningLibrary.Type t = pair.Value;
                PartialStateMachine machine;
                machines.TryGetValue(pair.Key, out machine);

                //New predicate for each machine state
                for (int i = 0; i < machine.StateCount; i++)
                {
                    Predicate p = new Predicate();
                    p.Name = pair.Key + "_state" + i;
                    p.ParameterTypes = new GeneralPlanningLibrary.Type[machine.ParametersInfo[i].Count + 1][];
                    p.ParameterNames = new string[p.ParameterTypes.Length];

                    p.ParameterTypes[0] = new GeneralPlanningLibrary.Type[] { t };
                    p.ParameterNames[0] = "?" + t.Name;

                    for (int j = 0; j < p.ParameterTypes.Length - 1; j++)
                    {
                        p.ParameterTypes[j + 1] = new GeneralPlanningLibrary.Type[] { machine.ParametersInfo[i][j].ParameterType };
                        p.ParameterNames[j + 1] = "?param" + (j + 1) + "_" + p.ParameterTypes[j + 1][0].Name;
                    }

                    newPredicates.Add(p.Name, p);

                    Output("  added predicate: '" + p.ToString() + "'");
                }
            }
            Output();

            if (overridePredicates)
            {
                m.Predicates = newPredicates;
            }
            else
            {
                foreach (var pair in newPredicates)
	            {
                    if (!m.Predicates.ContainsKey(pair.Key)) 
                    {
                        m.Predicates.Add(pair.Key, pair.Value);
                    }
	            }
            }

            //updating model's operators
            foreach (var pair in m.Operators)
            {
                Operator oper = pair.Value;
                oper.Preconditions = new Tree(TreeType.And, new List<Tree>(), oper);
                oper.Effects = new Tree(TreeType.And, new List<Tree>(), oper);

                //changing state for each operator's parameters
                for (int i = 0; i < oper.ParameterCount; i++)
                {
                    GeneralPlanningLibrary.Type t = oper.ParameterTypes[i][0];
                    PartialStateMachine machine;
                    machines.TryGetValue(t.Name, out machine);

                    int[] states = machine.GetOperatorsState(oper, i);

                    //finding predicates describing correct states
                    Predicate predicateBefore;
                    Predicate predicateAfter;
                    if (!newPredicates.TryGetValue(t.Name + "_state" + states[0], out predicateBefore) ||
                        !newPredicates.TryGetValue(t.Name + "_state" + states[1], out predicateAfter))
                    {
                        throw new Exception("Object of type " + t.Name + " was never used as parameter nr. " + (i + 1) + " in operator " + oper.Name + ".");
                    }

                    int[] parametersBefore = new int[predicateBefore.ParameterNames.Length];
                    int[] parametersAfter = new int[predicateAfter.ParameterNames.Length];
                    parametersAfter[0] = i;
                    parametersBefore[0] = i;

                    string transitionNameBefore = new TransitionInfo(oper, i, TransitionType.BeforeAction).GetName();
                    string transitionNameAfter = new TransitionInfo(oper, i, TransitionType.AfterAction).GetName();

                    //state parameters before action is performed
                    for (int j = 0; j < parametersBefore.Length - 1; j++)
                    {
                        foreach (string s in machine.ParametersInfo[states[0]][j].AsociatedTransitions)
                        {
                            if (s.StartsWith(transitionNameBefore))
                            {
                                parametersBefore[j + 1] = Int32.Parse(s.Substring(transitionNameBefore.Length + 1));
                                break;
                            }
                        }
                    }

                    //state parameters after action is performed
                    for (int j = 0; j < parametersAfter.Length - 1; j++)
                    {
                        foreach (string s in machine.ParametersInfo[states[1]][j].AsociatedTransitions)
                        {
                            if (s.StartsWith(transitionNameAfter))
                            {
                                parametersAfter[j + 1] = Int32.Parse(s.Substring(transitionNameAfter.Length + 1));
                                break;
                            }
                        }
                    }

                    //adding predicate references to operator's trees
                    PredicateReference prBefore = new PredicateReference(predicateBefore, parametersBefore, null, oper);
                    PredicateReference prAfter = new PredicateReference(predicateAfter, parametersAfter, null, oper);

                    Tree predicateRemoveLastState = new Tree(TreeType.Not, new Tree(prBefore, oper), oper);
                    Tree predicateAddNewState = new Tree(prAfter, oper);

                    oper.Preconditions.Children.Add(new Tree(prBefore, oper));
                    if (!prBefore.IsEqualTo(prAfter))
                    {
                        oper.Effects.Children.Add(predicateRemoveLastState);
                        oper.Effects.Children.Add(predicateAddNewState);
                    }
                }

                //polishing tree formating
                if (oper.Preconditions.Children.Count == 0) oper.Preconditions = null;
                else if (oper.Preconditions.Children.Count == 1) oper.Preconditions = oper.Preconditions.Children[0];
                if (oper.Effects.Children.Count == 0) oper.Effects = null;
                else if (oper.Effects.Children.Count == 1) oper.Effects = oper.Effects.Children[0];

                Output("  finished operator '" + oper.Name + "'");
            }
        }

        /// <summary>
        /// Returns list of actions from given plan that uses given object as a parameter.
        /// </summary>
        /// <param name="o">Object to look for</param>
        /// <param name="plan">Input plan to create subsequence from</param>
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
        /// Output to user.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="outputTab"></param>
        private void Output(string text = "", string outputTab = OutputManager.OutputTabName)
        {
            Form1.Manager.WriteLine(text, outputTab);
            //Console.WriteLine(text);
        }
    }
}
