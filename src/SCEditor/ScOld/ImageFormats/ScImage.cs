using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using KtxSharp;
using SCEditor.Helpers;
using SCEditor.ScOld.ImageDecoders;
using SCEditor.ScOld.ImageFormats;
using System.Collections.Generic;
using SCEditor.ScOld.ImageEncoder;
using Accord.Imaging.Filters;
using System.Windows.Media.Media3D;
using ZstdSharp;

namespace SCEditor.ScOld
{
    public abstract class ScImage : IDisposable
    {
        private bool _disposed;
        
        public int Width { get; set; }
        public int Height { get; set; }
        public uint KtxSize { get; set; }
        public string ExternalTexture { get; set; }
        public Bitmap Bitmap { get; set; }
        public bool Is32x32 { get; set; }
        
        public abstract string ImageFormat { get; }
        
        public void SetBitmap(Bitmap b)
        {
            Bitmap = b;
            Width = (ushort)b.Width;
            Height = (ushort)b.Height;

            b.SetResolution(96, 96);
        }
        
        public virtual void Print()
        {
            Console.WriteLine("Width: " + Width);
            Console.WriteLine("Height: " + Height);
        }
        
        public void Dispose()
        {
            Dispose(true);
        }

        internal static byte DecodeXBits(int value, int startBit, int bitCount)
        {
            int bitRange = 1 << bitCount;
            int rawValue = (value >> startBit) & (bitRange - 1);
            return (byte)((rawValue * 255 + ((bitRange >> 1) - 1)) / (bitRange - 1));
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Bitmap.Dispose();
            }

            _disposed = true;
        }
        
        public abstract void ReadImage(uint packetID, uint packetSize, BinaryReader br);
        public abstract void WriteImage(Stream bw);
    }

    public class ScImage<TFormat> : ScImage where TFormat : IImageFormat
    {
        public override string ImageFormat => TFormat.Name;

        private byte[] LoadKhronosTexture(byte[] data)
        {
            KtxStructure ktx = KtxLoader.LoadInput(new MemoryStream(data));

            Width = (int)ktx.header.pixelWidth;
            Height = (int)ktx.header.pixelHeight;

            var pixelData = GC.AllocateUninitializedArray<byte>(Width * Height * 4, true);
            byte[] inputBytes = ktx.textureData.textureDataOfMipmapLevel[0];
            (int blockWidth, int blockHeight) = ktx.header.glInternalFormat switch
            {
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_4x4_KHR => (4, 4),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_5x4_KHR => (5, 4),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_5x5_KHR => (5, 5),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_6x5_KHR => (6, 5),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_6x6_KHR => (6, 6),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_8x5_KHR => (8, 5),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_8x6_KHR => (8, 6),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_8x8_KHR => (8, 8),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_10x5_KHR => (10, 5),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_10x6_KHR => (10, 6),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_10x8_KHR => (10, 8),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_10x10_KHR => (10, 10),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_12x10_KHR => (12, 10),
                GlInternalFormat.GL_COMPRESSED_RGBA_ASTC_12x12_KHR => (12, 12),
                _ => throw new Exception("Invalid ASTC format")
            };
            Stopwatch sw = Stopwatch.StartNew();

            bool success = AstcDecoder.Decompress(inputBytes, (uint)inputBytes.Length, (uint)Width, (uint)Height,
                blockWidth, blockHeight, pixelData, (uint)pixelData.Length, (uint)Width * 4);
            if (!success)
            {
                throw new Exception("Failed to decompress ASTC");
            }

            Console.WriteLine($"Decompressed ASTC in {sw.ElapsedMilliseconds}ms ({Width}x{Height}, size: {inputBytes.Length / 1024}kb))");

            return pixelData;
        }

        public override unsafe void ReadImage(uint packetID, uint packetSize, BinaryReader br)
        {
            Width = br.ReadUInt16();
            Height = br.ReadUInt16();
            Is32x32 = (packetID - 27) < 3;

            byte[] pixelData;
            if (KtxSize != 0)
            {
                byte[] ktxBytes = br.ReadBytes((int)KtxSize);
                pixelData = LoadKhronosTexture(ktxBytes);
            } else if (ExternalTexture != "")
            {
                if (!Path.Exists(ExternalTexture))
                {
                    throw new Exception($"Failed to load external texture: {ExternalTexture}");
                }

                var extension = Path.GetExtension(ExternalTexture);

                if (extension == ".zktx")
                {
                    using (MemoryStream output = new MemoryStream())
                    {
                        using (FileStream input = new FileStream(ExternalTexture, FileMode.Open))
                        {
                            using (var decompressionStream = new DecompressionStream(input))
                            {
                                decompressionStream.CopyTo(output);
                                decompressionStream.Close();
                                decompressionStream.Dispose();
                            }
                        }

                        pixelData = LoadKhronosTexture(output.GetBuffer());
                    }
                } else if (extension == ".ktx")
                {
                    var ktxBytes = File.ReadAllBytes(ExternalTexture);
                    pixelData = LoadKhronosTexture(ktxBytes);
                } else
                {
                    throw new Exception($"Unknown file type: {ExternalTexture}");
                }

            }
            else
            {
                pixelData = br.ReadBytes(Width * Height * TFormat.PixelSize);
            }
            
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            if (Is32x32)
            {
                int[] pixelArray = new int[Height * Width];
                var span = new ReadOnlySpan<byte>(pixelData);
                for (int h = 0; h < Height; h++)
                for (int w = 0; w < Width; w++)
                {
                    TFormat.ReadColor(span, out int color);
                    pixelArray[h * Width + w] = color;
                    span = span.Slice(TFormat.PixelSize);
                }

                pixelArray = Utils.Solve32X32Blocks(Width, Height, pixelArray);

                var bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                var bitmapSpan = (byte*)bitmapData.Scan0;
                for (int h = 0; h < Height; h++)
                for (int w = 0; w < Width; w++)
                {
                    *(int*)bitmapSpan = pixelArray[h * Width + w];
                    bitmapSpan += 4;
                }

                Bitmap.UnlockBits(bitmapData);
            }
            else
            {
                var span = new ReadOnlySpan<byte>(pixelData);
                var bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                var bitmapSpan = (byte*) bitmapData.Scan0;
                for (int h = 0; h < Height; h++)
                for (int w = 0; w < Width; w++)
                {
                    TFormat.ReadColor(span, out int color);
                    *(int *) bitmapSpan = color;
                    span = span.Slice(TFormat.PixelSize);
                    bitmapSpan += 4;
                }
                
                Bitmap.UnlockBits(bitmapData);
            }
        }
        
        public override unsafe void WriteImage(Stream bw)
        {
            Span<byte> widthBytes = stackalloc byte[2];
            Span<byte> heightBytes = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(widthBytes, (ushort) Width);
            BinaryPrimitives.WriteUInt16LittleEndian(heightBytes, (ushort) Height);
            bw.Write(widthBytes);
            bw.Write(heightBytes);

            int offset = 0;
            Span<byte> colorBytesList = new byte[(Height * Width * TFormat.PixelSize)];

            if (Is32x32)
            {
                Color[,] oldPixelArray = new Color[Bitmap.Height, Bitmap.Width];
                int oHeight = Bitmap.Height;
                int oWidth = Bitmap.Width;
                for (int col = 0; col < oHeight; col++)
                for (int row = 0; row < oWidth; row++)
                {
                    oldPixelArray[col, row] = Bitmap.GetPixel(row, col);
                }

                Color[,] pixelArray = Utils.Create32x32Blocks(Bitmap.Width, Bitmap.Height, oldPixelArray);

                for (int column = 0; column < Bitmap.Height; column++)
                for (int row = 0; row < Bitmap.Width; row++)
                {
                    Span<byte> colorBytes = new byte[TFormat.PixelSize];
                    TFormat.WriteColor(colorBytes, pixelArray[column, row].ToArgb());
                    colorBytes.CopyTo(colorBytesList[offset..]);
                    offset += TFormat.PixelSize;
                }
            }
            else
            {
                for (int column = 0; column < Bitmap.Height; column++)
                for (int row = 0; row < Bitmap.Width; row++)
                {
                    Span<byte> colorBytes = new byte[TFormat.PixelSize];

                    int argb = Bitmap.GetPixel(row, column).ToArgb();
                    int a = (argb >> 24) & 0xFF;
                    int r = (argb >> 16) & 0xFF;
                    int g = (argb >> 8) & 0xFF;
                    int b = argb & 0xFF;

                    int abgr = (a << 24) | (b << 16) | (g << 8) | r;

                    TFormat.WriteColor(colorBytes, abgr);
                    colorBytes.CopyTo(colorBytesList[offset..]);
                    offset += TFormat.PixelSize;
                }
            }

            if (KtxSize != 0)
            {
                // todo
                // bw.Write(AstcEncoder.Encode(colorBytesList));
            }
            else
            {
                bw.Write(colorBytesList);
            }
        }
    }
}
