using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Definition of predicate type.
    /// </summary>
    public class Predicate
    {
        public string Name;
        public Type[][] ParameterTypes;
        public string[] ParameterNames;

        public int ParameterCount
        {
            get { return ParameterNames.Length; }
        }

        /// <summary>
        /// Ouputs predicate definition in PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "(" + Name;
            for (int i = 0; i < ParameterNames.Length; i++)
            {
                s += " " + ParameterNames[i];
                if (ParameterTypes[i][0] != Type.NoType)
                {
                    if (ParameterTypes[i].Length == 1)
                    {
                        s += " - " + ParameterTypes[i][0].Name;
                    }
                    else
                    {
                        s += " - (either";
                        for (int j = 0; j < ParameterTypes[i].Length; j++)
                        {
                            s += " " + ParameterTypes[i][j].Name;
                        }
                        s += ")";
                    }
                }
            }

            return s + ")";
        }

        /// <summary>
        /// Returns whether parameter of given index can have given type.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>
        public bool ParameterCanHaveType(Type t, int parameterIndex)
        {
            for (int i = 0; i < ParameterTypes[parameterIndex].Length; i++)
            {
                if (ParameterTypes[parameterIndex][i].IsFatherOf(t)) return true;
            }

            return false;
        }
    }
}
