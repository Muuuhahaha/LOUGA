using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelLearner.Algorithms.LOUGA.FitnessFunctions;
using GeneralPlanningLibrary.Utility;

namespace ModelLearner.Algorithms.LOUGA.Individuals
{
    /// <summary>
    /// Class representing standard individual with integer genome.
    /// </summary>
    public class IntegerIndividual : IIndividual, IComparable<IntegerIndividual>, IEquatable<IntegerIndividual>
    {
        public double Fitness;

        /// <summary>
        /// Info about individual's errors.
        /// </summary>
        public ErrorInfo ErrorInfo;

        /// <summary>
        /// Individual's genome.
        /// </summary>
        public int[] Genome;

        /// <summary>
        /// Number of generations individual survived.
        /// </summary>
        public int Age;

        private static Random rng = new Random();

        /// <summary>
        /// Creates new individual. If given array of possibilities, also randomizes it's gene.
        /// </summary>
        /// <param name="genomeLength"></param>
        /// <param name="possibilities"></param>
        public IntegerIndividual(int genomeLength, bool[,] possibilities = null)
        {
            this.Fitness = double.MinValue;
            this.Age = 0;

            Genome = new int[genomeLength];
            if (possibilities != null)
            {
                for (int i = 0; i < genomeLength; i++)
                {
                    Genome[i] = rng.Next(IntegerIndividualRepresenter.MaxValue);
                    while (!possibilities[i, Genome[i]])
                    {
                        Genome[i] = rng.Next(IntegerIndividualRepresenter.MaxValue);
                    }
                }
            }
        }

        /// <summary>
        /// Creates individual from given string of numbers.
        /// Used for testing purposes.
        /// </summary>
        /// <param name="s"></param>
        public IntegerIndividual(string s)
        {
            this.Age = 0;
            this.Fitness = double.MinValue;

            this.Genome = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                Genome[i] = s[i] - '0';
            }

        }

        public IntegerIndividual(int[] genome)
        {
            this.Genome = genome;
            Fitness = double.MinValue;
        }

        public int this[int i]
        {
            get { return Genome[i]; }
            set { Genome[i] = value; }
        }

        public double GetFitness()
        {
            return Fitness;
        }

        public void SetFitness(double fitness)
        {
            this.Fitness = fitness;
        }

        public IntegerIndividual Clone()
        {
            IntegerIndividual ii = new IntegerIndividual((int[])Genome.Clone());
            ii.Fitness = Fitness;
            ii.ErrorInfo = ErrorInfo;
            return ii;
        }


        IIndividual IIndividual.Clone()
        {
            return this.Clone();
        }

        public int GetGeneLength()
        {
            return Genome.Length;
        }

        public int GetAge()
        {
            return Age;
        }

        public void IncrementAge()
        {
            Age++;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Genome.Length; i++)
            {
                s += Genome[i];
            }

            return s;
        }

        /// <summary>
        /// Compares fitness to other individual's fitness. If they are the same, the older one is considered better.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public int CompareTo(IntegerIndividual individual)
        {
            if (Fitness != individual.Fitness) return Fitness.CompareTo(individual.Fitness);

            return Age.CompareTo(individual.Age);
        }

        /// <summary>
        /// Returns true if individual's genome is identical to given individual's genome.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IntegerIndividual other)
        {
            for (int i = 0; i < Genome.Length; i++)
            {
                if (other.Genome[i] != Genome[i]) return false;
            }

            return true;
        }
    }
}
