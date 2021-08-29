using MathNet.Numerics.LinearAlgebra;
using SCEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        public MovieClip(ScFile scs , short dataType)
        {
            _scFile = scs;
            _dataType = dataType;
            _shapes = new List<ScObject>();
            _frames = new List<ScObject>();
        }

        public MovieClip(MovieClip mv)
        {
            _scFile = mv.GetStorageObject();
            _dataType = mv.GetMovieClipDataType();
            _shapes = mv.GetShapes();
            _frames = mv.GetFrames();

            this.SetOffset(-System.Math.Abs(mv.GetOffset()));

            //Duplicate MovieClip
            using (FileStream input = new FileStream(_scFile.GetInfoFileName(), FileMode.Open))
            {
                input.Seek(System.Math.Abs(mv.GetOffset()), SeekOrigin.Begin);
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
            foreach (Shape s in _shapes)
            {
                Shape newShape = new Shape(s);
                newShape.SetId(maxShapeId);
                maxShapeId++;
                newShapes.Add(newShape);

                _scFile.AddShape(newShape); //Add to global shapelist
                _scFile.AddChange(newShape);
            }
            this._shapes = newShapes;
        }

        #endregion

        #region Fields & Properties

        private short _dataType;
        private ushort _clipId;
        private short _frameCount;
        private byte[] _flags;
        private byte _framePerSeconds;
        private List<ScObject> _shapes;
        private ScFile _scFile;
        private List<ScObject> _frames;
        private unsafe ushort* _timelineOffset;
        private ushort _timelineChildrenCount;
        private ushort[] _timelineChildrenId;
        private Rect _scalingGrid;
        private byte _uk63;
        private string[] _timelineChildrenNames;
        private int _timelineOffsetCount;
        private ushort[] _timelineOffsetArray;
        private exportType _exportType;
        private iconType _iconType;
        private animationType _animationType;
        private uint _length;
        private bool _hasShadow;
        public override ushort Id => _clipId;
        public override List<ScObject> Children => _shapes;
        public List<ScObject> Frames => _frames;
        public ushort[] timelineArray => _timelineOffsetArray;
        public bool hasShadow => _hasShadow;
        public byte FPS => _framePerSeconds;

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

        public long GetOffset()
        {
            return _offset;
        }

        public List<ScObject> GetShapes()
        {
            return _shapes;
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

            br.BaseStream.Seek(-(_timelineOffsetCount * 2), SeekOrigin.Current);

            _timelineOffset = (ushort*)Marshal.AllocHGlobal(_timelineOffsetCount * sizeof(ushort));
            br.Read(new Span<byte>((byte*)_timelineOffset, _timelineOffsetCount * 2));

            for (int i = 0; i < _frameCount; i++)
            {
                _frames.Add(new MovieClipFrame(_scFile));
            }

            ushort count = br.ReadUInt16();

            _timelineChildrenCount = count;
            _timelineChildrenId = new ushort[count];
            br.Read(MemoryMarshal.Cast<ushort, byte>((Span<ushort>)_timelineChildrenId));

            foreach (ushort tcId in _timelineChildrenId)
            {
                int findIndex = _scFile.GetShapes().FindIndex(shape => shape.Id == tcId);
                if (findIndex != -1)
                    _shapes.Add(_scFile.GetShapes()[findIndex]); ;
            }

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
            int previousFrameId = 0;
            int maxChildMemoryNeed = 0;

            MovieClipFrame previousFrame = null;

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
                            _uk63 = br.ReadByte();
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

                                frame.SetTimeline(&_timelineOffset[timelineOffsetIndex]);

                                maxChildMemoryNeed = previousFrame != null
                                ? System.Math.Max(maxChildMemoryNeed, frame.GetAmountOfChildMemoryNeeded(frame, frameId, previousFrameId))
                                : frameId;

                                timelineOffsetIndex += frameId * 3;
                                index++;
                                previousFrame = frame;
                                previousFrameId = frameId;
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
                int dataLength = 0;
                input.Write(BitConverter.GetBytes(12), 0, 1); // MovieClip Tag
                input.Write(BitConverter.GetBytes(0), 0, 4); // DATA LENGTH 

                // MovieClip Data { id, fps, framecount }
                input.Write(BitConverter.GetBytes(_clipId), 0, 2);
                input.Write(BitConverter.GetBytes(_framePerSeconds), 0, 1);
                input.Write(BitConverter.GetBytes(this.Frames.Count), 0, 2);
                dataLength += 5;

                // timelineOffset
                input.Write(BitConverter.GetBytes(_timelineOffsetCount / 3), 0, 4);
                dataLength += 4;

                byte[] target = new byte[_timelineOffsetArray.Length * 2];
                Buffer.BlockCopy(_timelineOffsetArray, 0, target, 0, _timelineOffsetArray.Length * 2);
                input.Write(target, 0, target.Length);
                dataLength += target.Length;

                int shapesCount = _shapes.Count;

                // Shapes Count
                input.Write(BitConverter.GetBytes((ushort)shapesCount), 0, 2);
                dataLength += 2;

                // Shape IDS
                for (int i = 0; i < _shapes.Count; i++)
                {
                    input.Write(BitConverter.GetBytes(_shapes[i].Id), 0, 2);
                    dataLength += 2;
                }

                // CHECK CHANGE
                if (_exportType == exportType.Icon)
                {
                    if (_iconType == iconType.Hero)
                    {
                        input.Write(BitConverter.GetBytes((ushort)5971), 0, 2);
                        dataLength += 2;
                    }
                }

                // Shapes Flags - CHECK
                for (int i = 0; i < _shapes.Count; i++)
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

                // CHECK CHANGE
                if (_exportType == exportType.Icon)
                {
                    if (_iconType == iconType.Hero)
                    {
                        input.WriteByte(0);
                        dataLength += 1;
                    }
                }

                // Shapes Names
                for (int i = 0; i < _shapes.Count; i++)
                {
                    /**
                    if (string.IsNullOrEmpty(_shapes[i].GetName()))
                    {
                        input.WriteByte(0);
                    }
                    else
                    {
                        input.Write(BitConverter.GetBytes(_shapes[i].GetName().Length), 0, 1);
                        input.Write(Encoding.ASCII.GetBytes(_shapes[i].GetName()), 0, _shapes[i].GetName().Length);
                    }
                    **/
                    input.WriteByte(0xFF);
                    dataLength += 1;
                }

                // CHECK CHANGE
                if (_exportType == exportType.Icon)
                {
                    if (_iconType == iconType.Hero)
                    {
                        string shapeNameHeroBounds = "bounds";
                        input.Write(BitConverter.GetBytes(shapeNameHeroBounds.Length), 0, 1);
                        input.Write(Encoding.ASCII.GetBytes(shapeNameHeroBounds), 0, shapeNameHeroBounds.Length);
                        dataLength += (1 + shapeNameHeroBounds.Length);
                    }
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
                                frameLength += 1;
                            }
                            else
                            {
                                input.Write(BitConverter.GetBytes(movieClipFrameData.Name.Length), 0, 1);
                                input.Write(Encoding.ASCII.GetBytes(movieClipFrameData.Name), 0, movieClipFrameData.Name.Length);

                                frameLength += (1 + movieClipFrameData.Name.Length);
                            }

                            dataLength += frameLength;

                            // Write FrameLength
                            input.Seek(-(frameLength + 4), SeekOrigin.Current);
                            input.Write(BitConverter.GetBytes(frameLength), 0, 4);
                            input.Seek(frameLength, SeekOrigin.Current);
                        }
                    }
                }

                // Write DataLength
                input.Seek(-(dataLength + 4), SeekOrigin.Current);
                input.Write(BitConverter.GetBytes(dataLength + 5), 0, 4);
                input.Seek(dataLength, SeekOrigin.Current);
            }
            else // DUPLICATE ITEMS
            {
                if (_offset < 0)
                {
                    using (
                    FileStream readInput = new FileStream(_scFile.GetInfoFileName(), FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite))
                    {
                        //Positionnement des curseurs
                        readInput.Seek(System.Math.Abs(_offset), SeekOrigin.Begin);
                        input.Seek(_scFile.GetEofOffset(), SeekOrigin.Begin);

                        //type and length
                        byte[] dataType = new byte[1];
                        readInput.Read(dataType, 0, 1);
                        byte[] dataLength = new byte[4];
                        readInput.Read(dataLength, 0, 4);
                        input.Write(dataType, 0, 1);
                        input.Write(dataLength, 0, 4);

                        //movieclip
                        readInput.Seek(2, SeekOrigin.Current);
                        input.Write(BitConverter.GetBytes(_clipId), 0, 2);

                        input.WriteByte((byte)readInput.ReadByte());
                        readInput.Seek(2, SeekOrigin.Current);
                        input.Write(BitConverter.GetBytes(_frameCount), 0, 2);

                        //int cnt1 = br.ReadInt32();
                        byte[] cnt1 = new byte[4];
                        readInput.Read(cnt1, 0, 4);
                        input.Write(cnt1, 0, 4);

                        for (int i = 0; i < BitConverter.ToInt32(cnt1, 0); i++)
                        {
                            byte[] uk1 = new byte[2];
                            readInput.Read(uk1, 0, 2);
                            input.Write(uk1, 0, 2);

                            byte[] uk2 = new byte[2];
                            readInput.Read(uk2, 0, 2);
                            input.Write(uk2, 0, 2);

                            byte[] uk3 = new byte[2];
                            readInput.Read(uk3, 0, 2);
                            input.Write(uk3, 0, 2);
                        }

                        //int cnt2 = br.ReadInt16();
                        byte[] cnt2 = new byte[2];
                        readInput.Read(cnt2, 0, 2);
                        input.Write(cnt2, 0, 2);

                        int cptShape = 0;
                        for (int i = 0; i < BitConverter.ToInt16(cnt2, 0); i++)
                        {
                            byte[] id = new byte[2];
                            readInput.Read(id, 0, 2);

                            int index = _scFile.GetShapes().FindIndex(shape => shape.Id == BitConverter.ToInt16(id, 0));
                            if (index != -1)
                            {
                                input.Write(BitConverter.GetBytes(_shapes[cptShape].Id), 0, 2);
                                cptShape++;
                            }
                            else
                            {
                                input.Write(id, 0, 2);
                            }
                        }
                        for (int i = 0; i < BitConverter.ToInt16(cnt2, 0); i++)
                        {
                            input.WriteByte((byte)readInput.ReadByte());
                        }


                        //read ascii
                        for (int i = 0; i < BitConverter.ToInt16(cnt2, 0); i++)
                        {
                            byte stringLength = (byte)readInput.ReadByte();
                            input.WriteByte(stringLength);
                            if (stringLength < 255)
                            {
                                for (int j = 0; j < stringLength; j++)
                                    input.WriteByte((byte)readInput.ReadByte());
                            }
                        }

                        byte[] lenght = new byte[4];
                        while (true)
                        {
                            byte v26;
                            while (true)
                            {
                                while (true)
                                {
                                    v26 = (byte)readInput.ReadByte();
                                    input.WriteByte(v26);

                                    //br.ReadInt32();
                                    readInput.Read(lenght, 0, 4);
                                    input.Write(lenght, 0, 4);

                                    if (v26 != 5)
                                        break;
                                }
                                if (v26 == 11)
                                {
                                    //short frameId = br.ReadInt16();
                                    byte[] frameId = new byte[2];
                                    readInput.Read(frameId, 0, 2);
                                    input.Write(frameId, 0, 2);

                                    byte frameNameLength = (byte)readInput.ReadByte();
                                    input.WriteByte(frameNameLength);

                                    if (frameNameLength < 255)
                                    {
                                        for (int i = 0; i < frameNameLength; i++)
                                        {
                                            input.WriteByte((byte)readInput.ReadByte());
                                        }
                                    }
                                }
                                else if (v26 == 31)
                                {
                                    int l = Convert.ToInt32(lenght);
                                    byte[] data = new byte[l];
                                    readInput.Read(data, 0, l);
                                    input.Write(data, 0, l);
                                }
                                else
                                    break;
                            }
                            if (v26 == 0)
                                break;
                            Console.WriteLine("Unknown tag " + v26);
                        }
                    }
                }
                else if (_offset > 0) 
                {
                    input.Seek(0, SeekOrigin.Begin);

                    _length = (uint)((_length - ((_frameCount * 6) * 2)) + _timelineOffsetArray.Length);

                    using (MemoryStream finalData = new MemoryStream())
                    {
                        byte[] beforeData = new byte[_offset + 8];
                        input.Read(beforeData, 0, beforeData.Length);
                        finalData.Write(beforeData, 0, beforeData.Length);

                        finalData.Seek(-7, SeekOrigin.Current);
                        finalData.Write(BitConverter.GetBytes(_length), 0, 4);
                        finalData.Seek(3, SeekOrigin.Current);

                        finalData.Write(BitConverter.GetBytes((short)_frames.Count),0,2);
                        finalData.Write(BitConverter.GetBytes(_timelineOffsetArray.Length / 3), 0, 4);

                        for (int i = 0; i < _timelineOffsetArray.Length; i++)
                        {
                            finalData.Write(BitConverter.GetBytes(_timelineOffsetArray[i]),0,2);
                        }

                        input.Seek((((_frameCount * 6) * 2) + 6), SeekOrigin.Current);

                        _frameCount = (short)_frames.Count;
                        _timelineOffsetCount = _frameCount * 2;

                        byte[] afterData = new byte[input.Length - input.Position];
                        input.Read(afterData, 0, afterData.Length);
                        finalData.Write(afterData, 0, afterData.Length);

                        input.Seek(0, SeekOrigin.Begin);
                        finalData.Seek(0, SeekOrigin.Begin);
                        finalData.CopyTo(input);
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

        public override Bitmap Render(RenderingOptions options)
        {
            return null;
        }

        public Bitmap renderAnimation(RenderingOptions options, int frameIndex)
        {
            if (timelineArray.Length > 0)
            {
                List<ushort> addId = new List<ushort>();
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
                }

                int totalFrameTimelineCount = 0;
                foreach(MovieClipFrame frame in this._frames)
                {
                    totalFrameTimelineCount += (frame.Id * 3);
                }

                if (addId.Count != this.Children.Count || this.timelineArray.Length % 3 != 0 || this.timelineArray.Length != totalFrameTimelineCount)
                {
                    MessageBox.Show("MoveClip:Render() ShapeCount does not match timeline count or timeline length is not sets of 6 or total shape count does not match frames count");
                    return null;
                }

                List<PointF> A = new List<PointF>();

                for (int i = 0; i < (timelineArray.Length / 3); i++)
                {
                    Shape shapeToRender = ((Shape)Children[timelineArray[(i * 3)]]);
                    Matrix matrixData = timelineArray[(i * 3) + 1] != 0xFFFF ? this._scFile.GetMatrixs()[timelineArray[(i * 3) + 1]] : null;
                    
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
            int timelineIndex = frameIndex * 6;

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
                Matrix matrixData = timelineArray[timelineIndex + 1] != 0xFFFF ? this._scFile.GetMatrixs()[timelineArray[timelineIndex + 1]] : new Matrix(1, 0, 0, 1, 0, 0);

                foreach (ShapeChunk chunk in ((Shape)Children[timelineArray[timelineIndex]]).Children)
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

                timelineIndex += 3;
            }
            

            ((MovieClipFrame)this.Frames[frameIndex]).setBitmap(finalShape);
        }

        public void SetId(ushort id)
        {
            _clipId = id;
        }

        public void SetFrameCount(short count)
        {
            _frameCount = count;
        }
        public void SetTimelineOffsetCount(int count)
        {
            _timelineOffsetCount = count * 3;
        }
        public void SetFramePerSecond(byte fps)
        {
            _framePerSeconds = fps;
        }
        public void AddShape(ScObject shape)
        {
            _shapes.Add(shape);
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
        public void SetShapes(List<ScObject> shapes)
        {
            _shapes = shapes;
        }
        public void SetFrames(List<ScObject> frames)
        {
            _frames = frames;
        }
        public void SetOffset(long position)
        {
            _offset = position;
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