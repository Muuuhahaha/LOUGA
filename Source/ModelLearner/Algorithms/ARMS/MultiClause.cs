using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// Extension of standard clause. It's literals can be standard literals or conjunction of them.
    /// </summary>
    public class MultiClause : IClause
    {
        /// <summary>
        /// Weight of the clause.
        /// </summary>
        public double Weight;

        /// <summary>
        /// List of conjuction of literals.
        /// </summary>
        public List<Literal[]> Literals;

        /// <summary>
        /// Reference to original formula.
        /// </summary>
        public Formula Formula;

        public MultiClause(Formula formula = null, double weight = 1)
        {
            Weight = weight;
            Literals = new List<Literal[]>();
            this.Formula = formula;
        }

        public MultiClause(int[][] variableIndices, bool[][] inverseNegations, Formula formula, double weight = 1)
        {
            this.Weight = weight;
            this.Literals = new List<Literal[]>();
            this.Formula = formula;

            for (int i = 0; i < variableIndices.Length; i++)
            {
                Literal[] multiliteral = new Literal[variableIndices[i].Length];

                for (int j = 0; j < variableIndices[i].Length; j++)
                {
                    multiliteral[j] = new Literal(variableIndices[i][j], inverseNegations[i][j], formula);
                }

                Literals.Add(multiliteral);
            }
        }

        /// <summary>
        /// Returns whether the multiclause is true with given parametrization.
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public bool IsTrue(bool[] parameterValues)
        {
            foreach (var multiliteral in Literals)
            {
                bool correct = true;

                for (int i = 0; i < multiliteral.Length; i++)
                {
                    if (multiliteral[i].TargetValue != parameterValues[multiliteral[i].VariableIndex])
                    {
                        correct = false;
                        break;
                    }
                }

                if (correct) return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a multiliteral to multiclause.
        /// </summary>
        /// <param name="indices">Indices of parameters in multiliteral.</param>
        /// <param name="targetValues">Target values of parameters.</param>
        public void AddMultiliteral(int[] indices, bool[] targetValues)
        {
            Literal[] multiliteral = new Literal[indices.Length];

            for (int i = 0; i < multiliteral.Length; i++)
            {
                multiliteral[i] = new Literal(indices[i], targetValues[i], Formula);
            }

            Literals.Add(multiliteral);
        }

        /// <summary>
        /// Transforms multiclause to printable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
        {
            string s = "";

            for (int i = 0; i < Literals.Count; i++)
	        {
                if (i != 0) s += " | \n\t\t";
                s += "(";

                for (int j = 0; j < Literals[i].Length; j++)
                {
                    if (j != 0) s += " & ";

                    if (!Literals[i][j].TargetValue) s += "!";
                    s += Formula.VariableNames[Literals[i][j].VariableIndex];
                }
                s += ")";
	        }

            if (Literals.Count > 1) return "(" + s + ")";
            return s;
        }
        
        /// <summary>
        /// Flips all literals in given multiliteral to make the multiclause true.
        /// </summary>
        /// <param name="parameterValues">Parametrization to addjust</param>
        /// <param name="id">ID of multiliteral to satisfy.</param>
        public void FlipLiteral(bool[] parameterValues, int id)
        {
            foreach (var literal in Literals[id])
            {
                parameterValues[literal.VariableIndex] ^= true;
            }
        }

        public int GetLiteralsCount()
        {
            return Literals.Count;
        }

        public double GetWeight()
        {
            return Weight;
        }
    }
}
