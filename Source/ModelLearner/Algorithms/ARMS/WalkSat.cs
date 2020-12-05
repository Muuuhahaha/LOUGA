using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// GSAT is algrotihm solving weighted MAX-SAT problem.
    /// Starts with random values and in each step picks one unsatisfied clause at random and flips one variable.
    /// Or with some probability flips a random variable.
    /// </summary>
    public class WalkSat : IMaxSatSolver
    {
        public bool[] Solve(Formula formula)
        {
            double globalMaximum = double.MinValue;
            double targetValue = formula.GetMaxValue();
            bool[] globalBestValues = new bool[formula.VariableNames.Count];
            int globalBestTrueCount = 0;

            Random rg = new Random();

            List<IClause> clauses = new List<IClause>();
            clauses.InsertRange(0, formula.Multiclauses);
            clauses.InsertRange(clauses.Count, formula.Clauses.Values);

            for (int n = 0; n < Properties.Settings.Default.ARMS_MaxSatNumberOfTries; n++)
            {
                double currentMaximum = double.MinValue;
                bool[] currentBestValues = new bool[formula.VariableNames.Count];
                int currentBestTrueCount = 0;
                
                int stepsInMaximum = 0;
                double currentValue = 0;
                bool[] values = formula.GetRandomValues();
                int currentTrueCount = GetTrueCount(values);

                while (stepsInMaximum < Properties.Settings.Default.ARMS_MaxSatRestartTreshold)
                {
                    currentValue = formula.GetValue(values);

                    //new maximum in current run
                    if (currentValue > currentMaximum)
                    {
                        currentMaximum = currentValue;
                        values.CopyTo(currentBestValues, 0);
                        currentBestTrueCount = currentTrueCount;
                        stepsInMaximum = 0;
                    }

                    //solution found
                    if (currentValue == targetValue) break;

                    int clauseId = rg.Next(clauses.Count);
                    while (clauses[clauseId].IsTrue(values)) clauseId = rg.Next(clauses.Count);

                    int index = -1;
                    //random flip
                    if (rg.NextDouble() < Properties.Settings.Default.ARMS_MaxSatRandomChoiceProbability)
                    {
                        index = rg.Next(clauses[clauseId].GetLiteralsCount());
                    }
                    //searching for best variable to flip
                    else
                    {
                        double bestValue = double.MinValue;

                        for (int i = 0; i < clauses[clauseId].GetLiteralsCount(); i++)
                        {
                            clauses[clauseId].FlipLiteral(values, i);

                            double value = formula.GetValue(values);
                            if (value > bestValue)
                            {
                                bestValue = value;
                                index = i;
                            }

                            clauses[clauseId].FlipLiteral(values, i);
                        }
                    }

                    clauses[clauseId].FlipLiteral(values, index);
                    currentTrueCount = GetTrueCount(values);

                    stepsInMaximum++;

                    if (Form1.Halt) return null;
                }

                //new global maximum
                if (currentMaximum > globalMaximum)
                {
                    globalMaximum = currentMaximum;
                    currentBestValues.CopyTo(globalBestValues, 0);
                    globalBestTrueCount = currentBestTrueCount;
                }

                ARMS.Log(" Try nr. " + n + " solution value: " + currentMaximum.ToString(ARMS.OutputDoubleFormat));

                //best possible solution found
                if (globalMaximum == targetValue)
                {
                    ARMS.Log("Found perfect solution");
                    break;
                }
            }

            //setting values of unnecesarry variables to false so that output model is as simple as possible.
            for (int i = 0; i < globalBestValues.Length; i++)
            {
                if (!globalBestValues[i]) continue;

                globalBestValues[i] = false;

                if (formula.GetValue(globalBestValues) < globalMaximum)
                {
                    globalBestValues[i] = true;
                }
            }

            return globalBestValues;
        }

        /// <summary>
        /// Returns number of parameters that are set to true in given parametrization.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private int GetTrueCount(bool[] values)
        {
            int c = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i]) c++;
            }

            return c;
        }
    }
}
