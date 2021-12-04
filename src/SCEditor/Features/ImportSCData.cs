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
                string bbBuildings = "tower_turret_lvl1;tower_turret_lvl2;tower_turret_lvl3;tower_turret_lvl4;tower_turret_lvl5;tower_turret_lvl6;tower_turret_lvl7;tower_turret_lvl8;tower_turret_lvl10;tower_turret_lvl11;tower_turret_lvl12;tower_turret_lvl13;tower_turret_lvl14;tower_turret_lvl15;tower_turret_lvl16;tower_turret_lvl17;tower_turret_lvl18;tower_turret_lvl19;tower_turret_lvl21;tower_turret_lvl22;mortar_lvl1;mortar_lvl2;mortar_lvl3;mortar_lvl4;mortar_lvl5;mortar_lvl6;mortar_lvl7;mortar_lvl8;mortar_lvl9;mortar_lvl11;mortar_lvl12;mortar_lvl13;mortar_lvl14;mortar_lvl16;mortar_lvl21;mortar_lvl22;mortar_lvl23;machinegun_turret_lvl1;machinegun_turret_lvl2;machinegun_turret_lvl3;machinegun_turret_lvl4;machinegun_turret_lvl5;machinegun_turret_lvl6;machinegun_turret_lvl8;machinegun_turret_lvl9;machinegun_turret_lvl11;machinegun_turret_lvl12;machinegun_turret_lvl21;machinegun_turret_lvl22;missile_launcher_lvl1;missile_launcher_lvl2;missile_launcher_lvl3;missile_launcher_lvl5;missile_launcher_lvl6;missile_launcher_lvl7;missile_launcher_lvl9;missile_launcher_lvl10;flame_thrower_lvl1;flame_thrower_lvl2;flame_thrower_lvl5;flame_thrower_lvl6;flame_thrower_lvl7;flame_thrower_lvl9;flame_thrower_lvl10;flame_thrower_lvl11;flame_thrower_lvl12;flame_thrower_lvl14;flame_thrower_lvl15;flame_thrower_lvl18;flame_thrower_lvl19;flame_thrower_lvl20;basic_cannon_lvl1;basic_cannon_lvl2;basic_cannon_lvl3;basic_cannon_lvl4;basic_cannon_lvl5;basic_cannon_lvl6;basic_cannon_lvl10;basic_cannon_lvl11;basic_cannon_lvl12;basic_cannon_lvl13;basic_cannon_lvl14;basic_cannon_lvl15;basic_cannon_lvl16;basic_cannon_lvl19;basic_cannon_lvl21;basic_cannon_lvl22;boom_cannon_lvl1;boom_cannon_lvl2;boom_cannon_lvl3;boom_cannon_lvl4;boom_cannon_lvl5;boom_cannon_lvl6;boom_cannon_lvl7;boom_cannon_lvl8;boom_cannon_lvl9;boom_cannon_lvl10;boom_cannon_lvl11;boom_cannon_lvl12;boom_cannon_lvl13;boom_cannon_lvl15;boom_cannon_lvl16;shock_launcher_lvl1;shock_launcher_lvl2;shock_launcher_lvl3;shock_launcher_lvl4;shock_launcher_lvl5;shock_launcher_lvl6;shock_launcher_lvl7;shock_launcher_lvl8;shock_minigun_lvl1;shock_minigun_lvl2;shock_minigun_lvl3;lazer_lvl1;lazer_lvl2;lazer_lvl3;attack_booster_lvl1;attack_booster_lvl2;attack_booster_lvl3;doom_cannon_lvl1;doom_cannon_lvl2;doom_cannon_lvl3;shieldgenerator_lvl1;shieldgenerator_lvl2;shieldgenerator_lvl3;harpoon_lvl1;harpoon_lvl2;harpoon_lvl3;flame_surprise_lvl1;flame_surprise_lvl1_appear;flame_surprise_lvl1_hide;flame_surprise_lvl1_hatch;flame_surprise_lvl2;flame_surprise_lvl2_appear;flame_surprise_lvl2_hide;flame_surprise_lvl2_hatch;flame_surprise_lvl3;flame_surprise_lvl3_appear;flame_surprise_lvl3_hide;flame_surprise_lvl3_hatch;super_sniper_lvl1;super_sniper_lvl2;super_sniper_lvl3;microwave_tower_lvl1;microwave_tower_lvl2;microwave_tower_lvl3;bubble_generator_lvl1;bubble_generator_lvl2;bubble_generator_lvl3;cannon_surprise_lvl1_appear;cannon_surprise_lvl1_hide;cannon_surprise_lvl1_hatch;cannon_surprise_lvl2_appear;cannon_surprise_lvl2_hide;cannon_surprise_lvl2_hatch;cannon_surprise_lvl3_appear;cannon_surprise_lvl3_hide;cannon_surprise_lvl3_hatch;bomb_tower_lvl1;bomb_tower_lvl2;bomb_tower_lvl3;bomb_tower_triggered;bomb_tower_triggered_2;bomb_tower_triggered_3;hidden_cannon_lvl1;hidden_cannon_lvl2;hidden_cannon_lvl3;freezeray_beam;laser_beam_glow";

                List<string> crTroopsList = crTroops.Split(';').ToList();
                List<string> bbTroopsList = bbTroops.Split(';').ToList();
                List<string> bbBuildingsList = bbBuildings.Split(';').ToList();

                foreach (scMergeSelection.exportItemClass item in selectImportExportsForm.checkedExports)
                {
                    Export exportToAdd = (Export)((ScObject)item.exportData);
                    bool crChangeName = false;
                    bool bbChangeName = false;
                    bool bbBuildingChangeName = true;

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

                        /**
                        if (int.TryParse(exportToAdd.GetName().Substring(exportToAdd.GetName().Length - 2, 2),out int _))
                        {
                            int value = int.Parse(exportToAdd.GetName().Substring(exportToAdd.GetName().Length - 2, 2));

                            string newValue = $"{(14 - value)}";

                            exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 2, 2) + newValue);
                        } 
                        else if (int.TryParse(exportToAdd.GetName().Substring(exportToAdd.GetName().Length - 1, 1), out int _))
                        {
                            int value = int.Parse(exportToAdd.GetName().Substring(exportToAdd.GetName().Length - 1, 1));

                            string newValue = $"{(14 - value)}";

                            exportToAdd.SetExportName(exportToAdd.GetName().Remove(exportToAdd.GetName().Length - 1, 1) + newValue);
                        }
                        **/
                    }

                    if (bbBuildingChangeName)
                    {
                        exportToAdd.SetExportName("bb_" + exportToAdd.GetName());
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
            newShape.setLength(shapeToAdd.length);

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
                            newTexToAdd.setCustomAdded(true);
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

            if (_queriesToPerform.ContainsKey("contains"))
            {
                foreach (var (key, value) in _queriesToPerform["contains"])
                {
                    if (_currentExportName.Contains(key))
                    {
                        if (value.ContainsKey("scale"))
                            scaleFactor = value["scale"];

                        break;
                    }
                }
            }

            bool useScaleFactor = false;
            if (scaleFactor != 1)
            {
                useScaleFactor = true;

                if (shapeChunkBitmap.Width <= 1 || shapeChunkBitmap.Height <= 1)
                {
                    scaleFactor = 1;
                    useScaleFactor = false;
                }

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
                shapeChunksTMatrix[shapeChunkTMatrixIndex].Item2.Add(getShapeChunkMatrixTransformation(shapeChunkToAdd, useScaleFactor));
            }
            else
            {
                List<Matrix> newList = new List<Matrix>();
                newList.Add(getShapeChunkMatrixTransformation(shapeChunkToAdd, useScaleFactor));
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
            bool isGeneratedTextureMaxRects = true;

            if (_createNewTexture)
            {
                inputDataDialog textureTypeDialog = new inputDataDialog(1);
                textureTypeDialog.setLabelText("Texture Type (0=RGBA8888, 1=RGBA4444):");

                while (true)
                {
                    if (textureTypeDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (textureTypeDialog.inputTextBoxInt != 0 && textureTypeDialog.inputTextBoxInt != 1)
                        {
                            MessageBox.Show("Please Type 0 for RGBA8888 or 1 for RGBA4444", "Invalid Texture Type");
                        }
                        else
                        {
                            if (textureTypeDialog.inputTextBoxInt == 1)
                                isGeneratedTextureRGBA4444 = true;

                            break;
                        }
                    }
                }
            }
            else
            {
                string textureTypeName = ((Texture)_scFile.GetTextures()[_textureToImportToID])._image.GetImageTypeName();
                isGeneratedTextureRGBA4444 = textureTypeName == "RGB4444" ? true : (textureTypeName == "RGB8888" ? false : throw new Exception("Not added"));
            } 

            int generatedTextureScale = 1;
            int generatedSpritesExtrude = 0;
            int generatedSpritesPadding = 3;
            int generatedTextureMaxWidth = 4096;
            int generatedTextureMaxHeight = 4096;
            int generatedTexturePolygonTolerance = 190;
            string generatedTexturePixelFormat = isGeneratedTextureRGBA4444 ? "RGBA4444 --dither-type Linear" : "RGBA8888";
            string generatedSpritesPackMode = "Best";
            string generatedSpritesAlphaHandling = "KeepTransparentPixels"; //"ClearTransparentPixels";
            string generatedTextureAlgorithim = isGeneratedTextureMaxRects ? "MaxRects --maxrects-heuristics Best" : "Polygon";

            string arguements = $"--scale {generatedTextureScale} --extrude {generatedSpritesExtrude} --texture-format png --pack-mode {generatedSpritesPackMode} --algorithm {generatedTextureAlgorithim} --alpha-handling {generatedSpritesAlphaHandling} --shape-padding {generatedSpritesPadding} --trim-mode Polygon --png-opt-level 0 --opt RGBA8888 --tracer-tolerance {generatedTexturePolygonTolerance} --multipack --disable-rotation --max-width {generatedTextureMaxWidth} --max-height {generatedTextureMaxHeight} --format json-array" + " --data \"" + tempFolder + "\\output\\data{n1}.json\" \"" + tempFolder + "\"";

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
                if (!File.Exists(tempFolder + "\\output\\data-1.json"))
                    throw new Exception("data file not found.");

                string jsonFileData = File.ReadAllText(tempFolder + "\\output\\data-1.json");
                JObject jsonParsedData = JObject.Parse(jsonFileData);

                int texturesCount = 1;
                if ((JArray)jsonParsedData["related_multi_packs"] != null)
                {
                    texturesCount = ((JArray)jsonParsedData["related_multi_packs"]).Count();
                    for (int i = 0; i < texturesCount; i++)
                    {
                        if (!File.Exists(tempFolder + "\\output\\data-" + (i + 1) + ".json"))
                            throw new Exception("data file not found.");

                        if (!File.Exists(tempFolder + "\\output\\data-" + (i + 1) + ".png"))
                            throw new Exception("data texture file not found.");
                    }
                }

                // Create New Texture
                if (_createNewTexture)
                {
                    for (int i = 0; i < texturesCount; i++)
                    {
                        Texture newTex = new Texture((byte)(isGeneratedTextureRGBA4444 == false ? 0 : 2), 1, 1, _scFile);
                        newTex.setCustomAdded(true);
                        _scFile.AddTexture(newTex);
                        _scFile.AddChange(newTex);

                        using (FileStream stream = new FileStream(tempFolder + ("\\output\\data-" + (i + 1) + ".png"), FileMode.Open, FileAccess.Read))
                        {
                            Bitmap newTexture = (Bitmap)Image.FromStream(stream);
                            ((Texture)_scFile.GetTextures()[(_textureToImportToID + 1)]).GetImage().SetBitmap(newTexture);
                        }
                    }
                }

                JArray framesData = new JArray();
                if (texturesCount > 1)
                {
                    for (int i = 0; i < texturesCount; i++)
                    {
                        string jsonFileDataX = File.ReadAllText(tempFolder + "\\output\\data-" + (i + 1) + ".json");
                        JObject jsonParsedDataX = JObject.Parse(jsonFileData);

                        for (int fi = 0; fi < jsonParsedDataX["frames"].Count(); fi++)
                        {
                            JObject item = ((JObject)((JArray)jsonParsedDataX["frames"])[fi]);
                            item.Add("textureCount", i);

                            framesData.Add(item);
                        }
                    }
                }
                else
                {
                    framesData = (JArray)jsonParsedData["frames"];
                }

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

                    int chunkTextureId = _textureToImportToID;
                    if (texturesCount > 1)
                        chunkTextureId += int.Parse((string)data["textureCount"]);

                    shapeChunkToEdit.SetTextureId((byte)chunkTextureId);
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

        private Matrix getShapeChunkMatrixTransformation(ShapeChunk shapeChunkIN, bool useScaleFactor)
        {
            PointF[] shapeChunkUVData = (PointF[])shapeChunkIN.UV.Clone();
            PointF[] shapeChunkXYData = (PointF[])shapeChunkIN.XY.Clone();
            float leftWidth = scToImportFrom.GetTextures()[shapeChunkIN.GetTextureId()].Bitmap.Width;
            float topHeight = scToImportFrom.GetTextures()[shapeChunkIN.GetTextureId()].Bitmap.Height;
            float scaleFactor = _scaleFactor;

            if (!useScaleFactor)
                scaleFactor = 1;

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

                if (scaleFactor != 1)
                {
                    shapeChunkUVData[i].X *= scaleFactor;
                    shapeChunkUVData[i].Y *= scaleFactor;
                }
            }

            if (scaleFactor != 1)
            {
                for (int i = 0; i < shapeChunkXYData.Length; i++)
                {

                    shapeChunkXYData[i].X *= scaleFactor;
                    shapeChunkXYData[i].Y *= scaleFactor;
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
                return;
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
