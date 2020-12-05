using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.Individuals;
using GeneralPlanningLibrary;
using GeneralPlanningLibrary.Utility;

namespace ModelLearner.Algorithms.LOUGA.FitnessFunctions
{
    /// <summary>
    /// This class serves as a speed up wrapper for general fitness function.
    /// Remembers previously computed values so that they don't have to be computed again.
    /// </summary>
    public class IntegerPrefixTreeWrapper : IFitnessFunction<IntegerIndividual>
    {
        /// <summary>
        /// Main fitness function.
        /// </summary>
        public IFitnessFunction<IntegerIndividual> Fitness;

        /// <summary>
        /// Prefix tree holding previously computed values.
        /// </summary>
        private PrefixTree Tree;

        /// <summary>
        /// statistical data
        /// </summary>
        private int FoundCount = 0;

        /// <summary>
        /// statistical data
        /// </summary>
        private int FailCount = 0;

        public IntegerPrefixTreeWrapper(IFitnessFunction<IntegerIndividual> fitness)
        {
            this.Fitness = fitness;
            this.Tree = new PrefixTree(IntegerIndividualRepresenter.MaxValue);
        }

        /// <summary>
        /// Returns value of given individual.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public double GetValue(IntegerIndividual individual)
        {
            if (Tree.Contains(individual))
            {
                FoundCount++;

                return Tree.GetValue(individual);
            }

            FailCount++;

            Fitness.GetValue(individual);
            Tree.Add(individual);

            return individual.Fitness;
        }



        /// <summary>
        /// Prefix tree class for fast value look-up.
        /// </summary>
        private class PrefixTree
        {
            /// <summary>
            /// Children of current node.
            /// </summary>
            public PrefixTree[] Children;

            public double Value;
            public GeneralPlanningLibrary.Utility.ErrorInfo ErrorInfo;

            public PrefixTree(int childrenCount)
            {
                Value = Double.MinValue;
                Children = new PrefixTree[childrenCount];
            }

            public bool Contains(IntegerIndividual individual)
            {
                return Contains(individual, 0);
            }

            private bool Contains(IntegerIndividual individual, int startingIndex)
            {
                if (startingIndex == individual.Genome.Length)
                {
                    return HasValue;
                }

                if (Children[individual[startingIndex]] == null)
                {
                    return false;
                }

                return Children[individual[startingIndex]].Contains(individual, startingIndex + 1);
            }


            public void Add(IntegerIndividual individual)
            {
                Add(individual, 0);
            }

            private void Add(IntegerIndividual individual, int startingIndex)
            {
                if (startingIndex == individual.Genome.Length)
                {
                    Value = individual.Fitness;
                    ErrorInfo = individual.ErrorInfo;
                    return;
                }

                int index = individual[startingIndex];
                if (Children[index] == null)
                {
                    Children[index] = new PrefixTree(Children.Length);
                }

                Children[index].Add(individual, startingIndex + 1);
            }


            public double GetValue(IntegerIndividual individual)
            {
                return GetValue(individual, 0);
            }

            private double GetValue(IntegerIndividual individual, int startingIndex)
            {
                if (startingIndex == individual.Genome.Length)
                {
                    individual.Fitness = Value;
                    individual.ErrorInfo = ErrorInfo;

                    return Value;
                }

                int index = individual[startingIndex];
                if (Children[index] == null) return double.MinValue;

                return Children[index].GetValue(individual, startingIndex + 1);
            }

            /// <summary>
            /// Returns true if this node has a value set.
            /// </summary>
            public bool HasValue
            {
                get { return Value != Double.MinValue; }
            }
        }
    }




}
