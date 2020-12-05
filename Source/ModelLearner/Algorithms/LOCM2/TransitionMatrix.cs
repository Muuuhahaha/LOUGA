using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM2
{
    /// <summary>
    /// Class containing info about which actions appeared after others in input plans for given type.
    /// 
    /// </summary>
    class TransitionMatrix
    {
        /// <summary>
        /// Type the matrix is describing.
        /// </summary>
        public GeneralPlanningLibrary.Type Type;

        /// <summary>
        /// Indices of transitions.
        /// Described keys are standard transition descriptions created by TransitionInfo class.
        /// </summary>
        public Dictionary<string, int> TransitionIndices;

        /// <summary>
        /// List of transitions that can happen to objects of this type.
        /// </summary>
        public List<TransitionInfo> Transitions;

        /// <summary>
        /// List of holes in matrix.
        /// </summary>
        public List<Tuple<int, int>> Holes;

        /// <summary>
        /// Created transition sets.
        /// Each transition set describes one state machine that represents object's behavior from particualar viewpoint.
        /// </summary>
        public List<TransitionSet> TransitionSets;

        /// <summary>
        /// Matrix describing which transitions appeared after others in input plans.
        /// </summary>
        public bool[,] TransitionPairs;

        public TransitionMatrix(GeneralPlanningLibrary.Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Creates matrix from input plans.
        /// </summary>
        /// <param name="worlds">Worlds to create matrix from.</param>
        /// <param name="m">Input model.</param>
        public void FormMatrix(List<World> worlds, Model m)
        {
            TransitionIndices = new Dictionary<string, int>();
            Transitions = new List<TransitionInfo>();

            //preparing transitions
            foreach (var oper in m.Operators) 
            {
                for (int i = 0; i < oper.Value.ParameterCount; i++)
                {
                    if (oper.Value.ParameterCanHaveType(Type, i))
                    {
                        TransitionIndices.Add(oper.Key + "." + i, TransitionIndices.Count);
                        Transitions.Add(new TransitionInfo(oper.Value, i, TransitionType.Universal));
                    }
                }
            }

            TransitionPairs = new bool[TransitionIndices.Count, TransitionIndices.Count];

            //parsing plan info
            foreach (var world in worlds)
            {
                foreach (var obj in world.Objects.Concat(m.Constants))
                {
                    if (obj.Value.Type != Type) continue;

                    foreach (var plan in world.Plans)
                    {
                        //parsing plan info from a single object's perspective
                        List<Tuple<GeneralPlanningLibrary.Action, int>> sequence = GetSubsequenceByObject(obj.Value, plan.Actions);

                        if (sequence.Count < 2) continue;

                        int lastId = TransitionIndices[GetName(sequence[0])];
                        for (int i = 1; i < sequence.Count; i++)
                        {
                            int id = TransitionIndices[GetName(sequence[i])];
                            TransitionPairs[lastId, id] = true;

                            lastId = id;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns list of action from given plan that have given object as a parameter.   
        /// </summary>
        /// <param name="o">Object to look for</param>
        /// <param name="plan">Plan from which subsequence should be picked.</param>
        /// <returns>List of actions and indices where in action's parameter list is object used.</returns>
        public static List<Tuple<GeneralPlanningLibrary.Action, int>> GetSubsequenceByObject(GeneralPlanningLibrary.Object o, GeneralPlanningLibrary.Action[] plan)
        {
            List<Tuple<GeneralPlanningLibrary.Action, int>> subsequence = new List<Tuple<GeneralPlanningLibrary.Action, int>>();

            for (int i = 0; i < plan.Length; i++)
            {
                for (int j = 0; j < plan[i].Parameters.Length; j++)
                {
                    if (plan[i].Parameters[j] == o)
                    {
                        subsequence.Add(new Tuple<GeneralPlanningLibrary.Action, int>(plan[i], j));
                        break;
                    }
                }
            }

            return subsequence;
        }

        /// <summary>
        /// Returns index of transition described by given Action-index tuple.
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        private int GetIndex(Tuple<GeneralPlanningLibrary.Action, int> parameterInfo)
        {
            return TransitionIndices[GetName(parameterInfo)];
        }

        /// <summary>
        /// Returns string describing info about parameter.
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        private string GetName(Tuple<GeneralPlanningLibrary.Action, int> parameterInfo)
        {
            return parameterInfo.Item1.Operator.Name + "." + parameterInfo.Item2;
        }

        /// <summary>
        /// Returns printable string of matrix contents.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int maxLenght = 0;
            foreach (var pair in TransitionIndices)
            {
                if (pair.Key.Length > maxLenght) maxLenght = pair.Key.Length;
            }

            string s = "  " + Type.Name + "\n";
            int index = 0;
            foreach (var pair in TransitionIndices)
            {
                s += "    " + pair.Key + new string(' ', maxLenght - pair.Key.Length);

                for (int i = 0; i < TransitionIndices.Count; i++)
                {
                    s += " " + ((TransitionPairs[index, i]) ? "x" : ".");
                }
                index++;
                s += "\n";
            }

            return s;
        }

        /// <summary>
        /// Finds holes in matrix.
        /// </summary>
        /// <param name="usable">Array of boolean values describing which transitions should be considered.</param>
        /// <returns>List of holes' coordinates.</returns>
        public List<Tuple<int, int>> FindHoles(bool[] usable = null)
        {
            Holes = new List<Tuple<int, int>>();

            //fixing input
            if (usable == null)
            {
                usable = new bool[TransitionIndices.Count];
                for (int i = 0; i < usable.Length; i++)
                {
                    usable[i] = true;
                }
            }

            //finding of holes
            for (int rr = 0; rr < TransitionIndices.Count; rr++)
            {
                if (!usable[rr]) continue;

                for (int cc = 0; cc < TransitionIndices.Count; cc++)
                {
                    if (!usable[cc]) continue;

                    bool found = false;
                    for (int r = 0; r < TransitionIndices.Count && !found; r++)
                    {
                        if (!usable[r] || r == rr) continue;

                        for (int c = 0; c < TransitionIndices.Count && !found; c++)
                        {
                            if (!usable[c] || c == cc) continue;

                            if (TransitionPairs[r, c] && TransitionPairs[rr, c] &&
                                TransitionPairs[r, cc] && !TransitionPairs[rr, cc])
                            {
                                Holes.Add(new Tuple<int, int>(rr, cc));
                                found = true;
                            }
                        }
                    }
                }
            }

            return Holes;
        }

        /// <summary>
        /// Forms transition sets from found holes according to LOCM2 algorithm.
        /// </summary>
        /// <param name="worlds">List of input worlds</param>
        public void FormTransitionSets(List<World> worlds)
        {
            TransitionSets = new List<TransitionSet>();

            //forming transition sets
            foreach (var hole in Holes)
            {
                bool found = false;
                foreach (var set in TransitionSets)
                {
                    if (set.Transitions[hole.Item1] && set.Transitions[hole.Item2])
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    bool[] subset = FormTransitionSet(hole.Item1, hole.Item2, worlds);

                    if (Form1.Halt) return;

                    if (subset != null) TransitionSets.Add(new TransitionSet(subset, this));
                }
            }

            //removing sets that are subsets of another
            for (int i = 0; i < TransitionSets.Count; i++)
            {
                for (int j = 0; j < TransitionSets.Count; j++)
                {
                    if (Form1.Halt) return;

                    if (i == j) continue;

                    bool subset = true;
                    for (int k = 0; k < TransitionSets[i].Transitions.Length && subset; k++)
                    {
                        if (TransitionSets[i].Transitions[k] && !TransitionSets[j].Transitions[k])
                        {
                            subset = false;
                        }
                    }

                    if (subset)
                    {
                        TransitionSets.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            //creating trivial set containing all transitions
            bool[] allTransitions = new bool[Transitions.Count];
            for (int i = 0; i < Transitions.Count; i++)
            {
                allTransitions[i] = true;
            }
            TransitionSets.Add(new TransitionSet(allTransitions, this));

            //finishing sets and creating hypotheses.
            foreach (var set in TransitionSets)
            {
                set.MakeStateIds(TransitionPairs);
                set.MakeHypotheses(TransitionPairs);
            }
        }

        /// <summary>
        /// Forms a transition set around given hole if possible.
        /// </summary>
        /// <param name="first">First coordinate of the hole.</param>
        /// <param name="second">Second coordinate of the hole.</param>
        /// <param name="worlds">List of input worlds</param>
        /// <returns>NULL if transition set could not be formed. Othervise array describing which transitions were used.</returns>
        private bool[] FormTransitionSet(int first, int second, List<World> worlds)
        {
            bool[] transitions = new bool[TransitionIndices.Count];

            transitions[first] = true;
            transitions[second] = true;
            int c = 2;
            if (first == second) c = 1;

            for (int i = 1; i < TransitionIndices.Count - c; i++)
			{
                bool[] subset = TestSubsets(transitions, 0, i, worlds);
                if (subset != null) return subset;

                if (Form1.Halt) return null;
			}

            return null;
        }

        /// <summary>
        /// Tests if a transition set without holes can be formed from given partially built subset.
        /// Returns found subset if possible.
        /// </summary>
        /// <param name="transitions">Partially finished subset</param>
        /// <param name="startingIndex">Index of transition the testing should start with.</param>
        /// <param name="length">Number of transitions to add to set</param>
        /// <param name="worlds">Input worlds</param>
        /// <returns>Correct subset that was found or NULL if no correct subset was found.</returns>
        private bool[] TestSubsets(bool[] transitions, int startingIndex, int length, List<World> worlds)
        {
            //given subset has desired size -> checking if given subset is correct
            if (length == 0)
            {
                if (FindHoles(transitions).Count > 0) return null;
                if (IsValid(transitions, worlds))
                {
                    return (bool[])transitions.Clone();
                }
                else
                {
                    return null;
                }
            }

            //adding a single transition to subset and continuing recursively
            for (int i = startingIndex; i < transitions.Length + 1 - length; i++)
            {
                if (Form1.Halt) return null;

                if (transitions[i]) continue;

                transitions[i] = true;

                bool[] subset = TestSubsets(transitions, i + 1, length - 1, worlds);
                if (subset != null) return subset;

                if (Form1.Halt) return null;

                transitions[i] = false;
            }

            return null;
        }

        /// <summary>
        /// Returns TRUE if given subset is valid according input worlds.
        /// Subset is valid if there are no transition sequences in input sequences filtered by transition set that didn't occur in input sequences.
        /// For example: if [open(x), open(x)] didn't occur consecutive input sequences, it should not happen in transition set's subsequence either.
        /// </summary>
        /// <param name="transitions"></param>
        /// <param name="worlds"></param>
        /// <returns></returns>
        private bool IsValid(bool[] transitions, List<World> worlds)
        {
            foreach (var world in worlds)
            {
                foreach (var obj in world.Objects.Concat(world.Model.Constants))
                {
                    if (obj.Value.Type != Type) continue;

                    foreach (var plan in world.Plans)
                    {
                        var subseqence = GetSubsequenceByObject(obj.Value, plan.Actions);
                        if (subseqence.Count == 0) continue;

                        int i = 1;
                        int state = GetIndex(subseqence[0]);
                        
                        //finding first relevant state
                        while (!transitions[state] && i < subseqence.Count)
                        {
                            state = GetIndex(subseqence[i]);
                            i++;
                        }

                        //going through rest of the states
                        while (i < subseqence.Count)
                        {
                            int next = GetIndex(subseqence[i]);
                            i++;

                            if (!transitions[next]) continue;

                            //validity check
                            if (!TransitionPairs[state, next])
                            {
                                return false;
                            }

                            state = next;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Begins testing of hypotheses of all transition sets on given subsequence
        /// </summary>
        /// <param name="subsequence">Subsequence of input plan concerning one object of correct type</param>
        /// <param name="obj">Object whose subsequence was picked.</param>
        public void TestHypotheses(List<GeneralPlanningLibrary.Action> subsequence, GeneralPlanningLibrary.Object obj)
        {
            foreach (var set in TransitionSets)
            {
                set.TestHypotheses(subsequence, obj);
                if (Form1.Halt) return;
            }
        }

        /// <summary>
        /// Induces parameters of each transition set from verified hypotheses
        /// </summary>
        public void InduceParameters()
        {
            Output("  inducing parameters of type: " + Type.Name);
            int i = 0;
            foreach (var set in TransitionSets)
            {
                Output("    transition set " + i++ + ": ");
                set.InduceParameters();

                if (Form1.Halt) return;
            }
        }

        /// <summary>
        /// Retruns TRUE if whole line of given ID is set to false.
        /// </summary>
        /// <param name="lineId"></param>
        /// <returns></returns>
        public bool LineIsEmpty(int lineId)
        {
            for (int i = 0; i < TransitionIndices.Count; i++)
            {
                if (TransitionPairs[lineId, i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Ouput to user.
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
