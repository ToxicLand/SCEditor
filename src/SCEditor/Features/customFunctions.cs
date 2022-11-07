using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCEditor.Features
{
    public class customFunctions
    {
        private ScFile _scFile { get; set; }

        public customFunctions(ScFile scfile)
        {
            _scFile = scfile;
        }

        public void initObject(ScObject data)
        {
            bool performFunction = false;
            if (!performFunction)
                return;

            TextField tx = new TextField(_scFile, (TextField)data, _scFile.getMaxId());
            tx.customAdded = true;

            _scFile.addTextField(tx);
            _scFile.AddChange(tx);

            Console.WriteLine($"Cloned TextField with id {tx.Id}");

            legendDecoJuneFunction(data);

            legendJuneBloomMovieclip(data);

            Console.WriteLine("Performing Object related custom function");

            if (data.objectType == ScObject.SCObjectType.Export || data.objectType == ScObject.SCObjectType.MovieClip)
            {
                if (data.objectType == ScObject.SCObjectType.Export)
                    data = data.GetDataObject();

                /** LEGEND Rock Deco bloom movieclip
                legendJuneBloomMovieclip(data);
                **/

                /** legend overlay matrix fix
                LegendJuneShieldMovieclipPositionFix(data);
                **/

                List<ushort> newTimelineArray = new List<ushort>();

                for (int i = 0; i < ((MovieClip)data).timelineArray.Length;)
                {
                    newTimelineArray.Add(((MovieClip)data).timelineArray[i]);
                    newTimelineArray.Add(((MovieClip)data).timelineArray[i + 1]);

                    ushort colorId = ((MovieClip)data).timelineArray[i + 2];

                    Tuple<Color, byte, Color> color = _scFile.getColors(((MovieClip)data)._transformStorageId)[colorId];

                    int alphaValue = 254;

                    if ((i / 3) < 127)
                    {
                        alphaValue = (i / 3);
                    }
                    else
                    {
                        alphaValue = 127 - (((i / 3) - 127));
                    }

                    if (alphaValue > 80)
                    {
                        alphaValue = 80;
                    }

                    Color toC = Color.FromArgb(255, 93, 10, 88);
                    Color fromC = Color.FromArgb(255, 93, 10, 88);

                    byte newAlpha = (byte)alphaValue; //(byte)((int)((float)(color.Item2 / 2)));

                    Console.WriteLine($"Alpha Value: {alphaValue}");

                    Tuple<Color, byte, Color> newColor = new Tuple<Color, byte, Color>(toC, newAlpha, fromC);

                    colorId = (ushort)_scFile.getColors(((MovieClip)data)._transformStorageId).Count;

                    _scFile.addColor(newColor, 0);
                    _scFile.addPendingColor(newColor, 0);

                    newTimelineArray.Add(colorId);

                    // new shape values
                    newTimelineArray.Add(1);
                    newTimelineArray.Add(ushort.MaxValue);
                    
                    if ((i / 3) < 127)
                    {
                        alphaValue = (i / 3);
                    }
                    else
                    {
                        alphaValue = 127 - (((i / 3) - 127));
                    }

                    Tuple<Color, byte, Color> newColor2 = new Tuple<Color, byte, Color>(toC, newAlpha, fromC);
                    colorId = (ushort)_scFile.getColors(((MovieClip)data)._transformStorageId).Count;
                    _scFile.addColor(newColor2, 0);
                    _scFile.addPendingColor(newColor2, 0);
                    newTimelineArray.Add(colorId);

                    i += 3;
                }

                List<ScObject> newFramesArray = new List<ScObject>();
                int x = 0;

                MovieClipFrame mvF = new MovieClipFrame(_scFile);
                mvF.SetId(2);
                while (x < ((MovieClip)data).GetFrames().Count)
                {
                    newFramesArray.Add(mvF);
                    x++;
                }
                ((MovieClip)data).SetFrames(newFramesArray);

                ((MovieClip)data).setTimelineOffsetArray(newTimelineArray.ToArray());
                _scFile.AddChange(data);
            }

            Console.WriteLine("Object related custom function performed!");
        }

        public void initNonObject()
        {
            //fixBoomBeachTanks();

            //makeFlagExports();

            /**
            ScObject newShape = _scFile.GetShapes().Where(s => s.Id == 6).FirstOrDefault();
            newShape.setCustomAdded(true);
            ((Shape)newShape).SetId(_scFile.getMaxId());

            _scFile.AddShape((Shape)newShape);
            _scFile.AddChange(newShape);
            **/

            bool performFunction = false;
            if (!performFunction)
                return;

            Console.WriteLine("Performing custom function");

            if (true) // duplicate textfield for legend june deco position overlay on shield
            {
                for (int i = 0; i < 3; i++)
                {
                    ScObject txX = _scFile.getTextFields().Where(tx => tx.Id == 57).FirstOrDefault();

                    ScObject tx = new TextField(_scFile, (TextField)txX, _scFile.getMaxId());

                    ((TextField)tx)._textData = (i + 1).ToString();
                    ((TextField)tx).customAdded = true;

                    _scFile.addTextField(tx);
                    _scFile.AddChange(tx);

                    Console.WriteLine(tx.Id);
                }
                
            }

            Console.WriteLine("Custom function performed!");
        }

        private void fixBoomBeachTanks()
        {
            Dictionary<string, Dictionary<string, List<Export>>> allTanksExports = new Dictionary<string, Dictionary<string, List<Export>>>();

            foreach (Export export in _scFile.GetExports())
            {
                if (export.GetName().Contains("tank_"))
                {
                    string[] splitName = export.GetName().Split('_');
                    string directionValue = splitName[splitName.Length - 1];

                    if (!allTanksExports.ContainsKey(splitName[0]))
                    {
                        allTanksExports.Add(splitName[0], new Dictionary<string, List<Export>>());
                    }

                    if (!allTanksExports[splitName[0]].ContainsKey(splitName[splitName.Length - 2]))
                    {
                        allTanksExports[splitName[0]].Add(splitName[splitName.Length - 2], new List<Export>());
                    }

                    allTanksExports[splitName[0]][splitName[splitName.Length - 2]].Add(export);
                }
            }
            
            foreach (var (tankName, tankTypesData) in allTanksExports)
            {
                

                foreach (var (animTypeName, animTypesList) in tankTypesData)
                {
                    List<Export> orderedList = animTypesList.OrderBy(x => Regex.Replace(x.GetName(), @"\d+", i => i.Value.PadLeft(2, '0'))).ToList();

                    int lastDirectionIndexUsed = 0;
                    foreach (Export eachDirectionAnimation in orderedList)
                    {
                        MovieClip mainMV = (MovieClip)eachDirectionAnimation.GetDataObject();

                        int barrelIndex = mainMV.timelineChildrenNames.ToList().FindIndex(x => x.ToLower() == "barrel");
                        int turretIndex = mainMV.timelineChildrenNames.ToList().FindIndex(x => x.ToLower() == "turret");

                        MovieClip turretMV = ((MovieClip)_scFile.GetMovieClips()[mainMV.timelineChildrenId[turretIndex]]);
                        MovieClip barrelMV = null;

                        if (barrelIndex != -1)
                            barrelMV = ((MovieClip)_scFile.GetMovieClips()[mainMV.timelineChildrenId[barrelIndex]]);

                        MovieClip turretMovieClip = new MovieClip(_scFile, mainMV.GetMovieClipDataType());
                        turretMovieClip.customAdded = true;
                        turretMovieClip.SetFramePerSecond(turretMV.FPS);
                        turretMovieClip._transformStorageId = turretMV._transformStorageId;

                        List<MovieClipFrame> framesArray = ((MovieClipFrame[])turretMV.Frames.ToArray().Clone()).ToList();
                        List<ushort> timelineOffsetArray = ((ushort[])turretMV.timelineArray.Clone()).ToList();
                        List<ushort> timelineChildrenIdsArray = ((ushort[])turretMV.timelineChildrenId.Clone()).ToList();
                        List<string> timelineChildrenNamesArray = ((string[])turretMV.timelineChildrenNames.Clone()).ToList();

                        for (int i = 0; i < timelineChildrenIdsArray.Count; i++)
                        {
                            ushort childId = timelineChildrenIdsArray[i];

                            // looping through each child id - remove extra ones and keep the shape and extra movieclips needed
                        }

                       byte[] flagsArray =  new byte[timelineChildrenIdsArray.Count];

                    }
                }
            }
        }

        private void removeChildrenData(int currentIndex, List<ushort> _timelineArray, List<ushort> _childrenIds, List<string> _childrenNames, List<MovieClipFrame> _frames)
        {
            _childrenIds.RemoveAt(currentIndex);
            _childrenNames.RemoveAt(currentIndex);

            int totalPassed = 0;
            for (int frameIndex = 0; frameIndex < _frames.Count; frameIndex++)
            {
                MovieClipFrame mvFrame = (MovieClipFrame)_frames[frameIndex];

                for (int i = 0; i < mvFrame.Id; i++)
                {
                    ushort childrenIndex = _timelineArray[(totalPassed * 3) + (i * 3)];

                    if (childrenIndex == currentIndex)
                    {
                        mvFrame.SetId((ushort)(mvFrame.Id - 1));

                        _timelineArray.RemoveRange(((totalPassed * 3) + (i * 3)), 3);

                        i--;
                    }

                    if (childrenIndex > currentIndex)
                    {
                        _timelineArray[(totalPassed * 3) + (i * 3)] = (ushort)(childrenIndex - 1);
                    }
                }

                if (mvFrame.Id == 0)
                {
                    _frames.RemoveAt(frameIndex);
                    frameIndex--;
                    continue;
                }

                totalPassed += mvFrame.Id;
            }
        }

        private void makeFlagExports()
        {
            ushort matrixId = (ushort)_scFile.GetMatrixs(0).Count;
            Matrix oldM = _scFile.GetMatrixs(0)[2984];

            oldM.Multiply(new Matrix(1f, 0f, 0f, 1f, 0.8f, 1.2f));
            _scFile.addPendingMatrix(oldM, 0);
            _scFile.addMatrix(oldM, 0);

            foreach (ScObject scObject in _scFile.GetExports())
            {
                if (scObject.GetName().StartsWith("deco_") && scObject.GetName().EndsWith("_flag"))
                {
                    MovieClip mv = (MovieClip)scObject.GetDataObject();

                    ushort[] tarr = mv.timelineArray;
                    tarr[4] = matrixId;

                    mv.setTimelineOffsetArray(tarr);

                    _scFile.AddChange(mv);
                }
            }


            return;
            Dictionary<string, List<ScObject>> flagExports = new Dictionary<string, List<ScObject>>();

            Export sample = (Export)_scFile.GetExports().Where(x => x.GetName() == "deco_turkey_flag").FirstOrDefault();
            MovieClip wavingFlag = (MovieClip)_scFile.GetMovieClips().Where(x => x.Id == 48).FirstOrDefault();

            if (sample is null || wavingFlag is null)
                return;

            foreach (ScObject scObject in _scFile.GetExports())
            {
                if (scObject.GetName().Contains("deco_flag"))
                {
                    string flagCountry = scObject.GetName().Replace("deco_flag_", "");
                    flagCountry = flagCountry.Substring(0, flagCountry.Length - 2);

                    if (!flagExports.ContainsKey(flagCountry))
                        flagExports.Add(flagCountry, new List<ScObject>());

                    flagExports[flagCountry].Add(scObject);
                }
            }

            Matrix flagMatrix = new Matrix(0.624f, -0.0322f, 0.0322f, 0.624f, -0.412f, -36.377f);
            _scFile.addMatrix(flagMatrix, 0);
            _scFile.addPendingMatrix(flagMatrix, 0);

            foreach (var (name, data) in flagExports)
            {
                MovieClip flagMV = new MovieClip((MovieClip)sample.GetDataObject());

                _scFile.AddMovieClip(flagMV);
                _scFile.AddChange(flagMV);

                ushort[] flagMVTimeline = flagMV.timelineArray;
                flagMVTimeline[4] = (ushort)(_scFile.GetMatrixs(0).Count - 1);

                Export flagExport = new Export(_scFile);
                flagExport.Rename($"deco_{name}_flag");
                flagExport.SetId(flagMV.Id);
                flagExport.setCustomAdded(true);
                flagExport.SetDataObject(flagMV);

                _scFile.AddExport(flagExport);
                _scFile.AddChange(flagExport);

                MovieClip newWavingFlag = new MovieClip(wavingFlag);

                _scFile.AddMovieClip(newWavingFlag);
                _scFile.AddChange(newWavingFlag);

                ushort[] childrenID = new ushort[5];
                foreach (ScObject eachFlagFrame in data)
                {
                    childrenID[int.Parse(eachFlagFrame.GetName().Substring(eachFlagFrame.GetName().Length - 1, 1))] = eachFlagFrame.GetDataObject().Children[0].Id;
                }
                newWavingFlag.setTimelineChildrenId(childrenID);

                ushort[] childrenIDMain = new ushort[2] { flagMV.timelineChildrenId[0], newWavingFlag.Id };
                flagMV.setTimelineChildrenId(childrenIDMain);
            }

            Console.WriteLine("flag done");
        }
        private void legendDecoJuneFunction(ScObject data)
        {
            if (data.objectType == ScObject.SCObjectType.Export)
                data = data.GetDataObject();

            List<ushort> newTimelineArray = new List<ushort>();
            List<ScObject> newFramesArray = new List<ScObject>();

            MovieClipFrame newMVFrame = new MovieClipFrame(_scFile) { customAdded = true };
            newMVFrame.SetId(3);

            int alphaValueShape1 = 1;

            for (int colorIndex = 0; colorIndex < 120; colorIndex++)
            {
                if (colorIndex > 60)
                {
                    alphaValueShape1 = (int)(alphaValueShape1 - 3);
                }
                else
                {
                    alphaValueShape1 = (int)(alphaValueShape1 + 3);
                }

                // original shape
                newTimelineArray.Add(0);
                newTimelineArray.Add(ushort.MaxValue);
                newTimelineArray.Add(ushort.MaxValue);

                // red alpha
                Tuple<Color, byte, Color> redAlpha = new Tuple<Color, byte, Color>(Color.FromArgb(255, 20, 20), (byte)alphaValueShape1, Color.FromArgb(0, 0, 0));

                newTimelineArray.Add(1);
                newTimelineArray.Add(ushort.MaxValue);

                int redColorIdx = _scFile.getColors(((MovieClip)data)._transformStorageId).FindIndex(c => c.Equals(redAlpha));
                if (redColorIdx == -1)
                {
                    redColorIdx = _scFile.getColors(((MovieClip)data)._transformStorageId).Count;

                    _scFile.addColor(redAlpha, 0);
                    _scFile.addPendingColor(redAlpha, 0);
                }

                newTimelineArray.Add((ushort)redColorIdx);

                // blue alpha
                Tuple<Color, byte, Color> blueAlpha = new Tuple<Color, byte, Color>(Color.FromArgb(20, 20, 255), (byte)alphaValueShape1, Color.FromArgb(0, 0, 0));

                newTimelineArray.Add(2);
                newTimelineArray.Add(ushort.MaxValue);

                int blueColorIdx = _scFile.getColors(((MovieClip)data)._transformStorageId).FindIndex(c => c.Equals(blueAlpha));
                if (blueColorIdx == -1)
                {
                    blueColorIdx = _scFile.getColors(((MovieClip)data)._transformStorageId).Count;

                    _scFile.addColor(blueAlpha, 0);
                    _scFile.addPendingColor(blueAlpha, 0);
                }

                newTimelineArray.Add((ushort)blueColorIdx);

                newFramesArray.Add(newMVFrame);
            }

            ((MovieClip)data).setTimelineOffsetArray(newTimelineArray.ToArray());
            ((MovieClip)data).SetFrames(newFramesArray);
            ((MovieClip)data).SetFramePerSecond(18); // check

            _scFile.AddChange(data);
        }

        private void legendJuneBloomMovieclip(ScObject data)
        {
            byte alphaValueShape1 = 1;
            byte alphaValueShape2 = 1;

            List<ushort> newTimelineArray = new List<ushort>();
            List<ScObject> newFramesArray = new List<ScObject>();

            MovieClipFrame newMVFrame = new MovieClipFrame(_scFile) { customAdded = true };
            newMVFrame.SetId(2);

            for (int colorIndex = 0; colorIndex < 120; colorIndex++)
            {
                if (colorIndex > 60)
                {
                    alphaValueShape1 -= 1;
                    alphaValueShape2 -= 2;

                    if (alphaValueShape2 < alphaValueShape1)
                        alphaValueShape2 = alphaValueShape1;
                }
                else
                {
                    alphaValueShape1 += 1;
                    alphaValueShape2 += 2;
                }

                Console.WriteLine($"S1: {alphaValueShape1}");

                //Shape 1
                Tuple<Color, byte, Color> newColorShape1 = new Tuple<Color, byte, Color>(Color.FromArgb(172, 18, 152), alphaValueShape1, Color.FromArgb(255, 255, 255));
                
                newTimelineArray.Add(0);
                newTimelineArray.Add(ushort.MaxValue);
                newTimelineArray.Add((ushort)_scFile.getColors(((MovieClip)data)._transformStorageId).Count);

                _scFile.addColor(newColorShape1, 0);
                _scFile.addPendingColor(newColorShape1, 0);

                //Shape 2
                Tuple<Color, byte, Color> newColorShape2 = new Tuple<Color, byte, Color>(Color.FromArgb(172, 18, 152), alphaValueShape2, Color.FromArgb(172, 18, 152));
                
                newTimelineArray.Add(1);
                newTimelineArray.Add(ushort.MaxValue);
                newTimelineArray.Add((ushort)_scFile.getColors(((MovieClip)data)._transformStorageId).Count);

                _scFile.addColor(newColorShape2, 0);
                _scFile.addPendingColor(newColorShape2, 0);

                newFramesArray.Add(newMVFrame);
            }

            ((MovieClip)data).setTimelineOffsetArray(newTimelineArray.ToArray());
            ((MovieClip)data).SetFrames(newFramesArray);
            ((MovieClip)data).SetFramePerSecond(24);
        }

        private void LegendJuneShieldMovieclipPositionFix(ScObject data)
        {
            int rightVal = 135;
            int downVal = 60;
            int clockwiseVal = 17;
            float increaseVal = 1.7F;

            List<ushort> newTimelineArray = new List<ushort>();

            int timelineIndex = 0;
            while (timelineIndex < ((MovieClip)data).timelineArray.Length)
            {
                newTimelineArray.Add(((MovieClip)data).timelineArray[timelineIndex]);

                ushort matrixId = ((MovieClip)data).timelineArray[timelineIndex + 1];

                if (matrixId != ushort.MaxValue)
                {
                    Matrix matrixData = _scFile.GetMatrixs(0)[matrixId];

                    Matrix newMatrix = new Matrix(1, 0, 0, 1, 0, 0);

                    //increase
                    newMatrix.Elements.SetValue((newMatrix.Elements[1] + increaseVal), 1);
                    newMatrix.Elements.SetValue((newMatrix.Elements[4] + increaseVal), 4);
                    float scaleUp = increaseVal < 1 ? 1 + increaseVal : increaseVal;
                    newMatrix.Scale(scaleUp, scaleUp);

                    // right
                    newMatrix.Elements.SetValue((newMatrix.Elements[3] + rightVal), 3);
                    newMatrix.Translate(rightVal, 0);

                    // down
                    newMatrix.Elements.SetValue((newMatrix.Elements[5] > 0 ? newMatrix.Elements[5] - downVal : ((Math.Abs(newMatrix.Elements[5]) + downVal) * -1)), 5);
                    newMatrix.Translate(0, downVal);

                    //clockwise
                    newMatrix.Rotate(clockwiseVal);

                    matrixId = (ushort)_scFile.GetMatrixs(0).Count;

                    newMatrix.Multiply(matrixData);

                    _scFile.addMatrix(newMatrix, (_scFile.GetTransformStorage().Count - 1));
                    _scFile.addPendingMatrix(newMatrix, (_scFile.GetTransformStorage().Count - 1));
                }

                newTimelineArray.Add(matrixId);

                newTimelineArray.Add(((MovieClip)data).timelineArray[timelineIndex + 2]);

                timelineIndex += 3;
            }

            ((MovieClip)data).setTimelineOffsetArray(newTimelineArray.ToArray());

            _scFile.AddChange(data);
        }
    }
}
