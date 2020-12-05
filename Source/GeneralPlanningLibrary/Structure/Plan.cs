using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralPlanningLibrary
{
    public class Plan
    {
        /// <summary>
        /// List of actions forming the plan.
        /// </summary>
        public Action[] Actions;

        /// <summary>
        /// List of states between actions. Any field can be null if no information about corresponding state is given.
        /// Length = Actions.Lenght + 1
        /// </summary>
        public State[] States;
        
        public Plan(Action[] actions, State[] states)
        {
            this.Actions = actions;
            this.States = states;
        }

        /// <summary>
        /// Outputs plan in PDDL representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string outcome = "(:plan";
            for (int i = 0; i < Actions.Length; i++)
            {
                if (States[i] != null) outcome += "\n\t\t" + States[i].ToString();
                outcome += "\n\t\t" + Actions[i].ToString();
            }

            if (States[Actions.Length] != null) outcome += "\n\t\t" + States[Actions.Length].ToString();

            outcome += ")";

            return outcome;
        }
    }
}
