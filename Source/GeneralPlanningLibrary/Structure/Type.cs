using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Represents type of object in model.
    /// </summary>
    public class Type
    {
        public string Name;
        
        /// <summary>
        /// Parent types.
        /// In the end not supported in program.
        /// </summary>
        public Type[] Parents;

        public Type(string Name) 
        {
            this.Name = Name;
            Parents = new Type[0];
        }

        /// <summary>
        /// Default type.
        /// </summary>
        public static Type NoType = new Type("object");
        
        /// <summary>
        /// Returns an array of given lenght filled with references to the NoType instance;
        /// </summary>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public static Type[][] NoTypeArray(int lenght)
        {
            Type[][] arr = new Type[lenght][];
            for (int i = 0; i < lenght; i++)
                arr[i] = new Type[] { NoType };

            return arr;
        }

        /// <summary>
        /// Returns if given type is child of current type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool IsFatherOf(Type t)
        {
            if (t == this) return true;

            foreach (Type x in t.Parents)
            {
                if (IsFatherOf(x)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if object of given type can be used as one of acceptableTypes' type.
        /// </summary>
        /// <param name="acceptableTypes"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool ParameterCanHaveType(Type[] acceptableTypes, Type t)
        {
            for (int i = 0; i < acceptableTypes.Length; i++)
            {
                if (acceptableTypes[i].IsFatherOf(t)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if object of some of requiredTypes' type can be used as one of acceptableTypes' type.
        /// </summary>
        /// <param name="acceptableTypes"></param>
        /// <param name="requiredTypes"></param>
        /// <returns></returns>
        public static bool ParameterCanHaveAnyType(Type[] acceptableTypes, Type[] requiredTypes)
        {
            for (int i = 0; i < requiredTypes.Length; i++)
            {
                if (!ParameterCanHaveType(acceptableTypes, requiredTypes[i])) return false;
            }

            return true;
        }
    }
}
