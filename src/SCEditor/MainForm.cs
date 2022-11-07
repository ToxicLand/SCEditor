using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using SCEditor.Compression;
using SCEditor.ScOld;
using static System.IO.Path;
using System.Linq;
using SCEditor.Prompts;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Drawing2D;
using SCEditor.Features;
using System.Text;
using System.Drawing.Imaging;
using Encoder = System.Drawing.Imaging.Encoder;
using System.Threading;
using System.Threading.Tasks;
using static SCEditor.ScOld.MovieClip;
using System.Diagnostics;

namespace SCEditor
{
    public partial class MainForm : Form
    {
        // SC file we're dealing with.
        internal ScFile _scFile;
        internal bool zoomed;
        private MovieClipState animationState;
        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource animationCancelToken = new CancellationTokenSource();

        public MainForm()
        {
            InitializeComponent();

            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new MyColorTable());
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            ScObject node = (ScObject)treeView1.SelectedNode.Tag;

            if (node == null || (node.Bitmap == null && node.Children != null))
                return;

            if (zoomed != true)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Size = new Size(node.Bitmap.Width, node.Bitmap.Height);
            }

            zoomed = true;

            double sf = 0.125;

            if (e.Delta > 0)
            {
                if ((node.Bitmap.Width * 5.5) >= pictureBox1.Width)
                {
                    pictureBox1.Size = new Size((pictureBox1.Size.Width + (int)(pictureBox1.Size.Width * sf)), (pictureBox1.Size.Height + (int)(pictureBox1.Size.Height * sf)));
                    pictureBox1.Width = pictureBox1.Width + (int)(pictureBox1.Width * sf);
                    pictureBox1.Height = pictureBox1.Height + (int)(pictureBox1.Height * sf);
                }
            }
            else
            {
                if ((node.Bitmap.Width / 5.5) <= pictureBox1.Width)
                {
                    pictureBox1.Size = new Size((pictureBox1.Size.Width - (int)(pictureBox1.Size.Width * sf)), (pictureBox1.Size.Height - (int)(pictureBox1.Size.Height * sf)));
                    pictureBox1.Width = pictureBox1.Width - (int)(pictureBox1.Width * sf);
                    pictureBox1.Height = pictureBox1.Height - (int)(pictureBox1.Height * sf);
                }
            }
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = @"Please select your infomation file",
                Filter = @"SC File (*.sc)|*.sc|All files (*.*)|*.*",
            };
            var dialog2 = new OpenFileDialog()
            {
                Title = @"Please select your texture file",
                Filter = @"Texture SC File (*_tex.sc)|*_tex.sc|All files (*.*)|*.*",

            };
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                DialogResult result2 = dialog2.ShowDialog();
                if (result2 == DialogResult.OK)
                {
                    try
                    {
                        if (_scFile != null)
                        {
                            await stopRendering();
                        }

                        LoadSc(dialog.FileName, dialog2.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            await stopRendering();

            pictureBox1.Image = null;
            label1.Text = null;
            zoomed = false;
            Render();
            RefreshMenu();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void viewPolygonsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Render();
        }

        public void RefreshMenu()
        {
            textureToolStripMenuItem.Visible = false;
            shapeToolStripMenuItem.Visible = false;
            objectToolStripMenuItem.Visible = false;
            chunkToolStripMenuItem.Visible = false;
            if (treeView1.SelectedNode?.Tag != null)
            {
                if (treeView1.SelectedNode.Name == "2")
                {
                    textureToolStripMenuItem.Visible = true;
                    addTextureToolStripMenuItem.Visible = true;
                }

                ScObject data = (ScObject)treeView1.SelectedNode.Tag;

                switch (data.GetDataType())
                {
                    case 99:
                        chunkToolStripMenuItem.Visible = true;
                        break;
                    case 0:
                        shapeToolStripMenuItem.Visible = true;
                        break;
                    case 2:
                        textureToolStripMenuItem.Visible = true;
                        break;
                    case 1:
                    case 7:
                        objectToolStripMenuItem.Visible = true;
                        break;
                    default:
                        break;
                }
            }
        }

        // Creates a new instance of the Decoder object and loads the decompressed SC files.
        private void LoadSc(string fileName, string textureFile)
        {
            _scFile = new ScFile(fileName, textureFile);
            _scFile.Load();

            //var scfile = ScFile.Load(fileName, ScFormatVersion.Version7);

            treeView1.Nodes.Clear();

            pictureBox1.Image = null;
            label1.Text = null;

            saveToolStripMenuItem.Visible = true;
            reloadToolStripMenuItem.Visible = true;
            exportAllShapeToolStripMenuItem.Visible = true;
            exportAllChunkToolStripMenuItem.Visible = true;
            exportAllAnimationToolStripMenuItem.Visible = true;
            compressionToolStripMenuItem.Visible = true;
            addTextureToolStripMenuItem.Visible = true;

            RefreshMenu();

            treeView1.Populate(_scFile.GetTextures());
            //treeView1.Populate(_scFile.GetExports().OrderBy(e => e.GetName()).ToList());
            treeView1.Populate(_scFile.GetExports()); // this or above
            treeView1.Populate(_scFile.GetShapes());
            treeView1.Populate(_scFile.GetMovieClips());
            treeView1.Populate(_scFile.getTextFields());
        }

        private async void Render()
        {
            await stopRendering();

            animationCancelToken = new CancellationTokenSource();

            RenderingOptions options = new RenderingOptions()
            {
                ViewPolygons = viewPolygonsToolStripMenuItem.Checked
            };

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject data = (ScObject)treeView1.SelectedNode.Tag;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                label1.Text = data.GetInfo();

                // CHECK WHY SHAPE?? MIGHT BE WRONG
                if (treeView1.SelectedNode?.Parent.Tag != null && (data.objectType == ScObject.SCObjectType.Shape || data.objectType == ScObject.SCObjectType.MovieClip))
                {
                    ScObject parentData = (ScObject)treeView1.SelectedNode?.Parent.Tag;

                    if (parentData.objectType == ScObject.SCObjectType.Export || parentData.objectType == ScObject.SCObjectType.MovieClip)
                    {
                        if (parentData.objectType == ScObject.SCObjectType.Export)
                            parentData = parentData.GetDataObject();

                        if (parentData != null)
                        {
                            List<int> framesMentionedIn = new List<int>();
                            int frameIdx = 0;
                            ushort timelineIndex = (ushort)((MovieClip)parentData).timelineChildrenId.ToList().FindIndex(x => x == data.Id);

                            int frameId = 0;
                            foreach (MovieClipFrame mvframe in ((MovieClip)parentData).Frames)
                            {
                                for (int i = 0; i < mvframe.Id; i++)
                                {
                                    if (((MovieClip)parentData).timelineArray[frameIdx + (i * 3)] == timelineIndex)
                                    {
                                        framesMentionedIn.Add(frameId);
                                    }
                                }

                                frameIdx += mvframe.Id * 3;
                                frameId++;
                            }

                            if (framesMentionedIn.Count != 0)
                            {
                                Console.WriteLine($"SCObject with ID {data.Id} is mentioned in following frames index:");
                                foreach (int frameIn in framesMentionedIn)
                                {
                                    Console.WriteLine($"{frameIn}");
                                }
                            }
                        }
                    }
                }

                if (data.Children == null && data.Bitmap != null)
                {
                    pictureBox1.Size = new Size(data.Bitmap.Width, data.Bitmap.Height);
                }

                if (data.GetDataType() == 7 || data.GetDataType() == 1)
                {
                    if (!playAnimationByDefaultToolStripMenuItem.Checked)
                        startRendering(options, data);
                }
                else
                {
                    pictureBox1.Image = data.Render(options);
                    pictureBox1.Refresh();
                }
            }
        }

        public void fixPoints()
        {
            ScObject data = (ScObject)treeView1.SelectedNode?.Tag;

            if (data == null)
                return;

            if (data.GetDataType() != 99)
                return;

            ScObject parentData = (ScObject)treeView1.SelectedNode.Parent?.Tag;

            if (parentData == null)
                return;

            if (parentData.GetDataType() != 0)
                return;

            fixChunksPoints fixP = new fixChunksPoints();
            bool newArray = fixP.fixPoints(data, _scFile);

            if (!newArray)
                return;

            int shapeChunkVexCount = 0;
            foreach (ShapeChunk chunk in parentData.Children)
            {
                shapeChunkVexCount += chunk.vertexCount;
            }

            ((Shape)parentData).setShapeChunkVertexCount(shapeChunkVexCount);

            _scFile.AddChange(parentData);

            Console.WriteLine("FIXED");
            Render();
        }

        private void startRendering(RenderingOptions options, ScObject inScData)
        {
            ScObject data = inScData;

            if (data.GetDataType() == 7)
                data = ((Export)inScData).GetDataObject();

            if (data == null)
                throw new Exception("MainForm:Render() datatype is 1 or 7 but dataobject is null");

            ((MovieClip)data)._lastPlayedFrame = 0;

            TimeSpan interval = TimeSpan.FromMilliseconds((1000 / ((MovieClip)data).FPS));
            _timer = new PeriodicTimer(interval);

            _timerTask = Task.Run(async () => {
                await renderAnimation(options, (MovieClip)data).ConfigureAwait(true);
            }, animationCancelToken.Token);
        }

        private async Task renderAnimation(RenderingOptions options, MovieClip data)
        {
            try
            {
                int totalFrameTimelineCount = 0;
                foreach (MovieClipFrame frame in (data).GetFrames())
                {
                    totalFrameTimelineCount += (frame.Id * 3);
                }

                if ((data).timelineArray.Length % 3 != 0 || (data).timelineArray.Length != totalFrameTimelineCount)
                {
                    await stopRendering();
                    MessageBox.Show("MoveClip timeline array length is not set equal to total frames count.");
                    return;
                }

                (data).initPointFList(null, animationCancelToken.Token);

                Console.WriteLine("Started Playing!");

                while (await _timer.WaitForNextTickAsync(animationCancelToken.Token))
                {
                    animationState = MovieClipState.Playing;

                    int frameIndex = (data)._lastPlayedFrame;

                    Bitmap image = (data).renderAnimation(new RenderingOptions() { ViewPolygons = viewPolygonsToolStripMenuItem.Checked }, frameIndex);

                    if (image == null)
                    {
                        animationState = MovieClipState.Stopped;
                        await stopRendering();
                        MessageBox.Show($"Frame Index {frameIndex} returned null image.");
                        return;
                    }

                    pictureBox1.Invoke((Action)(delegate
                    {
                        pictureBox1.Size = image.Size;
                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                        pictureBox1.Image = image;
                        pictureBox1.Refresh();
                    }));

                    image.Dispose();

                    if ((frameIndex + 1) != (data).GetFrames().Count)
                        (data)._lastPlayedFrame = frameIndex + 1;
                    else
                        (data)._lastPlayedFrame = 0;
                }

                if (animationCancelToken.IsCancellationRequested)
                {
                    animationState = MovieClipState.Stopped;
                    return;
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
            await stopRendering();
        }

        private async Task stopRendering()
        {
            if (_timerTask is null)
            {
                return;
            }

            animationCancelToken.Cancel();

            await Task.Run(() =>
            {
                while (animationState != MovieClipState.Stopped)
                {
                    continue;
                }
            });

            _timerTask = null;

            if (_scFile != null)
            {
                if (_scFile.CurrentRenderingMovieClips.Count > 0)
                {
                    foreach (MovieClip mv in this._scFile.CurrentRenderingMovieClips)
                    {
                        mv._animationState = MovieClipState.Stopped;
                        mv._lastPlayedFrame = 0;
                        mv.destroyPointFList();
                    }

                    _scFile.setRenderingItems(new List<ScObject>());
                }
            }

            if (animationState == MovieClipState.Playing)
                animationState = MovieClipState.Stopped;      

            Console.WriteLine("Animation Stopped!");
        }   

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            ScObject data = (ScObject)treeView1.SelectedNode?.Tag;

            if (data != null)
            {
                if (data.GetDataType() == 1 || data.GetDataType() == 7)
                {
                    if (data.GetDataType() == 7)
                        data = ((Export)data).GetDataObject();

                    if (data == null)
                        throw new Exception("MainForm:Render() datatype is 1 or 7 but dataobject is null");

                    ((MovieClip)data).destroyPointFList();
                }
            }
        }

        public void ExportAllChunk()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    int i = 0;
                    foreach (var shape in this._scFile.GetShapes())
                    {
                        foreach (var chunk in shape.Children)
                        {
                            chunk.Render(new RenderingOptions()).Save(fbd.SelectedPath + "/Chunk_" + i++ + ".png");
                        }
                    }
                }
            }
        }

        public void ExportAllShape()
        {
            DialogResult result = MessageBox.Show("Rendering shape is currently experimenetal.\nProceed?",
                "Experimental Rendering", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result1 = fbd.ShowDialog();
                    if (result1 == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        int i = 0;
                        foreach (var shape in this._scFile.GetShapes())
                        {
                            shape.Render(new RenderingOptions()).Save(fbd.SelectedPath + "/Shape_" + i++ + ".png");
                        }
                    }
                }
            }
        }

        public void Export()
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Image File | *.png";
                string filename = "export";
                if (!string.IsNullOrEmpty(treeView1.SelectedNode.Text))
                    filename = treeView1.SelectedNode.Text;
                dlg.FileName = filename + ".png";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(dlg.FileName))
                        File.Delete(dlg.FileName);

                    ScObject data = (ScObject)treeView1.SelectedNode?.Tag;

                    if (data != null)
                    {
                        if (data.GetDataType() == 2)
                        {
                            ((Texture)data).GetImage().GetBitmap().Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (data.GetDataType() == 99 || data.GetDataType() == 0)
                        {
                            RenderingOptions options = new RenderingOptions() { InternalRendering = true, ViewPolygons = viewPolygonsToolStripMenuItem.Checked };
                            data.Render(options).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            Console.WriteLine($"Data type {data.GetDataType()} not supported for export.");
                        }
                    }
                }

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_scFile.GetPendingChanges().Count > 0 || _scFile.GetPendingMatrixChanges().Count > 0)
            {
                using (FileStream input = new FileStream(_scFile.GetInfoFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    if (_scFile.GetInfoFileName() == _scFile.GetTextureFileName())
                    {
                        _scFile.Save(input, input);
                        return;
                    }
                    using (FileStream inputtex = new FileStream(_scFile.GetTextureFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        _scFile.Save(input, inputtex);
                    }
                }
            }
        }

        private void createTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (createTextureDialog form = new createTextureDialog())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Dictionary<byte, string> imageTypes = new Dictionary<byte, string>
                    {
                        {0, "ImageRGBA8888"},
                        {1, "ImageRGBA8888"},
                        {2, "ImageRGBA4444"},
                        {3, "ImageRGBA5551"},
                        {4, "ImageRGB565"},
                        {6, "ImageLuminance8Alpha8"},
                        {10, "ImageLuminance8"}
                    };

                    var textureTypeKey = imageTypes.FirstOrDefault(x => x.Value == form.textureImageType).Key;

                    Texture tex = new Texture(textureTypeKey, form.textureWidth, form.Height, _scFile);
                    _scFile.AddTexture(tex);
                    _scFile.AddChange(tex);

                    treeView1.Populate(new List<ScObject>() { tex });
                }
            }
        }

        private void addTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap textureImage = (Bitmap)Image.FromFile(dialog.FileName);
                    Texture data = new Texture(_scFile, textureImage);
                    data.setCustomAdded(true);

                    _scFile.AddTexture(data);
                    _scFile.AddChange(data);
                    treeView1.Populate(new List<ScObject>() { data });

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "SC File Location";
                sfd.Filter = "SC File (*.sc & *_tex.sc)|*.sc|All files (*.* & *_tex.*)|*.*";

                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(sfd.FileName))
                {
                    string filePath = Path.GetDirectoryName(sfd.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(sfd.FileName);
                    string fileExtension = Path.GetExtension(sfd.FileName);

                    string texFile = filePath + @"\" + fileName + "_tex" + fileExtension;

                    FileStream file = new FileStream(sfd.FileName, FileMode.Create);
                    file.Close();

                    FileStream fileTex = new FileStream(texFile, FileMode.Create);
                    fileTex.Close();

                    // Add Starting Data
                    ScFile _newScFile = new ScFile(sfd.FileName, texFile);
                    //Texture _defaultTexture = new Texture(0,800,800);
                    //_newScFile.AddTexture(_defaultTexture);

                    try
                    {
                        _newScFile.Load();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

            }
        }

        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void exportToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void exportToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    Bitmap chunk = (Bitmap)Image.FromFile(dialog.FileName);
                    if (treeView1.SelectedNode?.Tag != null)
                    {
                        ShapeChunk data = (ShapeChunk)treeView1.SelectedNode.Tag;
                        data.Replace(chunk);
                        _scFile.AddChange(_scFile.GetTextures()[data.GetTextureId()]);
                        Render();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag != null)
            {
                Texture data = new Texture((Texture)treeView1.SelectedNode.Tag);
                _scFile.AddTexture(data);
                _scFile.AddChange(data);
                treeView1.Populate(new List<ScObject>() { data });
            }
        }

        private void changeTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceTexture form = new ReplaceTexture();
            List<ushort> textureIds = new List<ushort>();
            foreach (Texture texture in _scFile.GetTextures())
                textureIds.Add(texture.GetTextureId());
            ((ComboBox)form.Controls["comboBox1"]).DataSource = textureIds;

            if (form.ShowDialog() == DialogResult.OK)
            {
                if (treeView1.SelectedNode?.Tag != null)
                {
                    ShapeChunk data = (ShapeChunk)treeView1.SelectedNode.Tag;

                    if (treeView1.SelectedNode.Parent?.Tag != null)
                    {
                        Shape sdata = (Shape)treeView1.SelectedNode.Parent.Tag;
                        data.SetTextureId(Convert.ToByte(((ComboBox)form.Controls["comboBox1"]).SelectedItem));

                        _scFile.AddChange(sdata);
                    }
                    else
                    {
                        return;
                    }

                    Render();
                }
            }
            form.Dispose();
        }

        private void createExportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private async void editChildrenDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScObject scData = (ScObject)treeView1.SelectedNode?.Tag;

            if (scData == null)
                return;

            if (scData.GetDataType() != 7 && scData.GetDataType() != 1)
                return;

            if (scData.GetDataType() == 7)
                scData = scData.GetDataObject();

            editChildrenData editChildrenDialog = new editChildrenData(_scFile, scData);

            await stopRendering();

            if (editChildrenDialog.ShowDialog() == DialogResult.OK)
            {
                ushort[] newChildrenId = editChildrenDialog.ChildrenIds;
                string[] newChildrenNames = editChildrenDialog.ChildrenNames;
                byte[] newFlags = editChildrenDialog.Flags;

                if (newChildrenId.Length != newChildrenNames.Length)
                    throw new Exception($"new children id and name arrays length dont match up.");

                //int frameIndex = 0;
                //foreach (MovieClipFrame mvFrame in ((MovieClip)scData).GetFrames())
                //{
                //    for (int i = 0; i < mvFrame.Id; i++)
                //    {
                //        ushort childrenIndex = ((MovieClip)scData).timelineArray[frameIndex + (i * 3)];

                //        if (newChildrenId.Length <= childrenIndex)
                //            throw new Exception($"Timeline array has a children index {childrenIndex} at {frameIndex}");
                //    }

                //    frameIndex += mvFrame.Id * 3;
                //}

                ((MovieClip)scData).setTimelineChildrenId(newChildrenId);
                ((MovieClip)scData).setTimelineChildrenNames(newChildrenNames);
                ((MovieClip)scData).setFlags(newFlags);
                ((MovieClip)scData).SetFrames(editChildrenDialog.Frames.ToList());
                ((MovieClip)scData).setTimelineOffsetArray(editChildrenDialog.TimelineArray);

                List<ScObject> childrenItem = new List<ScObject>();

                foreach (ushort childId in newChildrenId)
                {
                    int shapeIndex = _scFile.GetShapes().FindIndex(sco => sco.Id == childId);
                    if (shapeIndex != -1)
                    {
                        childrenItem.Add(_scFile.GetShapes()[shapeIndex]);
                    }
                    else
                    {
                        int movieClipIndex = _scFile.GetMovieClips().FindIndex(sco => sco.Id == childId);
                        if (movieClipIndex != -1)
                        {
                            childrenItem.Add(_scFile.GetMovieClips()[movieClipIndex]);
                        }
                        else
                        {
                            int textFieldIdnex = _scFile.getTextFields().FindIndex(sco => sco.Id == childId);
                            if (textFieldIdnex != -1)
                            {
                                childrenItem.Add(_scFile.getTextFields()[textFieldIdnex]);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                }

                ((MovieClip)scData).setChildrens(childrenItem);
                _scFile.AddChange(scData);

                // RESET Treeview node
                treeView1.SelectedNode.Nodes.Clear();
                treeView1.SelectedNode.PopulateChildren(scData);
            }

            Console.WriteLine("Editing Children Data Done!");
        }

        private void toolStripMenuItemEditCharacter_Click(object sender, EventArgs e)
        {
            ScObject scData = (ScObject)treeView1.SelectedNode?.Tag;

            try
            {
                using (editCharacter form = new editCharacter())
                {
                    if (scData != null)
                    {
                        form.setScData(scData, _scFile);
                        form.addData();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            if (form.saveAsMatrix != true)
                            {
                                List<OriginalData> saveChanges = form._originalData.ToList();

                                foreach (OriginalData data in saveChanges)
                                {
                                    Shape shapeData = (Shape)_scFile.GetShapes()[_scFile.GetShapes().FindIndex(s => s.Id == data.childrenId)];
                                    _scFile.AddChange(shapeData);
                                }
                            }
                            else
                            {
                                List<OriginalData> saveChanges = form._originalData.ToList();

                                DialogResult matrixReplaceAll = MessageBox.Show("Replace all matrix within the chosen export if timeline includes specified edited shape?\nYes to replace all automatically\nNo to ask for each edit\nCancel if you want to edit manually", "Replace Matrix in Timeline", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                                bool matrixEdit = false;

                                ScObject eachData = scData;

                                if (eachData.GetDataType() == 7)
                                    eachData = scData.GetDataObject();

                                foreach (OriginalData data in saveChanges)
                                {
                                    int matrixId = _scFile.GetMatrixs(((MovieClip)eachData)._transformStorageId).Count;
                                    Console.WriteLine($"Saved Matrix with id {matrixId} for Shape id {data.childrenId}");

                                    _scFile.addMatrix(data.matrixData, ((MovieClip)eachData)._transformStorageId);
                                    _scFile.addPendingMatrix(data.matrixData, ((MovieClip)eachData)._transformStorageId);

                                    if (matrixReplaceAll != DialogResult.Cancel)
                                    {
                                        bool replaceCurrent = false;

                                        if (matrixReplaceAll == DialogResult.No)
                                        {
                                            DialogResult matrixReplace = MessageBox.Show($"Replace edited matrix for all shapes with ID: {data.childrenId}?\nYes to replace matrix for specified shape id\nNo to skip editing matrix for specified shape ID\nCancel to not ask again for any shape edited", $"Replace Matrix in Timeline for Shape {data.childrenId}", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                                            if (matrixReplace == DialogResult.Cancel)
                                                matrixReplaceAll = DialogResult.Cancel;

                                            replaceCurrent = matrixReplace == DialogResult.Yes ? true : false;
                                        }

                                        bool replaceForAll = matrixReplaceAll == DialogResult.Yes ? true : false;

                                        if (replaceForAll || replaceCurrent)
                                        {
                                            ushort[] newArray = ((MovieClip)eachData).timelineArray;

                                            int shapeIdx = eachData.Children.FindIndex(shape => shape.Id == data.childrenId);

                                            if (shapeIdx != -1)
                                            {
                                                for (int i = 0; i < newArray.Length / 3; i++)
                                                {
                                                    int index = i * 3;

                                                    if (newArray[index] == (ushort)shapeIdx)
                                                    {
                                                        newArray[index + 1] = (ushort)matrixId;
                                                    }
                                                }

                                                matrixEdit = true;
                                            }
                                        }
                                    }
                                }

                                if (matrixEdit == true)
                                    _scFile.AddChange(eachData);
                            }

                            Render();
                        }
                        else
                        {
                            List<OriginalData> revertData = form._originalData.ToList();

                            if (revertData.Count > 0)
                            {
                                foreach (OriginalData data in revertData)
                                {
                                    Shape shapeData = (Shape)_scFile.GetShapes()[_scFile.GetShapes().FindIndex(s => s.Id == data.childrenId)];

                                    form.revertShape(shapeData);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void addEditMatrixtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (editMatrixes form = new editMatrixes(_scFile))
            {
                if (form.ShowDialog() == DialogResult.Yes)
                {
                    Console.WriteLine("Matrix changes");

                    foreach (var data in form.getAddedMatrixes())
                    {
                        int transformStorageId = data.Key;

                        foreach (var matrixData in data.Value)
                        {
                            Console.WriteLine($"Added Matrix with ID: {_scFile.GetMatrixs(transformStorageId).Count - 1} in Transform Storage ID: {transformStorageId}");

                            _scFile.addMatrix(matrixData, transformStorageId);
                            _scFile.addPendingMatrix(matrixData, transformStorageId);
                        }
                    }
                }
            }
        }

        private void scImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_scFile == null)
                return;

            ImportSCData importFeature = new ImportSCData(_scFile);

            if (importFeature.initiateImporting())
                reloadMenu();
        }

        private void editTimelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScObject data = (ScObject)treeView1.SelectedNode?.Tag;

            if (data == null)
                return;

            if (data.GetDataType() != 7 && data.GetDataType() != 1)
                return;

            if (data.GetDataType() == 7)
                data = data.GetDataObject();

            using (frameEditDialog form = new frameEditDialog(_scFile, data))
            {
                form.addItemsToBox();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    uint dataLength = ((MovieClip)data)._length;

                    uint frameLength = 0;
                    foreach (MovieClipFrame frame in ((MovieClip)data).Frames)
                    {
                        frameLength += 8;

                        if (frame.Name != null)
                            frameLength += (uint)Encoding.ASCII.GetBytes(frame.Name).Length;
                    }

                    dataLength = dataLength - (uint)((((MovieClip)data).timelineArray.Length * 2) + frameLength);

                    if (form.timelineArray.Length != ((MovieClip)data).timelineArray.Length)
                        ((MovieClip)data).SetFrames(form.frames.ToList());

                    ((MovieClip)data).setTimelineOffsetArray(form.timelineArray);

                    frameLength = 0;
                    foreach (MovieClipFrame frame in ((MovieClip)data).Frames)
                    {
                        frameLength += 8;

                        if (frame.Name != null)
                            frameLength += (uint)Encoding.ASCII.GetBytes(frame.Name).Length;
                    }
                    ((MovieClip)data)._length = (uint)(dataLength + (((MovieClip)data).timelineArray.Length * 2) + frameLength);

                    _scFile.AddChange(data);

                    RefreshMenu();
                }
            }

        }

        private void importExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_scFile == null)
                return;

            ImportExportData importExportData = new ImportExportData(_scFile);

            if (importExportData.initiateImporting() == true)
            {
                reloadMenu();
            }
            else
            {
                Console.WriteLine("Failed trying to import export data.");
            }
        }

        private void duplicateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject data = (ScObject)treeView1.SelectedNode.Tag;

                if (data.objectType == ScObject.SCObjectType.Export)
                {
                    CloneExport form = new CloneExport();
                    ((TextBox)form.Controls["textBox1"]).Text = data.GetName();
                    if (form.ShowDialog() == DialogResult.OK)
                    {

                        var result = ((TextBox)form.Controls["textBox1"]).Text;
                        if (!string.IsNullOrEmpty(result) && _scFile.GetExports().FindIndex(exp => exp.GetName() == result) == -1)
                        {
                            MovieClip mv = new MovieClip((MovieClip)data.GetDataObject());
                            _scFile.AddMovieClip(mv);
                            _scFile.AddChange(mv);

                            Export ex = new Export(_scFile);
                            ex.SetId(mv.Id);
                            ex.SetExportName(result);
                            ex.SetDataObject(mv);

                            _scFile.AddExport(ex);
                            _scFile.AddChange(ex);
                            treeView1.Populate(new List<ScObject>() { ex });
                        }
                        else
                        {
                            MessageBox.Show("Cloning failed. Invalid ExportName.");
                        }
                    }

                    form.Dispose();

                }
                else if (data.objectType == ScObject.SCObjectType.MovieClip)
                {
                    MovieClip mv = new MovieClip((MovieClip)data);
                    _scFile.AddMovieClip(mv);
                    _scFile.AddChange(mv);

                    Console.WriteLine($"Cloned MovieClip with id {mv.Id}");
                }
                else if (data.objectType == ScObject.SCObjectType.TextField)
                {
                    TextField tx = new TextField(_scFile, (TextField)data, _scFile.getMaxId());
                    tx.customAdded = true;

                    _scFile.addTextField(tx);
                    _scFile.AddChange(tx);

                    Console.WriteLine($"Cloned TextField with id {tx.Id}");
                }
                else
                {
                    MessageBox.Show("Not implemented for specific type.");
                }
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reloadMenu();
        }

        private void reloadMenu()
        {
            saveToolStripMenuItem.Visible = false;
            reloadToolStripMenuItem.Visible = false;

            treeView1.Nodes.Clear();

            pictureBox1.Image = null;
            label1.Text = null;

            saveToolStripMenuItem.Visible = true;
            reloadToolStripMenuItem.Visible = true;

            RefreshMenu();
            treeView1.Populate(_scFile.GetTextures());
            treeView1.Populate(_scFile.GetExports());
            treeView1.Populate(_scFile.GetShapes());
            treeView1.Populate(_scFile.GetMovieClips());
            treeView1.Populate(_scFile.getTextFields());
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (result == DialogResult.OK)
                {
                    try
                    {
                        Image texture = Image.FromFile(dialog.FileName);
                        Texture data = (Texture)treeView1.SelectedNode.Tag;

                        data._image.SetBitmap((Bitmap)texture);
                        _scFile.AddChange(data);
                        Render();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void exportAllChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportAllChunk();
        }

        private void exportAllShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportAllShape();
        }

        private void exportAllAnimationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ExportAllAnimations();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LZMAToolStripMenuItem.Checked)
            {
                LZMAToolStripMenuItem.Checked = false;
            }
            else if (lzhamToolStripMenuItem.Checked)
            {
                lzhamToolStripMenuItem.Checked = false;
            }
        }


        private async void LZMAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult warning = MessageBox.Show(
                "After the SC file has been compressed, the tool will clear all previous data to prevent reading errors.\nContinue?",
                "Beware!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (warning == DialogResult.Yes)
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Supercell Graphics (SC) | *.sc";
                    dlg.FileName = GetFileName(_scFile.GetInfoFileName());
                    dlg.OverwritePrompt = false;
                    dlg.CreatePrompt = false;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        Lzma.Compress(_scFile.GetInfoFileName(), dlg.FileName);

                        dlg.Title = "Please enter texture file location";
                        dlg.Filter = "Supercell Texture (SC) | *_tex.sc";
                        dlg.FileName = GetFileName(_scFile.GetTextureFileName());
                        if (dlg.ShowDialog() == DialogResult.OK)
                            Lzma.Compress(_scFile.GetTextureFileName(), dlg.FileName);
                    }
                }

                saveToolStripMenuItem.Visible = false;
                reloadToolStripMenuItem.Visible = false;
                exportAllShapeToolStripMenuItem.Visible = false;
                exportAllChunkToolStripMenuItem.Visible = false;
                exportAllAnimationToolStripMenuItem.Visible = false;
                compressionToolStripMenuItem.Visible = false;
                addTextureToolStripMenuItem.Visible = false;

                treeView1.Nodes.Clear();

                await stopRendering();

                pictureBox1.Image = null;
                label1.Text = null;
                _scFile = null;
            }
        }


        private async void ZstandardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult warning = MessageBox.Show(
                "After the SC file has been compressed, the tool will clear all previous data to prevent reading errors.\nContinue?",
                "Beware!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (warning == DialogResult.Yes)
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "Supercell Graphics (SC) | *.sc";
                    dlg.FileName = GetFileName(_scFile.GetInfoFileName());
                    dlg.OverwritePrompt = false;
                    dlg.CreatePrompt = false;

                    //bool scv4 = (MessageBox.Show("Compress file as SC Version 4? (By default it will be saved as SC Version 3", "Save as SC Version 4", MessageBoxButtons.YesNo) == DialogResult.Yes ? true : false);
                    //if (scv4)
                    //{
                    //    MessageBox.Show("v4 saving not implemented yet.");
                    //}

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        zstandard.Compress(_scFile.GetInfoFileName(), dlg.FileName);

                        dlg.Title = "Please enter texture file location";
                        dlg.Filter = "Supercell Texture (SC) | *_tex.sc";
                        dlg.FileName = GetFileName(_scFile.GetTextureFileName());
                        if (dlg.ShowDialog() == DialogResult.OK)
                            zstandard.Compress(_scFile.GetTextureFileName(), dlg.FileName);
                    }


                }

                saveToolStripMenuItem.Visible = false;
                reloadToolStripMenuItem.Visible = false;
                exportAllShapeToolStripMenuItem.Visible = false;
                exportAllChunkToolStripMenuItem.Visible = false;
                exportAllAnimationToolStripMenuItem.Visible = false;
                compressionToolStripMenuItem.Visible = false;
                addTextureToolStripMenuItem.Visible = false;

                treeView1.Nodes.Clear();

                await stopRendering();

                pictureBox1.Image = null;
                label1.Text = null;
                _scFile = null;
            }
        }

        private void lzhamToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }



        private void exportToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Export();
        }

        private void exportShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Exporting chunk from export name is currently experimenetal.\nProceed?", "Experimental Exporting", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result1 = fbd.ShowDialog();
                    if (result1 == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        Export data = (Export)treeView1.SelectedNode.Tag;

                        Console.WriteLine($"Shape count: {data.Children.Count}");

                        foreach (Shape Shape in data.Children)
                        {
                            foreach (ShapeChunk Chunk in Shape.GetChunks())
                            {
                                Chunk.Render(new RenderingOptions()).Save(fbd.SelectedPath + $"/{data.GetName()}_shape{Shape.Id}_chunk{Chunk.Id}.png");
                            }
                        }
                    }
                }
            }
        }

        public class MyColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return Color.FromArgb(15, 15, 15);
                }
            }

            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return Color.FromArgb(15, 15, 15);
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return Color.FromArgb(15, 15, 15);
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return Color.FromArgb(15, 15, 15); ;
                }
            }
            public override Color MenuBorder
            {
                get
                {
                    return Color.FromArgb(15, 15, 15);
                }
            }

            public override Color MenuItemBorder
            {
                get
                {
                    return Color.FromArgb(21, 21, 21);
                }
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return Color.FromArgb(31, 31, 31);
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return Color.FromArgb(31, 31, 31);
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return Color.FromArgb(31, 31, 31);
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return Color.FromArgb(31, 31, 31);
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return Color.FromArgb(31, 31, 31);
                }
            }
        }

        private void fixPointsChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fixPoints();
        }

        private void fixBoomBeachBuildings(ScObject data)
        {
            MovieClip mvData = (MovieClip)data;

            string[] childrenName = (string[])mvData.timelineChildrenNames.Clone();
            ushort[] childrenId = (ushort[])mvData.timelineChildrenId.Clone();
            ushort[] timelineArray = (ushort[])mvData.timelineArray.Clone();

            ushort? turretId = null;
            int? turretIndex = null;

            ushort? barrelId = null;
            int? barrelIndex = null;

            for (int i = 0; i < childrenName.Length; i++)
            {
                if (childrenName[i] == "turret")
                {
                    turretId = childrenId[i];
                    turretIndex = i;
                }

                if (childrenName[i] == "turret2") // change barrel, barrel2, turret2
                {
                    barrelId = childrenId[i];
                    barrelIndex = i;
                }
            }

            if (turretId == null || barrelId == null)
                return;

            MovieClip turretMVData = (MovieClip)_scFile.GetMovieClips().Find(m => m.Id == turretId);
            MovieClip barrelMVData = (MovieClip)_scFile.GetMovieClips().Find(m => m.Id == barrelId);

            MovieClip combinedMVData = new MovieClip(_scFile, turretMVData.GetMovieClipDataType());
            combinedMVData.SetFramePerSecond(turretMVData.FPS);
            combinedMVData.SetId(_scFile.getMaxId());
            combinedMVData.setCustomAdded(true);

            int combinedChildCount = turretMVData.timelineChildrenId.Length + barrelMVData.timelineChildrenId.Length;
            string[] combinedChildrenName = new string[combinedChildCount];
            ushort[] combinedChildrenId = new ushort[combinedChildCount];

            for (int i = 0; i < turretMVData.timelineChildrenId.Length; i++)
            {
                combinedChildrenName[i] = turretMVData.timelineChildrenNames[i];
                combinedChildrenId[i] = turretMVData.timelineChildrenId[i];
            }
            for (int i = 0; i < barrelMVData.timelineChildrenId.Length; i++)
            {
                int index = turretMVData.timelineChildrenId.Length + i;

                combinedChildrenName[index] = barrelMVData.timelineChildrenNames[i];
                combinedChildrenId[index] = barrelMVData.timelineChildrenId[i];
            }

            List<ushort> combinedTimelineArray = new List<ushort>();
            List<ScObject> combinedFrames = new List<ScObject>();

            if (turretMVData.Frames.Count != barrelMVData.Frames.Count)
            {
                Console.WriteLine("not equal");
                return;
            }

            int turretTimelineIndex = 0;
            int barrelTimelineIndex = 0;
            for (int frameIndex = 0; frameIndex < turretMVData.Frames.Count; frameIndex++)
            {
                MovieClipFrame turretFrame = (MovieClipFrame)turretMVData.Frames[frameIndex];
                MovieClipFrame barrelFrame = (MovieClipFrame)barrelMVData.Frames[frameIndex];

                for (int ti = 0; ti < turretFrame.Id; ti++)
                {
                    int idx = turretTimelineIndex + (ti * 3);
                    combinedTimelineArray.Add(turretMVData.timelineArray[idx]);
                    combinedTimelineArray.Add(turretMVData.timelineArray[idx + 1]);
                    combinedTimelineArray.Add(turretMVData.timelineArray[idx + 2]);
                }

                for (int ti = 0; ti < barrelFrame.Id; ti++)
                {
                    int idx = barrelTimelineIndex + (ti * 3);
                    combinedTimelineArray.Add((ushort)(barrelMVData.timelineArray[idx] + turretMVData.timelineChildrenId.Length));
                    combinedTimelineArray.Add(barrelMVData.timelineArray[idx + 1]);
                    combinedTimelineArray.Add(barrelMVData.timelineArray[idx + 2]);
                }

                turretTimelineIndex += turretFrame.Id * 3;
                barrelTimelineIndex += barrelFrame.Id * 3;

                MovieClipFrame newF = new MovieClipFrame(_scFile);
                newF.SetId((ushort)(turretFrame.Id + barrelFrame.Id));
                combinedFrames.Add(newF);
            }

            combinedMVData.setTimelineChildrenId(combinedChildrenId);
            combinedMVData.setTimelineChildrenNames(combinedChildrenName);
            combinedMVData.setTimelineOffsetArray(combinedTimelineArray.ToArray());
            combinedMVData.SetFrames(combinedFrames);

            List<string> childNameList = childrenName.ToList();
            List<ushort> childIdList = childrenId.ToList();

            childNameList.RemoveAt((int)barrelIndex);
            childIdList.RemoveAt((int)barrelIndex);

            childrenName = childNameList.ToArray();
            childrenId = childIdList.ToArray();

            childrenId[(int)(turretIndex > barrelIndex ? turretIndex - 1 : turretIndex)] = combinedMVData.Id;

            List<ushort> newTimeLineArray = new List<ushort>();

            List<ScObject> frames = ((ScObject[])mvData.Frames.ToArray().Clone()).ToList();
            List<ScObject> newFrames = new List<ScObject>();

            int frameTimeLineIndex = 0;
            for (int frameIndex = 0; frameIndex < frames.Count; frameIndex++)
            {
                int frameTimeLineCount = frames[frameIndex].Id;
                int barrelCount = 0;

                for (int i = 0; i < frameTimeLineCount; i++)
                {
                    int currentIdx = frameTimeLineIndex + (i * 3);
                    ushort childrenID = timelineArray[currentIdx];

                    if (childrenID == barrelIndex)
                    {
                        barrelCount++;
                        continue;
                    }

                    if (childrenID > barrelIndex)
                    {
                        newTimeLineArray.Add((ushort)(timelineArray[currentIdx] - 1));
                    }
                    else
                    {
                        newTimeLineArray.Add((ushort)(timelineArray[currentIdx]));
                    }

                    newTimeLineArray.Add(timelineArray[currentIdx + 1]);
                    newTimeLineArray.Add(timelineArray[currentIdx + 2]);
                }

                frameTimeLineIndex += frameTimeLineCount * 3;

                if (barrelCount == frameTimeLineCount)
                    continue;

                MovieClipFrame newF = new MovieClipFrame(_scFile);
                newF.SetId((ushort)(frameTimeLineCount - barrelCount));
                newFrames.Add(newF);
            }

            byte[] flagData = (byte[])mvData.flags.Clone();
            List<byte> fList = flagData.ToList();
            fList.RemoveAt((int)barrelIndex);

            mvData.setFlags(fList.ToArray());
            mvData.setTimelineChildrenId(childrenId);
            mvData.setTimelineChildrenNames(childrenName);
            mvData.SetFrames(newFrames);
            mvData.setTimelineOffsetArray(newTimeLineArray.ToArray());

            _scFile.AddChange(mvData);

            _scFile.AddChange(combinedMVData);
            _scFile.AddMovieClip(combinedMVData);

            mvData.Children.Remove(barrelMVData);
            mvData.Children.Remove(turretMVData);

            mvData.Children.Add(combinedMVData);

            treeView1.SelectedNode.Nodes.Clear();
            treeView1.SelectedNode.PopulateChildren(mvData);

            treeView1.Refresh();

            Console.WriteLine("Boom Beach Fixed");
        }

        private void fixBoomBeachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag == null)
                return;

            foreach (ScObject data in _scFile.GetExports())
            {
                if (!string.IsNullOrEmpty(data.GetName()))
                {
                    if (data.GetName().Length < 3)
                        continue;

                    if (data.GetName().Substring(0, 3) != "bb_")
                        continue;

                    ScObject data2 = data.GetDataObject();

                    if (data2 != null)
                        fixBoomBeachBuildings(data2);
                }
            }
        }

        private async void exportFramesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await stopRendering();

            RenderingOptions options = new RenderingOptions()
            {
                ViewPolygons = viewPolygonsToolStripMenuItem.Checked
            };

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject data = (ScObject)treeView1.SelectedNode.Tag;

                if (data.objectType == ScObject.SCObjectType.Export || data.objectType == ScObject.SCObjectType.MovieClip)
                {
                    if (data.objectType == ScObject.SCObjectType.Export)
                        data = data.GetDataObject();

                    FolderBrowserDialog saveFileDialog = new FolderBrowserDialog();
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            ((MovieClip)data).initPointFList(null);

                            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/png");
                            Encoder myEncoder = Encoder.Quality;
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                            myEncoderParameters.Param[0] = myEncoderParameter;

                            for (int frameIndex = 0; frameIndex < ((MovieClip)data).Frames.Count; frameIndex++)
                            {
                                ((MovieClip)data).renderAnimation(options, frameIndex).Save(saveFileDialog.SelectedPath + $"frame{(frameIndex + 1).ToString("D6")}.png", myImageCodecInfo, myEncoderParameters);
                            }

                            ((MovieClip)data).destroyPointFList();
                            MessageBox.Show($"{((MovieClip)data).Frames.Count} Frames Successfully Exported at:\n{saveFileDialog.SelectedPath}", "Frames Exported");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }
        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void disbleTextfieldRenderingToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            RenderingOptions.disableTextFieldRendering = this.disbleTextfieldRenderingToolStripMenuItem.Checked;
        }

        private void customFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            customFunctions cF = new customFunctions(_scFile);
            cF.initNonObject();

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject data = (ScObject)treeView1.SelectedNode.Tag;

                if (data is null)
                    return;    

                cF.initObject(data);
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {

        }

        private void colorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editColors form = new editColors(this._scFile);

            if (form.ShowDialog() == DialogResult.Yes)
            {
                Console.WriteLine("Colors changes");

                foreach (var data in form.getAddedColors())
                {
                    int transformStorageId = data.Key;

                    foreach (var colorData in data.Value)
                    {
                        Console.WriteLine($"Added Color with ID: {_scFile.getColors(transformStorageId).Count - 1} in Transform Storage ID: {transformStorageId}");

                        _scFile.addColor(colorData, transformStorageId);
                        _scFile.addPendingColor(colorData, transformStorageId);
                    }
                }
            }
        }

        private async void exportFrameImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScObject data = (ScObject)treeView1.SelectedNode?.Tag;

            if (data is null)
                return;

            if (data.objectType != ScObject.SCObjectType.MovieClip && data.objectType != ScObject.SCObjectType.Export)
                return;

            if (data.objectType == ScObject.SCObjectType.Export)
                data = data.GetDataObject();

            DialogResult currentFrame = MessageBox.Show("Export current frame? Yes for current, No to select custom Frame.", "Export Current Frame?", MessageBoxButtons.YesNo);

            int currentFrameIndex = ((MovieClip)data)._lastPlayedFrame;

            if (currentFrame == DialogResult.No)
            {
                inputDataDialog dialog = new inputDataDialog(1);
                dialog.setLabelText($"Frame Index: 0/{((MovieClip)data).Frames.Count}");

                while (true)
                {
                    break;
                }
            }

            await stopRendering();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ((MovieClip)data).initPointFList(null);
                Bitmap bmp = ((MovieClip)data).renderAnimation(new RenderingOptions(), 0);

                if (bmp != null)
                    bmp.Save(saveFileDialog.FileName);

                ((MovieClip)data).destroyPointFList();
            }
            
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject data = (ScObject)treeView1.SelectedNode.Tag;

                if (data.objectType == ScObject.SCObjectType.Shape)
                {
                    Shape cloneShape = new Shape((Shape)data);
                    cloneShape.SetId((ushort)(_scFile.getMaxId() + 1));
                    cloneShape.setCustomAdded(true);

                    _scFile.AddShape(cloneShape);
                    _scFile.AddChange(cloneShape);
                    treeView1.Populate(new List<ScObject>() { cloneShape });
                }
                else
                {
                    return;
                }
            }
        }
    }
}