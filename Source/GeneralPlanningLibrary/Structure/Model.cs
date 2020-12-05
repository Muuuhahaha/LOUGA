using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    public class Model
    {
        public string Name;
        public Dictionary<string, Operator> Operators;
        public Dictionary<string, Type> Types;
        public Dictionary<string, Predicate> Predicates;
        public Dictionary<string, Object> Constants;
        
        /// <summary>
        /// Array of requirements given model have.
        /// </summary>
        public string[] Requirements;

        /// <summary>
        /// True if model supports types.
        /// </summary>
        public bool Typing = false;

        /// <summary>
        /// Outputs model in PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string outcome = "(define (domain " + Name + ")\n";

            if (Requirements != null)
            {
                outcome += "\t (:requirements";
                foreach (string s in Requirements)
                {
                    outcome += " " + s;
                }
                outcome += ")\n";
            }

            if (Typing)
            {
                outcome += "\t (:types";
                foreach (var pair in Types)
                {
                    outcome += " " + pair.Key;
                }
                outcome += ")\n";
            }

            if (Constants != null && Constants.Count > 0)
            {
                outcome += "\t (:constants";
                foreach (var p in Constants)
                {
                    outcome += "\n\t\t" + p.Value.ToString();
                }
                outcome += ")\n";
            }
            
            if (Predicates != null)
            {
                outcome += "\t (:predicates";
                foreach (var p in Predicates)
                {
                    outcome += "\n\t\t" + p.Value.ToString();
                }
                outcome += ")\n";
            }
            if (Operators != null)
            {
                foreach (var p in Operators)
                {
                    outcome += "\t" + p.Value.ToString() + "\n";
                }
            }

            outcome += ")";

            return outcome;
        }

        /// <summary>
        /// Copies model. New model has same references to types, predicates and constants. 
        /// Clones only operators.
        /// </summary>
        /// <returns></returns>
        public Model MakeDeepCopy()
        {
            Model m = new Model();
            m.Name = (string)Name.Clone();
            m.Operators = new Dictionary<string, Operator>();
            foreach (var pair in Operators)
            {
                m.Operators.Add(pair.Key, pair.Value.MakeDeepCopy());
            }

            m.Types = new Dictionary<string, Type>();
            foreach (var pair in Types) m.Types.Add(pair.Key, pair.Value);

            m.Predicates = new Dictionary<string, Predicate>();
            foreach (var pair in Predicates) m.Predicates.Add(pair.Key, pair.Value);

            m.Constants = new Dictionary<string, Object>();
            foreach (var pair in Constants) m.Constants.Add(pair.Key, pair.Value);

            m.Requirements = (string[])Requirements.Clone();
            m.Typing = Typing;

            return m;
        }

        /// <summary>
        /// Deletes operators whose parameters have multiple type options and creates new operators whose parameters don't have type options.
        /// </summary>
        /// <param name="worlds"></param>
        public void RemoveOperatorsParameterTypeOptions(List<World> worlds = null)
        {
            Dictionary<string, Operator> newOperators = new Dictionary<string, Operator>();
            Dictionary<string, Operator> codedOperators = new Dictionary<string,Operator>();

            //creating new operators
            foreach (var pair in Operators)
            {
                Operator o = pair.Value;
                bool multipleOptions = false;
                string code = o.Name;
                for (int i = 0; i < o.ParameterCount; i++)
                {
                    if (o.ParameterTypes[i].Length > 1) multipleOptions = true;
                    code += "0";
                }
                if (multipleOptions)
                {
                    Operator temp = o.MakeDeepCopy();
                    int id = 0;
                    MakeOperators(o, temp, 0, ref id, o.Name, newOperators, codedOperators);
                }
                else
                {
                    newOperators.Add(o.Name, o);
                    codedOperators.Add(code, o);
                }
            }

            Operators = newOperators;

            foreach (var pair in Operators)
            {
                pair.Value.FormLists();
            }

            if (worlds == null) return;

            //updating worlds
            foreach (World w in worlds)
            {
                foreach (Plan p in w.Plans)
                {
                    for (int i = 0; i < p.Actions.Length; i++)
                    {
                        string code = p.Actions[i].Operator.Name + p.Actions[i].GetParameterTypesCode();

                        if (!codedOperators.TryGetValue(code, out p.Actions[i].Operator))
                        {
                            throw new Exception("Error in algorithm");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes precondition and effect trees from operators.
        /// </summary>
        public void RemoveOperatorInfo()
        {
            foreach (var oper in Operators)
            {
                oper.Value.Effects = null;
                oper.Value.Preconditions = null;
            }
        }

        /// <summary>
        /// Recursively removes parameter type options from given operator and creates multiple new operators.
        /// </summary>
        /// <param name="baseOperator">Original operator</param>
        /// <param name="newOperator">Partialy finished new operator</param>
        /// <param name="parameterIndex">Index of next parameter to update</param>
        /// <param name="id">Suffix to ad to new operator's name</param>
        /// <param name="code">Code of chosen types</param>
        /// <param name="newOperators">Already generated operators</param>
        /// <param name="codedOperators">Dictionary: code -> operator</param>
        private void MakeOperators(Operator baseOperator, Operator newOperator, int parameterIndex, ref int id, string code, Dictionary<string, Operator> newOperators, Dictionary<string, Operator> codedOperators)
        {
            if (parameterIndex == baseOperator.ParameterCount)
            {
                Operator final = newOperator.MakeDeepCopy();
                final.Name = baseOperator.Name + (id++);
                newOperators.Add(final.Name, final);
                codedOperators.Add(code, final);
                return;
            }

            for (int i = 0; i < baseOperator.ParameterTypes[parameterIndex].Length; i++)
            {
                newOperator.ParameterTypes[parameterIndex] = new Type[] { baseOperator.ParameterTypes[parameterIndex][i] };
                MakeOperators(baseOperator, newOperator, parameterIndex + 1, ref id, code + i, newOperators, codedOperators);
            }
        }
    }
}
