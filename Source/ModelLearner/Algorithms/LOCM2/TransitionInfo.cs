using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using GeneralPlanningLibrary;

namespace ModelLearner.Algorithms.LOCM2
{
    /// <summary>
    /// Info about one transition in PartialStateMachine.
    /// Transition is temporary state of PartialStateMachine that will be unified with other states to create correct machine.
    /// Transition consists of operator, index of its parameter and info whether it describes state before or after performing action.
    /// </summary>
    class TransitionInfo : IEquatable<TransitionInfo>
    {
        /// <summary>
        /// Unique name of the transition.
        /// </summary>
        public string Name;

        /// <summary>
        /// Operator performing state transition.
        /// </summary>
        public Operator Operator;

        /// <summary>
        /// Index of operator's parameter that 
        /// </summary>
        public int ParameterId;

        /// <summary>
        /// Info whether the transition describes state before or after action is performed.
        /// </summary>
        public TransitionType TransitionType;

        public TransitionInfo(Operator oper, int parameterId, TransitionType transitionType)
        {
            this.Operator = oper;
            this.ParameterId = parameterId;
            this.TransitionType = transitionType;
            this.Name = GetName(oper, parameterId, transitionType);
        }

        /// <summary>
        /// Returns unique name for given transition decription.
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="parameterId"></param>
        /// <param name="transitionType"></param>
        /// <returns></returns>
        public static string GetName(Operator oper, int parameterId, TransitionType transitionType = TransitionType.Universal)
        {
            string name = oper.Name + "." + parameterId;

            if (transitionType != TransitionType.Universal)
            {
                name += "-" + transitionType;
            }

            return name;
        }

        public bool Equals(TransitionInfo other)
        {
            return Name == other.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Info whether the transition describes state before or after action is performed.
    /// </summary>
    enum TransitionType
    {
        AfterAction,
        BeforeAction, 
        Universal
    }
}