using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Reference to predicate used in operator descriptions.
    /// Parameter list constits of references to operator's parameter list.
    /// </summary>
    public class PredicateReference
    {
        public Predicate Predicate;
        public Operator Operator;
        public int[] ParametersIds;
        public Object[] ConstantsReferences;

        public PredicateReference(Predicate predicateReference, int[] parametersIds, Object[] constantsReferences, Operator operatorReference)
        {
            this.Operator = operatorReference;
            this.Predicate = predicateReference;
            this.ParametersIds = parametersIds;
            this.ConstantsReferences = constantsReferences;
        }

        /// <summary>
        /// Outputs predicate reference in PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "(" + Predicate.Name;
            for (int i = 0; i < Predicate.ParameterNames.Length; i++)
            {
                if (ParametersIds[i] != -1)
                {
                    s += " " + Operator.ParameterNames[ParametersIds[i]];
                }
                else
                {
                    s += " " + ConstantsReferences[i].Name;
                }
            }
            s += ")";

            return s;
        }

        /// <summary>
        /// Creates new predicate reference that points to new operator.
        /// </summary>
        /// <param name="newOperatorReference">New operator that the new predicate reference should refer to.</param>
        /// <returns></returns>
        public PredicateReference MakeDeepCopy(Operator newOperatorReference)
        {
            return new PredicateReference(Predicate, (int[])ParametersIds.Clone(), (Object[])ConstantsReferences.Clone(), newOperatorReference);
        }

        /// <summary>
        /// Returns whether given predicate reference is indentical to current predicate instance.
        /// </summary>
        /// <param name="predicate">Predicate reference to compare current one to</param>
        /// <returns></returns>
        public bool IsEqualTo(PredicateReference pr)
        {
            if (Predicate != pr.Predicate) return false;
            if (Operator != pr.Operator) return false;

            for (int i = 0; i < ParametersIds.Length; i++)
            {
                if (pr.ParametersIds[i] != ParametersIds[i]) return false;
            }

            if (ConstantsReferences == null && pr.ConstantsReferences == null) return true;

            if (ConstantsReferences == null)
            {
                for (int i = 0; i < pr.ConstantsReferences.Length; i++)
                {
                    if (pr.ConstantsReferences[i] != null) return false;
                }
            }
            if (pr.ConstantsReferences == null)
            {
                for (int i = 0; i < ConstantsReferences.Length; i++)
                {
                    if (ConstantsReferences[i] != null) return false;
                }
            }

            for (int i = 0; i < ConstantsReferences.Length; i++)
            {
                if (ConstantsReferences[i] != pr.ConstantsReferences[i]) return false;
            }

            return true;
        }
    }
}
