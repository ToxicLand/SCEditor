using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SCEditor.Helpers
{
    public static class Utils
    {
        internal static readonly int[] ConvertMap = { 0x00,0x08,0x10,0x18,0x20,0x29,0x31,0x39,

                                                0x41,0x4A,0x52,0x5A,0x62,0x6A,0x73,0x7B,

                                                0x83,0x8B,0x94,0x9C,0xA4,0xAC,0xB4,0xBD,

                                                0xC5,0xCD,0xD5,0xDE,0xE6,0xEE,0xF6,0xFF };

        public static Color[,] Solve32X32Blocks(int width, int height, Color[,] pixelArrayOld)
        {
            var modWidth = width % 32;
            var timeWidth = (width - modWidth) / 32;

            var modHeight = height % 32;
            var timeHeight = (height - modHeight) / 32;

            var pixels = new Color[height, width];

            for (var timeH = 0; timeH < timeHeight + 1; timeH++)
            {
                var offsetX = 0;
                var offsetY = 0;

                var lineH = 32;

                if (timeH == timeHeight)
                    lineH = modHeight;

                for (var time = 0; time < timeWidth; time++)
                for (var positionY = 0; positionY < lineH; positionY++)
                for (var positionX = 0; positionX < 32; positionX++)
                {
                    offsetX = time * 32;
                    offsetY = timeH * 32;
                    pixels[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(pixelArrayOld, width);
                }

                for (var positionY = 0; positionY < lineH; positionY++)
                for (var positionX = 0; positionX < modWidth; positionX++)
                {
                    offsetX = timeWidth * 32;
                    offsetY = timeH * 32;
                    pixels[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(pixelArrayOld, width);
                }
            }
            _countGcfpxaH = 0;
            _countGcfpxa = 0;
            return pixels;
        }

        public static Color[,] Create32X32Blocks(int width, int height, Color[,] pixelArrayOld)
        {
            var modWidth = width % 32;
            var timeWidth = (width - modWidth) / 32;

            var modHeight = height % 32;
            var timeHeight = (height - modHeight) / 32;

            var pixels = new Color[height, width];

            for (var timeH = 0; timeH < timeHeight + 1; timeH++)
            {
                var offsetX = 0;
                var offsetY = 0;

                var lineH = 32;

                if (timeH == timeHeight)
                    lineH = modHeight;

                for (var time = 0; time < timeWidth; time++)
                    for (var positionY = 0; positionY < lineH; positionY++)
                        for (var positionX = 0; positionX < 32; positionX++)
                        {
                            offsetX = time * 32;
                            offsetY = timeH * 32;
                            pixels[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(pixelArrayOld, width);
                        }

                for (var positionY = 0; positionY < lineH; positionY++)
                    for (var positionX = 0; positionX < modWidth; positionX++)
                    {
                        offsetX = timeWidth * 32;
                        offsetY = timeH * 32;
                        pixels[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(pixelArrayOld, width);
                    }
            }
            _countGcfpxaH = 0;
            _countGcfpxa = 0;
            return pixels;
        }

        private static int _countGcfpxa;
        private static int _countGcfpxaH;

        public static Color GetColorFromPxArray(Color[,] pixelArrayOLD, int width)
        {
            //if (CountGCFPXA > width)
            if (_countGcfpxa > pixelArrayOLD.GetLength(1) - 1)
            {
                _countGcfpxa = 0;
                _countGcfpxaH += 1;
            }
            //Debug.Print($"test = {CountGCFPXA}-h{CountGCFPXA_H}");
            var goodColor = pixelArrayOLD[_countGcfpxaH, _countGcfpxa];
            //Color.PaleGreen;//pixelArrayOLD[CountGCFPXA_H,CountGCFPXA];
            _countGcfpxa += 1;
            return goodColor;
        }

        public static int Find32X32X(int upWidth, int fullWidth)
        {
            var xWidth = 0;
            xWidth = upWidth > fullWidth ? 0 : upWidth;
            return xWidth;
        }

        // Find the list's bounding box.
        private static Rectangle BoundingBox(IEnumerable<Point> points)
        {
            var x_query = from Point p in points select p.X;
            var xmin = x_query.Min();
            var xmax = x_query.Max();

            var y_query = from Point p in points select p.Y;
            var ymin = y_query.Min();
            var ymax = y_query.Max();

            return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
        }
        /*public static int Round(float value)
        {
            return (int)Math.Round(value);
        }*/
    }
}