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
                if (result2 != DialogResult.OK)
                {
                    return;
                }
            }
            else
            {
                return;
            }

            string importScFileInfo = dialog.FileName;
            string importScFileTex = dialog2.FileName;

            ScFile scToImportFrom = new ScFile(importScFileInfo, importScFileTex);

            try
            {
                scToImportFrom.LoadTextureFile();
                scToImportFrom.loadInfoFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }          

            scMergeSelection selectImportExportsForm = new scMergeSelection(scToImportFrom.GetExports());

            if (selectImportExportsForm.ShowDialog() == DialogResult.Yes)
            {
                long lastShapeOffset = 0;
                long lastMovieClipOffset = 0;
                ushort maxId = 20000;
                foreach (Export eE in _scFile.GetExports())
                {
                    if (eE.Id > maxId)
                    {
                        maxId = eE.Id;
                    }

                    if (eE.GetDataObject() != null)
                    {
                        MovieClip eMV = (MovieClip)eE.GetDataObject();
                        long eMVOffset = eMV.GetOffset();
                        if (eMVOffset > lastMovieClipOffset)
                        {
                            lastMovieClipOffset = eMVOffset;
                        }

                        if (eMV.Id > maxId)
                        {
                            maxId = eMV.Id;
                        }

                        if (eMV.Children != null)
                        {
                            foreach (Shape eS in eMV.Children)
                            {
                                long eSOffset = eS.GetOffset();

                                if (eSOffset > lastShapeOffset)
                                {
                                    lastShapeOffset = eSOffset;
                                }

                                if (eS.Id > maxId)
                                {
                                    maxId = eS.Id;
                                }
                            }
                        }
                    }
                }

                if (lastShapeOffset == 0)
                    lastShapeOffset = _scFile.GetSofTagsOffset();

                if (lastMovieClipOffset == 0)
                    lastMovieClipOffset = _scFile.GetSofTagsOffset();

                List<ushort> matricesToAdd = new List<ushort>();
                List<ushort> colorTransformToAdd = new List<ushort>();
                List<int> texturesToAdd = new List<int>();

                foreach (scMergeSelection.exportItemClass item in selectImportExportsForm.checkedExports)
                {
                    Export exportToAdd = (Export)((ScObject)item.exportData);
                    MovieClip movieClipToAdd = (MovieClip)exportToAdd.GetDataObject();
                    List<ScObject> shapesToAdd = movieClipToAdd.GetShapes();

                    // SET EXPORT DATA
                    Export newExport = new Export(_scFile);
                    newExport.setCustomAdded(true);
                    maxId++; newExport.SetId(maxId);

                    string newExportName = exportToAdd.GetName();
                    while (_scFile.exportExists(newExportName) != -1)
                    {
                        MessageBox.Show($"Export name {newExportName} already exists, changing name to {newExportName + "_imported"}");
                        newExportName = newExportName + "_imported";
                        Console.WriteLine($"Duplicate new name: {newExportName}");
                    }

                    newExport.SetExportName(newExportName);

                    // SET MOVIECLIP DATA
                    MovieClip newMoveClip = new MovieClip(_scFile, movieClipToAdd.GetMovieClipDataType());
                    newMoveClip.SetOffset(-lastMovieClipOffset);
                    newMoveClip.setCustomAdded(true);
                    newMoveClip.SetId(maxId);
                    newMoveClip.SetFrameCount((short)movieClipToAdd.GetFrames().Count);
                    newMoveClip.setFlags(movieClipToAdd.flags);
                    newMoveClip.SetFramePerSecond(movieClipToAdd.FPS);
                    newMoveClip.SetFrames(movieClipToAdd.Frames);
                    newMoveClip.setScalingGrid(movieClipToAdd.scalingGrid);
                    newMoveClip.setLength(movieClipToAdd.length);

                    // -Timeline Data
                    ushort[] newTimelineArray = new ushort[movieClipToAdd.timelineArray.Length];
                    int i = 0;

                    if (movieClipToAdd.timelineArray.Length % 3 != 0)
                        throw new Exception("timelineArray length not divisible by 3");

                    while (i < (movieClipToAdd.timelineArray.Length / 3))
                    {
                        newTimelineArray[i * 3] = movieClipToAdd.timelineArray[i * 3];

                        if (movieClipToAdd.timelineArray[3 * i + 1] == 65535)
                        {
                            newTimelineArray[3 * i + 1] = 65535;
                        }
                        else
                        {
                            int newMatrixId = matricesToAdd.FindIndex(m => m == movieClipToAdd.timelineArray[3 * i + 1]);

                            if (newMatrixId == -1)
                            {
                                matricesToAdd.Add(movieClipToAdd.timelineArray[3 * i + 1]);
                                newMatrixId = matricesToAdd.Count - 1;
                            }
                            else
                            {
                                newMatrixId = matricesToAdd.IndexOf(movieClipToAdd.timelineArray[3 * i + 1]);
                            }

                            newTimelineArray[3 * i + 1] = (ushort)(_scFile.GetMatrixs().Count + newMatrixId);
                        }

                        if (movieClipToAdd.timelineArray[3 * i + 2] == 65535)
                        {
                            newTimelineArray[3 * i + 2] = 65535;
                        }
                        else
                        {
                            int newcolorTransformId = colorTransformToAdd.FindIndex(c => c == movieClipToAdd.timelineArray[3 * i + 2]); ;

                            if (newcolorTransformId == -1)
                            {
                                colorTransformToAdd.Add(movieClipToAdd.timelineArray[3 * i + 2]);
                                newcolorTransformId = colorTransformToAdd.Count - 1;
                            }
                            else
                            {
                                newcolorTransformId = colorTransformToAdd.IndexOf(movieClipToAdd.timelineArray[3 * i + 2]);
                            }

                            newTimelineArray[3 * i + 2] = (ushort)(_scFile.getColors().Count + newcolorTransformId);
                        }

                        i++;
                    }

                    newMoveClip.setTimelineOffsetArray(newTimelineArray);
                    newMoveClip.setTimelineChildrenCount(movieClipToAdd.timelineChildrenCount);
                    newMoveClip.setTimelineChildrenId(movieClipToAdd.timelineChildrenId);
                    newMoveClip.setTimelineChildrenNames(movieClipToAdd.timelineChildrenNames);
                    newMoveClip.setTimelineOffsetCount(movieClipToAdd.timelineOffsetCount);

                    ushort[] newTimelineChildrenId = (ushort[])movieClipToAdd.timelineChildrenId.Clone();

                    // SHAPES DATA
                    List<ScObject> newShapes = new List<ScObject>();
                    foreach (Shape shapeToAdd in shapesToAdd)
                    {
                        Shape newShape = new Shape(_scFile);
                        newShape.setCustomAdded(true);
                        newShape.SetOffset(-lastShapeOffset);
                        maxId++; newShape.SetId(maxId);
                        newShape.SetLength(shapeToAdd.length);

                        int timelineId = 0;
                        while (timelineId < newTimelineChildrenId.Length)
                        {
                            if (newTimelineChildrenId[timelineId] == shapeToAdd.Id)
                            {
                                newTimelineChildrenId[timelineId] = maxId;
                                break;
                            }

                            timelineId++;
                        }

                        // SHAPE CHUNK DATA
                        List<ScObject> newShapeChunks = new List<ScObject>();

                        foreach (ShapeChunk shapeChunkToAdd in shapeToAdd.GetChunks())
                        {
                            ShapeChunk newShapeChunk = new ShapeChunk(_scFile);
                            newShapeChunk.SetChunkId(shapeChunkToAdd.Id);
                            newShapeChunk.SetShapeId(maxId);

                            bool texIdAdded = false;
                            int newtexId = 0;
                            foreach (int texValue in texturesToAdd)
                            {
                                if (texValue == shapeChunkToAdd.GetTextureId())
                                {
                                    texIdAdded = true;
                                    break;
                                }
                            }
                            if (texIdAdded == false)
                            {
                                texturesToAdd.Add((int)shapeChunkToAdd.GetTextureId());
                                newtexId = texturesToAdd.Count - 1;
                            }
                            else
                            {
                                newtexId = texturesToAdd.IndexOf(shapeChunkToAdd.GetTextureId());
                            }

                            newShapeChunk.SetTextureId((byte)(_scFile.GetTextures().Count + newtexId));
                            newShapeChunk.SetChunkType(shapeChunkToAdd.GetChunkType());
                            newShapeChunk.SetUV(shapeChunkToAdd.UV);
                            newShapeChunk.SetXY(shapeChunkToAdd.XY);
                            newShapeChunk.SetVertexCount(shapeChunkToAdd.vertexCount);

                            newShapeChunks.Add(newShapeChunk);
                        }

                        newShape.setChunks(newShapeChunks);
                        newShapes.Add(newShape);
                    }

                    newMoveClip.setTimelineChildrenId(newTimelineChildrenId);

                    if (newTimelineChildrenId.Length != newShapes.Count)
                    {
                        Console.WriteLine($"{newExportName}: {newShapes.Count} Shapes added while {newTimelineChildrenId.Length} children ids noted.");
                    }

                    newMoveClip.SetShapes(newShapes);
                    newExport.SetDataObject(newMoveClip);     

                    foreach (Shape shape in newMoveClip.GetShapes())
                    {
                        _scFile.AddShape(shape);
                        _scFile.AddChange(shape);
                    }

                    _scFile.AddMovieClip(newMoveClip);
                    _scFile.AddChange(newMoveClip);

                    _scFile.AddExport(newExport);
                    _scFile.AddChange(newExport);  
                }

                foreach (int id in texturesToAdd)
                {
                    Texture newTextureToAdd = new Texture(_scFile, ((Texture)scToImportFrom.GetTextures()[id]).Bitmap);
                    _scFile.AddTexture(newTextureToAdd);
                    _scFile.AddChange(newTextureToAdd);
                }

                foreach (int id in matricesToAdd)
                {
                    if (scToImportFrom.GetMatrixs().Count < id)
                    {
                        Console.WriteLine($"{id} matrix not found");
                        continue;
                    }
                        
                    _scFile.addMatrix(scToImportFrom.GetMatrixs()[id]);
                    _scFile.addPendingMatrix(scToImportFrom.GetMatrixs()[id]);
                }

                foreach (int id in colorTransformToAdd)
                {
                    if (scToImportFrom.getColors().Count < id)
                    {
                        Console.WriteLine($"{id} color not found");
                        continue;
                    }

                    _scFile.addColor(scToImportFrom.getColors()[id]);
                    _scFile.addPendingColor(scToImportFrom.getColors()[id]);
                }

                reloadMenu();
            }
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
            MessageBox.Show("In order to import export names, use TexturePacker by Code&Web and export sprite data as JSON.\nMake sure to import the texture and select the corrosponding one.");

            using (createExportDialog form = new createExportDialog())
            {
                if (_scFile != null)
                {
                    int texCount = _scFile.GetTextures().Count;
                    object[] texList = new object[texCount];

                    for (int i = 0; i < texCount; i++)
                    {
                        texList[i] = i;
                    }

                    form.addTextureToBox(texList);

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            int selectedTexture = form.selectedTexture;

                            string jsonFileData = File.ReadAllText(form.selectedFile);
                            JObject jsonParsedData = JObject.Parse(jsonFileData);

                            JToken metaData = jsonParsedData["meta"];
                            JObject metaSizeData = JObject.Parse(metaData["size"].ToString());

                            int metaSizeWidth = (int)metaSizeData["w"];
                            int metaSizeHeight = (int)metaSizeData["h"];

                            Console.WriteLine("Meta Width: {0}\nMeta Height: {1}", metaSizeWidth, metaSizeHeight);

                            ScObject textureData = _scFile.GetTextures()[selectedTexture];

                            if (textureData != null)
                            {
                                if (textureData.Bitmap.Width != metaSizeWidth || textureData.Bitmap.Height != metaSizeHeight)
                                {
                                    MessageBox.Show("Texture Size Choosen and the Size in JSON File for the texture file do not match.", "Texture Size Invalid/Mismatch");
                                    return;
                                }
                            }

                            JArray framesData = (JArray)jsonParsedData["frames"];

                            foreach (JObject data in framesData)
                            {
                                string frameName = Path.GetFileNameWithoutExtension((string)data["filename"]).Replace(" ", "_");
                                JArray frameVerticies = (JArray)data["vertices"];
                                JArray frameVerticiesUV = (JArray)data["verticesUV"];
                                JArray frameTriangles = (JArray)data["triangles"];

                                exportType _exportType = exportType.Default;
                                iconType _iconType = iconType.None;
                                animationType _animationType = animationType.None;

                                if (frameName.Length >= 3)
                                {
                                    if (frameName.ToLower().Substring(0, 4) == "icon")
                                    {
                                        _exportType = exportType.Icon;

                                        if (frameName.Length >= 8)
                                        {
                                            switch (frameName.ToLower().Substring(5, 4))
                                            {
                                                case "unit":
                                                    _iconType = iconType.Unit;
                                                    break;

                                                case "hero":
                                                    _iconType = iconType.Hero;

                                                    if (frameName.Contains("profile"))
                                                        _iconType = iconType.HeroProfile;
                                                    break;

                                                default:
                                                    Console.WriteLine($"Unknown Icon Type for: {frameName}");
                                                    _iconType = iconType.Default;
                                                    break;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (frameName.ToLower().Contains("idle"))
                                            _animationType = animationType.Idle;

                                        if (frameName.ToLower().Contains("attack"))
                                            _animationType = animationType.Attack;

                                        if (frameName.ToLower().Contains("run") || frameName.ToLower().Contains("walk"))
                                            _animationType = animationType.Run;

                                        if (_animationType != animationType.None)
                                            _exportType = exportType.Animation;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Unknown Export Type for: {frameName}");
                                }

                                bool exportExist = false;
                                int exportIndex = -1;

                                string animationParentName = "";

                                if (_exportType == exportType.Animation)
                                {
                                    int charTimes = 0;
                                    int mainChar = -1;

                                    for (int i = 0; i < frameName.Length; i++)
                                    {
                                        if (frameName[i] == '_')
                                        {
                                            if (mainChar != -1)
                                            {
                                                charTimes++;

                                                if (charTimes == 2)
                                                {
                                                    animationParentName = frameName.Substring(0, i);
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                switch (_animationType)
                                                {
                                                    case animationType.Idle:
                                                        if (frameName.ToLower().Substring(i + 1, 4) == "idle")
                                                        {
                                                            mainChar = i;
                                                        }

                                                        break;

                                                    case animationType.Attack:
                                                        string test = frameName.ToLower().Substring(i + 1, 6);
                                                        if (frameName.ToLower().Substring(i + 1, 6) == "attack")
                                                        {
                                                            mainChar = i;
                                                        }
                                                        break;

                                                    case animationType.Run:
                                                        if (frameName.ToLower().Substring(i + 1, 3) == "run" || frameName.ToLower().Substring(i + 1, 4) == "walk")
                                                        {
                                                            mainChar = i;
                                                        }
                                                        break;

                                                    default:
                                                        throw new Exception("not implemented");
                                                }
                                            } 
                                        }
                                    }

                                    if (string.IsNullOrEmpty(animationParentName))
                                        throw new Exception("unexpected");
                                }

                                while (true)
                                {
                                    if (!string.IsNullOrEmpty(frameName))
                                    {
                                        if (_exportType == exportType.Animation)
                                        {
                                            exportIndex = _scFile.exportExists(animationParentName);
                                            if (exportIndex != -1)
                                            {
                                                exportExist = true;
                                                break;
                                            }

                                        }
                                        else
                                        {
                                            if (_scFile.exportExists(frameName) == -1)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (exportExist == false && exportIndex == -1 && _exportType == exportType.Animation)
                                        break;

                                    MessageBox.Show($"Export with name {frameName} already exists or is empty. Please change name.", "Invalid/Missing Export Name");

                                    CloneExport changeName = new CloneExport();
                                    ((TextBox)changeName.Controls["textBox1"]).Text = _animationType != animationType.None ? frameName.Substring(0, frameName.Length - 4) : frameName;

                                    if (changeName.ShowDialog() == DialogResult.OK)
                                    {
                                        var result = ((TextBox)changeName.Controls["textBox1"]).Text;
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            frameName = _exportType == exportType.Animation ? result + frameName.Substring(frameName.Length - 4) : result;
                                        }
                                    }
                                }

                                long lastShapeOffset = 0;
                                long lastShapeChunkOffset = 0;
                                long lastMovieClipOffset = 0;

                                ushort maxId = 30000;

                                foreach (Export eE in _scFile.GetExports())
                                {
                                    if (eE.Id > maxId)
                                    {
                                        maxId = eE.Id;
                                    }    

                                    if (eE.GetDataObject() != null)
                                    {
                                        MovieClip eMV = (MovieClip) eE.GetDataObject();
                                        long eMVOffset = eMV.GetOffset();
                                        if (eMVOffset > lastMovieClipOffset)
                                        {
                                            lastMovieClipOffset = eMVOffset;
                                        }

                                        if (eMV.Id > maxId)
                                        {
                                            maxId = eMV.Id;
                                        }

                                        if (eMV.Children != null)
                                        {
                                            foreach (Shape eS in eMV.Children)
                                            {
                                                long eSOffset = eS.GetOffset();

                                                if (eSOffset > lastShapeOffset)
                                                {
                                                    lastShapeOffset = eSOffset;
                                                }

                                                if (eS.Id > maxId)
                                                {
                                                    maxId = eS.Id;
                                                }  

                                                if (eS.Children != null)
                                                {
                                                    foreach (ShapeChunk eSC in eS.Children)
                                                    {
                                                        long eSCOffset = eSC.GetOffset();

                                                        if (eSCOffset > lastShapeChunkOffset)
                                                        {
                                                            lastShapeChunkOffset = eSCOffset;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }        
                                }

                                if (lastShapeOffset == 0)
                                    lastShapeOffset = _scFile.GetSofTagsOffset();

                                if (lastShapeChunkOffset == 0)
                                    lastShapeChunkOffset = _scFile.GetSofTagsOffset();

                                if (lastMovieClipOffset == 0)
                                    lastMovieClipOffset = _scFile.GetSofTagsOffset();

                                maxId++;

                                // SHAPECHUNK DATA
                                ShapeChunk shapeChunkData = new ShapeChunk(_scFile);
                                shapeChunkData.SetChunkId(0);
                                shapeChunkData.SetTextureId(Convert.ToByte(selectedTexture));
                                shapeChunkData.SetChunkType(22);
                                shapeChunkData.setCustomAdded(true);
                                shapeChunkData.SetOffset(lastShapeChunkOffset * -1);
                                shapeChunkData.SetShapeId(maxId);

                                PointF[] frameXY = new PointF[frameVerticies.Count];
                                PointF[] frameUV = new PointF[frameVerticiesUV.Count];

                                int idx = 0;
                                foreach (JToken array in frameVerticies)
                                {
                                    frameXY[idx] = new PointF((float)array[0], (float)array[1]);
                                    idx++;
                                }

                                idx = 0;
                                foreach (JToken array in frameVerticiesUV)
                                {
                                    frameUV[idx] = new PointF((float)array[0], (float)array[1]);
                                    idx++;
                                }

                                shapeChunkData.SetXY(frameXY);
                                shapeChunkData.SetUV(frameUV);

                                // SHAPE DATA
                                Shape shapeData = new Shape(_scFile);

                                shapeData.SetId(maxId);
                                shapeData.AddChunk(shapeChunkData);
                                shapeData.setCustomAdded(true);
                                shapeData.SetOffset(lastShapeOffset * -1);

                                _scFile.AddShape(shapeData);
                                _scFile.AddChange(shapeData);

                                maxId++;

                                // MOVIECLIP DATA
                                MovieClip movieClipData = new MovieClip(_scFile, 12);

                                if (exportExist)
                                {
                                    movieClipData = (MovieClip) _scFile.GetMovieClips()[_scFile.GetMovieClips().FindIndex(mv => mv.Id == _scFile.GetExports()[exportIndex].GetDataObject().Id)];
                                }
                                else
                                {
                                    movieClipData.SetId(maxId);
                                    movieClipData.SetOffset(lastMovieClipOffset * -1);
                                    movieClipData.SetDataType(12);
                                    movieClipData.setCustomAdded(true);
                                    movieClipData.SetFramePerSecond(24);
                                    movieClipData.setExportType(_exportType);
                                }
                              
                                movieClipData.SetFrameCount((short)(movieClipData.GetFrames().Count + 1));

                                if (!exportExist && movieClipData.GetShapes().Count == 0 && _exportType == exportType.Animation)
                                {
                                    inputDataDialog shadowIdInput = new inputDataDialog(1);
                                    shadowIdInput.Text = "Input Shadow Shape ID";
                                    shadowIdInput.setLabelText("Shadow ID");

                                    while (true)
                                    {
                                        DialogResult addShadowPopup = MessageBox.Show("Would you like to add shadow?", "Add Shadow?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                                        if (addShadowPopup == DialogResult.Yes)
                                        {
                                            if (shadowIdInput.ShowDialog() == DialogResult.OK)
                                            {
                                                int shapeIndex = _scFile.shapeExists(shadowIdInput.inputTextBoxInt);
                                                if (shapeIndex != -1)
                                                {
                                                    movieClipData.AddShape(_scFile.GetShapes()[shapeIndex]);
                                                    movieClipData.setHasShadow(true);
                                                    break;
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Shadow Shape with that ID does not exist", "Shadow does not Exist");
                                                }
                                            }
                                        }
                                        else if (addShadowPopup == DialogResult.No)
                                        {
                                            break;
                                        }
                                    }

                                }

                                movieClipData.AddShape(shapeData);                          

                                ushort[] timelineOffset = new ushort[] { 0, 65535, 65535 };

                                if (_exportType == exportType.Icon)
                                {
                                    movieClipData.setIconType(_iconType);

                                    if (_iconType == iconType.Hero)
                                    {
                                        timelineOffset = new ushort[] { 0, 24353, 65535, 1, 65535, 65535 }; // change after every coc update
                                    }
                                }
                                else if (_exportType == exportType.Animation)
                                {
                                    movieClipData.setAnimationType(_animationType);

                                    timelineOffset = exportExist == false ? new ushort[0] : movieClipData.timelineArray;
                                    int timeLineOffsetLength = timelineOffset.Length;

                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (movieClipData.hasShadow == true)
                                        {
                                            Array.Resize(ref timelineOffset, timeLineOffsetLength + 3);

                                            timelineOffset[timeLineOffsetLength++] = 0;
                                            timelineOffset[timeLineOffsetLength++] = 65535;
                                            timelineOffset[timeLineOffsetLength++] = 65535;
                                        }

                                        Array.Resize(ref timelineOffset, timeLineOffsetLength + 3);

                                        timelineOffset[timeLineOffsetLength++] = (ushort)(movieClipData.GetShapes().Count - 1);
                                        timelineOffset[timeLineOffsetLength++] = 65535;
                                        timelineOffset[timeLineOffsetLength++] = 65535;
                                    }
                                }

                                movieClipData.SetTimelineOffsetCount(timelineOffset.Length / 3);
                                movieClipData.setTimelineOffsetArray(timelineOffset);

                                for (int i = 0; i < 2; i++)
                                {
                                    // MOVIECLIP FRAME DATA
                                    MovieClipFrame movieClipFrameData = new MovieClipFrame(_scFile);

                                    ushort MovieClipFrameId = (ushort)(_exportType == exportType.Animation ? 2 : _exportType == exportType.Icon ? _iconType == iconType.Hero ? 2 : 1 : 1);

                                    movieClipFrameData.SetId(MovieClipFrameId);
                                    movieClipData.AddFrame(movieClipFrameData);

                                    if (_exportType != exportType.Animation)
                                        break;
                                }

                                if (exportExist == true && exportIndex != -1)
                                {
                                    int changesIdx = _scFile.GetPendingChanges().FindIndex(sco => sco.Id == movieClipData.Id && sco.GetDataType() == 1);

                                    if (changesIdx != -1)
                                    {
                                        _scFile.GetPendingChanges().RemoveAt(changesIdx);
                                    }    
                                }

                                _scFile.AddChange(movieClipData);

                                // EXPORT DATA
                                if (!exportExist)
                                {
                                    Export exportData = new Export(_scFile);

                                    exportData.SetId(maxId);
                                    string exportName = _exportType == exportType.Animation ? animationParentName : frameName;
                                    exportData.SetExportName(exportName);
                                    exportData.SetDataObject(movieClipData);
                                    exportData.setCustomAdded(true);

                                    _scFile.AddMovieClip(movieClipData);

                                    _scFile.AddExport(exportData);
                                    _scFile.AddChange(exportData);
                                }
                            }

                            reloadMenu();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error");
                        }
                    }
                }
                
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