using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// Class representing one literal of a clause.
    /// </summary>
    public struct Literal
    {
        /// <summary>
        /// Index of variable in formula's variable list.
        /// </summary>
        public int VariableIndex;

        /// <summary>
        /// Value the variable shoudl have to be true.
        /// </summary>
        public bool TargetValue;

        /// <summary>
        /// Reference to main formula.
        /// </summary>
        public Formula Formula;

        public Literal(int variableIndex, bool targetValue, Formula formula)
        {
            this.VariableIndex = variableIndex;
            this.TargetValue = targetValue;
            this.Formula = formula;
        }

        public override string ToString()
        {
            if (TargetValue) return Formula.VariableNames[VariableIndex];
            return "!" + Formula.VariableNames[VariableIndex];
        }
    }
}
