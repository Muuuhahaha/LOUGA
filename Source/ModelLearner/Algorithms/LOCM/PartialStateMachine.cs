using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM
{
    /// <summary>
    /// Partially finished state machine for one type.
    /// </summary>
    class PartialStateMachine
    {
        /// <summary>
        /// Type the machine is describing.
        /// </summary>
        public GeneralPlanningLibrary.Type ObjectType;
        
        /// <summary>
        /// Array describing next state for each state after particular action is performed.
        /// [StateId, TransitionId] -> new StateId
        /// </summary>
        public int[,] StateTransitions;

        /// <summary>
        /// Lists of transitions for each state.
        /// </summary>
        public List<TransitionInfo>[] TransitionsInfo;

        /// <summary>
        /// List of parameters for each state.
        /// </summary>
        public List<ParameterInfo>[] ParametersInfo;

        /// <summary>
        /// Total number of transitions.
        /// </summary>
        public int TransitionCount;

        /// <summary>
        /// Current number of states.
        /// </summary>
        public int StateCount;

        /// <summary>
        /// Mapping "transition decsription string" -> StateId the transition currently belongs to.
        /// Transition description consist of operator's name + index of parameter.
        /// </summary>
        public Dictionary<string, int> StateIndices;

        /// <summary>
        /// Mapping "transition decsription string" -> transition id
        /// Transition description consist of operator's name + index of parameter.
        /// </summary>
        public Dictionary<string, int> TransitionIndices; 

        /// <summary>
        /// List of current hypotheses about parameters.
        /// </summary>
        public List<Hypothesis> Hypotheses;

        /// <summary>
        /// List of states to unify after unifying the current ones.
        /// </summary>
        List<int[]> unifyNext;

        public PartialStateMachine(GeneralPlanningLibrary.Type objectType, int transitionCount, Dictionary<string, int> transitionIndices, List<TransitionInfo> transitions)
        {
            this.ObjectType = objectType;
            this.TransitionCount = transitionCount;
            StateCount = 0;
            StateIndices = new Dictionary<string, int>();
            TransitionIndices = transitionIndices;
            StateTransitions = new int[transitionCount * 2, transitionCount];
            TransitionsInfo = new List<TransitionInfo>[transitionCount * 2];
            Hypotheses = new List<Hypothesis>();

            unifyNext = new List<int[]>();

            for (int i = 0; i < transitionCount * 2; i++)
            {
                for (int j = 0; j < transitionCount; j++)
                {
                    StateTransitions[i, j] = -1;
                }
            }

            foreach (var transition in transitions)
            {
                TryAddState(transition);
            }
        }

        /// <summary>
        /// Unifies states in regard to given sequence of actions for given object.
        /// </summary>
        /// <param name="subsequence"></param>
        /// <param name="obj"></param>
        public void Update(List<GeneralPlanningLibrary.Action> subsequence, GeneralPlanningLibrary.Object obj)
        {
            TransitionInfo lastTransition = null;
            GeneralPlanningLibrary.Action lastAction = null;
            for (int i = 0; i < subsequence.Count; i++)
            {
                GeneralPlanningLibrary.Action a = subsequence[i];
                int index = 0, count = 0;

                for (int j = 0; j < a.Parameters.Length; j++)
                {
                    if (a.Parameters[j] == obj)
                    {
                        index = j;
                        count++;
                    }
                }

                //object was used multiple times in parameter list of one action -> ignoring the action
                if (count > 1)
                {
                    lastTransition = null;
                    continue;
                }

                //finding transition indices
                TransitionInfo before = new TransitionInfo(a.Operator, index, TransitionType.BeforeAction);
                TransitionInfo after = new TransitionInfo(a.Operator, index, TransitionType.AfterAction);

                int indexBefore = TryAddState(before);
                int indexAfter = TryAddState(after);
                int transitionIndex;
                TransitionIndices.TryGetValue(before.GetActionCode(), out transitionIndex);

                StateTransitions[indexBefore, transitionIndex] = indexAfter;

                //unifying
                if (lastTransition != null)
                {
                    Unify(lastTransition.GetName(), before.GetName());
                }

                lastTransition = after;
                lastAction = a;
            }

        }

        /*private List<int[]> PotentionalParameters(TransitionInfo lastState, TransitionInfo beforeAction, Action lastAction, Action action)
        {
            List<int[]> parameters = new List<int[]>();

            for (int i = 0; i < lastState.Operator.ParameterCount; i++)
            {
                if (i == lastState.ParameterIndex) continue;
                for (int k = 0; k < beforeAction.Operator.ParameterCount; k++)
                {
                    if (lastAction.Parameters[i] == action.Parameters[k])
                    {
                        parameters.Add(new int[] { i, k });
                    }
                }
            }

            return parameters;
        }*/

        /// <summary>
        /// Induces states' parameters from tested hypotheses.
        /// </summary>
        public void InduceParameters()
        {
            ParametersInfo = new List<ParameterInfo>[StateCount];
            List<Hypothesis>[] statesHypotheses = new List<Hypothesis>[StateCount];

            for (int i = 0; i < StateCount; i++) statesHypotheses[i] = new List<Hypothesis>();

            foreach (var h in Hypotheses)
            {
                int state;
                if (!StateIndices.TryGetValue(h.TransitionBefore.GetName(), out state)) throw new Exception("PartialStateMachine inner inconsistency.");
                statesHypotheses[state].Add(h);
            }

            for (int i = 0; i < StateCount; i++)
            {
                ParametersInfo[i] = new List<ParameterInfo>();

                foreach (var h in statesHypotheses[i])
                {
                    ParametersInfo[i].Add(new ParameterInfo(h));
                }

                bool change = true;
                while (change)
                {
                    change = false;
                    for (int j = 0; j < ParametersInfo[i].Count && !change; j++)
                    {
                        for (int k = 0; k < ParametersInfo[i].Count && !change; k++)
                        {
                            if (j == k) continue;

                            if (ParametersInfo[i][j].TryMerge(ParametersInfo[i][k]))
                            {
                                change = true;
                                ParametersInfo[i].RemoveAt(k);
                            }
                        }
                    }
                }

                for (int j = 0; j < ParametersInfo[i].Count; j++)
                {
                    if (ParametersInfo[i][j].AsociatedTransitions.Count < TransitionsInfo[i].Count)
                    {
                        ParametersInfo[i].RemoveAt(j);
                        j--;
                    }
                    else if (ParametersInfo[i][j].AsociatedTransitions.Count > TransitionsInfo[i].Count) 
                    {
                        //often removed due to not enough input examples (some transitions are missing)
                        ParametersInfo[i].RemoveAt(j);
                        j--;
                    }
                }
                Output("  " + ObjectType.Name + "_state" + i + " has now " + ParametersInfo[i].Count + " parameters.");
            }
        }

        /// <summary>
        /// Prepares hypotheses about which state can potentionally have some parameters.
        /// </summary>
        public void MakeHypotheses()
        {
            for (int i = 0; i < StateCount; i++)
            {
                foreach (var first in TransitionsInfo[i])
                {
                    if (first.TransitionType == TransitionType.BeforeAction) continue;

                    foreach (var second in TransitionsInfo[i])
                    {
                        if (second.TransitionType == TransitionType.AfterAction) continue;

                        MakeHypothesis(first, second);

                        if (Form1.Halt) return;
                    }
                }
            }
        }

        /// <summary>
        /// Make a hypothesis about parameters of state betweed two transitions
        /// </summary>
        /// <param name="first">Transition before state</param>
        /// <param name="second">Transition after state</param>
        private void MakeHypothesis(TransitionInfo first, TransitionInfo second)
        {
            for (int i = 0; i < first.Operator.ParameterCount; i++)
            {
                if (first.ParameterIndex == i) continue;
                for (int j = 0; j < second.Operator.ParameterCount; j++)
                {
                    if (second.ParameterIndex == j) continue;

                    if (first.Operator.ParameterTypes[i][0] == second.Operator.ParameterTypes[j][0])
                    {
                        Hypothesis h = new Hypothesis(first, second, i, j);
                        Hypotheses.Add(h);

                        Output("  adding hypothesis: " + h.GetName() + " " + h.IdBefore + " -> " + h.IdAfter);
                    }
                }
            }
        }

        /// <summary>
        /// Test current hypotheses on given sequence of actions for given object.
        /// </summary>
        /// <param name="subsequence"></param>
        /// <param name="obj"></param>
        public void TestHypotheses(List<GeneralPlanningLibrary.Action> subsequence, GeneralPlanningLibrary.Object obj)
        {
            GeneralPlanningLibrary.Action lastAction = null;
            foreach (var a in subsequence)
            {
                if (lastAction != null)
                {
                    for (int i = 0; i < Hypotheses.Count; i++)
                    {
                        if (!Hypotheses[i].IsTrue(lastAction, a, obj))
                        {
                            Output("  removing hypothesis: " + Hypotheses[i].GetName() + " " + Hypotheses[i].IdBefore + " -> " + Hypotheses[i].IdAfter);
                            Hypotheses.RemoveAt(i);
                            i--;

                            if (Form1.Halt) return;
                        }
                    }
                }
                lastAction = a;
            }
        }

        /// <summary>
        /// Add new transitions' info if they don't already exist and unifies corresponding states.
        /// </summary>
        /// <param name="state1"></param>
        /// <param name="state2"></param>
        private void AddAndUnify(TransitionInfo state1, TransitionInfo state2)
        {
            int index1 = TryAddState(state1);
            int index2 = TryAddState(state2);
            Unify(index1, index2);
        }

        /// <summary>
        /// Unifies two states described with strings.
        /// </summary>
        /// <param name="state1"></param>
        /// <param name="state2"></param>
        private void Unify(string state1, string state2)
        {
            int index1, index2;
            StateIndices.TryGetValue(state1, out index1);
            StateIndices.TryGetValue(state2, out index2);

            if (index1 == index2) return;

            Unify(index1, index2);
        }

        /// <summary>
        /// Unifies two states of given indices.
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private void Unify(int index1, int index2)
        {
            if (index1 == index2) return;

            if (index1 >= StateCount || index2 >= StateCount)
            {
                throw new Exception("Unexpected behavior of LOCM");
            }

            foreach (var arr in unifyNext)
            {
                if (arr[0] == index2)
                {
                    arr[0] = index1;
                }
                if (arr[1] == index2)
                {
                    arr[1] = index1;
                }
            }

            foreach (var info in TransitionsInfo[index2])
            {
                TransitionsInfo[index1].Add(info);

                StateIndices.Remove(info.GetName());
                StateIndices.Add(info.GetName(), index1);
            }

            for (int i = 0; i < TransitionCount; i++)
            {
                if (StateTransitions[index1, i] != -1 &&
                    StateTransitions[index2, i] != -1 &&
                    StateTransitions[index1, i] != StateTransitions[index2, i])
                {
                    unifyNext.Add(new int[] { StateTransitions[index1, i], StateTransitions[index2, i] });
                    Console.WriteLine(StateTransitions[index1, i] + ", " + StateTransitions[index2, i]);
                }
                if (StateTransitions[index1, i] == -1) StateTransitions[index1, i] = StateTransitions[index2, i];
            }

            for (int i = 0; i < StateCount; i++)
            {
                for (int j = 0; j < TransitionCount; j++)
                {
                    if (StateTransitions[i, j] == index2) StateTransitions[i, j] = index1;
                }
            }

            if (index2 + 1 != StateCount)
            {
                foreach (var arr in unifyNext)
                {
                    if (arr[0] == StateCount - 1) arr[0] = index2;
                    if (arr[1] == StateCount - 1) arr[1] = index2;
                    if (arr[0] == index2) arr[0] = index1;
                    if (arr[1] == index2) arr[1] = index1;
                }


                TransitionsInfo[index2] = TransitionsInfo[StateCount - 1];
                TransitionsInfo[StateCount - 1] = null;

                foreach (var info in TransitionsInfo[index2])
                {
                    StateIndices.Remove(info.GetName());
                    StateIndices.Add(info.GetName(), index2);
                }

                for (int i = 0; i < TransitionCount; i++)
                {
                    StateTransitions[index2, i] = StateTransitions[StateCount - 1, i];
                }

                for (int i = 0; i < StateCount; i++)
                {
                    for (int j = 0; j < TransitionCount; j++)
                    {
                        if (StateTransitions[i, j] == StateCount - 1) StateTransitions[i, j] = index2;
                    }
                }
            }
            else
            {
                TransitionsInfo[index2] = null;
            }

            if (StateCount == 1)
            {
                throw new Exception("Unexpected behavior of LOCM");
            }
            StateCount--;

            foreach (var arr in unifyNext)
            {
                if (arr[0] >= StateCount || arr[1] >= StateCount)
                {
                    throw new Exception("Unexpected behavior of LOCM");
                }
            }

            while (unifyNext.Count > 0)
            {
                int[] arr = unifyNext[0];
                unifyNext.Remove(arr);
                Unify(arr[0], arr[1]);
            }
        }

        /// <summary>
        /// Tries to add new state if it does not already exist.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>index of state the transition is now part of</returns>
        private int TryAddState(TransitionInfo state)
        {
            int index;
            string name = state.GetName();

            if (StateIndices.TryGetValue(name, out index)) return index;

            index = StateCount++;
            StateIndices.Add(name, index);
            for (int i = 0; i < TransitionCount; i++)
            {
                StateTransitions[index, i] = -1;
            }

            TransitionsInfo[index] = new List<TransitionInfo>();
            TransitionsInfo[index].Add(state);

            return index;
        }

        /// <summary>
        /// Returns 2 indices of states the object is before and after performing given action.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="parameterIndex">Index in operator's parameters' list where the object is used.</param>
        /// <returns></returns>
        public int[] GetOperatorsState(Operator o, int parameterIndex)
        {
            int before, after;

            StateIndices.TryGetValue(TransitionInfo.GetName(o, parameterIndex, TransitionType.BeforeAction), out before);
            StateIndices.TryGetValue(TransitionInfo.GetName(o, parameterIndex, TransitionType.AfterAction), out after);

            return new int[] { before, after };
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
