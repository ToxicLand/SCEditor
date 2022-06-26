using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
            legendDecoJuneFunction(data);

            bool performFunction = false;
            if (!performFunction)
                return;

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
                ScObject tx = _scFile.getTextFields().Where(tx => tx.Id == 7415).FirstOrDefault();

                ((TextField)tx).setId(_scFile.getMaxId());
                ((TextField)tx)._textData = "1";
                ((TextField)tx).customAdded = true;

                _scFile.addTextField(tx);
                _scFile.AddChange(tx);

                Console.WriteLine(tx.Id);
            }

            Console.WriteLine("Custom function performed!");
        }

        private void legendDecoJuneFunction(ScObject data)
        {
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
                Tuple<Color, byte, Color> redAlpha = new Tuple<Color, byte, Color>(Color.FromArgb(255, 80, 80), (byte)alphaValueShape1, Color.FromArgb(255, 255, 255));

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
                Tuple<Color, byte, Color> blueAlpha = new Tuple<Color, byte, Color>(Color.FromArgb(80, 80, 255), (byte)alphaValueShape1, Color.FromArgb(255, 255, 255));

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
