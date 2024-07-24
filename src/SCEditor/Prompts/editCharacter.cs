using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCEditor.ScOld;
using static SCEditor.ScOld.MovieClip;

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
        private bool _saveAsMatrix = false;
        private CancellationTokenSource _renderingCancelToken;
        private MovieClipState animationState { get; set; }
        private Task animationTask { get; set; }
        private bool singleFrame { get; set; }
        private int singleFrameIndex { get; set; } = 0;


        public bool saveAsMatrix => _saveAsMatrix;

        public editCharacter()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            _originalData = new List<OriginalData>();
            _checkedNodes = new List<TreeNode>();
            _renderingCancelToken = new CancellationTokenSource();
            scaleFactor = 0.25F;
            singleFrame = false;
        }

        public void Render()
        {
            if (animationTask != null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (!animationTask.IsCompleted && !(animationTask.Status == TaskStatus.WaitingForActivation || animationTask.Status == TaskStatus.WaitingToRun))
                {
                    if (sw.Elapsed.TotalSeconds > 30)
                    {
                        Console.WriteLine("Waited animation to stop for 30 seconds. Force stopping.");
                        break;
                    }

                    continue;
                }
            }

            stopAnimationPlaying(true);

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject selectedData = (ScObject)treeView1.SelectedNode.Tag;

                if (selectedData.GetDataType() != 0 && selectedData.GetDataType() != 7 && selectedData.GetDataType() != 1)
                    return;

                RenderingOptions options = new RenderingOptions()
                {
                    ViewPolygons = false
                };

                if (selectedData.GetDataType() == 7)
                    selectedData = selectedData.GetDataObject();

                if (selectedData.GetDataType() == 1)
                {
                    MovieClip mvData = (MovieClip)selectedData;

                    if (mvData.Frames.Count > 0)
                    {
                        Dictionary<ushort, Matrix> editedMatrix = new Dictionary<ushort, Matrix>();
                        if (saveAsMatrix)
                        {
                            foreach (OriginalData data in _originalData)
                            {
                                editedMatrix.Add(data.childrenId, data.matrixData);
                            }
                        }

                        options.editedMatrixPerChildren = editedMatrix;

                        _renderingCancelToken = new CancellationTokenSource();

                        ((MovieClip)mvData)._lastPlayedFrame = 0;
                        animationTask = Task.Run(() => renderFrame(mvData, options, _renderingCancelToken));
                    }

                    return;
                }
                else
                {
                    int idx = _originalData.FindIndex(data => data.childrenId == selectedData.Id);
                    if (idx != -1)
                    {
                        options.MatrixData = _originalData[idx].matrixData;
                    }

                    Bitmap shapeBMP = selectedData.Render(options);

                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox1.Size = new Size(shapeBMP.Width, shapeBMP.Height);
                    pictureBox1.Image = shapeBMP;

                    pictureBox1.Refresh();

                    shapeBMP.Dispose();
                    shapeBMP = null;
                }
            }
        }

        private async Task renderFrame(ScObject data, RenderingOptions rOptions, CancellationTokenSource token)
        {
            try
            {
                int totalFrameTimelineCount = 0;
                foreach (MovieClipFrame frame in ((MovieClip)data).GetFrames())
                {
                    totalFrameTimelineCount += (frame.Id * 3);
                }

                if (((MovieClip)data).timelineArray.Length % 3 != 0 || ((MovieClip)data).timelineArray.Length != totalFrameTimelineCount)
                {
                    MessageBox.Show("MoveClip timeline array length is not set equal to total frames count.");
                    return;
                }

                ((MovieClip)data).initPointFList(null, token.Token);

                while (!token.IsCancellationRequested)
                {
                    animationState = MovieClipState.Playing;

                    int frameIndex = ((MovieClip)data)._lastPlayedFrame;

                    if (renderSingleFrameCheckBox.Checked)
                        frameIndex = int.Parse(renderSingleFrameTextBox.Text);

                    Bitmap image = ((MovieClip)data).renderAnimation(rOptions, frameIndex);

                    if (image == null)
                    {
                        stopAnimationPlaying(true);
                        Console.WriteLine($"Frame Index {frameIndex} returned null image.");
                        return;
                    }

                    pictureBox1.Invoke((Action)(delegate
                    {
                        pictureBox1.Image = image;
                        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                        pictureBox1.Refresh();
                    }));

                    image.Dispose();
                    image = null;

                    if ((frameIndex + 1) != ((MovieClip)data).GetFrames().Count)
                        ((MovieClip)data)._lastPlayedFrame = frameIndex + 1;
                    else
                        ((MovieClip)data)._lastPlayedFrame = 0;

                    await Task.Delay((1000 / ((MovieClip)data).FPS), token.Token);


                    if (token.IsCancellationRequested)
                    {
                        animationState = MovieClipState.Stopped;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(OperationCanceledException) || ex.GetType() == typeof(TaskCanceledException))
                {
                    animationState = MovieClipState.Stopped;
                    return;
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }

            animationState = MovieClipState.Stopped;
            return;
        }

        private void stopAnimationPlaying(bool isEnd)
        {
            _renderingCancelToken.Cancel();

            while (this.animationState == MovieClipState.Playing)
            {           
                if (this.animationTask != null)
                {
                    if (this.animationTask.Status == TaskStatus.WaitingForActivation && this.animationTask.AsyncState == null)
                    {
                        this.animationState = MovieClipState.Stopped;
                    }
                }

                continue;
            }

            pictureBox1.Image = null;
            pictureBox1.Refresh();

            if (_scFile != null)
            {
                if (_scFile.CurrentRenderingMovieClips.Count > 0)
                {
                    foreach (MovieClip mv in this._scFile.CurrentRenderingMovieClips)
                    {
                        mv._lastPlayedFrame = 0;

                        if (isEnd)
                            mv.destroyPointFList();
                    }

                    _scFile.setRenderingItems(new List<ScObject>());
                }
            }

            animationState = MovieClipState.Stopped;
        }

        private void editData(editType type)
        {
            bool isEdited = true;

            if (_checkedNodes.Count > 0)
            {
                foreach (TreeNode oData in _checkedNodes)
                {
                    if (oData?.Tag != null)
                    {
                        editChildren((ScObject)oData.Tag, type);
                    }
                    else
                    {
                        MessageBox.Show($"One of the tree node that is checked has a null tag... {oData.Text}");
                    }
                }
            }

            if (isEdited)
                Render();
        }

        private void editChildren(ScObject childrenData, editType type)
        {
            if (!saveAsMatrix)
            {
                if (childrenData.GetDataType() == 7 || childrenData.GetDataType() == 1)
                {
                    ScObject tempObject = childrenData;

                    if (tempObject.GetDataType() == 7)
                        tempObject = tempObject.GetDataObject();

                    initChildsPointFEdit(tempObject, type);
                }
                else if (childrenData.GetDataType() == 0)
                {
                    int childIndex = _originalData.FindIndex(data => data.childrenId == childrenData.Id);
                    if (childIndex == -1)
                    {
                        addShapeOriginalData((Shape)childrenData);
                    }

                    applyPointsEdit(type, (Shape)childrenData);
                }
            }
            else
            {
                OriginalData childrenOriginalData = null;
                int childIndex = _originalData.FindIndex(data => data.childrenId == childrenData.Id);

                if (childIndex == -1)
                {
                    childrenOriginalData = new OriginalData() { childrenId = childrenData.Id, chunkXYData = null, matrixData = new Matrix(1, 0, 0, 1, 0, 0) };

                    _originalData.Add(childrenOriginalData);
                }
                else
                {
                    childrenOriginalData = _originalData[childIndex];
                }

                applyMatrixEdit(type, childrenOriginalData);
            }
        }

        private void initChildsPointFEdit(ScObject tempObject, editType type)
        {
            foreach (ScObject exChild in ((MovieClip)tempObject).getChildrensWithoutDuplicates())
            {
                if (exChild.GetDataType() == 0)
                {
                    int exShapeIndex = _originalData.FindIndex(data => data.childrenId == tempObject.Id);
                    if (exShapeIndex == -1)
                    {
                        addShapeOriginalData((Shape)exChild);
                    }

                    applyPointsEdit(type, (Shape)exChild);
                }
                else if (exChild.GetDataType() == 7)
                {
                    initChildsPointFEdit(exChild, type);
                }
            }
        }

        private void addShapeOriginalData(Shape childrenData)
        {
            List<PointF[]> addPointF = new List<PointF[]>();

            foreach (ShapeChunk chunk in (childrenData).GetChunks())
            {
                PointF[] pointFData = new PointF[chunk.XY.Length];
                for (int i = 0; i < chunk.XY.Length; i++)
                {
                    pointFData[i].X = chunk.XY[i].X;
                    pointFData[i].Y = chunk.XY[i].Y;
                }
                addPointF.Add(pointFData);
            }

            OriginalData childOriginalData = new OriginalData() { childrenId = childrenData.Id, chunkXYData = addPointF, matrixData = null };
            _originalData.Add(childOriginalData);
        }

        private void applyMatrixEdit(editType type, OriginalData data)
        {
            switch (type)
            {
                case editType.PositionUp:
                    data.matrixData.Elements.SetValue((data.matrixData.Elements[5] + scaleFactor), 5);
                    data.matrixData.Translate(0, -scaleFactor);
                    break;

                case editType.PositionDown:
                    data.matrixData.Elements.SetValue((data.matrixData.Elements[5] > 0 ? data.matrixData.Elements[5] - scaleFactor : ((Math.Abs(data.matrixData.Elements[5]) + scaleFactor) * -1)), 5);
                    data.matrixData.Translate(0, scaleFactor);
                    break;

                case editType.PositionLeft:
                    data.matrixData.Elements.SetValue((data.matrixData.Elements[3] > 0 ? data.matrixData.Elements[3] -= scaleFactor : ((Math.Abs(data.matrixData.Elements[3]) + scaleFactor) * -1)), 3);
                    data.matrixData.Translate(-scaleFactor, 0);
                    break;

                case editType.PositionRight:
                    data.matrixData.Elements.SetValue((data.matrixData.Elements[3] + scaleFactor), 3);

                    data.matrixData.Translate(scaleFactor, 0);
                    break;

                case editType.IncreaseSize:
                    data.matrixData.Elements.SetValue((data.matrixData.Elements[1] + scaleFactor), 1);
                    data.matrixData.Elements.SetValue((data.matrixData.Elements[4] + scaleFactor), 4);

                    float scaleUp = scaleFactor < 1 ? 1 + scaleFactor : scaleFactor;
                    data.matrixData.Scale(scaleUp, scaleUp);
                    break;

                case editType.DecreaseSize:
                    if (data.matrixData.Elements[1] >= 0)
                        data.matrixData.Elements.SetValue((data.matrixData.Elements[1] - scaleFactor <= 0 ? 0 : data.matrixData.Elements[1] - scaleFactor), 1);

                    if (data.matrixData.Elements[4] >= 0)
                        data.matrixData.Elements.SetValue((data.matrixData.Elements[4] - scaleFactor <= 0 ? 0 : data.matrixData.Elements[4] - scaleFactor), 4);

                    float scaleDown = scaleFactor > 1 ? 1 / scaleFactor : 1 - scaleFactor;
                    data.matrixData.Scale(scaleDown, scaleDown);
                    break;

                case editType.AngleAntiClockWise:
                    data.matrixData.Rotate(-scaleFactor);
                    break;

                case editType.AngleClockWise:
                    data.matrixData.Rotate(scaleFactor);
                    break;
            }
        }

        private void applyPointsEdit(editType type, Shape childrenData)
        {
            for (int s = 0; s < childrenData.GetChunks().Count; s++)
            {
                PointF[] xyData = ((ShapeChunk)childrenData.GetChunks()[s]).XY;

                if (type != editType.AngleClockWise && type != editType.AngleAntiClockWise)
                {
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
                else
                {
                    GraphicsPath test = new GraphicsPath();
                    test.AddPolygon(xyData);
                    Matrix rotateMatrix = new Matrix(1, 0, 0, 1, 0, 0);
                    PointF origin = findCenter(xyData);

                    if (type == editType.AngleClockWise)
                    {
                        rotateMatrix.RotateAt(scaleFactor, origin);
                    }
                    else if (type == editType.AngleAntiClockWise)
                    {
                        rotateMatrix.RotateAt(-scaleFactor, origin);
                    }

                    test.Transform(rotateMatrix);
                    ((ShapeChunk)childrenData.GetChunks()[s]).SetXY(test.PathPoints);
                }

            }
        }

        public PointF findCenter(PointF[] data)
        {
            float totalX = 0;
            float totalY = 0;

            foreach (PointF p in data)
            {
                totalX += p.X;
                totalY += p.Y;
            }

            float centerX = totalX / data.Length;
            float centerY = totalY / data.Length;

            return new PointF(centerX, centerY);
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
            stopAnimationPlaying(true);

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
                    rotateAntiClockWiseButton.Enabled = true;
                    rotateClockWiseButton.Enabled = true;
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
                rotateAntiClockWiseButton.Enabled = false;
                rotateClockWiseButton.Enabled = false;
            }

            pictureBox1.Image = null;
            pictureBox1.Refresh();
            Render();
            zoomed = false;
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            return; // DISABLED

#pragma warning disable CS0162 // Unreachable code detected
            ScObject node = (ScObject)treeView1.SelectedNode.Tag;
#pragma warning restore CS0162 // Unreachable code detected

            if (node == null)
                return;

            int width = 0;
            int height = 0;

            if (node.Bitmap != null)
            {
                width = node.Bitmap.Width;
                height = node.Bitmap.Height;
            }
            else
            {
                width = pictureBox1.Width;
                height = pictureBox1.Height;
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

        public void revertShape(Shape shapeData)
        {
            int index = _originalData.FindIndex(data => data.childrenId == shapeData.Id);

            if (index == -1)
                return;

            int originalIndex = _scFile.GetShapes().FindIndex(Shape => Shape.Id == _originalData[index].childrenId);

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

        private void rotateAntiClockWiseButton_Click(object sender, EventArgs e)
        {
            editData(editType.AngleAntiClockWise);
        }

        private void rotateClockWiseButton_Click(object sender, EventArgs e)
        {
            editData(editType.AngleClockWise);
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
            PositionRight,
            AngleClockWise,
            AngleAntiClockWise
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

                        if (node.Nodes.Count > 0 || ((ScObject)node?.Tag).GetDataType() != 0)
                            removeChecked(node.Nodes, dataType);
                    }
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

            if (animationTask != null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (!animationTask.IsCompleted && !(animationTask.Status == TaskStatus.WaitingForActivation || animationTask.Status == TaskStatus.WaitingToRun))
                {
                    if (sw.Elapsed.TotalSeconds > 30)
                    {
                        DialogResult t = MessageBox.Show($"Waited {sw.Elapsed.TotalSeconds} seconds but animation task is still running.\nPress yes to wait 30seconds more or no to close form right now.", "Error", MessageBoxButtons.YesNo);

                        if (t == DialogResult.No)
                        {
                            break;
                        }
                        else
                        {
                            sw.Restart();
                            continue;
                        }
                    }
                }
                sw.Stop();
            }

            pictureBox1.Image = null;
            pictureBox1.Refresh();
        }

        private void editMatrixButton_Click(object sender, EventArgs e)
        {
            if (_originalData.Count > 0)
            {
                DialogResult result = MessageBox.Show("There are changes not yet made of " + (_saveAsMatrix == true ? "Matrix Edit Type" : "PointF Edit Type") + ". Revert edits and change edit type?", "Changes not saved", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    if (_saveAsMatrix)
                    {
                        _originalData = new List<OriginalData>();
                    }
                    else
                    {
                        for (int i = 0; i < _originalData.Count; i++)
                        {
                            Shape shapeData = (Shape)_scFile.GetShapes()[_scFile.GetShapes().FindIndex(shape => shape.Id == _originalData[i].childrenId)];

                            revertShape(shapeData);
                        }
                    }
                    Render();
                }
                else
                {
                    Render();
                    return;
                }
            }

            if (saveAsMatrix)
            {
                this.editMatrixButton.BackColor = Color.FromArgb(56, 56, 56);
            }
            else
            {
                this.editMatrixButton.BackColor = Color.FromArgb(16, 77, 16);
            }

            _saveAsMatrix = !_saveAsMatrix;
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.Text != "Exports")
            {
                if ((ScObject)e.Node?.Tag != null)
                {
                    int type = ((ScObject)e.Node?.Tag).GetDataType();
                    if (type == 0 || type == 7 || type == 1)
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

        private void renderSingleFrameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (renderSingleFrameCheckBox.Checked)
            {
                if (int.TryParse(renderSingleFrameTextBox.Text, out int value))
                {
                    ScObject tempObj = _data;

                    if (tempObj.GetDataType() == 7)
                        tempObj = tempObj.GetDataObject();

                    if (value >= ((MovieClip)tempObj).GetFrames().Count)
                    {
                        MessageBox.Show($"Max frame value {((MovieClip)tempObj).GetFrames().Count}");
                    }
                }
            }

            renderSingleFrameTextBox.Text = "0";
        }

        private void renderSingleFrameCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            renderSingleFrameTextBox.Enabled = !renderSingleFrameCheckBox.Checked;
        }
    }

    public class OriginalData
    {
        public ushort childrenId { get; set; }
        public List<PointF[]> chunkXYData { get; set; }
        public Matrix matrixData { get; set; }
    }
}
