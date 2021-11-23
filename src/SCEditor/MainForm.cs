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

namespace SCEditor
{
    public partial class MainForm : Form
    {
        // SC file we're dealing with.
        internal ScFile _scFile;
        internal bool zoomed;
        static System.Windows.Forms.Timer _animationTimer = new System.Windows.Forms.Timer();
        static int frameCounter = 0;
        static bool exitFlag = false;
        private MovieClipState animationState;

        public MainForm()
        {
            InitializeComponent();

            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new MyColorTable());
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            ScObject node = (ScObject)treeView1.SelectedNode.Tag;

            if (node == null || (node.Bitmap == null && node.Children != null ))
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            killAnimationTimer();

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
            treeView1.Populate(_scFile.GetExports());
            treeView1.Populate(_scFile.GetShapes());
            treeView1.Populate(_scFile.GetMovieClips());
            treeView1.Populate(_scFile.getTextFields());
        }

        private void Render()
        {
            killAnimationTimer();

            RenderingOptions options = new RenderingOptions()
            {
                ViewPolygons = viewPolygonsToolStripMenuItem.Checked
            };

            if (treeView1.SelectedNode.Name == "2" && treeView1.SelectedNode?.Tag == null)
            {
                textureToolStripMenuItem.Visible = true;
                addTextureToolStripMenuItem.Visible = true;
            }

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject data = (ScObject)treeView1.SelectedNode.Tag;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                label1.Text = data.GetInfo();

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
                    renderAnimation(options, data);
                }
                else
                {
                    pictureBox1.Image = data.Render(options);
                    pictureBox1.Refresh();
                }
            }
        }


        public void renderAnimation(RenderingOptions options, ScObject data)
        {
            if (data.GetDataType() == 7)
                data = ((Export)data).GetDataObject();

            if (data == null)
                throw new Exception("MainForm:Render() datatype is 1 or 7 but dataobject is null");

            ((MovieClip)data).generatePointFList(null);

            if (((MovieClip)data).Frames.Count <= 2)
            {
                pictureBox1.Image = ((MovieClip)data).renderAnimation(options, 0);
                pictureBox1.Refresh();
                return;
            }

            frameCounter = 0;
            animationState = MovieClipState.Stopped;

            _animationTimer.Interval = 500;
            _animationTimer.Tick += new EventHandler(animationTimer_Tick);
            _animationTimer.Start();
        }

        public void animationTimer_Tick(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                ScObject data = (ScObject)this.treeView1.SelectedNode?.Tag;

                if (exitFlag == true && animationState == MovieClipState.Stopped)
                {
                    _animationTimer.Enabled = false;
                    exitFlag = false;
                }

                _animationTimer.Stop();

                if (data == null || data.GetDataType() != 1 && data.GetDataType() != 7)
                {
                    killAnimationTimer();
                    return;
                }

                if (data.GetDataType() == 7)
                    data = ((Export)data).GetDataObject();

                _animationTimer.Interval = 1000 / ((MovieClip)data).FPS;
                animationState = MovieClipState.Playing;

                if (frameCounter + 1 != ((MovieClip)data).Frames.Count)
                {
                    Bitmap image = ((MovieClip)data).renderAnimation(new RenderingOptions() { ViewPolygons = viewPolygonsToolStripMenuItem.Checked }, frameCounter);

                    if (image == null)
                        killAnimationTimer();

                    pictureBox1.Image = image;
                    pictureBox1.Refresh();
                    frameCounter += 1;
                    _animationTimer.Enabled = true;
                    image.Dispose();
                }
                else
                {
                    frameCounter = 0;
                    _animationTimer.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception MainForm:animationTimer_Tick()");

                killAnimationTimer();
            }
        }

        private void killAnimationTimer()
        {
            _animationTimer.Dispose();
            _animationTimer = new System.Windows.Forms.Timer();
            _animationTimer.Enabled = false;
            animationState = MovieClipState.Stopped;
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
                    pictureBox1.Image.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_scFile.GetPendingChanges().Count > 0 || _scFile.GetPendingMatrixChanges().Count > 0)
            {
                using (FileStream input = new FileStream(_scFile.GetInfoFileName(), FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
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
                    Bitmap textureImage = (Bitmap) Image.FromFile(dialog.FileName);
                    Texture data = new Texture(_scFile, textureImage);

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
                    data.SetTextureId(Convert.ToByte(((ComboBox)form.Controls["comboBox1"]).SelectedItem));
                    _scFile.AddChange(data);
                    Render();
                }
            }
            form.Dispose();
        }

        private void createExportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItemEditCharacter_Click(object sender, EventArgs e)
        {
            ScObject scData = (ScObject)treeView1.SelectedNode?.Tag;

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
                                Shape shapeData = (Shape)_scFile.GetShapes()[_scFile.GetShapes().FindIndex(s => s.Id == data.shapeId)];

                                foreach (ShapeChunk chunkData in shapeData.GetChunks())
                                {
                                    _scFile.AddChange(chunkData);
                                }
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
                                int matrixId = _scFile.GetMatrixs().Count;
                                Console.WriteLine($"Saved Matrix with id {matrixId} for Shape id {data.shapeId}");

                                _scFile.addMatrix(data.matrixData);
                                _scFile.addPendingMatrix(data.matrixData);

                                if (matrixReplaceAll != DialogResult.Cancel)
                                {
                                    bool replaceCurrent = false;

                                    if (matrixReplaceAll == DialogResult.No)
                                    {
                                        DialogResult matrixReplace = MessageBox.Show($"Replace edited matrix for all shapes with ID: {data.shapeId}?\nYes to replace matrix for specified shape id\nNo to skip editing matrix for specified shape ID\nCancel to not ask again for any shape edited", $"Replace Matrix in Timeline for Shape {data.shapeId}", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                                        if (matrixReplace == DialogResult.Cancel)
                                            matrixReplaceAll = DialogResult.Cancel;

                                        replaceCurrent = matrixReplace == DialogResult.Yes ? true : false;    
                                    }

                                    bool replaceForAll = matrixReplaceAll == DialogResult.Yes ? true : false;

                                    if (replaceForAll || replaceCurrent)
                                    {
                                        ushort[] newArray = ((MovieClip)eachData).timelineArray;

                                        int shapeIdx = eachData.Children.FindIndex(shape => shape.Id == data.shapeId);

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
                                Shape shapeData = (Shape)_scFile.GetShapes()[_scFile.GetShapes().FindIndex(s => s.Id == data.shapeId)];

                                form.revertShape(shapeData);
                            }
                        }
                    }
                }
            }
        }

        private void addEditMatrixtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Matrix> matrixList = this._scFile.GetMatrixs();

            using (editMatrixes form = new editMatrixes(matrixList))
            {
                if (form.ShowDialog() == DialogResult.Yes)
                {
                    foreach (Matrix m in form.addedMatrixes)
                    {
                        _scFile.addMatrix(m);
                        _scFile.addPendingMatrix(m);
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

            foreach (ScObject check in _scFile.GetPendingChanges())
            {
                if (check.Id == data.Id)
                {
                    MessageBox.Show("Data you are trying to edit is already in pending changes. Please save before trying to use this function", "Save before proceeding");
                    return;
                }
            }

            if (data.GetDataType() == 7)
                data = data.GetDataObject();

            using (timelineEditDialog form = new timelineEditDialog(_scFile, data))
            {
                form.addItemsToBox();

                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.timelineArray.Length != ((MovieClip)data).timelineArray.Length)
                        ((MovieClip)data).SetFrames(form.frames.ToList());

                    ((MovieClip)data).setTimelineOffsetArray(form.timelineArray);

                    _scFile.AddChange(data);

                    Render();
                }
            }

        }

        private void importExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_scFile == null)
                return;

            ImportExportData importExportData = new ImportExportData(_scFile);

            if(importExportData.initiateImporting() == true)
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
                Export data = (Export)treeView1.SelectedNode.Tag;
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
                        Bitmap texture = (Bitmap)Image.FromFile(dialog.FileName);
                        Texture data = (Texture)treeView1.SelectedNode.Tag;
                        
                        if (texture.Width != data.Bitmap.Width || texture.Height != data.Bitmap.Height)
                        {
                            //data.GetImage().SetHeight((ushort)texture.Height);
                            //data.GetImage().SetWidth((ushort)texture.Width);
                        }

                        data._image.SetBitmap(texture);
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


        private void LZMAToolStripMenuItem_Click(object sender, EventArgs e)
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

                pictureBox1.Image = null;
                label1.Text = null;
                _scFile = null;
            }
        }


        private void ZstandardToolStripMenuItem_Click(object sender, EventArgs e)
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

        public enum MovieClipState
        {
            Stopped,
            Playing,
            None
        }
    }
}