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
        private List<Matrix> matrixes;
        private List<Matrix> newMatrixes;
        public List<Matrix> addedMatrixes => newMatrixes;

        public editMatrixes(List<Matrix> mL)
        {
            InitializeComponent();
            matrixes = mL;
            newMatrixes = new List<Matrix>();

            populateMatrixList();
        }

        private void populateMatrixList()
        {
            matrixList.Items.Clear();

            for (int i = 0; i < matrixes.Count; i++)
            {
                matrixList.Items.Add(i.ToString());
            }
        }

        private void matrixList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Matrix data = matrixList.SelectedIndex + 1 <= matrixes.Count ? matrixes[matrixList.SelectedIndex] : newMatrixes[(matrixList.SelectedIndex - matrixes.Count)];
            textBox1.Text = data.Elements[0].ToString();
            textBox2.Text = data.Elements[1].ToString();
            textBox3.Text = data.Elements[2].ToString();
            textBox4.Text = data.Elements[3].ToString();
            textBox5.Text = data.Elements[4].ToString();
            textBox6.Text = data.Elements[5].ToString();
        }

        private void addMatrixButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;

            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
        }

        private void saveMatrixButton_Click(object sender, EventArgs e)
        {
            try
            {
                float value1 = float.Parse(textBox1.Text);
                float value2 = float.Parse(textBox2.Text);
                float value3 = float.Parse(textBox3.Text);
                float value4 = float.Parse(textBox4.Text);
                float value5 = float.Parse(textBox5.Text);
                float value6 = float.Parse(textBox6.Text);

                Matrix newMatrix = new Matrix(value1, value2, value3, value4, value5, value6);

                newMatrixes.Add(newMatrix);

                MessageBox.Show($"Matrix with ID {matrixes.Count + newMatrixes.Count - 1} added", "New Matrix Added", MessageBoxButtons.OK);

                refreshMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception", ex.ToString());
            }
        }

        private void refreshMenu()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox6.Text = string.Empty;

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;

            matrixList.Items.Clear();

            for (int i = 0; i < matrixes.Count; i++)
            {
                matrixList.Items.Add(i.ToString());
            }

            if (newMatrixes.Count > 0)
            {
                for (int i = 0; i < newMatrixes.Count; i++)
                {
                    matrixList.Items.Add((i + matrixes.Count).ToString());
                }
            }

            saveMatrixButton.Enabled = false;
        }

        private void editMatrixes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (newMatrixes.Count > 0)
            {
                DialogResult closing = MessageBox.Show("You have made changes. Save them?", $"{newMatrixes.Count} newly added Matrixes", MessageBoxButtons.YesNoCancel);

                if (closing == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    this.DialogResult = closing;
                }
            }
        }
    }
}
