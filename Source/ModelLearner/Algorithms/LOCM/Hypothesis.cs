using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM
{
    /// <summary>
    /// Hypothesis that state between two transitions has a parameter.
    /// </summary>
    class Hypothesis
    {
        /// <summary>
        /// Transition before related state.
        /// </summary>
        public TransitionInfo TransitionBefore;

        /// <summary>
        /// Transition after related state.
        /// </summary>
        public TransitionInfo TransitionAfter;

        /// <summary>
        /// Id of parameter in transition before state.
        /// </summary>
        public int IdBefore;

        /// <summary>
        /// ID of parameter in transition after state.
        /// </summary>
        public int IdAfter;

        public Hypothesis(TransitionInfo transitionBefore, TransitionInfo transitionAfter, int idBefore, int idAfter)
        {
            this.TransitionBefore = transitionBefore;
            this.TransitionAfter = transitionAfter;
            this.IdBefore = idBefore;
            this.IdAfter = idAfter;
        }

        /// <summary>
        /// Returns unique name of hypothesis.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return GetName(TransitionBefore, TransitionAfter);
        }

        /// <summary>
        /// Returns unique name of hypothesis.
        /// </summary>
        public static string GetName(TransitionInfo transitionBefore, TransitionInfo transitionAfter)
        {
            return transitionBefore.GetName() + "->" + transitionAfter.GetName();
        }

        /// <summary>
        /// Returns true if hypothesis is true for given two consecutive actions.
        /// First checks whether the hypothesis concerns given actions.
        /// </summary>
        /// <param name="actionBefore"></param>
        /// <param name="actionAfter"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsTrue(GeneralPlanningLibrary.Action actionBefore, GeneralPlanningLibrary.Action actionAfter, GeneralPlanningLibrary.Object obj)
        {
            if (TransitionBefore.Operator != actionBefore.Operator) return true;
            if (actionBefore.Parameters[TransitionBefore.ParameterIndex] != obj) return true;
            if (TransitionAfter.Operator != actionAfter.Operator) return true;
            if (actionAfter.Parameters[TransitionAfter.ParameterIndex] != obj) return true;

            return actionBefore.Parameters[IdBefore] == actionAfter.Parameters[IdAfter];
        }

        /// <summary>
        /// Returns type of parameter the hypothesis is describing.
        /// </summary>
        /// <returns></returns>
        public GeneralPlanningLibrary.Type GetParameterType()
        {
            return TransitionBefore.Operator.ParameterTypes[IdBefore][0];
        }

        /// <summary>
        /// Ouptuts hypothesis in printable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TransitionBefore.GetName() + "-" + IdBefore + "->" + TransitionAfter.GetName() +"-" + IdAfter;
        }
    }
}
