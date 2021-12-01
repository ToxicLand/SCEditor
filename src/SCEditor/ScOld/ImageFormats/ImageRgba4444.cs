using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using SCEditor.Helpers;

namespace SCEditor.ScOld
{
    internal class ImageRgba4444 : ScImage
    {
        public ImageRgba4444()
        {
            // Space
        }

        public override string GetImageTypeName()
        {
            return "RGB4444";
        }

        public override void ReadImage(uint packetID, uint packetSize, BinaryReader br)
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


            Color[,] pixelArray = new Color[_height, _width];
            for (int row = 0; row < pixelArray.GetLength(0); row++)
            {
                for (int col = 0; col < pixelArray.GetLength(1); col++)
                {
                    ushort color = br.ReadUInt16();
                    //Debug.Write(color+"|");
                    int alpha = DecodeXBits(color, 0, 4);
                    int red = DecodeXBits(color, 12, 4);
                    int green = DecodeXBits(color, 8, 4);
                    int blue = DecodeXBits(color, 4, 4);
                    pixelArray[row, col] = Color.FromArgb(alpha, red, green, blue);

                }

            }
            if (Is32x32)
                pixelArray = Utils.Solve32X32Blocks(_width, _height, pixelArray);

            for (int row = 0; row < pixelArray.GetLength(0); row++)
            {
                for (int col = 0; col < pixelArray.GetLength(1); col++)
                {
                    //Color pxColor = Color.Red;
                    Color pxColor = pixelArray[row, col];
                    _bitmap.SetPixel(col, row, pxColor);
                }
            }

            sw.Stop();
            Console.WriteLine("ImageRgba4444.ReadImage finished in {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        public override void Print()
        {
            base.Print();
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
                    int val = 0;
                    val += (a / 0x10);
                    val += ((b / 0x10) << 4);
                    val += ((g / 0x10) << 8);
                    val += ((r / 0x10) << 12);
                    UInt16 cc4444 = (ushort)val;

                    input.Write(BitConverter.GetBytes(cc4444), 0, 2);
                }
            }
        }
    }
}
