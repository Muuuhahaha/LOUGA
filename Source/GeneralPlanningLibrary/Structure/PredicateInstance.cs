using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    /// <summary>
    /// Instance of predicate type.
    /// </summary>
    public class PredicateInstance : IEquatable<PredicateInstance>, IComparable<PredicateInstance>
    {
        public Predicate Type;
        public Object[] Parameters;

        public PredicateInstance() { }

        public PredicateInstance(Object[] parameters, Predicate type)
        {
            this.Parameters = parameters;
            this.Type = type;
        }

        /// <summary>
        /// Outputs predicate instance in PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string outcome = "(" + Type.Name;
            foreach (var o in Parameters)
            {
                outcome += " " + o.Name;
            }

            outcome += ")";

            return outcome;
        }

        /// <summary>
        /// Returns whether given predicate instance is indentical to current predicate instance.
        /// </summary>
        /// <param name="predicate">Predicate instance to compare current one to</param>
        /// <returns></returns>
        public bool Equals(PredicateInstance predicate)
        {
            if (predicate.Type != Type) return false;
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (predicate.Parameters[i] != Parameters[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Compares given predicate instance to current predicate instance.
        /// </summary>
        /// <param name="predicate">Predicate instance to compare current one to</param>
        /// <returns></returns>
        public int CompareTo(PredicateInstance predicate)
        {
            int c = Type.Name.CompareTo(predicate.Type.Name);
            if (c != 0) return c;
            c = Type.ParameterCount.CompareTo(predicate.Type.ParameterCount);
            if (c != 0) return c;
            for (int i = 0; i < Parameters.Length; i++)
            {
                c = Parameters[i].Name.CompareTo(predicate.Parameters[i].Name);
                if (c != 0) return c;
                c = Parameters[i].Type.Name.CompareTo(predicate.Parameters[i].Type.Name);
                if (c != 0) return c;
            }

            return 0;
        }

        public PredicateInstance Clone()
        {
            return new PredicateInstance((Object[])Parameters.Clone(), Type);
        }
    }
}
