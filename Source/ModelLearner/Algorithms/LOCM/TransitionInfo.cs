using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM
{
    /// <summary>
    /// Info about one transition in PartialStateMachine.
    /// Transition is temporary state of PartialStateMachine that will be unified with other states to create correct machine.
    /// Transition consists of operator, index of its parameter and info whether it describes state before or after performing action.
    /// </summary>
    class TransitionInfo
    {
        /// <summary>
        /// Operator performing state transition.
        /// </summary>
        public Operator Operator;

        /// <summary>
        /// Index of operator's parameter that 
        /// </summary>
        public int ParameterIndex;

        /// <summary>
        /// Info whether the transition describes state before or after action is performed.
        /// </summary>
        public TransitionType TransitionType;

        public TransitionInfo(Operator o, int parameterIndex, TransitionType transitionType)
        {
            this.Operator = o;
            this.ParameterIndex = parameterIndex;
            this.TransitionType = transitionType;
        }

        /// <summary>
        /// Returns unique name for the transition.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Operator.Name + "-" + ParameterIndex + "-" + TransitionType;
        }

        /// <summary>
        /// Returns unique name for transition described by given parameters.
        /// </summary>
        /// <param name="o">Operator used in transition</param>
        /// <param name="parameterIndex"></param>
        /// <param name="transitionType"></param>
        /// <returns></returns>
        public static string GetName(Operator o, int parameterIndex, TransitionType transitionType)
        {
            return o.Name + "-" + parameterIndex + "-" + transitionType;
        }

        public string GetActionCode()
        {
            return Operator.Name + "-" + ParameterIndex;
        }

        public bool IsEqualTo(TransitionInfo info)
        {
            return Operator == info.Operator && ParameterIndex == info.ParameterIndex && TransitionType == info.TransitionType;
        }

        public override string ToString()
        {
            return GetName();
        }
    }

    /// <summary>
    /// Info whether the transition describes state before or after action is performed.
    /// </summary>
    enum TransitionType
    {
        AfterAction,
        BeforeAction
    }
}
