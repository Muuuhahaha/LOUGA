using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    public class World
    {
        public string ModelName;
        
        /// <summary>
        /// Name of world.
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Model the worlds uses.
        /// </summary>
        public Model Model;
        public Dictionary<string,Object> Objects; 
        
        /// <summary>
        /// Example plans in world
        /// </summary>
        public Plan[] Plans;

        /// <summary>
        /// Outputs world in PDDL format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string outcome = "(define (world " + Name + ")\n";
            outcome += "\t(:domain " + ModelName + ")\n";
            if (Objects != null) 
            {
                outcome += "\t(:objects ";
                foreach (var p in Objects)
                {
                    outcome += "\n\t\t" + p.Value.ToString();
                }
                outcome += ")\n";
            }
            foreach (Plan p in Plans)
            {
                outcome += "\t" + p.ToString() + "\n";
            }

            outcome += ")";

            return outcome;
        }

        /// <summary>
        /// Generates random instantiatized predicate.
        /// </summary>
        /// <returns></returns>
        public PredicateInstance GenerateRandomPredicateInstance()
        {
            Random rg = new Random();

            for (int tryNumber = 0; tryNumber < 1000; tryNumber++)
            {
                Predicate predicate = Model.Predicates.ElementAt(rg.Next(Model.Predicates.Count)).Value;

                Object[] parameters = new Object[predicate.ParameterCount];

                bool finished = true;
                //chosing random objects
                for (int i = 0; i < parameters.Length; i++)
                {
                    int possibleObjectsCount = 0;

                    foreach (var pair in Objects)
                    {
                        if (predicate.ParameterCanHaveType(pair.Value.Type, i)) possibleObjectsCount++;
                    }
                    foreach (var pair in Model.Constants)
                    {
                        if (predicate.ParameterCanHaveType(pair.Value.Type, i)) possibleObjectsCount++;
                    }

                    if (possibleObjectsCount == 0)
                    {
                        finished = false;
                        break;
                    }

                    int index = rg.Next(possibleObjectsCount);

                    foreach (var pair in Objects)
                    {
                        if (!predicate.ParameterCanHaveType(pair.Value.Type, i)) continue;

                        if (index == 0)
                        {
                            parameters[i] = pair.Value;
                            index--;
                            break;
                        }
                        else
                        {
                            index--;
                        }
                    }

                    foreach (var pair in Model.Constants)
                    {
                        if (predicate.ParameterCanHaveType(pair.Value.Type, i)) continue;

                        if (index == 0)
                        {
                            parameters[i] = pair.Value;
                            break;
                        }
                        else
                        {
                            index--;
                        }
                    }
                }

                if (finished)
                {
                    return new PredicateInstance(parameters, predicate);
                }
            }

            return null;
        }
    }
}
