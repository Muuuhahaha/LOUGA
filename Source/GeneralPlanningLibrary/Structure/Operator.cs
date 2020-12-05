using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    public class Operator
    {
        public string Name;
        public string[] ParameterNames;
        public Type[][] ParameterTypes;
        public Tree Preconditions;
        public Tree Effects;

        /// <summary>
        /// List of predicates that should be present in state before action is performed.
        /// </summary>
        public List<PredicateReference> PositivePreconditions;

        /// <summary>
        /// List of predicates that should NOT be present in state before action is performed.
        /// </summary>
        public List<PredicateReference> NegativePreconditions;

        public List<PredicateReference> AddList;
        public List<PredicateReference> DelList;

        public int ParameterCount
        {
            get {
                if (ParameterNames == null) return 0;
                return ParameterNames.Length; 
            }
        }

        /// <summary>
        /// Outputs operator in PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string outcome = "(:action " + Name;
            if (ParameterNames != null && ParameterNames.Length != 0)
            {
                outcome += "\n\t\t:parameters (";
                for (int i = 0; i < ParameterNames.Length; i++)
                {
                    if (i != 0) outcome += " ";
                    outcome += ParameterNames[i];
                    if (ParameterTypes[i][0] != Type.NoType)
                    {
                        if (ParameterTypes[i].Length == 1)
                        {
                            outcome += " - " + ParameterTypes[i][0].Name;
                        }
                        else
                        {
                            outcome += " - (either";
                            for (int j = 0; j < ParameterTypes[i].Length; j++)
                            {
                                outcome += " " + ParameterTypes[i][j].Name;
                            }
                            outcome += ")";
                        }
                    }
                }
                outcome += ")";
            }
            if (Preconditions != null)
            {
                outcome += "\n\t\t:precondition ";
                outcome += Preconditions.ToString();
            }
            if (Effects != null)
            {
                outcome += "\n\t\t:effect ";
                outcome += Effects.ToString();
            }
            outcome += ")";

            return outcome;
        }

        /// <summary>
        /// Returns true if parameter of given ID can have given type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>
        public bool ParameterCanHaveType(Type t, int parameterIndex)
        {
            for (int i = 0; i < ParameterTypes[parameterIndex].Length; i++)
            {
                if (ParameterTypes[parameterIndex][i].IsFatherOf(t)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns index of parameter with given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetParameterIndex(string name)
        {
            for (int i = 0; i < ParameterNames.Length; i++)
            {
                if (ParameterNames[i] == name) return i;
            }

            return -1;
        }

        /// <summary>
        /// Creates identical operator with precondition and effect trees copied, so that they can be harmlessly changed.
        /// </summary>
        /// <returns></returns>
        public Operator MakeDeepCopy()
        {
            Operator newOperator = new Operator();

            newOperator.Name = (string)Name.Clone();
            newOperator.ParameterTypes = new Type[ParameterTypes.Length][];
            for (int i = 0; i < ParameterTypes.Length; i++)
                newOperator.ParameterTypes[i] = (Type[])ParameterTypes[i].Clone(); 
            newOperator.ParameterNames = (string[])ParameterNames.Clone();
            if (Preconditions != null) newOperator.Preconditions = Preconditions.MakeDeepCopy(newOperator);
            if (Effects != null) newOperator.Effects = Effects.MakeDeepCopy(newOperator);

            return newOperator;
        }

        /// <summary>
        /// Generates list of every possible predicate that operator can add or delete from world.
        /// </summary>
        /// <param name="predicates">List of predicate types in model.</param>
        /// <param name="parametersCanRepeat">True if objects can repeat in a predicate's parameter list.</param>
        /// <returns></returns>
        public List<PredicateReference> GeneratePossiblePredicates(Dictionary<string, Predicate> predicates, bool parametersCanRepeat)
        {
            List<PredicateReference> possiblePredicates = new List<PredicateReference>();

            foreach (var pair in predicates)
            {
                int[] parameters = new int[pair.Value.ParameterNames.Length];
                bool[] used = new bool[ParameterCount];
                GeneratePredicatesRecursively(pair.Value, 0, parameters, used, possiblePredicates, parametersCanRepeat);
            }

            return possiblePredicates;
        }

        /// <summary>
        /// Generates list of every possible predicate that operator can add or delete from world.
        /// </summary>
        /// <param name="predicates">List of predicate types in model.</param>
        /// <param name="parametersCanRepeat">True if objects can repeat in a predicate's parameter list.</param>
        /// <returns></returns>
        private void GeneratePredicatesRecursively(Predicate predicate, int startingIndex, int[] currentParameters, bool[] usedParameters, List<PredicateReference> finishedReferences, bool parametersCanRepeat)
        {
            if (startingIndex == predicate.ParameterCount)
            {
                PredicateReference pr = new PredicateReference(predicate, (int[])currentParameters.Clone(), null, this);

                foreach (var item in finishedReferences)
                {
                    if (item.IsEqualTo(pr)) return;
                }

                finishedReferences.Add(pr);
                return;
            }

            for (int i = 0; i < ParameterCount; i++)
            {
                if (usedParameters[i] && !parametersCanRepeat) continue;

                foreach (Type t in ParameterTypes[i])
                {
                    if (predicate.ParameterCanHaveType(t, startingIndex))
                    {
                        currentParameters[startingIndex] = i;
                        usedParameters[i] = true;
                        GeneratePredicatesRecursively(predicate, startingIndex + 1, currentParameters, usedParameters, finishedReferences, parametersCanRepeat);
                        usedParameters[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Generates list of actions that can be performed from given state.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="constants"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<Action> GeneratePossibleActions(Dictionary<string, Object> objects, Dictionary<string, Object> constants, State s = null)
        {
            List<Action> possibleActions = new List<Action>();
            Object[] parameters = new Object[ParameterCount];

            GeneratePossibleActionsRecursively(0, possibleActions, parameters, objects, constants, s);

            return possibleActions;
        }

        /// <summary>
        /// Generates list of actions that can be performed from given state.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="constants"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private void GeneratePossibleActionsRecursively(int index, List<Action> generatedActions, Object[] parameters, Dictionary<string, Object> objects, Dictionary<string, Object> constants, State s)
        {
            if (index == ParameterCount)
            {
                Action a = new Action(this, (Object[])parameters.Clone());

                if (s == null || a.IsApplicable(s))
                {
                    generatedActions.Add(a);
                }

                return;
            }

            foreach (var pair in objects)
            {
                if (ParameterCanHaveType(pair.Value.Type, index))
                {
                    parameters[index] = pair.Value;
                    GeneratePossibleActionsRecursively(index + 1, generatedActions, parameters, objects, constants, s);
                }
            }
            foreach (var pair in constants)
            {
                if (ParameterCanHaveType(pair.Value.Type, index))
                {
                    parameters[index] = pair.Value;
                    GeneratePossibleActionsRecursively(index + 1, generatedActions, parameters, objects, constants, s);
                }
            }
        }

        /// <summary>
        /// Creates add, delete and preconditions lists from operator's respective trees.
        /// </summary>
        public void FormLists ()
        {
            PositivePreconditions = new List<PredicateReference>();
            NegativePreconditions = new List<PredicateReference>();

            DecodeTree(Preconditions, PositivePreconditions, NegativePreconditions);

            AddList = new List<PredicateReference>();
            DelList = new List<PredicateReference>();

            DecodeTree(Effects, AddList, DelList);
        }   

        /// <summary>
        /// Decodes given tree and adds positive/negative predicates to respective lists.
        /// </summary>
        /// <param name="tree">Tree to decode</param>
        /// <param name="positivePredicates">Output list of possitive predicates</param>
        /// <param name="negativePredicates">Output list of negative predicates</param>
        private void DecodeTree(Tree tree, List<PredicateReference> positivePredicates, List<PredicateReference> negativePredicates)
        {
            switch (tree.Type)
            {
                case TreeType.Predicate:
                    positivePredicates.Add(tree.Predicate);
                    return;
                case TreeType.And:
                    foreach (var pair in tree.Children)
                    {
                        DecodeTree(pair, positivePredicates, negativePredicates);
                    }
                    break;
                case TreeType.Not:
                    DecodeTree(tree.Children[0], negativePredicates, positivePredicates);
                    break;
                case TreeType.Or:
                case TreeType.Equality:
                    //throw new NotSupportedException();
                    break;
                default:
                    break;
            }



        }
    }
}
