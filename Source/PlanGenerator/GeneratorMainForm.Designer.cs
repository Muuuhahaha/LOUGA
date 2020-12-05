namespace PlanGenerator
{
    partial class panel
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
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.buttonSetFile = new System.Windows.Forms.Button();
            this.checkBoxRemoveOperatorsLists = new System.Windows.Forms.CheckBox();
            this.textBoxInputFile = new System.Windows.Forms.TextBox();
            this.textBoxOutputFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPredicateChance = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxGeneratingMethod = new System.Windows.Forms.ComboBox();
            this.comboBoxGoalState = new System.Windows.Forms.ComboBox();
            this.comboBoxInitState = new System.Windows.Forms.ComboBox();
            this.comboBoxTransitionStates = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxPlansCount = new System.Windows.Forms.TextBox();
            this.panelSequences = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSequencesDesiredActionsCount = new System.Windows.Forms.TextBox();
            this.textBoxSequencesMinActions = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBoxRemoveTypeOptions = new System.Windows.Forms.CheckBox();
            this.panelBFS = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxBFSMinimalPlanLength = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxBFSMaxVisitedStates = new System.Windows.Forms.TextBox();
            this.textBoxBFSGoalPredicates = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.panelBFSWide = new System.Windows.Forms.Panel();
            this.textBoxBFSWideMinimalPlanLength = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxBFSVisitedStates = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBoxRemoveTyping = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.labelVisited = new System.Windows.Forms.Label();
            this.labelFound = new System.Windows.Forms.Label();
            this.labelDepth = new System.Windows.Forms.Label();
            this.panelBFSProgress = new System.Windows.Forms.Panel();
            this.labelCurrentDepth = new System.Windows.Forms.Label();
            this.labelStatesCount = new System.Windows.Forms.Label();
            this.labelStatesVisited = new System.Windows.Forms.Label();
            this.labelPlansGenerated = new System.Windows.Forms.Label();
            this.labelCurrentInitStateId = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxParametersCanRepeatInActions = new System.Windows.Forms.CheckBox();
            this.panelSequences.SuspendLayout();
            this.panelBFS.SuspendLayout();
            this.panelBFSWide.SuspendLayout();
            this.panelBFSProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Location = new System.Drawing.Point(249, 409);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(160, 54);
            this.buttonGenerate.TabIndex = 0;
            this.buttonGenerate.Text = "generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonSetFile
            // 
            this.buttonSetFile.Location = new System.Drawing.Point(12, 12);
            this.buttonSetFile.Name = "buttonSetFile";
            this.buttonSetFile.Size = new System.Drawing.Size(124, 22);
            this.buttonSetFile.TabIndex = 1;
            this.buttonSetFile.Text = "Set input file";
            this.buttonSetFile.UseVisualStyleBackColor = true;
            this.buttonSetFile.Click += new System.EventHandler(this.buttonSetFile_Click);
            // 
            // checkBoxRemoveOperatorsLists
            // 
            this.checkBoxRemoveOperatorsLists.AutoSize = true;
            this.checkBoxRemoveOperatorsLists.Location = new System.Drawing.Point(12, 302);
            this.checkBoxRemoveOperatorsLists.Name = "checkBoxRemoveOperatorsLists";
            this.checkBoxRemoveOperatorsLists.Size = new System.Drawing.Size(135, 17);
            this.checkBoxRemoveOperatorsLists.TabIndex = 3;
            this.checkBoxRemoveOperatorsLists.Text = "Remove operators\' lists";
            this.checkBoxRemoveOperatorsLists.UseVisualStyleBackColor = true;
            this.checkBoxRemoveOperatorsLists.CheckedChanged += new System.EventHandler(this.checkBoxRemoveOperatorsLists_CheckedChanged);
            // 
            // textBoxInputFile
            // 
            this.textBoxInputFile.Location = new System.Drawing.Point(143, 13);
            this.textBoxInputFile.Name = "textBoxInputFile";
            this.textBoxInputFile.ReadOnly = true;
            this.textBoxInputFile.Size = new System.Drawing.Size(225, 20);
            this.textBoxInputFile.TabIndex = 5;
            this.textBoxInputFile.TextChanged += new System.EventHandler(this.textBoxInputFile_TextChanged);
            // 
            // textBoxOutputFile
            // 
            this.textBoxOutputFile.Location = new System.Drawing.Point(143, 39);
            this.textBoxOutputFile.Name = "textBoxOutputFile";
            this.textBoxOutputFile.Size = new System.Drawing.Size(225, 20);
            this.textBoxOutputFile.TabIndex = 6;
            this.textBoxOutputFile.Validated += new System.EventHandler(this.textBoxOutputFile_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Output file:";
            // 
            // textBoxPredicateChance
            // 
            this.textBoxPredicateChance.Location = new System.Drawing.Point(143, 406);
            this.textBoxPredicateChance.Name = "textBoxPredicateChance";
            this.textBoxPredicateChance.Size = new System.Drawing.Size(100, 20);
            this.textBoxPredicateChance.TabIndex = 11;
            this.textBoxPredicateChance.Validated += new System.EventHandler(this.textBoxPredicateChance_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 328);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Init state output:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 382);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Goal state output:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 355);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Trasition states output:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 409);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Predicate output chance:";
            // 
            // comboBoxGeneratingMethod
            // 
            this.comboBoxGeneratingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGeneratingMethod.FormattingEnabled = true;
            this.comboBoxGeneratingMethod.Items.AddRange(new object[] {
            "Random actions",
            "Generate goals and BFS plans",
            "BFS and choose random goals"});
            this.comboBoxGeneratingMethod.Location = new System.Drawing.Point(143, 89);
            this.comboBoxGeneratingMethod.Name = "comboBoxGeneratingMethod";
            this.comboBoxGeneratingMethod.Size = new System.Drawing.Size(189, 21);
            this.comboBoxGeneratingMethod.TabIndex = 17;
            this.comboBoxGeneratingMethod.SelectedIndexChanged += new System.EventHandler(this.comboBoxGeneratingMethod_SelectedIndexChanged);
            // 
            // comboBoxGoalState
            // 
            this.comboBoxGoalState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGoalState.FormattingEnabled = true;
            this.comboBoxGoalState.Items.AddRange(new object[] {
            "Whole state",
            "Some predicates",
            "Goal predicates",
            "Don\'t output goal state"});
            this.comboBoxGoalState.Location = new System.Drawing.Point(143, 379);
            this.comboBoxGoalState.Name = "comboBoxGoalState";
            this.comboBoxGoalState.Size = new System.Drawing.Size(189, 21);
            this.comboBoxGoalState.TabIndex = 18;
            this.comboBoxGoalState.SelectedIndexChanged += new System.EventHandler(this.comboBoxGoalState_SelectedIndexChanged);
            // 
            // comboBoxInitState
            // 
            this.comboBoxInitState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxInitState.FormattingEnabled = true;
            this.comboBoxInitState.Items.AddRange(new object[] {
            "Whole state",
            "Some predicates",
            "Don\'t output init state"});
            this.comboBoxInitState.Location = new System.Drawing.Point(143, 325);
            this.comboBoxInitState.Name = "comboBoxInitState";
            this.comboBoxInitState.Size = new System.Drawing.Size(189, 21);
            this.comboBoxInitState.TabIndex = 19;
            this.comboBoxInitState.SelectedIndexChanged += new System.EventHandler(this.comboBoxInitState_SelectedIndexChanged);
            // 
            // comboBoxTransitionStates
            // 
            this.comboBoxTransitionStates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTransitionStates.FormattingEnabled = true;
            this.comboBoxTransitionStates.Items.AddRange(new object[] {
            "Whole state",
            "Some predicates",
            "Don\'t output transition states"});
            this.comboBoxTransitionStates.Location = new System.Drawing.Point(143, 352);
            this.comboBoxTransitionStates.Name = "comboBoxTransitionStates";
            this.comboBoxTransitionStates.Size = new System.Drawing.Size(189, 21);
            this.comboBoxTransitionStates.TabIndex = 20;
            this.comboBoxTransitionStates.SelectedIndexChanged += new System.EventHandler(this.comboBoxTransitionStates_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "Generating method:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Plans count:";
            // 
            // textBoxPlansCount
            // 
            this.textBoxPlansCount.Location = new System.Drawing.Point(143, 116);
            this.textBoxPlansCount.Name = "textBoxPlansCount";
            this.textBoxPlansCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxPlansCount.TabIndex = 23;
            this.textBoxPlansCount.Validated += new System.EventHandler(this.textBoxPlansCount_Validated);
            // 
            // panelSequences
            // 
            this.panelSequences.Controls.Add(this.label9);
            this.panelSequences.Controls.Add(this.textBoxSequencesDesiredActionsCount);
            this.panelSequences.Controls.Add(this.textBoxSequencesMinActions);
            this.panelSequences.Controls.Add(this.label8);
            this.panelSequences.Location = new System.Drawing.Point(12, 137);
            this.panelSequences.Name = "panelSequences";
            this.panelSequences.Size = new System.Drawing.Size(356, 85);
            this.panelSequences.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(-3, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Desired actions count:";
            // 
            // textBoxSequencesDesiredActionsCount
            // 
            this.textBoxSequencesDesiredActionsCount.Location = new System.Drawing.Point(131, 33);
            this.textBoxSequencesDesiredActionsCount.Name = "textBoxSequencesDesiredActionsCount";
            this.textBoxSequencesDesiredActionsCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxSequencesDesiredActionsCount.TabIndex = 26;
            this.textBoxSequencesDesiredActionsCount.Validated += new System.EventHandler(this.textBoxSequencesDesiredActionsCount_Validated);
            // 
            // textBoxSequencesMinActions
            // 
            this.textBoxSequencesMinActions.Location = new System.Drawing.Point(131, 7);
            this.textBoxSequencesMinActions.Name = "textBoxSequencesMinActions";
            this.textBoxSequencesMinActions.Size = new System.Drawing.Size(100, 20);
            this.textBoxSequencesMinActions.TabIndex = 25;
            this.textBoxSequencesMinActions.Validated += new System.EventHandler(this.textBoxSequencesMinActions_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(-3, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "Minimum actions count:";
            // 
            // checkBoxRemoveTypeOptions
            // 
            this.checkBoxRemoveTypeOptions.AutoSize = true;
            this.checkBoxRemoveTypeOptions.Location = new System.Drawing.Point(12, 256);
            this.checkBoxRemoveTypeOptions.Name = "checkBoxRemoveTypeOptions";
            this.checkBoxRemoveTypeOptions.Size = new System.Drawing.Size(232, 17);
            this.checkBoxRemoveTypeOptions.TabIndex = 30;
            this.checkBoxRemoveTypeOptions.Text = "Remove operators\' parameters\' type options";
            this.checkBoxRemoveTypeOptions.UseVisualStyleBackColor = true;
            this.checkBoxRemoveTypeOptions.CheckedChanged += new System.EventHandler(this.checkBoxRemoveTypeOptions_CheckedChanged);
            // 
            // panelBFS
            // 
            this.panelBFS.Controls.Add(this.label12);
            this.panelBFS.Controls.Add(this.textBoxBFSMinimalPlanLength);
            this.panelBFS.Controls.Add(this.label10);
            this.panelBFS.Controls.Add(this.textBoxBFSMaxVisitedStates);
            this.panelBFS.Controls.Add(this.textBoxBFSGoalPredicates);
            this.panelBFS.Controls.Add(this.label11);
            this.panelBFS.Location = new System.Drawing.Point(386, 230);
            this.panelBFS.Name = "panelBFS";
            this.panelBFS.Size = new System.Drawing.Size(356, 85);
            this.panelBFS.TabIndex = 28;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(-3, 62);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(100, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "Minimal plan length:";
            // 
            // textBoxBFSMinimalPlanLength
            // 
            this.textBoxBFSMinimalPlanLength.Location = new System.Drawing.Point(131, 59);
            this.textBoxBFSMinimalPlanLength.Name = "textBoxBFSMinimalPlanLength";
            this.textBoxBFSMinimalPlanLength.Size = new System.Drawing.Size(100, 20);
            this.textBoxBFSMinimalPlanLength.TabIndex = 28;
            this.textBoxBFSMinimalPlanLength.Validated += new System.EventHandler(this.textBoxBFSMinimalPlanLength_Validated);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(-3, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Max visited states:";
            // 
            // textBoxBFSMaxVisitedStates
            // 
            this.textBoxBFSMaxVisitedStates.Location = new System.Drawing.Point(131, 33);
            this.textBoxBFSMaxVisitedStates.Name = "textBoxBFSMaxVisitedStates";
            this.textBoxBFSMaxVisitedStates.Size = new System.Drawing.Size(100, 20);
            this.textBoxBFSMaxVisitedStates.TabIndex = 26;
            this.textBoxBFSMaxVisitedStates.Validated += new System.EventHandler(this.textBoxBFSMaxVisitedStates_Validated);
            // 
            // textBoxBFSGoalPredicates
            // 
            this.textBoxBFSGoalPredicates.Location = new System.Drawing.Point(131, 7);
            this.textBoxBFSGoalPredicates.Name = "textBoxBFSGoalPredicates";
            this.textBoxBFSGoalPredicates.Size = new System.Drawing.Size(100, 20);
            this.textBoxBFSGoalPredicates.TabIndex = 25;
            this.textBoxBFSGoalPredicates.Validated += new System.EventHandler(this.textBoxBFSGoalPredicates_Validated);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(-3, 10);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(134, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "Number of goal predicates:";
            // 
            // panelBFSWide
            // 
            this.panelBFSWide.Controls.Add(this.textBoxBFSWideMinimalPlanLength);
            this.panelBFSWide.Controls.Add(this.label15);
            this.panelBFSWide.Controls.Add(this.textBoxBFSVisitedStates);
            this.panelBFSWide.Controls.Add(this.label13);
            this.panelBFSWide.Location = new System.Drawing.Point(452, 381);
            this.panelBFSWide.Name = "panelBFSWide";
            this.panelBFSWide.Size = new System.Drawing.Size(356, 85);
            this.panelBFSWide.TabIndex = 28;
            // 
            // textBoxBFSWideMinimalPlanLength
            // 
            this.textBoxBFSWideMinimalPlanLength.Location = new System.Drawing.Point(131, 33);
            this.textBoxBFSWideMinimalPlanLength.Name = "textBoxBFSWideMinimalPlanLength";
            this.textBoxBFSWideMinimalPlanLength.Size = new System.Drawing.Size(100, 20);
            this.textBoxBFSWideMinimalPlanLength.TabIndex = 31;
            this.textBoxBFSWideMinimalPlanLength.Validated += new System.EventHandler(this.textBoxBFSWideMinimalPlanLength_Validated);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(-3, 36);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 13);
            this.label15.TabIndex = 30;
            this.label15.Text = "Minimal plan length:";
            // 
            // textBoxBFSVisitedStates
            // 
            this.textBoxBFSVisitedStates.Location = new System.Drawing.Point(131, 7);
            this.textBoxBFSVisitedStates.Name = "textBoxBFSVisitedStates";
            this.textBoxBFSVisitedStates.Size = new System.Drawing.Size(100, 20);
            this.textBoxBFSVisitedStates.TabIndex = 25;
            this.textBoxBFSVisitedStates.Validated += new System.EventHandler(this.textBoxBFSVisitedStates_Validated);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(-3, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(123, 13);
            this.label13.TabIndex = 23;
            this.label13.Text = "Number of visited states:";
            // 
            // checkBoxRemoveTyping
            // 
            this.checkBoxRemoveTyping.AutoSize = true;
            this.checkBoxRemoveTyping.Location = new System.Drawing.Point(12, 279);
            this.checkBoxRemoveTyping.Name = "checkBoxRemoveTyping";
            this.checkBoxRemoveTyping.Size = new System.Drawing.Size(97, 17);
            this.checkBoxRemoveTyping.TabIndex = 29;
            this.checkBoxRemoveTyping.Text = "Remove typing";
            this.checkBoxRemoveTyping.UseVisualStyleBackColor = true;
            this.checkBoxRemoveTyping.CheckedChanged += new System.EventHandler(this.checkBoxRemoveTyping_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(383, 17);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(87, 13);
            this.label14.TabIndex = 30;
            this.label14.Text = "Plans generated:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(383, 42);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(107, 13);
            this.label16.TabIndex = 31;
            this.label16.Text = "Current init state\'s ID:";
            // 
            // labelVisited
            // 
            this.labelVisited.AutoSize = true;
            this.labelVisited.Location = new System.Drawing.Point(11, 10);
            this.labelVisited.Name = "labelVisited";
            this.labelVisited.Size = new System.Drawing.Size(73, 13);
            this.labelVisited.TabIndex = 32;
            this.labelVisited.Text = "States visited:";
            // 
            // labelFound
            // 
            this.labelFound.AutoSize = true;
            this.labelFound.Location = new System.Drawing.Point(11, 35);
            this.labelFound.Name = "labelFound";
            this.labelFound.Size = new System.Drawing.Size(70, 13);
            this.labelFound.TabIndex = 33;
            this.labelFound.Text = "States found:";
            // 
            // labelDepth
            // 
            this.labelDepth.AutoSize = true;
            this.labelDepth.Location = new System.Drawing.Point(11, 60);
            this.labelDepth.Name = "labelDepth";
            this.labelDepth.Size = new System.Drawing.Size(74, 13);
            this.labelDepth.TabIndex = 34;
            this.labelDepth.Text = "Current depth:";
            // 
            // panelBFSProgress
            // 
            this.panelBFSProgress.Controls.Add(this.labelCurrentDepth);
            this.panelBFSProgress.Controls.Add(this.labelStatesCount);
            this.panelBFSProgress.Controls.Add(this.labelStatesVisited);
            this.panelBFSProgress.Controls.Add(this.labelVisited);
            this.panelBFSProgress.Controls.Add(this.labelDepth);
            this.panelBFSProgress.Controls.Add(this.labelFound);
            this.panelBFSProgress.Location = new System.Drawing.Point(371, 57);
            this.panelBFSProgress.Name = "panelBFSProgress";
            this.panelBFSProgress.Size = new System.Drawing.Size(258, 100);
            this.panelBFSProgress.TabIndex = 35;
            // 
            // labelCurrentDepth
            // 
            this.labelCurrentDepth.AutoSize = true;
            this.labelCurrentDepth.Location = new System.Drawing.Point(130, 60);
            this.labelCurrentDepth.Name = "labelCurrentDepth";
            this.labelCurrentDepth.Size = new System.Drawing.Size(25, 13);
            this.labelCurrentDepth.TabIndex = 39;
            this.labelCurrentDepth.Text = "100";
            // 
            // labelStatesCount
            // 
            this.labelStatesCount.AutoSize = true;
            this.labelStatesCount.Location = new System.Drawing.Point(130, 35);
            this.labelStatesCount.Name = "labelStatesCount";
            this.labelStatesCount.Size = new System.Drawing.Size(25, 13);
            this.labelStatesCount.TabIndex = 38;
            this.labelStatesCount.Text = "100";
            // 
            // labelStatesVisited
            // 
            this.labelStatesVisited.AutoSize = true;
            this.labelStatesVisited.Location = new System.Drawing.Point(130, 10);
            this.labelStatesVisited.Name = "labelStatesVisited";
            this.labelStatesVisited.Size = new System.Drawing.Size(25, 13);
            this.labelStatesVisited.TabIndex = 38;
            this.labelStatesVisited.Text = "100";
            // 
            // labelPlansGenerated
            // 
            this.labelPlansGenerated.AutoSize = true;
            this.labelPlansGenerated.Location = new System.Drawing.Point(501, 17);
            this.labelPlansGenerated.Name = "labelPlansGenerated";
            this.labelPlansGenerated.Size = new System.Drawing.Size(25, 13);
            this.labelPlansGenerated.TabIndex = 36;
            this.labelPlansGenerated.Text = "100";
            // 
            // labelCurrentInitStateId
            // 
            this.labelCurrentInitStateId.AutoSize = true;
            this.labelCurrentInitStateId.Location = new System.Drawing.Point(501, 42);
            this.labelCurrentInitStateId.Name = "labelCurrentInitStateId";
            this.labelCurrentInitStateId.Size = new System.Drawing.Size(25, 13);
            this.labelCurrentInitStateId.TabIndex = 37;
            this.labelCurrentInitStateId.Text = "100";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(452, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 22);
            this.button1.TabIndex = 38;
            this.button1.Text = "Generate test data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // checkBoxParametersCanRepeatInActions
            // 
            this.checkBoxParametersCanRepeatInActions.AutoSize = true;
            this.checkBoxParametersCanRepeatInActions.Location = new System.Drawing.Point(12, 233);
            this.checkBoxParametersCanRepeatInActions.Name = "checkBoxParametersCanRepeatInActions";
            this.checkBoxParametersCanRepeatInActions.Size = new System.Drawing.Size(181, 17);
            this.checkBoxParametersCanRepeatInActions.TabIndex = 39;
            this.checkBoxParametersCanRepeatInActions.Text = "Parameters can repeat in actions";
            this.checkBoxParametersCanRepeatInActions.UseVisualStyleBackColor = true;
            this.checkBoxParametersCanRepeatInActions.CheckedChanged += new System.EventHandler(this.checkBoxParametersCanRepeatInActions_CheckedChanged);
            // 
            // panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 474);
            this.Controls.Add(this.checkBoxParametersCanRepeatInActions);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBoxRemoveTypeOptions);
            this.Controls.Add(this.labelCurrentInitStateId);
            this.Controls.Add(this.labelPlansGenerated);
            this.Controls.Add(this.panelBFSProgress);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.checkBoxRemoveTyping);
            this.Controls.Add(this.panelBFSWide);
            this.Controls.Add(this.panelBFS);
            this.Controls.Add(this.panelSequences);
            this.Controls.Add(this.textBoxPlansCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBoxTransitionStates);
            this.Controls.Add(this.comboBoxInitState);
            this.Controls.Add(this.comboBoxGoalState);
            this.Controls.Add(this.comboBoxGeneratingMethod);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPredicateChance);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxOutputFile);
            this.Controls.Add(this.textBoxInputFile);
            this.Controls.Add(this.checkBoxRemoveOperatorsLists);
            this.Controls.Add(this.buttonSetFile);
            this.Controls.Add(this.buttonGenerate);
            this.Name = "panel";
            this.Text = "Plan generator";
            this.panelSequences.ResumeLayout(false);
            this.panelSequences.PerformLayout();
            this.panelBFS.ResumeLayout(false);
            this.panelBFS.PerformLayout();
            this.panelBFSWide.ResumeLayout(false);
            this.panelBFSWide.PerformLayout();
            this.panelBFSProgress.ResumeLayout(false);
            this.panelBFSProgress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGenerate;
        private System.Windows.Forms.Button buttonSetFile;
        private System.Windows.Forms.CheckBox checkBoxRemoveOperatorsLists;
        private System.Windows.Forms.TextBox textBoxInputFile;
        private System.Windows.Forms.TextBox textBoxOutputFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPredicateChance;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxGeneratingMethod;
        private System.Windows.Forms.ComboBox comboBoxGoalState;
        private System.Windows.Forms.ComboBox comboBoxInitState;
        private System.Windows.Forms.ComboBox comboBoxTransitionStates;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPlansCount;
        private System.Windows.Forms.Panel panelSequences;
        private System.Windows.Forms.TextBox textBoxSequencesMinActions;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxSequencesDesiredActionsCount;
        private System.Windows.Forms.Panel panelBFS;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxBFSMaxVisitedStates;
        private System.Windows.Forms.TextBox textBoxBFSGoalPredicates;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panelBFSWide;
        private System.Windows.Forms.TextBox textBoxBFSVisitedStates;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxBFSMinimalPlanLength;
        private System.Windows.Forms.TextBox textBoxBFSWideMinimalPlanLength;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox checkBoxRemoveTyping;
        private System.Windows.Forms.CheckBox checkBoxRemoveTypeOptions;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label labelVisited;
        private System.Windows.Forms.Label labelFound;
        private System.Windows.Forms.Label labelDepth;
        private System.Windows.Forms.Panel panelBFSProgress;
        private System.Windows.Forms.Label labelPlansGenerated;
        private System.Windows.Forms.Label labelCurrentInitStateId;
        private System.Windows.Forms.Label labelStatesVisited;
        private System.Windows.Forms.Label labelCurrentDepth;
        private System.Windows.Forms.Label labelStatesCount;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxParametersCanRepeatInActions;
    }
}

