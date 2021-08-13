using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCEditor.ScOld;

namespace SCEditor.Prompts
{
    public partial class editCharacter : Form
    {
        private ScObject _data;
        private ScFile _scFile;
        private bool zoomed;
        private float scaleFactor;
        public List<OriginalData> _originalData;
        private List<TreeNode> _checkedNodes;

        public editCharacter()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            _originalData = new List<OriginalData>();
            _checkedNodes = new List<TreeNode>();
            scaleFactor = 0.25F;
        }

        public void Render()
        {
            RenderingOptions options = new RenderingOptions()
            {
                ViewPolygons = false
            };

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject selectedData = (ScObject)treeView1.SelectedNode.Tag;

                if (selectedData.GetDataType() != 0 && selectedData.GetDataType() != 7)
                    return;

                pictureBox1.Image = selectedData.Render(options);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                if (selectedData.Children == null && selectedData.Bitmap != null)
                {
                    pictureBox1.Size = new Size(selectedData.Bitmap.Width, selectedData.Bitmap.Height);
                }
                else if (selectedData.Children != null)
                {
                }

                pictureBox1.Refresh();
            }
        }

        private void editData(editType type)
        {
            bool isEdited = true;

            if (_checkedNodes.Count > 0)
            {
                if (((ScObject)_checkedNodes[0].Tag).GetDataType() == 7)
                {
                    if (_checkedNodes.Count > 1)
                        throw new Exception("First node in _checkedNodes is export but the size is greater than 1");

                    foreach (Shape shapeData in ((Export)_checkedNodes[0].Tag).getChildren())
                    {
                        editShape(shapeData, type);
                    }
                }
                else if (_checkedNodes.Count >= 1)
                {
                    foreach (TreeNode node in _checkedNodes)
                    {
                        if (((ScObject)_checkedNodes[0].Tag).GetDataType() != 0)
                            throw new Exception($"CheckNode count is not 1 but selected nodes are not all shapes, {node.Text}");
                    }

                    foreach (TreeNode node in _checkedNodes)
                    {
                        editShape((Shape)node.Tag, type);
                    }
                }
            }
            else
            {
                ScObject selectedData = (ScObject)treeView1.SelectedNode.Tag;

                if (treeView1.SelectedNode?.Tag != null)
                {
                    if (selectedData.GetDataType() == 0)
                    {
                        editShape((Shape)selectedData, type);
                    }
                    else if (selectedData.GetDataType() == 7)
                    {
                        foreach (Shape shapeData in ((Export)selectedData).getChildren())
                        {
                            editShape(shapeData, type);
                        }
                    }
                }
                else
                {
                    isEdited = false;
                }
            }
            
            if (isEdited)
                Render();
        }

        private void editShape(Shape shapeData, editType type)
        {
            if (_originalData.FindIndex(data => data.shapeId == shapeData.Id) == -1)
            {
                int addShapeId = shapeData.Id;
                List<PointF[]> addPointF = new List<PointF[]>();

                foreach (ShapeChunk chunk in shapeData.GetChunks())
                {
                    PointF[] pointFData = new PointF[chunk.XY.Length];
                    for (int i = 0; i < chunk.XY.Length; i++)
                    {
                        pointFData[i].X = chunk.XY[i].X;
                        pointFData[i].Y = chunk.XY[i].Y;
                    }
                    addPointF.Add(pointFData);
                }

                OriginalData data = new OriginalData() { shapeId = addShapeId, chunkXYData = addPointF };
                _originalData.Add(data);
            }

            for (int s = 0; s < shapeData.GetChunks().Count; s++)
            {
                PointF[] xyData = ((ShapeChunk)shapeData.GetChunks()[s]).XY;

                for (int i = 0; i < xyData.Length; i++)
                {
                    switch (type)
                    {
                        case editType.PositionUp:
                            xyData[i].Y = xyData[i].Y > 0 ? xyData[i].Y - scaleFactor : ((Math.Abs(xyData[i].Y) + scaleFactor) * -1);
                            break;

                        case editType.PositionDown:
                            xyData[i].Y += scaleFactor;
                            break;

                        case editType.PositionLeft:
                            xyData[i].X = xyData[i].X > 0 ? xyData[i].X -= scaleFactor : ((Math.Abs(xyData[i].X) + scaleFactor) * -1);
                            break;

                        case editType.PositionRight:
                            xyData[i].X += scaleFactor;
                            break;

                        case editType.IncreaseSize:
                            xyData[i].X += xyData[i].X * scaleFactor;
                            xyData[i].Y += xyData[i].Y * scaleFactor;
                            break;

                        case editType.DecreaseSize:
                            xyData[i].X -= xyData[i].X * scaleFactor;
                            xyData[i].Y -= xyData[i].Y * scaleFactor;
                            break;
                    }
                }
            }
        }

        public void addData()
        {
            treeView1.Populate(new List<ScObject> { _data });
        }

        public void setScData(ScObject data, ScFile scfile)
        {
            _data = data;
            _scFile = scfile;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ScObject data = (ScObject)e.Node?.Tag;

            if (data != null)
            {
                if (data.GetDataType() == 0 || data.GetDataType() == 7)
                {
                    editCharacterButtonDown.Enabled = true;
                    editCharacterButtonUp.Enabled = true;
                    editCharacterButtonLeft.Enabled = true;
                    editCharacterButtonRight.Enabled = true;
                    editCharacterButtonSizeDecrease.Enabled = true;
                    editCharacterButtonSizeIncrease.Enabled = true;
                }
            }
            else
            {
                editCharacterButtonDown.Enabled = false;
                editCharacterButtonUp.Enabled = false;
                editCharacterButtonLeft.Enabled = false;
                editCharacterButtonRight.Enabled = false;
                editCharacterButtonSizeDecrease.Enabled = false;
                editCharacterButtonSizeIncrease.Enabled = false;
            }

            pictureBox1.Image = null;
            Render();
            zoomed = false;
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            ScObject node = (ScObject)treeView1.SelectedNode.Tag;

            if (node == null)
                return;

            int width = 0;
            int height = 0;

            if (node.Bitmap != null)
            {
                width = node.Bitmap.Width;
                height = node.Bitmap.Height;
            }
            else if (node.GetDataType() == 7 || node.GetDataType() == 0)
            {
                List<PointF> A = new List<PointF>();

                if (node.GetDataType() == 7)
                {
                    foreach (Shape s in node.Children)
                    {
                        PointF[] pointsXY = s.Children.SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                        A.AddRange(pointsXY.ToArray());
                    }
                }
                else
                {
                    PointF[] pointsXY = ((Shape)node).GetChunks().SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                    A.AddRange(pointsXY.ToArray());
                }

                var xyPath = new GraphicsPath();

                xyPath.AddPolygon(A.ToArray());

                var xyBound = Rectangle.Round(xyPath.GetBounds());

                width = xyBound.Width;
                height = xyBound.Height;
            }

            if (zoomed != true)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Size = new Size(width, height);
            }

            zoomed = true;

            double sf = 0.125;

            if (e.Delta > 0)
            {
                if ((width * 5.5) >= pictureBox1.Width)
                {
                    pictureBox1.Size = new Size((pictureBox1.Size.Width + (int)(pictureBox1.Size.Width * sf)), (pictureBox1.Size.Height + (int)(pictureBox1.Size.Height * sf)));
                    pictureBox1.Width = pictureBox1.Width + (int)(pictureBox1.Width * sf);
                    pictureBox1.Height = pictureBox1.Height + (int)(pictureBox1.Height * sf);
                }
            }
            else
            {
                if ((height / 5.5) <= pictureBox1.Width)
                {
                    pictureBox1.Size = new Size((pictureBox1.Size.Width - (int)(pictureBox1.Size.Width * sf)), (pictureBox1.Size.Height - (int)(pictureBox1.Size.Height * sf)));
                    pictureBox1.Width = pictureBox1.Width - (int)(pictureBox1.Width * sf);
                    pictureBox1.Height = pictureBox1.Height - (int)(pictureBox1.Height * sf);
                }
            }
        }

        private void editCharacterButtonSizeIncrease_Click(object sender, EventArgs e)
        {
            editData(editType.IncreaseSize);
        }

        private void editCharacterButtonSizeDecrease_Click(object sender, EventArgs e)
        {
            editData(editType.DecreaseSize);
        }

        private void editCharacterButtonUp_Click(object sender, EventArgs e)
        {
            editData(editType.PositionUp);
        }

        private void editCharacterButtonLeft_Click(object sender, EventArgs e)
        {
            editData(editType.PositionLeft);
        }

        private void editCharacterButtonRight_Click(object sender, EventArgs e)
        {
            editData(editType.PositionRight);
        }

        private void editCharacterButtonDown_Click(object sender, EventArgs e)
        {
            editData(editType.PositionDown);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_originalData.Count > 0)
            {
                DialogResult result = MessageBox.Show("Save your changes?", "Save Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private enum editType
        {
            IncreaseSize,
            DecreaseSize,
            PositionUp,
            PositionDown,
            PositionLeft,
            PositionRight
        }

        private void scaleFactorTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(scaleFactorTextBox.Text))
            {
                scaleFactor = float.Parse(scaleFactorTextBox.Text);
            }
            else
            {
                scaleFactorTextBox.Text = "0.25";
                scaleFactor = 0.25F;
            }
        }

        private void scaleFactorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(scaleFactorTextBox.Text))
                return;

            if (e.KeyData == Keys.OemPeriod)
            {
                if (scaleFactorTextBox.Text.Contains('.'))
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                else
                {
                    e.SuppressKeyPress = false;
                    return;
                }
            }

            if (e.KeyData != Keys.Back)
            {
                if (int.TryParse(Convert.ToString((char)e.KeyData), out int _))
                {
                    if (float.Parse(string.Format("{0}{1}", scaleFactor, int.Parse(Convert.ToString((char)e.KeyData)))) >= float.MaxValue)
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

        public void GetCheckedNodes(TreeNodeCollection nodes, List<TreeNode> list)
        {
            foreach (TreeNode aNode in nodes)
            {
                if (aNode.Checked)
                    list.Add(aNode);

                GetCheckedNodes(aNode.Nodes, list);
            }
        }

        private bool justChecked = false;
        private bool beforeChecked = false;

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (_checkedNodes.Contains(e.Node) && e.Node.Checked)
            {
                _checkedNodes.Remove(e.Node);
                beforeChecked = true;
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (beforeChecked == false)
            {
                if (justChecked == false)
                {
                    justChecked = true;

                    TreeNodeCollection nodeCollection = treeView1.Nodes;
                    ScObject nodeData = (ScObject)e.Node?.Tag;

                    if (nodeData != null)
                    {
                        if (!_checkedNodes.Contains(e.Node))
                        {
                            _checkedNodes.Add(e.Node);
                        }

                        if (nodeData.GetDataType() == 7)
                        {
                            removeChecked(nodeCollection, 0);
                            justChecked = false;
                        }
                        else if (nodeData.GetDataType() == 0)
                        {
                            removeChecked(nodeCollection, 7);
                            justChecked = false;
                        }
                    }
                }
            }
            beforeChecked = false;
        }

        private void removeChecked(TreeNodeCollection nodes, int dataType)
        {
            if (_checkedNodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    if ((ScObject)node?.Tag != null)
                    {
                        if (((ScObject)node?.Tag).GetDataType() == dataType)
                        {
                            if (_checkedNodes.Contains(node))
                                _checkedNodes.Remove(node);

                            node.Checked = false;
                        }

                    }

                    if (node.Nodes.Count > 0 || ((ScObject)node?.Tag).GetDataType() != 0)
                        removeChecked(node.Nodes, dataType);
                }
            }
        }

        private void checkedNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node == null)
                    continue;

                if (node.Checked == true)
                {
                    if (!_checkedNodes.Contains(node))
                        _checkedNodes.Add(node);
                }

                if (node.Nodes.Count > 0)
                    checkedNodes(node.Nodes);
            }
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            if (_originalData.Count > 0)
            {
                if (_checkedNodes.Count > 0)
                {
                    if (((ScObject)_checkedNodes[0].Tag).GetDataType() == 7)
                    {
                        if (_checkedNodes.Count > 1)
                            throw new Exception("First node in _checkedNodes is export but the size is greater than 1");

                        foreach (Shape shapeData in ((Export)_checkedNodes[0].Tag).getChildren())
                        {
                            revertShape(shapeData);
                        }
                    }
                    else if (_checkedNodes.Count >= 1)
                    {
                        foreach (TreeNode node in _checkedNodes)
                        {
                            if (((ScObject)_checkedNodes[0].Tag).GetDataType() != 0)
                                throw new Exception($"CheckNode count is not 1 but selected nodes are not all shapes, {node.Text}");
                        }

                        foreach (TreeNode node in _checkedNodes)
                        {
                            revertShape((Shape)node.Tag);
                        }
                    }
                }
                else
                {
                    if (treeView1.SelectedNode?.Tag != null)
                    {
                        ScObject selectedData = (ScObject)treeView1.SelectedNode.Tag;

                        if (selectedData != null)
                        {
                            if (selectedData.GetDataType() == 0)
                            {
                                revertShape((Shape)selectedData);
                            }
                            else if (selectedData.GetDataType() == 7)
                            {
                                foreach (Shape shapeData in ((Export)selectedData).getChildren())
                                {
                                    revertShape(shapeData);
                                }
                            }
                        }
                        
                    }
                }

                Render();
            }
        }

        public void revertShape(Shape shapeData)
        {
            int index = _originalData.FindIndex(data => data.shapeId == shapeData.Id);

            if (index == -1)
                return;

            int originalIndex = _scFile.GetShapes().FindIndex(Shape => Shape.Id == _originalData[index].shapeId);

            if (originalIndex == -1)
                throw new Exception("originalShapeData index wrong");

            Shape originalShapeData = (Shape)_scFile.GetShapes()[originalIndex];

            for (int i = 0; i < originalShapeData.GetChunks().Count; i++)
            {
                ((ShapeChunk)originalShapeData.GetChunks()[i]).SetXY(_originalData[index].chunkXYData[i]);
            }

            treeView1.SelectedNode.ForeColor = Color.White;
            _originalData.RemoveAt(index);
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.Text != "Exports")
            {
                if ((ScObject)e.Node?.Tag != null)
                {
                    int type = ((ScObject)e.Node?.Tag).GetDataType();
                    if (type == 0 || type == 7)
                    {
                        e.Graphics.DrawString(e.Node.Text, e.Node.TreeView.Font, Brushes.White, e.Node.Bounds.X, e.Node.Bounds.Y);
                        return;
                    }   
                }
            }

            HideCheckBox(e.Node);
            e.DrawDefault = true;
        }

        private void HideCheckBox(TreeNode node)
        {
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvi));
            Marshal.StructureToPtr(tvi, lparam, false);
            SendMessage(node.TreeView.Handle, TVM_SETITEM, IntPtr.Zero, lparam);
        }

        private void editCharacter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                if (_originalData.Count > 0)
                {
                    DialogResult result = MessageBox.Show("There are changes not yet saved! Closing will revert any changes made. Proceed to close?", "Changes not saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

                    e.Cancel = (result == DialogResult.No);
                }
            }
        }
    }

    public class OriginalData
    {
        public int shapeId { get; set; }
        public List<PointF[]> chunkXYData { get; set; }
    }
}
