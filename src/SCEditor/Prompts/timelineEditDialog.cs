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
    public partial class timelineEditDialog : Form
    {
        private ushort[] _timelineArray;
        private ScFile _scfile;
        private ScObject _data;
        private ScObject[] _frames;
        private bool _isEdited;
        public ushort[] timelineArray => _timelineArray;
        public ScObject[] frames => _frames;
        public timelineEditDialog(ScFile file, ScObject data)
        {
            InitializeComponent();

            _scfile = file;
            _data = data;
            _timelineArray = (ushort[])((MovieClip)_data).timelineArray.Clone();
            _frames = (ScObject[])((MovieClip)_data).Frames.ToArray().Clone();
            _isEdited = false;
        }

        public void addItemsToBox()
        {
            for (int i = 0; i < _timelineArray.Length / 6; i++)
            {
                timelineArrayBox.Items.Add(i + " | Shape " + _timelineArray[i * 6] + "," + _timelineArray[(i * 6) + 3]);
            }

        }

        public void addItemDataToBox()
        {
            string[] dataType = new string[3] { "Shape ID: ", "Matrix ID: ", "ColorT ID: " };

            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                if (index == 3)
                    index = 0;

                timelineDataBox.Items.Add(dataType[index] + _timelineArray[(timelineArrayBox.SelectedIndex * 6) + i]);
                index++;
            }
        }

        public void setTimelineArray(ushort[] array)
        {
            _timelineArray = array;
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

        private void timelineArrayBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addFrameAfterSelectedButton.Enabled = true;
            addFrameBeforeSelectedButton.Enabled = true;
            deleteSelectedButton.Enabled = true;
            dataTypeTextBox.Enabled = false;
            dataTypeEditSubmitButton.Enabled = false;
            dataTypeTextBox.Text = string.Empty;
            timelineDataBox.Items.Clear();

            addItemDataToBox();
        }

        private void timelineDataBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataTypeTextBox.Enabled = true;
            dataTypeEditSubmitButton.Enabled = true;

            dataTypeTextBox.Text = _timelineArray[(timelineArrayBox.SelectedIndex * 6) + timelineDataBox.SelectedIndex].ToString();
        }

        private void dataTypeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            int data = 0;
            if (e.KeyData != Keys.Back)
            {
                if (int.TryParse(Convert.ToString((char)e.KeyData), out int _))
                {
                    if (Convert.ToInt64(string.Format("{0}{1}", data, int.Parse(Convert.ToString((char)e.KeyData)))) >= Int32.MaxValue)
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

        private void changeAllButton_Click(object sender, EventArgs e)
        {
            DialogResult ask = MessageBox.Show("This is not to be used on production.", "Experimental Feature", MessageBoxButtons.OKCancel);

            if (!string.IsNullOrEmpty(dataTypeTextBox.Text) && ask == DialogResult.OK)
            {
                int newValue = int.Parse(dataTypeTextBox.Text);

                if (timelineDataBox.SelectedIndex == 0 || timelineDataBox.SelectedIndex == 3)
                {
                    // TODO
                    return;
                }
                else if (timelineDataBox.SelectedIndex == 1 || timelineDataBox.SelectedIndex == 4)
                {
                    if (newValue >= _scfile.GetMatrixs().Count && newValue != 65535)
                    {
                        MessageBox.Show($"Matrix with specified ID {newValue} not found.");
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < (_timelineArray.Length / 6); i++)
                        {
                            _timelineArray[(i * 6) + 1] = (ushort)newValue;
                        }
                    }
                }
                else if (timelineDataBox.SelectedIndex == 2 || timelineDataBox.SelectedIndex == 5)
                {
                    // TODO
                    return;
                }
                else
                {
                    throw new Exception("Not possible");
                }

                _isEdited = true;
                refreshMenu();
            }
        }

        private void dataTypeEditSubmitButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(dataTypeTextBox.Text))
            {
                int newValue = int.Parse(dataTypeTextBox.Text);

                if (timelineDataBox.SelectedIndex == 0 || timelineDataBox.SelectedIndex == 3)
                {
                    if (newValue >= _data.Children.Count)
                    {
                        MessageBox.Show($"Shape with specified ID {newValue} not found.");
                        return;
                    }
                    else
                    {
                        _timelineArray[(timelineArrayBox.SelectedIndex * 6) + timelineDataBox.SelectedIndex] = (ushort)newValue;
                    }
                }
                else if (timelineDataBox.SelectedIndex == 1 || timelineDataBox.SelectedIndex == 4)
                {
                    if (newValue >= _scfile.GetMatrixs().Count && newValue != 65535)
                    {
                        MessageBox.Show($"Matrix with specified ID {newValue} not found.");
                        return;
                    }
                    else
                    {
                        _timelineArray[(timelineArrayBox.SelectedIndex * 6) + timelineDataBox.SelectedIndex] = (ushort)newValue;
                    }
                }
                else if (timelineDataBox.SelectedIndex == 2 || timelineDataBox.SelectedIndex == 5)
                {
                    // TODO
                    return;
                }
                else
                {
                    throw new Exception("Not possible");
                }

                _isEdited = true;
                refreshMenu();
            }
        }

        private void refreshMenu()
        {
            addFrameAfterSelectedButton.Enabled = false;
            addFrameBeforeSelectedButton.Enabled = false;
            deleteSelectedButton.Enabled = false;
            timelineDataBox.Items.Clear();
            timelineArrayBox.Items.Clear();
            dataTypeTextBox.Text = string.Empty;
            addItemsToBox();
        }

        private void timelineEditDialog_FormClosing(object sender, FormClosingEventArgs e)
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

        private void addFrameBeforeSelectedButton_Click(object sender, EventArgs e)
        {
            int index = timelineArrayBox.SelectedIndex;

            List<ushort> newList = new List<ushort>(_timelineArray.ToList());

            ushort[] data = new ushort[6] { 0, 65535, 65535, 0, 65535, 65535 };

            for (int i = 0; i < 6; i++)
            {
                newList.Insert(((index * 6) + i), data[i]);
            }

            _timelineArray = newList.ToArray();

            addFrame();
            refreshMenu();
        }

        private void addFrameAfterSelectedButton_Click(object sender, EventArgs e)
        {
            int index = timelineArrayBox.SelectedIndex;

            List<ushort> newList = new List<ushort>(_timelineArray.ToList());

            ushort[] data = new ushort[6] { 0, 65535, 65535, 0, 65535, 65535 };

            for (int i = 0; i < 6; i++)
            {
                newList.Insert((((index + 1) * 6) + i), data[i]);
            }

            _timelineArray = newList.ToArray();

            addFrame();
            refreshMenu();
        }

        private void addFrame()
        {
            if (_timelineArray.Length / 6 != (_frames.Length + 1))
                throw new Exception("unexpected");

            MovieClipFrame data = new MovieClipFrame(_scfile);
            data.SetId(2);
            data.setCustomAdded(true);

            List<ScObject> newFramesList = new List<ScObject>(_frames.ToList());
            newFramesList.Add(data);
            _frames = newFramesList.ToArray();
            _isEdited = true;
        }

        private void deleteSelectedButton_Click(object sender, EventArgs e)
        {
            int index = timelineArrayBox.SelectedIndex;
            List<ushort> newList = new List<ushort>(_timelineArray.ToList());

            for (int i = 0; i < 6; i++)
            {
                newList.RemoveAt((index * 6));
            }

            _timelineArray = newList.ToArray();

            if (_timelineArray.Length / 6 != (_frames.Length - 1))
                throw new Exception("unexpected");

            List<ScObject> newFramesList = new List<ScObject>(_frames.ToList());
            newFramesList.RemoveAt(0);
            _frames = newFramesList.ToArray();

            _isEdited = true;
            refreshMenu();
        }

        private void replaceMultipleShapeDataButton_Click(object sender, EventArgs e)
        {
            int changes = 0;
            
            using (inputDataDialog form = new inputDataDialog(1))
            {
                form.setLabelText("Enter Shape ID:");

                if (form.ShowDialog() == DialogResult.OK)
                {
                    int? shapeIndex = form.inputTextBoxInt;

                    if (shapeIndex != null)
                    {
                        shapeIndex = form.inputTextBoxInt;

                        if (shapeIndex >= _data.Children.Count)
                        {
                            MessageBox.Show($"Shape with specified ID {shapeIndex} not found.");
                            return;
                        }

                        using (inputDataDialog matrixForm = new inputDataDialog(1))
                        {
                            matrixForm.setLabelText("Enter Matrix ID:");

                            if (matrixForm.ShowDialog() == DialogResult.OK)
                            {
                                int? matrixId = matrixForm.inputTextBoxInt;

                                if (matrixId != null)
                                {
                                    if (matrixId >= _scfile.GetMatrixs().Count)
                                    {
                                        MessageBox.Show($"Matrix with specified ID {matrixId} not found.");
                                        return;
                                    }

                                    for (int i = 0; i < _timelineArray.Length / 3; i++)
                                    {
                                        if (_timelineArray[i * 3] == shapeIndex)
                                        {
                                            _timelineArray[(i * 3) + 1] = (ushort)matrixId;

                                            changes++;
                                        }
                                    }

                                    if (changes > 0)
                                        MessageBox.Show($"Successfully changed {changes} matrix with id {matrixId} for shape index {shapeIndex}.", "Matrix Replaced");
                                    else
                                        MessageBox.Show($"No changes for shape index {shapeIndex}.", "No changes");
                                }  
                            }
                        }
                    }
                }
            } 
        }

        
    }
}
