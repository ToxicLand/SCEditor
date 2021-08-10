using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using SCEditor.Helpers;

namespace SCEditor.ScOld
{
    internal class ImageRgba8888 : ScImage
    {
        #region Constructors
        public ImageRgba8888()
        {
            // Space
        }
        #endregion

        #region Methods

        public override unsafe void ReadImage(uint packetID, uint packetSize, BinaryReader br)
        {
            base.ReadImage(packetID, packetSize, br);

            var sw = Stopwatch.StartNew();

            bool Is32x32 = (packetID - 27) < 3;

            Console.WriteLine(@"packetID: " + packetID);
            Console.WriteLine(@"packetSize: " + packetSize);
            Console.WriteLine(@"texPixelFormat: " + _imageType);
            Console.WriteLine(@"texWidth: " + _width);
            Console.WriteLine(@"texHeight: " + _height);
            Console.WriteLine(@"Is32x32: " + Is32x32);

            _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);

            if (Is32x32)
            {
                Color[,] pixelArray = new Color[_height, _width];
                for (int row = 0; row < pixelArray.GetLength(0); row++)
                {
                    for (int col = 0; col < pixelArray.GetLength(1); col++)
                    {
                        byte r = br.ReadByte();
                        byte g = br.ReadByte();
                        byte b = br.ReadByte();
                        byte a = br.ReadByte();
                        pixelArray[row, col] = Color.FromArgb((int) ((a << 24) | (r << 16) | (g << 8) | b));
                    }
                }

                pixelArray = Utils.Solve32X32Blocks(_width, _height, pixelArray);

                for (int row = 0; row < pixelArray.GetLength(0); row++)
                {
                    for (int col = 0; col < pixelArray.GetLength(1); col++)
                    {
                        Color pxColor = pixelArray[row, col];
                        _bitmap.SetPixel(col, row, pxColor);
                    }
                }
            }
            else
            {
                var rect = new Rectangle(0, 0, _width, _height);
                var data = _bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                var length = data.Stride * data.Height;
                var sourceBytes = br.ReadBytes(length);

                byte* dst = (byte*) data.Scan0.ToPointer();
                fixed (byte* fixSrc = sourceBytes)
                {
                    for (byte* src = fixSrc; src < fixSrc + length; src += 4)
                    {
                        var r = *(src);
                        var g = *(src + 1);
                        var b = *(src + 2);
                        var a = *(src + 3);

                        *(dst) = b;
                        *(dst + 1) = g;
                        *(dst + 2) = r;
                        *(dst + 3) = a;

                        dst += 4;
                    }
                }
                _bitmap.UnlockBits(data);
            }

            sw.Stop();
            Console.WriteLine(@"ImageRgba8888.ReadImage finished in {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        public override void WriteImage(Stream input)
        {
            base.WriteImage(input);

            for (int column = 0; column < _bitmap.Height; column++)
            {
                for (int row = 0; row < _bitmap.Width; row++)
                {
                    Color c = _bitmap.GetPixel(row, column);
                    var r = c.R;
                    var g = c.G;
                    var b = c.B;
                    var a = c.A;

                    if (a == 0)
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }

                    input.WriteByte(r);
                    input.WriteByte(g);
                    input.WriteByte(b);
                    input.WriteByte(a);
                }
            }
        }

        public override string GetImageTypeName()
        {
            return "RGB8888";
        }
        #endregion
    }
}
