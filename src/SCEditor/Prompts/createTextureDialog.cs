using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCEditor
{
    public partial class createTextureDialog : Form
    {
        public string textureImageType { get; set; }
        public int textureWidth { get; set; }
        public int textureHeight { get; set; }
        public createTextureDialog()
        {
            InitializeComponent();
        }

        private void textureTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            textureImageType = textureTypeComboBox.SelectedItem.ToString();
        }

        private void textureWidthTextBox_TextChanged(object sender, EventArgs e)
        {
            textureWidth = int.Parse(textureWidthTextBox.Text);
        }

        private void textureHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            textureHeight = int.Parse(textureHeightTextBox.Text);
        }

        private void textureWidthTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Back)
            {
                if (int.TryParse(Convert.ToString((char)e.KeyData), out int _))
                {
                    if (Convert.ToInt64(string.Format("{0}{1}", textureWidth, int.Parse(Convert.ToString((char)e.KeyData)))) >= Int32.MaxValue)
                    {
                        e.SuppressKeyPress = true;
                    }
                }
                else
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void textureHeightTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Back)
            {
                if (int.TryParse(Convert.ToString((char)e.KeyData), out int _))
                {
                    if (Convert.ToInt64(string.Format("{0}{1}", textureHeight,int.Parse(Convert.ToString((char)e.KeyData)))) >= Int32.MaxValue)
                    {
                        e.SuppressKeyPress = true;
                    }
                }
                else
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void createTextureButton_Click(object sender, EventArgs e)
        {
            string error = "";
            if (string.IsNullOrWhiteSpace(textureImageType))
            {
                error = "Please select texture type.\n";
            }

            if (textureWidth == 0 || textureWidth < 0 && textureWidth !> 0)
            {
                error = error + "Please enter a valid width.";
            }

            if (textureHeight == 0 || textureHeight < 0 && textureHeight !> 0)
            {
                error = error + "\nPlease enter a valid height.";
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                MessageBox.Show(error, "Invalid or Missing Input");
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
            
        }
    }
}
