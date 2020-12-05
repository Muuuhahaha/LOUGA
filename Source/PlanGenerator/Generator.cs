﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeneralPlanningLibrary;

namespace PlanGenerator
{
    public class Generator
    {
        private Random RG;

        private const int ReportAfterNumberOfStates = 10;

        public static int initStateId = 0;
        
        public Generator()
        {
            this.RG = new Random();
        }

        /// <summary>
        /// Generates a random input file for a learning problem.
        /// Starts from initState and every succeeding state generates by performing a random correct action.
        /// </summary>
        /// <param name="worlds">World in which sequences should be generated.</param>
        /// <param name="plansCount">Number of generated sequences.</param>
        /// <param name="reportFunction">Function for reporting progress.</param>
        public void GenerateRandomSequences(List<World> worlds, int plansCount, Action<int, object> reportFunction)
        {
            int initStates = 0;
            int currentInitState = 0;
            int generatedPlans = 0;

            foreach (var world in worlds)
            {
                initStates += world.Plans.Length;
            }

            foreach (var world in worlds)
            {
                List<Plan> plans = new List<Plan>();

                foreach (var plan in world.Plans)
                {
                    int count = 0;
                    while (count < plansCount / (initStates - currentInitState))
                    {
                        State initState = plan.States[0];

                        Console.WriteLine("Generating plan " + (generatedPlans));

                        Plan p = GenerateSequence(initState, world);

                        if (p != null)
                        {
                            plans.Add(p);
                            count++;
                            generatedPlans++;
                        }

                        reportFunction(generatedPlans / plansCount, new ProgressState(generatedPlans, currentInitState + 1, -1, -1, -1));
                    }

                    plansCount -= plansCount / (initStates - currentInitState);
                    currentInitState++;
                }

                world.Plans = plans.ToArray();
            }
        }

        /// <summary>
        /// Does BFS from input states and for each possible set of 3 or less predicates saves the shortest plan that found that combination of predicates.
        /// After set amount of states, returns random plans
        /// </summary>
        /// <param name="worlds">Input worlds</param>
        /// <param name="plansCount">Number of plans to generate.</param>
        /// <param name="reportFunction">Function for reporting progress.</param>
        public void BFSRandomSequences(List<World> worlds, int plansCount, Action<int, object> reportFunction)
        {
            int initStates = 0;
            int currentInitState = 0;
            int generatedPlans = 0;

            foreach (var world in worlds)
            {
                initStates += world.Plans.Length;
            }

            foreach (var world in worlds)
            {
                List<Plan> plans = new List<Plan>();

                foreach (var plan in world.Plans)
                {
                    ProgressState ps = new ProgressState(generatedPlans, currentInitState + 1, 0, 0, 0);
                    reportFunction(-1, ps);

                    List<Plan> tmpPlans = BFSSequences(plan.States[0], world, world.Model, plansCount / initStates, reportFunction, ps);

                    int i = 0;
                    foreach (Plan p in tmpPlans)
                    {
                        plans.Add(p);
                        Console.WriteLine(i++);
                        string s = p.ToString();
                    }
                    
                    generatedPlans += tmpPlans.Count;
                    plansCount -= tmpPlans.Count;

                    initStates--;
                    currentInitState++;
                }

                world.Plans = plans.ToArray();
            }

            reportFunction(-1, new ProgressState(generatedPlans, currentInitState, 0, 0, 0));
        }

        /// <summary>
        /// Generates action/state sequences for learning problem.
        /// Starts from initState and every succeeding state is generated by performing a random correct action.
        /// </summary>
        /// <param name="initState">Initial state for sequence.</param>
        /// <param name="world">World in which the sequence should be generated.</param>
        /// <param name="length">Lenght of the sequence.</param>
        /// <returns>Generated sequence</returns>
        private Plan GenerateSequence(State initState, World world)
        {
            Model model = world.Model;

            List<State> states = new List<State>();
            List<GeneralPlanningLibrary.Action> actions = new List<GeneralPlanningLibrary.Action>();

            State s = initState.Clone();
            states.Add(s);
            GeneralPlanningLibrary.Action lastAction = null;

            for (int i = 0; i < Properties.Settings.Default.SequencesTargetActions; i++)
            {
                if (i % 20 == 0) Console.WriteLine("\t action nr." + i);
                GeneralPlanningLibrary.Action a = GetRandomAplicableAction(s, world, model, lastAction);

                if (a == null)
                {
                    Console.WriteLine("\t plan ended with " + i + " actions.");

                    if (i < Properties.Settings.Default.SequencesMinActions)
                    {
                        Console.WriteLine("\t not enough actions");
                        return null;
                    }

                    break;
                }
                s = a.ApplicateToState(s);

                states.Add(s);
                actions.Add(a);

                lastAction = a;
            }

            Plan p = new Plan(actions.ToArray(), states.ToArray());

            FormatPlan(p, null);

            return p;
        }

        /// <summary>
        /// Generates a random action that can be aplicated to given state.
        /// </summary>
        /// <param name="state">Initial state.</param>
        /// <param name="world">World where the actions is generated.</param>
        /// <param name="model">Model of the world.</param>
        /// <returns>Generated action</returns>
        private GeneralPlanningLibrary.Action GetRandomAplicableAction(State state, World world, Model model, GeneralPlanningLibrary.Action lastAction = null)
        {
            List<GeneralPlanningLibrary.Action> actions;

            if (RG.NextDouble() < 0.6 && lastAction != null)
            {
                actions = GeneratePossibleActions(state, world, model, lastAction.Operator);
                if (actions.Count == 0)
                {
                    actions = GeneratePossibleActions(state, world, model);
                }
            }
            else
            {
                actions = GeneratePossibleActions(state, world, model);
            }

            if (actions.Count == 0)
            {
                return null;
            }

            int index = RG.Next(actions.Count);
            var a = actions[index];

            return a;
        }

        /// <summary>
        /// Geerates every possible action that can be performed from given state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="world"></param>
        /// <param name="model"></param>
        /// <param name="skipOperator"></param>
        /// <returns></returns>
        private List<GeneralPlanningLibrary.Action> GeneratePossibleActions(State state, World world, Model model, Operator skipOperator = null)
        {
            List<GeneralPlanningLibrary.Action> possibleActions = new List<GeneralPlanningLibrary.Action>();

            foreach (var pair1 in model.Operators)
            {
                Operator oper = pair1.Value;

                if (oper == skipOperator) continue;

                GeneralPlanningLibrary.Object[] parameters = new GeneralPlanningLibrary.Object[oper.ParameterCount];

                GeneratePossibleActionsRecursively(state, world, model, oper, 0, parameters, possibleActions);
            }

            return possibleActions;
        }

        /// <summary>
        /// Generates every possible parametrization of given operator that can be performed from given state 
        /// by recursively trying every possible combination.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="world"></param>
        /// <param name="model"></param>
        /// <param name="oper"></param>
        /// <param name="paramId">ID of next parameter that should be decided</param>
        /// <param name="parameters">current partial parametrization</param>
        /// <param name="possibleActions">list with all possible actions found so far</param>
        private void GeneratePossibleActionsRecursively(State state, World world, Model model, Operator oper, int paramId, GeneralPlanningLibrary.Object[] parameters, List<GeneralPlanningLibrary.Action> possibleActions)
        {
            if (paramId == oper.ParameterCount)
            {
                GeneralPlanningLibrary.Action a = new GeneralPlanningLibrary.Action(oper, parameters);
                if (!a.IsApplicable(state)) return;

                GeneralPlanningLibrary.Object[] finishedParameters = new GeneralPlanningLibrary.Object[parameters.Length];
                for (int i = 0; i < finishedParameters.Length; i++)
                {
                    finishedParameters[i] = parameters[i];
                }
                a.Parameters = finishedParameters;

                possibleActions.Add(a);
                return;
            }

            foreach (var obj in model.Constants.Concat(world.Objects))
            {
                if (oper.ParameterCanHaveType(obj.Value.Type, paramId))
                {
                    //check if object wasnt already used
                    if (!Properties.Settings.Default.ParametersCanRepeatInActions)
                    {
                        bool found = false;

                        for (int i = 0; i < paramId; i++)
                        {
                            if (parameters[i] == obj.Value)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }
                    }

                    parameters[paramId] = obj.Value;

                    //checking if current parametrization is applicable to speed up generation
                    if (!IsPartialParametrizationApplicable(state, oper, paramId + 1, parameters))
                    {
                        continue;
                    }

                    GeneratePossibleActionsRecursively(state, world, model, oper, paramId + 1, parameters, possibleActions);
                }
            }
        }

        /// <summary>
        /// Returns false if there is predicates in action's preconditions list that uses already generated parameters and is not present in given state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="oper"></param>
        /// <param name="paramId">number of parameters generated so far</param>
        /// <param name="parameters">current parameters</param>
        /// <returns></returns>
        private bool IsPartialParametrizationApplicable(State state, Operator oper, int paramId, GeneralPlanningLibrary.Object[] parameters)
        {
            GeneralPlanningLibrary.Action a = new GeneralPlanningLibrary.Action(oper, parameters);

            foreach (PredicateReference predicate in oper.PositivePreconditions)
            {
                bool allParametersDecided = true;

                for (int i = 0; i < predicate.ParametersIds.Length; i++)
                {
                    if (predicate.ParametersIds[i] >= paramId)
                    {
                        allParametersDecided = false;
                        break;
                    }
                }

                //predicate isn't fully instanciated yet.
                if (!allParametersDecided) continue;

                PredicateInstance pi = a.InstantiatePredicate(predicate);

                if (!state.Contains(pi))
                {
                    return false;
                }
            }

            foreach (PredicateReference predicate in oper.NegativePreconditions)
            {
                bool allParametersDecided = true;

                for (int i = 0; i < predicate.ParametersIds.Length; i++)
                {
                    if (predicate.ParametersIds[i] >= paramId)
                    {
                        allParametersDecided = false;
                        break;
                    }
                }

                //predicate isn't fully instanciated yet.
                if (!allParametersDecided) continue;

                PredicateInstance pi = a.InstantiatePredicate(predicate);

                if (state.Contains(pi))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Generates random action with random parameters.
        /// </summary>
        /// <param name="world">World where the actions is generated.</param>
        /// <param name="model">Model of the world.</param>
        /// <returns>Random Action.</returns>
        private GeneralPlanningLibrary.Action GetRandomAction(World world, Model model)
        {
            GeneralPlanningLibrary.Action a = new GeneralPlanningLibrary.Action();

            Operator o = model.Operators.ElementAt(RG.Next(model.Operators.Count)).Value;
            a.Operator = o;
            a.Parameters = new GeneralPlanningLibrary.Object[o.ParameterCount];

            for (int i = 0; i < o.ParameterCount; i++)
            {
                GeneralPlanningLibrary.Type t = o.ParameterTypes[i][RG.Next(o.ParameterTypes[i].Length)];

                int k;
                bool found = false;
                GeneralPlanningLibrary.Object obj = null;
                while (!found) {
                    k = RG.Next(world.Objects.Count + model.Constants.Count);
                    if (k < world.Objects.Count) obj = world.Objects.ElementAt(k).Value;
                    else obj = model.Constants.ElementAt(k - world.Objects.Count).Value;
                    
                    if (t.IsFatherOf(obj.Type)) found = true;
                }

                a.Parameters[i] = obj;
            }

            return a;
        }

        /// <summary>
        /// Generates random plans by first generating a goal condition and then performing BFS for each plan.
        /// </summary>
        /// <param name="worlds"></param>
        /// <param name="model"></param>
        /// <param name="planCount">number of plans to generate</param>
        /// <param name="reportFunction">function used to report progress</param>
        public void GenerateRandomPlans(List<World> worlds, Model model, int planCount, Action<int, object> reportFunction)
        {
            int initStatesCount = 0;
            for (int i = 0; i < worlds.Count; i++)
            {
                initStatesCount += worlds[i].Plans.Length;
            }

            int generatedPlans = 0;
            int currentInitState = 0;

            foreach (var world in worlds)
            {
                List<Plan> plans = new List<Plan>();

                foreach (var plan in world.Plans)
                {
                    int plansToDo = planCount / initStatesCount;
                    State initState = plan.States[0];

                    while (plansToDo > 0)
                    {
                        //reporting progress
                        ProgressState ps = new ProgressState(generatedPlans, currentInitState + 1, 0, 0, 0);
                        reportFunction(generatedPlans / planCount, ps);

                        List<PredicateInstance> goals = new List<PredicateInstance>();

                        //generating random goals
                        int triesCount = 0;
                        while (goals.Count < Properties.Settings.Default.BFSGoalPredicatesCount)
                        {
                            PredicateInstance goal = world.GenerateRandomPredicateInstance();
                            if (triesCount++ > 5000 || goal == null) break; // goal states couldn't be generated

                            if (goals.Contains(goal)) continue;
                            if (initState.Contains(goal)) continue;

                            goals.Add(goal);
                        }
                        if (triesCount > 5000) continue;

                        //searching for plan
                        Plan p = BFSPlan(initState, goals, world, model, reportFunction, ps);

                        //adding found plan
                        if (p != null && p.Actions.Length >= Properties.Settings.Default.BFSMinimalPlanLength)
                        {
                            Console.WriteLine(p);

                            FormatPlan(p, goals.ToArray());
                            plans.Add(p);

                            plansToDo--;
                            generatedPlans++;
                        }
                    }

                    initStatesCount--;
                    currentInitState++;
                }

                world.Plans = plans.ToArray();
            }
        }

        /// <summary>
        /// Adjust number of predicate occurences in given plan according to Properties.Settings.Default
        /// </summary>
        /// <param name="p"></param>
        /// <param name="goals"></param>
        private void FormatPlan(Plan p, PredicateInstance[] goals)
        {
            if (p.States.Length == 0) return;

            //Init state
            switch (Properties.Settings.Default.InitStateOutputId)
            {
                case 0: // whole state
                    break;

                case 1: // partial state
                    FormatState(p.States[0]);

                    if (p.States[0].Predicates.Length == 0) p.States[0] = null;

                    break;

                case 2: // don't output init state
                    p.States[0] = null;
                    break;

                default:
                    break;
            }


            if (p.States.Length == 1) return;

            //Goal state
            switch (Properties.Settings.Default.GoalStateOutputId)
            {
                case 0: // whole state
                    break;

                case 1: // some predicates
                    FormatState(p.States[p.States.Length - 1]);

                    if (p.States[p.States.Length - 1].Predicates.Length == 0) p.States[p.States.Length - 1] = null;

                    break;

                case 2: // goal predicates only
                    if (goals == null || goals.Length == 0)
                    {
                        p.States[p.States.Length - 1] = null;
                    }
                    else
                    {
                        p.States[p.States.Length - 1].Predicates = goals;
                    }
                    break;

                case 3: // don't output goal state
                    p.States[p.States.Length - 1] = null;
                    break;

                default:
                    break;
            }


            //Transition states
            for (int i = 1; i < p.States.Length - 1; i++)
            {
                switch (Properties.Settings.Default.TransitionStatesOuputIds)
                {
                    case 0: // whole state
                        break;

                    case 1: // partial state
                        FormatState(p.States[i]);

                        if (p.States[i].Predicates.Length == 0) p.States[i] = null;

                        break;

                    case 2: // don't output init state
                        p.States[i] = null;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Removes some predicates from givem state according to Properties.Setting.Default.PredicateChance variable value.
        /// </summary>
        /// <param name="s"></param>
        private void FormatState(State s)
        {
            List<PredicateInstance> newPredicates = new List<PredicateInstance>();
            foreach (var predicate in s.Predicates)
            {
                if (Properties.Settings.Default.PredicateChance != 0 && RG.NextDouble() <= Properties.Settings.Default.PredicateChance)
                {
                    newPredicates.Add(predicate);
                }
            }
            s.Predicates = newPredicates.ToArray();
        }

        /// <summary>
        /// tries to find a sequence of actions from given init state that generate goal predicates
        /// </summary>
        /// <param name="initState">state to start from</param>
        /// <param name="goals">goal condition</param>
        /// <param name="world"></param>
        /// <param name="model"></param>
        /// <param name="reportFunction">function used to report progress</param>
        /// <param name="progressState">progress state to insert progress to</param>
        /// <returns></returns>
        private Plan BFSPlan(State initState, List<PredicateInstance> goals, World world, Model model, Action<int, object> reportFunction, ProgressState progressState)
        {
            List<PathState> visitedStates = new List<PathState>();
            List<PathState> foundStates = new List<PathState>();
            HashSet<string> isStateFound = new HashSet<string>();

            foundStates.Add(new PathState(initState, -1, null, null));
            isStateFound.Add(initState.ToString());

            while (foundStates.Count > 0 && visitedStates.Count < Properties.Settings.Default.BFSMaxVisitedStates)
            {
                //Treshold for reporting progress
                if (visitedStates.Count % ReportAfterNumberOfStates == 0)
                {
                    Console.WriteLine(visitedStates.Count + "  " + foundStates.Count);

                    progressState.BFSStatesVisited = visitedStates.Count;
                    progressState.BFSOpenStates = foundStates.Count;
                    progressState.BFSCurrentDepth = foundStates[0].Depth;

                    reportFunction(-1, progressState);
                }

                PathState ps = foundStates[0];
                foundStates.RemoveAt(0);

                //goal state is found
                if (ps.State.Contains(goals))
                {
                    State[] states = new State[ps.Depth + 1];
                    GeneralPlanningLibrary.Action[] actions = new GeneralPlanningLibrary.Action[ps.Depth];

                    PathState pss = ps;
                    while (pss.Depth > 0)
                    {
                        states[pss.Depth] = pss.State.Clone();
                        actions[pss.Depth - 1] = pss.PrecedingAction;

                        pss = visitedStates[pss.ParentState];
                    }

                    states[0] = pss.State.Clone();

                    return new Plan(actions, states);
                }

                //exploring neighbor states of current state
                List<GeneralPlanningLibrary.Action> possibleActions = GetAplicableActions(ps.State, world, model);

                foreach (var a in possibleActions)
                {
                    State s = a.ApplicateToState(ps.State);

                    if (isStateFound.Contains(s.ToString()))
                    {
                        continue;
                    }

                    isStateFound.Add(s.ToString());
                    foundStates.Add(new PathState(s, visitedStates.Count, ps, a, ps.Depth + 1));
                }

                visitedStates.Add(ps);
            }

            return null;
        }

        /// <summary>
        /// Generates random plans from given init state by performing BFS and storing possible goal states, then picking random plans from
        /// set of generated plans.
        /// </summary>
        /// <param name="initState"></param>
        /// <param name="world"></param>
        /// <param name="model"></param>
        /// <param name="planCount"></param>
        /// <param name="reportFunction">function used to report progress</param>
        /// <param name="progressState">progress state to insert progress to</param>
        /// <returns></returns>
        private List<Plan> BFSSequences(State initState, World world, Model model, int planCount, Action<int, object> reportFunction, ProgressState progressState)
        {
            List<PathState> visitedStates = new List<PathState>();
            List<PathState> foundStates = new List<PathState>();
            HashSet<string> isStateFound = new HashSet<string>();
            Dictionary<string, PathState> goalStates = new Dictionary<string, PathState>();

            foundStates.Add(new PathState(initState, -1, null, null));
            isStateFound.Add(initState.ToString());

            //performing BFS
            while (foundStates.Count > 0 && visitedStates.Count < Properties.Settings.Default.BFSVisitedStates)
            {
                if (visitedStates.Count % ReportAfterNumberOfStates == 0)
                {
                    Console.WriteLine(visitedStates.Count + "  " + foundStates.Count);
                    progressState.BFSCurrentDepth = foundStates[0].Depth;
                    progressState.BFSOpenStates = foundStates.Count;
                    progressState.BFSStatesVisited = visitedStates.Count;
                    reportFunction(-1, progressState);
                }

                PathState ps = foundStates[0];
                foundStates.RemoveAt(0);

                List<GeneralPlanningLibrary.Action> possibleActions = GetAplicableActions(ps.State, world, model);

                foreach (var a in possibleActions)
                {
                    if (ps.PrecedingAction == a) continue;

                    State s = a.ApplicateToState(ps.State);

                    if (isStateFound.Contains(s.ToString()))
                    {
                        continue;
                    }

                    isStateFound.Add(s.ToString());

                    PathState newPS = new PathState(s, visitedStates.Count, ps, a, ps.Depth + 1);
                    foundStates.Add(newPS);

                    newPS.FindNewPredicates(initState);
                    newPS.AddNewGoalStates(goalStates);
                }

                visitedStates.Add(ps);
            }

            //preparing array with info about which plans were already used
            var possibleEndings = goalStates.ToArray();
            List<int> unusedEndings = new List<int>();
            for (int i = 0; i < possibleEndings.Length; i++)
            {
                unusedEndings.Add(i);
            }
            foundPlans.Add(possibleEndings); 

            //picking random plans from set of generated plans
            List<Plan> plans = new List<Plan>();
            while (plans.Count < planCount && unusedEndings.Count > 0)
            {
                int index = RG.Next(unusedEndings.Count);
                int i = unusedEndings[index];
                unusedEndings.RemoveAt(index);

                PathState ps = possibleEndings[i].Value;

                if (ps.Depth < Properties.Settings.Default.BFSMinimalPlanLength) continue;

                State[] states = new State[ps.Depth + 1];
                GeneralPlanningLibrary.Action[] actions = new GeneralPlanningLibrary.Action[ps.Depth];

                PathState pss = ps;
                while (pss.Depth > 0)
                {
                    states[pss.Depth] = pss.State.Clone();
                    actions[pss.Depth - 1] = pss.PrecedingAction;

                    pss = visitedStates[pss.ParentState];
                }

                states[0] = pss.State.Clone();

                Plan p = new Plan(actions, states);
                FormatPlan(p, ps.GoalPredicates);

                string s = p.ToString();

                plans.Add(p);
            }

            return plans;
        }

        /// <summary>
        /// Generates a set of actions that can be aplicated to given state.
        /// </summary>
        /// <param name="state">Initial state.</param>
        /// <param name="world">World where the actions is generated.</param>
        /// <param name="model">Model of the world.</param>
        /// <returns>Generated actions</returns>
        private List<GeneralPlanningLibrary.Action> GetAplicableActions(State state, World world, Model model)
        { 
            List<GeneralPlanningLibrary.Action> possibleActions = new List<GeneralPlanningLibrary.Action>();

            foreach (var pair in model.Operators)
            {
                possibleActions.AddRange(pair.Value.GeneratePossibleActions(world.Objects, model.Constants, state));
            }

            return possibleActions;
        }

        /// <summary>
        /// Outputs given model and given world to output file.
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="m"></param>
        /// <param name="worlds"></param>
        public void WriteToFile(StreamWriter sw, Model m, List<World> worlds)
        {
            if (Properties.Settings.Default.RemoveOperatorsLists) m.RemoveOperatorInfo();

            sw.WriteLine(m.ToString());

            foreach (var world in worlds)
            {
                sw.WriteLine(world.ToString());
            }

            sw.Close();
        }


        private static List<KeyValuePair<string, PathState>[]> foundPlans = new List<KeyValuePair<string, PathState>[]>();
        public static int[] levelsofPartialInformationInTestData = new int[] { 0, 5, 10, 15, 20, 30, 40, 60, 80, 100 };

        /// <summary>
        /// This method is used to generate multiple test datasets with variating level of partial information 
        /// according to percentages stored in levelsofPartialInformationInTestData array.
        /// </summary>
        /// <param name="worlds"></param>
        /// <param name="plansCount"></param>
        /// <param name="reportFunciton"></param>
        public void GenerateTestData(List<World> worlds, int plansCount, Action<int, object> reportFunciton)
        {
            BFSRandomSequences(worlds, plansCount, reportFunciton);
            List<List<int>> unused = new List<List<int>>();

            //setup 
            for (int stateId = 0; stateId < foundPlans.Count; stateId++)
            {
                unused.Add(new List<int>());
                for (int i = 0; i < foundPlans[stateId].Length; i++)
                {
                    if (foundPlans[stateId][i].Value.Depth >= Properties.Settings.Default.BFSMinimalPlanLength)
                    {
                        unused[stateId].Add(i);
                    }
                }
            }


            for (int infoRatio = 0; infoRatio < levelsofPartialInformationInTestData.Length; infoRatio++)
            {
                Properties.Settings.Default.PredicateChance = levelsofPartialInformationInTestData[infoRatio] / 100d;

                int remainingPlans = plansCount;

                for (int stateId = 0; stateId < foundPlans.Count; stateId++)
                {
                    int plansToDo = remainingPlans / (foundPlans.Count - stateId);
                    remainingPlans -= plansToDo;

                    List<Plan> plans = new List<Plan>();

                    while (plansToDo-- > 0 && unused[stateId].Count > 0)
                    {
                        int index = RG.Next(unused[stateId].Count);

                        int planIndex = unused[stateId][index];
                        unused[stateId].RemoveAt(index);

                        PathState ps = foundPlans[stateId][planIndex].Value;

                        State[] states = new State[ps.Depth + 1];
                        GeneralPlanningLibrary.Action[] actions = new GeneralPlanningLibrary.Action[ps.Depth];

                        PathState pss = ps;
                        while (pss.Depth > 0)
                        {
                            states[pss.Depth] = pss.State.Clone();
                            actions[pss.Depth - 1] = pss.PrecedingAction;

                            pss = pss.ParentStateReference;
                        }

                        states[0] = pss.State.Clone();

                        Plan p = new Plan(actions, states);
                        FormatPlan(p, ps.GoalPredicates);

                        string s = p.ToString();

                        plans.Add(p);
                    }

                    worlds[stateId].Plans = plans.ToArray();
                }

                StreamWriter sw = new StreamWriter("Test - ARMS3 - " + levelsofPartialInformationInTestData[infoRatio] + " - Blocksworld.txt");

                WriteToFile(sw, worlds[0].Model, worlds);
            }
        }

    }
}