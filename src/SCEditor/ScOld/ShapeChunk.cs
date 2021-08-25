using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using SCEditor.Helpers;
using Point = System.Drawing.Point;
using System.Linq;

namespace SCEditor.ScOld
{
    public class ShapeChunk : ScObject, IDisposable
    {
        #region Constructors

        public ShapeChunk(ScFile scs)
        {
            _scFile = scs;
            //_pointsXY = new List<PointF>();
            //_pointsUV = new List<PointF>();
        }

        public ShapeChunk(ShapeChunk chunk)
        {
            this.SetChunkId(chunk._chunkId);
            this.SetShapeId(chunk._shapeId);
            this.SetTextureId(chunk._textureId);
            this.SetXY(chunk._xy);
            this.SetUV(chunk._uv);
            this.SetOffset(chunk._offset);
            this.SetVertexCount(chunk._vertexCount);
            this._scFile = chunk._scFile;
        }

        #endregion

        #region Fields & Properties

        private readonly ScFile _scFile;

        private ushort _chunkId;
        private ushort _shapeId;
        private byte _textureId;
        private byte _chunkType;
        private PointF[] _xy;
        private PointF[] _uv;
        private int _vertexCount;
        private bool _disposed;

        public PointF[] XY => _xy;

        public PointF[] UV => _uv;

        public override ushort Id => _chunkId;

        #endregion

        #region Methods

        public byte GetChunkType()
        {
            return _chunkType;
        }

        public override int GetDataType()
        {
            return 99;
        }

        public override string GetDataTypeName()
        {
            return "ShapeChunks";
        } 

        public override string GetName()
        {
            return "Chunk " + Id;
        } 

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ChunkId: " + _chunkId);
            sb.AppendLine("ShapeId: " + _shapeId);
            sb.AppendLine("TextureId: " + _textureId);
            #if DEBUG
            sb.AppendLine("ChunkType: " + _chunkType);
            #endif
            return sb.ToString();
        }

        public long GetOffset()
        {
            return _offset;
        }

        public ushort GetShapeId()
        {
            return _shapeId;
        }

        public byte GetTextureId()
        {
            return _textureId;
        }

        public override bool IsImage()
        {
            return true;
        }

        public override void Read(BinaryReader br, string id)
        {
            _offset = br.BaseStream.Position;
            _textureId = br.ReadByte();

            var texture = (Texture) _scFile.GetTextures()[_textureId];

            if (texture == null)
                throw new InvalidOperationException($"Texture {_textureId} wasn't loaded yet.");

            _vertexCount = id == "04" ? Convert.ToByte(4) : br.ReadByte();

            _xy = new PointF[_vertexCount];
            _uv = new PointF[_vertexCount];

            for (int i = 0; i < _vertexCount; i++)
            {
                var x = (float)((float)br.ReadInt32() * 0.05f);
                var y = (float)((float)br.ReadInt32() * 0.05f);
                _xy[i] = new PointF(x, y);
            }

            if (_chunkType == 22)
            {
                for (int i = 0; i < _vertexCount; i++)
                {
                    var u = (float)((float)br.ReadUInt16() / 65535f) * texture.GetImage().Width;
                    var v = (float)((float)br.ReadUInt16() / 65535f) * texture.GetImage().Height;

                    //var u = br.ReadUInt16();
                    //var v = br.ReadUInt16();

                    _uv[i] = new PointF(u, v);
                }
            }
            else
            {
                for (int i = 0; i < _vertexCount; i++)
                {
                    float u = (float)((float)br.ReadUInt16()); // image.Width);
                    float v = (float)((float)br.ReadUInt16()); // image.Height);//(short) (65535 * br.ReadInt16() / image.Height);

                    //float u = (ushort)(0xFFFFL * br.ReadUInt16() / texture._image.Width);
                    //float v = (ushort)(0xFFFFL * br.ReadUInt16() / texture._image.Height);

                    _uv[i] = new PointF(u, v);
                }
            }
        }

        public override Bitmap Render(RenderingOptions options)
        {

            Console.WriteLine("Rendering Chunk " + _chunkId + " from shape " + _shapeId + " | Texture ID: " + _textureId + " | Chunk Type: " + _chunkType);

            Bitmap result = null;

            var texture = (Texture) _scFile.GetTextures()[_textureId];
            if (texture != null)
            {
                Bitmap bitmap = texture.Bitmap;

                Console.WriteLine("Rendering polygon image of " + _uv.Length + " points");
                foreach (PointF uv in _uv)
                {
                    Console.WriteLine("u: " + uv.X + ", v: " + uv.Y);
                }

                using (var gpuv = new GraphicsPath())
                {
                    gpuv.AddPolygon(_uv.ToArray());

                    var uvBounds = Rectangle.Round(gpuv.GetBounds());

                    var gpuvWidth = (int) uvBounds.Width;
                    gpuvWidth = gpuvWidth > 0 ? gpuvWidth : 1;
                    Console.WriteLine("gpuvWidth: " + gpuvWidth);

                    var gpuvHeight = (int) uvBounds.Height;
                    gpuvHeight = gpuvHeight > 0 ? gpuvHeight : 1;

                    Console.WriteLine("gpuvHeight: " + gpuvHeight);

                    var x = uvBounds.X;
                    var y = uvBounds.Y;

                    var chunk = new Bitmap(gpuvWidth, gpuvHeight);
                    if (gpuvWidth > 0 && gpuvHeight > 0)
                    {
                        //bufferizing shape
                        using (Graphics g = Graphics.FromImage(chunk))
                        {
                            //On conserve la qualité de l'image intacte
                            gpuv.Transform(new Matrix(1, 0, 0, 1, -x, -y));
                            g.SetClip(gpuv);
                            g.DrawImage(bitmap, -x, -y);

                            if (options.ViewPolygons)
                                g.DrawPath(new Pen(Color.DeepSkyBlue, 1), gpuv);
                            g.Flush();
                        }
                    }

                    result = chunk;
                }
            }
            return result;
        }

        public void Replace(Bitmap chunk)
        {
            var texture = (Texture)_scFile.GetTextures()[_textureId];
            if (texture != null)
            {
                Bitmap bitmap = texture.Bitmap;

                GraphicsPath gpuv = new GraphicsPath();
                gpuv.AddPolygon(_uv);

                var uvBounds = Rectangle.Round(gpuv.GetBounds());
                var x = uvBounds.X;
                var y = uvBounds.Y;
                var width = (int)uvBounds.Width;
                var height = (int)uvBounds.Height;

                GraphicsPath gpChunk = new GraphicsPath();
                gpChunk.AddRectangle(new Rectangle(0, 0, width, height));

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    gpChunk.Transform(new Matrix(1, 0, 0, 1, x, y));
                    g.SetClip(gpuv);
                    g.Clear(Color.Transparent);
                    g.DrawImage(chunk, x, y);
                    g.Flush();
                }
            }
        }

        public override void Write(FileStream input)
        {
            if (_offset < 0)
            {
                _offset = input.Position;
                input.WriteByte(_textureId);
                input.WriteByte((byte)_uv.Length);
                foreach (var pointXY in _xy)
                {
                    input.Write(BitConverter.GetBytes((int)(pointXY.X * 20)), 0, 4);
                    input.Write(BitConverter.GetBytes((int)(pointXY.Y * 20)), 0, 4);
                }

                var texture = (Texture)_scFile.GetTextures()[_textureId];

                if (_chunkType == 22)
                {
                    foreach (var pointUV in _uv)
                    {
                        input.Write(BitConverter.GetBytes((ushort)((pointUV.X / texture.GetImage().Width) * 65535f)), 0, 2);
                        input.Write(BitConverter.GetBytes((ushort)((pointUV.Y / texture.GetImage().Height) * 65535f)), 0, 2);
                    }
                }
                else
                {
                    foreach (var pointUV in _uv)
                    {
                        input.Write(BitConverter.GetBytes((ushort)(pointUV.X)), 0, 2);
                        input.Write(BitConverter.GetBytes((ushort)(pointUV.Y)), 0, 2);
                    }
                }
            }
            else
            {
                input.Seek(_offset, SeekOrigin.Begin);
                input.WriteByte(_textureId);
                input.Position += 1;

                foreach (var pointXY in _xy)
                {
                    input.Write(BitConverter.GetBytes((int)(pointXY.X * 20)), 0, 4);
                    input.Write(BitConverter.GetBytes((int)(pointXY.Y * 20)), 0, 4);
                }
            }
        }
        public override void Write(FileStream input, int inOffset, out int outOffset)
        {
            outOffset = inOffset;
            var texture = (Texture)_scFile.GetTextures()[_textureId];

            // Texture ID
            input.Write(BitConverter.GetBytes(_textureId), 0, 1);
            outOffset += 1;

            int shapePointCount = _xy.Length == _uv.Length ? _xy.Length : throw new Exception("shapePointCount Not Match WRITE()");

            // Chunk Points
            input.Write(BitConverter.GetBytes(shapePointCount), 0, 1);
            outOffset += 1;

            // XY Points
            foreach (var pointXY in _xy)
            {
                input.Write(BitConverter.GetBytes((int)(pointXY.X * 20)), 0, 4);
                input.Write(BitConverter.GetBytes((int)(pointXY.Y * 20)), 0, 4);
                outOffset += 8;
            }

            // UV Points
            if (_chunkType == 22)
            {
                foreach (var pointUV in _uv)
                {
                    input.Write(BitConverter.GetBytes((ushort)((pointUV.X / texture.GetImage().Width) * 65535f)), 0, 2);
                    input.Write(BitConverter.GetBytes((ushort)((pointUV.Y / texture.GetImage().Height) * 65535f)), 0, 2);
                    outOffset += 4;
                }

            }
            else
            {
                foreach (var pointUV in _uv)
                {
                    input.Write(BitConverter.GetBytes((ushort)(pointUV.X)), 0, 2);
                    input.Write(BitConverter.GetBytes((ushort)(pointUV.Y)), 0, 2);
                    outOffset += 4;
                }
            }
        }
        public override void Dispose()
        {
            if (_disposed)
                return;

            //_path?.Dispose();

            _disposed = true;
        }
        public void SetChunkId(ushort id)
        {
            _chunkId = id;
        }

        public void SetChunkType(byte type)
        {
            _chunkType = type;
        }

        public void SetOffset(long offset)
        {
            _offset = offset;
        }

        public void SetVertexCount(int count)
        {
            _vertexCount = count;
        }

        public void SetShapeId(ushort id)
        {
            _shapeId = id;
        }

        public void SetXY(PointF[] xy)
        {
            _xy = xy;
        }

        public void SetUV(PointF[] uv)
        {
            _uv = uv;
        }

        public void SetTextureId(byte id)
        {
            _textureId = id;
        }

        public ShapeChunk Clone()
        {
            ShapeChunk newChunk = (ShapeChunk)this.MemberwiseClone();
            return newChunk;
        }
        #endregion
    }
}
