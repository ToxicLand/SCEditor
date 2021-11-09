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

        public scMergeSelection(List<ScObject> exportsList)
        {
            InitializeComponent();
            exportsToList = exportsList;
            populateListBox();
        }

        public class exportItemClass
        {
            public string exportName { get; set; }
            public object exportData { get; set; }
        }

        private void populateListBox()
        {
            foreach (object export in exportsToList)
            {
                exportsListBox.Items.Add(new exportItemClass { exportName = ((Export)export).GetName(), exportData = export }, false);
            }

            exportsListBox.DisplayMember = "exportName";
            exportsListBox.ValueMember = "exportData";

            exportsListBox.Refresh();
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
    }
}
