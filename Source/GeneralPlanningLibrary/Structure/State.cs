using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    public class State
    {
        public PredicateInstance[] Predicates;

        public State(PredicateInstance[] predicates)
        {
            this.Predicates = predicates;
        }

        /// <summary>
        /// Outputs state reference in my version of PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string outcome = "(:state";

            if (Predicates != null)
            {
                foreach (var o in Predicates)
                {
                    outcome += "\n\t\t\t" + o.ToString();
                }
            }

            outcome += ")";

            return outcome;
        }

        /// <summary>
        /// Returns whether state contains given predicate instance.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(PredicateInstance predicate)
        {
            for (int i = 0; i < Predicates.Length; i++)
            {
                if (Predicates[i].Equals(predicate)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether state contains given predicates.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(List<PredicateInstance> predicates)
        {
            foreach (var predicate in predicates)
            {
                if (!Contains(predicate)) return false;
            }

            return true;
        }

        /// <summary>
        /// Clones the state.
        /// </summary>
        /// <returns></returns>
        public State Clone()
        {
            PredicateInstance[] predicates = new PredicateInstance[Predicates.Length];

            for (int i = 0; i < predicates.Length; i++)
            {
                predicates[i] = Predicates[i].Clone();
            }

            return new State(predicates);
        }
    }
}
