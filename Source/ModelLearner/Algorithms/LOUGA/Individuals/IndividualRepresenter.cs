using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOUGA.Individuals
{
    /// <summary>
    /// This class represents the way how a model is encoded to a general invidual.
    /// </summary>
    public class IndividualRepresenter
    {
        /// <summary>
        /// ID of "predicate-operator's list" pair in genome.
        /// Key is "predicate-operator-parametrization" string combination.
        /// </summary>
        public Dictionary<string, int> PairID;

        /// <summary>
        /// Dictionary storing PredicateReference objects representing a predicate in operator's lists.
        /// Key is "predicate-operator-parametrization" string combination.
        /// </summary>
        public Dictionary<string, PredicateReference> PairReference;

        /// <summary>
        /// Names of "predicate-operator" pairs represented by corresponding genes.
        /// </summary>
        public string[] Names;

        /// <summary>
        /// Lenght of genomes of individuals.
        /// </summary>
        public int GenomeLength;

        /// <summary>
        /// Original model.
        /// </summary>
        public Model M;

        /// <summary>
        /// Input worlds.
        /// </summary>
        public List<World> Worlds;

        /// <summary>
        /// List of possible predicates for each operator.
        /// Key is operator's name.
        /// Value is list of predicate's names with parametrizations.
        /// </summary>
        public Dictionary<string, List<string>> PossiblePredicates;
        
        /// <summary>
        /// List of genes that affect given operator.
        /// Key is operator's name.
        /// Value is list of indices in genome.
        /// </summary>
        public Dictionary<string, List<int>> OperatorIndices;

        public IndividualRepresenter(Model m, List<World> worlds)
        {
            List<string> names = new List<string>();
            PairID = new Dictionary<string, int>();
            PairReference = new Dictionary<string, PredicateReference>();
            PossiblePredicates = new Dictionary<string,List<string>>();
            OperatorIndices = new Dictionary<string, List<int>>();
            this.M = m;
            this.Worlds = worlds;   

            foreach (var pair in m.Operators)
            {
                Operator oper = pair.Value;

                List<string> list = new List<string>();
                List<int> indexList = new List<int>();
                List<PredicateReference> prList = oper.GeneratePossiblePredicates(m.Predicates, Properties.Settings.Default.LOUGA_ParametersCanRepeat);

                foreach (var pr in prList)
                {
                    list.Add(pr.ToString());
                    string id = GetName(oper.Name, pr.ToString());
                    PairID.Add(id, names.Count);
                    indexList.Add(names.Count);
                    PairReference.Add(id, pr);
                    names.Add(id);
                }

                PossiblePredicates.Add(oper.Name, list);
                OperatorIndices.Add(oper.Name, indexList);
            }

            Names = names.ToArray();
            GenomeLength = Names.Length;
        }

        /// <summary>
        /// Generates unique name for "predicate-operator's list" pair
        /// </summary>
        /// <returns></returns>
        public string GetName(string oper, string predicate)
        {
            string s = predicate + "->" + oper;
            return s;
        }

        public PredicateReference GetPredicateReference(string oper, string predicate)
        {
            return PairReference[GetName(oper, predicate)];
        }
    }
}
