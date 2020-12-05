using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// GSAT is algrotihm solving weighted MAX-SAT problem.
    /// Starts with random values and in each step flips one variable so that it minimalizes number of unsatisfied clauses.
    /// Or with some probability flips a random variable.
    /// </summary>
    class GSAT : IMaxSatSolver
    {
        public bool[] Solve(Formula formula)
        {
            double globalMaximum = double.MinValue;
            double targetValue = formula.GetMaxValue();
            bool[] globalBestValues = new bool[formula.VariableNames.Count];
            int globalBestTrueCount = 0;

            Random rg = new Random();

            for (int n = 0; n < Properties.Settings.Default.ARMS_MaxSatNumberOfTries; n++)
            {
                //inicialization of run
                double currentMaximum = double.MinValue;
                bool[] currentBestValues = new bool[formula.VariableNames.Count];
                int currentBestTrueCount = 0;
                
                int stepsInMaximum = 0;
                double currentValue = 0;
                bool[] values = formula.GetRandomValues();

                int trueCount = 0;
                for (int i = 0; i < values.Length; i++)
                    if (values[i]) trueCount++;

                //computing part of algorithm
                while (stepsInMaximum < Properties.Settings.Default.ARMS_MaxSatRestartTreshold)
                {
                    currentValue = formula.GetValue(values);
                    
                    //new maximum in current run
                    if (currentValue > currentMaximum)
                    {
                        currentMaximum = currentValue;
                        values.CopyTo(currentBestValues, 0);
                        stepsInMaximum = 0;
                        currentBestTrueCount = trueCount;
                    }

                    //solution found
                    if (currentValue == targetValue) break;

                    int index = -1;
                    //random flip
                    if (rg.NextDouble() < Properties.Settings.Default.ARMS_MaxSatRandomChoiceProbability)
                    {
                        index = rg.Next(values.Length);
                    }
                    //searching for best variable to flip
                    else
                    {
                        double bestValue = double.MinValue;

                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] ^= true;

                            double value = formula.GetValue(values);
                            if (value > bestValue/* || (value == bestValue && !values[i])*/)
                            {
                                bestValue = value;
                                index = i;
                            }
                            values[i] ^= true;
                        }
                    }

                    values[index] ^= true;
                    
                    if (values[index]) trueCount++;
                    else trueCount--;

                    stepsInMaximum++;

                    if (Form1.Halt) return null;
                }

                //new global maximum
                if (currentMaximum > globalMaximum || (currentMaximum == globalMaximum && currentBestTrueCount > globalBestTrueCount))
                {
                    globalMaximum = currentMaximum;
                    globalBestTrueCount = currentBestTrueCount;
                    currentBestValues.CopyTo(globalBestValues, 0);
                }

                ARMS.Log(" Try nr. " + n + " solution value: " + currentMaximum.ToString(ARMS.OutputDoubleFormat));

                //best possible solution found
                if (currentMaximum == targetValue)
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
    }
}
