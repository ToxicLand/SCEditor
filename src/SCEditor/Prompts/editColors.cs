using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCEditor.Prompts
{
    public partial class editColors : Form
    {
        private readonly ScFile _scfile;
        private AddState addColorState { get; set; } = AddState.Add;
        private Dictionary<int, List<Tuple<Color, byte, Color>>> addedColors;
        public editColors(ScFile scFile)
        {
            InitializeComponent();

            _scfile = scFile;

            addedColors = new Dictionary<int, List<Tuple<Color, byte, Color>>>();

            this.transformIDNum.Maximum = _scfile.GetTransformStorage().Count - 1;
            this.transformIDNum.Value = 0;

            toColorNumR.Enabled = false;
            toColorNumG.Enabled = false;
            toColorNumB.Enabled = false;
            toColorNumA.Enabled = false;

            fromColorNumR.Enabled = false;
            fromColorNumG.Enabled = false;
            fromColorNumB.Enabled = false;
            fromColorNumA.Enabled = false;

            colorAlphaNum.Enabled = false;

            refreshListBox();
        }

        private void transformIDNum_ValueChanged(object sender, EventArgs e)
        {
            if (this.transformIDNum.Value < _scfile.GetTransformStorage().Count)
            {
                refreshListBox();
            }
            else
            {
                this.transformIDNum.Value = 0;
            }
        }

        private void colorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int totalCount = _scfile.getColors((int)this.transformIDNum.Value).Count;

            if (this.addedColors.ContainsKey((int)this.transformIDNum.Value))
                totalCount += this.addedColors[(int)this.transformIDNum.Value].Count;

            if (this.colorsListBox.SelectedIndex < totalCount)
            {
                this.colorIdNum.Value = this.colorsListBox.SelectedIndex;

                Tuple<Color, byte, Color> colorData = null;

                if (this.colorsListBox.SelectedIndex < _scfile.getColors((int)this.transformIDNum.Value).Count)
                {
                    colorData = _scfile.getColors((int)this.transformIDNum.Value)[this.colorsListBox.SelectedIndex];
                }
                else
                {
                    colorData = addedColors[(int)this.transformIDNum.Value][(totalCount - _scfile.getColors((int)this.transformIDNum.Value).Count) - 1];
                }

                toColorNumR.Value = colorData.Item1.R;
                toColorNumG.Value = colorData.Item1.G;
                toColorNumB.Value = colorData.Item1.B;
                toColorNumA.Value = colorData.Item1.A;

                fromColorNumR.Value = colorData.Item3.R;
                fromColorNumG.Value = colorData.Item3.G;
                fromColorNumB.Value = colorData.Item3.B;
                fromColorNumA.Value = colorData.Item3.A;

                colorAlphaNum.Value = colorData.Item2;
            }
        }

        private void selectId_Click(object sender, EventArgs e)
        {
            if (this.colorIdNum.Value < _scfile.getColors((int)this.transformIDNum.Value).Count) // fix add saved ids too
            {
                this.colorsListBox.SelectedIndex = (int)this.colorIdNum.Value;
                this.colorsListBox.SelectedIndexChanged += null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.addColorState == AddState.Add)
            {
                toColorNumR.Enabled = true;
                toColorNumG.Enabled = true;
                toColorNumB.Enabled = true;
                toColorNumA.Enabled = true;

                fromColorNumR.Enabled = true;
                fromColorNumG.Enabled = true;
                fromColorNumB.Enabled = true;
                fromColorNumA.Enabled = true;

                colorAlphaNum.Enabled = true;

                this.button1.Text = "Confirm";

                this.addColorState = AddState.Confirm;
            }
            else if (this.addColorState == AddState.Confirm)
            {
                Tuple<Color, byte, Color> colorData = new Tuple<Color, byte, Color>(
                    Color.FromArgb((int)toColorNumA.Value, (int)toColorNumR.Value, (int)toColorNumG.Value, (int)toColorNumB.Value),
                    (byte)colorAlphaNum.Value,
                    Color.FromArgb((int)fromColorNumA.Value, (int)fromColorNumR.Value, (int)fromColorNumG.Value, (int)fromColorNumB.Value)
                    );

                if (addedColors.ContainsKey((int)this.transformIDNum.Value))
                {
                    addedColors[(int)this.transformIDNum.Value].Add(colorData);
                }
                else
                {
                    addedColors.Add((int)this.transformIDNum.Value, new List<Tuple<Color, byte, Color>>() { colorData });
                }

                toColorNumR.Enabled = false;
                toColorNumG.Enabled = false;
                toColorNumB.Enabled = false;
                toColorNumA.Enabled = false;

                fromColorNumR.Enabled = false;
                fromColorNumG.Enabled = false;
                fromColorNumB.Enabled = false;
                fromColorNumA.Enabled = false;

                colorAlphaNum.Enabled = false;

                this.button1.Text = "Add Color";

                this.colorsListBox.Items.Add(this.colorsListBox.Items.Count.ToString());

                this.colorIdNum.Maximum = this.colorsListBox.Items.Count - 1;

                this.colorsListBox.SelectedIndex = (this.colorsListBox.Items.Count - 1);
                this.colorsListBox.SelectedIndexChanged += null;

                this.addColorState = AddState.Add;
            }


        }

        private void refreshListBox()
        {
            this.colorsListBox.Items.Clear();

            this.colorIdNum.Maximum = (_scfile.getColors((int)this.transformIDNum.Value).Count - 1);

            for (int i = 0; i < _scfile.getColors((int)this.transformIDNum.Value).Count; i++)
            {
                this.colorsListBox.Items.Add(i.ToString());
            }

            if (addedColors.ContainsKey((int)this.transformIDNum.Value))
            {
                this.colorIdNum.Maximum += addedColors[(int)this.transformIDNum.Value].Count;

                for (int i = 0; i < addedColors[(int)this.transformIDNum.Value].Count; i++)
                {
                    this.colorsListBox.Items.Add((i + this.colorsListBox.Items.Count).ToString());
                }
            }
        }

        public Dictionary<int, List<Tuple<Color, byte, Color>>> getAddedColors()
        {
            return this.addedColors;
        }

        private void editColors_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (addedColors.Count > 0)
            {
                DialogResult msgBox = MessageBox.Show("You have made changes to colors array. Are you sure you want to save the changes?", "Confirm Colors Changes", MessageBoxButtons.YesNoCancel);

                if (msgBox != DialogResult.Cancel)
                {
                    this.DialogResult = msgBox;
                    return;
                }
                
                e.Cancel = true;
            }
        }
    }

    public enum AddState
    {
        Add,
        Confirm
    }
}
