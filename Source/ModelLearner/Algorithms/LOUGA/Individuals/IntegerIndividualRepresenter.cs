using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOUGA.Individuals
{
    public class IntegerIndividualRepresenter : IndividualRepresenter
    {
        /// <summary>
        /// Number of possible gene's values.
        /// </summary>
        public const int MaxValue = 5;

        /// <summary>
        /// Algorithm presumes initial state is always complete.
        /// </summary>
        public const bool FirstStateIsComplete = true;

        public const int None = 0;

        /// <summary>
        /// Action adds predicate to state and requires it not to be present before action is carried out.
        /// </summary>
        public const int Add = 1;
        
        /// <summary>
        /// Action deletes predicate from state and requires it to be present before being carried out.
        /// </summary>
        public const int Delete = 2;

        /// <summary>
        /// Operator requires predicate to be present in state before action.
        /// This case is only used in post-processing after algorithm is finished.
        /// </summary>
        public const int Precondition = 3;

        /// <summary>
        /// Operator requires predicate NOT to be present in state before action.
        /// This case is only used in post-processing after algorithm is finished.
        /// </summary>
        public const int NegativePrecondition = 4;

        /// <summary>
        /// Possible[i,j] represents whether gene nr. i can have value j.
        /// </summary>
        public bool[,] Possible;

        /// <summary>
        /// Represents whether gene can be set to other option than to "None";
        /// </summary>
        public bool[] MultipleOptions;

        private static Random rng = new Random();

        public IntegerIndividualRepresenter(Model m, List<World> worlds) : base(m, worlds)
        {
            Possible = new bool[GenomeLength, MaxValue];
            MultipleOptions = new bool[GenomeLength];

            for (int i = 0; i < GenomeLength; i++)
            {
                for (int j = 0; j < MaxValue; j++)
                {
                    Possible[i, j] = true;
                }
                MultipleOptions[i] = true;
            }
        }

        /// <summary>
        /// Removes some possibilities from genes according to observations in input plans.
        /// If a predicate is present before action is carried out, that action cannot add it again.
        /// If a predicate is present after action is carried out, that action couldn't delete it.
        /// </summary>
        public void RemovePossibilities() 
        {
            foreach (var world in Worlds)
            {
                foreach (var plan in world.Plans)
                {
                    if (plan.Actions.Length == 0) continue;

                    if (FirstStateIsComplete)
                    {
                        GeneralPlanningLibrary.Action a = plan.Actions[0];
                        List<string> predicates = PossiblePredicates[a.Operator.Name];

                        foreach (var pred in predicates)
                        {
                            string key = GetName(a.Operator.Name, pred);
                            int id = PairID[key];
                            PredicateReference pr = PairReference[key];
                            PredicateInstance pi = a.InstantiatePredicate(pr);

                            if (plan.States[0] != null && plan.States[0].Contains(pi))
                            {
                                Possible[id, Add] = false;
                            }
                            else
                            {
                                Possible[id, Precondition] = false;
                                Possible[id, Delete] = false;
                            }
                        }
                    }


                    if (Properties.Settings.Default.LOUGA_GoalStateComplete)
                    {
                        GeneralPlanningLibrary.Action a = plan.Actions[plan.Actions.Length - 1];
                        State state = plan.States[plan.States.Length - 1];
                        List<string> predicates = PossiblePredicates[a.Operator.Name];

                        foreach (var pred in predicates)
                        {
                            string key = GetName(a.Operator.Name, pred);
                            int id = PairID[key];
                            PredicateReference pr = PairReference[key];
                            PredicateInstance pi = a.InstantiatePredicate(pr);

                            if (state != null && state.Contains(pi))
                            {
                                Possible[id, Delete] = false;
                            }
                            else
                            {
                                Possible[id, Add] = false;
                            }
                        }
                    }
                }
            }

            foreach (var world in Worlds)
            {
                foreach (var plan in world.Plans)
                {
                    if (plan.Actions.Length == 0) continue;

                    RemovePossibilitiesFromWholePlan(plan);
                }
            }

            for (int i = 0; i < GenomeLength; i++)
            {
                Possible[i, Precondition] = false;
                Possible[i, NegativePrecondition] = false;

                int count = 0;
                for (int j = 0; j < MaxValue; j++)
                {
                    if (Possible[i, j]) count++;
                }
                if (count == 0) throw new Exception("Error in input data.");
                if (count == 1) MultipleOptions[i] = false;
                else MultipleOptions[i] = true;
            }
        }

        /// <summary>
        /// Smarter way to remove possibilities. Maintains 2 list when going through plan:
        /// one for predicates that are definitely in current state and one for predicates that can be possible in current state in some model.
        /// If a predicate can't be in state before action, that action can't delete it. 
        /// If a predicate definitely is in state before action, that action can't add it.
        /// </summary>
        /// <param name="p">Plan to go through.</param>
        private void RemovePossibilitiesFromWholePlan(Plan p)
        {
            HashSet<string> predicatesDefinitelyInWorld = new HashSet<string>();
            HashSet<string> predicatesPossiblyInWorld = new HashSet<string>();

            if (p.States[0] != null)
            {
                foreach (var predicate in p.States[0].Predicates)
                {
                    predicatesDefinitelyInWorld.Add(predicate.ToString());
                }
            }


            foreach (var action in p.Actions)
            {
                var indices = OperatorIndices[action.Operator.Name];

                foreach (var index in indices)
                {
                    string predicate = action.InstantiatePredicate(PairReference[Names[index]]).ToString();

                    if (predicatesDefinitelyInWorld.Contains(predicate))
                    {
                        Possible[index, Add] = false;

                        predicatesDefinitelyInWorld.Remove(predicate);
                        predicatesPossiblyInWorld.Add(predicate);
                    }
                    else if (!predicatesPossiblyInWorld.Contains(predicate))
                    {
                        Possible[index, Delete] = false;

                        predicatesPossiblyInWorld.Add(predicate);
                    }
                }
            }
        }


        /// <summary>
        /// Switches a position in gene to random value, different than the previous one if not said otherwise.
        /// </summary>
        /// <param name="individual"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RandomizeGene(IntegerIndividual individual, int index, bool canBeSame = false)
        {
            if (!MultipleOptions[index] && !canBeSame) return false;

            int current = individual[index];
            int next = rng.Next(MaxValue);

            while ((next == current && !canBeSame) || !Possible[index, next])
            {
                next = rng.Next(MaxValue);
            }

            individual[index] = next;
            
            return true;
        }

        /// <summary>
        /// Outputs individual to printable string.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public string OutputModel(IntegerIndividual individual)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var pair in PossiblePredicates)
            {
                sb.AppendLine(pair.Key);
                string add = "  add: ";
                string pre = "  pre: ";
                string del = "  del: ";

                foreach (var predicate in pair.Value)
	            {
                    string name = GetName(pair.Key, predicate);
                    int i = PairID[name];
                    
                    switch (individual.Genome[i])
                    {
                        case Add:
                            add += predicate + " ";
                            break;
                        /*case Precondition:
                            pre += predicate + " ";
                            break;*/
                        case Delete:
                            del += predicate + " ";
                            pre += predicate + " ";
                            break;
                        default:
                            break;
                    }
	            }

                //Console.WriteLine(pre);
                sb.AppendLine(add);
                sb.AppendLine(del); 
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts individual to standard model representation.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public Model DecodeModel(IntegerIndividual individual, Predicate p = null)
        {
            return DecodeModel(individual.Genome, p);
        }

        /// <summary>
        /// Converts gene to standard model representation.
        /// </summary>
        /// <param name="genome"></param>
        /// <returns></returns>
        public Model DecodeModel(int[] genome, Predicate p = null)
        {
            FindPreconditions(genome);
            
            Model newModel = M.MakeDeepCopy();
            
            //lists representing each operator's lists
            Dictionary<string, List<PredicateReference>> preconditions = new Dictionary<string,List<PredicateReference>>();
            Dictionary<string, List<PredicateReference>> negativePreconditions = new Dictionary<string,List<PredicateReference>>();
            Dictionary<string, List<PredicateReference>> additions = new Dictionary<string, List<PredicateReference>>();
            Dictionary<string, List<PredicateReference>> deletes = new Dictionary<string, List<PredicateReference>>();

            foreach (var oper in newModel.Operators)
            {
                preconditions.Add(oper.Key, new List<PredicateReference>());
                negativePreconditions.Add(oper.Key, new List<PredicateReference>());
                additions.Add(oper.Key, new List<PredicateReference>());
                deletes.Add(oper.Key, new List<PredicateReference>());
            }

            //decoding lists from genes
            for (int i = 0; i < Names.Length; i++)
            {
                PredicateReference pr = PairReference[Names[i]];

                //skipping predicates different than p
                if (p != null && PairReference[Names[i]].Predicate != p) continue;

                switch (genome[i])
                {
                    case Add:
                        additions[pr.Operator.Name].Add(pr);
                        negativePreconditions[pr.Operator.Name].Add(pr);
                        break;
                    case Delete:
                        deletes[pr.Operator.Name].Add(pr);
                        preconditions[pr.Operator.Name].Add(pr);
                        break;
                    case Precondition:
                        preconditions[pr.Operator.Name].Add(pr);
                        break;
                    case NegativePrecondition:
                        negativePreconditions[pr.Operator.Name].Add(pr);
                        break;
                    default:
                        break;
                }
            }

            //forming operators from lists
            foreach (var pair in newModel.Operators)
            {
                Operator o = pair.Value;
                List<PredicateReference> prList = preconditions[o.Name];
                List<PredicateReference> negPrList = negativePreconditions[o.Name];
                List<PredicateReference> addList = additions[o.Name];
                List<PredicateReference> delList = deletes[o.Name];
                List<Tree> children;

                //precondition lists
                if (prList.Count + negPrList.Count == 0) o.Preconditions = null;
                else if (prList.Count == 1 && negPrList.Count == 0) o.Preconditions = new Tree(prList[0], o);
                else if (prList.Count == 0 && negPrList.Count == 1) o.Preconditions = new Tree(TreeType.Not, new Tree(negPrList[0], o), o);
                else
                {
                    children = new List<Tree>();
                    foreach (var pr in prList)
                    {
                        children.Add(new Tree(pr, o));
                    }
                    if (Properties.Settings.Default.LOUGA_GenerateNegativePreconditions)
                    {
                        foreach (var pr in negPrList)
                        {
                            children.Add(new Tree(TreeType.Not, new Tree(pr, o), o));
                        }
                    }

                    if (children.Count > 1)
                    {
                        o.Preconditions = new Tree(TreeType.And, children, o);
                    }
                    else if (children.Count == 1)
                    {
                        o.Preconditions = children[0];
                    }
                    else
                    {
                        o.Preconditions = null;
                    }
                }

                //effects list
                children = new List<Tree>();
                foreach (var pr in addList)
                {
                    children.Add(new Tree(pr, o));
                }
                foreach (var pr in delList)
                {
                    children.Add(new Tree(TreeType.Not, new Tree(pr, o), o));
                }

                if (children.Count == 0)
                {
                    o.Effects = null;
                }
                else if (children.Count == 1)
                {
                    o.Effects = children[0];
                }
                else
                {
                    o.Effects = new Tree(TreeType.And, children, o);
                }
            }

            return newModel;
        }

        /// <summary>
        /// This function return abstraction of DecodeModel function for OutputManager
        /// </summary>
        /// <returns></returns>
        public OutputManager.IndividualToModel GetDecodeFunction()
        {
            return new OutputManager.IndividualToModel((IntegerIndividual individual) => {
                return DecodeModel(individual);
            });
        }

        /// <summary>
        /// This function return abstraction of DecodeModel function for OutputManager.
        /// Returns DecodeModel function for given predicate
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public OutputManager.IndividualToModel GetDecodeFunctionForPredicate(Predicate p)
        {
            return new OutputManager.IndividualToModel((IntegerIndividual individual) =>
            {
                return DecodeModel(individual, p);
            });
        }
        
        /// <summary>
        /// Finds additional preconditions for given model.
        /// Goes through input sequnces and checks for each operator how many times is each relevant predicate present in state before performing action.
        /// If precentage of occurences is small enough, algorithm assumes the predicate is in operator's preconditon list.
        /// Also search for negative preconditions (= operator requires predicate not to be present before performing action).
        /// </summary>
        /// <param name="genome"></param>
        public void FindPreconditions(int[] genome)
        {
            for (int i = 0; i < GenomeLength; i++)
            {
                if (genome[i] == Precondition || genome[i] == NegativePrecondition)
                {
                    genome[i] = None;
                }
            }

            if (!Properties.Settings.Default.LOUGA_GenerateCompletePreconditionsLists) return;

            int[] negativeOccurences = new int[GenomeLength];
            int[] occurences = new int[GenomeLength];

            foreach (var world in Worlds)
            {
                foreach (var plan in world.Plans)
                {
                    List<PredicateInstance> state = new List<PredicateInstance>();

                    foreach (var predicate in plan.States[0].Predicates)
                    {
                        state.Add(predicate);
                    }

                    for (int i = 0; i < plan.Actions.Length; i++)
                    {
                        GeneralPlanningLibrary.Action a = plan.Actions[i];
                        Operator o = a.Operator;

                        List<string> predicates = PossiblePredicates[o.Name];

                        foreach (var str in predicates)
                        {
                            string s = this.GetName(o.Name, str);

                            PredicateInstance pi = a.InstantiatePredicate(PairReference[s]);
                            int id = PairID[s];

                            switch (genome[id])
                            {
                                case Add:
                                    if (!state.Contains(pi)) state.Add(pi);
                                    break;
                                case Delete:
                                    if (state.Contains(pi))
                                    {
                                        state.RemoveAt(state.FindIndex(x => x.Equals(pi)));
                                    }
                                    break;
                                case None:
                                    if (state.Contains(pi)) occurences[id]++;
                                    else negativeOccurences[id]++;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < GenomeLength; i++)
            {
                if (genome[i] != 0) continue;

                if ((occurences[i] > negativeOccurences[i] || !Properties.Settings.Default.LOUGA_GenerateNegativePreconditions) &&
                    (double)negativeOccurences[i] / (occurences[i] + negativeOccurences[i]) <= Properties.Settings.Default.LOUGA_MaxPredicateErrorRate)
                {
                    genome[i] = Precondition;
                }
                else if (negativeOccurences[i] > occurences[i] && Properties.Settings.Default.LOUGA_GenerateNegativePreconditions &&
                         (double)occurences[i] / (occurences[i] + negativeOccurences[i]) <= Properties.Settings.Default.LOUGA_MaxPredicateErrorRate)
                {
                    genome[i] = NegativePrecondition;
                }
            }
        }
    }
}
