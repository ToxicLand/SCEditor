using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using SCEditor.Helpers;

namespace SCEditor.ScOld
{
    internal class ImageRgba5551 : ScImage
    {
        public ImageRgba5551()
        {
            // Space
        }

        public override string GetImageTypeName()
        {
            return "RGBA5551";
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
                    int red = Utils.ConvertMap[(color >> 11) & 0x1F];
                    int green = Utils.ConvertMap[(color >> 6) & 0x1F];
                    int blue = Utils.ConvertMap[(color >> 1) & 0x1F];
                    int alpha = (int)(color & 0x0001) == 1 ? 0xFF : 0x00;
                    pixelArray[row, col] = Color.FromArgb(alpha, red, green, blue); ;
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
            Console.WriteLine("ImageRgba5551.ReadImage finished in {0}ms", sw.Elapsed.TotalMilliseconds);
        }

        public override void Print()
        {
            base.Print();
        }
    }
}
