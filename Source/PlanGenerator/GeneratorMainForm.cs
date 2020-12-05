using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeneralPlanningLibrary;
using GeneralPlanningLibrary.Utility;

namespace PlanGenerator
{
    public partial class panel : Form
    {
        public panel()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            textBoxPredicateChance.Text = Properties.Settings.Default.PredicateChance.ToString();
            textBoxBFSGoalPredicates.Text = Properties.Settings.Default.BFSGoalPredicatesCount.ToString();
            textBoxBFSMaxVisitedStates.Text = Properties.Settings.Default.BFSMaxVisitedStates.ToString();
            textBoxBFSVisitedStates.Text = Properties.Settings.Default.BFSVisitedStates.ToString();
            textBoxBFSWideMinimalPlanLength.Text = Properties.Settings.Default.BFSMinimalPlanLength.ToString();
            textBoxBFSMinimalPlanLength.Text = Properties.Settings.Default.BFSMinimalPlanLength.ToString();
            textBoxInputFile.Text = Properties.Settings.Default.InputFile;
            textBoxOutputFile.Text = Properties.Settings.Default.OutputFile;
            textBoxPlansCount.Text = Properties.Settings.Default.PlansCount.ToString();
            textBoxSequencesDesiredActionsCount.Text = Properties.Settings.Default.SequencesTargetActions.ToString();
            textBoxSequencesMinActions.Text = Properties.Settings.Default.SequencesMinActions.ToString();

            comboBoxGeneratingMethod.SelectedIndex = Properties.Settings.Default.GeneratingMethodId;
            comboBoxGoalState.SelectedIndex = Properties.Settings.Default.GoalStateOutputId;
            comboBoxInitState.SelectedIndex = Properties.Settings.Default.InitStateOutputId;
            comboBoxTransitionStates.SelectedIndex = Properties.Settings.Default.TransitionStatesOuputIds;

            checkBoxRemoveOperatorsLists.Checked = Properties.Settings.Default.RemoveOperatorsLists;
            checkBoxRemoveTypeOptions.Checked = Properties.Settings.Default.RemoveTypeOptions;
            checkBoxRemoveTyping.Checked = Properties.Settings.Default.RemoveTyping;
            checkBoxParametersCanRepeatInActions.Checked = Properties.Settings.Default.ParametersCanRepeatInActions;

            labelPlansGenerated.Text = "";
            labelStatesCount.Text = "";
            labelStatesVisited.Text = "";
            labelCurrentDepth.Text = "";
            labelCurrentInitStateId.Text = "";
            panelBFSProgress.Hide();
        }

        private void Generate()
        {
            string path = textBoxInputFile.Text;

            //Input file check
            if (!File.Exists(path))
            {
                MessageBox.Show("Input file does not exist.");
                return;
            }


            StreamReader sr = new StreamReader(path);
            
            string input = sr.ReadToEnd();
            
            sr.Close();

            //Input data parse and check
            List<InputException> errors;
            List<World> worlds;
            Model m = StringDomainReader.DecodeString(input, out errors, out worlds);

            if (errors.Count > 0)
            {
                MessageBox.Show("Error in input data: \n" + errors[0].Text + "\nOn line " + errors[0].Line + ", character " + errors[0].IndexInLine);
                return;
            }

            //checking if every plan has init state
            for (int i = 0; i < worlds.Count; i++)
            {
                if (worlds[i].Plans == null)
                {
                    MessageBox.Show("World " + i + " is missing plans.");
                    return;
                }
                for (int j = 0; j < worlds[i].Plans.Length; j++)
                {
                    if (worlds[i].Plans[j].States == null || worlds[i].Plans[j].States.Length == 0 || worlds[i].Plans[j].States[0] == null)
                    {
                        MessageBox.Show("World " + i + " plan " + j + " missing init state.");
                        return;
                    }
                }
            }

            //checking if it's possible to create output file
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(textBoxOutputFile.Text);
            }
            catch
            {
                MessageBox.Show("Output file could not be created.");
                return;
            }
            if (sw == null)
            {
                MessageBox.Show("Output file could not be created.");
                return;
            }

            //removing type options
            if (checkBoxRemoveTypeOptions.Checked)
            {
                m.RemoveOperatorsParameterTypeOptions(worlds);
            }

            //removing types
            if (checkBoxRemoveTyping.Checked)
            {
                Utility.RemoveTyping(m, worlds);
            }

            //preparing operators
            foreach (var o in m.Operators)
            {
                o.Value.FormLists();
            }

            //preparing background worker
            Generator g = new Generator();
            BackgroundWorker bw = new BackgroundWorker();
            
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(workerProgressChanged);
            bw.DoWork += new DoWorkEventHandler((sender, e) => {
                bw.ReportProgress(-1, new ProgressState(0, 0, 0, 0, 0));

                switch (Properties.Settings.Default.GeneratingMethodId)
                {
                    case 0: // random sequences
                        g.GenerateRandomSequences(worlds, Properties.Settings.Default.PlansCount, bw.ReportProgress);
                        break;
                    case 1: // random plans one by one
                        g.GenerateRandomPlans(worlds, m, Properties.Settings.Default.PlansCount, bw.ReportProgress);
                        break;
                    case 2: // BFS and pick random goal states
                        g.BFSRandomSequences(worlds, Properties.Settings.Default.PlansCount, bw.ReportProgress);
                        break;

                    default:
                        break;
                }
                
                g.WriteToFile(sw, m, worlds);
                sw.Close();
                bw.ReportProgress(int.MaxValue);
            });

            //setting UI
            SetEnableUI(false);

            if (comboBoxGeneratingMethod.SelectedIndex == 0) panelBFSProgress.Hide();
            else panelBFSProgress.Show();

            bw.RunWorkerAsync();
        }

        /// <summary>
        /// This method is used for reporting progress of generating process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == int.MaxValue)
            {
                SetEnableUI(true);
            }
            else
            {
                ProgressState state = (ProgressState)e.UserState;

                labelPlansGenerated.Text = state.PlansGenerated + "/" + textBoxPlansCount.Text;
                labelStatesCount.Text = state.BFSOpenStates.ToString();
                labelStatesVisited.Text = state.BFSStatesVisited.ToString();
                labelCurrentDepth.Text = state.BFSCurrentDepth.ToString();
                labelCurrentInitStateId.Text = state.CurrentInitState.ToString();
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Generate();
        }

        private void buttonSetFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;

            //opening current file's directory
            int index = textBoxInputFile.Text.Length - 1;
            if (index < 0) index = 0;

            while (index > 0 && textBoxInputFile.Text[index] != '\\') index--;

            string dir = textBoxInputFile.Text.Substring(0, index);
            if (Directory.Exists(dir)) ofd.InitialDirectory = dir;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxInputFile.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Parses int from given text. If text doesn't contain string, returns defaultValue. 
        /// If the parsed number is not in given bounds, moves it within these bounds.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private int ParseInt(string text, int defaultValue, int min = Int32.MinValue, int max = Int32.MaxValue)
        {
            int result;

            if (Int32.TryParse(text, out result))
            {
                if (result < min) result = min;
                else if (result > max) result = max;

                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Parses int from given text. If text doesn't contain string, returns defaultValue. 
        /// If the parsed number is not in given bounds, moves it within these bounds.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private double ParseDouble(string text, double defaultValue, double min = double.MinValue, double max = double.MaxValue)
        {
            double result;

            if (Double.TryParse(text, out result))
            {
                if (result < min) result = min;
                else if (result > max) result = max;

                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Enables or disables UI
        /// </summary>
        /// <param name="enabled">TRUE for enable, FALSE for disable</param>
        private void SetEnableUI(bool enabled)
        {
            textBoxBFSGoalPredicates.Enabled = enabled;
            textBoxBFSMaxVisitedStates.Enabled = enabled;
            textBoxBFSMinimalPlanLength.Enabled = enabled;
            textBoxBFSVisitedStates.Enabled = enabled;
            textBoxBFSWideMinimalPlanLength.Enabled = enabled;
            textBoxOutputFile.Enabled = enabled;
            textBoxPlansCount.Enabled = enabled;
            textBoxPredicateChance.Enabled = enabled;
            textBoxSequencesDesiredActionsCount.Enabled = enabled;
            textBoxSequencesMinActions.Enabled = enabled;

            comboBoxGeneratingMethod.Enabled = enabled;
            comboBoxGoalState.Enabled = enabled;
            comboBoxInitState.Enabled = enabled;
            comboBoxTransitionStates.Enabled = enabled;

            checkBoxRemoveOperatorsLists.Enabled = enabled;
            checkBoxRemoveTyping.Enabled = enabled;
            checkBoxRemoveTypeOptions.Enabled = enabled;
            checkBoxParametersCanRepeatInActions.Enabled = enabled;

            buttonGenerate.Enabled = enabled;
            buttonSetFile.Enabled = enabled;
        }

        private void textBoxOutputFile_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.OutputFile = textBoxOutputFile.Text;
            Properties.Settings.Default.Save();
        }
        
        /// <summary>
        /// User selected different generating method -> UI panels switch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxGeneratingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.GeneratingMethodId = comboBoxGeneratingMethod.SelectedIndex;
            Properties.Settings.Default.Save();

            switch (comboBoxGeneratingMethod.SelectedIndex)
            {
                case 0: // random sequences
                    panelBFS.Hide();
                    panelBFSWide.Hide();
                    panelSequences.Location = new Point(12, 137);
                    panelSequences.Show();
                    break;
                case 1: // random plans one by one
                    panelSequences.Hide();
                    panelBFSWide.Hide();
                    panelBFS.Location = new Point(12, 137);
                    panelBFS.Show();
                    break;
                case 2: // BFS and pick random goal states
                    panelBFS.Hide();
                    panelSequences.Hide();
                    panelBFSWide.Location = new Point(12, 137);
                    panelBFSWide.Show();
                    break;
                default:
                    break;
            }
        }

        private void comboBoxInitState_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.InitStateOutputId = comboBoxInitState.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void comboBoxTransitionStates_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TransitionStatesOuputIds = comboBoxTransitionStates.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void comboBoxGoalState_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.GoalStateOutputId = comboBoxGoalState.SelectedIndex;
            Properties.Settings.Default.Save();
        }


        private void textBoxPlansCount_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxPlansCount.Text, Properties.Settings.Default.PlansCount, 1);

            Properties.Settings.Default.PlansCount = result;
            Properties.Settings.Default.Save();

            textBoxPlansCount.Text = result.ToString();
        }

        private void textBoxSequencesMinActions_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxSequencesMinActions.Text, Properties.Settings.Default.SequencesMinActions, 1);

            Properties.Settings.Default.SequencesMinActions = result;
            Properties.Settings.Default.Save();

            textBoxSequencesMinActions.Text = result.ToString();
        }

        private void textBoxSequencesDesiredActionsCount_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxSequencesDesiredActionsCount.Text, Properties.Settings.Default.SequencesTargetActions, 1);

            Properties.Settings.Default.SequencesTargetActions = result;
            Properties.Settings.Default.Save();

            textBoxSequencesDesiredActionsCount.Text = result.ToString();
        }

        private void textBoxPredicateChance_Validated(object sender, EventArgs e)
        {
            double result = ParseDouble(textBoxPredicateChance.Text, Properties.Settings.Default.PredicateChance, 0, 1);

            Properties.Settings.Default.PredicateChance = result;
            Properties.Settings.Default.Save();

            textBoxPredicateChance.Text = result.ToString();
        }

        private void textBoxBFSGoalPredicates_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxBFSGoalPredicates.Text, Properties.Settings.Default.BFSGoalPredicatesCount, 1);

            Properties.Settings.Default.BFSGoalPredicatesCount = result;
            Properties.Settings.Default.Save();

            textBoxBFSGoalPredicates.Text = result.ToString();
        }

        private void textBoxBFSMaxVisitedStates_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxBFSMaxVisitedStates.Text, Properties.Settings.Default.BFSMaxVisitedStates, 1);

            Properties.Settings.Default.BFSMaxVisitedStates = result;
            Properties.Settings.Default.Save();

            textBoxBFSMaxVisitedStates.Text = result.ToString();
        }

        private void textBoxBFSMinimalPlanLength_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxBFSMinimalPlanLength.Text, Properties.Settings.Default.BFSMinimalPlanLength, 1);

            Properties.Settings.Default.BFSMinimalPlanLength = result;
            Properties.Settings.Default.Save();

            textBoxBFSMinimalPlanLength.Text = result.ToString();
            textBoxBFSWideMinimalPlanLength.Text = result.ToString();
        }

        private void textBoxBFSVisitedStates_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxBFSVisitedStates.Text, Properties.Settings.Default.BFSVisitedStates, 1);

            Properties.Settings.Default.BFSVisitedStates = result;
            Properties.Settings.Default.Save();

            textBoxBFSVisitedStates.Text = result.ToString();
        }

        private void textBoxBFSWideMinimalPlanLength_Validated(object sender, EventArgs e)
        {
            int result = ParseInt(textBoxBFSWideMinimalPlanLength.Text, Properties.Settings.Default.BFSMinimalPlanLength, 1);

            Properties.Settings.Default.BFSMinimalPlanLength = result;
            Properties.Settings.Default.Save();

            textBoxBFSMinimalPlanLength.Text = result.ToString();
            textBoxBFSWideMinimalPlanLength.Text = result.ToString();
        }

        private void checkBoxRemoveOperatorsLists_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RemoveOperatorsLists = checkBoxRemoveOperatorsLists.Checked;
            Properties.Settings.Default.Save();
        }

        private void textBoxInputFile_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.InputFile = textBoxInputFile.Text;
            Properties.Settings.Default.Save();
        }

        private void checkBoxRemoveTypeOptions_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RemoveTypeOptions = checkBoxRemoveTypeOptions.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBoxRemoveTyping_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RemoveTyping = checkBoxRemoveTyping.Checked;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// This method is used to generate multiple test datasets with variating level of partial information 
        /// according to array defined in Generator class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(textBoxInputFile.Text);

            string input = sr.ReadToEnd();

            sr.Close();

            List<InputException> errors;
            List<World> worlds;
            Model m = StringDomainReader.DecodeString(input, out errors, out worlds);

            //preparing operators
            foreach (var o in m.Operators)
            {
                o.Value.FormLists();
            }

            //preparing background worker
            Generator g = new Generator();
            BackgroundWorker bw = new BackgroundWorker();

            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(workerProgressChanged);
            bw.DoWork += new DoWorkEventHandler((S, E) =>
            {
                bw.ReportProgress(-1, new ProgressState(0, 0, 0, 0, 0));

                g.GenerateTestData(worlds, Properties.Settings.Default.PlansCount, bw.ReportProgress);

                bw.ReportProgress(int.MaxValue);
            });

            //setting UI
            SetEnableUI(false);

            if (comboBoxGeneratingMethod.SelectedIndex == 0) panelBFSProgress.Hide();
            else panelBFSProgress.Show();

            bw.RunWorkerAsync();
        }

        private void checkBoxParametersCanRepeatInActions_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ParametersCanRepeatInActions = checkBoxParametersCanRepeatInActions.Checked;
            Properties.Settings.Default.Save();
        }
    }

    /// <summary>
    /// Class that represents current progress of generator.
    /// Instances of this class are used by BackgroudWorker that runs generator.
    /// </summary>
    public class ProgressState
    {
        public int PlansGenerated;
        public int CurrentInitState;
        public int BFSStatesVisited;
        public int BFSOpenStates;
        public int BFSCurrentDepth;

        public ProgressState(int plansGenerated, int currentInitState, int BFSStatesVisited, int BFSOpenStates, int BFSCurrentDepth)
        {
            this.PlansGenerated = plansGenerated;
            this.CurrentInitState = currentInitState;
            this.BFSCurrentDepth = BFSCurrentDepth;
            this.BFSOpenStates = BFSOpenStates;
            this.BFSStatesVisited = BFSStatesVisited;
        }
    }
}
