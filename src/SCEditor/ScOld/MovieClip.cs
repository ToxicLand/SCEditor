using MathNet.Numerics.LinearAlgebra;
using SCEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SCEditor.ScOld
{
    public sealed class MovieClip : ScObject
    {
        #region Constructors

        MovieClip()
        {
            _transformStorageId = 0;
        }

        public MovieClip(ScFile scs , short dataType)
        {
            _scFile = scs;
            _dataType = dataType;
            _childrens = new List<ScObject>();
            _frames = new List<ScObject>();
        }

        public MovieClip(MovieClip mv)
        {
            _scFile = mv.GetStorageObject();
            _dataType = mv.GetMovieClipDataType();
            _childrens = mv.getChildrens();
            _frames = mv.GetFrames();

            this.SetOffset(-System.Math.Abs(mv.offset));

            //Duplicate MovieClip
            using (FileStream input = new FileStream(_scFile.GetInfoFileName(), FileMode.Open))
            {
                input.Seek(System.Math.Abs(mv.offset), SeekOrigin.Begin);
                using (var br = new BinaryReader(input))
                {

                    var packetId = br.ReadByte().ToString("X2");
                    var packetSize = br.ReadUInt32();
                    this.Read(br, packetId);
                }
            }

            //Set new clip id
            ushort maxMovieClipId = this.Id;
            foreach (MovieClip clip in _scFile.GetMovieClips())
            {
                if (clip.Id > maxMovieClipId)
                    maxMovieClipId = clip.Id;
            }
            maxMovieClipId++;
            this.SetId(maxMovieClipId);

            //Get max shape id
            ushort maxShapeId = 30000; //avoid collision with other objects in MovieClips
            foreach (Shape shape in _scFile.GetShapes())
            {
                if (shape.Id > maxShapeId)
                    maxShapeId = shape.Id;
            }
            maxShapeId++;

            //Duplicate shapes associated to clip
            List<ScObject> newShapes = new List<ScObject>();
            foreach (Shape s in _childrens)
            {
                throw new NotImplementedException();

                //Shape newShape = new Shape(s);
                //newShape.SetId(maxShapeId);
                //maxShapeId++;
                //newShapes.Add(newShape);

                //_scFile.AddShape(newShape); //Add to global shapelist
                //_scFile.AddChange(newShape);
            }
            this._childrens = newShapes;
        }

        #endregion

        #region Fields & Properties

        private short _dataType;
        private ushort _clipId;
        private short _frameCount;
        private byte[] _flags;
        private byte _framePerSeconds;
        private List<ScObject> _childrens;
        private ScFile _scFile;
        private List<ScObject> _frames;
        private ushort _timelineChildrenCount;
        private ushort[] _timelineChildrenId;
        private Rect _scalingGrid;
        public byte _transformStorageId { get; set; } // change
        private string[] _timelineChildrenNames;
        private int _timelineOffsetCount;
        private ushort[] _timelineOffsetArray;
        private exportType _exportType;
        private iconType _iconType;
        private animationType _animationType;
        private bool _hasShadow;
        private List<PointF> _pointFList;
        public override ushort Id => _clipId;
        public override List<ScObject> Children => _childrens;
        public List<ScObject> Frames => _frames;
        public ushort[] timelineArray => _timelineOffsetArray;
        public bool hasShadow => _hasShadow;
        public byte FPS => _framePerSeconds;
        public int _lastPlayedFrame { get; set; }
        public override SCObjectType objectType => SCObjectType.MovieClip;
        private string _packetId;

        #endregion

        #region Methods

        public override int GetDataType()
        {
            return 1;
        }

        public override string GetDataTypeName()
        {
            return "MovieClips";
        }

        public short GetMovieClipDataType()
        {
            return _dataType;
        }

        public List<ScObject> getChildrens()
        {
            return _childrens;
        }

        public ScFile GetStorageObject()
        {
            return _scFile;
        }

        public string findExportnames(uint clipID)
        {
            var exportResult = "";
            //exportNames[exportIDs.IndexOf(clipID)]
            var result = Enumerable.Range(0, _scFile.GetExports().Count).Where(i => _scFile.GetExports()[i].Id == clipID).ToList();
            bool isfirst = true;
            foreach (var item in result)
            {
                if (!isfirst)
                    exportResult += "-";
                else
                    isfirst = false;
                exportResult += _scFile.GetExports()[item].GetName();
            }
            return exportResult;
        }

        public unsafe override ushort ReadMV(BinaryReader br, string packetId, uint length)
        {
            _clipId = br.ReadUInt16();
            _framePerSeconds = br.ReadByte();
            _frameCount = br.ReadInt16();
            _length = length;
            _packetId = packetId;

            if (packetId == "0E")
            {
                throw new Exception("TAG_MOVIE_CLIP_4 no longer supported");
            }

            if (packetId == "03")
            {
                throw new Exception("TAG_MOVIE_CLIP no longer supported");
            }

            _timelineOffsetCount = br.ReadInt32() * 3;

            _timelineOffsetArray = new ushort[_timelineOffsetCount];
            for (int i = 0; i < _timelineOffsetCount / 3; i++)
            {
                _timelineOffsetArray[3 * i] = br.ReadUInt16();
                _timelineOffsetArray[3 * i + 1] = br.ReadUInt16();
                _timelineOffsetArray[3 * i + 2] = br.ReadUInt16();
            }

            for (int i = 0; i < _frameCount; i++)
            {
                _frames.Add(new MovieClipFrame(_scFile));
            }

            ushort count = br.ReadUInt16();

            _timelineChildrenCount = count;
            _timelineChildrenId = new ushort[count];
            br.Read(MemoryMarshal.Cast<ushort, byte>((Span<ushort>)_timelineChildrenId));
            
            if (packetId == "0C" || packetId == "23")
            {
                _flags = new byte[count];
                br.Read(_flags, 0, count);
            }
            else
            {
                _flags = new byte[count];
            }

            _timelineChildrenNames = new string[count];

            bool hasFrameLabel = false;

            for (int i = 0; i < count; i++)
            {
                byte stringLength = br.ReadByte();
                string timelinechildname = null;
                if (stringLength < 255)
                {
                    timelinechildname = Encoding.ASCII.GetString(br.ReadBytes(stringLength));
                    _timelineChildrenNames[i] = timelinechildname;
                }
                hasFrameLabel |= timelinechildname != null;
            }

            int index = 0;
            int timelineOffsetIndex = 0;

            while (true)
            {
                byte frameTag;
                int frameLength;

                while (true)
                {
                    frameTag = br.ReadByte();
                    frameLength = br.ReadInt32();
                    if (frameTag < 0)
                        throw new Exception("Negative tag length in MovieClip. Tag " + frameTag);

                    switch (frameTag)
                    {
                        case 0:
                            //MovieClipFrame movieClipFrame = (MovieClipFrame) _frames[_frameCount];

                            //movieClipFrame.SetTimeline(&this._timelineOffset[timelineOffsetIndex]);

                            //if (_frameCount >= 2)
                            //{
                            //    maxChildMemoryNeed = System.Math.Max(maxChildMemoryNeed, previousFrame.GetAmountOfChildMemoryNeeded((MovieClipFrame) this._frames[0], GetFrameDataLength(0) / 3, previousFrameId));
                            //}

                            //_childMemoryNeeded = (ushort)maxChildMemoryNeed;

                            return _clipId;

                        case 5:
                            throw new Exception("TAG_MOVIE_CLIP_FRAME no longer supported");

                        case 31:
                            if (_scalingGrid != null)
                                throw new Exception("multiple scaling grids");

                            float a = br.ReadInt32() / 20f;
                            float b = br.ReadInt32() / 20f;
                            float c = br.ReadInt32() / 20f;
                            float d = br.ReadInt32() / 20f;

                            _scalingGrid = new Rect(a, b, c, d);
                            break;

                        case 41:
                            _transformStorageId = br.ReadByte();
                            break;

                        case 11:
                            {
                                MovieClipFrame frame = (MovieClipFrame) _frames[index];
                                
                                ushort frameId = br.ReadUInt16();
                                byte frameNameLength = br.ReadByte();
                                string frameName = null;
                                if (frameNameLength < 255)
                                {
                                    byte[] FNArray = new byte[frameNameLength];
                                    br.Read(FNArray, 0, frameNameLength);
                                    frameName = Encoding.ASCII.GetString(FNArray);
                                }

                                frame.SetId(frameId);
                                frame.SetName(frameName);

                                timelineOffsetIndex += frameId * 3;
                                index++;
                                break;
                            }

                        default:
                            throw new Exception("Unknown tag in MovieClip");
                    }

                }
            }
        }

        public unsafe int GetFrameDataLength(int index)
        {
            MovieClipFrame movieClipFrameFirst = (MovieClipFrame) _frames[index];
            MovieClipFrame movieClipFrameSecond = (MovieClipFrame) _frames[index + 1];

            return (int)((ulong)movieClipFrameSecond.GetTimeline() - (ulong)movieClipFrameFirst.GetTimeline()) >> 1;
        }

        public override void Write(FileStream input)
        {
            if (customAdded == true) // CUSTOM ITEMS
            {
                input.Seek(_scFile.GetEofOffset(), SeekOrigin.Begin);

                // DataType and Length
                input.Write(BitConverter.GetBytes(12), 0, 1);
                input.Write(BitConverter.GetBytes(0), 0, 4);

                int dataLength = writeData(input);

                // Write DataLength
                input.Seek(-(dataLength + 4), SeekOrigin.Current);
                input.Write(BitConverter.GetBytes(dataLength + 5), 0, 4);
                input.Seek(dataLength, SeekOrigin.Current);
            }
            else // DUPLICATE ITEMS
            {
                if (_offset < 0)
                {
                    throw new NotImplementedException();
                }
                else if (_offset > 0) 
                {
                    input.Seek(0, SeekOrigin.Begin);

                    using (MemoryStream finalData = new MemoryStream())
                    {
                        // Data Before Copy
                        byte[] beforeData = new byte[_offset + 1];
                        input.Read(beforeData, 0, beforeData.Length);
                        finalData.Write(beforeData, 0, beforeData.Length);

                        // OLD LENGTH
                        byte[] beforeByte = new byte[4];
                        input.Read(beforeByte, 0, 4);
                        uint beforeLength = BitConverter.ToUInt32(beforeByte);

                        // TEMP Length
                        finalData.Write(BitConverter.GetBytes(beforeLength), 0, 4);

                        // All Data
                        int dataLength = writeData(finalData);

                        // Write DataLength
                        finalData.Seek(_offset + 1, SeekOrigin.Begin);
                        finalData.Write(BitConverter.GetBytes(dataLength + 5), 0, 4);
                        finalData.Seek(dataLength, SeekOrigin.Current);

                        // End of Data
                        finalData.Write(new byte[] { 0, 0, 0, 0, 0 }, 0, 5);

                        // Copy Rest of Data
                        long newLength = _offset + 5 + beforeLength;
                        input.Seek(newLength, SeekOrigin.Begin);
                        byte[] afterData = new byte[input.Length - newLength];
                        input.Read(afterData, 0, afterData.Length);
                        finalData.Write(afterData, 0, afterData.Length);

                        if ((input.Length - beforeLength) != (finalData.Length - (dataLength + 5)))
                            throw new Exception("Data abnormal");

                        // Copy To Input
                        input.Flush();
                        input.SetLength(input.Length - (beforeLength - (dataLength + 5)));
                        input.Seek(0, SeekOrigin.Begin);
                        finalData.Seek(0, SeekOrigin.Begin);
                        finalData.CopyTo(input);

                        this.setLength((uint)(dataLength + 5));
                    }

                    
                    _scFile.SetEofOffset(input.Length - 5);
                }
            }

            if (_offset < 0 || customAdded == true)
            {
                input.Write(new byte[] { 0, 0, 0, 0, 0 }, 0, 5);
                _offset = _scFile.GetEofOffset();
            }
        }

        private int writeData(Stream input)
        {
            int dataLength = 0;

            // MovieClip Data { id, fps, framecount }
            input.Write(BitConverter.GetBytes(_clipId), 0, 2);
            input.Write(BitConverter.GetBytes(_framePerSeconds), 0, 1);
            input.Write(BitConverter.GetBytes(this.Frames.Count), 0, 2);
            dataLength += 5;

            // timelineOffset
            input.Write(BitConverter.GetBytes(timelineOffsetCount / 3), 0, 4);
            dataLength += 4;

            byte[] target = new byte[_timelineOffsetArray.Length * 2];
            Buffer.BlockCopy(_timelineOffsetArray, 0, target, 0, _timelineOffsetArray.Length * 2);
            input.Write(target, 0, target.Length);
            dataLength += target.Length;

            // Childrens Count
            input.Write(BitConverter.GetBytes((ushort)timelineChildrenId.Length), 0, 2);
            dataLength += 2;

            // Childrens IDS
            for (int i = 0; i < timelineChildrenId.Length; i++)
            {
                input.Write(BitConverter.GetBytes(timelineChildrenId[i]), 0, 2);
                dataLength += 2;
            }

            // Childrens Flags - CHECK
            for (int i = 0; i < timelineChildrenId.Length; i++)
            {
                if (_flags != null)
                {
                    input.WriteByte(_flags[i]);
                }
                else
                {
                    input.WriteByte(0);
                }

                dataLength += 1;
            }

            // Childrens Names
            for (int i = 0; i < timelineChildrenId.Length; i++)
            {
                if (string.IsNullOrEmpty(timelineChildrenNames[i]))
                {
                    input.WriteByte((byte)0xFF);
                }
                else
                {
                    byte[] stringData = Encoding.ASCII.GetBytes(timelineChildrenNames[i]);
                    input.WriteByte((byte)stringData.Length);
                    input.Write(stringData, 0, stringData.Length);

                    dataLength += stringData.Length;
                }
                dataLength += 1;
            }

            // Frames
            if (_frames != null)
            {
                if (_frames.Count > 0)
                {
                    for (int i = 0; i < _frames.Count; i++)
                    {
                        input.Write(BitConverter.GetBytes(11), 0, 1);
                        input.Write(BitConverter.GetBytes(0), 0, 4);
                        dataLength += 5;

                        int frameLength = 0;

                        MovieClipFrame movieClipFrameData = (MovieClipFrame)_frames[i];

                        input.Write(BitConverter.GetBytes(movieClipFrameData.Id), 0, 2);
                        frameLength += 2;

                        if (string.IsNullOrEmpty(movieClipFrameData.Name))
                        {
                            input.WriteByte(0xFF);
                        }
                        else
                        {
                            byte[] stringData = Encoding.ASCII.GetBytes(movieClipFrameData.Name);
                            input.WriteByte((byte)stringData.Length);
                            input.Write(stringData, 0, stringData.Length);

                            frameLength += stringData.Length;
                        }
                        frameLength += 1;

                        dataLength += frameLength;

                        // Write FrameLength
                        input.Seek(-(frameLength + 4), SeekOrigin.Current);
                        input.Write(BitConverter.GetBytes(frameLength), 0, 4);
                        input.Seek(frameLength, SeekOrigin.Current);
                    }
                }
            }

            return dataLength;
        }

        public override Bitmap Render(RenderingOptions options)
        {
            return null;
        }

        public Bitmap renderAnimation(RenderingOptions options, int frameIndex)
        {
            if (timelineArray.Length > 0)
            {
                /**List<ushort> addId = new List<ushort>();
                List<int> IdValue = new List<int>();
                for (int i = 0; i < this.timelineArray.Length; i++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        if (x == 0)
                        {
                            int idx = addId.FindIndex(g => g == this.timelineArray[i]);
                            if (idx == -1)
                            {
                                addId.Add(this.timelineArray[i]);
                                IdValue.Add(1);
                            }
                            else
                            {

                                IdValue[idx] += 1;
                            }
                        }

                        x++; i++;
                    }
                }**/

                int totalFrameTimelineCount = 0;
                foreach(MovieClipFrame frame in this._frames)
                {
                    totalFrameTimelineCount += (frame.Id * 3);
                }

                if (/**addId.Count != this.Children.Count || **/this.timelineArray.Length % 3 != 0 || this.timelineArray.Length != totalFrameTimelineCount)
                {
                    MessageBox.Show("MoveClip:Render() ShapeCount does not match timeline count or timeline length is not sets of 6 or total shape count does not match frames count");
                    return null;
                }

                // ^ move this check when the animation starts

                List<PointF> A = _pointFList;

                using (var xyPath = new GraphicsPath())
                {
                    xyPath.AddPolygon(A.ToArray());
                    var xyBound = Rectangle.Round(xyPath.GetBounds());

                    setFrame(frameIndex, options, xyBound);

                    // REMOVE OLD DATA
                    if (frameIndex != 0)
                    {
                        ((MovieClipFrame)this.Frames[frameIndex - 1]).setBitmap(null);
                    }

                    return getFrame(frameIndex);
                }
            }
            return null;
        }

        public Bitmap getFrame(int frameIndex)
        {
            return ((MovieClipFrame)this.Frames[frameIndex]).Bitmap != null ? ((MovieClipFrame)this.Frames[frameIndex]).Bitmap : throw new Exception("getFrame Data is null?");
        }

        public void setFrame(int frameIndex, RenderingOptions options, Rectangle xyBound)
        {
            if (this._scFile.CurrentRenderingMovieClips.FindIndex(mv => mv.Id == this.Id) == -1)
                this._scFile.addRenderingItem(this);

            int frameIndextoAdd = 0;
            int idxToAdd = 0;
            foreach (MovieClipFrame mvframe in Frames)
            {
                if (idxToAdd == frameIndex)
                {
                    break;
                }

                frameIndextoAdd += mvframe.Id;
                idxToAdd++;
            }

            int timelineIndex = frameIndextoAdd * 3;

            var x = xyBound.X;
            var y = xyBound.Y;

            var width = xyBound.Width;
            width = width > 0 ? width : 1;

            var height = xyBound.Height;
            height = height > 0 ? height : 1;

            var finalShape = new Bitmap(width, height);

            int frameTimelineCount = _frames[frameIndex].Id;

            for (int i = 0; i < frameTimelineCount; i++)
            {
                Matrix childrenMatrixData = timelineArray[timelineIndex + 1] != 0xFFFF ? this._scFile.GetMatrixs(_transformStorageId)[timelineArray[timelineIndex + 1]] : null;
                Matrix matrixData = childrenMatrixData != null ? childrenMatrixData.Clone() : new Matrix(1, 0, 0, 1, 0, 0);
                Tuple<Color, byte, Color> colorData = timelineArray[timelineIndex + 2] != 0xFFFF ? this._scFile.getColors(_transformStorageId)[timelineArray[timelineIndex + 2]] : null;

                if (options.MatrixData != null)
                    matrixData.Multiply(options.MatrixData);

                ushort childrenId = timelineChildrenId[timelineArray[timelineIndex]];

                int shapeIndex = _scFile.GetShapes().FindIndex(s => s.Id == childrenId);
                if (shapeIndex != -1)
                {   
                    foreach (ShapeChunk chunk in ((Shape)_scFile.GetShapes()[shapeIndex]).Children)
                    {
                        var texture = (Texture)_scFile.GetTextures()[chunk.GetTextureId()];

                        if (texture != null)
                        {
                            Bitmap bitmap = texture.Bitmap;
                            using (var gpuv = new GraphicsPath())
                            {
                                gpuv.AddPolygon(chunk.UV.ToArray());

                                var gxyBound = Rectangle.Round(gpuv.GetBounds());

                                int gpuvWidth = gxyBound.Width;
                                gpuvWidth = gpuvWidth > 0 ? gpuvWidth : 1;

                                int gpuvHeight = gxyBound.Height;
                                gpuvHeight = gpuvHeight > 0 ? gpuvHeight : 1;

                                var shapeChunk = new Bitmap(gpuvWidth, gpuvHeight);

                                var chunkX = gxyBound.X;
                                var chunkY = gxyBound.Y;

                                using (var g = Graphics.FromImage(shapeChunk))
                                {
                                    gpuv.Transform(new Matrix(1, 0, 0, 1, -chunkX, -chunkY));
                                    g.SetClip(gpuv);
                                    g.DrawImage(bitmap, -chunkX, -chunkY);
                                }

                                GraphicsPath gp = new GraphicsPath();
                                gp.AddPolygon(new[] { new Point(0, 0), new Point(gpuvWidth, 0), new Point(0, gpuvHeight) });

                                double[,] matrixArrayUV =
                                {
                                            {
                                                gpuv.PathPoints[0].X, gpuv.PathPoints[1].X, gpuv.PathPoints[2].X
                                            },
                                            {
                                                gpuv.PathPoints[0].Y, gpuv.PathPoints[1].Y, gpuv.PathPoints[2].Y
                                            },
                                            {
                                                1, 1, 1
                                            }
                                        };

                                PointF[] newXY = new PointF[chunk.XY.Length];

                                for (int xyIdx = 0; xyIdx < newXY.Length; xyIdx++)
                                {
                                    float xNew = matrixData.Elements[4] + matrixData.Elements[0] * chunk.XY[xyIdx].X + matrixData.Elements[2] * chunk.XY[xyIdx].Y;
                                    float yNew = matrixData.Elements[5] + matrixData.Elements[1] * chunk.XY[xyIdx].X + matrixData.Elements[3] * chunk.XY[xyIdx].Y;

                                    newXY[xyIdx] = new PointF(xNew, yNew);
                                }

                                double[,] matrixArrayXY =
                                {
                                            {
                                                newXY[0].X, newXY[1].X, newXY[2].X
                                            },
                                            {
                                                newXY[0].Y, newXY[1].Y, newXY[2].Y
                                            },
                                            {
                                                1, 1, 1
                                            }
                                        };

                                var matrixUV = Matrix<double>.Build.DenseOfArray(matrixArrayUV);
                                var matrixXY = Matrix<double>.Build.DenseOfArray(matrixArrayXY);
                                var inverseMatrixUV = matrixUV.Inverse();
                                var transformMatrix = matrixXY * inverseMatrixUV;
                                var m = new Matrix((float)transformMatrix[0, 0], (float)transformMatrix[1, 0], (float)transformMatrix[0, 1], (float)transformMatrix[1, 1], (float)transformMatrix[0, 2], (float)transformMatrix[1, 2]);

                                //Perform transformations
                                gp.Transform(m);

                                using (Graphics g = Graphics.FromImage(finalShape))
                                {
                                    //Set origin
                                    Matrix originTransform = new Matrix();
                                    originTransform.Translate(-x, -y);
                                    g.Transform = originTransform;

                                    ImageAttributes attr = new ImageAttributes();

                                    if (colorData != null)
                                    {
                                        //ColorMap[] colorMap = new ColorMap[1];
                                        //colorMap[0] = new ColorMap();
                                        //colorMap[0].OldColor = colorData.Item1;

                                        //if (colorData.Item2 != 0xFF)
                                        //{
                                        //    Color newCol = Color.FromArgb(colorData.Item2, colorData.Item3.R, colorData.Item3.G, colorData.Item3.B);
                                        //    colorMap[0].NewColor = newCol;
                                        //}
                                        //else
                                        //{
                                        //    colorMap[0].NewColor = colorData.Item3;
                                        //}

                                        //attr.SetRemapTable(colorMap);
                                    }       

                                    g.DrawImage(shapeChunk, gp.PathPoints, gpuv.GetBounds(), GraphicsUnit.Pixel, attr);

                                    if (options.ViewPolygons)
                                    {
                                        gpuv.Transform(m);
                                        g.DrawPath(new Pen(Color.DeepSkyBlue, 1), gpuv);
                                    }
                                    g.Flush();
                                }
                            }
                        }
                    }
                }
                else 
                {
                    int movieClipIndex = _scFile.GetMovieClips().FindIndex(s => s.Id == childrenId);

                    if (movieClipIndex != -1)
                    {
                        MovieClip extramovieClip = (MovieClip)_scFile.GetMovieClips()[movieClipIndex];

                        Matrix newChildrenMatrixData = timelineArray[timelineIndex + 1] != 0xFFFF ? this._scFile.GetMatrixs(_transformStorageId)[timelineArray[timelineIndex + 1]] : null;
                        Matrix newMatrix = newChildrenMatrixData != null ? newChildrenMatrixData.Clone() : new Matrix(1, 0, 0, 1, 0, 0);


                        if (options.MatrixData != null)
                        {
                            if (newMatrix != null)
                            {
                                newMatrix.Multiply(options.MatrixData);
                            }
                            else
                            {
                                newMatrix = new Matrix();
                                newMatrix.Multiply(options.MatrixData);
                            }
                        }
                        
                        extramovieClip.generatePointFList(newMatrix);

                        if (extramovieClip._lastPlayedFrame + 1 >= extramovieClip.Frames.Count || frameIndex == 0)
                            extramovieClip._lastPlayedFrame = 0;

                        int extraFrameIndex = extramovieClip._lastPlayedFrame;

                        RenderingOptions newOptions = new RenderingOptions() { MatrixData = newMatrix, ViewPolygons = options.ViewPolygons };

                        extramovieClip.setFrame(extraFrameIndex, newOptions, xyBound);
                        Bitmap frameData = extramovieClip.getFrame(extraFrameIndex);
                        extramovieClip._lastPlayedFrame += 1;

                        using (Graphics g = Graphics.FromImage(finalShape))
                        {
                            g.DrawImage(frameData, 0, 0);

                            g.Flush();
                        }

                        extramovieClip.destroyPointFList();
                    }
                    else
                    {
                        // implement
                    }
                    
                }


                timelineIndex += 3;
            }
            

            ((MovieClipFrame)this.Frames[frameIndex]).setBitmap(finalShape);
        }

        public List<PointF> getChildrensPointF(Matrix matrixIn)
        {
            List<PointF> A = new List<PointF>();

            for (int i = 0; i < (timelineArray.Length / 3); i++)
            {
                int shapeIndex = _scFile.GetShapes().FindIndex(s => s.Id == timelineChildrenId[timelineArray[(i * 3)]]);
                if (shapeIndex != -1)
                {
                    Shape shapeToRender = (Shape)_scFile.GetShapes()[shapeIndex];

                    Matrix childrenMatrixData = timelineArray[(i * 3) + 1] != 0xFFFF ? this._scFile.GetMatrixs(_transformStorageId)[timelineArray[(i * 3) + 1]] : null;
                    Matrix matrixData = childrenMatrixData != null ? childrenMatrixData.Clone() : new Matrix(1, 0, 0, 1, 0, 0);

                    if (matrixIn != null)
                    {
                        if (matrixData != null)
                        {
                            matrixData.Multiply(matrixIn);
                        }
                        else
                        {
                            matrixData = new Matrix(1, 0, 0, 1, 0, 0);
                            matrixData.Multiply(matrixIn);
                        }
                    }

                    if (matrixData != null)
                    {
                        foreach (ShapeChunk chunk in shapeToRender.GetChunks())
                        {
                            PointF[] newXY = new PointF[chunk.XY.Length];

                            for (int xyIdx = 0; xyIdx < newXY.Length; xyIdx++)
                            {
                                float xNew = matrixData.Elements[4] + matrixData.Elements[0] * chunk.XY[xyIdx].X + matrixData.Elements[2] * chunk.XY[xyIdx].Y;
                                float yNew = matrixData.Elements[5] + matrixData.Elements[1] * chunk.XY[xyIdx].X + matrixData.Elements[3] * chunk.XY[xyIdx].Y;

                                newXY[xyIdx] = new PointF(xNew, yNew);
                            }

                            A.AddRange(newXY);
                        }
                    }
                    else
                    {
                        PointF[] pointsXY = shapeToRender.Children.SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                        A.AddRange(pointsXY.ToArray());
                    }
                }
                else
                {
                    int movieClipIndex = _scFile.GetMovieClips().FindIndex(s => s.Id == timelineChildrenId[timelineArray[(i * 3)]]);

                    if (movieClipIndex != -1)
                    {

                        A.AddRange(((MovieClip)_scFile.GetMovieClips()[movieClipIndex]).getChildrensPointF(matrixIn));
                    }
                    else if (_scFile.getTextFields().FindIndex(t => t.Id == timelineChildrenId[timelineArray[(i * 3)]]) != -1)
                    {
                        // implement
                    }
                    else
                    {
                        this.GetName();
                        throw new Exception($"Unknown type of children with id {timelineChildrenId[timelineArray[(i * 3)]]} for movieclip id {this.Id}");
                    }
                }
            }

            return A;
        }

        public void SetId(ushort id)
        {
            _clipId = id;
        }

        public void SetFrameCount(short count)
        {
            _frameCount = count;
        }
        public void SetFramePerSecond(byte fps)
        {
            _framePerSeconds = fps;
        }
        public void addChildren(ScObject data)
        {
            _childrens.Add(data);
        }

        public void addChildrenId(ushort id)
        {
            List<ushort> temp = _timelineChildrenId.ToList();
            temp.Add(id);

            _timelineChildrenId = temp.ToArray();
        }

        public void addChildrenName(string name)
        {
            List<string> temp = _timelineChildrenNames.ToList();
            temp.Add(name);

            _timelineChildrenNames = temp.ToArray();
        }

        public void generatePointFList(Matrix matrixIn)
        {
            _pointFList = getChildrensPointF(matrixIn);
        }

        public void destroyPointFList()
        {
            _pointFList = null;
        }
        public void setFlags(byte[] data)
        {
            _flags = data;
        }

        public void setScalingGrid(Rect data)
        {
            _scalingGrid = data;
        }

        public Rect scalingGrid => _scalingGrid;
        public byte[] flags => _flags;

        public ushort timelineChildrenCount => _timelineChildrenCount;
        public ushort[] timelineChildrenId => _timelineChildrenId;
        public string[] timelineChildrenNames => _timelineChildrenNames;
        public int timelineOffsetCount => _timelineOffsetArray.Length;

        public void setTimelineChildrenCount(ushort count)
        {
            _timelineChildrenCount = count;
        }
        public void setTimelineChildrenId(ushort[] id)
        {
            _timelineChildrenId = id;
        }
        public void setTimelineChildrenNames(string[] names)
        {
            _timelineChildrenNames = names;
        }

        public List<ScObject> GetFrames()
        {
            return _frames;
        }
        public void AddFrame(ScObject frame)
        {
            _frames.Add(frame);
        }
        public void SetDataType(short dataType)
        {
            _dataType = dataType;
        }
        public void setChildrens(List<ScObject> data)
        {
            _childrens = data;
        }
        public void SetFrames(List<ScObject> frames)
        {
            _frames = frames;
        }

        public void setExportType(exportType type)
        {
            _exportType = type;
        }

        public void setIconType(iconType type)
        {
            _iconType = type;
        }
        public void setAnimationType(animationType type)
        {
            _animationType = type;
        }

        public void setTimelineOffsetArray(ushort[] array)
        {
            _timelineOffsetArray = array;
        }

        public void setHasShadow(bool value)
        {
            _hasShadow = value;
        }

        #endregion
    }
}