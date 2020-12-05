using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// Class representing a single clause in formula.
    /// </summary>
    public class Clause : IClause
    {
        /// <summary>
        /// Weight of clause
        /// </summary>
        public double Weight;

        /// <summary>
        /// Literals present in clause. Dictionary is used to prevent duplicities.
        /// </summary>
        public SortedDictionary<string, Literal> Literals;

        /// <summary>
        /// Formula the clause is part of.
        /// </summary>
        public Formula Formula;

        /// <summary>
        /// Constructor for empty caluse.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="weight"></param>
        public Clause(Formula formula = null, double weight = 1)
        {
            this.Formula = formula;
            Weight = weight;
            Literals = new SortedDictionary<string, Literal>();
        }

        /// <summary>
        /// Constructor for clause with multiple variables.
        /// </summary>
        /// <param name="variableIndices">Indices of variables in formula's variable list.</param>
        /// <param name="inverseNegations">Value variables should have in order to be true.</param>
        /// <param name="formula">Formula the clause is part of</param>
        /// <param name="weight">Weight of the clause.</param>
        public Clause(int[] variableIndices, bool[] targetValues, Formula formula, double weight = 1)
        {
            this.Weight = weight;
            this.Literals = new SortedDictionary<string, Literal>();
            this.Formula = formula;

            for (int i = 0; i < variableIndices.Length; i++)
            {
                Literal l = new Literal(variableIndices[i], targetValues[i], formula);
                Literals.Add(l.ToString(), l);
            }
        }

        /// <summary>
        /// Constructor for clause with multiple variables.
        /// </summary>
        /// <param name="variableIndex">Index of variable in formula's variable list.</param>
        /// <param name="targetValue">Value the variable should have in order to be true.</param>
        /// <param name="formula">Formula the clause is part of</param>
        /// <param name="weight">Weight of the clause.</param>
        public Clause(int variableIndex, bool targetValue, Formula formula, double weight = 1)
        {
            this.Weight = weight;
            this.Literals = new SortedDictionary<string, Literal>();
            this.Formula = formula;

            Literal l = new Literal(variableIndex, targetValue, formula);
            Literals.Add(l.ToString(), l);
        }

        /// <summary>
        /// Returns TRUE if the clause is true with given parameters valuation.
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public bool IsTrue(bool[] parameterValues)
        {
            foreach (var pair in Literals)
            {
                if (pair.Value.TargetValue == parameterValues[pair.Value.VariableIndex]) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns text representation of the clause.
        /// </summary>
        /// <returns></returns>
        public string GetName() 
        {
            string s = "";

            bool first = true;
            foreach (var pair in Literals)
	        {
                if (!first) s += " | ";
                else first = false;

                s += pair.Value.ToString();
	        }

            if (Literals.Count > 1) return "(" + s + ")";
            return s;
        }

        /// <summary>
        /// Adds literal to clause if it's not already present.
        /// </summary>
        /// <param name="literal"></param>
        public void TryAddLiteral(Literal literal)
        {
            if (Literals.ContainsKey(literal.ToString())) return;

            Literals.Add(literal.ToString(), literal);
        }

        /// <summary>
        /// Flips value of variable present in given literal. (in given valuation array) 
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <param name="id"></param>
        public void FlipLiteral(bool[] parameterValues, int id)
        {
            parameterValues[Literals.ElementAt(id).Value.VariableIndex] ^= true;
        }

        /// <summary>
        /// Returns number of literals in clause.
        /// </summary>
        /// <returns></returns>
        public int GetLiteralsCount()
        {
            return Literals.Count;
        }

        /// <summary>
        /// Returns weight of the clause.
        /// </summary>
        /// <returns></returns>
        public double GetWeight()
        {
            return Weight;
        }
    }
}
