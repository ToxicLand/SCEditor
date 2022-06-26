using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCEditor.Prompts
{
    public partial class editMatrixes : Form
    {
        private AddState addMatrixState { get; set; } = AddState.Add;
        private Dictionary<int, List<Matrix>> addedMatrixes;

        private readonly ScFile _scfile;

        public editMatrixes(ScFile scFile)
        {
            InitializeComponent();

            _scfile = scFile;

            addedMatrixes = new Dictionary<int, List<Matrix>>();

            this.transformIDNum.Maximum = _scfile.GetTransformStorage().Count - 1;
            this.transformIDNum.Value = 0;

            this.numericUpDown1.Enabled = false;
            this.numericUpDown2.Enabled = false;
            this.numericUpDown3.Enabled = false;
            this.numericUpDown4.Enabled = false;
            this.numericUpDown5.Enabled = false;
            this.numericUpDown6.Enabled = false;

            this.numericUpDown1.DecimalPlaces = 6;
            this.numericUpDown2.DecimalPlaces = 6;
            this.numericUpDown3.DecimalPlaces = 6;
            this.numericUpDown4.DecimalPlaces = 6;
            this.numericUpDown5.DecimalPlaces = 6;
            this.numericUpDown6.DecimalPlaces = 6;

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

        private void matrixList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int totalCount = _scfile.GetMatrixs((int)this.transformIDNum.Value).Count;

            if (this.addedMatrixes.ContainsKey((int)this.transformIDNum.Value))
                totalCount += this.addedMatrixes[(int)this.transformIDNum.Value].Count;

            if (this.matrixList.SelectedIndex < totalCount)
            {
                this.matrixIdNum.Value = this.matrixList.SelectedIndex;

                Matrix matrixData = null;

                if (this.matrixList.SelectedIndex < _scfile.GetMatrixs((int)this.transformIDNum.Value).Count)
                {
                    matrixData = _scfile.GetMatrixs((int)this.transformIDNum.Value)[this.matrixList.SelectedIndex];
                }
                else
                {
                    matrixData = addedMatrixes[(int)this.transformIDNum.Value][(totalCount - _scfile.GetMatrixs((int)this.transformIDNum.Value).Count) - 1];
                }

                this.numericUpDown1.Value = (decimal)matrixData.Elements[0];
                this.numericUpDown2.Value = (decimal)matrixData.Elements[1];
                this.numericUpDown3.Value = (decimal)matrixData.Elements[2];
                this.numericUpDown4.Value = (decimal)matrixData.Elements[3];
                this.numericUpDown5.Value = (decimal)matrixData.Elements[4];
                this.numericUpDown6.Value = (decimal)matrixData.Elements[5];
            }
        }

        private void selectId_Click(object sender, EventArgs e)
        {
            if (this.matrixIdNum.Value < _scfile.GetMatrixs((int)this.transformIDNum.Value).Count) // fix add saved ids too
            {
                this.matrixList.SelectedIndex = (int)this.matrixIdNum.Value;
                this.matrixList.SelectedIndexChanged += null;
            }
        }

        private void saveMatrixButton_Click(object sender, EventArgs e)
        {
            if (this.addMatrixState == AddState.Add)
            {
                this.numericUpDown1.Enabled = true;
                this.numericUpDown2.Enabled = true;
                this.numericUpDown3.Enabled = true;
                this.numericUpDown4.Enabled = true;
                this.numericUpDown5.Enabled = true;
                this.numericUpDown6.Enabled = true;

                this.saveMatrixButton.Text = "Confirm";

                this.addMatrixState = AddState.Confirm;
            }
            else if (this.addMatrixState == AddState.Confirm)
            {
                Matrix matrixData = new Matrix(
                    (float)this.numericUpDown1.Value,
                    (float)this.numericUpDown2.Value,
                    (float)this.numericUpDown3.Value,
                    (float)this.numericUpDown4.Value,
                    (float)this.numericUpDown5.Value,
                    (float)this.numericUpDown6.Value
                    );

                if (addedMatrixes.ContainsKey((int)this.transformIDNum.Value))
                {
                    addedMatrixes[(int)this.transformIDNum.Value].Add(matrixData);
                }
                else
                {
                    addedMatrixes.Add((int)this.transformIDNum.Value, new List<Matrix>() { matrixData });
                }

                this.numericUpDown1.Enabled = false;
                this.numericUpDown2.Enabled = false;
                this.numericUpDown3.Enabled = false;
                this.numericUpDown4.Enabled = false;
                this.numericUpDown5.Enabled = false;
                this.numericUpDown6.Enabled = false;

                this.saveMatrixButton.Text = "Add Matrix";

                this.matrixList.Items.Add(this.matrixList.Items.Count.ToString());

                this.matrixIdNum.Maximum = this.matrixList.Items.Count - 1;

                this.matrixList.SelectedIndex = (this.matrixList.Items.Count - 1);
                this.matrixList.SelectedIndexChanged += null;

                this.addMatrixState = AddState.Add;
            }
        }

        private void refreshListBox()
        {
            matrixList.Items.Clear();

            this.matrixList.Items.Clear();

            this.matrixIdNum.Maximum = (_scfile.GetMatrixs((int)this.transformIDNum.Value).Count - 1);

            for (int i = 0; i < _scfile.GetMatrixs((int)this.transformIDNum.Value).Count; i++)
            {
                this.matrixList.Items.Add(i.ToString());
            }

            if (addedMatrixes.ContainsKey((int)this.transformIDNum.Value))
            {
                this.matrixIdNum.Maximum += addedMatrixes[(int)this.transformIDNum.Value].Count;

                for (int i = 0; i < addedMatrixes[(int)this.transformIDNum.Value].Count; i++)
                {
                    this.matrixList.Items.Add((i + this.matrixList.Items.Count).ToString());
                }
            }
        }

        public Dictionary<int, List<Matrix>> getAddedMatrixes()
        {
            return this.addedMatrixes;
        }

        private void editMatrixes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (addedMatrixes.Count > 0)
            {
                DialogResult msgBox = MessageBox.Show("You have made changes to matrix array. Are you sure you want to save the changes?", "Confirm Matrixes Changes", MessageBoxButtons.YesNoCancel);

                if (msgBox != DialogResult.Cancel)
                {
                    this.DialogResult = msgBox;
                    return;
                }

                e.Cancel = true;
            }
        }

    }
}
