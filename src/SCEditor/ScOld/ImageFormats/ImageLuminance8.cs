using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using SCEditor.Helpers;

namespace SCEditor.ScOld.ImageFormats
{
    internal class ImageLuminance8 : ScImage
    {
        public ImageLuminance8()
        {

        }
        public override string GetImageTypeName()
        {
            return "Luminance8";
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

                    ushort color = br.ReadByte();
                    int rgb = color;
                    int red = rgb;
                    int green = rgb;
                    int blue = rgb;
                    pixelArray[row, col] = Color.FromArgb(red, green, blue);
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
            Console.WriteLine("ImageLuminance8.ReadImage finished in {0}ms", sw.Elapsed.TotalMilliseconds);
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
                    input.WriteByte(_bitmap.GetPixel(row, column).R);
                }
            }
        }
    }
}
