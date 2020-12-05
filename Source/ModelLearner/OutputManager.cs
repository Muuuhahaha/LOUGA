using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ModelLearner.Algorithms.LOUGA.Individuals;
using ModelLearner.Algorithms.LOUGA.Policies;
using GeneralPlanningLibrary;

namespace ModelLearner
{
    /// <summary>
    /// This class conveys communications between UI and algorithm in different thread.
    /// </summary>
    public class OutputManager
    {
        /// <summary>
        /// Indices of tabs of given name.
        /// </summary>
        public Dictionary<string, int> TabsIndices;
        public TabControl Tabs;
        public RichTextBox[] TextBoxes;
        public ListBox[] ListBoxesBestIndividuals;


        /// <summary>
        /// List of tab pages. 
        /// </summary>
        private TabPage[] TabPages;

        /// <summary>
        /// Visibility of corresponding tab pages.
        /// </summary>
        private bool[] TabPageVisible;

        /// <summary>
        /// Texts to add to corresponding tabs.
        /// </summary>
        public string[] Texts;

        /// <summary>
        /// RTFs to set corresponding tabs to.
        /// </summary>
        public string[] RTFs;

        /// <summary>
        /// True if tab contents were changed in algorithm's run and should be updated in main WF thread.
        /// </summary>
        public bool[] Changed;

        /// <summary>
        /// True if tab should be cleared.
        /// </summary>
        public bool[] ClearTab;

        /// <summary>
        /// Types of tabs. Can be standard text box or list box with LOUGA's best found individuals.
        /// </summary>
        public TabType[] TabTypes;

        /// <summary>
        /// True if tab should be highlighted in next tick by main WF thread.
        /// </summary>
        public bool[] HighlightQueued;

        /// <summary>
        /// Best individuals of corresponding tab.
        /// </summary>
        public List<IntegerIndividual>[] BestIndividuals;

        /// <summary>
        /// Individuals that should be added to corresponding tab.
        /// </summary>
        public List<IntegerIndividual>[] TmpBestIndividuals;

        /// <summary>
        /// Solution of LOUGA - predicate by predicate run.
        /// </summary>
        public IntegerIndividual Solution;

        public delegate Model IndividualToModel(IntegerIndividual individual);
        public delegate string IndividualToString(IntegerIndividual individual);
        public delegate void HighlightFuncion(RichTextBox box);

        /// <summary>
        /// Function that converts individual to corresponding model representation.
        /// </summary>
        public IndividualToModel[] ITM;

        /// <summary>
        /// Function that converts individual to printable add/delete list representation.
        /// </summary>
        public IndividualToString[] ITS;
        
        /// <summary>
        /// Function for highlighting model in PDDL format in RichTextBox.
        /// </summary>
        public HighlightFuncion Highlight;

        /// <summary>
        /// ComboBox with predicates selection during LOUGA predicate by predicate mode
        /// </summary>
        public ComboBox ComboBoxPredicate;

        /// <summary>
        /// Standart font used in every TextBox
        /// </summary>
        public static Font TextboxFont = new Font("Consolas", 8.25f, FontStyle.Regular);

        public const string SolutionTabName = "Solution";
        public const string OutputTabName = "Output";

        /// <summary>
        /// Standard ListBox that is copied before algorithm's start to tabs that shows lists of individuals.
        /// </summary>
        private ListBox BestIndividualsListBoxToCopy;

        /// <summary>
        /// Standard TextBox that is copied before algorithm's start to tabs that shows lists of individuals.
        /// </summary>
        private RichTextBox BestIndividualsTextBoxToCopy;

        /// <summary>
        /// Functions that update individuals PDDL representation in tabs that shows lists of individuals.
        /// Used when user cahnges output format.
        /// </summary>
        private List<EventHandler> IndividualsFormatUpdateFunctions;

        public OutputManager(TabControl tabs)
        {
            this.Tabs = tabs;
            TabsIndices = new Dictionary<string, int>();
            
            TextBoxes = new RichTextBox[0];
            Texts = new string[0];
            Changed = new bool[0];
            ClearTab = new bool[0];
            HighlightQueued = new bool[0];
            IndividualsFormatUpdateFunctions = new List<EventHandler>();
        }

        /// <summary>
        /// Sets standard TextBox and ListBox that are copied before algorithm's start to tabs that shows lists of individuals.
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="richTextBox"></param>
        public void SetBoxesToCopy (ListBox listBox, RichTextBox richTextBox)
        {
            this.BestIndividualsListBoxToCopy = listBox;
            this.BestIndividualsTextBoxToCopy = richTextBox;
        }

        /// <summary>
        /// Deletes all tabs.
        /// </summary>
        public void DeleteOutputTabs()
        {
            while (Tabs.TabPages.Count > 1) Tabs.TabPages.RemoveAt(1);
        }

        /// <summary>
        /// Creates new tabs of given names and given types.
        /// </summary>
        /// <param name="names">Names of new tabs</param>
        /// <param name="tabTypes">Types of new tabs. Default are simple text tabs.</param>
        public void SetOutputTabs(string[] names, TabType[] tabTypes = null, bool[] visibleTabs = null)
        {
            if (Tabs.TabPages.Count > 1) DeleteOutputTabs();
            TabsIndices.Clear();

            TabPages = new TabPage[names.Length];
            TabPageVisible = new bool[names.Length];
            TextBoxes = new RichTextBox[names.Length];
            Texts = new string[names.Length];
            RTFs = new string[names.Length];
            ClearTab = new bool[names.Length];
            Changed = new bool[names.Length];
            HighlightQueued = new bool[names.Length];
            TmpBestIndividuals = new List<IntegerIndividual>[names.Length];
            ITM = new IndividualToModel[names.Length];
            ITS = new IndividualToString[names.Length];
            IndividualsFormatUpdateFunctions = new List<EventHandler>();
            
            if (tabTypes != null)
            {
                BestIndividuals = new List<IntegerIndividual>[tabTypes.Length];
                ListBoxesBestIndividuals = new ListBox[tabTypes.Length];
                TabTypes = tabTypes;
            }
            else
            {
                TabTypes = new TabType[names.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    TabTypes[i] = TabType.OutputTab;
                }
            }

            for (int i = 0; i < names.Length; i++)
            {
                TabPages[i] = new TabPage(names[i]);
                if (visibleTabs == null || visibleTabs[i])
                {
                    Tabs.TabPages.Add(TabPages[i]);
                    TabPageVisible[i] = true;
                }
               
                TabsIndices.Add(names[i], i);
                RTFs[i] = null;

                //creation of standard text tab
                if (tabTypes == null || tabTypes.Length <= i || tabTypes[i] == TabType.OutputTab)
                {
                    TextBoxes[i] = new RichTextBox();
                    TextBoxes[i].Font = TextboxFont;
                    TextBoxes[i].Location = new Point(0, 1);
                    TextBoxes[i].Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                    TextBoxes[i].Size = new Size(Tabs.Size.Width - 8, Tabs.Size.Height - 28);
                    TextBoxes[i].WordWrap = false;
                    TextBoxes[i].ReadOnly = true;
                    TextBoxes[i].HideSelection = false;
                    TabPages[i].Controls.Add(TextBoxes[i]);
                }
                //creation of standart best individuals tab
                else
                {
                    ListBox listBox = new ListBox();
                    listBox.Size = BestIndividualsListBoxToCopy.Size;
                    listBox.Location = BestIndividualsListBoxToCopy.Location;
                    listBox.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);

                    var handler = ShowIndividual(i);
                    IndividualsFormatUpdateFunctions.Add(handler);
                    listBox.SelectedIndexChanged += handler;

                    ListBoxesBestIndividuals[i] = listBox;
                    TabPages[i].Controls.Add(listBox);

                    
                    RichTextBox textBox = new RichTextBox();
                    textBox.Size = BestIndividualsTextBoxToCopy.Size;
                    textBox.Location = BestIndividualsTextBoxToCopy.Location;
                    textBox.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
                    textBox.Font = TextboxFont;
                    textBox.WordWrap = false;
                    textBox.ReadOnly = true;

                    TextBoxes[i] = textBox;
                    TabPages[i].Controls.Add(textBox);
                    
                    BestIndividuals[i] = new List<IntegerIndividual>();
                    TmpBestIndividuals[i] = new List<IntegerIndividual>();
                }
            }
        }

        /// <summary>
        /// Clears tab of given name.
        /// </summary>
        /// <param name="tabName"></param>
        public void ClearOutputTab(string tabName)
        {
            ClearOutputTab(TabsIndices[tabName]);
        }

        /// <summary>
        /// Clears tab of given id.
        /// </summary>
        /// <param name="tabId"></param>
        public void ClearOutputTab(int tabId)
        {
            lock (this)
            {
                ClearTab[tabId] = true;
                Texts[tabId] = "";
                RTFs[tabId] = null;
                Changed[tabId] = true;
            }
        }

        /// <summary>
        /// Sets contents of tab of given name to given text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tabName"></param>
        public void SetTabText(string text, string tabName)
        {
            SetTabText(text, TabsIndices[tabName]);
        }

        /// <summary>
        /// Sets contents of tab of given id to given text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tabId"></param>
        public void SetTabText(string text, int tabId)
        {
            lock (this)
            {
                ClearTab[tabId] = true;
                Texts[tabId] = text;
                Changed[tabId] = true;
            }
        }

        /// <summary>
        /// Sets contents of tab of given name to given rtf text.
        /// </summary>
        /// <param name="rtf"></param>
        /// <param name="tabName"></param>
        public void SetTabRTF(string rtf, string tabName = SolutionTabName)
        {
            SetTabRTF(rtf, TabsIndices[tabName]);
        }

        /// <summary>
        /// Sets contents of tab of given id to given rtf text.
        /// </summary>
        /// <param name="rtf"></param>
        /// <param name="tabId"></param>
        public void SetTabRTF(string rtf, int tabId)
        {
            lock (this)
            {
                RTFs[tabId] = rtf;
                Changed[tabId] = true;
                Texts[tabId] = "";
                ClearTab[tabId] = false;
            }
        }

        /// <summary>
        /// Highlights PDDL syntax in tab of given name.
        /// </summary>
        /// <param name="tabName"></param>
        public void HighlightTab(string tabName = SolutionTabName)
        {
            HighlightTab(TabsIndices[tabName]);
        }

        /// <summary>
        /// Highlight PDDL syntax in tab of given id.
        /// </summary>
        /// <param name="tabId"></param>
        public void HighlightTab(int tabId)
        {
            lock (this)
            {
                HighlightQueued[tabId] = true;
            }
        }

        /// <summary>
        /// Sets tab of given name to be active tab.
        /// </summary>
        /// <param name="tab"></param>
        public void SetActiveTab(string tab)
        {
            SetActiveTab(TabsIndices[tab]);
        }

        /// <summary>
        /// Sets tab of given id to be active tab.
        /// </summary>
        /// <param name="tabId"></param>
        public void SetActiveTab(int tabId)
        {
            Tabs.SelectedTab = Tabs.TabPages[tabId + 1];
        }

        /// <summary>
        /// Writes given text to tab of given name.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tabName"></param>
        public void Write(string text, string tabName)
        {
            Write(text, TabsIndices[tabName]);
        }

        /// <summary>
        /// Writes given text to tab of given id.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tabId"></param>
        public void Write(string text, int tabId)
        {
            lock (this)
            {
                Texts[tabId] += text;
                Changed[tabId] = true;
            }
        }

        /// <summary>
        /// Adds new individuals to tab of given ID
        /// </summary>
        /// <param name="individuals">Individuals to add to tab's list.</param>
        /// <param name="id">ID of tab</param>
        public void AddBestIndividuals(List<IntegerIndividual> individuals, int id)
        {
            lock (this)
            {
                if (Changed[id])
                {
                    TmpBestIndividuals[id].AddRange(individuals);
                }
                else
                {
                    TmpBestIndividuals[id] = individuals;
                    Changed[id] = true;
                }
            }
        }

        /// <summary>
        /// Writes given line of text to tab of given name.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tabName"></param>
        public void WriteLine(string text = "", string tabName = OutputTabName)
        {
            WriteLine(text, TabsIndices[tabName]);
        }

        /// <summary>
        /// Writes given line of text to tab of given name.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tabId"></param>
        public void WriteLine(string text, int tabId)
        {
            Write(text, tabId);
            Write("\n", tabId);
        }

        /// <summary>
        /// Updates Windows.Forms controls.
        /// Should be called from main WF thread.
        /// </summary>
        public void Update()
        {
            lock (this)
            {
                //update of simple text tabs
                for (int i = 0; i < Texts.Length; i++)
                {
                    if (!Changed[i] || TabTypes[i] == TabType.ListBoxBestIndividuals) continue;

                    if (ClearTab[i])
                    {
                        ClearTab[i] = false;
                        TextBoxes[i].Text = "";
                    }

                    Changed[i] = false;

                    bool focused = false;
                    int index = TextBoxes[i].SelectionStart;
                    int length = TextBoxes[i].SelectionLength;

                    if (TextBoxes[i].Focused)
                    {
                        focused = true;
                        Form1.LabelForFocusing.Focus();
                    }

                    if (Texts[i] != null)
                    {
                        TextBoxes[i].AppendText(Texts[i]);
                        Texts[i] = "";
                    }

                    if (RTFs[i] != null)
                    {
                        TextBoxes[i].Rtf = RTFs[i];
                        RTFs[i] = null;
                    }

                    if (focused)
                    {
                        TextBoxes[i].Focus();
                    }
                }

                //update of tabs showing best individuals
                for (int index = 0; index < Texts.Length; index++)
                {
                    if (!Changed[index] || TabTypes[index] == TabType.OutputTab) continue;

                    //ListBoxesBestIndividuals[index].BeginUpdate();

                    //adding new individuals
                    foreach (var individual in TmpBestIndividuals[index])
                    {
                        bool found = false;

                        for (int i = BestIndividuals[index].Count - 1; i >= 0; i--)
                        {
                            if (BestIndividuals[index][i].Fitness >= individual.Fitness)
                            {
                                BestIndividuals[index].Insert(i + 1, individual);
                                ListBoxesBestIndividuals[index].Items.Insert(i + 1, individual.Fitness.ToString("F3") + ": " + individual.ToString());
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            BestIndividuals[index].Insert(0, individual);
                            ListBoxesBestIndividuals[index].Items.Insert(0, individual.Fitness.ToString("F3") + ": " + individual.ToString());
                        }
                    }

                    //deleting obsolete individuals
                    if (BestIndividuals[index].Count > BestIndividualsLogger.MaxBestIndividuals)
                    {
                        BestIndividuals[index].RemoveRange(BestIndividualsLogger.MaxBestIndividuals, BestIndividuals[index].Count - BestIndividualsLogger.MaxBestIndividuals);
                        while (ListBoxesBestIndividuals[index].Items.Count > BestIndividualsLogger.MaxBestIndividuals)
                        {
                            ListBoxesBestIndividuals[index].Items.RemoveAt(BestIndividualsLogger.MaxBestIndividuals);
                        }
                    }

                    if (ListBoxesBestIndividuals[index].SelectedIndex == -1)
                    {
                        ListBoxesBestIndividuals[index].SelectedIndex = 0;
                    }

                    if (ComboBoxPredicate.Visible)
                    {
                        //renaming field in ComboBox with predicates to show current best individual
                        string name = ((string)ComboBoxPredicate.Items[(index - 3) / 2]).Split(' ')[0];
                        name += " - ";

                        if (BestIndividuals[index][0].Fitness == 1) name += "done";
                        else name += BestIndividuals[index][0].Fitness.ToString("n4");

                        ComboBoxPredicate.Items[(index - 3) / 2] = name;
                    }

                    //ListBoxesBestIndividuals[index].EndUpdate();

                    Changed[index] = false;
                }

                //highlighting PDDL syntax in tabs that require it
                for (int i = 0; i < Texts.Length; i++)
                {
                    if (HighlightQueued[i])
                    {
                        this.Highlight(TextBoxes[i]);
                        HighlightQueued[i] = false;
                    }
                }
            }
        }

        /// <summary>
        /// Creates EventHandler that is used to show individual when it is selected in given tab's ListBox
        /// </summary>
        /// <param name="id">ID of tab that created handler should manage</param>
        /// <returns></returns>
        private EventHandler ShowIndividual(int id) 
        {
            return new EventHandler((object sender, EventArgs e) =>
            {
                lock (this)
                {
                    if (ListBoxesBestIndividuals[id].SelectedIndex == -1)
                    {
                        TextBoxes[id].Text = "";
                    }
                    else
                    {
                        if (!Properties.Settings.Default.LOUGA_PDDLOutput)
                        {
                            TextBoxes[id].Text = ITS[id](BestIndividuals[id][ListBoxesBestIndividuals[id].SelectedIndex]);
                        }
                        else
                        {
                            TextBoxes[id].Text = ITM[id](BestIndividuals[id][ListBoxesBestIndividuals[id].SelectedIndex]).ToString();
                            //Highlight(TextBoxes[id]);
                        }
                    }
                }
            });
        }

        public void HideTab(int id)
        {
            if (!TabPageVisible[id]) return;

            Tabs.TabPages.Remove(TabPages[id]);

            TabPageVisible[id] = false;
        }

        public void ShowUIForPredicate(int predicateId, string predicateName)
        {
            /*int index = Tabs.SelectedIndex;
            
            predicate = " " + predicate;
            for (int i = 2; i < TabPages.Length; i++)
            {
                if (TabPages[i].Text.EndsWith(predicate)) ShowTab(i);
                else HideTab(i);
            }

            Tabs.SelectedIndex = index;*/

            TextBoxes[predicateId].Size = new Size(Tabs.Size.Width - 8, Tabs.Size.Height - 28);

            Tabs.TabPages[3].Controls.Clear();
            Tabs.TabPages[3].Controls.Add(TextBoxes[predicateId]);
            
            Console.WriteLine("width: " + (Tabs.Size.Width - TextBoxes[predicateId + 1].Size.Width));
            Console.WriteLine("height: " + (Tabs.Size.Height - TextBoxes[predicateId + 1].Size.Height));

            TextBoxes[predicateId + 1].Size = new Size(Tabs.Size.Width - 165, Tabs.Size.Height - 33);
            ListBoxesBestIndividuals[predicateId + 1].Size = new Size(139, Tabs.Size.Height - 33);



            //TextBoxes[predicateId + 1].Size.Height = ;


            //ListBoxesBestIndividuals[predicateId + 1].Size = BestIndividualsListBoxToCopy.Size;
            //TextBoxes[predicateId + 1].Size = BestIndividualsTextBoxToCopy.Size;

            Tabs.TabPages[4].Controls.Clear();
            Tabs.TabPages[4].Controls.Add(ListBoxesBestIndividuals[predicateId + 1]);
            Tabs.TabPages[4].Controls.Add(TextBoxes[predicateId + 1]);


            Tabs.TabPages[3].Text = "Output - " + predicateName;
            Tabs.TabPages[4].Text = "Best individuals - " + predicateName;
        }


        public void ShowTab(int id)
        {
            if (TabPageVisible[id]) return;

            int count = 1;
            for (int i = 0; i < id; i++)
            {
                if (TabPageVisible[i]) count++;
            }

            Tabs.TabPages.Insert(count, TabPages[id]);

            TabPageVisible[id] = true;
        }

        /// <summary>
        /// Reformats all individuals representations in all tabs.
        /// Used when user changes individual's output format.
        /// </summary>
        public void ReformatIndividuals()
        {
            lock (this) {
                foreach (var handler in IndividualsFormatUpdateFunctions)
                {
                    handler(null, null);
                }

                if (Solution != null)
                {
                    TextBoxes[TabsIndices[SolutionTabName]].Text = ITM[1](Solution).ToString();
                    //TextBoxes[TabsIndices[SolutionTabName]].AppendText("\n" + Solution.ErrorInfo.ToString());
                    Highlight(TextBoxes[TabsIndices[SolutionTabName]]);
                }
            }
        }
    }

    /// <summary>
    /// Type of tab
    /// </summary>
    public enum TabType
    {
        OutputTab, 
        ListBoxBestIndividuals
    }

}
