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
    public partial class editChildrenData : Form
    {
        private ScFile _scFile;

        private ScObject _data;
        private List<ushort> _childrenIds;
        private List<string> _childrenNames;
        private List<byte> _flags;
        public ushort[] ChildrenIds => _childrenIds.ToArray();
        public string[] ChildrenNames => _childrenNames.ToArray();
        public byte[] Flags => _flags.ToArray();

        private bool _isEdited;
        public editChildrenData(ScFile scs, ScObject scData)
        {
            InitializeComponent();

            _scFile = scs;
            _data = scData;

            _childrenIds = ((ushort[])((MovieClip)_data).timelineChildrenId.Clone()).ToList();
            _childrenNames = ((string[])((MovieClip)_data).timelineChildrenNames.Clone()).ToList();
            _flags = ((byte[])((MovieClip)_data).flags.Clone()).ToList();

            addDataToItems();

            _isEdited = false;
        }

        private void addDataToItems()
        {
            for (int i = 0; i < _childrenIds.Count; i++)
            {
                childrenIdListBox.Items.Add(_childrenIds[i]);
                childrenNameListBox.Items.Add((_childrenNames[i] == null ? "" : _childrenNames[i]));
            }
        }

        private void addChildrenBefore_Click(object sender, EventArgs e)
        {
            addChildren(0);
        }

        private void addChildrenAfter_Click(object sender, EventArgs e)
        {
            addChildren(1);
        }

        private void addChildren(int beforeAfter)
        {
            int currentIndex = childrenIdListBox.SelectedIndex;

            ushort newChildId = ushort.Parse(childrenIdTextBox.Text);
            string newChildName = childrenNameTextBox.Text;

            if (string.IsNullOrEmpty(newChildName))
                newChildName = null;

            if (_scFile.GetShapes().FindIndex(sco => sco.Id == newChildId) == -1
                && _scFile.GetMovieClips().FindIndex(sco => sco.Id == newChildId) == -1
                && _scFile.getTextFields().FindIndex(sco => sco.Id == newChildId) == -1)
            {
                MessageBox.Show($"Children ID {newChildId} you are trying to add does not exist.");
                return;
            }

            int newIndex = beforeAfter == 0 ? currentIndex : currentIndex + 1;

            _childrenIds.Insert(newIndex, newChildId);
            _childrenNames.Insert(newIndex, newChildName);
            _flags.Insert(newIndex, 0);

            _isEdited = true;
            refreshMenu();
        }

        private void changeDataButton_Click(object sender, EventArgs e)
        {
            int currentIndex = childrenIdListBox.SelectedIndex;

            ushort newChildId = ushort.Parse(childrenIdTextBox.Text);
            string newChildName = childrenNameTextBox.Text;

            if (string.IsNullOrEmpty(newChildName))
                newChildName = null;

            if (_scFile.GetShapes().FindIndex(sco => sco.Id == newChildId) == -1
                && _scFile.GetMovieClips().FindIndex(sco => sco.Id == newChildId) == -1
                && _scFile.getTextFields().FindIndex(sco => sco.Id == newChildId) == -1)
            {
                MessageBox.Show($"Children ID {newChildId} you are trying to edit does not exist.");
                return;
            }

            _childrenIds[currentIndex] = newChildId;
            _childrenNames[currentIndex] = newChildName;

            _isEdited = true;
            refreshMenu();
        }

        private void deleteChildrenButtom_Click(object sender, EventArgs e)
        {
            int currentIndex = childrenIdListBox.SelectedIndex;

            _childrenIds.RemoveAt(currentIndex);
            _childrenNames.RemoveAt(currentIndex);
            _flags.RemoveAt(currentIndex);

            _isEdited = true;
            refreshMenu();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_isEdited)
            {
                DialogResult saveChanges = MessageBox.Show("Would you like to save changes?", "Save Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (saveChanges == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void childrenIdListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentIndexChanged(0);
        }

        private void childrenNameListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentIndexChanged(1);
        }

        private void currentIndexChanged(int listBoxType)
        {
            int newIndex = listBoxType == 0 ? childrenIdListBox.SelectedIndex : childrenNameListBox.SelectedIndex;

            childrenIdListBox.SelectedIndex = newIndex;
            childrenNameListBox.SelectedIndex = newIndex;

            childrenIdTextBox.Enabled = true;
            childrenNameTextBox.Enabled = true;
            addChildrenBefore.Enabled = true;
            addChildrenAfter.Enabled = true;
            deleteChildrenButtom.Enabled = true;

            childrenIdTextBox.Text = _childrenIds[newIndex].ToString();
            childrenNameTextBox.Text = _childrenNames[newIndex] == null ? "" : _childrenNames[newIndex];
        }

        private void refreshMenu()
        {
            if (_isEdited == true)
                saveButton.Enabled = true;

            childrenIdTextBox.Enabled = false;
            childrenNameTextBox.Enabled = false;
            addChildrenBefore.Enabled = false;
            addChildrenAfter.Enabled = false;
            deleteChildrenButtom.Enabled = false;
            changeDataButton.Enabled = false;

            childrenIdTextBox.Text = "";
            childrenNameTextBox.Text = "";

            childrenIdListBox.Items.Clear();
            childrenNameListBox.Items.Clear();

            addDataToItems();

            childrenNameListBox.Refresh();
            childrenIdListBox.Refresh();
        }

        private void childrenIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            int data = 0;
            if (e.KeyData != Keys.Back)
            {
                if (ushort.TryParse(Convert.ToString((char)e.KeyData), out ushort _))
                {
                    if (Convert.ToInt64(string.Format("{0}{1}", data, ushort.Parse(Convert.ToString((char)e.KeyData)))) >= ushort.MaxValue)
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

        private void editChildrenData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isEdited && this.DialogResult != DialogResult.OK)
            {
                DialogResult closeForm = MessageBox.Show("There are unsaved changes. Are you sure you want to close without saving?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (closeForm != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void childrenNameTextBox_TextChanged(object sender, EventArgs e)
        {
            changeDataButton.Enabled = true;
        }

        private void childrenIdTextBox_TextChanged(object sender, EventArgs e)
        {
            changeDataButton.Enabled = true;
        }
    }
}
