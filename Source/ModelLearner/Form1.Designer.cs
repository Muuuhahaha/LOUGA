namespace ModelLearner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonLearn = new System.Windows.Forms.Button();
            this.InputTextBox = new System.Windows.Forms.RichTextBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panelLOUGA = new System.Windows.Forms.Panel();
            this.textBoxLOUGAObservationErrorWeight = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.checkBoxLOUGAParametersCanRepeat = new System.Windows.Forms.CheckBox();
            this.checkBoxLOUGAGenerateCompletePreconditionsLists = new System.Windows.Forms.CheckBox();
            this.checkBoxLOUGAGenerateNegativePreconditions = new System.Windows.Forms.CheckBox();
            this.textBoxLOUGAMaximalErrorRate = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.checkBoxLOUGAPredicateByPredicate = new System.Windows.Forms.CheckBox();
            this.checkBoxLOUGAEndAfterFirstSolution = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLOUGACrossWithOldPopTreshold = new System.Windows.Forms.TextBox();
            this.textBoxLOUGAOldPopRestartTreshold = new System.Windows.Forms.TextBox();
            this.textBoxLOUGAGreedySearchTreshold = new System.Windows.Forms.TextBox();
            this.textBoxLOUGAPopRestartTreshold = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLOUGAGenCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLOUGAPopSize = new System.Windows.Forms.TextBox();
            this.checkBoxLOUGAPDDLOutput = new System.Windows.Forms.CheckBox();
            this.checkBoxLOUGAGoalStateComplete = new System.Windows.Forms.CheckBox();
            this.panelARMS = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxARMSMaxSatRandomChoiceProbability = new System.Windows.Forms.TextBox();
            this.comboBoxARMSMaxSatAlgorithm = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxARMSMaxSatRestartTreshold = new System.Windows.Forms.TextBox();
            this.textBoxARMSMaxSatNrOfTries = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxARMSProbTreshold = new System.Windows.Forms.TextBox();
            this.textBoxARMSInfConstrWeight = new System.Windows.Forms.TextBox();
            this.textBoxARMSMaxAConstrWeight = new System.Windows.Forms.TextBox();
            this.textBoxARMSMinAConstrWeight = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxAlgorithmSelect = new System.Windows.Forms.ComboBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panelBestIndividuals = new System.Windows.Forms.Panel();
            this.listBoxIndividualsList = new System.Windows.Forms.ListBox();
            this.textBoxIndividualsExplorer = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.SolutionTextBox = new System.Windows.Forms.RichTextBox();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.buttonStartStop = new System.Windows.Forms.Button();
            this.hiddenLabelForFocusing = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonNew = new System.Windows.Forms.Button();
            this.labelErrors = new System.Windows.Forms.Label();
            this.comboBoxLOUGAPredicates = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panelLOUGA.SuspendLayout();
            this.panelARMS.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panelBestIndividuals.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLearn
            // 
            this.buttonLearn.Location = new System.Drawing.Point(188, 550);
            this.buttonLearn.Name = "buttonLearn";
            this.buttonLearn.Size = new System.Drawing.Size(260, 75);
            this.buttonLearn.TabIndex = 0;
            this.buttonLearn.Text = "Learn";
            this.buttonLearn.UseVisualStyleBackColor = true;
            this.buttonLearn.Click += new System.EventHandler(this.buttonLearn_Click);
            // 
            // InputTextBox
            // 
            this.InputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.InputTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.InputTextBox.Location = new System.Drawing.Point(12, 34);
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(618, 635);
            this.InputTextBox.TabIndex = 1;
            this.InputTextBox.Text = "";
            this.InputTextBox.WordWrap = false;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(74, 6);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(39, 23);
            this.buttonLoad.TabIndex = 3;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(637, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(615, 657);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panelLOUGA);
            this.tabPage1.Controls.Add(this.panelARMS);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.buttonLearn);
            this.tabPage1.Controls.Add(this.comboBoxAlgorithmSelect);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(607, 631);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Algorithm";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panelLOUGA
            // 
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAObservationErrorWeight);
            this.panelLOUGA.Controls.Add(this.label17);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAParametersCanRepeat);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAGenerateCompletePreconditionsLists);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAGenerateNegativePreconditions);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAMaximalErrorRate);
            this.panelLOUGA.Controls.Add(this.label16);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAPredicateByPredicate);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAEndAfterFirstSolution);
            this.panelLOUGA.Controls.Add(this.label7);
            this.panelLOUGA.Controls.Add(this.label6);
            this.panelLOUGA.Controls.Add(this.label5);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGACrossWithOldPopTreshold);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAOldPopRestartTreshold);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAGreedySearchTreshold);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAPopRestartTreshold);
            this.panelLOUGA.Controls.Add(this.label4);
            this.panelLOUGA.Controls.Add(this.label3);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAGenCount);
            this.panelLOUGA.Controls.Add(this.label2);
            this.panelLOUGA.Controls.Add(this.textBoxLOUGAPopSize);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAPDDLOutput);
            this.panelLOUGA.Controls.Add(this.checkBoxLOUGAGoalStateComplete);
            this.panelLOUGA.Location = new System.Drawing.Point(21, 86);
            this.panelLOUGA.Name = "panelLOUGA";
            this.panelLOUGA.Size = new System.Drawing.Size(561, 372);
            this.panelLOUGA.TabIndex = 4;
            // 
            // textBoxLOUGAObservationErrorWeight
            // 
            this.textBoxLOUGAObservationErrorWeight.Location = new System.Drawing.Point(214, 319);
            this.textBoxLOUGAObservationErrorWeight.Name = "textBoxLOUGAObservationErrorWeight";
            this.textBoxLOUGAObservationErrorWeight.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAObservationErrorWeight.TabIndex = 23;
            this.textBoxLOUGAObservationErrorWeight.Validated += new System.EventHandler(this.textBoxLOUGAObservationErrorWeight_Validated);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 322);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(159, 13);
            this.label17.TabIndex = 22;
            this.label17.Text = "Fitness observation error weight:";
            // 
            // checkBoxLOUGAParametersCanRepeat
            // 
            this.checkBoxLOUGAParametersCanRepeat.AutoSize = true;
            this.checkBoxLOUGAParametersCanRepeat.Location = new System.Drawing.Point(3, 95);
            this.checkBoxLOUGAParametersCanRepeat.Name = "checkBoxLOUGAParametersCanRepeat";
            this.checkBoxLOUGAParametersCanRepeat.Size = new System.Drawing.Size(196, 17);
            this.checkBoxLOUGAParametersCanRepeat.TabIndex = 21;
            this.checkBoxLOUGAParametersCanRepeat.Text = "Parameters can repeat in predicates";
            this.checkBoxLOUGAParametersCanRepeat.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAParametersCanRepeat.CheckedChanged += new System.EventHandler(this.checkBoxLOUGAParametersCanRepeat_CheckedChanged);
            // 
            // checkBoxLOUGAGenerateCompletePreconditionsLists
            // 
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.AutoSize = true;
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.Location = new System.Drawing.Point(3, 49);
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.Name = "checkBoxLOUGAGenerateCompletePreconditionsLists";
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.Size = new System.Drawing.Size(202, 17);
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.TabIndex = 20;
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.Text = "Generate complete preconditions lists";
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAGenerateCompletePreconditionsLists.CheckedChanged += new System.EventHandler(this.checkBoxLOUGAGenerateCompletePreconditionsLists_CheckedChanged);
            // 
            // checkBoxLOUGAGenerateNegativePreconditions
            // 
            this.checkBoxLOUGAGenerateNegativePreconditions.AutoSize = true;
            this.checkBoxLOUGAGenerateNegativePreconditions.Location = new System.Drawing.Point(3, 72);
            this.checkBoxLOUGAGenerateNegativePreconditions.Name = "checkBoxLOUGAGenerateNegativePreconditions";
            this.checkBoxLOUGAGenerateNegativePreconditions.Size = new System.Drawing.Size(180, 17);
            this.checkBoxLOUGAGenerateNegativePreconditions.TabIndex = 19;
            this.checkBoxLOUGAGenerateNegativePreconditions.Text = "Generate negative preconditions";
            this.checkBoxLOUGAGenerateNegativePreconditions.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAGenerateNegativePreconditions.CheckedChanged += new System.EventHandler(this.checkBoxLOUGAGenerateNegativePreconditions_CheckedChanged);
            // 
            // textBoxLOUGAMaximalErrorRate
            // 
            this.textBoxLOUGAMaximalErrorRate.Location = new System.Drawing.Point(214, 345);
            this.textBoxLOUGAMaximalErrorRate.Name = "textBoxLOUGAMaximalErrorRate";
            this.textBoxLOUGAMaximalErrorRate.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAMaximalErrorRate.TabIndex = 18;
            this.textBoxLOUGAMaximalErrorRate.Validated += new System.EventHandler(this.textBoxLOUGAMaximalErrorRate_Validated);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 348);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(160, 13);
            this.label16.TabIndex = 17;
            this.label16.Text = "Maximal error rate for predicates:";
            // 
            // checkBoxLOUGAPredicateByPredicate
            // 
            this.checkBoxLOUGAPredicateByPredicate.AutoSize = true;
            this.checkBoxLOUGAPredicateByPredicate.Location = new System.Drawing.Point(3, 26);
            this.checkBoxLOUGAPredicateByPredicate.Name = "checkBoxLOUGAPredicateByPredicate";
            this.checkBoxLOUGAPredicateByPredicate.Size = new System.Drawing.Size(193, 17);
            this.checkBoxLOUGAPredicateByPredicate.TabIndex = 16;
            this.checkBoxLOUGAPredicateByPredicate.Text = "Find solution predicate by predicate";
            this.checkBoxLOUGAPredicateByPredicate.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAPredicateByPredicate.CheckedChanged += new System.EventHandler(this.checkBoxLOUGAPredicateByPredicate_CheckedChanged);
            // 
            // checkBoxLOUGAEndAfterFirstSolution
            // 
            this.checkBoxLOUGAEndAfterFirstSolution.AutoSize = true;
            this.checkBoxLOUGAEndAfterFirstSolution.Location = new System.Drawing.Point(3, 118);
            this.checkBoxLOUGAEndAfterFirstSolution.Name = "checkBoxLOUGAEndAfterFirstSolution";
            this.checkBoxLOUGAEndAfterFirstSolution.Size = new System.Drawing.Size(148, 17);
            this.checkBoxLOUGAEndAfterFirstSolution.TabIndex = 15;
            this.checkBoxLOUGAEndAfterFirstSolution.Text = "End after solution is found";
            this.checkBoxLOUGAEndAfterFirstSolution.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAEndAfterFirstSolution.CheckedChanged += new System.EventHandler(this.checkBoxLOUGAEndAfterFirstSolution_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 296);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(206, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Treshold for crossover with old population:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 270);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Treshold for local search:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 244);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(167, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Treshold for old population restart:";
            // 
            // textBoxLOUGACrossWithOldPopTreshold
            // 
            this.textBoxLOUGACrossWithOldPopTreshold.Location = new System.Drawing.Point(214, 293);
            this.textBoxLOUGACrossWithOldPopTreshold.Name = "textBoxLOUGACrossWithOldPopTreshold";
            this.textBoxLOUGACrossWithOldPopTreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGACrossWithOldPopTreshold.TabIndex = 11;
            this.textBoxLOUGACrossWithOldPopTreshold.Validated += new System.EventHandler(this.textBoxLOUGACrossWithOldPopTreshold_Validated);
            // 
            // textBoxLOUGAOldPopRestartTreshold
            // 
            this.textBoxLOUGAOldPopRestartTreshold.Location = new System.Drawing.Point(214, 241);
            this.textBoxLOUGAOldPopRestartTreshold.Name = "textBoxLOUGAOldPopRestartTreshold";
            this.textBoxLOUGAOldPopRestartTreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAOldPopRestartTreshold.TabIndex = 10;
            this.textBoxLOUGAOldPopRestartTreshold.Validated += new System.EventHandler(this.textBoxLOUGAOldPopRestartTreshold_Validated);
            // 
            // textBoxLOUGAGreedySearchTreshold
            // 
            this.textBoxLOUGAGreedySearchTreshold.Location = new System.Drawing.Point(214, 267);
            this.textBoxLOUGAGreedySearchTreshold.Name = "textBoxLOUGAGreedySearchTreshold";
            this.textBoxLOUGAGreedySearchTreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAGreedySearchTreshold.TabIndex = 9;
            this.textBoxLOUGAGreedySearchTreshold.Validated += new System.EventHandler(this.textBoxLOUGAGreedySearchTreshold_Validated);
            // 
            // textBoxLOUGAPopRestartTreshold
            // 
            this.textBoxLOUGAPopRestartTreshold.Location = new System.Drawing.Point(214, 215);
            this.textBoxLOUGAPopRestartTreshold.Name = "textBoxLOUGAPopRestartTreshold";
            this.textBoxLOUGAPopRestartTreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAPopRestartTreshold.TabIndex = 8;
            this.textBoxLOUGAPopRestartTreshold.Validated += new System.EventHandler(this.textBoxLOUGAPopRestartTreshold_Validated);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Treshold for population restart:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Nr. of generations";
            // 
            // textBoxLOUGAGenCount
            // 
            this.textBoxLOUGAGenCount.Location = new System.Drawing.Point(214, 189);
            this.textBoxLOUGAGenCount.Name = "textBoxLOUGAGenCount";
            this.textBoxLOUGAGenCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAGenCount.TabIndex = 5;
            this.textBoxLOUGAGenCount.Validated += new System.EventHandler(this.textBoxLOUGAGenCount_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Population size:";
            // 
            // textBoxLOUGAPopSize
            // 
            this.textBoxLOUGAPopSize.Location = new System.Drawing.Point(214, 163);
            this.textBoxLOUGAPopSize.Name = "textBoxLOUGAPopSize";
            this.textBoxLOUGAPopSize.Size = new System.Drawing.Size(100, 20);
            this.textBoxLOUGAPopSize.TabIndex = 3;
            this.textBoxLOUGAPopSize.Validated += new System.EventHandler(this.textBoxLOUGAPopSize_Validated);
            // 
            // checkBoxLOUGAPDDLOutput
            // 
            this.checkBoxLOUGAPDDLOutput.AutoSize = true;
            this.checkBoxLOUGAPDDLOutput.Location = new System.Drawing.Point(3, 141);
            this.checkBoxLOUGAPDDLOutput.Name = "checkBoxLOUGAPDDLOutput";
            this.checkBoxLOUGAPDDLOutput.Size = new System.Drawing.Size(133, 17);
            this.checkBoxLOUGAPDDLOutput.TabIndex = 2;
            this.checkBoxLOUGAPDDLOutput.Text = "Output in PDDL format";
            this.checkBoxLOUGAPDDLOutput.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAPDDLOutput.CheckedChanged += new System.EventHandler(this.checkBoxPDDLOutput_CheckedChanged);
            // 
            // checkBoxLOUGAGoalStateComplete
            // 
            this.checkBoxLOUGAGoalStateComplete.AutoSize = true;
            this.checkBoxLOUGAGoalStateComplete.Location = new System.Drawing.Point(3, 3);
            this.checkBoxLOUGAGoalStateComplete.Name = "checkBoxLOUGAGoalStateComplete";
            this.checkBoxLOUGAGoalStateComplete.Size = new System.Drawing.Size(143, 17);
            this.checkBoxLOUGAGoalStateComplete.TabIndex = 1;
            this.checkBoxLOUGAGoalStateComplete.Text = "Goal states are complete";
            this.checkBoxLOUGAGoalStateComplete.UseVisualStyleBackColor = true;
            this.checkBoxLOUGAGoalStateComplete.CheckedChanged += new System.EventHandler(this.checkBoxGoalStateComplete_CheckedChanged);
            // 
            // panelARMS
            // 
            this.panelARMS.Controls.Add(this.label15);
            this.panelARMS.Controls.Add(this.textBoxARMSMaxSatRandomChoiceProbability);
            this.panelARMS.Controls.Add(this.comboBoxARMSMaxSatAlgorithm);
            this.panelARMS.Controls.Add(this.label14);
            this.panelARMS.Controls.Add(this.textBoxARMSMaxSatRestartTreshold);
            this.panelARMS.Controls.Add(this.textBoxARMSMaxSatNrOfTries);
            this.panelARMS.Controls.Add(this.label13);
            this.panelARMS.Controls.Add(this.label12);
            this.panelARMS.Controls.Add(this.textBoxARMSProbTreshold);
            this.panelARMS.Controls.Add(this.textBoxARMSInfConstrWeight);
            this.panelARMS.Controls.Add(this.textBoxARMSMaxAConstrWeight);
            this.panelARMS.Controls.Add(this.textBoxARMSMinAConstrWeight);
            this.panelARMS.Controls.Add(this.label11);
            this.panelARMS.Controls.Add(this.label10);
            this.panelARMS.Controls.Add(this.label9);
            this.panelARMS.Controls.Add(this.label8);
            this.panelARMS.Location = new System.Drawing.Point(21, 86);
            this.panelARMS.Name = "panelARMS";
            this.panelARMS.Size = new System.Drawing.Size(561, 372);
            this.panelARMS.TabIndex = 15;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 193);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(177, 13);
            this.label15.TabIndex = 18;
            this.label15.Text = "MAX-SAT random choice probability";
            // 
            // textBoxARMSMaxSatRandomChoiceProbability
            // 
            this.textBoxARMSMaxSatRandomChoiceProbability.Location = new System.Drawing.Point(196, 190);
            this.textBoxARMSMaxSatRandomChoiceProbability.Name = "textBoxARMSMaxSatRandomChoiceProbability";
            this.textBoxARMSMaxSatRandomChoiceProbability.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSMaxSatRandomChoiceProbability.TabIndex = 17;
            this.textBoxARMSMaxSatRandomChoiceProbability.Validated += new System.EventHandler(this.textBoxARMSMaxSatRandomChoiceProbability_Validated);
            // 
            // comboBoxARMSMaxSatAlgorithm
            // 
            this.comboBoxARMSMaxSatAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxARMSMaxSatAlgorithm.FormattingEnabled = true;
            this.comboBoxARMSMaxSatAlgorithm.Items.AddRange(new object[] {
            "GSAT",
            "WalkSat"});
            this.comboBoxARMSMaxSatAlgorithm.Location = new System.Drawing.Point(196, 111);
            this.comboBoxARMSMaxSatAlgorithm.Name = "comboBoxARMSMaxSatAlgorithm";
            this.comboBoxARMSMaxSatAlgorithm.Size = new System.Drawing.Size(100, 21);
            this.comboBoxARMSMaxSatAlgorithm.Sorted = true;
            this.comboBoxARMSMaxSatAlgorithm.TabIndex = 16;
            this.comboBoxARMSMaxSatAlgorithm.TabStop = false;
            this.comboBoxARMSMaxSatAlgorithm.SelectedIndexChanged += new System.EventHandler(this.comboBoxARMSMaxSatAlgorithm_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 167);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(126, 13);
            this.label14.TabIndex = 16;
            this.label14.Text = "MAX-SAT restart treshold";
            // 
            // textBoxARMSMaxSatRestartTreshold
            // 
            this.textBoxARMSMaxSatRestartTreshold.Location = new System.Drawing.Point(196, 164);
            this.textBoxARMSMaxSatRestartTreshold.Name = "textBoxARMSMaxSatRestartTreshold";
            this.textBoxARMSMaxSatRestartTreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSMaxSatRestartTreshold.TabIndex = 15;
            this.textBoxARMSMaxSatRestartTreshold.Validated += new System.EventHandler(this.textBoxARMSMaxSatRestartTreshold_Validated);
            // 
            // textBoxARMSMaxSatNrOfTries
            // 
            this.textBoxARMSMaxSatNrOfTries.Location = new System.Drawing.Point(196, 138);
            this.textBoxARMSMaxSatNrOfTries.Name = "textBoxARMSMaxSatNrOfTries";
            this.textBoxARMSMaxSatNrOfTries.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSMaxSatNrOfTries.TabIndex = 14;
            this.textBoxARMSMaxSatNrOfTries.Validated += new System.EventHandler(this.textBoxARMSMaxSatNrOfTries_Validated);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 141);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(126, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "MAX-SAT number of tries";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(8, 115);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(99, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "MAX-SAT algorithm";
            // 
            // textBoxARMSProbTreshold
            // 
            this.textBoxARMSProbTreshold.Location = new System.Drawing.Point(196, 86);
            this.textBoxARMSProbTreshold.Name = "textBoxARMSProbTreshold";
            this.textBoxARMSProbTreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSProbTreshold.TabIndex = 11;
            this.textBoxARMSProbTreshold.Validated += new System.EventHandler(this.textBoxARMSProbTreshold_Validated);
            // 
            // textBoxARMSInfConstrWeight
            // 
            this.textBoxARMSInfConstrWeight.Location = new System.Drawing.Point(196, 60);
            this.textBoxARMSInfConstrWeight.Name = "textBoxARMSInfConstrWeight";
            this.textBoxARMSInfConstrWeight.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSInfConstrWeight.TabIndex = 10;
            this.textBoxARMSInfConstrWeight.Validated += new System.EventHandler(this.textBoxARMSInfConstrWeight_Validated);
            // 
            // textBoxARMSMaxAConstrWeight
            // 
            this.textBoxARMSMaxAConstrWeight.Location = new System.Drawing.Point(196, 34);
            this.textBoxARMSMaxAConstrWeight.Name = "textBoxARMSMaxAConstrWeight";
            this.textBoxARMSMaxAConstrWeight.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSMaxAConstrWeight.TabIndex = 9;
            this.textBoxARMSMaxAConstrWeight.Validated += new System.EventHandler(this.textBoxARMSMaxAConstrWeight_Validated);
            // 
            // textBoxARMSMinAConstrWeight
            // 
            this.textBoxARMSMinAConstrWeight.Location = new System.Drawing.Point(196, 7);
            this.textBoxARMSMinAConstrWeight.Name = "textBoxARMSMinAConstrWeight";
            this.textBoxARMSMinAConstrWeight.Size = new System.Drawing.Size(100, 20);
            this.textBoxARMSMinAConstrWeight.TabIndex = 8;
            this.textBoxARMSMinAConstrWeight.Validated += new System.EventHandler(this.textBoxARMSMinAConstrWeight_Validated);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 63);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(145, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Information constraint weight:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(127, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Minimum operator weight:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 89);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Probability treshold:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Maximum operator weight:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Algorithm:";
            // 
            // comboBoxAlgorithmSelect
            // 
            this.comboBoxAlgorithmSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAlgorithmSelect.FormattingEnabled = true;
            this.comboBoxAlgorithmSelect.Items.AddRange(new object[] {
            "ARMS",
            "LOCM",
            "LOCM2",
            "LOUGA"});
            this.comboBoxAlgorithmSelect.Location = new System.Drawing.Point(77, 41);
            this.comboBoxAlgorithmSelect.Name = "comboBoxAlgorithmSelect";
            this.comboBoxAlgorithmSelect.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAlgorithmSelect.Sorted = true;
            this.comboBoxAlgorithmSelect.TabIndex = 0;
            this.comboBoxAlgorithmSelect.TabStop = false;
            this.comboBoxAlgorithmSelect.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panelBestIndividuals);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(607, 631);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Output";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panelBestIndividuals
            // 
            this.panelBestIndividuals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBestIndividuals.Controls.Add(this.listBoxIndividualsList);
            this.panelBestIndividuals.Controls.Add(this.textBoxIndividualsExplorer);
            this.panelBestIndividuals.Location = new System.Drawing.Point(3, 3);
            this.panelBestIndividuals.Name = "panelBestIndividuals";
            this.panelBestIndividuals.Size = new System.Drawing.Size(604, 625);
            this.panelBestIndividuals.TabIndex = 5;
            // 
            // listBoxIndividualsList
            // 
            this.listBoxIndividualsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxIndividualsList.FormattingEnabled = true;
            this.listBoxIndividualsList.Location = new System.Drawing.Point(3, 3);
            this.listBoxIndividualsList.Name = "listBoxIndividualsList";
            this.listBoxIndividualsList.Size = new System.Drawing.Size(139, 615);
            this.listBoxIndividualsList.TabIndex = 6;
            // 
            // textBoxIndividualsExplorer
            // 
            this.textBoxIndividualsExplorer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxIndividualsExplorer.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.textBoxIndividualsExplorer.Location = new System.Drawing.Point(148, 3);
            this.textBoxIndividualsExplorer.Name = "textBoxIndividualsExplorer";
            this.textBoxIndividualsExplorer.ReadOnly = true;
            this.textBoxIndividualsExplorer.Size = new System.Drawing.Size(450, 619);
            this.textBoxIndividualsExplorer.TabIndex = 5;
            this.textBoxIndividualsExplorer.Text = "";
            this.textBoxIndividualsExplorer.WordWrap = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.SolutionTextBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(607, 631);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Solution";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // SolutionTextBox
            // 
            this.SolutionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SolutionTextBox.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.SolutionTextBox.Location = new System.Drawing.Point(3, 3);
            this.SolutionTextBox.Name = "SolutionTextBox";
            this.SolutionTextBox.Size = new System.Drawing.Size(601, 628);
            this.SolutionTextBox.TabIndex = 0;
            this.SolutionTextBox.Text = "";
            this.SolutionTextBox.WordWrap = false;
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 10;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Location = new System.Drawing.Point(571, 6);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(59, 22);
            this.buttonStartStop.TabIndex = 5;
            this.buttonStartStop.Text = "Start";
            this.buttonStartStop.UseVisualStyleBackColor = true;
            this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
            // 
            // hiddenLabelForFocusing
            // 
            this.hiddenLabelForFocusing.AutoSize = true;
            this.hiddenLabelForFocusing.Location = new System.Drawing.Point(224, 5);
            this.hiddenLabelForFocusing.Name = "hiddenLabelForFocusing";
            this.hiddenLabelForFocusing.Size = new System.Drawing.Size(0, 13);
            this.hiddenLabelForFocusing.TabIndex = 7;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(119, 6);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(42, 23);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonNew
            // 
            this.buttonNew.Location = new System.Drawing.Point(12, 6);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(56, 23);
            this.buttonNew.TabIndex = 9;
            this.buttonNew.Text = "New file";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // labelErrors
            // 
            this.labelErrors.AutoSize = true;
            this.labelErrors.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelErrors.Location = new System.Drawing.Point(499, 11);
            this.labelErrors.Name = "labelErrors";
            this.labelErrors.Size = new System.Drawing.Size(0, 13);
            this.labelErrors.TabIndex = 10;
            this.labelErrors.Click += new System.EventHandler(this.labelErrors_Click);
            // 
            // comboBoxLOUGAPredicates
            // 
            this.comboBoxLOUGAPredicates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLOUGAPredicates.FormattingEnabled = true;
            this.comboBoxLOUGAPredicates.Items.AddRange(new object[] {
            "ARMS",
            "LOCM",
            "LOCM2",
            "LOUGA"});
            this.comboBoxLOUGAPredicates.Location = new System.Drawing.Point(1100, 7);
            this.comboBoxLOUGAPredicates.Name = "comboBoxLOUGAPredicates";
            this.comboBoxLOUGAPredicates.Size = new System.Drawing.Size(145, 21);
            this.comboBoxLOUGAPredicates.Sorted = true;
            this.comboBoxLOUGAPredicates.TabIndex = 16;
            this.comboBoxLOUGAPredicates.TabStop = false;
            this.comboBoxLOUGAPredicates.SelectedIndexChanged += new System.EventHandler(this.comboBoxLOUGAPredicates_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(325, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(59, 22);
            this.button1.TabIndex = 17;
            this.button1.Text = "ARMS";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(390, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(59, 22);
            this.button2.TabIndex = 18;
            this.button2.Text = "LOUGA";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(455, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(59, 22);
            this.button3.TabIndex = 19;
            this.button3.Text = "LOUGAstd";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxLOUGAPredicates);
            this.Controls.Add(this.labelErrors);
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.hiddenLabelForFocusing);
            this.Controls.Add(this.buttonStartStop);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.InputTextBox);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1220, 600);
            this.Name = "Form1";
            this.Text = "Model Learner";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panelLOUGA.ResumeLayout(false);
            this.panelLOUGA.PerformLayout();
            this.panelARMS.ResumeLayout(false);
            this.panelARMS.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panelBestIndividuals.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLearn;
        private System.Windows.Forms.RichTextBox InputTextBox;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox SolutionTextBox;
        private System.Windows.Forms.ComboBox comboBoxAlgorithmSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelLOUGA;
        private System.Windows.Forms.CheckBox checkBoxLOUGAGoalStateComplete;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.CheckBox checkBoxLOUGAPDDLOutput;
        private System.Windows.Forms.Panel panelBestIndividuals;
        private System.Windows.Forms.ListBox listBoxIndividualsList;
        private System.Windows.Forms.RichTextBox textBoxIndividualsExplorer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLOUGAPopSize;
        private System.Windows.Forms.TextBox textBoxLOUGAPopRestartTreshold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLOUGAGenCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLOUGACrossWithOldPopTreshold;
        private System.Windows.Forms.TextBox textBoxLOUGAOldPopRestartTreshold;
        private System.Windows.Forms.TextBox textBoxLOUGAGreedySearchTreshold;
        private System.Windows.Forms.Panel panelARMS;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxARMSProbTreshold;
        private System.Windows.Forms.TextBox textBoxARMSInfConstrWeight;
        private System.Windows.Forms.TextBox textBoxARMSMaxAConstrWeight;
        private System.Windows.Forms.TextBox textBoxARMSMinAConstrWeight;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonStartStop;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxARMSMaxSatRestartTreshold;
        private System.Windows.Forms.TextBox textBoxARMSMaxSatNrOfTries;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBoxARMSMaxSatAlgorithm;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxARMSMaxSatRandomChoiceProbability;
        private System.Windows.Forms.CheckBox checkBoxLOUGAEndAfterFirstSolution;
        private System.Windows.Forms.Label hiddenLabelForFocusing;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonNew;
        private System.Windows.Forms.Label labelErrors;
        private System.Windows.Forms.CheckBox checkBoxLOUGAPredicateByPredicate;
        private System.Windows.Forms.TextBox textBoxLOUGAMaximalErrorRate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox checkBoxLOUGAGenerateCompletePreconditionsLists;
        private System.Windows.Forms.CheckBox checkBoxLOUGAGenerateNegativePreconditions;
        private System.Windows.Forms.CheckBox checkBoxLOUGAParametersCanRepeat;
        private System.Windows.Forms.TextBox textBoxLOUGAObservationErrorWeight;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ComboBox comboBoxLOUGAPredicates;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

