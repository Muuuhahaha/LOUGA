using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralPlanningLibrary;

namespace PlanGenerator
{
    /// <summary>
    /// This class represents state during generator's BFS.
    /// </summary>
    class PathState
    {
        /// <summary>
        /// Predicates of current state.
        /// </summary>
        public State State;

        /// <summary>
        /// Preceeding state in BFS.
        /// </summary>
        public int ParentState;

        /// <summary>
        /// Preceeding state in BFS.
        /// </summary>
        public PathState ParentStateReference;

        /// <summary>
        /// Action that created this state from previous state.
        /// </summary>
        public GeneralPlanningLibrary.Action PrecedingAction;

        /// <summary>
        /// Depth of this state in BFS.
        /// </summary>
        public int Depth;

        /// <summary>
        /// Predicates in this state that weren't in initial state of plan.
        /// </summary>
        public PredicateInstance[] NewPredicates;

        /// <summary>
        /// Goal predicates for a plan that ends in current state.
        /// </summary>
        public PredicateInstance[] GoalPredicates;

        public PathState(State state, int parentState, PathState parentStateReference, GeneralPlanningLibrary.Action precedingAction, int depth = 0)        
        {
            this.State = state;
            this.ParentStateReference = parentStateReference;
            this.ParentState = parentState;
            this.PrecedingAction = precedingAction;
            this.Depth = depth;
        }

        /// <summary>
        /// Finds predicates in current state that weren't in initial state.
        /// </summary>
        public void FindNewPredicates(State initState)
        {
            List<PredicateInstance> newPredicates = new List<PredicateInstance>();
            foreach (var predicate in State.Predicates)
            {
                if (!initState.Contains(predicate))
                {
                    newPredicates.Add(predicate);
                }
            }

            newPredicates.Sort();

            NewPredicates = newPredicates.ToArray();
        }

        /// <summary>
        /// Finds every combination of new predicates in this state that wasn't already found in another state and adds 
        /// it to given dictionary.
        /// </summary>
        /// <param name="goalStates">Dictionary with goal states found.</param>
        public void AddNewGoalStates(Dictionary<string, PathState> goalStates)
        {
            if (NewPredicates == null || NewPredicates.Length == 0) return;

            List<PredicateInstance> goals = new List<PredicateInstance>();

            for (int i = 0; i < NewPredicates.Length; i++)
            {
                goals.Add(NewPredicates[i]);
                AddNewGoalState(goals, goalStates);

                for (int j = i + 1; j < NewPredicates.Length; j++)
                {
                    goals.Add(NewPredicates[j]);
                    AddNewGoalState(goals, goalStates);

                    for (int k = j + 1; k < NewPredicates.Length; k++)
                    {
                        goals.Add(NewPredicates[k]);
                        AddNewGoalState(goals, goalStates);
                        goals.RemoveAt(2);
                    }

                    goals.RemoveAt(1);
                }

                goals.RemoveAt(0);
            }
        }

        /// <summary>
        /// Checks if given combination of goal predicates was already found and if nto, creates new PathState and inserts it
        /// in given dictionary.
        /// </summary>
        /// <param name="goals">Combinations of new predicates to check</param>
        /// <param name="goalStates">Dictionary with goal states found so far.</param>
        private void AddNewGoalState(List<PredicateInstance> goals, Dictionary<string, PathState> goalStates)
        {
            string hash = "";
            foreach (var predicate in goals)
            {
                hash += predicate.ToString() + " ";
            }

            if (goalStates.ContainsKey(hash)) return;

            PathState goalPathState = new PathState(State, ParentState, ParentStateReference, PrecedingAction, Depth);
            goalPathState.NewPredicates = this.NewPredicates;
            goalPathState.GoalPredicates = goals.ToArray();
            
            goalStates.Add(hash, goalPathState);
        }
    }
}
