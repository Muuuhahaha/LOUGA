using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM2
{
    /// <summary>
    /// This class provides info about one state's parameter.
    /// </summary>
    class ParameterInfo
    {
        /// <summary>
        /// Type of objects that serve as parameter.
        /// </summary>
        public GeneralPlanningLibrary.Type ParameterType;

        /// <summary>
        /// Transitions asociated with the parameter.
        /// </summary>
        public List<TransitionInfo> AsociatedTransitions;

        public ParameterInfo(Hypothesis h)
        {
            ParameterType = h.GetParameterType();

            AsociatedTransitions = new List<TransitionInfo>();

            Operator operBefore = h.Matrix.Transitions[h.TransitionBefore].Operator;
            Operator operAfter = h.Matrix.Transitions[h.TransitionAfter].Operator;

            AsociatedTransitions.Add(new TransitionInfo(operBefore, h.IdBefore, TransitionType.AfterAction));
            AsociatedTransitions.Add(new TransitionInfo(operAfter, h.IdAfter, TransitionType.BeforeAction));
        }

        /// <summary>
        /// Tries to merge parameter info with another one.
        /// Merge will happen if infos have at least one transition in common.
        /// </summary>
        /// <param name="parameterInfo">ParameterInfo to merge with</param>
        /// <returns>TRUE if infos were merged successfully</returns>
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
