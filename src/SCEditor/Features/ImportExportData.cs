using Newtonsoft.Json.Linq;
using SCEditor.Prompts;
using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SCEditor.Features
{
    public class ImportExportData
    {
        private ScFile _scFile;

        public ImportExportData(ScFile scs)
        {
            _scFile = scs;
        }

        public bool initiateImporting()
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
                                    return false;
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

                                ushort maxId = _scFile.getMaxId();
                                maxId++;

                                // SHAPECHUNK DATA
                                ShapeChunk shapeChunkData = new ShapeChunk(_scFile);
                                shapeChunkData.SetChunkId(0);
                                shapeChunkData.SetTextureId(Convert.ToByte(selectedTexture));
                                shapeChunkData.SetChunkType(22);
                                shapeChunkData.setCustomAdded(true);
                                shapeChunkData.SetOffset(-1);
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
                                shapeData.SetOffset(-1);

                                _scFile.AddShape(shapeData);
                                _scFile.AddChange(shapeData);

                                maxId++;

                                // MOVIECLIP DATA
                                MovieClip movieClipData = new MovieClip(_scFile, 12);

                                if (exportExist)
                                {
                                    movieClipData = (MovieClip)_scFile.GetMovieClips()[_scFile.GetMovieClips().FindIndex(mv => mv.Id == _scFile.GetExports()[exportIndex].GetDataObject().Id)];
                                }
                                else
                                {
                                    movieClipData.SetId(maxId);
                                    movieClipData.SetOffset(-1);
                                    movieClipData.SetDataType(12);
                                    movieClipData.setCustomAdded(true);
                                    movieClipData.SetFramePerSecond(24);
                                    movieClipData.setExportType(_exportType);
                                    movieClipData.setTimelineChildrenId(new ushort[0]);
                                    movieClipData.setTimelineChildrenNames(new string[0]);
                                }

                                movieClipData.SetFrameCount((short)(movieClipData.GetFrames().Count + 1));

                                if (!exportExist && movieClipData.getChildrens().Count == 0 && _exportType == exportType.Animation)
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
                                                    movieClipData.addChildren(_scFile.GetShapes()[shapeIndex]);
                                                    movieClipData.addChildrenId(_scFile.GetShapes()[shapeIndex].Id);
                                                    movieClipData.addChildrenName(null);
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

                                movieClipData.addChildren(shapeData);
                                movieClipData.addChildrenId(shapeData.Id);
                                movieClipData.addChildrenName(null);

                                ushort[] timelineOffset = new ushort[] { 0, 65535, 65535 };

                                if (_exportType == exportType.Icon)
                                {
                                    movieClipData.setIconType(_iconType);

                                    if (_iconType == iconType.Hero)
                                    {
                                        //timelineOffset = new ushort[] { 0, 24353, 65535, 1, 65535, 65535 }; // change after every coc update
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

                                        timelineOffset[timeLineOffsetLength++] = (ushort)(movieClipData.getChildrens().Count - 1);
                                        timelineOffset[timeLineOffsetLength++] = 65535;
                                        timelineOffset[timeLineOffsetLength++] = 65535;
                                    }
                                }

                                movieClipData.setTimelineOffsetArray(timelineOffset);

                                for (int i = 0; i < (_exportType == exportType.Animation ? 2 : 1); i++)
                                {
                                    // MOVIECLIP FRAME DATA
                                    MovieClipFrame movieClipFrameData = new MovieClipFrame(_scFile);

                                    //ushort MovieClipFrameId = (ushort)(_exportType == exportType.Animation ? 2 : _exportType == exportType.Icon ? _iconType == iconType.Hero ? 2 : 1 : 1);
                                    ushort MovieClipFrameId = (ushort)(_exportType == exportType.Animation ? 2 : 1);

                                    movieClipFrameData.SetId(MovieClipFrameId);
                                    movieClipData.AddFrame(movieClipFrameData);
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

                            return true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error");
                            return false;
                        }
                    }
                }
                return false;
            }
        }
    }
}
