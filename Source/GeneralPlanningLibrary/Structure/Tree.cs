using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Precondition of effects tree used in operator's definition.
    /// Can be AND tree with multiple children, NOT tree with a single child or a predicate reference.
    /// </summary>
    public class Tree
    {
        public Operator Operator;
        public PredicateReference Predicate;
        public List<Tree> Children;
        public TreeType Type;
        public int[] VariableReferences;
        public Object[] ConstantsReferences;

        private Tree() { }

        /// <summary>
        /// Constructor for leaf - tree is just a predicate reference.
        /// </summary>
        /// <param name="predicate">Predicate reference the tree is representing.</param>
        /// <param name="operatorReference">Operator tree is referencing to.</param>
        public Tree(PredicateReference predicate, Operator operatorReference)
        {
            this.Predicate = predicate;
            this.Type = TreeType.Predicate;
            this.Operator = operatorReference;
        }

        /// <summary>
        /// Tree represents equality comparison of two parameters.
        /// In the end not completely supported by this program.
        /// </summary>
        /// <param name="variableReferences"></param>
        /// <param name="constantsReferences"></param>
        /// <param name="operatorReference"></param>
        public Tree(int[] variableReferences, Object[] constantsReferences, Operator operatorReference)
        {
            this.VariableReferences = variableReferences;
            this.Type = TreeType.Equality;
            this.Operator = operatorReference;
            this.ConstantsReferences = constantsReferences;
        }

        /// <summary>
        /// Constructor used only for AND type - tree with multiple tree children.
        /// </summary>
        /// <param name="operation">Operaton the tree represents.</param>
        /// <param name="children">List of tree children</param>
        /// <param name="operatorReference">Operator tree is referencing to.</param>
        public Tree(TreeType operation, List<Tree> children, Operator operatorReference)
        {
            this.Type = operation;
            this.Children = children;
            this.Operator = operatorReference;
        }

        /// <summary>
        /// Constructor used for AND/NOT type - tree with single child.
        /// </summary>
        /// <param name="operation">Operaton the tree represents.</param>
        /// <param name="singleChild">Single tree child</param>
        /// <param name="operatorReference">Operator tree is referencing to.</param>
        public Tree(TreeType operation, Tree singleChild, Operator operatorReference)
        {
            this.Type = operation;
            this.Children = new List<Tree>();
            this.Children.Add(singleChild);
            this.Operator = operatorReference;
        }

        /// <summary>
        /// Returns child of given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Tree this [int i] 
        {
            get { return Children[i]; }
        }

        /// <summary>
        /// Outputs tree in PDDL format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case TreeType.Predicate:
                    return Predicate.ToString();
                case TreeType.And:
                case TreeType.Or:
                case TreeType.Not:
                    string s = "(" + Type.ToString().ToLower() + " ";
                    for (int i = 0; i < Children.Count; i++)
                    {
                        if (i > 0) s += " ";
                        s += Children[i].ToString();
                    }
                    s += ")";
                    return s;
                case TreeType.Equality:
                    string text = "(= ";

                    if (VariableReferences[0] != -1) text += Operator.ParameterNames[VariableReferences[0]];
                    else text += ConstantsReferences[0].Name;

                    text += " ";

                    if (VariableReferences[1] != -1) text += Operator.ParameterNames[VariableReferences[1]];
                    else text += ConstantsReferences[1].Name;

                    text += ")";
                    return text;
                default:
                    return "Tree";
            }
        }

        /// <summary>
        /// Copies tree with references to new operator.
        /// </summary>
        /// <param name="newOperatorReference">New operator that new tree should reference to.</param>
        /// <returns></returns>
        public Tree MakeDeepCopy(Operator newOperatorReference)
        {
            Tree t = new Tree();
            t.Operator = newOperatorReference;
            t.Type = Type;

            switch (Type)
            {
                case TreeType.Predicate:
                    t.Predicate = Predicate.MakeDeepCopy(newOperatorReference);
                    break;
                case TreeType.And:
                case TreeType.Or:
                case TreeType.Not:
                    t.Children = new List<Tree>();
                    foreach (var child in Children)
                    {
                        t.Children.Add(child.MakeDeepCopy(newOperatorReference));
                    }
                    break;
                case TreeType.Equality:
                    t.VariableReferences = (int[])VariableReferences.Clone();
                    t.ConstantsReferences = (Object[])ConstantsReferences.Clone();
                    break;
                default:
                    break;
            }

            return t;
        }
    }

    /// <summary>
    /// Type of tree.
    /// </summary>
    public enum TreeType
    {
        Predicate,
        And, 
        Or,
        Not, 
        Equality
    }
}
