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
                    var packetId = br.ReadByte().ToString("X2");
                    var packetSize = br.ReadUInt32();
                    this.Read(br, packetId);
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

        public sealed override void Read(BinaryReader br, string id)
        {
            _shapeId = br.ReadUInt16();
            _shapeChunkCount = br.ReadUInt16();

            for (int i = 0; i < _shapeChunkCount; i++)
            {
                _chunks.Add(new ShapeChunk(_scFile));
            }

            _shapeChunkVertexCount = id == "12" ? br.ReadUInt16() : 4 * _shapeChunkCount;

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
                        ShapeChunk chunk = (ShapeChunk) _chunks[index];
                        chunk.SetChunkId((ushort)index);
                        chunk.SetShapeId(_shapeId);
                        chunk.SetChunkType(chunkType);
                        chunk.Read(br, id);

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

        public override Bitmap Render(RenderingOptions options)
        {
            try
            {
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

                    Console.WriteLine($"Rendering shape: W:{finalShape.Width} H:{finalShape.Height}\n");
                    Console.WriteLine("Length: " + _length + " | Offset: " + _offset);

                    foreach (ShapeChunk chunk in _chunks)
                    {
                        var texture = (Texture)_scFile.GetTextures()[chunk.GetTextureId()];
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

                            //Calculate transformation Matrix of UV
                            //double[,] matrixArrayUV = { { polygonUV[0].X, polygonUV[1].X, polygonUV[2].X }, { polygonUV[0].Y, polygonUV[1].Y, polygonUV[2].Y }, { 1, 1, 1 } };
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
                            //m = new Matrix((float)transformMatrix[0, 0], (float)transformMatrix[1, 0], (float)transformMatrix[0, 1], (float)transformMatrix[1, 1], (float)Math.Round(transformMatrix[0, 2]), (float)Math.Round(transformMatrix[1, 2]));

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

                    return finalShape;

                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception Shape:Render()");
                return null;
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
                    shapeVertexCount += chunk.XY.Length;

                    // ChunkType and Length
                    input.WriteByte(chunk.GetChunkType());
                    input.Write(BitConverter.GetBytes(0), 0, 4);
                    dataLength += 5;

                    int chunkLenght = 0;

                    // Chunk Data
                    _chunks[i].Write(input, chunkLenght, out chunkLenght);

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

                // Shape DataLength
                input.Seek(-(dataLength + 4), SeekOrigin.Current);
                input.Write(BitConverter.GetBytes(dataLength + 5), 0, 4);
                input.Seek(dataLength, SeekOrigin.Current);
            }
            else
            {
                if (_offset < 0) //new
                {
                    using (FileStream readInput = new FileStream(_scFile.GetInfoFileName(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //Positionnement des curseurs
                        readInput.Seek(Math.Abs(_offset), SeekOrigin.Begin);
                        input.Seek(_scFile.GetEofOffset(), SeekOrigin.Begin);

                        //type and length
                        byte dataType = (byte)readInput.ReadByte();
                        byte[] dataLength = new byte[4];
                        readInput.Read(dataLength, 0, 4);

                        input.WriteByte(dataType);
                        input.Write(dataLength, 0, 4);

                        //shape
                        readInput.Seek(2, SeekOrigin.Current);
                        input.Write(BitConverter.GetBytes(_shapeId), 0, 2);

                        byte[] unknown1 = new byte[2];
                        readInput.Read(unknown1, 0, 2); //0100
                        input.Write(unknown1, 0, 2);

                        if (dataType == 18)
                        {
                            byte[] unknown2 = new byte[2];
                            readInput.Read(unknown2, 0, 2); //0400
                            input.Write(unknown2, 0, 2);
                        }

                        int chunkCounter = 0;
                        while (true)
                        {
                            byte shapeType;
                            byte[] length = new byte[4];
                            while (true)
                            {
                                shapeType = (byte)readInput.ReadByte(); //11
                                input.WriteByte(shapeType);

                                readInput.Read(length, 0, 4); //32000000
                                input.Write(length, 0, 4);

                                if (shapeType == 17 || shapeType == 22)
                                {
                                    Console.WriteLine("Managed shape type " + shapeType);
                                    _chunks[chunkCounter].Write(input);
                                    chunkCounter++;
                                    readInput.Seek(BitConverter.ToInt32(length, 0), SeekOrigin.Current);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (shapeType == 0)
                            {
                                break;
                            }
                            Console.WriteLine("Unmanaged shape type " + shapeType);
                            for (int i = 0; i < BitConverter.ToInt32(length, 0); i++)
                            {
                                input.WriteByte((byte)readInput.ReadByte());
                            }
                        }
                    }
                }
            }

            input.Write(new byte[] { 0, 0, 0, 0, 0 }, 0, 5);
            _offset = _scFile.GetEofOffset();
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