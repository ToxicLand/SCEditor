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
    public partial class scMergeSelection : Form
    {
        private List<ScObject> exportsToList;
        public CheckedListBox.CheckedItemCollection checkedExports => exportsListBox.CheckedItems;
        private bool allChecked;
        public bool newTextureChecked;
        public float scaleFactor { get; private set; }

        public scMergeSelection(List<ScObject> exportsList)
        {
            InitializeComponent();

            if (MessageBox.Show("Sort exports by name in ascending order?", "Sort Export by Name", MessageBoxButtons.YesNo) == DialogResult.Yes)
                exportsToList = exportsList.OrderBy(ex => ex.GetName()).ToList();
            else
                exportsToList = exportsList;

            allChecked = false;
            newTextureChecked = false;
            populateListBox();
        }

        public class exportItemClass
        {
            public string exportName { get; set; }
            public object exportData { get; set; }
        }

        private void populateListBox()
        {
            bool afterThisChecked = false;

            foreach (object export in exportsToList)
            {
                if (((Export)export).GetName().StartsWith("bb_attack_booster_lvl1") && true)
                    afterThisChecked = true;

                exportsListBox.Items.Add(new exportItemClass { exportName = ((Export)export).GetName(), exportData = export }, afterThisChecked);
            }  

            exportsListBox.DisplayMember = "exportName";
            exportsListBox.ValueMember = "exportData";

            askForExportList();

            exportsListBox.Refresh();
        }

        private void askForExportList()
        {
            inputDataDialog toImportExports = new inputDataDialog(0);
            toImportExports.setLabelText("Exports Separated with (;):");

            while (true)
            {
                if (toImportExports.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(toImportExports.inputTextBoxString))
                        break;

                    string[] toImport = toImportExports.inputTextBoxString.Split(';');

                    if (toImport.Length == 0)
                    {
                        DialogResult msgBox = MessageBox.Show("Input exports count is 0. Please make sure your exports are separated by ;\nIf you wish to cancel, press cancel.", "Invalid Input Data", MessageBoxButtons.RetryCancel);

                        if (msgBox == DialogResult.Cancel)
                            break;
                        else if (msgBox == DialogResult.Retry)
                            continue;
                    }
                    else
                    {
                        for (int i = 0; i < exportsListBox.Items.Count; i++)
                        {
                            if (toImport.Contains(((exportItemClass)exportsListBox.Items[i]).exportName))
                                exportsListBox.SetItemChecked(i, true);
                        }

                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (exportsListBox.CheckedItems.Count > 0)
            {
                DialogResult confirm = MessageBox.Show("Are you sure you want to import the checked exports?","Confirm", MessageBoxButtons.YesNoCancel);

                if (confirm != DialogResult.Cancel)
                {
                    if (confirm == DialogResult.Yes)
                        this.DialogResult = DialogResult.Yes;

                    this.Close();
                }
            }
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < exportsListBox.Items.Count; i++)
                exportsListBox.SetItemChecked(i, !allChecked);

            allChecked = !allChecked;
        }

        private void newTextureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            newTextureChecked = !newTextureChecked;
        }

        private void scaleFactorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Back)
            {
                if (scaleFactorTextBox.Text.Contains(".") && e.KeyData == Keys.OemPeriod)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                else if (e.KeyData != Keys.OemPeriod)
                {
                    if (float.TryParse(Convert.ToString((char)e.KeyData), out float _))
                    {
                        if ((float)Convert.ToDouble(string.Format("{0}{1}", scaleFactor, float.Parse(Convert.ToString((char)e.KeyData)))) >= float.MaxValue)
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
        }

        private void scaleFactorTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(scaleFactorTextBox.Text))
                scaleFactor = float.Parse(scaleFactorTextBox.Text);
            else
                scaleFactor = 0;
        }
    }
}
