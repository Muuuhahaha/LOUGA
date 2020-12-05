using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM2
{
    /// <summary>
    /// This class represents set of transitions forming one set.
    /// At the end of algorithm, one state machine is created from each set.
    /// </summary>
    class TransitionSet
    {
        /// <summary>
        /// Matrix the set is generated from.
        /// Also has stored type the set is describing.
        /// </summary>
        public TransitionMatrix Matrix;

        /// <summary>
        /// Array storing which transitions belong to the set
        /// </summary>
        public bool[] Transitions;
        
        /// <summary>
        /// Id of state the object is in before transition takes place.
        /// </summary>
        public int[] PrecedingState;

        /// <summary>
        /// Id of state the object is in after transition takes place.
        /// </summary>
        public int[] NextState;

        /// <summary>
        /// Number of states of this set
        /// </summary>
        public int StatesCount;

        /// <summary>
        /// Number of transitions asociated with states.
        /// </summary>
        public int[] TransitionsCount;

        /// <summary>
        /// List of parameters for each state
        /// </summary>
        public List<ParameterInfo>[] ParametersInfo;

        /// <summary>
        /// List of hypotheses about state parameters.
        /// </summary>
        public List<Hypothesis> Hypotheses;

        public TransitionSet(bool[] transitions, TransitionMatrix matrix)
        {
            this.Transitions = transitions;
            this.Matrix = matrix;
        }

        /// <summary>
        /// Creates states for each transition and then unifies them according to transition matrix.
        /// </summary>
        /// <param name="transitionPairs">Transition matrix</param>
        public void MakeStateIds(bool[,] transitionPairs)
        {
            PrecedingState = new int[Transitions.Length];
            NextState = new int[Transitions.Length];

            for (int i = 0; i < Transitions.Length; i++)
            {
                if (Transitions[i])
                {
                    PrecedingState[i] = i;
                    NextState[i] = i + Transitions.Length;
                }
                else
                {
                    PrecedingState[i] = -1;
                    NextState[i] = -1;
                }
            }

            //unifying states
            for (int x = 0; x < Transitions.Length; x++)
            {
                if (!Transitions[x]) continue;

                for (int y = 0; y < Transitions.Length; y++)
                {
                    if (!Transitions[y]) continue;
                    if (!Matrix.TransitionPairs[x,y]) continue;

                    if (NextState[x] != PrecedingState[y])
                    {
                        int before = PrecedingState[y];
                        int after = NextState[x];

                        RenameState(before, after);
                    }
                }
            }

            //reindexing states so that they numbered "0,1,2,3.."
            int current = 0;
            int max = Transitions.Length * 2;
            while (current < max)
            {
                bool found = false;
                max = 0;

                for (int i = 0; i < Transitions.Length; i++)
                {
                    if (PrecedingState[i] > max) max = PrecedingState[i];
                    if (NextState[i] > max) max = NextState[i];

                    if (PrecedingState[i] == current || NextState[i] == current) found = true;
                }

                if (!found && max > current) RenameState(max, current);

                current++;
            }

            //recounting number of states
            max = 0;
            for (int i = 0; i < PrecedingState.Length; i++)
            {
                if (PrecedingState[i] > max) max = PrecedingState[i];
                if (NextState[i] > max) max = NextState[i];
            }
            StatesCount = max + 1;

            //counting number of transitions for each state
            TransitionsCount = new int[StatesCount];
            for (int i = 0; i < PrecedingState.Length; i++)
            {
                if (!Transitions[i]) continue;
                TransitionsCount[PrecedingState[i]]++;
                TransitionsCount[NextState[i]]++;
            }
        }

        private void RenameState(int idBefore, int idAfter)
        {
            for (int i = 0; i < Transitions.Length; i++)
            {
                if (PrecedingState[i] == idBefore)
                {
                    PrecedingState[i] = idAfter;
                }
                if (NextState[i] == idBefore)
                {
                    NextState[i] = idAfter;
                }
            }
        }

        /// <summary>
        /// Unifies two states.
        /// </summary>
        /// <param name="main">State to preserve</param>
        /// <param name="deleted">State to overwrite with first state</param>
        /// <returns></returns>
        private int UnifyStates(int main, int deleted)
        {
            int max = deleted;
            for (int i = 0; i < PrecedingState.Length; i++)
            {
                if (PrecedingState[i] > max) max = PrecedingState[i];
                if (PrecedingState[i] == deleted) PrecedingState[i] = main;
            }

            if (deleted != max)
            {
                for (int i = 0; i < PrecedingState.Length; i++)
                {
                    if (PrecedingState[i] == max) PrecedingState[i] = deleted;
                }
            }

            if (max == main) return deleted;
            return main;
        }

        /// <summary>
        /// Makes hypotheses about states' parameters
        /// </summary>
        /// <param name="transitionPairs">transition matrix</param>
        public void MakeHypotheses(bool[,] transitionPairs)
        {
            Hypotheses = new List<Hypothesis>();

            for (int i = 0; i < PrecedingState.Length; i++)
            {
                if (!Transitions[i]) continue;
                for (int j = 0; j < PrecedingState.Length; j++)
                {
                    if (!Transitions[j] || !transitionPairs[i, j]) continue;

                    MakeHypothesis(i, j);
                }
            }
            Console.WriteLine(Matrix.Type.Name + "have " + Hypotheses.Count + "hypotheses");
        }

        /// <summary>
        /// Makes hypotheses about parameters of state between two given transitions.
        /// </summary>
        /// <param name="first">Transition before state</param>
        /// <param name="second">Transition after state</param>
        private void MakeHypothesis(int first, int second)
        {
            TransitionInfo tifirst = Matrix.Transitions[first];
            TransitionInfo tisecond = Matrix.Transitions[second];

            for (int i = 0; i < tifirst.Operator.ParameterCount; i++)
            {
                if (i == tifirst.ParameterId) continue;
                for (int j = 0; j < tisecond.Operator.ParameterCount; j++)
                {
                    if (j == tisecond.ParameterId) continue;

                    if (tifirst.Operator.ParameterTypes[i][0] == tisecond.Operator.ParameterTypes[j][0])
                    {
                        Hypotheses.Add(new Hypothesis(first, second, i, j, PrecedingState[second], Matrix));
                    }
                }
            }
        }

        /// <summary>
        /// Tests all hypotheses on given sequence of actions for given object.
        /// </summary>
        /// <param name="subsequence">subsequnce of plan's actions for given object</param>
        /// <param name="obj">object whose subsequence is given</param>
        public void TestHypotheses(List<GeneralPlanningLibrary.Action> subsequence, GeneralPlanningLibrary.Object obj)
        {
            GeneralPlanningLibrary.Action lastAction = null;
            foreach (GeneralPlanningLibrary.Action a in subsequence)
            {
                int paramId = 0;
                while (a.Parameters[paramId] != obj) paramId++;

                //Transition set doesn't use given action
                if (!Transitions[Matrix.TransitionIndices[TransitionInfo.GetName(a.Operator, paramId)]]) continue;

                if (lastAction != null)
                {
                    for (int i = 0; i < Hypotheses.Count; i++)
                    {
                        //testing hypothesis
                        if (!Hypotheses[i].IsTrue(lastAction, a, obj))
                        {
                            Hypotheses.RemoveAt(i);
                            i--;
                        }

                        if (Form1.Halt) return;
                    }
                }
                lastAction = a;
            }
        }

        /// <summary>
        /// Inducing states' parameters according to tested hypotheses
        /// </summary>
        public void InduceParameters()
        {
            ParametersInfo = new List<ParameterInfo>[StatesCount];

            for (int i = 0; i < StatesCount; i++)
            {
                ParametersInfo[i] = new List<ParameterInfo>();

                //Creating parameter infos from hypotheses
                foreach (var h in Hypotheses)
                {
                    if (h.StateId == i)
                    {
                        ParametersInfo[i].Add(new ParameterInfo(h));
                    }
                }

                //merging parameters' infos
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

                            if (Form1.Halt) return;
                        }
                    }
                }

                //removing parameters' infos with not enough transitions.
                for (int j = 0; j < ParametersInfo[i].Count; j++)
                {
                    if (ParametersInfo[i][j].AsociatedTransitions.Count < TransitionsCount[i])
                    {
                        ParametersInfo[i].RemoveAt(j);
                        j--;
                    }
                    else if (ParametersInfo[i][j].AsociatedTransitions.Count > TransitionsCount[i])
                    {
                        ParametersInfo[i].RemoveAt(j);
                        j--;
                    }
                }

                if (Form1.Halt) return;
                Output("        " + Matrix.Type.Name + "_state_" + i + " has " + ParametersInfo[i].Count + " parameters.");
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
