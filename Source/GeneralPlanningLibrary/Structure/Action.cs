using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Instance of operator.
    /// </summary>
    public class Action
    {
        public Operator Operator;
        public Object[] Parameters;

        public Action()
        {
        }

        public Action(Operator oper, Object[] parameters)
        {
            this.Operator = oper;
            this.Parameters = parameters;
        }

        public override string ToString()
        {
            string outcome = "(" + Operator.Name;
            foreach (var o in Parameters)
            {
                outcome += " " + o.Name;
            }

            outcome += ")";

            return outcome;
        }

        /// <summary>
        /// Returns true if the action is applicable to given state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsApplicable(State state)
        {
            return IsSubtreeTrue(Operator.Preconditions, state);
        }

        /// <summary>
        /// Recursively checks whether is given precondition tree true in given state.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool IsSubtreeTrue(Tree t, State state)
        {
            if (t == null) return true;

            switch (t.Type)
            {
                case TreeType.Predicate:
                    return state.Contains(InstantiatePredicate(t.Predicate));
                case TreeType.And:
                    foreach (var v in t.Children)
                    {
                        if (!IsSubtreeTrue(v, state)) return false;
                    }
                    return true;
                case TreeType.Or:
                    foreach (var v in t.Children)
                    {
                        if (IsSubtreeTrue(v, state)) return true;
                    }
                    return false;
                case TreeType.Not:
                    return !IsSubtreeTrue(t.Children[0], state);
                case TreeType.Equality:
                    Object first, second;
                    if (t.VariableReferences[0] == -1) first = t.ConstantsReferences[0];
                    else first = Parameters[t.VariableReferences[0]];
                    if (t.VariableReferences[1] == -1) second = t.ConstantsReferences[1];
                    else second = Parameters[t.VariableReferences[1]];

                    return first == second;
                default:
                    return false;
            }
        }


        /// <summary>
        /// Used in operators whose operators have multiple parameter options.
        /// Returns indices of types used in current action.
        /// </summary>
        /// <returns></returns>
        public int[] GetParameterTypesIndices()
        {
            int[] indices = new int[Parameters.Length];

            for (int i = 0; i < Parameters.Length; i++)
            {
                for (int j = 0; j < Operator.ParameterTypes[i].Length; j++)
                {
                    if (Parameters[i].Type == Operator.ParameterTypes[i][j])
                    {
                        indices[i] = j;
                        break;
                    }
                }
            }

            return indices;
        }

        /// <summary>
        /// Used in operators whose operators have multiple parameter options.
        /// Returns code of types indeces used in current action.
        /// </summary>
        /// <returns></returns>
        public string GetParameterTypesCode()
        {
            int[] indices = GetParameterTypesIndices();
            string code = "";

            for (int i = 0; i < indices.Length; i++)
            {
                code += indices[i];
            }

            return code;
        }

        /// <summary>
        /// Makes predicate instance from given predicate reference.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public PredicateInstance InstantiatePredicate(PredicateReference predicate)
        {
            PredicateInstance pi = new PredicateInstance();
            pi.Type = predicate.Predicate;
            pi.Parameters = new Object[predicate.ParametersIds.Length];

            for (int i = 0; i < predicate.ParametersIds.Length; i++)
            {
                if (predicate.ParametersIds[i] != -1)
                {
                    pi.Parameters[i] = Parameters[predicate.ParametersIds[i]];
                }
                else
                {
                    pi.Parameters[i] = predicate.ConstantsReferences[i];
                }
            }

            return pi;
        }


        /// <summary>
        /// Returns new state that forms by applicating current action to given state.
        /// Doesn't check whether action's preconditions hold true.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public State ApplicateToState(State state)
        {
            List<PredicateInstance> add = new List<PredicateInstance>();
            List<PredicateInstance> del = new List<PredicateInstance>();

            Tree t = Operator.Effects;

            if (t == null) return new State((PredicateInstance[])state.Predicates.Clone());

            switch (t.Type)
            {
                case TreeType.Predicate:
                    add.Add(InstantiatePredicate(t.Predicate));
                    break;
                case TreeType.And:
                    foreach (var v in t.Children)
                    {
                        if (v.Type == TreeType.Predicate) add.Add(InstantiatePredicate(v.Predicate));
                        else if (v.Type == TreeType.Not) del.Add(InstantiatePredicate(v.Children[0].Predicate));
                        else throw new Exception("Effect list of operator " + Operator.Name + " has wrong format.");
                    }
                    break;
                case TreeType.Not:
                    if (t.Children[0].Type != TreeType.Predicate) throw new Exception("Effect list of operator " + Operator.Name + " has wrong format.");
                    del.Add(InstantiatePredicate(t.Children[0].Predicate));
                    break;
                default:
                    throw new Exception("Effect list of operator " + Operator.Name + " has wrong format.");
            }

            bool[] deleted = new bool[state.Predicates.Length];
            for (int i = 0; i < deleted.Length; i++) deleted[i] = false;

            foreach (var predicate in del)
            {
                for (int i = 0; i < deleted.Length; i++)
                {
                    if (state.Predicates[i].Equals(predicate))
                    {
                        deleted[i] = true;
                        break;
                    }
                }
            }

            foreach (var predicate in add)
            {
                for (int i = 0; i < deleted.Length; i++)
                {
                    if (state.Predicates[i].Equals(predicate))
                    {
                        deleted[i] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < state.Predicates.Length; i++)
            {
                if (!deleted[i]) add.Add(state.Predicates[i]);
            }

            add.Sort((x, y) => (x.CompareTo(y)));

            return new State(add.ToArray());
        }

        /// <summary>
        /// Tries to pair current action's parameters with predicate instance's parameters.
        /// Returns every funcion possible.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<int[]> GetParametrization(PredicateInstance predicate)
        {
            int[] indices = new int[predicate.Parameters.Length];
            List<int[]> possibilities = new List<int[]>();

            CreateParametrizationRecursively(predicate, 0, indices, possibilities);

            return possibilities;
        }

        private void CreateParametrizationRecursively(PredicateInstance predicate, int startingIndex, int[] currentParametrization, List<int[]> foundParametrizations)
        {
            if (startingIndex == predicate.Parameters.Length)
            {
                foundParametrizations.Add((int[])currentParametrization.Clone());
                return;
            }

            for (int i = 0; i < Parameters.Length; i++)
            {
                if (Parameters[i] == predicate.Parameters[startingIndex])
                {
                    currentParametrization[startingIndex] = i;
                    CreateParametrizationRecursively(predicate, startingIndex + 1, currentParametrization, foundParametrizations);
                }
            }
        }
    }
}
