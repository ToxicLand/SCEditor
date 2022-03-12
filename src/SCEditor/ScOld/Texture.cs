using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows;
using SCEditor.ScOld.ImageFormats;

namespace SCEditor.ScOld
{
    public class Texture : ScObject, IDisposable
    {
        #region Constants

        private static readonly Dictionary<byte, Type> s_imageTypes;

        #endregion

        #region Constructors

        static Texture()
        {
            s_imageTypes = new Dictionary<byte, Type>
            {
                {0, typeof(ImageRgba8888)},
                {1, typeof(ImageRgba8888)},
                {2, typeof(ImageRgba4444)},
                {3, typeof(ImageRgba5551 )},
                {4, typeof(ImageRgb565)},
                {6, typeof(ImageLuminance8Alpha8)},
                {9, typeof(ImageRgba4444)},
                {10, typeof(ImageLuminance8)}
            };
        }

        public Texture(ScFile scs)
        {
            _scFile = scs;
            _textureId = (ushort) _scFile.GetTextures().Count();
        }

        public Texture(ScFile scs, Bitmap bitmap)
        {
            _scFile = scs;
            _textureId = (ushort)_scFile.GetTextures().Count();
            _imageType = 0;
            _image = (ScImage)Activator.CreateInstance(s_imageTypes[_imageType]);
            _image.SetBitmap(bitmap);
            _textureId = (ushort)_scFile.GetTextures().Count();
            _offset = -1;
        }

        public Texture(ScFile scs, Bitmap bitmap, byte imageType)
        {
            _scFile = scs;
            _textureId = (ushort)_scFile.GetTextures().Count();
            _imageType = imageType;
            _image = (ScImage)Activator.CreateInstance(s_imageTypes[_imageType]);
            _image.SetBitmap(bitmap);
            _textureId = (ushort)_scFile.GetTextures().Count();
            _offset = -1;
        }

        public Texture(byte imageType, int width, int height, ScFile scfile)
        {
            _imageType = imageType;
            _scFile = scfile;
            _textureId = (ushort) scfile.GetTextures().Count;
            _image = (ScImage)Activator.CreateInstance(s_imageTypes[_imageType]);
            _image.SetBitmap(new Bitmap(width, height));
            _offset = -1;
        }

        public Texture(Texture t)
        {
            _imageType = t.GetImageType();
            _scFile = t.GetStorageObject();
            _textureId = (ushort) _scFile.GetTextures().Count();
            if (s_imageTypes.ContainsKey(_imageType))
            {
                _image = (ScImage) Activator.CreateInstance(s_imageTypes[_imageType]);
            }
            else
            {
                _image = new ScImage();
            }
            _image.SetBitmap(new Bitmap(t.Bitmap));
            _offset = t.GetOffset() > 0 ? -t.GetOffset() : -1 /*t.GetOffset()*/;
        }

        #endregion

        #region Fields & Properties

        internal byte PacketId;
        internal byte _imageType;
        internal ushort _textureId;
        internal uint _packetSize;

        internal bool _disposed;
        internal ScFile _scFile;
        internal ScImage _image;

        public override Bitmap Bitmap => _image.GetBitmap();

        #endregion

        #region Methods

        public override ushort Id => _textureId;

        public override int GetDataType()
        {
            return 2;
        }

        public override string GetDataTypeName()
        {
            return "Textures";
        }

        internal ScImage GetImage()
        {
            return _image;
        }

        public byte GetImageType()
        {
            return _imageType;
        }

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TextureId: " + _textureId);
            sb.AppendLine("ImageType: " + _imageType);
            sb.AppendLine("ImageFormat: " + _image.GetImageTypeName());
            sb.AppendLine("Width: " + _image.Width);
            sb.AppendLine("Height: " + _image.Height);
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

        public ushort GetTextureId()
        {
            return _textureId;
        }

        public override bool IsImage()
        {
            return true;
        }

        public new void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _image.Dispose();
            }

            _disposed = true;
        }
        public void Read(byte packetID, uint packetSize, BinaryReader br)
        {
            this.PacketId = packetID;
            this._packetSize = packetSize;
            _imageType = br.ReadByte();

            if (s_imageTypes.ContainsKey(_imageType))
                _image = (ScImage)Activator.CreateInstance(s_imageTypes[_imageType]);
            else
                _image = new ScImage();

            _image.ReadImage(packetID, packetSize, br);
        }

        public override Bitmap Render(RenderingOptions options)
        {
            return Bitmap;
        }

        public override void Write(FileStream input)
        {
            int bytesForPXFormat = 4;
            switch (_imageType)
            {
                case 2:
                case 9:
                    bytesForPXFormat = 2;
                    break;
                case 3:
                    bytesForPXFormat = 2;
                    break;
                case 4:
                    bytesForPXFormat = 2;
                    break;
                case 6:
                    bytesForPXFormat = 2;
                    break;
                case 10:
                    bytesForPXFormat = 1;
                    break;
            }

            UInt32 packetSize = (uint) ((_image.Width) * (_image.Height) * bytesForPXFormat) + 5;

            _image.is32x32 = (this.PacketId - 27) < 3;

            if (_offset < 0) // New
            { 
                input.Seek(_scFile.GetEofTexOffset(), SeekOrigin.Begin);
                input.WriteByte(1);
                input.Write(BitConverter.GetBytes(packetSize), 0, 4);
                input.WriteByte(_imageType);

                _image.WriteImage(input);
                _offset = _scFile.GetEofTexOffset();
                _scFile.SetEofTexOffset(input.Position);

                input.Write(new byte[] {0, 0, 0, 0, 0}, 0, 5);
            }
            else // Existing
            {
                if (packetSize != this._packetSize)
                {
                    input.Seek(0, SeekOrigin.Begin);

                    using (MemoryStream newTexData = new MemoryStream())
                    {
                        for (int i = 0; i < _scFile.GetTextures().Count; i++)
                        {
                            Texture tex = (Texture) _scFile.GetTextures()[i];

                            if (tex.Id == this.Id)
                            {
                                newTexData.WriteByte(this.PacketId);
                                newTexData.Write(BitConverter.GetBytes(packetSize), 0, 4);
                                newTexData.WriteByte(_imageType);
                                _image.WriteImage(newTexData);
                            }
                            else
                            {
                                input.Seek(tex._offset, SeekOrigin.Begin);

                                byte[] texData = new byte[tex._packetSize + 5];
                                input.Read(texData, 0, texData.Length);

                                newTexData.Write(texData, 0, texData.Length);
                            }   

                            if (i + 1 == _scFile.GetTextures().Count)
                                _scFile.SetEofTexOffset(newTexData.Length);

                            if (this.Id != tex.Id && tex.offset > this._offset)
                            {
                                uint offsetDifference = packetSize - this._packetSize;

                                tex.SetOffset(tex._offset + (offsetDifference));
                            }
                        }

                        newTexData.Write(new byte[] { 0, 0, 0, 0, 0 }, 0, 5);

                        long calSize = (input.Length + packetSize) - this._packetSize;
                        if (newTexData.Length != calSize)
                            throw new Exception($"Texture newData length is abnormal\nnewData: {newTexData.Length} | Calculated Size: {calSize}");

                        input.Seek(0, SeekOrigin.Begin);
                        newTexData.Seek(0, SeekOrigin.Begin);
                        newTexData.CopyTo(input);
                    }

                    this._packetSize = packetSize;
                }
                else
                {
                    /**
                    input.Seek(_offset, SeekOrigin.Current);
                    input.WriteByte(Convert.ToByte(1));
                    input.Write(BitConverter.GetBytes(packetSize), 0, 4);
                    input.WriteByte(_imageType);
                    _image.WriteImage(input);
                    **/
                    
                    using (MemoryStream newTexData = new MemoryStream())
                    {
                        input.Seek(0, SeekOrigin.Begin);
                        byte[] newData = new byte[_offset];
                        input.Read(newData, 0, newData.Length);
                        newTexData.Write(newData, 0, newData.Length);

                        newTexData.WriteByte(Convert.ToByte(this.PacketId));

                        newTexData.Write(BitConverter.GetBytes(packetSize), 0, 4);
                        newTexData.WriteByte(_imageType);
                        _image.WriteImage(newTexData);

                        input.Seek(packetSize + 5, SeekOrigin.Current);
                        newData = new byte[input.Length - input.Position];

                        input.Read(newData, 0, newData.Length);
                        newTexData.Write(newData, 0, newData.Length);

                        if (newTexData.Length != input.Length)
                            throw new Exception($"Texture newData length is not equal\nnewData: {newTexData.Length} | Calculated Size: {input.Length}");

                        input.Seek(0, SeekOrigin.Begin);
                        newTexData.Seek(0, SeekOrigin.Begin);
                        newTexData.CopyTo(input);
                    }
                    
                }
            }
        }

        #endregion
    }
}
