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
    public partial class createExportDialog : Form
    {
        public int selectedTexture { get; set; }
        public string selectedFile { get; set; }
        

        public createExportDialog()
        {
            InitializeComponent();
        }

        public void addTextureToBox(object[] list)
        {
            this.selectTextureComboBox.Items.AddRange(list);
        }

        private void importExportsButton_Click(object sender, EventArgs e)
        {
            string error = "";
            if (this.selectTextureComboBox.SelectedItem == null)
            {
                error = "Please select a texture.\n";
            }

            if (string.IsNullOrWhiteSpace(selectedFile))
            {
                error = error + "JSON File not selected.";
            }

            if (string.IsNullOrEmpty(error))
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(error, "Invalid or Missing Input");
            }
        }

        private void selectTextureComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            selectedTexture = this.selectTextureComboBox.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON File (*.json)|*.json|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedFile = ofd.FileName;
                }
            }
        }
    }
}
