using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM
{
    /// <summary>
    /// This class describes info about one machine's state parameter.
    /// </summary>
    class ParameterInfo
    {
        /// <summary>
        /// Type of object that serves as parameter.
        /// </summary>
        public GeneralPlanningLibrary.Type ParameterType;

        /// <summary>
        /// List of transitions asocieated with parameter.
        /// </summary>
        public List<string> AsociatedTransitions;

        public ParameterInfo(Hypothesis h)
        {
            ParameterType = h.GetParameterType();

            AsociatedTransitions = new List<string>();
            AsociatedTransitions.Add(h.TransitionBefore.GetName() + "-" + h.IdBefore);
            AsociatedTransitions.Add(h.TransitionAfter.GetName() + "-" + h.IdAfter);
        }

        /// <summary>
        /// Tries merging with another ParameterInfo if they have at least one transition in common.
        /// </summary>
        /// <param name="parameterInfo">PamrameterInfo to try merge with</param>
        /// <returns>TRUE if merging was successful</returns>
        public bool TryMerge(ParameterInfo parameterInfo)
        {
            if (ParameterType != parameterInfo.ParameterType) return false;

            bool found = false;
            foreach (var transition in parameterInfo.AsociatedTransitions)
            {
                if (AsociatedTransitions.Contains(transition))
                {
                    found = true;
                    break;
                }
            }

            if (!found) return false;

            foreach (var transition in parameterInfo.AsociatedTransitions)
            {
                if (!AsociatedTransitions.Contains(transition))
                {
                    AsociatedTransitions.Add(transition);
                }
            }

            return true;
        }
    }
}
