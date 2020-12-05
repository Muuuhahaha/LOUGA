using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM2
{
    /// <summary>
    /// Hypothesis that state between two transitions has a parameter.
    /// </summary>
    class Hypothesis
    {
        /// <summary>
        /// ID of transition before 
        /// </summary>
        public int TransitionBefore;
        public int TransitionAfter;
        public int IdBefore;
        public int IdAfter;
        public int StateId;
        public TransitionMatrix Matrix;

        public Hypothesis(int transitionBefore, int transitionAfter, int idBefore, int idAfter, int stateId, TransitionMatrix matrix)
        {
            this.TransitionAfter = transitionAfter;
            this.TransitionBefore = transitionBefore;
            this.IdBefore = idBefore;
            this.IdAfter = idAfter;
            this.Matrix = matrix;
            this.StateId = stateId;
        }

        public bool IsTrue(GeneralPlanningLibrary.Action actionBefore, GeneralPlanningLibrary.Action actionAfter, GeneralPlanningLibrary.Object obj)
        {
            if (Matrix.Transitions[TransitionBefore].Operator != actionBefore.Operator) return true;
            if (actionBefore.Parameters[Matrix.Transitions[TransitionBefore].ParameterId] != obj) return true;
            if (Matrix.Transitions[TransitionAfter].Operator != actionAfter.Operator) return true;
            if (actionAfter.Parameters[Matrix.Transitions[TransitionAfter].ParameterId] != obj) return true;

            return actionBefore.Parameters[IdBefore] == actionAfter.Parameters[IdAfter];
        }

        /// <summary>
        /// Returns type of parameter the hypothesis is about.
        /// </summary>
        /// <returns></returns>
        public GeneralPlanningLibrary.Type GetParameterType()
        {
            return Matrix.Transitions[TransitionBefore].Operator.ParameterTypes[IdBefore][0];
        }

        public override string ToString()
        {
            return Matrix.Transitions[TransitionBefore].Name + "-" + IdBefore + "->" + Matrix.Transitions[TransitionAfter].Name + "-" + IdAfter;
        }
    }
}
