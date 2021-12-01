using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json.Linq;
using SCEditor.Prompts;
using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using Shape = SCEditor.ScOld.Shape;

namespace SCEditor.Features
{
    public class ImportSCData
    {
        private ScFile _scFile;
        private ScFile scToImportFrom;
        private List<PointF> _pointsAdded;
        private Dictionary<ushort, ushort> _shapesAlreadyAdded;
        private Dictionary<ushort, ushort> _movieClipsAlreadyAdded;
        private Dictionary<ushort, ushort> _textFieldsAlreadyAdded;
        private List<ushort> _matricesToAdd;
        private List<ushort> _colorTransformToAdd;
        private bool _createNewTexture;
        private bool _newTextureImport = false;
        private ushort _textureToImportToID;
        private float _scaleFactor;
        private string tempFolder;
        private Process texturePackerProcess;
        private List<(ushort, List<Matrix>)> shapeChunksTMatrix;
        private List<(byte, int)> textureToAdd;
        private string _currentExportName;
        private Dictionary<string, Dictionary<string, Dictionary<string , float>>> _queriesToPerform; // Type, (keyword, ()) -> example = contains, (barbarian, (scale, value))

        public ImportSCData(ScFile scFile)
        {
            _scFile = scFile;
            _pointsAdded = new List<PointF>();
            _shapesAlreadyAdded = new Dictionary<ushort, ushort>();
            _movieClipsAlreadyAdded = new Dictionary<ushort, ushort>();
            _textFieldsAlreadyAdded = new Dictionary<ushort, ushort>();
            _matricesToAdd = new List<ushort>();
            _colorTransformToAdd = new List<ushort>();
            shapeChunksTMatrix = new List<(ushort, List<Matrix>)>();
            textureToAdd = new List<(byte, int)>();
            _queriesToPerform = new Dictionary<string, Dictionary<string, Dictionary<string, float>>>();

            try
            {
                DirectoryInfo di = Directory.CreateDirectory(System.IO.Path.GetTempPath() + "sceditor\\chunks");
                tempFolder = di.FullName;

                clearTempFolder();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception: addChunksBitmapToTexture()");
            }
        }

        public bool initiateImporting()
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
                if (result2 != DialogResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string importScFileInfo = dialog.FileName;
            string importScFileTex = dialog2.FileName;

            scToImportFrom = new ScFile(importScFileInfo, importScFileTex);

            try
            {
                if (importScFileInfo != importScFileTex)
                    scToImportFrom.LoadTextureFile();
                scToImportFrom.loadInfoFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

            scMergeSelection selectImportExportsForm = new scMergeSelection(scToImportFrom.GetExports());

            if (selectImportExportsForm.ShowDialog() == DialogResult.Yes)
            {
                _createNewTexture = selectImportExportsForm.newTextureChecked;
                _scaleFactor = selectImportExportsForm.scaleFactor == 0 ? 1 : selectImportExportsForm.scaleFactor;

                while (!_createNewTexture)
                {
                    inputDataDialog inputTextureToMergeDialog = new inputDataDialog(1);
                    inputTextureToMergeDialog.setLabelText("Texture ID:");

                    if (inputTextureToMergeDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (_scFile.GetTextures().FindIndex(t => t.Id == (ushort)inputTextureToMergeDialog.inputTextBoxInt) != -1)
                        {
                            _textureToImportToID = (ushort)inputTextureToMergeDialog.inputTextBoxInt;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Texture ID not found. Please enter the correct texture index.", "Texture ID not Found");
                        }
                    }
                }

                if (_createNewTexture == true)
                {
                    while (true)
                    {
                        DialogResult askImportTextureNew = MessageBox.Show("Would you like to import the full texture?", "Import Full Texture", MessageBoxButtons.YesNo);

                        if (askImportTextureNew == DialogResult.Yes)
                        {
                            _newTextureImport = true;
                            break;
                        }
                        else if (askImportTextureNew == DialogResult.No)
                        {
                            _newTextureImport = false;
                            break;
                        }
                    }
                    
                    if (!_newTextureImport)
                    {
                        _textureToImportToID = (byte)_scFile.GetTextures().Count;
                    }
                }

                ushort maxId = _scFile.getMaxId();

                List<ScObject> exportsToImport = new List<ScObject>();

                string crTroops = "bandit_;chr_musketeer_;electro_wizard_;elixir_blob_;elixir_golem_;elixir_golemite_;GoldenKnight_;magic_archer_;mega_knight_;princess_;rascal_;skeletondragon_;SkeletonKing_";
                string bbTroops = "tank_";
                List<string> crTroopsList = crTroops.Split(';').ToList();
                List<string> bbTroopsList = bbTroops.Split(';').ToList();

                foreach (scMergeSelection.exportItemClass item in selectImportExportsForm.checkedExports)
                {
                    Export exportToAdd = (Export)((ScObject)item.exportData);
                    bool crChangeName = true;
                    bool bbChangeName = true;

                    if (crChangeName && crTroopsList.FindIndex(name => exportToAdd.GetName().Contains(name)) != -1)
                    {
                        switch (exportToAdd.GetName()[exportToAdd.GetName().Length - 1])
                        {
                            case '3':
                                exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 1, 1) + "1");
                                break;

                            case '5':
                                exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 1, 1) + "2");
                                break;

                            case '7':
                                exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 1, 1) + "3");
                                break;

                            default:
                                break;
                        }
                    }

                    if (bbChangeName && bbTroopsList.FindIndex(name => exportToAdd.GetName().Contains(name)) != -1)
                    {
                        if (float.TryParse(exportToAdd.GetName().Substring((exportToAdd.GetName().Length - 3), 3), out float _))
                        {
                            float val = float.Parse(exportToAdd.GetName().Substring((exportToAdd.GetName().Length - 3), 3));
                            int newVal = 0;

                            switch (val)
                            {
                                case 0.3F:
                                    newVal = 2;
                                    break;

                                case 0.6F:
                                    newVal = 3;
                                    break;

                                case 1.3F:
                                    newVal = 5;
                                    break;

                                case 1.6F:
                                    newVal = 6;
                                    break;

                                case 2.3F:
                                    newVal = 8;
                                    break;

                                case 2.6F:
                                    newVal = 9;
                                    break;

                                case 3.3F:
                                    newVal = 11;
                                    break;

                                case 3.6F:
                                    newVal = 12;
                                    break;
                            }

                            exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 3, 3) + newVal.ToString());
                        }
                        else if (int.TryParse(exportToAdd.GetName().Substring((exportToAdd.GetName().Length - 1), 1), out int _))
                        {
                            int val = int.Parse(exportToAdd.GetName().Substring((exportToAdd.GetName().Length - 1), 1));
                            int newVal = 0;

                            switch (val)
                            {
                                case 0:
                                    newVal = 1;
                                    break;

                                case 1:
                                    newVal = 4;
                                    break;

                                case 2:
                                    newVal = 7;
                                    break;

                                case 3:
                                    newVal = 10;
                                    break;

                                case 4:
                                    newVal = 13;
                                    break;
                            }

                            exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 1, 1) + newVal.ToString());
                        }
                        else
                        {
                            throw new Exception("WOT");
                        }
                    }

                    exportsToImport.Add(exportToAdd);
                }

                performQueries();

                exportsToImport = exportsToImport.OrderBy(ex => ex.GetName()).ToList();
                foreach (Export exportToAdd in exportsToImport)
                {
                    MovieClip movieClipToAdd = (MovieClip)exportToAdd.GetDataObject();

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

                    _currentExportName = newExportName;
                    newExport.SetExportName(newExportName);

                    MovieClip newMovieClip = null;

                    if (_movieClipsAlreadyAdded.ContainsKey(movieClipToAdd.Id))
                    {
                        newMovieClip = (MovieClip)_scFile.GetMovieClips().Find(mv => mv.Id == _movieClipsAlreadyAdded[movieClipToAdd.Id]);

                        int checkExportID = _scFile.GetExports().FindIndex(ex => ex.Id == newMovieClip.Id);
                        if (checkExportID != -1)
                        {
                            newExport.SetId(newMovieClip.Id);
                            maxId--;
                        }
                        else
                        {
                            newExport.SetId(newMovieClip.Id);
                        }
                    }
                    else
                    {
                        newMovieClip = addImportedMovieClip(movieClipToAdd, ref maxId, scToImportFrom, newExportName);
                    }

                    if (newMovieClip == null)
                        throw new Exception("Movieclip can not be null?");

                    newExport.SetDataObject(newMovieClip);

                    _scFile.AddExport(newExport);
                    _scFile.AddChange(newExport);
                }

                foreach (int id in _matricesToAdd)
                {
                    if (scToImportFrom.GetMatrixs().Count < id)
                    {
                        Console.WriteLine($"{id} matrix not found");
                        continue;
                    }

                    _scFile.addMatrix(scToImportFrom.GetMatrixs()[id]);
                    _scFile.addPendingMatrix(scToImportFrom.GetMatrixs()[id]);
                }

                foreach (int id in _colorTransformToAdd)
                {
                    if (scToImportFrom.getColors().Count < id)
                    {
                        Console.WriteLine($"{id} color not found");
                        continue;
                    }

                    _scFile.addColor(scToImportFrom.getColors()[id]);
                    _scFile.addPendingColor(scToImportFrom.getColors()[id]);
                }

                if (_createNewTexture)
                    if (_newTextureImport)
                        return true;

                generateChunksTexture();
                return true;
            }

            return false;
        }

        private MovieClip addImportedMovieClip(MovieClip movieClipToAdd, ref ushort maxId, ScFile scToImportFrom, string newExportName)
        {
            // SET MOVIECLIP DATA
            MovieClip newMovieClip = new MovieClip(_scFile, movieClipToAdd.GetMovieClipDataType());
            newMovieClip.SetOffset(-1);
            newMovieClip.setCustomAdded(true);
            newMovieClip.SetId(maxId);
            newMovieClip.SetFrameCount((short)movieClipToAdd.GetFrames().Count);
            newMovieClip.setFlags(movieClipToAdd.flags);
            newMovieClip.SetFramePerSecond(movieClipToAdd.FPS);
            newMovieClip.SetFrames(movieClipToAdd.Frames);
            newMovieClip.setScalingGrid(movieClipToAdd.scalingGrid);
            newMovieClip.setLength(movieClipToAdd.length);

            _movieClipsAlreadyAdded.Add(movieClipToAdd.Id, maxId);

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
                    int newMatrixId = _matricesToAdd.FindIndex(m => m == movieClipToAdd.timelineArray[3 * i + 1]);

                    if (newMatrixId == -1)
                    {
                        _matricesToAdd.Add(movieClipToAdd.timelineArray[3 * i + 1]);
                        newMatrixId = _matricesToAdd.Count - 1;
                    }
                    else
                    {
                        newMatrixId = _matricesToAdd.IndexOf(movieClipToAdd.timelineArray[3 * i + 1]);
                    }

                    newTimelineArray[3 * i + 1] = (ushort)(_scFile.GetMatrixs().Count + newMatrixId);
                }

                if (movieClipToAdd.timelineArray[3 * i + 2] == 65535)
                {
                    newTimelineArray[3 * i + 2] = 65535;
                }
                else
                {
                    int newcolorTransformId = _colorTransformToAdd.FindIndex(c => c == movieClipToAdd.timelineArray[3 * i + 2]); ;

                    if (newcolorTransformId == -1)
                    {
                        _colorTransformToAdd.Add(movieClipToAdd.timelineArray[3 * i + 2]);
                        newcolorTransformId = _colorTransformToAdd.Count - 1;
                    }
                    else
                    {
                        newcolorTransformId = _colorTransformToAdd.IndexOf(movieClipToAdd.timelineArray[3 * i + 2]);
                    }

                    newTimelineArray[3 * i + 2] = (ushort)(_scFile.getColors().Count + newcolorTransformId);
                }

                i++;
            }

            newMovieClip.setTimelineOffsetArray(newTimelineArray);
            newMovieClip.setTimelineChildrenCount(movieClipToAdd.timelineChildrenCount);

            string[] newTimelineChildrenNames = (string[])movieClipToAdd.timelineChildrenNames.Clone();

            for (int tcnIdx = 0; tcnIdx < newTimelineChildrenNames.Length; tcnIdx++)
            {
                if (!string.IsNullOrEmpty(newTimelineChildrenNames[tcnIdx]))
                {
                    Console.WriteLine($"{newExportName} children name {newTimelineChildrenNames[tcnIdx]}");
                    if (newTimelineChildrenNames[tcnIdx].Contains("pivot"))
                    {
                        newTimelineChildrenNames[tcnIdx] = "attack_pivot";
                    }
                }
            }

            ushort[] newTimelineChildrenId = (ushort[])movieClipToAdd.timelineChildrenId.Clone();

            // SHAPES DATA
            List<ScObject> newShapes = new List<ScObject>();

            int idx = 0;
            while (idx < newTimelineChildrenId.Length)
            {
                ushort childrenId = newTimelineChildrenId[idx];

                if (scToImportFrom.GetShapes().FindIndex(s => s.Id == childrenId) != -1)
                {
                    if (_shapesAlreadyAdded.ContainsKey(childrenId))
                    {
                        newTimelineChildrenId[idx] = _shapesAlreadyAdded[childrenId];

                        int alreadyShapeIdx = newShapes.FindIndex(s => s.Id == newTimelineChildrenId[idx]);
                        if (alreadyShapeIdx == -1)
                        {
                            if (_scFile.GetShapes().Find(s => s.Id == newTimelineChildrenId[idx]) == null)
                                throw new Exception("Shape is not supposed to be null?");

                            newShapes.Add(_scFile.GetShapes().Find(s => s.Id == newTimelineChildrenId[idx]));
                        }
                    }
                    else
                    {
                        Shape shapeToAdd = (Shape)scToImportFrom.GetShapes().Find(s => s.Id == childrenId);
                        Shape newShape = addImportedShape(ref maxId, shapeToAdd);

                        if (newShape == null)
                            throw new Exception("Shape is not supposed to be null?");

                        if (_scFile.GetShapes().FindIndex(s => s.Id == newShape.Id) == -1)
                        {
                            _scFile.AddShape(newShape);
                            _scFile.AddChange(newShape);
                        }

                        newShapes.Add(newShape);
                        newTimelineChildrenId[idx] = maxId;
                        _shapesAlreadyAdded.Add(childrenId, maxId);
                    }
                }
                else if (scToImportFrom.GetMovieClips().FindIndex(mv => mv.Id == childrenId) != -1)
                {
                    if (_movieClipsAlreadyAdded.ContainsKey(childrenId))
                    {
                        newTimelineChildrenId[idx] = _movieClipsAlreadyAdded[childrenId];
                    }
                    else
                    {
                        maxId++;
                        MovieClip extraMovieClip = (MovieClip)scToImportFrom.GetMovieClips().Find(mv => mv.Id == childrenId);
                        MovieClip extraNewMovieClip = addImportedMovieClip(extraMovieClip, ref maxId, scToImportFrom, newExportName);

                        _scFile.AddMovieClip(extraNewMovieClip);
                        _scFile.AddChange(extraNewMovieClip);

                        newTimelineChildrenId[idx] = extraNewMovieClip.Id;
                    }
                }
                else if (scToImportFrom.getTextFields().FindIndex(n => n.Id == childrenId) != -1)
                {
                    if (_textFieldsAlreadyAdded.ContainsKey(childrenId))
                    {
                        newTimelineChildrenId[idx] = _textFieldsAlreadyAdded[childrenId];
                    }
                    else
                    {
                        maxId++;
                        TextField extraTextField = (TextField)scToImportFrom.getTextFields().Find(tf => tf.Id == childrenId);
                        TextField extraNewTextField = new TextField(_scFile, extraTextField, maxId);

                        extraNewTextField.setId(maxId);
                        extraNewTextField.setCustomAdded(true);

                        _textFieldsAlreadyAdded.Add(childrenId, maxId);

                        _scFile.addTextField(extraNewTextField);
                        _scFile.AddChange(extraNewTextField);

                        if (_scFile.getFontNames().FindIndex(fn => fn == extraNewTextField.fontName) == -1)
                        {
                            Console.WriteLine($"[Font Name Missing]: Imported {newExportName} has TextField with font name {extraNewTextField.fontName} missing in current SC File.");
                            _scFile.addFontName(extraNewTextField.fontName);
                        }

                        newTimelineChildrenId[idx] = maxId;
                    }
                }
                else
                {
                    throw new Exception($"{newExportName}: unknown type of children id {childrenId}");
                }

                idx++;
            }

            newMovieClip.setTimelineChildrenNames(newTimelineChildrenNames);
            newMovieClip.setTimelineOffsetCount(movieClipToAdd.timelineOffsetCount);
            newMovieClip.setTimelineChildrenId(newTimelineChildrenId);
            newMovieClip.setChildrens(newShapes);

            _scFile.AddMovieClip(newMovieClip);
            _scFile.AddChange(newMovieClip);

            return newMovieClip;
        }

        private Shape addImportedShape(ref ushort maxId, Shape shapeToAdd)
        {
            Shape newShape = new Shape(_scFile);
            newShape.setCustomAdded(true);
            newShape.SetOffset(-1);
            maxId++; newShape.SetId(maxId);
            newShape.SetLength(shapeToAdd.length);

            // SHAPE CHUNK DATA
            List<ScObject> newShapeChunks = new List<ScObject>();

            int shapeChunkIndex = 0;
            foreach (ShapeChunk shapeChunkToAdd in shapeToAdd.GetChunks())
            {
                int texId = _textureToImportToID;

                if (_createNewTexture)
                {
                    if (!_newTextureImport)
                    {
                        exportChunksBitmap(shapeChunkToAdd, shapeChunkIndex, maxId, _scaleFactor);
                    }
                    else
                    {
                        int texIndex = textureToAdd.FindIndex(tx => tx.Item1 == shapeChunkToAdd.GetTextureId());
                        if (texIndex == -1)
                        {
                            Texture texToAdd = (Texture)scToImportFrom.GetTextures()[(int)shapeChunkToAdd.GetTextureId()];
                            Texture newTexToAdd = new Texture(_scFile, texToAdd.Bitmap, texToAdd._imageType);
                            textureToAdd.Add((shapeChunkToAdd.GetTextureId(), newTexToAdd.Id));

                            _scFile.AddTexture(newTexToAdd);
                            _scFile.AddChange(newTexToAdd);

                            texId = newTexToAdd.Id;
                        }
                        else
                        {
                            texId = textureToAdd[texIndex].Item2;
                        }
                    }
                }
                else
                {
                    exportChunksBitmap(shapeChunkToAdd, shapeChunkIndex, maxId, _scaleFactor);
                }

                ShapeChunk newShapeChunk = new ShapeChunk(_scFile);
                newShapeChunk.SetChunkId(shapeChunkToAdd.Id);
                newShapeChunk.SetShapeId(maxId);
                newShapeChunk.SetTextureId((byte)texId);
                newShapeChunk.SetChunkType(shapeChunkToAdd.GetChunkType());
                newShapeChunk.SetUV(shapeChunkToAdd.UV);
                newShapeChunk.SetXY(shapeChunkToAdd.XY);
                newShapeChunk.SetVertexCount(0);

                newShapeChunks.Add(newShapeChunk);
                shapeChunkIndex++;
            }

            newShape.setChunks(newShapeChunks);

            return newShape;
        }

        public void exportChunksBitmap(ShapeChunk shapeChunkToAdd, int shapeChunkIndex, ushort maxId, float scaleFactor)
        {
            RenderingOptions renderOptions = new RenderingOptions() { ViewPolygons = false, InternalRendering = true };
            Bitmap shapeChunkBitmap = shapeChunkToAdd.Render(renderOptions);

            foreach (var (key, value) in _queriesToPerform["contains"])
            {
                if (_currentExportName.Contains(key))
                {
                    if (value.ContainsKey("scale"))
                        scaleFactor = value["scale"];

                    break;
                }
            }

            if (scaleFactor != 1)
            {
                int scaleWidth = (int)(shapeChunkBitmap.Width * scaleFactor);
                int scaleHeight = (int)(shapeChunkBitmap.Height * scaleFactor);

                Bitmap scaledShapeChunkBitmap = new Bitmap(scaleWidth, scaleHeight);

                using (Graphics grfx = Graphics.FromImage(scaledShapeChunkBitmap))
                {
                    grfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grfx.CompositingQuality = CompositingQuality.HighQuality;
                    grfx.SmoothingMode = SmoothingMode.AntiAlias;
                    grfx.DrawImage(shapeChunkBitmap, new System.Drawing.Rectangle(0, 0, scaleWidth, scaleHeight));
                }

                shapeChunkBitmap = scaledShapeChunkBitmap;
            }
 
            int shapeChunkTMatrixIndex = shapeChunksTMatrix.FindIndex(X => X.Item1 == maxId);
            if (shapeChunkTMatrixIndex != -1)
            {
                shapeChunksTMatrix[shapeChunkTMatrixIndex].Item2.Add(getShapeChunkMatrixTransformation(shapeChunkToAdd));
            }
            else
            {
                List<Matrix> newList = new List<Matrix>();
                newList.Add(getShapeChunkMatrixTransformation(shapeChunkToAdd));
                shapeChunksTMatrix.Add((maxId, ((Matrix[])newList.ToArray().Clone()).ToList()));
            }

            shapeChunkBitmap.Save(tempFolder + "\\" + maxId + "_" + shapeChunkIndex + ".png", ImageFormat.Png);
        }

        private void generateChunksTexture()
        {
            if (!_createNewTexture)
            {
                _scFile.GetTextures()[_textureToImportToID].Bitmap.Save(tempFolder + "\\" + _textureToImportToID + "_texture.png", ImageFormat.Png);
                _scFile.AddChange(_scFile.GetTextures()[_textureToImportToID]);
            }

            OpenFileDialog texturePackerPathDialog = new OpenFileDialog() { Filter = "Texture Packer CI Executable (TexturePacker.exe) | *.exe" };
            string texturePackerEXEPath = @"C:\Program Files\CodeAndWeb\TexturePacker\bin\TexturePacker.exe";

            if (!File.Exists(texturePackerEXEPath))
            {
                texturePackerPathDialog.ShowDialog();
                texturePackerEXEPath = texturePackerPathDialog.FileName;
            }

            bool isGeneratedTextureRGBA4444 = false;

            if (_createNewTexture)
            {
                inputDataDialog textureTypeDialog = new inputDataDialog(1);
                textureTypeDialog.setLabelText("Texture Type (0=RGBA8888, 1=RGBA4444):");

                while (true)
                {
                    if (textureTypeDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (textureTypeDialog.inputTextBoxInt != 0 || textureTypeDialog.inputTextBoxInt != 1)
                        {
                            if (textureTypeDialog.inputTextBoxInt == 1)
                                isGeneratedTextureRGBA4444 = true;

                            break;
                        }
                        else
                        {
                            MessageBox.Show("Please Type 0 for RGBA8888 or 1 for RGBA4444", "Invalid Texture Type");
                        }
                    }
                }

                Texture newTex = new Texture((byte)(isGeneratedTextureRGBA4444 == false ? 0 : 2), 1, 1, _scFile);
                _scFile.AddTexture(newTex);
                _scFile.AddChange(newTex);
            }
            else
            {
                string textureTypeName = ((Texture)_scFile.GetTextures()[_textureToImportToID])._image.GetImageTypeName();
                isGeneratedTextureRGBA4444 = textureTypeName == "RGB4444" ? true : (textureTypeName == "RGB8888" ? false : throw new Exception("Not added"));
            } 

            int generatedTextureScale = 1;
            int generatedSpritesExtrude = 0;
            int generatedSpritesPadding = 6;
            int generatedTextureMaxWidth = 4096;
            int generatedTextureMaxHeight = 4096;
            int generatedTexturePolygonTolerance = 200;
            string generatedTexturePixelFormat = isGeneratedTextureRGBA4444 ? "RGBA4444 --dither-type Linear" : "RGBA8888";
            string generatedSpritesPackMode = "Best";
            string generatedSpritesAlphaHandling = "ClearTransparentPixels";

            string arguements = $"--scale {generatedTextureScale} --extrude {generatedSpritesExtrude} --texture-format png --pack-mode {generatedSpritesPackMode} --algorithm Polygon --alpha-handling {generatedSpritesAlphaHandling} --shape-padding {generatedSpritesPadding} --trim-mode Polygon --png-opt-level 0 --opt RGBA8888 --tracer-tolerance {generatedTexturePolygonTolerance} --disable-rotation --max-width {generatedTextureMaxWidth} --max-height {generatedTextureMaxHeight} --format json-array" + " --data \"" + tempFolder + "\\output\\data.json\" \"" + tempFolder + "\"";

            texturePackerProcess = new Process();
            launchProcess(texturePackerEXEPath, arguements);

            try
            {
                Console.WriteLine("Generating new texture!");
                texturePackerProcess.Start();
                texturePackerProcess.BeginErrorReadLine();
                texturePackerProcess.BeginOutputReadLine();
                texturePackerProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (texturePackerProcess.ExitCode == 0)
            {
                if (!File.Exists(tempFolder + "\\output\\data.png"))
                    throw new Exception("data.png not found. generatesChunksTexture()");

                using (FileStream stream = new FileStream(tempFolder + "\\output\\data.png", FileMode.Open, FileAccess.Read))
                {
                    Bitmap newTexture = (Bitmap)Image.FromStream(stream);
                    ((Texture)_scFile.GetTextures()[_textureToImportToID]).GetImage().SetBitmap(newTexture);
                }

                string jsonFileData = File.ReadAllText(tempFolder + "\\output\\data.json");
                JObject jsonParsedData = JObject.Parse(jsonFileData);

                JArray framesData = (JArray)jsonParsedData["frames"];

                foreach (JObject data in framesData)
                {
                    if (data["filename"].ToString().Contains("texture"))
                        continue;

                    string frameName = ((string)data["filename"]).Split('.')[0];
                    ushort shapeID = ushort.Parse(frameName.Split('_')[0]);
                    int chunkIndex = ushort.Parse(frameName.Split('_')[1]);

                    JArray chunkVerticies = (JArray)data["vertices"];
                    JArray chunkVerticiesUV = (JArray)data["verticesUV"];
                    JArray chunkTriangles = (JArray)data["triangles"];

                    PointF[] frameXY = new PointF[chunkVerticies.Count];
                    PointF[] frameUV = new PointF[chunkVerticiesUV.Count];

                    int idx = 0;
                    foreach (JToken array in chunkVerticies)
                    {
                        frameXY[idx] = new PointF((float)array[0], (float)array[1]);
                        idx++;
                    }

                    idx = 0;
                    foreach (JToken array in chunkVerticiesUV)
                    {
                        frameUV[idx] = new PointF((float)array[0], (float)array[1]);
                        idx++;
                    }

                    ShapeChunk shapeChunkToEdit = (ShapeChunk)((Shape)_scFile.GetShapes().Find(s => s.Id == shapeID)).GetChunks()[chunkIndex];
                    shapeChunkToEdit.SetUV(frameUV);
                    shapeChunkToEdit.SetXY(findUVXYDifference(frameXY, shapeID, chunkIndex));
                    shapeChunkToEdit.SetVertexCount(frameUV.Length);
                }
            }
            else
            {
                throw new Exception("Not supposed to happen.");
            }

            texturePackerProcess = null;
            clearTempFolder();
        }

        public PointF[] findUVXYDifference(PointF[] XYArray, ushort shapeId, int chunkIndex)
        {
            PointF[] newPointF = (PointF[])XYArray.Clone();
            Matrix chunkTransform = shapeChunksTMatrix.Find(T => T.Item1 == shapeId).Item2[chunkIndex];

            chunkTransform.TransformPoints(newPointF);

            return newPointF;
        }

        private Matrix getShapeChunkMatrixTransformation(ShapeChunk shapeChunkIN)
        {
            PointF[] shapeChunkUVData = (PointF[])shapeChunkIN.UV.Clone();
            PointF[] shapeChunkXYData = (PointF[])shapeChunkIN.XY.Clone();
            float leftWidth = scToImportFrom.GetTextures()[shapeChunkIN.GetTextureId()].Bitmap.Width;
            float topHeight = scToImportFrom.GetTextures()[shapeChunkIN.GetTextureId()].Bitmap.Height;

            for (int i = 0; i < shapeChunkUVData.Length; i++)
            {
                if (shapeChunkUVData[i].X < leftWidth)
                {
                    leftWidth = shapeChunkUVData[i].X;
                }

                if (shapeChunkUVData[i].Y < topHeight)
                {
                    topHeight = shapeChunkUVData[i].Y;
                }
            }

            for (int i = 0; i < shapeChunkUVData.Length; i++)
            {
                shapeChunkUVData[i].X -= leftWidth;
                shapeChunkUVData[i].Y -= topHeight;

                if (_scaleFactor != 1)
                {
                    shapeChunkUVData[i].X *= _scaleFactor;
                    shapeChunkUVData[i].Y *= _scaleFactor;
                }
            }

            if (_scaleFactor != 1)
            {
                for (int i = 0; i < shapeChunkXYData.Length; i++)
                {

                    shapeChunkXYData[i].X *= _scaleFactor;
                    shapeChunkXYData[i].Y *= _scaleFactor;
                }
            }

            double[,] matrixArrayUV =
            {
                {
                    shapeChunkUVData[0].X, shapeChunkUVData[1].X, shapeChunkUVData[2].X
                },
                {
                    shapeChunkUVData[0].Y, shapeChunkUVData[1].Y, shapeChunkUVData[2].Y
                },
                {
                    1, 1, 1
                }
            };



            double[,] matrixArrayXY = {
                {
                     shapeChunkIN.XY[0].X, shapeChunkIN.XY[1].X, shapeChunkIN.XY[2].X
                },
                {
                     shapeChunkIN.XY[0].Y, shapeChunkIN.XY[1].Y, shapeChunkIN.XY[2].Y
                },
                {
                     1, 1, 1
                }
            };

            var matrixUV = Matrix<double>.Build.DenseOfArray(matrixArrayUV);
            var matrixXY = Matrix<double>.Build.DenseOfArray(matrixArrayXY);
            var inverseMatrixUV = matrixUV.Inverse();
            var transformMatrix = matrixXY * inverseMatrixUV;

            return new Matrix((float)transformMatrix[0, 0], (float)transformMatrix[1, 0], (float)transformMatrix[0, 1], (float)transformMatrix[1, 1], (float)transformMatrix[0, 2], (float)transformMatrix[1, 2]);
        }

        public void performQueries()
        {
            if (_scaleFactor != 1)
            {
                throw new Exception("Scalefactor should be 1 to perform queries!");
            }

            if (!_newTextureImport)
            {
                DialogResult inputQuery = MessageBox.Show("Would you like to perform custom query?", "Perform Extra", MessageBoxButtons.YesNo);

                if (inputQuery == DialogResult.Yes)
                {
                    // Query Template
                    _queriesToPerform.Add("contains", new Dictionary<string, Dictionary<string, float>>());

                    inputDataDialog queryDialog = new inputDataDialog(0);
                    queryDialog.setLabelText("Query");
                    while (true)
                    {
                        string error = "Unknown";

                        if (queryDialog.ShowDialog() == DialogResult.OK)
                        {
                            string queryData = queryDialog.inputTextBoxString;

                            if (!string.IsNullOrEmpty(queryData))
                            {
                                bool isWorking = false;
                                string[] eachQuery = queryData.Split(';');
                                foreach (string eachQueryItem in eachQuery)
                                {
                                    string[] queryItemData = eachQueryItem.Split(' ');

                                    if (queryItemData.Length < 4)
                                        error = $"Invalid query type in: {eachQueryItem}";

                                    switch (queryItemData[0].ToLower())
                                    {
                                        case "contains":
                                            string keyword = queryItemData[1];

                                            if (queryItemData[2].ToLower() == "scale")
                                            {
                                                if (float.TryParse(queryItemData[3], out float _))
                                                {
                                                    float scaleValue = float.Parse(queryItemData[3]);

                                                    Dictionary<string, float> itemTypeDict = new Dictionary<string, float>();
                                                    itemTypeDict.Add("scale", scaleValue);

                                                    try
                                                    {
                                                        _queriesToPerform["contains"].Add(keyword, itemTypeDict);
                                                        isWorking = true;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        error = ex.Message;
                                                        isWorking = false;
                                                    }

                                                }
                                                else
                                                {
                                                    error = $"Invalid query scale value in (only float allowed): {eachQueryItem}";
                                                }
                                            }
                                            else
                                            {
                                                goto default;
                                            }
                                            break;

                                        default:
                                            error = $"Invalid query type in: {eachQueryItem}";
                                            break;
                                    }

                                    if (!isWorking)
                                        break;
                                }

                                if (isWorking)
                                    break;
                            }
                            else
                            {
                                error = "Query input can not be empty!";
                            }
                        }

                        if (MessageBox.Show($"{error}\n Press cancel to skip query input.", "Error Query", MessageBoxButtons.RetryCancel) == DialogResult.Cancel)
                            break;
                    }
                }
            }
        }

        public void addChunksBitmapToTexture(ref List<ScObject> shapeChunksInput, Texture textureAppendTo, float scaleFactor)
        {
            Bitmap finalImage = null;

            foreach (ShapeChunk shapeChunkInput in shapeChunksInput)
            {
                RenderingOptions renderOptions = new RenderingOptions() { ViewPolygons = false };
                Bitmap shapeChunkBitmap = shapeChunkInput.Render(renderOptions);

                if (scaleFactor != 1)
                {
                    int scaleWidth = (int)(shapeChunkBitmap.Width * scaleFactor);
                    int scaleHeight = (int)(shapeChunkBitmap.Height * scaleFactor);

                    Bitmap scaledShapeChunkBitmap = new Bitmap(scaleWidth, scaleHeight);

                    using (Graphics grfx = Graphics.FromImage(scaledShapeChunkBitmap))
                    {
                        grfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        grfx.CompositingQuality = CompositingQuality.HighQuality;
                        grfx.SmoothingMode = SmoothingMode.AntiAlias;
                        grfx.DrawImage(shapeChunkBitmap, new System.Drawing.Rectangle(0, 0, scaleWidth, scaleHeight));
                    }

                    shapeChunkBitmap = scaledShapeChunkBitmap;
                }

                int textureWidth = textureAppendTo.Bitmap.Width;
                int textureHeight = textureAppendTo.Bitmap.Height;

                int newWidth = -1;
                int newHeight = -1;

                if (textureHeight > shapeChunkBitmap.Height)
                {
                    newWidth = textureWidth + shapeChunkBitmap.Width;
                    newHeight = textureHeight;
                }
                else
                {
                    newWidth = textureWidth + shapeChunkBitmap.Width;
                    newHeight = textureHeight + shapeChunkBitmap.Height;
                }

                finalImage = new Bitmap(newWidth, newHeight);

                using (Graphics grfx = Graphics.FromImage(finalImage))
                {
                    grfx.DrawImage(textureAppendTo.Bitmap, 0, 0);
                    grfx.DrawImage(shapeChunkBitmap, textureWidth, 0);
                }

                PointF[] shapeChunkUVData = (PointF[])shapeChunkInput.UV.Clone();
                float leftWidth = _scFile.GetTextures()[(int)shapeChunkInput.GetTextureId()].Bitmap.Width;
                float topHeight = _scFile.GetTextures()[(int)shapeChunkInput.GetTextureId()].Bitmap.Height;

                for (int i = 0; i < shapeChunkUVData.Length; i++)
                {
                    if (shapeChunkUVData[i].X < leftWidth)
                    {
                        leftWidth = shapeChunkUVData[i].X;
                    }

                    if (shapeChunkUVData[i].Y < topHeight)
                    {
                        topHeight = shapeChunkUVData[i].Y;
                    }
                }

                for (int i = 0; i < shapeChunkUVData.Length; i++)
                {
                    if (leftWidth != 0)
                        shapeChunkUVData[i].X -= leftWidth;

                    if (topHeight != 0)
                        shapeChunkUVData[i].Y -= topHeight;

                    if (scaleFactor != 1)
                    {
                        shapeChunkUVData[i].X *= scaleFactor;
                        shapeChunkUVData[i].Y *= scaleFactor;
                    }

                    shapeChunkUVData[i].X += textureWidth;
                    shapeChunkUVData[i].Y += 0; // TODO 
                }

                shapeChunkInput.SetUV(shapeChunkUVData);

                if (scaleFactor != (float)1)
                {
                    PointF[] shapeChunkXYData = (PointF[])shapeChunkInput.XY.Clone();
                    for (int i = 0; i < shapeChunkXYData.Length; i++)
                    {
                        shapeChunkXYData[i].X *= scaleFactor;
                        shapeChunkXYData[i].Y *= scaleFactor;
                    }
                    shapeChunkInput.SetXY(shapeChunkXYData);
                }
            }

            textureAppendTo.GetImage().SetBitmap(finalImage);
        }

        private void launchProcess(string fileName, string arguements)
        {
            texturePackerProcess.EnableRaisingEvents = true;
            texturePackerProcess.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            texturePackerProcess.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            texturePackerProcess.Exited += new System.EventHandler(process_Exited);

            texturePackerProcess.StartInfo.FileName = fileName;
            texturePackerProcess.StartInfo.Arguments = arguements;
            texturePackerProcess.StartInfo.UseShellExecute = false;
            texturePackerProcess.StartInfo.RedirectStandardError = true;
            texturePackerProcess.StartInfo.RedirectStandardOutput = true;
        }

        private void clearTempFolder()
        {
            DirectoryInfo di = Directory.CreateDirectory(System.IO.Path.GetTempPath() + "sceditor\\chunks");

            if (di.GetFiles().Length != 0)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            if (di.GetDirectories().Length != 0)
            {
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        void process_Exited(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("process exited with code {0}\n", texturePackerProcess.ExitCode.ToString()));
        }

        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
    }
}
