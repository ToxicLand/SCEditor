using MathNet.Numerics.LinearAlgebra;
using SCEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public MovieClip(ScFile scs, short dataType)
        {
            _scFile = scs;
            _dataType = dataType;
            _childrens = new List<ScObject>();
            _frames = new List<ScObject>();
        }

        public MovieClip(MovieClip mv)
        {
            _scFile = mv.GetStorageObject();

            _offset = -1;
            _clipId = _scFile.getMaxId();
            customAdded = true;

            _dataType = mv.GetMovieClipDataType();
            _childrens = ((ScObject[])mv._childrens.ToArray().Clone()).ToList();

            _framePerSeconds = mv._framePerSeconds;
            _frames = mv._frames;

            _timelineOffsetArray = (ushort[])mv.timelineArray.Clone();
            _timelineChildrenId = (ushort[])mv._timelineChildrenId.Clone();
            _timelineChildrenNames = (string[])mv.timelineChildrenNames.Clone();
            _flags = mv.flags;

            _transformStorageId = mv._transformStorageId;
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
        public byte _transformStorageId { get; set; } = 0;
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
        public override SCObjectType objectType => SCObjectType.MovieClip;
        private string _packetId;

        public int _lastPlayedFrame { get; set; }
        public MovieClipState _animationState { get; set; }
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

        public List<ScObject> getChildrensWithoutDuplicates()
        {
            List<ScObject> list = _childrens.Distinct().ToList();

            return list;
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
                                MovieClipFrame frame = (MovieClipFrame)_frames[index];

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
            MovieClipFrame movieClipFrameFirst = (MovieClipFrame)_frames[index];
            MovieClipFrame movieClipFrameSecond = (MovieClipFrame)_frames[index + 1];

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

            if (_transformStorageId > 0)
            {
                input.Write(BitConverter.GetBytes(41), 0, 1);
                input.Write(BitConverter.GetBytes(0), 0, 4);
                input.Write(new byte[1] { _transformStorageId }, 0, 1);
                dataLength += 6;
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
            this._animationState = MovieClipState.Playing;

            if (timelineArray.Length > 0)
            {
                if (_pointFList?.Count == 0 || _pointFList == null)
                {
                    Console.WriteLine("renderAnimation(): List<PointF> A empty or null.");
                    return null;
                }

                if (frameIndex == 0)
                {
                    if (this.Frames.Count != 1)
                    {
                        ((MovieClipFrame)this.Frames[this.Frames.Count - 1]).setBitmap(null);
                    }
                }
                else
                {
                    ((MovieClipFrame)this.Frames[frameIndex - 1]).setBitmap(null);
                }

                using (var xyPath = new GraphicsPath())
                {
                    xyPath.AddPolygon(_pointFList.ToArray());
                    var xyBound = Rectangle.Round(xyPath.GetBounds());

                    setFrame(frameIndex, options, xyBound);
                    return getFrame(frameIndex);
                }
            }

            return null;
        }

        public Bitmap getFrame(int frameIndex)
        {
            return ((MovieClipFrame)this.Frames[frameIndex]).Bitmap != null ? ((MovieClipFrame)this.Frames[frameIndex]).Bitmap : new Bitmap(1, 1);
        }

        public void setFrame(int frameIndex, RenderingOptions options, Rectangle xyBound)
        {
            try
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

                var finalShape = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                int frameTimelineCount = _frames[frameIndex].Id;

                for (int i = 0; i < frameTimelineCount; i++)
                {
                    if (timelineChildrenNames[timelineArray[timelineIndex]] == "bounds")
                    {
                        timelineIndex += 3;
                        continue;
                    }

                    Bitmap temporaryBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    ushort childrenId = timelineChildrenId[timelineArray[timelineIndex]];

                    Tuple<Color, byte, Color> colorData = timelineArray[timelineIndex + 2] != 0xFFFF ? this._scFile.getColors(_transformStorageId)[timelineArray[timelineIndex + 2]] : null;

                    // SHAPE
                    int shapeIndex = _scFile.GetShapes().FindIndex(s => s.Id == childrenId);
                    if (shapeIndex != -1)
                    {
                        Matrix childrenMatrixData = timelineArray[timelineIndex + 1] != 0xFFFF ? this._scFile.GetMatrixs(_transformStorageId)[timelineArray[timelineIndex + 1]] : null;
                        //Matrix matrixData = childrenMatrixData != null ? childrenMatrixData.Clone() : null;
                        Matrix matrixData = childrenMatrixData != null ? childrenMatrixData.Clone() : new Matrix();

                        if (options.MatrixData != null)
                            matrixData.Multiply(options.MatrixData);

                        if (options.editedMatrixPerChildren.ContainsKey(childrenId))
                            matrixData.Multiply(options.editedMatrixPerChildren[childrenId]);

                        /**
                        temporaryBitmap = _scFile.GetShapes()[shapeIndex].Render(new RenderingOptions() {
                            MatrixData = matrixData,
                            ViewPolygons = options.ViewPolygons,
                            LogConsole = false
                        });

                        if (temporaryBitmap == null)
                        {
                            Console.WriteLine($"Unable to render {childrenId} of frame {frameIndex}");
                            timelineIndex += 3;
                            continue;
                        }
                        **/


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
                                    int gpuvHeight = gxyBound.Height;

                                    if (gpuvWidth > 0 && gpuvHeight > 0)
                                    {

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


                                        using (Graphics g = Graphics.FromImage(temporaryBitmap))
                                        {
                                            //Set origin
                                            Matrix originTransform = new Matrix();
                                            originTransform.Translate(-x, -y);
                                            g.Transform = originTransform;

                                            g.DrawImage(shapeChunk, gp.PathPoints, gpuv.GetBounds(), GraphicsUnit.Pixel);

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

                    }
                    else
                    {
                        // Movieclip
                        int movieClipIndex = _scFile.GetMovieClips().FindIndex(s => s.Id == childrenId);
                        if (movieClipIndex != -1)
                        {
                            MovieClip extramovieClip = (MovieClip)_scFile.GetMovieClips()[movieClipIndex];

                            Matrix newChildrenMatrixData = timelineArray[timelineIndex + 1] != 0xFFFF ? this._scFile.GetMatrixs(_transformStorageId)[timelineArray[timelineIndex + 1]] : null;
                            Matrix newMatrix = newChildrenMatrixData != null ? newChildrenMatrixData.Clone() : new Matrix();

                            if (options.MatrixData != null)
                            {
                                newMatrix.Multiply(options.MatrixData);
                            }

                            if (options.editedMatrixPerChildren.ContainsKey(extramovieClip.Id))
                            {
                                newMatrix.Multiply(options.editedMatrixPerChildren[extramovieClip.Id]);
                            }

                            if ((extramovieClip.getPointFList() == null || extramovieClip.getPointFList().Count == 0) && extramovieClip._animationState == MovieClipState.Stopped)
                                extramovieClip.initPointFList(newMatrix);

                            if (_scFile.CurrentRenderingMovieClips.FindIndex(s => s.Id == extramovieClip.Id) == -1)
                                _scFile.CurrentRenderingMovieClips.Add(extramovieClip);

                            int extraFrameIndex = extramovieClip._lastPlayedFrame;

                            RenderingOptions newOptions = new RenderingOptions() { editedMatrixPerChildren = options.editedMatrixPerChildren, ViewPolygons = options.ViewPolygons };
                            if (newChildrenMatrixData != new Matrix())
                                newOptions.MatrixData = newMatrix; // confirm what to use here

                            extramovieClip.setFrame(extraFrameIndex, newOptions, xyBound);

                            Bitmap frameData = extramovieClip.getFrame(extraFrameIndex);

                            extramovieClip._lastPlayedFrame += 1;

                            if (extramovieClip._lastPlayedFrame == extramovieClip.Frames.Count)
                                extramovieClip._lastPlayedFrame = 0;

                            using (Graphics g = Graphics.FromImage(temporaryBitmap))
                            {
                                g.DrawImage(frameData, 0, 0);
                                g.Flush();
                            }
                        }
                        else
                        {
                            //Textfield
                            int textFieldIndex = _scFile.getTextFields().FindIndex(s => s.Id == childrenId);
                            if (textFieldIndex != -1)
                            {
                                if (!RenderingOptions.disableTextFieldRendering)
                                {
                                    TextField textFieldData = (TextField)_scFile.getTextFields()[textFieldIndex];

                                    using (Graphics g = Graphics.FromImage(temporaryBitmap))
                                    {
                                        StringFormat sf = new StringFormat();
                                        sf.Alignment = StringAlignment.Center;
                                        sf.LineAlignment = StringAlignment.Far;

                                        InstalledFontCollection fonts = new InstalledFontCollection();
                                        FontFamily textFontFamily = fonts.Families.Where(f => f.Name == textFieldData._fontName).FirstOrDefault();
                                        if (textFontFamily == null)
                                        {
                                            MessageBox.Show($"Movieclip childiren {childrenId} textfield font {textFieldData._fontName} not installed");
                                            textFontFamily = SystemFonts.DefaultFont.FontFamily;
                                        }

                                        var p = new Pen(textFieldData._fontOutlineColor, 0);
                                        p.LineJoin = LineJoin.Round;
                                        if (textFieldData._fontOutlineColor != (new Color()))
                                        {
                                            p.Width = 5;
                                        }

                                        string textRender = string.Empty;
                                        if (string.IsNullOrEmpty(textFieldData._textData))
                                        {
                                            if (string.IsNullOrEmpty(timelineChildrenNames[timelineArray[timelineIndex]]))
                                            {
                                                textRender = "Text1";
                                            }
                                            else
                                            {
                                                textRender = timelineChildrenNames[timelineArray[timelineIndex]];
                                            }
                                        }
                                        else
                                        {
                                            textRender = textFieldData._textData;
                                        }

                                        GraphicsPath gp = new GraphicsPath();
                                        Rectangle r = new Rectangle(0, 0, finalShape.Width, finalShape.Height);
                                        gp.AddString(textRender, textFontFamily, (int)FontStyle.Regular, textFieldData._fontSize, r, sf);

                                        g.SmoothingMode = SmoothingMode.AntiAlias;
                                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                        g.DrawPath(p, gp);
                                        g.DrawString(textRender, (new Font(textFontFamily, textFieldData._fontSize, FontStyle.Regular, GraphicsUnit.Pixel)), (new SolidBrush(textFieldData._fontColor)), r, sf);

                                        gp.Dispose();
                                        g.Flush();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Movieclip {this.Id} contains children {childrenId} of unknown type.");
                            }
                        }
                    }

                    Rectangle originalRectangle = new Rectangle(0, 0, temporaryBitmap.Width, temporaryBitmap.Height);

                    if (colorData != null)
                    {
                        Color originalColor = colorData.Item3;
                        Color replacementColor = colorData.Item1;

                        byte alphaByte = colorData.Item2;

                        ChangeColour(temporaryBitmap, originalColor.A, originalColor.R, originalColor.G, originalColor.B, alphaByte, replacementColor.R, replacementColor.G, replacementColor.B);

                        //var orgARGB = new byte[] { originalColor.A, originalColor.R, originalColor.G, originalColor.B };
                        //var repARGB = new byte[] { replacementColor.A, replacementColor.R, replacementColor.G, replacementColor.B };

                        //var orgBytes = orgARGB;
                        //var repBytes = repARGB;

                        //ReplaceColorUnsafe(temporaryBitmap, orgBytes, repBytes, alphaByte);

                        /**
                        ImageAttributes imageAttributes = new ImageAttributes();
                        ColorMatrix matrix = new ColorMatrix();

                        ColorMap colorMap = new ColorMap();
                        ColorMap[] remapTable = { colorMap };

                        colorMap.OldColor = colorData.Item3;
                        colorMap.NewColor = colorData.Item1;

                        matrix.Matrix33 = (float)(colorData.Item2 / 255F);

                        imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                        Bitmap tempBmp1 = new Bitmap(temporaryBitmap.Width, temporaryBitmap.Height);
                        using (Graphics g = Graphics.FromImage(tempBmp1))
                        {
                            g.DrawImage(temporaryBitmap, originalRectangle, 0, 0, temporaryBitmap.Width, temporaryBitmap.Height, GraphicsUnit.Pixel, imageAttributes);
                        }

                        imageAttributes = new ImageAttributes();
                        imageAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        temporaryBitmap = new Bitmap(temporaryBitmap.Width, temporaryBitmap.Height);
                        using (Graphics g = Graphics.FromImage(temporaryBitmap))
                        {
                            g.DrawImage(tempBmp1, originalRectangle, 0, 0, temporaryBitmap.Width, temporaryBitmap.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        **/
                    }

                    using (Graphics g = Graphics.FromImage(finalShape))
                    {
                        g.DrawImage(temporaryBitmap, originalRectangle);
                    }

                    timelineIndex += 3;
                }

                ((MovieClipFrame)this.Frames[frameIndex]).setBitmap(finalShape);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(OverflowException))
                {
                    MessageBox.Show(ex.Message + $" in setFrame({frameIndex}) | " + ex.StackTrace);
                }
                else
                {
                    Console.WriteLine(ex.Message + $" in setFrame({frameIndex}) | " + ex.StackTrace);
                    Task.Delay(2000);
                }
            }

        }

        private static void ChangeColour(Bitmap bmp, byte inColourA, byte inColourR, byte inColourG, byte inColourB, byte outColourA, byte outColourR, byte outColourG, byte outColourB)
        {
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
            bmp.LockBits(rect, ImageLockMode.ReadWrite,
                         pxf);

            IntPtr ptr = bmpData.Scan0;

            int numBytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[numBytes];

            float alphaPercent = (float)(outColourA / 255F);

            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            for (int counter = 0; counter < rgbValues.Length; counter += 4)
            {
                if ((inColourR == 255 && inColourG == 255 && inColourB == 255) && (outColourR == 0 && outColourG == 0 && outColourB == 0))
                {
                    rgbValues[counter + 3] = (byte)(rgbValues[counter + 3] * alphaPercent);
                    continue;
                }

                if (rgbValues[counter] == inColourR && rgbValues[counter + 1] == inColourG && rgbValues[counter + 2] == inColourB && rgbValues[counter + 3] != 0)
                {
                    rgbValues[counter] = outColourR;
                    rgbValues[counter + 1] = outColourG;
                    rgbValues[counter + 2] = outColourB;
                    rgbValues[counter + 3] = (byte)(rgbValues[counter + 3] * alphaPercent);
                }
            }

            Marshal.Copy(rgbValues, 0, ptr, numBytes);
            bmp.UnlockBits(bmpData);
        }

        public List<PointF> generateChildrensPointF(Matrix matrixIn, CancellationToken token = new CancellationToken())
        {
            List<PointF> A = new List<PointF>();

            for (int i = 0; i < (timelineArray.Length / 3); i++)
            {
                if (token.IsCancellationRequested)
                    return null;

                int shapeIndex = _scFile.GetShapes().FindIndex(s => s.Id == timelineChildrenId[timelineArray[(i * 3)]]);
                if (shapeIndex != -1)
                {
                    Shape shapeToRender = (Shape)_scFile.GetShapes()[shapeIndex];

                    Matrix childrenMatrixData = timelineArray[(i * 3) + 1] != 0xFFFF ? this._scFile.GetMatrixs(_transformStorageId)[timelineArray[(i * 3) + 1]] : null;
                    Matrix matrixData = childrenMatrixData != null ? childrenMatrixData.Clone() : new Matrix();

                    if (matrixIn != null)
                    {
                        matrixData.Multiply(matrixIn);
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
                        A.AddRange(pointsXY);
                    }
                }
                else
                {
                    int movieClipIndex = _scFile.GetMovieClips().FindIndex(s => s.Id == timelineChildrenId[timelineArray[(i * 3)]]);

                    if (movieClipIndex != -1)
                    {
                        List<PointF> templist = ((MovieClip)_scFile.GetMovieClips()[movieClipIndex]).generateChildrensPointF(matrixIn, token);

                        if (templist == null || token.IsCancellationRequested)
                            return null;

                        A.AddRange(templist);
                    }
                    else if (_scFile.getTextFields().FindIndex(t => t.Id == timelineChildrenId[timelineArray[(i * 3)]]) != -1)
                    {
                        // implement
                    }
                    else
                    {
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

        public void initPointFList(Matrix matrixIn, CancellationToken token = new CancellationToken())
        {
            _pointFList = generateChildrensPointF(matrixIn, token);
        }

        public List<PointF> getPointFList()
        {
            return _pointFList;
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

        public enum MovieClipState
        {
            Stopped,
            Playing,
            None
        }
    }
}