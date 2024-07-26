using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using MathNet.Numerics.LinearAlgebra;
using SCEditor.Helpers;
using System.Windows.Forms;

namespace SCEditor.ScOld
{
    public class Shape : ScObject
    {
        #region Constructors

        public Shape(ScFile scFile)
        {
            _scFile = scFile;
            _chunks = new List<ScObject>();
        }

        public Shape(Shape shape, bool customAdd)
        {
            this.SetId(shape._shapeId);
            this.SetOffset(shape._offset);
            this.setLength(shape._length);
            this._scFile = shape._scFile;
            this._chunks = new List<ScObject>();

            for (int i = 0; i < shape._chunks.Count; i++)
            {
                ShapeChunk newChunk = new ShapeChunk((ShapeChunk)shape._chunks[i]);
                this._chunks.Add(newChunk);
            }

            this._shapeChunkCount = this._chunks.Count;
        }

        public Shape(Shape shape)
        {
            _scFile = shape.GetStorageObject();
            _chunks = new List<ScObject>();

            SetOffset(-Math.Abs(shape.GetOffset()));

            //Duplicate Shape
            using (FileStream input = new FileStream(_scFile.GetInfoFileName(), FileMode.Open))
            {
                input.Seek(Math.Abs(shape.GetOffset()), SeekOrigin.Begin);
                using (var br = new BinaryReader(input))
                {
                    var packetId = br.ReadByte();
                    var packetSize = br.ReadUInt32();
                    this.Read(null, br, packetId);
                }
            }

            foreach (ShapeChunk chunk in _chunks)
            {
                chunk.SetOffset(-Math.Abs(chunk.GetOffset()));
            }
        }

        #endregion

        #region Fields & Properties


        private bool _disposed;
        private ushort _shapeId;
        private List<ScObject> _chunks;
        private ScFile _scFile;
        private int _shapeChunkCount;
        private int _shapeChunkVertexCount;
        private Matrix _matrix;

        // internal string _shapeType; NOT USED
        public override ushort Id => _shapeId;
        public override List<ScObject> Children => _chunks;
        public Matrix Matrix => _matrix;
        public int shapeChunkCount => _shapeChunkCount;
        public int shapeChunkVertexCount => _shapeChunkVertexCount;
        public override SCObjectType objectType => SCObjectType.Shape;

        #endregion

        #region Methods

        public override int GetDataType()
        {
            return 0;
        }

        public override string GetDataTypeName()
        {
            return "Shapes";
        }

        public override string GetName()
        {
            return "Shape " + Id.ToString();
        }

        public List<ScObject> GetChunks()
        {
            return _chunks;
        }

        public void AddChunk(ScObject chunk)
        {
            _chunks.Add(chunk);
        }

        public void setChunks(List<ScObject> chunks)
        {
            _chunks = chunks;
        }

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/!\\ Experimental Rendering");
            sb.AppendLine("");
            sb.AppendLine("ShapeId: " + _shapeId);
            sb.AppendLine("Polygons: " + _chunks.Count);
            return sb.ToString();
        }

        public long GetOffset()
        {
            return _offset;
        }

        public ScFile GetStorageObject()
        {
            return _scFile;
        }

        public override bool IsImage()
        {
            return true;
        }

        public override Bitmap Render(RenderingOptions options)
        {
            try
            {
                if (options.LogConsole)
                    Console.WriteLine("Rendering image of " + _chunks.Count + " polygons");

                List<PointF> A = new List<PointF>();

                if (options.MatrixData != null)
                {
                    foreach (ShapeChunk chunk in _chunks)
                    {
                        PointF[] newXY = new PointF[chunk.XY.Length];

                        for (int xyIdx = 0; xyIdx < newXY.Length; xyIdx++)
                        {
                            float xNew = options.MatrixData.Elements[4] + options.MatrixData.Elements[0] * chunk.XY[xyIdx].X + options.MatrixData.Elements[2] * chunk.XY[xyIdx].Y;
                            float yNew = options.MatrixData.Elements[5] + options.MatrixData.Elements[1] * chunk.XY[xyIdx].X + options.MatrixData.Elements[3] * chunk.XY[xyIdx].Y;

                            newXY[xyIdx] = new PointF(xNew, yNew);
                        }

                        A.AddRange(newXY);
                    }
                }
                else
                {
                    PointF[] pointsXY = _chunks.SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                    A.AddRange(pointsXY.ToArray());
                }

                using (var xyPath = new GraphicsPath())
                {
                    xyPath.AddPolygon(A.ToArray());

                    var xyBound = Rectangle.Round(xyPath.GetBounds());

                    var width = xyBound.Width;
                    width = width > 0 ? width : 1;

                    var height = xyBound.Height;
                    height = height > 0 ? height : 1;

                    var x = xyBound.X;
                    var y = xyBound.Y;

                    var finalShape = new Bitmap(width, height);

                    if (options.LogConsole)
                    {
                        Console.WriteLine($"Rendering shape: W:{finalShape.Width} H:{finalShape.Height}\n");
                        Console.WriteLine("Length: " + _length + " | Offset: " + _offset);
                    }

                    foreach (ShapeChunk chunk in _chunks)
                    {
                        var texture = (Texture)_scFile.GetTextures()[chunk.GetTextureId()];
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


                                double[,] matrixArrayUV = {
                            {
                                gpuv.PathPoints[0].X, gpuv.PathPoints[1].X, gpuv.PathPoints[2].X
                            },
                            {
                                gpuv.PathPoints[0].Y, gpuv.PathPoints[1].Y, gpuv.PathPoints[2].Y
                            },
                            {
                                1, 1, 1
                            }};

                                PointF[] newXY = new PointF[chunk.XY.Length];

                                if (options.MatrixData != null)
                                {
                                    for (int xyIdx = 0; xyIdx < newXY.Length; xyIdx++)
                                    {
                                        float xNew = options.MatrixData.Elements[4] + options.MatrixData.Elements[0] * chunk.XY[xyIdx].X + options.MatrixData.Elements[2] * chunk.XY[xyIdx].Y;
                                        float yNew = options.MatrixData.Elements[5] + options.MatrixData.Elements[1] * chunk.XY[xyIdx].X + options.MatrixData.Elements[3] * chunk.XY[xyIdx].Y;

                                        newXY[xyIdx] = new PointF(xNew, yNew);
                                    }
                                }
                                else
                                {
                                    newXY = chunk.XY;
                                }

                                double[,] matrixArrayXY = {
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

                    return finalShape;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception Shape:Render()");
                return null;
            }

        }

        public sealed override void Read(ScFile swf, BinaryReader br, byte id)
        {
            _shapeId = br.ReadUInt16();
            _shapeChunkCount = br.ReadUInt16();

            for (int i = 0; i < _shapeChunkCount; i++)
            {
                _chunks.Add(new ShapeChunk(_scFile));
            }

            _shapeChunkVertexCount = id == 2 ? 4 * _shapeChunkCount : br.ReadUInt16();

            int index = 0;

            while (true)
            {
                byte chunkType;
                while (true)
                {
                    chunkType = br.ReadByte();
                    _length = (uint)br.ReadInt32();

                    if (_length < 0)
                        throw new Exception("Negative tag length in Shape.");

                    if (chunkType == 17 || chunkType == 22)
                    {
                        ShapeChunk chunk = (ShapeChunk)_chunks[index];
                        chunk.SetChunkId((ushort)index);
                        chunk.SetShapeId(_shapeId);
                        chunk.SetChunkType(chunkType);
                        chunk.Read(swf, br, id);

                        index++;
                    }
                    else if (chunkType == 6)
                    {
                        throw new Exception("SupercellSWF::TAG_SHAPE_DRAW_COLOR_FILL_COMMAND not supported");
                    }
                    else
                    {
                        break;
                    }
                }

                if (chunkType == 0)
                    break;

                Console.WriteLine("Unmanaged chunk type " + chunkType);
                br.ReadBytes((int)_length);
            }
        }

        public override void Write(FileStream input)
        {
            if (customAdded == true)
            {
                input.Seek(_scFile.GetEofOffset(), SeekOrigin.Begin);

                // DataType and Length
                input.Write(BitConverter.GetBytes(18), 0, 1);
                input.Write(BitConverter.GetBytes(0), 0, 4);

                // Write Data
                int dataLength = writeData(input);

                // Shape DataLength
                input.Seek(-(dataLength + 4), SeekOrigin.Current);
                input.Write(BitConverter.GetBytes(dataLength + 5), 0, 4);
                input.Seek(dataLength, SeekOrigin.Current);
            }
            else
            {
                if (_offset > 0)
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
                else
                {
                    throw new NotImplementedException();
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

            // ID And Chunk Count
            input.Write(BitConverter.GetBytes(_shapeId), 0, 2);
            input.Write(BitConverter.GetBytes(_chunks.Count), 0, 2);
            dataLength += 4;

            // Shape Chunk Vertex Count
            int shapeVertexCount = 0;
            int totalChunkLength = 0;
            input.Write(BitConverter.GetBytes(0), 0, 2);
            dataLength += 2;

            // Shape Chunk
            for (int i = 0; i < _chunks.Count; i++)
            {
                ShapeChunk chunk = (ShapeChunk)_chunks[i];

                // Add Vertex Count
                shapeVertexCount += chunk.UV.Length;

                // ChunkType and Length
                input.WriteByte(chunk.GetChunkType());
                input.Write(BitConverter.GetBytes(0), 0, 4);
                dataLength += 5;

                // Chunk Data
                int chunkLenght = 0;
                chunk.Write(input, out chunkLenght);

                // Chunk Length
                input.Seek(-(chunkLenght + 4), SeekOrigin.Current);
                input.Write(BitConverter.GetBytes(chunkLenght), 0, 4);
                input.Seek(chunkLenght, SeekOrigin.Current);

                totalChunkLength += chunkLenght + 5;
                dataLength += chunkLenght;
            }

            // Change Vertex Count
            input.Seek(-(totalChunkLength + 2), SeekOrigin.Current);
            input.Write(BitConverter.GetBytes((ushort)shapeVertexCount), 0, 2);
            input.Seek(totalChunkLength, SeekOrigin.Current);

            return dataLength;
        }

        public override void Dispose()
        {
            if (_disposed)
                return;

            foreach (var chunk in _chunks)
            {
                chunk.Dispose();
            }

            _disposed = true;
        }
        public void SetId(ushort id)
        {
            _shapeId = id;
        }

        public void setMatrix(Matrix data)
        {
            _matrix = data;
        }

        public void setShapeChunkVertexCount(int count)
        {
            _shapeChunkVertexCount = count;
        }

        public Shape Clone()
        {
            Shape newShape = (Shape)this.MemberwiseClone();
            newShape._chunks = new List<ScObject>();

            foreach (ShapeChunk chunk in this._chunks)
            {
                newShape.AddChunk(chunk.Clone());
            }

            return newShape;
        }

        #endregion
    }
}