using ModelLearner.Algorithms;
using ModelLearner.Algorithms.ARMS;
using ModelLearner.Algorithms.LOUGA;
using ModelLearner.Algorithms.LOCM;
using ModelLearner.Algorithms.LOCM2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using FastColoredTextBoxNS;
using GeneralPlanningLibrary;
using GeneralPlanningLibrary.Utility;

namespace ModelLearner
{
    public partial class Form1 : Form
    {
        public static OutputManager Manager;

        public static object RunCommunicationLock;
        public static bool AlgRunning = false;
        public static bool Halt = false;

        /// <summary>
        /// Hidden label used that is focused before editing TextBoxes contents to prevent flickering.
        /// </summary>
        public static Label LabelForFocusing;

        private static Color KeyWordColor = Color.Blue;
        private static Color LesserKeyWordColor = Color.DarkBlue;
        private static Color ParameterColor = Color.FromArgb(100,100,100);
        private static Color CommentColor = Color.Green;
        private static Color DefaultColor = Color.Black;

        private static Style keywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private static Style lesserKeywordStyle = new TextStyle(Brushes.DarkBlue, null, FontStyle.Regular);
        private static Style parameterStyle = new TextStyle(new SolidBrush(Color.FromArgb(100, 100, 100)), null, FontStyle.Regular);
        private static Style commentStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        private static Style errorStyle = new TextStyle(Brushes.Red, null, FontStyle.Underline);


        //names used in algorithm selection ComboBox
        private const string AlgNameARMS = "ARMS";
        private const string AlgNameLOCM = "LOCM";
        private const string AlgNameLOCM2 = "LOCM2";
        private const string AlgNameLOUGA = "LOUGA";

        /// <summary>
        /// Separate thread for learning algorithm.
        /// </summary>
        BackgroundWorker LearningAlgorithmThread;

        /// <summary>
        /// Current learning algorithm.
        /// </summary>
        private static ILearningAlgorithm alg;

        /// <summary>
        /// Errors in input file.
        /// </summary>
        private static List<InputException> Errors;

        /// <summary>
        /// Decoded model.
        /// </summary>
        private static Model inputModel;

        /// <summary>
        /// Output of learning algorithm.
        /// </summary>
        private static Model finishedModel;


        /// <summary>
        /// Decoded worlds.
        /// </summary>
        private static List<World> worlds;

        /// <summary>
        /// Tooltip describing error.
        /// </summary>
        private static ToolTip ErrorToolTip;
        
        /// <summary>
        /// Index of error which is currently shown.
        /// </summary>
        private static int CurrentErrorIndex;

        /// <summary>
        /// List of IDs of current predicates. Needed in LOUGA's predicate by predicate mode.
        /// </summary>
        private Dictionary<string, int> PredicateIds;

        /// <summary>
        /// Index of currently selected index in ComboBoxLOUGAPredicates. 
        /// Used to prevent blicking of TextBoxex when updating ComboBoxLOUGAPredicates contents.
        /// </summary>
        private int ComboBoxPredicatesLastSelectedIndex;


        /// <summary>
        /// Stopwatch for measuring length of algoritm's run.
        /// </summary>
        private Stopwatch AlgorithmStopwatch;

        /// <summary>
        /// Input text box for user's input.
        /// </summary>
        private FastColoredTextBox InputFastTextBox;
        
        /// <summary>
        /// Cooldown for next error test. Resets after input from user.
        /// Starts at value set by ErrorTestDelay constant, is decremented every updateTimer tick and 
        /// when it reaches zero, test for errors is started.
        /// </summary>
        private int ErrorTestCountdown;

        /// <summary>
        /// Delay in updateTimer ticks after which error test is started.
        /// (countdown resets after each user change in InputFastTExtBox)
        /// </summary>
        private const int ErrorTestDelay = 40;


        /// <summary>
        /// Name of currently opened file.
        /// </summary>
        private static string FileName;
        public Form1()
        {
            InitializeComponent();

            Manager = new OutputManager(tabControl1);
            Manager.SetBoxesToCopy(listBoxIndividualsList, textBoxIndividualsExplorer);
            Manager.SetOutputTabs(new string[0]);
            Manager.Highlight = HighlightPDDLSyntax;
            Manager.ComboBoxPredicate = comboBoxLOUGAPredicates;
            RunCommunicationLock = new object();

            LabelForFocusing = hiddenLabelForFocusing;

            Errors = new List<InputException>();
            ErrorToolTip = new ToolTip();
            CurrentErrorIndex = -1;

            InputTextBox.Visible = false;
            InputFastTextBox = new FastColoredTextBox();
            InputFastTextBox.Location = InputTextBox.Location;
            InputFastTextBox.Size = InputTextBox.Size;
            InputFastTextBox.Font = InputTextBox.Font;
            InputFastTextBox.Anchor = InputTextBox.Anchor;
            InputFastTextBox.BorderStyle = InputTextBox.BorderStyle;
            InputFastTextBox.AutoIndentExistingLines = false;
            InputFastTextBox.ToolTipDelay = 50;
            InputFastTextBox.VisibleRangeChanged += new EventHandler(fastTextBox_VisibleRangeChanged);
            InputFastTextBox.TextChanged += new EventHandler<TextChangedEventArgs>(InputFastTextBox_TextChanged);
            InputFastTextBox.ToolTipNeeded += new EventHandler<ToolTipNeededEventArgs>(InputFastTextBox_ToolTipNeeded);

            InputFastTextBox.AddStyle(errorStyle);
            InputFastTextBox.AddStyle(commentStyle);
            InputFastTextBox.AddStyle(keywordStyle);
            InputFastTextBox.AddStyle(lesserKeywordStyle);
            InputFastTextBox.AddStyle(parameterStyle);
            this.Controls.Add(InputFastTextBox);

            LoadSettings();

            AlgorithmStopwatch = new Stopwatch();
        }

        /// <summary>
        /// Loads last settings from previous application run.
        /// </summary>
        private void LoadSettings()
        {
            comboBoxAlgorithmSelect.SelectedIndex = Properties.Settings.Default.SelectedAlgorithm;
            checkBoxLOUGAPDDLOutput.Checked = Properties.Settings.Default.LOUGA_PDDLOutput;
            checkBoxLOUGAGoalStateComplete.Checked = Properties.Settings.Default.LOUGA_GoalStateComplete;
            checkBoxLOUGAEndAfterFirstSolution.Checked = Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound;
            checkBoxLOUGAPredicateByPredicate.Checked = Properties.Settings.Default.LOUGA_PredicateByPredicate;
            checkBoxLOUGAGenerateNegativePreconditions.Checked = Properties.Settings.Default.LOUGA_GenerateNegativePreconditions;
            checkBoxLOUGAGenerateCompletePreconditionsLists.Checked = Properties.Settings.Default.LOUGA_GenerateCompletePreconditionsLists;
            checkBoxLOUGAParametersCanRepeat.Checked = Properties.Settings.Default.LOUGA_ParametersCanRepeat;

            textBoxLOUGAPopSize.Text = Properties.Settings.Default.LOUGA_PopulationSize.ToString();
            textBoxLOUGAPopRestartTreshold.Text = Properties.Settings.Default.LOUGA_PopulationRestartTreshold.ToString();
            textBoxLOUGAOldPopRestartTreshold.Text = Properties.Settings.Default.LOUGA_OldPopulationRestartTreshold.ToString();
            textBoxLOUGAGreedySearchTreshold.Text = Properties.Settings.Default.LOUGA_GreedySearchTreshold.ToString();
            textBoxLOUGAGenCount.Text = Properties.Settings.Default.LOUGA_GenerationsCount.ToString();
            textBoxLOUGACrossWithOldPopTreshold.Text = Properties.Settings.Default.LOUGA_CrossoverWithOldPopulationTreshold.ToString();
            textBoxLOUGAMaximalErrorRate.Text = Properties.Settings.Default.LOUGA_MaxPredicateErrorRate.ToString();
            textBoxLOUGAObservationErrorWeight.Text = Properties.Settings.Default.LOUGA_ObservationErrorWeight.ToString();

            textBoxARMSMaxAConstrWeight.Text = Properties.Settings.Default.ARMS_MaxActionConstraintWeight.ToString();
            textBoxARMSMinAConstrWeight.Text = Properties.Settings.Default.ARMS_MinActionConstraintWeight.ToString();
            textBoxARMSInfConstrWeight.Text = Properties.Settings.Default.ARMS_InformationConstraintWeight.ToString();
            textBoxARMSProbTreshold.Text = Properties.Settings.Default.ARMS_ProbabilityTreshold.ToString();

            comboBoxARMSMaxSatAlgorithm.Text = Properties.Settings.Default.ARMS_MaxSatAlgorithm;
            textBoxARMSMaxSatNrOfTries.Text = Properties.Settings.Default.ARMS_MaxSatNumberOfTries.ToString();
            textBoxARMSMaxSatRestartTreshold.Text = Properties.Settings.Default.ARMS_MaxSatRestartTreshold.ToString();
            textBoxARMSMaxSatRandomChoiceProbability.Text = Properties.Settings.Default.ARMS_MaxSatRandomChoiceProbability.ToString();

            InputTextBox.AcceptsTab = true;
            comboBoxLOUGAPredicates.Visible = false;

            LoadFile(Properties.Settings.Default.File);

            string justToDiasableUndo = InputTextBox.Text;
        }

        /// <summary>
        /// Clears model's operators' preconditions and effect trees.
        /// </summary>
        /// <param name="m"></param>
        private void ResetOperators(Model m)
        {
            foreach (var oper in m.Operators)
            {
                oper.Value.Preconditions = null;
                oper.Value.Effects = null;
                oper.Value.AddList = null;
                oper.Value.DelList = null;
                oper.Value.PositivePreconditions = null;
                oper.Value.NegativePreconditions = null;
            }

        }

        /// <summary>
        /// Tests input data for errors and checks if selected algorithm is compatible with them.
        /// </summary>
        /// <returns></returns>
        private bool TestInputData()
        {
            FindErrors();

            if (Errors.Count > 0)
            {
                ShowNextError();

                if (Errors.Count > 1) MessageBox.Show("There are errors in input data.", "Input error");
                else MessageBox.Show("There is an error in input data.", "Input error");

                return false;
            }

            if (inputModel == null)
            {
                MessageBox.Show("No input model.", "Input error");

                return false;
            }

            //check if there is at least one valid plan 
            //if selected algorithm is ARMS or LOUGA, checks if every plan has an initial state
            //if selected algorithm is LOCM or LOCM2, checks if no action uses an object twice in its parameter list.
            int plansCount = 0;
            foreach (var world in worlds)
            {
                foreach (var plan in world.Plans)
                {
                    if (plan != null && plan.Actions != null && plan.Actions.Length > 0) plansCount++;

                    if (plan.States[0] == null && (comboBoxAlgorithmSelect.Text == AlgNameARMS || comboBoxAlgorithmSelect.Text == AlgNameLOUGA))
                    {
                        MessageBox.Show("There is at least one plan without an initial state in input data.", "Input error");

                        return false;
                    }

                    if (comboBoxAlgorithmSelect.Text == AlgNameLOCM || comboBoxAlgorithmSelect.Text == AlgNameLOCM2)
                    {
                        foreach (var action in plan.Actions)
                        {
                            for (int i = 0; i < action.Parameters.Length; i++)
                            {
                                for (int j = i + 1; j < action.Parameters.Length; j++)
                                {
                                    if (action.Parameters[i] == action.Parameters[j])
                                    {
                                        MessageBox.Show("There is an action that uses an object twice in its parameter list. \nLOCM and LOCM2 don't know how to work with such actions.", "Input error");

                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (plansCount == 0)
            {
                MessageBox.Show("No plans in input data.", "Input error");

                return false;
            }

            //LOCM and LOCM2 can't work with operators that have "either" in parameter list
            if (comboBoxAlgorithmSelect.Text == AlgNameLOCM || comboBoxAlgorithmSelect.Text == AlgNameLOCM2)
            {
                foreach (var op in inputModel.Operators)
                {
                    for (int i = 0; i < op.Value.ParameterCount; i++)
                    {
                        if (op.Value.ParameterTypes[i].Length > 1)
                        {
                            MessageBox.Show("Operator '" + op.Key + "' has 'either' keyword in parameter list. \nLOCM and LOCM2 don't know how to work with such operators.", "Input error");

                            return false;
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Starts algorithm if there are no errors present in input text.
        /// </summary>
        /// <returns>TRUE if algorithm was started successfully</returns>
        private bool StartAlgorithm()
        {
            if (!TestInputData()) return false;

            Manager.Solution = null;
            comboBoxLOUGAPredicates.Visible = false;

            //Preparing UI and setting for selected algorithm
            switch (comboBoxAlgorithmSelect.Text)
            {
                case "ARMS":
                    Manager.SetOutputTabs(
                        new string[] { OutputManager.OutputTabName, OutputManager.SolutionTabName }, 
                        null
                    );

                    if (comboBoxARMSMaxSatAlgorithm.Text == "GSAT") alg = new ARMS(new GSAT());
                    else alg = new ARMS(new WalkSat());
                    break;
                case "LOCM":
                    Manager.SetOutputTabs(
                        new string[] { OutputManager.OutputTabName, OutputManager.SolutionTabName }, 
                        null
                    );
                    
                    alg = new LOCMMachine();
                    break;
                case "LOCM2":
                    Manager.SetOutputTabs(
                        new string[] { OutputManager.OutputTabName, OutputManager.SolutionTabName }, 
                        null
                    );

                    alg = new LOCM2Machine();
                    break;
                case "LOUGA":
                    panelBestIndividuals.Width = this.Size.Width - 1280 + 604;
                    panelBestIndividuals.Height = this.Size.Height - 720 + 625;

                    if (checkBoxLOUGAPredicateByPredicate.Checked)
                    {
                        string[] tabNames = new string[2 + 2 * inputModel.Predicates.Count];
                        TabType[] tabTypes = new TabType[tabNames.Length];
                        bool[] visibleTabs = new bool[tabNames.Length];

                        tabNames[0] = OutputManager.OutputTabName;
                        tabNames[1] = OutputManager.SolutionTabName;

                        visibleTabs[0] = true;
                        visibleTabs[1] = true;
                        visibleTabs[2] = true;
                        visibleTabs[3] = true;

                        comboBoxLOUGAPredicates.Items.Clear();
                        PredicateIds = new Dictionary<string,int>();

                        //sorting predicates
                        SortedDictionary<string, Predicate> sortedPredicates = new SortedDictionary<string, Predicate>();
                        foreach (var pair in inputModel.Predicates)
                        {
                            sortedPredicates.Add(pair.Key, pair.Value);
                        }

                        //creating new tab for each predicate
                        int id = 2;
                        foreach (var predicate in sortedPredicates)
                        {
                            tabNames[id] = "Ouput - " + predicate.Key;
                            tabNames[id + 1] = "Individuals - " + predicate.Key;
                            
                            tabTypes[id + 1] = TabType.ListBoxBestIndividuals;

                            comboBoxLOUGAPredicates.Items.Add(predicate.Key);

                            PredicateIds.Add(predicate.Key, id);

                            id += 2;
                        }

                        Manager.SetOutputTabs(tabNames, tabTypes, visibleTabs);
                        comboBoxLOUGAPredicates.Visible = true;
                        comboBoxLOUGAPredicates.SelectedIndex = 0;
                        comboBoxLOUGAPredicates.Location = new Point(this.Size.Width - 180, 7);
                        ComboBoxPredicatesLastSelectedIndex = -1;

                        alg = new LOUGAPredicateByPredicate();
                    }
                    else { 
                        Manager.SetOutputTabs(
                            new string[] { OutputManager.OutputTabName, LOUGA.CurrentBestTabName },
                            new TabType[] { TabType.OutputTab, TabType.ListBoxBestIndividuals }
                        );
                        alg = new LOUGA();
                    }
                    break;

                default:
                    alg = new LOUGA();
                    break;
            }

            tabControl1.SelectedIndex = 1;

            ResetOperators(inputModel);

            LearningAlgorithmThread = new BackgroundWorker();
            LearningAlgorithmThread.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args)
                {
                    AlgRunning = true;

                    finishedModel = alg.Learn(inputModel, worlds);

                    AlgRunning = false;
                    Halt = true;
                }
            );

            finishedModel = null;
            AlgorithmStopwatch.Reset();
            AlgorithmStopwatch.Start();
            LearningAlgorithmThread.RunWorkerAsync();
            return true;
        }

        /// <summary>
        /// Disables or enables algorithms settings. 
        /// </summary>
        /// <param name="value"></param>
        private void SetEnableUI(bool value)
        {
            comboBoxAlgorithmSelect.Enabled = value;
            checkBoxLOUGAGoalStateComplete.Enabled = value;
            buttonLoad.Enabled = value;
            buttonNew.Enabled = value;
            InputTextBox.ReadOnly = !value;
            checkBoxLOUGAPredicateByPredicate.Enabled = value;
            checkBoxLOUGAParametersCanRepeat.Enabled = value;

            if (value)
            {
                buttonLearn.Text = "Learn";
                buttonStartStop.Text = "Start";
            }
            else
            {
                buttonLearn.Text = "Stop";
                buttonStartStop.Text = "Stop";
            }
        }

        /// <summary>
        /// Starts learning algorithm if possible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLearn_Click(object sender, EventArgs e)
        {
            lock (RunCommunicationLock)
            {
                if (Halt) return;
                if (!AlgRunning)
                {
                    if (StartAlgorithm())
                    {
                        SetEnableUI(false);
                    }
                }
                else
                {
                    Halt = true;
                }
            }
        }

        /// <summary>
        /// Shows open file dialog and loads selected file if possible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;

            if (FileName == null) FileName = "";

            int index = FileName.Length - 1;
            if (index < 0) index = 0;

            while (index > 0 && FileName[index] != '\\') index--;

            string dir = FileName.Substring(0, index);
            if (Directory.Exists(dir)) ofd.InitialDirectory = dir;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFile(ofd.FileName);
            }
        }

        /// <summary>
        /// Loads file specified by given path.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool LoadFile(string file)
        {
            if (!System.IO.File.Exists(file)) return false;

            try
            {
                FileName = file;

                System.IO.StreamReader sr = new System.IO.StreamReader(file);

                InputFastTextBox.Text = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("File could not be loaded.\n" + e.Message, "IO error");
                InputFastTextBox.Text = "";
                Properties.Settings.Default.File = "";
                Properties.Settings.Default.Save();
                return false;
            }

            string shortName = FileName.Split('\\')[FileName.Split('\\').Length - 1];

            this.Text = "Model Learner - " + shortName;

            Properties.Settings.Default["File"] = file;
            Console.WriteLine(Properties.Settings.Default.File);
            Properties.Settings.Default.Save();

            InputFastTextBox.SelectionStart = 0;
            InputFastTextBox.SelectionLength = 0;

            ResetErrorTestCountdown();

            return true;
        }

        /// <summary>
        /// Saves current file if possible
        /// </summary>
        /// <param name="file"></param>
        private void SaveFile(string file)
        {
            System.IO.StreamWriter sw = null;
            try
            {
                sw = new System.IO.StreamWriter(file);

                sw.Write(InputFastTextBox.Text);

                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("File could not be saved.\n" + e.Message, "IO error");
            }
            finally
            {
                if (sw != null) sw.Close();
            }

        }

        /// <summary>
        /// Selected algorithm change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SelectedAlgorithm = comboBoxAlgorithmSelect.SelectedIndex;
            Properties.Settings.Default.Save();

            if ((string)comboBoxAlgorithmSelect.SelectedItem == "LOUGA") panelLOUGA.Visible = true;
            else panelLOUGA.Visible = false;
            
            if ((string)comboBoxAlgorithmSelect.SelectedItem == "ARMS") panelARMS.Visible = true;
            else panelARMS.Visible = false;
        }

        /// <summary>
        /// Managing of periodic events in application: mainly updating OutputManager and checking for Highlighter outcome.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            //manager update
            lock (Manager)
            {
                if (AlgRunning)
                {
                    Manager.Update();
                }
                else if (Halt)
                {
                    Manager.Update();

                    if (Manager.TabsIndices.ContainsKey(OutputManager.SolutionTabName))
                    {
                        HighlightPDDLSyntax(Manager.TextBoxes[Manager.TabsIndices[OutputManager.SolutionTabName]]);
                    }
                    SetEnableUI(true);
                    Halt = false;

                    AlgorithmStopwatch.Stop();
                    Manager.WriteLine("\nElapsed time: " + AlgorithmStopwatch.ElapsedMilliseconds / 1000d + " seconds.");
                    Manager.Update();
                }
            }

            ErrorTestCountdown--;
            if (ErrorTestCountdown == 0) 
            {
                FindErrors();

                fastTextBox_VisibleRangeChanged(InputFastTextBox, null);
            }
        }

        private void checkBoxPDDLOutput_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_PDDLOutput = checkBoxLOUGAPDDLOutput.Checked;
            Properties.Settings.Default.Save();

            Manager.ReformatIndividuals();
        }

        private void checkBoxGoalStateComplete_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_GoalStateComplete = checkBoxLOUGAGoalStateComplete.Checked;
            Properties.Settings.Default.Save();
        }

        private void textBoxLOUGAPopSize_Validated(object sender, EventArgs e)
        {
            int popSize;
            if (Int32.TryParse(textBoxLOUGAPopSize.Text, out popSize))
            {
                if (popSize < 2)
                {
                    popSize = 2;
                    textBoxLOUGAPopSize.Text = "2";
                }
                Properties.Settings.Default.LOUGA_PopulationSize = popSize;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGAPopSize.Text = Properties.Settings.Default.LOUGA_PopulationSize.ToString();

        }

        private void textBoxLOUGAGenCount_Validated(object sender, EventArgs e)
        {
            int genCount;
            if (Int32.TryParse(textBoxLOUGAGenCount.Text, out genCount))
            {
                if (genCount < 1)
                {
                    genCount = 1;
                    textBoxLOUGAPopSize.Text = "1";
                }
                Properties.Settings.Default.LOUGA_GenerationsCount = genCount;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGAGenCount.Text = Properties.Settings.Default.LOUGA_GenerationsCount.ToString();

        }

        private void textBoxLOUGAPopRestartTreshold_Validated(object sender, EventArgs e)
        {
            int number;
            if (Int32.TryParse(textBoxLOUGAPopRestartTreshold.Text, out number))
            {
                Properties.Settings.Default.LOUGA_PopulationRestartTreshold = number;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGAPopRestartTreshold.Text = Properties.Settings.Default.LOUGA_PopulationRestartTreshold.ToString();

        }

        private void textBoxLOUGAOldPopRestartTreshold_Validated(object sender, EventArgs e)
        {
            int number;
            if (Int32.TryParse(textBoxLOUGAOldPopRestartTreshold.Text, out number))
            {
                Properties.Settings.Default.LOUGA_OldPopulationRestartTreshold = number;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGAOldPopRestartTreshold.Text = Properties.Settings.Default.LOUGA_OldPopulationRestartTreshold.ToString();
        }

        private void textBoxLOUGAGreedySearchTreshold_Validated(object sender, EventArgs e)
        {
            int number;
            if (Int32.TryParse(textBoxLOUGAGreedySearchTreshold.Text, out number))
            {
                Properties.Settings.Default.LOUGA_GreedySearchTreshold = number;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGAGreedySearchTreshold.Text = Properties.Settings.Default.LOUGA_GreedySearchTreshold.ToString();
        }

        private void textBoxLOUGACrossWithOldPopTreshold_Validated(object sender, EventArgs e)
        {
            int number;
            if (Int32.TryParse(textBoxLOUGACrossWithOldPopTreshold.Text, out number))
            {
                Properties.Settings.Default.LOUGA_CrossoverWithOldPopulationTreshold = number;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGACrossWithOldPopTreshold.Text = Properties.Settings.Default.LOUGA_CrossoverWithOldPopulationTreshold.ToString();
        }

        private void textBoxARMSMinAConstrWeight_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxARMSMinAConstrWeight.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxARMSMinAConstrWeight.Text = "0";
                }
                Properties.Settings.Default.ARMS_MinActionConstraintWeight = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSMinAConstrWeight.Text = Properties.Settings.Default.ARMS_MinActionConstraintWeight.ToString();
        }

        private void textBoxARMSMaxAConstrWeight_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxARMSMaxAConstrWeight.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxARMSMaxAConstrWeight.Text = "0";
                }
                Properties.Settings.Default.ARMS_MaxActionConstraintWeight = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSMaxAConstrWeight.Text = Properties.Settings.Default.ARMS_MaxActionConstraintWeight.ToString();
        }

        private void textBoxARMSInfConstrWeight_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxARMSInfConstrWeight.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxARMSInfConstrWeight.Text = "0";
                }
                Properties.Settings.Default.ARMS_InformationConstraintWeight = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSInfConstrWeight.Text = Properties.Settings.Default.ARMS_InformationConstraintWeight.ToString();
        }

        private void textBoxARMSProbTreshold_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxARMSProbTreshold.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxARMSProbTreshold.Text = "0";
                }
                Properties.Settings.Default.ARMS_ProbabilityTreshold = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSProbTreshold.Text = Properties.Settings.Default.ARMS_ProbabilityTreshold.ToString();
        }

        private void buttonStartStop_Click(object sender, EventArgs e)
        {
            buttonLearn_Click(buttonLearn, e);
        }

        private void textBoxARMSMaxSatNrOfTries_Validated(object sender, EventArgs e)
        {
            int number;
            if (Int32.TryParse(textBoxARMSMaxSatNrOfTries.Text, out number))
            {
                if (number < 1)
                {
                    number = 1;
                    textBoxARMSMaxSatNrOfTries.Text = "1";
                }
                Properties.Settings.Default.ARMS_MaxSatNumberOfTries = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSMaxSatNrOfTries.Text = Properties.Settings.Default.ARMS_MaxSatNumberOfTries.ToString();
        }

        private void textBoxARMSMaxSatRestartTreshold_Validated(object sender, EventArgs e)
        {
            int number;
            if (Int32.TryParse(textBoxARMSMaxSatRestartTreshold.Text, out number))
            {
                if (number < 2)
                {
                    number = 2;
                    textBoxARMSMaxSatRestartTreshold.Text = "2";
                }
                Properties.Settings.Default.ARMS_MaxSatRestartTreshold = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSMaxSatRestartTreshold.Text = Properties.Settings.Default.ARMS_MaxSatRestartTreshold.ToString();
        }

        private void textBoxARMSMaxSatRandomChoiceProbability_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxARMSMaxSatRandomChoiceProbability.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxARMSMaxSatRandomChoiceProbability.Text = "0";
                }
                if (number > 1)
                {
                    number = 1;
                    textBoxARMSMaxSatRandomChoiceProbability.Text = "1";
                }
                Properties.Settings.Default.ARMS_MaxSatRandomChoiceProbability = number;
                Properties.Settings.Default.Save();
            }
            else textBoxARMSMaxSatRandomChoiceProbability.Text = Properties.Settings.Default.ARMS_MaxSatRandomChoiceProbability.ToString();
        }

        private void comboBoxARMSMaxSatAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ARMS_MaxSatAlgorithm = comboBoxARMSMaxSatAlgorithm.Text;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Highlights PDDL syntax in given RichTextBox
        /// </summary>
        /// <param name="textBox"></param>
        public static void HighlightPDDLSyntax(RichTextBox textBox)
        {
            HighlightPDDLSyntax(textBox, 0, textBox.Text.Length);
        }

        /// <summary>
        /// Highlights PDDL keywords and comments in given part of given RichTextBox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public static void HighlightPDDLSyntax(RichTextBox textBox, int index, int length)
        {
            int originalIndex = textBox.SelectionStart;
            int originalLength = textBox.SelectionLength;

            if (index + length > textBox.Text.Length) length = textBox.Text.Length - index;

            string substring = textBox.Text.Substring(index, length);
            
            //adding spaces for correct behavior at the beggining and end of string
            substring = " " + substring + " ";

            string prefix = @"(?<=[\s()])";
            string suffix = @"(?=[\s();])";

            //keywords
            string keywords = prefix + @"(define|domain|world)" + suffix;
            keywords += "|" + @":(domain|plan|state|predicates|action|types|requirements|objects|constants)\s";
            MatchCollection keywordMatches = Regex.Matches(substring, keywords);

            keywords = prefix + @"(and|not)" + suffix;
            keywords += "|" + @":(parameters|precondition|effect)\s";
            MatchCollection lesserkeywordMatches = Regex.Matches(substring, keywords);

            keywords = prefix + @"\?.+?" + suffix;
            MatchCollection parameterMatches = Regex.Matches(substring, keywords);

            //comments
            keywords = @";.*\n";
            MatchCollection commentMatches = Regex.Matches(substring, keywords, RegexOptions.Multiline);

            bool focused = false;
            if (textBox.Focused)
            {
                focused = true;
                LabelForFocusing.Focus();
            }

            textBox.SelectionStart = index;
            textBox.SelectionLength = length;
            textBox.SelectionColor = DefaultColor;
            textBox.SelectionFont = new Font(textBox.Font, FontStyle.Regular);


            //recoloring
            foreach (Match m in keywordMatches)
            {
                textBox.SelectionStart = m.Index + index - 1;
                textBox.SelectionLength = m.Length;
                textBox.SelectionColor = KeyWordColor;
            }

            foreach (Match m in lesserkeywordMatches)
            {
                textBox.SelectionStart = m.Index + index - 1;
                textBox.SelectionLength = m.Length;
                textBox.SelectionColor = LesserKeyWordColor;
            }

            foreach (Match m in parameterMatches)
            {
                textBox.SelectionStart = m.Index + index - 1;
                textBox.SelectionLength = m.Length;
                textBox.SelectionColor = ParameterColor;
            }

            foreach (Match m in commentMatches)
            {
                textBox.SelectionStart = m.Index + index - 1;
                textBox.SelectionLength = m.Length;
                textBox.SelectionColor = CommentColor;
            }

            textBox.SelectionStart = originalIndex;
            textBox.SelectionLength = originalLength;

            //focusing the textbox again
            if (focused)
            {
                textBox.Focus();
            }
        }

        /// <summary>
        /// Decodes model and worlds from InputFastTextBox and finds errors.
        /// </summary>
        private void FindErrors()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            string text = InputFastTextBox.Text;

            try
            {
                inputModel = StringDomainReader.DecodeString(text, out Errors, out worlds);
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not parse given text.\n" + e.Message, "Parse error");
            }

            Errors.Sort((x, y) => x.Index.CompareTo(y.Index));

            if (Errors.Count > 1)
            {
                labelErrors.Text = Errors.Count + " errors";
            }
            else if (Errors.Count == 1)
            {
                labelErrors.Text = Errors.Count + " error";
            }
            else
            {
                labelErrors.Text = "";
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Sets error style to current selected text in given RichTextBox.
        /// </summary>
        /// <param name="box"></param>
        private void SetErrorStyle(RichTextBox box)
        {
            InputTextBox.SelectionFont = new Font(InputTextBox.SelectionFont, FontStyle.Underline);
            InputTextBox.SelectionColor = Color.Red;
        }

        private void checkBoxLOUGAEndAfterFirstSolution_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_EndAfterSolutionIsFound = checkBoxLOUGAEndAfterFirstSolution.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// File saving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (FileName == null || FileName == "")
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.DefaultExt = "txt";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FileName = sfd.FileName;
                    
                    Text = "Model Learner - " + FileName.Split('\\')[FileName.Split('\\').Length - 1];
                    Properties.Settings.Default.File = FileName;
                    Properties.Settings.Default.Save();
                }
            }


            if (FileName != null)
            {
                SaveFile(FileName);
            }
        }

        /// <summary>
        /// Creates a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNew_Click(object sender, EventArgs e)
        {
            if (InputTextBox.Text != "") 
            {   
                DialogResult result = MessageBox.Show("Do you really want to create a new file?", "Create new file?", MessageBoxButtons.YesNo);
                if (result != System.Windows.Forms.DialogResult.Yes) return;
            }
            InputFastTextBox.Text = "";
            FileName = "";
            
            Properties.Settings.Default.File = "";
            Properties.Settings.Default.Save();

            Text = "Model Learner";
        }

        /// <summary>
        /// Shows user next error in input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelErrors_Click(object sender, EventArgs e)
        {
            ShowNextError();
        }

        /// <summary>
        /// Highlights next error in current content of InputFastTextBox
        /// </summary>
        private void ShowNextError()
        {
            if (Errors != null && Errors.Count > 0)
            {
                CurrentErrorIndex++;
                if (CurrentErrorIndex >= Errors.Count) CurrentErrorIndex %= Errors.Count;

                InputFastTextBox.Selection = InputFastTextBox.GetRange(
                    new Place(Errors[CurrentErrorIndex].IndexInLine, Errors[CurrentErrorIndex].Line),
                    new Place(Errors[CurrentErrorIndex].IndexInLine + Errors[CurrentErrorIndex].Length, Errors[CurrentErrorIndex].Line));

                InputFastTextBox.DoCaretVisible();
            }
        }

        private void checkBoxLOUGAPredicateByPredicate_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_PredicateByPredicate = checkBoxLOUGAPredicateByPredicate.Checked;
            Properties.Settings.Default.Save();

            checkBoxLOUGAEndAfterFirstSolution.Enabled = !checkBoxLOUGAPredicateByPredicate.Checked;
            if (checkBoxLOUGAPredicateByPredicate.Checked) checkBoxLOUGAEndAfterFirstSolution.Checked = true;
        }

        private void textBoxLOUGAMaximalErrorRate_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxLOUGAMaximalErrorRate.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxLOUGAMaximalErrorRate.Text = "0";
                }
                if (number > 1)
                {
                    number = 1;
                    textBoxLOUGAMaximalErrorRate.Text = "1";
                }
                Properties.Settings.Default.LOUGA_MaxPredicateErrorRate = number;
                Properties.Settings.Default.Save();

                Manager.ReformatIndividuals();
            }
            else textBoxLOUGAMaximalErrorRate.Text = Properties.Settings.Default.LOUGA_MaxPredicateErrorRate.ToString();
        }

        private void checkBoxLOUGAGenerateNegativePreconditions_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_GenerateNegativePreconditions = checkBoxLOUGAGenerateNegativePreconditions.Checked;
            Properties.Settings.Default.Save();
            Manager.ReformatIndividuals();
        }

        private void checkBoxLOUGAGenerateCompletePreconditionsLists_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_GenerateCompletePreconditionsLists = checkBoxLOUGAGenerateCompletePreconditionsLists.Checked;
            Properties.Settings.Default.Save();
            Manager.ReformatIndividuals();
        }

        private void checkBoxLOUGAParametersCanRepeat_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LOUGA_ParametersCanRepeat = checkBoxLOUGAParametersCanRepeat.Checked;
            Properties.Settings.Default.Save();
        }

        private void textBoxLOUGAObservationErrorWeight_Validated(object sender, EventArgs e)
        {
            double number;
            if (Double.TryParse(textBoxLOUGAObservationErrorWeight.Text, out number))
            {
                if (number < 0)
                {
                    number = 0;
                    textBoxLOUGAObservationErrorWeight.Text = "0";
                }
                if (number > 1)
                {
                    number = 1;
                    textBoxLOUGAObservationErrorWeight.Text = "1";
                }
                Properties.Settings.Default.LOUGA_ObservationErrorWeight = number;
                Properties.Settings.Default.Save();
            }
            else textBoxLOUGAObservationErrorWeight.Text = Properties.Settings.Default.LOUGA_ObservationErrorWeight.ToString();
        }

        private void comboBoxLOUGAPredicates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLOUGAPredicates.SelectedIndex == ComboBoxPredicatesLastSelectedIndex) return;

            ComboBoxPredicatesLastSelectedIndex = comboBoxLOUGAPredicates.SelectedIndex;

            string predicateName = comboBoxLOUGAPredicates.SelectedItem.ToString().Split(' ')[0];
            string name = predicateName;
            if (name.Length > 18) name = predicateName.Substring(0,18);
                
            Manager.ShowUIForPredicate(PredicateIds[predicateName], name);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            comboBoxLOUGAPredicates.Location = new Point(this.Size.Width - 180, comboBoxLOUGAPredicates.Location.Y);
            
        }

        /// <summary>
        /// NFold test used in experiments showed in paper.
        /// To enable, make button1, button2 and button3 visible. 
        /// Method uses current settings the same as 
        /// </summary>
        /// <param name="algs">Array of instances of learning algorithms. Number of algorithms sets number of folds used during test.</param>
        /// <param name="fileName">name of input file</param>
        private void DoNFoldTest(ILearningAlgorithm[] algs, string fileName)
        {
            int N = algs.Length;
            Random RG = new Random();

            StreamReader sr = new StreamReader(fileName);
            string s = sr.ReadToEnd();
            sr.Close();

            List<World> worlds;
            List<InputException> errors;
            Model m = StringDomainReader.DecodeString(s, out errors, out worlds);

            int planCount = 0;
            foreach (var world in worlds)
            {
                planCount += world.Plans.Length;
            }

            int addErrorCount = 0;
            int delErrorCount = 0;
            int preErrorCount = 0;
            int obsErrorCount = 0;
            int totalTime = 0;

            double addErrorSum = 0;
            double delErrorSum = 0;
            double preErrorSum = 0;
            double obsErrorSum = 0;

            //splitting input plans in groups
            List<int> unusedPlans = new List<int>();
            for (int i = 0; i < planCount; i++)
            {
                unusedPlans.Add(i);
            }
            
            List<int>[] parts = new List<int>[N];
            for (int i = 0; i < N; i++)
			{
                parts[i] = new List<int>();
			}

            int remaining = planCount;
            for (int i = 0; i < N; i++)
            {
                while (parts[i].Count < remaining / (N - i))
                {
                    int index = RG.Next(unusedPlans.Count);
                    parts[i].Add(unusedPlans[index]);
                    unusedPlans.RemoveAt(index);
                }

                remaining -= remaining / (N - i);
            }

            //Performing tests
            string output = "";
            for (int i = 0; i < N; i++)
            {
                m = StringDomainReader.DecodeString(s, out errors, out worlds);

                //separating control group
                List<Plan>[] inputPlans = new List<Plan>[worlds.Count];
                List<Plan>[] controlGroup = new List<Plan>[worlds.Count];

                for (int j = 0; j < worlds.Count; j++)
                {
                    inputPlans[j] = new List<Plan>();
                    controlGroup[j] = new List<Plan>();
                }

                int currentIndex = 0;
                int worldIndex = 0;
                foreach (var world in worlds)
                {
                    foreach (var plan in world.Plans)
                    {
                        if (parts[i].Contains(currentIndex))
                        {
                            controlGroup[worldIndex].Add(plan);

                            Console.WriteLine(currentIndex + "C");
                        }
                        else
                        {
                            inputPlans[worldIndex].Add(plan);

                            //Console.WriteLine(i);
                        }

                        currentIndex++;
                    }
                    world.Plans = inputPlans[worldIndex].ToArray();

                    worldIndex++;
                }

                Stopwatch sw = new Stopwatch();
                
                sw.Start();
                Model outputModel = algs[i].Learn(m, worlds);
                sw.Stop();

                worldIndex = 0;
                foreach (var world in worlds)
                {
                    world.Plans = controlGroup[worldIndex].ToArray();

                    worldIndex++;
                }

                ErrorInfo err = Utility.CountErrors(outputModel, worlds);

                output += err.ToString();
                output += "\n";
                output += "Elapsed time: " + sw.ElapsedMilliseconds / 1000d + " seconds.\n\n";
                
                totalTime += (int)sw.ElapsedMilliseconds;
                if (err.TotalAdd > 0) { addErrorCount++; addErrorSum += (double)err.ErrorsAdd / err.TotalAdd; }
                if (err.TotalDel > 0) { delErrorCount++; delErrorSum += (double)err.ErrorsDel / err.TotalDel; }
                if (err.TotalPre > 0) { preErrorCount++; preErrorSum += (double)err.ErrorsPre / err.TotalPre; }
                if (err.TotalObservation > 0) { obsErrorCount++; obsErrorSum += (double)err.ErrorsObservation / err.TotalObservation; }
            }

            Console.WriteLine(output);
            Console.WriteLine();

            Console.WriteLine("Average add error rate; {0}", addErrorSum / addErrorCount);
            Console.WriteLine("Average del error rate; {0}", delErrorSum / delErrorCount);
            Console.WriteLine("Average pre error rate; {0}", preErrorSum / preErrorCount);
            Console.WriteLine("Average obs error rate; {0}", obsErrorSum / obsErrorCount);

            Console.WriteLine("Average time: {0}", totalTime / 1000d / N);
        }

        /// <summary>
        /// Does NFoldTest for ARMS algorithm.
        /// Uses current settings shown in UI.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="fileName"></param>
        private void NFoldTestARMS(int N, string fileName)
        {
            Manager.SetOutputTabs(
                new string[] { OutputManager.OutputTabName, OutputManager.SolutionTabName },
                null
            );

            IMaxSatSolver solver;

            if (comboBoxARMSMaxSatAlgorithm.Text == "GSAT") solver = new GSAT();
            else solver = new WalkSat();

            ILearningAlgorithm[] algs = new ILearningAlgorithm[N];
            for (int i = 0; i < N; i++)
            {
                algs[i] = new ARMS(solver);
            }

            DoNFoldTest(algs, fileName);
        }

        /// <summary>
        /// Does NFold test on predicate by predicate mode of LOUGA algorithm
        /// Uses current settings shown in UI.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="fileName"></param>
        private void NFoldTestLOUGA(int N, string fileName)
        {
            panelBestIndividuals.Width = this.Size.Width - 1280 + 604;
            panelBestIndividuals.Height = this.Size.Height - 720 + 625;

            string[] tabNames = new string[2 + 2 * inputModel.Predicates.Count];
            TabType[] tabTypes = new TabType[tabNames.Length];
            bool[] visibleTabs = new bool[tabNames.Length];

            tabNames[0] = OutputManager.OutputTabName;
            tabNames[1] = OutputManager.SolutionTabName;

            visibleTabs[0] = true;
            visibleTabs[1] = true;
            visibleTabs[2] = true;
            visibleTabs[3] = true;

            comboBoxLOUGAPredicates.Items.Clear();
            PredicateIds = new Dictionary<string, int>();

            //sorting predicates
            SortedDictionary<string, Predicate> sortedPredicates = new SortedDictionary<string, Predicate>();
            foreach (var pair in inputModel.Predicates)
            {
                sortedPredicates.Add(pair.Key, pair.Value);
            }

            //creating new tab for each predicate
            int id = 2;
            foreach (var predicate in sortedPredicates)
            {
                tabNames[id] = "Ouput - " + predicate.Key;
                tabNames[id + 1] = "Individuals - " + predicate.Key;

                tabTypes[id + 1] = TabType.ListBoxBestIndividuals;

                comboBoxLOUGAPredicates.Items.Add(predicate.Key);

                PredicateIds.Add(predicate.Key, id);

                id += 2;
            }

            Manager.SetOutputTabs(tabNames, tabTypes, visibleTabs);
            comboBoxLOUGAPredicates.Visible = true;
            comboBoxLOUGAPredicates.SelectedIndex = 0;
            comboBoxLOUGAPredicates.Location = new Point(this.Size.Width - 180, 7);
            ComboBoxPredicatesLastSelectedIndex = -1;

            ILearningAlgorithm[] algs = new ILearningAlgorithm[N];
            for (int i = 0; i < N; i++)
            {
                algs[i] = new LOUGAPredicateByPredicate();
            }

            DoNFoldTest(algs, fileName);
        }

        /// <summary>
        /// Does NFold test on regular mode of LOUGA algortihm
        /// Uses current settings shown in UI.
        /// </summary>
        /// <param name="N"></param>
        /// <param name="fileName"></param>
        private void NFoldTestLOUGAStd(int N, string fileName)
        {
            panelBestIndividuals.Width = this.Size.Width - 1280 + 604;
            panelBestIndividuals.Height = this.Size.Height - 720 + 625;

            Manager.SetOutputTabs(
                new string[] { OutputManager.OutputTabName, LOUGA.CurrentBestTabName },
                new TabType[] { TabType.OutputTab, TabType.ListBoxBestIndividuals }
            );

            ILearningAlgorithm[] algs = new ILearningAlgorithm[N];
            for (int i = 0; i < N; i++)
            {
                algs[i] = new LOUGA();
            }

            DoNFoldTest(algs, fileName);
        }

        const int NFoldTestNumberOfFolds = 100;

        private void button1_Click(object sender, EventArgs e)
        {
            NFoldTestARMS(NFoldTestNumberOfFolds, FileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NFoldTestLOUGA(NFoldTestNumberOfFolds, FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NFoldTestLOUGAStd(NFoldTestNumberOfFolds, FileName);
        }

        /// <summary>
        /// Syntax highlighting of text in InputFastTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fastTextBox_VisibleRangeChanged(object sender, EventArgs e)
        {
            Range range = ((FastColoredTextBox)sender).VisibleRange;

            range.ClearStyle(keywordStyle, commentStyle, lesserKeywordStyle, errorStyle, parameterStyle);

            string prefix = @"(?<=([\s()]|^))";
            string suffix = @"(?=([\s();]|$))";

            //comments
            string keywords = @";.*\n";
            range.SetStyle(commentStyle, prefix + keywords);

            //keywords
            keywords = @"(define|domain|world)";
            keywords += "|" + @":(domain|predicates|plan|state|action|types|requirements|objects|constants)";
            range.SetStyle(keywordStyle, prefix + keywords + suffix);

            keywords = @"(and|not)";
            keywords += "|" + @":(parameters|precondition|effect)";
            range.SetStyle(lesserKeywordStyle, prefix + keywords + suffix);

            keywords = @"\?.+?";
            range.SetStyle(parameterStyle, prefix + keywords + suffix);


            if (Errors != null)
            {
                foreach (var error in Errors)
                {
                    if (error.Line > range.ToLine || error.Line < range.FromLine) continue;

                    Range errorRange = InputFastTextBox.GetRange(new Place(error.IndexInLine, error.Line), new Place(error.IndexInLine + error.Length, error.Line));

                    errorRange.SetStyle(errorStyle);
                }
            }
        }

        private void InputFastTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetErrorTestCountdown();
            CurrentErrorIndex = -1;
        }

        private void ResetErrorTestCountdown() 
        {
            Errors.Clear();
            ErrorTestCountdown = ErrorTestDelay;
        }

        /// <summary>
        /// This method shows info about errors in InputFastTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InputFastTextBox_ToolTipNeeded(object sender, ToolTipNeededEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.HoveredWord))
            {
                foreach (var error in Errors)
                {
                    if (e.Place.iLine == error.Line && e.Place.iChar >= error.IndexInLine - 1 && e.Place.iChar <= error.IndexInLine + error.Length)
                    {
                        e.ToolTipTitle = e.HoveredWord;
                        e.ToolTipText = error.Text;
                    }
                }
            }
        }
    }
}
