using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.ARMS
{
    /// <summary>
    /// This class represents frequent action pair in input plans.
    /// </summary>
    class ActionPair
    {
        public Operator FirstOperator;
        public Operator SecondOperator;
        public int SupportRate;

        /// <summary>
        /// Conector[i] describes which parameter of SecondOperator is connected to i-th parameter of FirstOpeator.
        /// </summary>
        public int[] Connector;

        /// <summary>
        /// Possible types of connected variables.
        /// </summary>
        private GeneralPlanningLibrary.Type[][] PossibleTypes;

        public ActionPair(Operator firstOperator, Operator secondOperator, int[] connector, int supportRate = 1)
        {
            this.FirstOperator = firstOperator;
            this.SecondOperator = secondOperator;
            this.Connector = connector;
            this.SupportRate = supportRate;

            this.PossibleTypes = null;
        }

        /// <summary>
        /// Returns unique name for given pair + their connector.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            string s = FirstOperator.Name + ":" + SecondOperator.Name + "(";

            for (int i = 0; i < Connector.Length; i++)
            {
                if (i > 0) s += ",";
                s += Connector[i];
            }
            s += ")";

            return s;
        }

        /// <summary>
        /// Generates 2 dimensional array of all possible types that connected parameters can have.
        /// </summary>
        private void GeneratePossibleTypes()
        {
            PossibleTypes = new GeneralPlanningLibrary.Type[Connector.Length][];

            for (int i = 0; i < Connector.Length; i++)
            {
                if (Connector[i] == -1) continue;

                List<GeneralPlanningLibrary.Type> types = new List<GeneralPlanningLibrary.Type>();
                for (int j = 0; j < FirstOperator.ParameterTypes[i].Length; j++)
                {
                    if (types.Contains(FirstOperator.ParameterTypes[i][j])) continue;

                    if (SecondOperator.ParameterCanHaveType(FirstOperator.ParameterTypes[i][j], Connector[i]))
                    {
                        types.Add(FirstOperator.ParameterTypes[i][j]);
                    }
                }
                for (int j = 0; j < SecondOperator.ParameterTypes[Connector[i]].Length; j++)
                {
                    if (types.Contains(SecondOperator.ParameterTypes[Connector[i]][j])) continue;

                    if (FirstOperator.ParameterCanHaveType(SecondOperator.ParameterTypes[Connector[i]][j], i))
                    {
                        types.Add(SecondOperator.ParameterTypes[Connector[i]][j]);
                    }
                }

                PossibleTypes[i] = types.ToArray();
            }
        }

        /// <summary>
        /// Generates possible predicates that can explain why does the action pair occur so frequently.
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public Tuple<List<PredicateReference>,List<PredicateReference>> GeneratePossiblePredicates(Dictionary<string, Predicate> predicates)
        {
            if (PossibleTypes == null) GeneratePossibleTypes();

            Tuple<List<PredicateReference>, List<PredicateReference>> possiblePredicates = new Tuple<List<PredicateReference>, List<PredicateReference>>(new List<PredicateReference>(), new List<PredicateReference>());

            foreach (var pair in predicates)
            {
                int[] firstOpParams = new int[pair.Value.ParameterNames.Length];
                int[] secondOpParams = new int[pair.Value.ParameterNames.Length];
                GeneratePredicatesRecursively(pair.Value, 0, firstOpParams, secondOpParams, possiblePredicates);
            }

            return possiblePredicates;
        }

        /// <summary>
        /// Recursively generates possible predicates that can explain why does the action pair occur so frequently.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="startingIndex"></param>
        /// <param name="firstOpParams"></param>
        /// <param name="secondOpParams"></param>
        /// <param name="finishedReferences"></param>
        private void GeneratePredicatesRecursively(Predicate predicate, int startingIndex, int[] firstOpParams, int[] secondOpParams, Tuple<List<PredicateReference>, List<PredicateReference>> finishedReferences)
        {
            //end of recursion
            if (startingIndex == predicate.ParameterCount)
            {
                finishedReferences.Item1.Add(new PredicateReference(predicate, (int[])firstOpParams.Clone(), null, FirstOperator));
                finishedReferences.Item2.Add(new PredicateReference(predicate, (int[])secondOpParams.Clone(), null, SecondOperator));
                return;
            }

            //chosing of parameter and next level of recursion
            for (int i = 0; i < Connector.Length; i++)
            {
                if (Connector[i] == -1) continue;

                foreach (GeneralPlanningLibrary.Type t in PossibleTypes[i])
                {
                    if (predicate.ParameterCanHaveType(t, startingIndex))
                    {
                        firstOpParams[startingIndex] = i;
                        secondOpParams[startingIndex] = Connector[i];
                        GeneratePredicatesRecursively(predicate, startingIndex + 1, firstOpParams, secondOpParams, finishedReferences);
                    }
                }
            }
        }
    }
}
