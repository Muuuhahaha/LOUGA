using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary.Utility
{
    public class Utility
    {
        private static Random rng = new Random();

        /// <summary>
        /// Counts number of errors given model makes when ging through given worlds.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        /// <returns></returns>
        public static ErrorInfo CountErrors(Model m, List<World> worlds)
        {
            ErrorInfo info = new ErrorInfo();

            foreach (World w in worlds)
            {
                foreach (Plan p in w.Plans)
                {
                    List<PredicateInstance> state = new List<PredicateInstance>();
                    if (p.States[0] != null)
                    {
                        foreach (var pi in p.States[0].Predicates)
                        {
                            state.Add(new PredicateInstance(pi.Parameters, pi.Type));
                        }
                    }

                    for (int i = 0; i < p.Actions.Length; i++)
                    {
                        Action a = p.Actions[i];
                        a.Operator = m.Operators[a.Operator.Name];

                        //preconditions
                        var errors = CountPreconditionErrors(a, a.Operator.Preconditions, state);
                        info.ErrorsPre += errors.Item1;
                        info.TotalPre += errors.Item2;

                        //Effects
                        List<PredicateInstance> nextState;
                        var err = CountEffectErrorsAndApplicateAction(a, state, out nextState);
                        state = nextState;

                        info.ErrorsAdd += err.Item1;
                        info.TotalAdd += err.Item2;
                        info.ErrorsDel += err.Item3;
                        info.TotalDel += err.Item4;

                        //Observations
                        if (p.States[i + 1] == null) continue;

                        foreach (var predicate in p.States[i + 1].Predicates)
                        {
                            info.TotalObservation++;
                            if (!state.Contains(predicate)) info.ErrorsObservation++;
                        }

                    }
                }
            }

            return info;
        }

        /// <summary>
        /// Count number of missing preconditions and total number of preconditions
        /// </summary>
        /// <param name="a">Action that is being applicated</param>
        /// <param name="tree">Precondition tree</param>
        /// <param name="state">List of predicates in current state.</param>
        /// <returns>"Number of errors / number of checked predicates"</returns>
        private static Tuple<int, int> CountPreconditionErrors(Action a, Tree tree, List<PredicateInstance> state)
        {
            Tuple<int, int> tuple;

            if (tree == null) return new Tuple<int, int>(0, 0);

            switch (tree.Type)
            {
                case TreeType.Predicate:
                    PredicateInstance pi = a.InstantiatePredicate(tree.Predicate);
                    if (state.Contains(pi)) return new Tuple<int, int>(0, 1);
                    else return new Tuple<int, int>(1, 1);

                case TreeType.And:
                    int count = 0;
                    int errors = 0;

                    foreach (var child in tree.Children)
                    {
                        tuple = CountPreconditionErrors(a, child, state);
                        errors += tuple.Item1;
                        count += tuple.Item2;
                    }
                    return new Tuple<int, int>(errors, count);

                case TreeType.Or:
                    throw new Exception("not supported");

                case TreeType.Not:
                    tuple = CountPreconditionErrors(a, tree.Children[0], state);
                    return new Tuple<int, int>(1 - tuple.Item1, tuple.Item2);

                case TreeType.Equality:
                    throw new Exception("not supported");
                    
                default:
                    break;
            }

            throw new Exception("not supported");
        }

        /// <summary>
        /// Counts number of add and delete errors/errors and applicates actions to current state.
        /// </summary>
        /// <param name="a">Action t hat is being applicated</param>
        /// <param name="state">List of predicates in current state.</param>
        /// <param name="newState">State after action.</param>
        /// <returns>Number of add errors / total number of add operations / number of del errors / total number of del operations.</returns>
        private static Tuple<int, int, int, int> CountEffectErrorsAndApplicateAction(Action a, List<PredicateInstance> state, out List<PredicateInstance> newState)
        {
            Operator o = a.Operator;
            newState = state;
            if (o.Effects == null) return new Tuple<int, int, int, int>(0, 0, 0, 0);

            List<PredicateInstance> add = new List<PredicateInstance>();
            List<PredicateInstance> del = new List<PredicateInstance>();
            
            switch (o.Effects.Type)
            {
                case TreeType.Predicate:
                    add.Add(a.InstantiatePredicate(o.Effects.Predicate));
                    break;
                case TreeType.And:
                    foreach (var v in o.Effects.Children)
                    {
                        if (v.Type == TreeType.Predicate) add.Add(a.InstantiatePredicate(v.Predicate));
                        else if (v.Type == TreeType.Not) del.Add(a.InstantiatePredicate(v.Children[0].Predicate));
                        else throw new Exception("Effect list of operator " + o.Name + " has wrong format.");
                    }
                    break;
                case TreeType.Not:
                    if (o.Effects.Children[0].Type != TreeType.Predicate) throw new Exception("Effect list of operator " + o.Name + " has wrong format.");
                    del.Add(a.InstantiatePredicate(o.Effects.Children[0].Predicate));
                    break;
                default:
                    throw new Exception("Effect list of operator " + o.Name + " has wrong format.");
            }

            int errorsAdd = add.Count;
            int errorsDel = del.Count;

            newState = new List<PredicateInstance>();

            //deleting
            foreach (var predicate in state)
            {
                if (del.Contains(predicate))
                {
                    errorsDel--;
                }
                else
                {
                    newState.Add(predicate);
                }
            }

            //adding
            foreach (var predicate in add)
            {
                if (!newState.Contains(predicate))
                {
                    errorsAdd--;
                    newState.Add(predicate);
                }
            }

            return new Tuple<int, int, int, int>(errorsAdd, add.Count, errorsDel, del.Count);
        }



        /// <summary>
        /// Shuffles fields of given array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void Shuffle<T>(List<T> array)
        {
            for (int i = array.Count; i > 1; i--)
            {
                int index = rng.Next(i);
                if (index == i - 1) continue;

                T tmp = array[i - 1];
                array[i - 1] = array[index];
                array[index] = tmp;
            }
        }
        

        /// <summary>
        /// Removes types from model description and creates a new predicate for each type.
        /// Also resets types for worlds' objects.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        public static void RemoveTyping(Model m, List<World> worlds)
        {
            m.RemoveOperatorsParameterTypeOptions();
            m.Typing = false;

            List<string> req = new List<string>();
            for (int i = 0; i < m.Requirements.Length; i++)
            {
                if (m.Requirements[i] != ":typing") req.Add(m.Requirements[i]);
            }
            m.Requirements = req.ToArray();

            Dictionary<string, Predicate> types = new Dictionary<string,Predicate>();

            foreach (var pair in m.Predicates)
            {
                Predicate p = pair.Value;

                for (int i = 0; i < p.ParameterCount; i++)
                {
                    p.ParameterTypes[i] = new Type[] { Type.NoType };
                }
            }

            foreach (var type in m.Types)
            {
                string name = "type-" + type.Key;
                if (m.Predicates.ContainsKey(name))
                {
                    int i = 0;
                    while (m.Predicates.ContainsKey(name))
                    {
                        name = "type-" + type.Key + i;
                        i++;
                    }
                }

                Predicate p = new Predicate();
                p.Name = name;
                p.ParameterNames = new string[] { "?" + type.Key };
                p.ParameterTypes = new Type[][] { new Type[] { Type.NoType } };
                
                types.Add(type.Key, p);
                m.Predicates.Add(p.Name, p);
            }

            foreach (var item in m.Constants)
            {
                item.Value.Type = Type.NoType;
            }

            foreach (var pair in m.Operators)
            {
                Operator o = pair.Value;
                if (o.ParameterCount == 0) continue;

                if (o.Preconditions == null) o.Preconditions = new Tree(TreeType.And, new List<Tree>(), o);
                else if (o.Preconditions.Type != TreeType.And) o.Preconditions = new Tree(TreeType.And, o.Preconditions, o); 

                for (int i = 0; i < o.ParameterCount; i++)
                {
                    Type t = o.ParameterTypes[i][0];

                    PredicateReference pr = new PredicateReference(types[t.Name], new int[] { i }, null, o);
                    o.Preconditions.Children.Add(new Tree(pr, o));

                    o.ParameterTypes[i] = new Type[] { Type.NoType };
                }
            }

            m.Types = new Dictionary<string, Type>();
            m.Types.Add(Type.NoType.Name, Type.NoType);

            foreach (var world in worlds)
            {
                foreach (var plan in world.Plans)
                {
                    if (plan.States == null || plan.States[0] == null) continue;

                    State s = plan.States[0];

                    List<PredicateInstance> predicates = new List<PredicateInstance>();

                    foreach (var predicate in s.Predicates)
                    {
                        predicates.Add(predicate);
                    }

                    if (world.Objects != null)
                    {
                        foreach (var obj in world.Objects)
                        {
                            predicates.Add(new PredicateInstance(new Object[] { obj.Value }, types[obj.Value.Type.Name]));
                        }
                    }
                    if (m.Constants != null)
                    {
                        foreach (var obj in m.Constants)
                        {
                            predicates.Add(new PredicateInstance(new Object[] { obj.Value }, types[obj.Value.Type.Name]));
                        }
                    }

                    plan.States[0].Predicates = predicates.ToArray();
                }

                foreach (var obj in world.Objects)
                {
                    obj.Value.Type = Type.NoType;
                }
            }


        }
    }
}
