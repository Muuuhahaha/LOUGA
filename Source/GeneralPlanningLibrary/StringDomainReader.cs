using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// This class decodes model's and worlds' representations from given string and finds all errors.
    /// </summary>
    public class StringDomainReader
    {
        /// <summary>
        /// Current index in input text
        /// </summary>
        private int Index;

        /// <summary>
        /// Current line ID
        /// </summary>
        private int Line;

        /// <summary>
        /// Index in current line.
        /// </summary>
        private int IndexInLine;

        /// <summary>
        /// Text that is being decoded
        /// </summary>
        private string Text;

        /// <summary>
        /// List of found errors.
        /// </summary>
        private List<InputException> Errors;

        /// <summary>
        /// Depth of recursion for preventing StackOverflowException
        /// </summary>
        private int _RecursionDepth;

        public StringDomainReader(string text)
        {
            this.Text = text.Replace((char)13 + "\n", "\n");
            Errors = new List<InputException>();

            _RecursionDepth = 0;
        }

        /// <summary>
        /// Decodes given string and returns found errors and decoded model and worlds if possible.
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="errors">Output array of found errors</param>
        /// <param name="worlds">Output array of decoded worlds</param>
        /// <returns>Decoded model</returns>
        public static Model DecodeString(string text, out List<InputException> errors, out List<World> worlds)
        {
            StringDomainReader sr = new StringDomainReader(text);

            Model m = sr.Decode(out worlds);
            errors = sr.Errors;

            return m;
        }

        /// <summary>
        /// Decodes given string and returns found errors and decoded model and worlds if possible.
        /// </summary>
        /// <param name="worlds">Output array of decoded worlds</param>
        /// <returns>Decoded model</returns>
        public Model Decode(out List<World> worlds)
        {
            List<ReaderTree> trees = new List<ReaderTree>();
            worlds = new List<World>();

            //parsing of input into trees
            ReaderTree tree = MakeTree();
            while (tree != null)
            {
                tree.FinishTreeInfo();
                trees.Add(tree);
                tree = MakeTree();
            }

            if (trees.Count == 0) return null;

            Model m = DecodeModel(trees[0]);

            if (m == null) return null;

            for (int i = 1; i < trees.Count; i++)
            {
                World w = DecodeWorld(trees[i], m);
                if (w != null)
                {
                    worlds.Add(w);
                }
            }

            return m;
        }

        /// <summary>
        /// Decodes model from given ReaderTree.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private Model DecodeModel(ReaderTree tree)
        {
            Model m = new Model();

            if (tree[1].Text != "define") { Errors.Add(new UnexpectedKeywordException("define", tree[1])); return null; }

            //model's name
            m.Name = DecodeDomainName(tree[2]);
            if (m.Name == null) return null;

            int i = 3;

            //model's requirements
            m.Requirements = DecodeRequirements(tree[i]);
            if (m.Requirements == null && Errors.Count > 0) return null;
            if (m.Requirements != null) i++;
            else m.Requirements = new string[0];
            if (m.Requirements.Contains(":typing")) m.Typing = true;

            
            //model's types
            if (m.Typing)
            {
                m.Types = DecodeTypes(tree[i]);
                if (m.Types == null && Errors.Count > 0) return null;
                if (m.Types != null && (m.Requirements == null || !m.Requirements.Contains(":typing"))) Errors.Add(new InputException("Model does not support typing", tree[i][1]));
                if (m.Types != null) i++;
            }
            else
            {
                m.Types = new Dictionary<string, Type>();
                m.Types.Add(Type.NoType.Name, Type.NoType);
            }

            //model's constants (if there are any)
            if (!tree[i].Leaf && tree[i][1].Text == ":constants")
            {
                m.Constants = DecodeObjects(tree[i], m, "constants");
                if (m.Constants == null && Errors.Count > 0) return null;
                if (m.Constants != null) i++;
            }
            if (m.Constants == null) m.Constants = new Dictionary<string, Object>();

            //model's predicates
            m.Predicates = DecodePredicates(tree[i], m);
            if (m.Predicates == null && Errors.Count > 0) return null;
            if (m.Predicates != null) i++;

            //model's operators
            m.Operators = new Dictionary<string, Operator>();
            while (i < tree.Children.Count - 1)
            {
                Operator o = DecodeOperator(tree[i], m);
                if (o == null) return null;

                m.Operators.Add(o.Name, o);
                i++;
            }

            return m;
        }

        /// <summary>
        /// Decodes name of model or world from given reader tree
        /// </summary>
        /// <param name="tree">Tree to decode name from</param>
        /// <param name="keyword">Should be "domain" for model or "world" for world</param>
        /// <returns></returns>
        private string DecodeDomainName(ReaderTree tree, string keyword = "domain")
        {
            if (tree.Leaf) { Errors.Add(new UnexpectedWordException("(", tree)); return null; }

            if (tree[1].Text != keyword) { Errors.Add(new UnexpectedKeywordException(keyword, tree[1])); return null; }
            if (!tree[2].Leaf || tree.Children.Count == 3 || !IsCorrectName(tree[2].Text)) { Errors.Add(new InputException("Expected name.", tree[2])); return null; }
            if (tree.Children.Count > 4) { Errors.Add(new UnexpectedWordException(")", tree[3])); }

            return tree[2].Text;
        }

        /// <summary>
        /// Decodes requirements of model
        /// </summary>
        /// <param name="tree">Tree to decode requirements from</param>
        /// <returns></returns>
        private string[] DecodeRequirements(ReaderTree tree)
        {
            if (tree.Leaf) { Errors.Add(new UnexpectedWordException("(", tree)); return null; }

            if (tree[1].Text != ":requirements") return null; // Not error - requirements aren't needed

            string[] requirements = new string[tree.Children.Count - 3];

            for (int i = 2; i < tree.Children.Count - 1; i++)
            {
                if (!tree[i].Leaf || tree[i].Text[0] != ':' || !IsCorrectName(tree[i].Text.Substring(1))) { Errors.Add(new InputException("Expected requirements keyword.", tree[i])); return null; }
                requirements[i - 2] = tree[i].Text;
            }

            return requirements;
        }

        /// <summary>
        /// Decodes types used in model.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private Dictionary<string, Type> DecodeTypes(ReaderTree tree)
        {
            if (tree.Leaf) { Errors.Add(new UnexpectedWordException("(", tree)); return null; }
            if (tree[1].Text != ":types") { Errors.Add(new UnexpectedKeywordException(":types", tree[1])); return null; }

            Dictionary<string, Type> types = new Dictionary<string, Type>();

            for (int i = 2; i < tree.Children.Count - 1; i++)
            {
                if (!tree[i].Leaf || !IsCorrectName(tree[i].Text)) { Errors.Add(new InputException("Expected type definition.", tree[i])); return null; }
                if (types.ContainsKey(tree[i].Text)) { Errors.Add(new InputException("Duplicate type definition.", tree[i])); return null; }

                types.Add(tree[i].Text, new Type(tree[i].Text));
            }

            return types;
        }

        /// <summary>
        /// Decodes predicates definitons from given reader tree
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private Dictionary<string, Predicate> DecodePredicates(ReaderTree tree, Model m)
        {
            if (tree.Leaf) { Errors.Add(new UnexpectedWordException("(", tree)); return null; }
            if (tree[1].Text != ":predicates") { Errors.Add(new UnexpectedKeywordException(":predicates", tree[1])); return null; }

            Dictionary<string, Predicate> predicates = new Dictionary<string, Predicate>();

            for (int i = 2; i < tree.Children.Count - 1; i++)
            {
                if (tree[i].Leaf) { Errors.Add(new InputException("Expected predicate definition.", tree[i])); continue; }

                Predicate p = DecodePredicate(tree[i], m);
                if (p == null) return null;

                if (predicates.ContainsKey(p.Name)) { Errors.Add(new InputException("Duplicate predicate definition.", tree[i][1])); }
                else predicates.Add(p.Name, p);
            }

            return predicates;
        }

        /// <summary>
        /// Decodes definition of single predicate from given tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m">Model the predicate is part of</param>
        /// <returns></returns>
        private Predicate DecodePredicate(ReaderTree tree, Model m)
        {
            if (!tree[1].Leaf || !IsCorrectName(tree[1].Text)) { Errors.Add(new InputException("Expected predicate name.", tree[1])); return null; }

            Predicate p = new Predicate();
            p.Name = tree[1].Text;

            p.ParameterNames = DecodeParametersDefinition(tree, 2, m, out p.ParameterTypes);
            if (p.ParameterNames == null) return null;

            return p;
        }

        /// <summary>
        /// Decodes definitions of parameters in predicate's or operator's definitions.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="index">index in reader tree the decoding should start at (2 for predicate and 1 for operator)</param>
        /// <param name="m"></param> 
        /// <param name="types">Output field of possible types of parameters</param>
        /// <returns></returns>
        private string[] DecodeParametersDefinition(ReaderTree tree, int index, Model m, out Type[][] types)
        {
            List<Type[]> typesList = new List<Type[]>();
            List<string> names = new List<string>();
            types = null;

            List<string> curNames = new List<string>();
            for (int i = index; i < tree.Children.Count - 1; i++)
            {
                //parameter name
                if (tree[i].Leaf && IsCorrectParamName(tree[i].Text))
                {
                    if (curNames.Contains(tree[i].Text) || names.Contains(tree[i].Text)) { Errors.Add(new InputException("Parameter '" + tree[i].Text + "' is already defined", tree[i])); continue; }
                    curNames.Add(tree[i].Text);

                }
                //setting of preceeding parameters' types
                else if (tree[i].Text == "-")
                {
                    if (!m.Requirements.Contains(":typing")) { Errors.Add(new InputException("Model does not support typing", tree[i])); i++; continue; }
                    if (curNames.Count == 0) { Errors.Add(new InputException("No preceeding parameters definitions.", tree[i])); i++; continue; }
                    i++;

                    Type[] t = DecodeType(tree[i], m);
                    if (t == null)
                    {
                        curNames.Clear();
                        continue;
                    }

                    foreach (var name in curNames)
                    {
                        typesList.Add(t);
                        names.Add(name);
                    }
                    curNames.Clear();
                }
                else { Errors.Add(new InputException("Expected parameter definition.", tree[i])); }
            }

            if (curNames.Count > 0)
            {
                if (m.Requirements.Contains(":typing")) { Errors.Add(new InputException("Expected type reference.", tree[tree.Children.Count - 1])); return null; }

                foreach (var name in curNames)
                {
                    names.Add(name);
                    typesList.Add(new Type[] { Type.NoType });
                }
            }

            types = typesList.ToArray();
            return names.ToArray();
        }

        /// <summary>
        /// Decodes possible types of a parameter in predicate's or operator's definition.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private Type[] DecodeType(ReaderTree tree, Model m)
        {
            if (tree.Leaf)
            {
                if (m.Types.ContainsKey(tree.Text))
                {
                    return new Type[] { m.Types[tree.Text] };
                }

                else { Errors.Add(new InputException("Type \"" + tree.Text + "\" not found", tree)); return null; }
            }

            else
            {
                if (tree[1].Text != "either") { Errors.Add(new UnexpectedKeywordException("either", tree[1])); return null; }

                List<Type> types = new List<Type>();

                for (int i = 2; i < tree.Children.Count - 1; i++)
                {
                    if (m.Types.ContainsKey(tree[i].Text))
                    {
                        types.Add(m.Types[tree[i].Text]);
                    }
                    else { Errors.Add(new InputException("Type \"" + tree[i].Text + "\" not found", tree[i])); return null; }
                }

                return types.ToArray();
            }
        }

        /// <summary>
        /// Decodes operator's definition from ReaderTree
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private Operator DecodeOperator(ReaderTree tree, Model m)
        {
            Operator o = new Operator();

            if (tree.Leaf) { Errors.Add(new InputException("Expected operator definition", tree)); return null; }
            if (tree[1].Text != ":action") { Errors.Add(new UnexpectedKeywordException(":action", tree[1])); return null; }
            if (!tree[2].Leaf || !IsCorrectName(tree[2].Text)) { Errors.Add(new InputException("Expected operator's name", tree[2])); return null; }
            o.Name = tree[2].Text;

            int i = 3;
            if (tree[i].Text == ":parameters")
            {
                if (tree[i + 1].Leaf) { Errors.Add(new InputException("Expected parameters' definitions", tree[i + 1])); return null; }
                o.ParameterNames = DecodeParametersDefinition(tree[i + 1], 1, m, out o.ParameterTypes);
                if (o.ParameterNames == null) return null;

                i += 2;
            }

            if (tree[i].Text == ":precondition")
            {
                o.Preconditions = DecodeTree(tree[i + 1], m, o);

                i += 2;
            }

            if (tree[i].Text == ":effect")
            {
                o.Effects = DecodeTree(tree[i + 1], m, o);

                i += 2;
            }

            while (i != tree.Children.Count - 1)
            {
                Errors.Add(new UnexpectedWordException(tree[i]));
                i++;
            }

            if (o.ParameterNames == null)
            {
                o.ParameterNames = new string[0];
                o.ParameterTypes = new Type[0][];
            }

            return o;
        }

        /// <summary>
        /// Decodes precondition or effect tree from given ReaderTree
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m">Model the operator is part of</param>
        /// <param name="currentOperator">Operator whose tree this method decodes</param>
        /// <returns></returns>
        private Tree DecodeTree(ReaderTree tree, Model m, Operator currentOperator)
        {
            if (tree.Leaf) 
            { 
                Errors.Add(new InputException("Expected opening bracket", tree)); 
                return null; 
            }
            if (!tree[1].Leaf) 
            { 
                Errors.Add(new InputException("Expected and, not or predicate reference", tree[1]));
                return null; 
            }

            //tree is AND or NOT bracket
            if (tree[1].Text == "and" || tree[1].Text == "not")
            {
                TreeType type;
                if (tree[1].Text == "and") type = TreeType.And;
                else type = TreeType.Not;

                List<Tree> children = new List<Tree>();

                for (int i = 2; i < tree.Children.Count - 1; i++)
                {
                    Tree child = DecodeTree(tree[i], m, currentOperator);
                    if (child == null)
                    {
                        return null;
                    }

                    children.Add(child);
                }

                if (type == TreeType.Not && (children.Count != 1 || (children[0].Type != TreeType.Predicate && children[0].Type != TreeType.Equality))) Errors.Add(new InputException("Operator NOT should have only one predicate or equality comparison as a parameter.", tree[1]));
                
                if (children.Count == 0) 
                { 
                    Errors.Add(new InputException("Expected opening bracket", tree[2])); 
                    return null; 
                }
                if (children.Count == 1) return new Tree(type, children[0], currentOperator);
                else return new Tree(type, children, currentOperator);
            }
            //tree is equality comparison
            else if (tree[1].Text == "=")
            {
                if (tree.Children.Count != 5) { Errors.Add(new InputException("Comparison has wrong number of arguments", tree[1])); }

                int[] parameterIds = new int[2];
                Object[] constantsReferences = new Object[2];

                for (int i = 2; i < 4; i++)
                {
                    if (m.Constants != null && m.Constants.ContainsKey(tree[i].Text))
                    {
                        parameterIds[i - 2] = -1;
                        constantsReferences[i - 2] = m.Constants[tree[i].Text];
                    }
                    else
                    {
                        bool found = false;

                        if (currentOperator.ParameterNames != null)
                        {
                            for (int j = 0; j < currentOperator.ParameterNames.Length; j++)
                            {
                                if (currentOperator.ParameterNames[j] == tree[i].Text)
                                {
                                    parameterIds[i - 2] = j;
                                    found = true;
                                }
                            }
                        }

                        if (!found)
                        {
                            Errors.Add(new InputException("Object or parameter reference not found", tree[i]));
                            continue;
                        }
                    }
                }

                return new Tree(parameterIds, constantsReferences, currentOperator);
            }
            //tree is predicate reference
            else
            {
                Predicate p;
                if (!m.Predicates.TryGetValue(tree[1].Text, out p)) { Errors.Add(new InputException("Expected and, not or predicate reference", tree[1])); return null; }

                if (tree.Children.Count - 3 != p.ParameterCount) { Errors.Add(new InputException("Predicate has wrong number of parameters", tree[1])); }

                int[] parameterIds = new int[p.ParameterCount];
                Object[] constantsReferences = new Object[p.ParameterCount];

                int argc = p.ParameterCount;
                if (argc > tree.Children.Count - 3) argc = tree.Children.Count - 3;

                //decoding of predicate's parameters
                for (int i = 2; i < argc + 2; i++)
                {
                    if (m.Constants != null && m.Constants.ContainsKey(tree[i].Text))
                    {
                        parameterIds[i - 2] = -1;
                        constantsReferences[i - 2] = m.Constants[tree[i].Text];
                    }
                    else
                    {
                        bool found = false;

                        if (currentOperator.ParameterNames != null)
                        {
                            for (int j = 0; j < currentOperator.ParameterNames.Length; j++)
                            {
                                if (currentOperator.ParameterNames[j] == tree[i].Text)
                                {
                                    for (int k = 0; k < currentOperator.ParameterTypes[j].Length; k++)
                                    {
                                        if (p.ParameterCount <= i - 2)
                                        {
                                            Errors.Add(new InputException("Too many parameters", tree[i]));
                                        }
                                        else if (!p.ParameterCanHaveType(currentOperator.ParameterTypes[j][k], i - 2))
                                        {
                                            Errors.Add(new InputException("Parameter has wrong type", tree[i]));
                                        }
                                    }

                                    parameterIds[i - 2] = j;
                                    found = true;
                                }
                            }
                        }

                        if (!found)
                        {
                            Errors.Add(new InputException("Object or parameter reference not found", tree[i]));
                            continue;
                        }
                    }
                }

                PredicateReference pr = new PredicateReference(p, parameterIds, constantsReferences, currentOperator);
                return new Tree(pr, currentOperator);
            }
        }

        /// <summary>
        /// Decodes world'd definition from given reader tree
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private World DecodeWorld(ReaderTree tree, Model m)
        {
            World w = new World();

            if (tree[1].Text != "define") { Errors.Add(new UnexpectedKeywordException("define", tree[1])); return null; }

            w.Name = DecodeDomainName(tree[2], "world");
            if (w.Name == null) return null;

            w.ModelName = DecodeDomainName(tree[3], ":domain");
            if (w.ModelName == null) return null;
            if (w.ModelName != m.Name)
            {
                Errors.Add(new InputException("Wrong model", tree[3][2]));
            }
            w.Model = m;

            w.Objects = DecodeObjects(tree[4], m);
            if (w.Objects == null) return null;

            List<Plan> plans = new List<Plan>();
            for (int i = 5; i < tree.Children.Count - 1; i++)
            {
                Plan p = DecodePlan(tree[i], m, w);
                if (p != null) plans.Add(p);
            }

            w.Plans = plans.ToArray();
            return w;
        }

        /// <summary>
        /// Decodes plan from given reader tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m">Model the plan is part of.</param>
        /// <param name="w">World the plan is part of.</param>
        /// <returns></returns>
        private Plan DecodePlan(ReaderTree tree, Model m, World w)
        {
            if (tree.Leaf) { Errors.Add(new InputException("Expected plan", tree)); return null; }
            if (tree[1].Text != ":plan") { Errors.Add(new UnexpectedKeywordException(":plan", tree[1])); return null; }

            List<State> states = new List<State>();
            List<Action> actions = new List<Action>();
            State s = null;

            for (int i = 2; i < tree.Children.Count - 1; i++)
            {
                if (tree[i].Leaf) { Errors.Add(new UnexpectedWordException(tree[i])); continue; }

                if (tree[i][1].Leaf && tree[i][1].Text == ":state")
                {
                    if (s != null) { Errors.Add(new InputException("Expected action", tree[i])); }
                    s = DecodeState(tree[i], w, m);
                }
                else
                {
                    Action a = DecodeAction(tree[i], w, m);
                    if (a != null)
                    {
                        states.Add(s);
                        actions.Add(a);

                        s = null;
                    }
                }
            }

            states.Add(s);

            return new Plan(actions.ToArray(), states.ToArray());
        }

        /// <summary>
        /// Decodes action from given reader tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m">Model the action is part of.</param>
        /// <param name="w">World the action is part of.</param>
        /// <returns></returns>
        private Action DecodeAction(ReaderTree tree, World w, Model m)
        {
            if (tree.Leaf) { Errors.Add(new InputException("Expected action", tree)); return null; }

            if (!tree[1].Leaf) { Errors.Add(new InputException("Expected operator reference", tree)); return null; }
            if (!m.Operators.ContainsKey(tree[1].Text)) { Errors.Add(new InputException("Operator not found", tree[1])); return null; }

            Operator o = m.Operators[tree[1].Text];
            Object[] parameters = new Object[o.ParameterCount];

            if (tree.Children.Count - 3 != o.ParameterCount) { Errors.Add(new InputException("Action has wrong number of arguments", tree[1])); }

            int argc = o.ParameterCount;
            if (o.ParameterCount > tree.Children.Count - 3) argc = tree.Children.Count - 3;

            //Decoding of parameters
            for (int i = 2; i < 2 + argc; i++)
            {
                if (!tree[i].Leaf) { Errors.Add(new InputException("Expected object", tree[i])); continue; }

                Object obj = null;
                if ((m.Constants == null || !m.Constants.TryGetValue(tree[i].Text, out obj)) && 
                    (w.Objects == null || !w.Objects.TryGetValue(tree[i].Text, out obj))) 
                {
                    Errors.Add(new InputException("Object not found", tree[i])); 
                    continue; 
                }

                if (!o.ParameterCanHaveType(obj.Type, i - 2)) { Errors.Add(new InputException("Object has wrong type", tree[i])); continue; }

                parameters[i - 2] = obj;
            }

            return new Action(o, parameters);
        }

        /// <summary>
        /// Decodes state from given reader tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m">Model the state is part of.</param>
        /// <param name="w">World the state is part of.</param>
        /// <returns></returns>
        private State DecodeState(ReaderTree tree, World w, Model m)
        {
            if (tree.Leaf) { Errors.Add(new InputException("Expected state", tree)); return null; }
            if (tree[1].Text != ":state") { Errors.Add(new UnexpectedKeywordException(":state", tree[1])); return null; }

            List<PredicateInstance> predicates = new List<PredicateInstance>();

            for (int i = 2; i < tree.Children.Count - 1; i++)
            {
                PredicateInstance predicate = DecodePredicateInstance(tree[i], w, m);
                if (predicate != null)
                {
                    if (!predicates.Contains(predicate))
                    {
                        predicates.Add(predicate);
                    }
                    else
                    {
                        Errors.Add(new InputException("Predicate is already present in state.", tree[i]));
                    }
                }
            }

            return new State(predicates.ToArray());
        }

        /// <summary>
        /// Decodes predicate instance from given reader tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m">Model the predicate instance is part of.</param>
        /// <param name="w">World the predicate instance is part of.</param>
        /// <returns></returns>
        private PredicateInstance DecodePredicateInstance(ReaderTree tree, World w, Model m)
        {
            if (tree.Leaf) { Errors.Add(new InputException("Expected predicate", tree)); return null; }
            if (!tree[1].Leaf) { Errors.Add(new InputException("Expected predicate reference", tree)); return null; }

            Predicate p;
            if (!m.Predicates.TryGetValue(tree[1].Text, out p)) { Errors.Add(new InputException("Predicate not found", tree[1])); return null; }
            Object[] parameters = new Object[p.ParameterCount];

            if (tree.Children.Count - 3 != p.ParameterCount) { Errors.Add(new InputException("Predicate has wrong number of arguments", tree[1])); }

            int argc = p.ParameterCount;
            if (p.ParameterCount > tree.Children.Count - 3) argc = tree.Children.Count - 3;

            for (int i = 2; i < 2 + argc; i++)
            {
                if (!tree[i].Leaf) { Errors.Add(new InputException("Expected object", tree[i])); continue; }

                Object obj = null;
                if ((m.Constants == null || !m.Constants.TryGetValue(tree[i].Text, out obj)) &&
                    (w.Objects == null || !w.Objects.TryGetValue(tree[i].Text, out obj)))
                {
                    Errors.Add(new InputException("Object not found", tree[i]));
                    continue;
                }

                if (!p.ParameterCanHaveType(obj.Type, i - 2)) { Errors.Add(new InputException("Object has wrong type", tree[i])); continue; }

                parameters[i - 2] = obj;
            }

            return new PredicateInstance(parameters, p);
        }

        /// <summary>
        /// Decodes objects declarations used in a world or in a model (defined as constants)
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="m"></param>
        /// <param name="keyword">"objects" for world or "constants" for model</param>
        /// <returns></returns>
        private Dictionary<string, Object> DecodeObjects(ReaderTree tree, Model m, string keyword = "objects")
        {
            if (tree.Leaf) { Errors.Add(new InputException("Expected " + keyword + " definition", tree)); return null; }
            if (tree[1].Text != ":" + keyword) { Errors.Add(new UnexpectedKeywordException(":" + keyword, tree[1])); return null; }

            Dictionary<string, Object> objects = new Dictionary<string,Object>();
            List<string> names = new List<string>();
            for (int i = 2; i < tree.Children.Count - 1; i++)
            {
                if (!tree[i].Leaf) { Errors.Add(new InputException("Expected " + keyword + " declaration", tree[i])); continue; }

                // defining type of preceeding objects
                if (tree[i].Text == "-")
                {
                    if (!m.Requirements.Contains(":typing")) { Errors.Add(new InputException("Model does not support typing", tree[i])); i++; continue; }
                    if (names.Count == 0) { Errors.Add(new InputException("No preceeding parameters definitions", tree[i])); i++; continue; }
                    i++;

                    Type[] t = DecodeType(tree[i], m);
                    if (t == null)
                    {
                        names.Clear();
                        continue;
                    }
                    if (t.Length > 1) { Errors.Add(new InputException("Object can have only 1 type", tree[i])); i++; continue; }

                    foreach (var name in names)
                    {
                        objects.Add(name, new Object(name, t[0]));
                    }
                    names.Clear();
                }
                //new object's name
                else if (IsCorrectName(tree[i].Text))
                {
                    if (objects.ContainsKey(tree[i].Text) || names.Contains(tree[i].Text) || (m.Constants != null && m.Constants.ContainsKey(tree[i].Text)))
                    {
                        Errors.Add(new InputException("Object \"" + tree[i].Text + "\" already exists", tree[i])); 
                        continue; 
                    }

                    names.Add(tree[i].Text);
                }
                else { Errors.Add(new InputException("Expected object declaration", tree[i])); continue; }
            }

            if (names.Count > 0)
            {
                if (m.Requirements.Contains(":typing")) { Errors.Add(new InputException("Expected type", tree[tree.Children.Count - 1])); }
                else
                {
                    foreach (var name in names)
                    {
                        objects.Add(name, new Object(name, Type.NoType));
                    }
                }
            }

            return objects;
        }

        /// <summary>
        /// Returns true if given word is in correct name format.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool IsCorrectName(string word)
        {
            return Regex.IsMatch(word, "[a-zA-Z][a-zA-Z0-9-_]*");
        }

        /// <summary>
        /// Returns true if given word is in correct parameter name format. (parameters' names start with '?')
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private bool IsCorrectParamName(string word)
        {
            return word[0] == '?' && IsCorrectName(word.Substring(1));
        }

        /// <summary>
        /// Decodes part of code from opening bracket till coresponding closing bracket.
        /// </summary>
        /// <returns></returns>
        private ReaderTree MakeTree()
        {
            List<ReaderTree> children = new List<ReaderTree>();

            _RecursionDepth++;
            if (_RecursionDepth >= 1000) throw new Exception("Too many opening brackets.");

            //waiting for opening bracket
            while (Index < Text.Length && Text[Index] != '(')
            {
                switch (Text[Index])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                        IncrementIndex();
                        break;

                    //comment
                    case ';':
                        while (Index < Text.Length && Text[Index] != '\n') IncrementIndex();
                        IncrementIndex();
                        break;

                    default:
                        int end = Index;
                        while (end < Text.Length && Text[end] != ' ' && Text[end] != '\t' && Text[end] != '\n' && Text[end] != ';' && Text[end] != '(') end++;

                        Errors.Add(new UnexpectedWordException("(", Line, IndexInLine, Index, end - Index));
                        while (Index < end) IncrementIndex();
                        break;
                }
            }

            //no opening bracket found
            if (Index == Text.Length)
            {
                _RecursionDepth--;
                return null;
            }

            children.Add(new ReaderTree("(", Line, IndexInLine, Index));
            IncrementIndex();

            //body of bracket
            while (Index < Text.Length && Text[Index] != ')')
            {
                switch (Text[Index])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                        IncrementIndex();
                        break;

                    //comment
                    case ';':
                        while (Index < Text.Length && Text[Index] != '\n') IncrementIndex();
                        IncrementIndex();
                        break;

                    //sub-bracket
                    case '(':
                        children.Add(MakeTree());
                        break;

                    case '?':
                    case ':':
                        if (Index == Text.Length - 1 || !IsLetter(Text[Index + 1]))
                        {
                            Errors.Add(new UnexpectedCharException(Text[Index], Line, IndexInLine, Index, 1));
                            IncrementIndex();
                        }
                        else
                        {
                            IncrementIndex();
                            children.Add(ReadWord(1));
                        }
                        break;

                    //equality comparison
                    case '=':
                        children.Add(new ReaderTree("=", Line, IndexInLine, Index));
                        IncrementIndex();

                        break;

                    default:
                        if (!IsNameChar())
                        {
                            int line = Line;
                            int indexInLine = IndexInLine;
                            int index = Index;

                            while (!IsUsefulChar()) IncrementIndex();

                            int length = Index - index;

                            Errors.Add(new UnexpectedWordException(line, indexInLine, index, length));
                        }
                        else children.Add(ReadWord());
                        break;
                }
            }

            //no closing bracket found
            if (Index == Text.Length)
            {
                int index = Index - 1;

                while (Text[index] == '\n' || Text[index] == ' ' || Text[index] == '\t') index--;
                Errors.Add(new InputException("Unexpected end of file", Line, IndexInLine, index, Index - index));
            }

            children.Add(new ReaderTree(")", Line, IndexInLine, Index));
            if (Index < Text.Length) IncrementIndex();

            _RecursionDepth--;
            return new ReaderTree(children);
        }

        /// <summary>
        /// Returns reader tree representation of next world in input string.
        /// </summary>
        /// <param name="prefixLength">Defines how many characters befire current index should be taken as part of the word.</param>
        /// <returns></returns>
        private ReaderTree ReadWord(int prefixLength = 0)
        {
            int beg = Index - prefixLength;
            int length = prefixLength;
            while (beg + length < Text.Length && IsNameChar(Text[beg + length]))
            {
                IncrementIndex();
                length++;
            }

            return new ReaderTree(Text.Substring(beg, length), Line, IndexInLine - length, Index - length);
        }

        /// <summary>
        /// Moves cursor to next character. Adjusts necesarry indices.
        /// </summary>
        private void IncrementIndex()
        {
            if (Index == Text.Length) return;

            if (Text[Index] == '\n')
            {
                IndexInLine = -1;
                Line++;
            }

            IndexInLine++;
            Index++;
        }

        /// <summary>
        /// Returns true if next char in input string is useful in PDDL representation.
        /// </summary>
        /// <returns></returns>
        private bool IsUsefulChar()
        {
            return IsUsefulChar(Text[Index]);
        }

        /// <summary>
        /// Returns true if given char is useful in PDDL representation.
        /// </summary>
        /// <returns></returns>
        private bool IsUsefulChar(char c)
        {
            return IsNameChar(c) || IsWhiteChar(c) || c == '(' || c == ')' || c == ';';
        }

        /// <summary>
        /// Returns true if given char is a letter
        /// </summary>
        /// <returns></returns>
        private bool IsLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// Returns true if next char in input string can be used in a name.
        /// </summary>
        /// <returns></returns>
        private bool IsNameChar()
        {
            return IsNameChar(Text[Index]);
        }

        /// <summary>
        /// Returns true if given char can be used in a name.
        /// </summary>
        /// <returns></returns>
        private bool IsNameChar(char c)
        {
            return IsLetter(c) || c == '_' || c == '-' || (c >= '0' && c <= '9');
        }

        /// <summary>
        /// Returns true if given char is space, tab or new line char.
        /// </summary>
        /// <returns></returns>
        private bool IsWhiteChar(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        enum WordType
        {
            Keyword,
            Name,
            definition
        }
    }

    /// <summary>
    /// Exception representing error in input string.
    /// Isn't usually thrown.
    /// </summary>
    public class InputException : Exception
    {
        public string Text;
        public int Line;
        public int IndexInLine;
        public int Index;
        public int Length;

        public InputException(string text, int line, int indexInLine, int index, int length)
            : base(text + " at line " + line + " char " + indexInLine + ".")
        {
            this.Text = text;
            this.Line = line;
            this.IndexInLine = indexInLine;
            this.Index = index;
            this.Length = length;
        }

        public InputException(string text, ReaderTree tree)
            : this(text, tree.Line, tree.IndexInLine, tree.Index, tree.Length)
        {
        }

        public override string ToString()
        {
            return Text + "\n Line: " + Line + " char: " + IndexInLine;
        }
    }

    /// <summary>
    /// Exception representing unexpected char error in input string.
    /// Isn't usually thrown.
    /// </summary>
    public class UnexpectedCharException : InputException
    {
        public char Char;

        public UnexpectedCharException(char c, int line, int indexInLine, int index, int length)
            : base("Unexpected character \"" + c + "\"", line, indexInLine, index, length)
        {
            this.Char = c;
        }
    }


    /// <summary>
    /// Exception representing unexpected keyword error in input string.
    /// Isn't usually thrown.
    /// </summary>
    public class UnexpectedKeywordException : InputException
    {
        public string ExpectedKeyword;

        public UnexpectedKeywordException(string expectedKeyword, int line, int indexInLine, int index, int length)
            : base("Expected keyword \"" + expectedKeyword + "\".", line, indexInLine, index, length)
        {
            this.ExpectedKeyword = expectedKeyword;
        }

        public UnexpectedKeywordException(string expectedKeyword, ReaderTree tree)
            : this(expectedKeyword, tree.Line, tree.IndexInLine, tree.Index, tree.Length)
        {
        }
    }


    /// <summary>
    /// Exception representing unexpected word error in input string.
    /// Isn't usually thrown.
    /// </summary>
    public class UnexpectedWordException : InputException
    {
        public string ExpectedKeyword;

        public UnexpectedWordException(string expectedWord, int line, int indexInLine, int index, int length)
            : base("Expected \"" + expectedWord + "\".", line, indexInLine, index, length)
        {
            this.ExpectedKeyword = expectedWord;
        }

        public UnexpectedWordException(int line, int indexInLine, int index, int length)
            : base("Unexpected word.", line, indexInLine, index, length)
        {
        }

        public UnexpectedWordException(string expectedWord, ReaderTree tree)
            : this(expectedWord, tree.Line, tree.IndexInLine, tree.Index, tree.Length)
        {
        }

        public UnexpectedWordException(ReaderTree tree)
            : this(tree.Line, tree.IndexInLine, tree.Index, tree.Length)
        {
        }
    }
}
