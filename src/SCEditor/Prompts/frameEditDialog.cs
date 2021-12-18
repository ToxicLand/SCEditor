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
    public partial class frameEditDialog : Form
    {
        private ScFile _scfile;

        private ScObject _data;
        private ushort[] _timelineArray;
        private ScObject[] _frames;

        private bool _isEdited;

        public ushort[] timelineArray => _timelineArray;
        public ScObject[] frames => _frames;

        public frameEditDialog(ScFile file, ScObject data)
        {
            InitializeComponent();

            _scfile = file;
            _data = data;
            _timelineArray = (ushort[])((MovieClip)_data).timelineArray.Clone();
            _frames = (ScObject[])((MovieClip)_data).Frames.ToArray().Clone();

            _isEdited = false;
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
                int frameIndex = getSelectedFrameIndex();
                int frameTimelineIndex = frameTimelineDataBox.SelectedIndex;
                int frameTimelineTypeIndex = (frameIndex * 3) + frameTimelineIndex;

                int oldValue = _timelineArray[frameTimelineTypeIndex];
                int newValue = int.Parse(dataTypeTextBox.Text); 

                if (frameTimelineIndex == 0 || frameTimelineIndex % 3 == 0)
                {
                    if (newValue >= ((MovieClip)_data).timelineChildrenId.Length)
                    {
                        MessageBox.Show($"Children with specified index {newValue} out of range.");
                        return;
                    }

                    DialogResult askChangeAll = MessageBox.Show($"Change children index with choosen childiren index {frameTimelineTypeIndex} or all? (Yes for specified - no for all)", "Replace All or Only Children", MessageBoxButtons.YesNo);
                    int currentChildrenId = _timelineArray[frameTimelineTypeIndex];

                    for (int i = 0; i < (_timelineArray.Length / 3); i++)
                    {
                        if (askChangeAll == DialogResult.Yes)
                        {
                            if (_timelineArray[i * 3] == currentChildrenId)
                            {
                                _timelineArray[i * 3] = (ushort)newValue;
                                continue;
                            }
                        }

                        _timelineArray[i * 3] = (ushort)newValue;
                    }

                    return;
                }
                else if (frameTimelineIndex == 1 || (frameTimelineIndex - 1) % 3 == 0)
                {
                    if (newValue >= _scfile.GetMatrixs(((MovieClip)_data)._transformStorageId).Count && newValue != 65535)
                    {
                        MessageBox.Show($"Matrix with specified index {newValue} not found.");
                        return;
                    }

                    DialogResult askChangeAll = MessageBox.Show($"Change matrix for only with specified children index {frameTimelineTypeIndex} or all? (Yes for specified - no for all)", "Replace All or Only Children", MessageBoxButtons.YesNo);
                    int currentChildrenId = _timelineArray[frameTimelineTypeIndex - 1];

                    for (int i = 0; i < (_timelineArray.Length / 3); i++)
                    {
                        if (askChangeAll == DialogResult.Yes)
                        {
                            if (_timelineArray[i * 3] == currentChildrenId)
                            {
                                _timelineArray[(i * 3) + 1] = (ushort)newValue;
                                continue;
                            }
                        }

                        _timelineArray[(i * 3) + 1] = (ushort)newValue;
                    }

                }
                else if (frameTimelineIndex == 2 || (frameTimelineIndex - 2) % 3 == 0)
                {
                    if (newValue >= _scfile.getColors(((MovieClip)_data)._transformStorageId).Count && newValue != 65535)
                    {
                        MessageBox.Show($"Color with specified index {newValue} not found.");
                        return;
                    }

                    DialogResult askChangeAll = MessageBox.Show($"Change color for only with specified children index {frameTimelineTypeIndex} or all? (Yes for specified - no for all)", "Replace All or Only Children", MessageBoxButtons.YesNo);
                    int currentChildrenId = _timelineArray[frameTimelineTypeIndex - 2];

                    for (int i = 0; i < (_timelineArray.Length / 3); i++)
                    {
                        if (askChangeAll == DialogResult.Yes)
                        {
                            if (_timelineArray[i * 3] == currentChildrenId)
                            {
                                _timelineArray[(i * 3) + 2] = (ushort)newValue;
                                continue;
                            }
                        }

                        _timelineArray[(i * 3) + 2] = (ushort)newValue;
                    }
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
                int frameIndex = getSelectedFrameIndex();
                int frameTimelineIndex = frameTimelineDataBox.SelectedIndex;
                int frameTimelineTypeIndex = (frameIndex * 3) + frameTimelineIndex;

                int newValue = int.Parse(dataTypeTextBox.Text);

                if (frameTimelineIndex == 0 || frameTimelineIndex % 3 == 0)
                {
                    if (newValue >= _data.Children.Count)
                    {
                        MessageBox.Show($"Shape with specified index {newValue} not found.");
                        return;
                    }
                    else
                    {
                        _timelineArray[frameTimelineTypeIndex] = (ushort)newValue;
                    }
                }
                else if (frameTimelineIndex == 1 || (frameTimelineIndex - 1) % 3 == 0)
                {
                    if (newValue >= _scfile.GetMatrixs(((MovieClip)_data)._transformStorageId).Count && newValue != 65535)
                    {
                        MessageBox.Show($"Matrix with specified index {newValue} not found.");
                        return;
                    }
                    else
                    {
                        _timelineArray[frameTimelineTypeIndex] = (ushort)newValue;
                    }
                }
                else if (frameTimelineIndex == 2 || (frameTimelineIndex - 2) % 3 == 0)
                {
                    if (newValue >= _scfile.getColors(((MovieClip)_data)._transformStorageId).Count && newValue != 65535)
                    {
                        MessageBox.Show($"Color with specified index {newValue} not found.");
                        return;
                    }
                    else
                    {
                        _timelineArray[frameTimelineTypeIndex] = (ushort)newValue;
                    }
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

        private void addFrameBeforeSelectedButton_Click(object sender, EventArgs e)
        {
            addFrame(0);
            refreshMenu();
        }

        private void addFrameAfterSelectedButton_Click(object sender, EventArgs e)
        {
            addFrame(1);
            refreshMenu();
        }

        private void deleteSelectedButton_Click(object sender, EventArgs e)
        {
            removeFrame();
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

        private void timelineArrayBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            addFrameAfterSelectedButton.Enabled = true;
            addFrameBeforeSelectedButton.Enabled = true;
            deleteSelectedButton.Enabled = true;
            dataTypeTextBox.Enabled = false;
            dataTypeEditSubmitButton.Enabled = false;
            dataTypeTextBox.Text = string.Empty;
            frameTimelineDataBox.Items.Clear();

            addItemDataToBox();
        }

        private void frameTimelineDataBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataTypeTextBox.Enabled = true;
            dataTypeEditSubmitButton.Enabled = true;

            int frameIndex = getSelectedFrameIndex();

            dataTypeTextBox.Text = _timelineArray[(frameIndex * 3) + frameTimelineDataBox.SelectedIndex].ToString();
        }

        private void refreshMenu()
        {
            addFrameAfterSelectedButton.Enabled = false;
            addFrameBeforeSelectedButton.Enabled = false;
            deleteSelectedButton.Enabled = false;
            frameTimelineDataBox.Items.Clear();
            FramesArrayBox.Items.Clear();
            dataTypeTextBox.Text = string.Empty;
            addItemsToBox();
        }

        public void addItemsToBox()
        {
            for (int i = 0; i < _frames.Length; i++)
            {
                FramesArrayBox.Items.Add( i + " | Frame - " + _frames[i].Id);
            }
        }

        public void addItemDataToBox()
        {
            string[] dataType = new string[3] { "Children Index: ", "Matrix Index: ", "Color Index: " };
            int timelineCount = _frames[FramesArrayBox.SelectedIndex].Id;

            int frameIndex = getSelectedFrameIndex();

            int index = 0;
            for (int i = 0; i < (timelineCount * 3); i++)
            {
                if (index == 3)
                    index = 0;

                frameTimelineDataBox.Items.Add(dataType[index] + _timelineArray[(frameIndex * 3) + i]);
                index++;
            }
        }

        private int addFrame(int beforeAfter)
        {
            int frameTimelineCount = 1;
            inputDataDialog frameTimelineCountDialog = new inputDataDialog(1);
            frameTimelineCountDialog.setLabelText("Frame Timeline Count");
            while (true)
            {
                if (frameTimelineCountDialog.ShowDialog() == DialogResult.OK)
                {
                    frameTimelineCount = frameTimelineCountDialog.inputTextBoxInt;

                    if (frameTimelineCount != 0)
                        break;

                    MessageBox.Show("Count can not be 0", "Error");
                }
            }

            List<ushort> newList = new List<ushort>(_timelineArray.ToList());
            ushort[] dataType = new ushort[3] { 0, 65535, 65535 };
            int dataTypeIndex = 0;
            int frameIndex = getSelectedFrameIndex();
            int frameEditIndex = beforeAfter == 0 ? frameIndex - (_frames[FramesArrayBox.SelectedIndex].Id * 3) : frameIndex;


            for (int i = 0; i < (frameTimelineCount * 3); i++)
            {
                if (dataTypeIndex == 3)
                    dataTypeIndex = 0;

                newList.Insert((frameEditIndex + i), dataType[dataTypeIndex]);
                dataTypeIndex++;
            }

            _timelineArray = newList.ToArray();

            MovieClipFrame data = new MovieClipFrame(_scfile);
            data.SetId((ushort)frameTimelineCount);
            data.setCustomAdded(true);

            List<ScObject> newFramesList = new List<ScObject>(_frames.ToList());
            newFramesList.Add(data);
            _frames = newFramesList.ToArray();
            _isEdited = true;

            return frameTimelineCount;
        }

        private void removeFrame()
        {
            int index = getSelectedFrameIndex();
            List<ushort> newList = new List<ushort>(_timelineArray.ToList());

            for (int i = 0; i < (_frames[FramesArrayBox.SelectedIndex].Id * 3); i++)
            {
                newList.RemoveAt((index * 3));
            }

            _timelineArray = newList.ToArray();

            List<ScObject> newFramesList = new List<ScObject>(_frames.ToList());
            newFramesList.RemoveAt(FramesArrayBox.SelectedIndex);
            _frames = newFramesList.ToArray();

            _isEdited = true;
        }

        private int getSelectedFrameIndex()
        {
            int frameIndex = 0;
            for (int i = 0; i < _frames.Count(); i++)
            {
                if (FramesArrayBox.SelectedIndex == i)
                    break;

                frameIndex += _frames[i].Id;
            }

            return frameIndex;
        }

        public void setTimelineArray(ushort[] array)
        {
            _timelineArray = array;
        }

    }
}
