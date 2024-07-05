using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ViewModel;
namespace UserInterface
{
    public partial class Form1 : Form
    {
        ModelViewRequirementBase MV;
        int stateIndex;
        public Form1()
        {
            MV = new ModelViewRequirementBase();
            InitializeComponent();
            DataBind();
            stateIndex = 3;
            this.AcceptButton = SearchButton;
        }
        public Form1(string[] requirementArray) : this()
        {
            string requirement = string.Join(" ", requirementArray);
            MV.searchBox.requirement1 = requirement;
            MV.searchBox.requirement2 = "";
            MV.searchBox.ChangeStateIndex(stateIndex);
            MV.Search();
        }
        private void DataBind()
        {
            Count.DataBindings.Add("Text", MV, "Count");

            MoreButton.DataBindings.Add("Visible", MV, "More");
            MoreButton.Tag = new Action(MV.LoadMore);
            MoreButton.Click += ButtonCLickVerseAction;

            PreviousButton.DataBindings.Add("Visible", MV, "Previous");
            PreviousButton.Tag = new Action(MV.LoadPrevious);
            PreviousButton.Click += ButtonCLickVerseAction;

            SourceButton.Tag = new Action(MV.ShowSources);
            SourceButton.Click += ButtonCLickVerseAction;

            TargetButton.Tag = new Action(MV.ShowTargets);
            TargetButton.Click += ButtonCLickVerseAction;


            for (int i = 0; i < ModelViewRequirementBase.numberOfLinkBoxes; i++)
            {
                CreateLinkTable(i);
            }
            RequirementLabel.DataBindings.Add("Text", MV.requirementBox, "RequirementDescription");
            RequirementTextBox.DataBindings.Add("Text", MV.requirementBox, "Text");
            RequirementTextBox.TextChanged += VerseTextChanged;


            BibleChooseBox.DataSource = MV.searchBox.bibleNames;
            BibleChooseBox.DataBindings.Add("SelectedIndex", MV.searchBox, "CurrentBibleIndex");
            BibleChooseBox.SelectedIndexChanged += BibleChooseBox_SelectedIndexChanged;


            helpRichTextBox.DataBindings.Add("Text", MV.searchBox, "HelpText");
        }
        private void CreateLinkTable(int i)
        {
            FoundVersesTable.SuspendLayout();

            TableLayoutPanel table = new TableLayoutPanel();
            table.SuspendLayout();
            FoundVersesTable.Controls.Add(table, 0, i + 1);
            TableProportionsSetUp(table);
            LinkBox linkBox = MV.linkBoxes[i];
            table.Tag = linkBox;

            Label sourceLabel = new Label();
            Label s = new Label();
            s.Text = "S:";
            Label t = new Label();
            t.Text = "T:";
            Label targetLabel = new Label();
            Label occurance = new Label();
            Button back = new Button();
            Button next = new Button();
            RichTextBox verse = new RichTextBox();

            //text appearance
            verse.ReadOnly = true;
            verse.TextChanged += VerseTextChanged;
            back.Text = "<";
            next.Text = ">";
            sourceLabel.AutoSize = true;
            sourceLabel.MinimumSize = new Size(100, 0);
            sourceLabel.TextAlign = ContentAlignment.MiddleLeft;
            s.TextAlign = ContentAlignment.MiddleLeft;
            targetLabel.AutoSize = true;
            targetLabel.TextAlign = ContentAlignment.MiddleLeft;
            t.TextAlign = ContentAlignment.MiddleLeft;
            occurance.TextAlign = ContentAlignment.MiddleCenter;
            occurance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);

            table.DataBindings.Add("Visible", linkBox, "Visible");
            //databinding - text
            verse.DataBindings.Add("Text", linkBox, "Text");
            sourceLabel.DataBindings.Add("Text", linkBox, "SourceLabel");
            targetLabel.DataBindings.Add("Text", linkBox, "TargetLabel");
            occurance.DataBindings.Add("Text", linkBox, "Occurance");
            //databinding - buttons
            back.DataBindings.Add("Visible", linkBox, "Back");
            next.DataBindings.Add("Visible", linkBox, "Next");
            back.Tag = new Action(linkBox.BackVerse);
            next.Tag = new Action(linkBox.NextVerse);


            table.Controls.Add(sourceLabel, 1, 0);
            table.Controls.Add(t, 0, 1);
            table.Controls.Add(s, 0, 0);
            table.Controls.Add(targetLabel, 1, 1);
            table.Controls.Add(occurance, 5, 0);
            table.Controls.Add(back, 2, 0);
            table.Controls.Add(verse, 3, 0);
            table.Controls.Add(next, 4, 0);

            table.SetRowSpan(back, 2);
            table.SetRowSpan(next, 2);
            table.SetRowSpan(verse, 2);

            foreach (Control control in table.Controls)
            {
                control.Dock = DockStyle.Fill;
                if (control is Button button)
                {
                    button.UseVisualStyleBackColor = true;
                    button.Click += ButtonCLickVerseAction;
                }
            }

            table.ResumeLayout(false);
            FoundVersesTable.ResumeLayout(false);
        }

        private void TableProportionsSetUp(TableLayoutPanel table)
        {
            table.Dock = DockStyle.Fill;
            table.ColumnCount = 6;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            table.ColumnStyles.Add(new ColumnStyle());
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            table.RowCount = 2;
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        }
        private void ButtonCLickVerseAction(object? sender, EventArgs? e)
        {
            if (sender is Button button && button.Tag is Action action)
            {
                action.Invoke();
            }
        }

        private void BibleChooseBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox box)
            {
                MV.searchBox.ChangeBible(box.SelectedIndex);
                MV.Reset();
            }
        }

        private void PlusButton_Click(object sender, EventArgs e)
        {
            if (sender is Button bt)
            {
                if (MV.searchBox.addedRequirement)
                {
                    RequirementTextBox2.Visible = false;
                    MV.searchBox.addedRequirement = false;
                    bt.Text = "+";
                    StateLabel.Visible = true;
                    StateTable.Visible = true;
                    SwitchButton.Visible = false;
                }
                else
                {
                    RequirementTextBox2.Visible = true;
                    MV.searchBox.addedRequirement = true;
                    bt.Text = "-";
                    StateLabel.Visible = false;
                    StateTable.Visible = false;
                    SwitchButton.Visible = true;
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            helpRichTextBox.Visible = false;
            MV.searchBox.requirement1 = RequirementTextBox1.Text;
            MV.searchBox.requirement2 = RequirementTextBox2.Text;
            MV.searchBox.ChangeStateIndex(stateIndex);
            MV.Search();

        }
        private void VerseTextChanged(object? sender, EventArgs e)
        {
            if (sender is RichTextBox r && r != null)
            {
                r.TextChanged -= VerseTextChanged;
                UpdateForBreaksInText(r);
                UpdateColorForTags(r);
                r.TextChanged += VerseTextChanged;
            }
        }
        private void UpdateForBreaksInText(RichTextBox r)
        {
            string[] patterns = { "<br/>", "<pb/>", "<h>", "</h>" };
            int count = patterns.Length;
            for (int i = 0; i < count; i++)
            {
                var matches = Regex.Matches(r.Text, patterns[i]);
                int countM = matches.Count - 1;
                for (int k = countM; k >= 0; k--)
                {
                    Match match = matches[k];
                    int start = match.Index;
                    int length = match.Value.Length;
                    r.Select(start, length);
                    r.SelectedText = Environment.NewLine;
                }
                r.DeselectAll();
            }
        }
        private void UpdateColorForTags(RichTextBox r)
        {
            (string, Color)[] patternsColors = {
                (@"<S>(.*?)</S>", Color.Yellow),
                (@"<m>(.*?)</m>",Color.Yellow),
                (@"<i>(.*?)</i>",Color.Blue),
                (@"<J>(.*?)</J>", Color.Red),
                (@"<n>(.*?)</n>",Color.DimGray),
                (@"<e>(.*?)</e>",Color.Black),
                (@"<t>(.*?)</t>",Color.Black),
                (@"<f>(.*?)</f>",Color.Black),
            };
            int length = patternsColors.Length;
            for (int i = 0; i < length; i++)
            {
                UpdateTextColor(r, patternsColors[i]);
            }
        }
        private void UpdateTextColor(RichTextBox r, (string, Color) selection)
        {
            string pattern = selection.Item1;
            Color color = selection.Item2;
            var matches = Regex.Matches(r.Text, pattern);
            int count = matches.Count - 1;
            for (int i = count; i >= 0; i--)
            {
                Match match = matches[i];
                int start = match.Index;
                int length = match.Groups[1].Value.Length;

                r.Select(start, length + 7);
                r.SelectedText = match.Groups[1].Value;
                r.Select(start, length);
                r.SelectionColor = color;
            }
            r.DeselectAll();
        }

        private void SortRadioButton_Click(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                MV.ChangeSortingWay((int)rb.Tag);
            }
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            string temp = RequirementTextBox1.Text;
            RequirementTextBox1.Text = RequirementTextBox2.Text;
            RequirementTextBox2.Text = temp;
        }

        private void StateButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb is not null && rb.Checked)
            {
                stateIndex = (int)rb.Tag;
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            helpRichTextBox.Visible = !helpRichTextBox.Visible;
        }
    }
}