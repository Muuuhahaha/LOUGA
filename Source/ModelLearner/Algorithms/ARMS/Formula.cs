using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// Class representing a formula in CNF;
    /// </summary>
    public class Formula
    {
        public const CollisionHandlingPolicy DefaultPolicy = CollisionHandlingPolicy.MaxWeight;

        /// <summary>
        /// Indices of variables (indexed by variable names).
        /// </summary>
        public Dictionary<string, int> VariableIndices;

        /// <summary>
        /// List of all variable names used.
        /// </summary>
        public List<string> VariableNames;

        /// <summary>
        /// ID if operator described by variable with the same index.
        /// -1 if variable is not representing any operator.
        /// </summary>
        public List<int> OperatorId;

        /// <summary>
        /// Dictionary of clauses present in formula.
        /// (Dictionary is used for collision handling of duplicate formulas).
        /// </summary>
        public Dictionary<string, Clause> Clauses;

        /// <summary>
        /// List of multiclauses representing frequent action pairs.
        /// </summary>
        public List<MultiClause> Multiclauses;


        /// <summary>
        /// Upper bound of how many predicates can operator have in pred, add or del lists
        /// </summary>
        public const int UpperBoud = 3;
        
        /// <summary>
        /// penalty for each operator that has too many predicates in one of its lists
        /// </summary>
        public const double Penalty = 10;

        public Formula()
        {
            this.VariableIndices = new Dictionary<string, int>();
            this.VariableNames = new List<string>();
            this.Clauses = new Dictionary<string, Clause>();
            this.Multiclauses = new List<MultiClause>();
            this.OperatorId = new List<int>();
        }

        public Formula(Dictionary<string, int> variableIndices, List<string> variableNames, Dictionary<string, Clause> clauses)
        {
            this.VariableIndices = variableIndices;
            this.VariableNames = variableNames;
            this.Clauses = clauses;
            this.Multiclauses = new List<MultiClause>();
            this.OperatorId = new List<int>();
        }

        /// <summary>
        /// Returns sum of weights of clauses and multiclauses.
        /// </summary>
        /// <returns></returns>
        public double GetMaxValue()
        {
            double total = 0;

            foreach (var clause in Clauses)
            {
                total += clause.Value.Weight;
            }

            foreach (var multiclause in Multiclauses)
            {
                total += multiclause.Weight;
            }

            return total;
        }

        /// <summary>
        /// Returns value of formula with given variable values.
        /// </summary>
        /// <param name="variableValues"></param>
        /// <returns></returns>
        public double GetValue(bool[] variableValues)
        {
            double total = 0;

            foreach (var pair in Clauses)
            {
                if (pair.Value.IsTrue(variableValues)) total += pair.Value.Weight;
            }

            foreach (var multiclause in Multiclauses)
            {
                if (multiclause.IsTrue(variableValues)) total += multiclause.Weight;
            }

            int maxid = 0;
            foreach (var id in OperatorId)
            {
                if (id > maxid) maxid = id;
            }

            int[] listSizes = new int[maxid + 1];
            for (int i = 0; i < variableValues.Length; i++)
            {
                if (OperatorId[i] == -1) continue;
                if (!variableValues[i]) continue;

                listSizes[OperatorId[i]]++;
            }

            for (int i = 0; i < listSizes.Length; i++)
            {
                if (listSizes[i] > UpperBoud) total -= Penalty;
            }

            return total;
        }

        /// <summary>
        /// Returns true if all clauses are satisfied with given valuation.
        /// </summary>
        /// <param name="variableValues"></param>
        /// <returns></returns>
        public bool AllTrue(bool[] variableValues)
        {
            foreach (var pair in Clauses)
            {
                if (!pair.Value.IsTrue(variableValues)) return false;
            }

            foreach (var multiclause in Multiclauses)
            {
                if (!multiclause.IsTrue(variableValues)) return false;
            }

            return true;
        }

        /// <summary>
        /// Parses formula from simple string. Used only for testing.
        /// Letters represents variables, words represents clauses. 
        /// Capital letter means variable negation, small letter represents a regualar variable.
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>Generated formula instance.</returns>
        public static Formula ParseFromString(string input)
        {
            string[] words = input.Split(new char[]{' ', '\t', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> variableIndices = new Dictionary<string, int>();
            List<string> variableNames = new List<string>();
            Dictionary<string, Clause> clauses = new Dictionary<string, Clause>();
            Formula formula = new Formula();

            foreach (var word in words)
            {
                Clause clause = new Clause();

                foreach (char c in word)
                {
                    if (!(c >= 'a' && c <= 'z') && !(c >= 'A' && c <= 'Z')) return null;

                    string param = c.ToString();
                    int index;

                    if (!variableIndices.TryGetValue(param.ToLower(), out index))
                    {
                        index = variableNames.Count;
                        variableIndices.Add(param.ToLower(), index);
                        variableNames.Add(param.ToLower());
                    }
                    bool notNegation = (c >= 'a' && c <= 'z');

                    clause.TryAddLiteral(new Literal(index, notNegation, formula));
                }

                clauses.Add(clause.GetName(), clause);
            }

            formula.VariableIndices = variableIndices;
            formula.VariableNames = variableNames;
            formula.Clauses = clauses;

            return formula;
        }

        /// <summary>
        /// Returns random valuation of variables.
        /// </summary>
        /// <returns></returns>
        public bool[] GetRandomValues()
        {
            bool[] valuation = new bool[VariableNames.Count];
            Random rg = new Random();
            for (int i = 0; i < valuation.Length; i++)
            {
                valuation[i] = rg.Next(2) == 0;
            }

            return valuation;
        }

        /// <summary>
        /// Returns printable representation of formula.
        /// </summary>
        /// <param name="multipleLines">True if clauses should be printed on separate lines.</param>
        /// <param name="printWeights"></param>
        /// <returns></returns>
        public string ToString(bool multipleLines, bool printWeights = true) 
        {
            string s = "";
            bool first = true;
            double maxValue = 0;

            foreach (var pair in Clauses)
            {
                if (!first)
                {
                    if (multipleLines) s += "\n";
                    s += " & ";
                }
                else first = false;
                
                s += pair.Value.Weight.ToString(ARMS.OutputDoubleFormat) + " " + pair.Key;
                maxValue += pair.Value.Weight;
            }

            foreach (MultiClause multiclause in Multiclauses)
            {
                if (!first)
                {
                    if (multipleLines) s += "\n";
                    s += " & ";
                }
                else first = false;

                s += multiclause.Weight.ToString(ARMS.OutputDoubleFormat) + " " + multiclause.ToString();
                maxValue += multiclause.Weight;
            }

            s += "\nMax Weight: " + maxValue.ToString(ARMS.OutputDoubleFormat);

            return s;
        }

        /// <summary>
        /// Returns printable string of variable names and their values.
        /// </summary>
        /// <param name="values">Values of variables</param>
        /// <returns></returns>
        public string GetPrintableString(bool[] values)
        {
            if (values.Length != VariableNames.Count) return "";

            string s = "";
            for (int i = 0; i < values.Length; i++)
            {
                s += " " + VariableNames[i] + ": " + values[i] + "\n";
            }

            return s;
        }

        /// <summary>
        /// Adds new variable if it's not already present. Returns its index.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="operatorId"></param>
        /// <returns>Index of the variable.</returns>
        public int AddParameterOrGetIndex(string variableName, int operatorId = -1)
        {
            int index;
            if (!VariableIndices.TryGetValue(variableName, out index))
            {
                index = VariableNames.Count;
                VariableNames.Add(variableName);
                VariableIndices.Add(variableName, index);
                OperatorId.Add(operatorId);
            }

            return index;
        }

        /// <summary>
        /// Adds given clause to the formula. If is the clause already present, solves the situation according to policy parameter.
        /// </summary>
        /// <param name="clause"></param>
        /// <param name="policy"></param>
        public void TryAddClause(Clause clause, CollisionHandlingPolicy policy = DefaultPolicy)
        {
            string name = clause.GetName();
            Clause orig;

            if (!Clauses.TryGetValue(name, out orig)) 
            {
                Clauses.Add(name, clause);

                return;
            }

            switch (policy)
            {
                case CollisionHandlingPolicy.Ignore:
                    break;
                case CollisionHandlingPolicy.Replace:
                    Clauses.Remove(name);
                    Clauses.Add(name, clause);
                    break;
                case CollisionHandlingPolicy.SumWeights:
                    orig.Weight += clause.Weight;
                    break;
                case CollisionHandlingPolicy.MaxWeight:
                    if (clause.Weight > orig.Weight) orig.Weight = clause.Weight;
                    break;
                case CollisionHandlingPolicy.MinWeight:
                    if (clause.Weight < orig.Weight) orig.Weight = clause.Weight;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Policy how to handle collisions when inserting new clause to formula.
    /// </summary>
    public enum CollisionHandlingPolicy 
    {
        Ignore, 
        Replace, 
        SumWeights,
        MaxWeight, 
        MinWeight
    }
}
