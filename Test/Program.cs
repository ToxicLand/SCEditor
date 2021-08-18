using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZstdNet;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //decompress(@"C:\Users\darks\Downloads\zstd-1.5.0\build\VS2010\bin\Win32_Debug\test.sc");
            compress(@"C:\Users\darks\Downloads\zstd-1.5.0\build\VS2010\bin\Win32_Debug\test.sc.clone");
            //byte[] test = new byte[] {0xE2, 0x12, 0x00, 0x00 };
            //byte[] test2 = new byte[] { 0x00, 0x10, 0x97, 0x45 };

            //Console.WriteLine(BitConverter.ToInt32(test));
            //Console.WriteLine(BitConverter.ToInt32(test2));

            //Create32X32Blocks();

            //Console.WriteLine("Done!");
        }


        static void decompress(string file)
        {
            Console.WriteLine("Start");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\darks\Downloads\zstd-1.5.0\build\VS2010\bin\x64_Debug\zstd.exe";
            startInfo.Arguments = $"--decompress \"{file}\" -o \"{file}.clone\"";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;

            try
            {
                using (Process proc = Process.Start(startInfo))
                {
                    proc.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }

            Console.WriteLine("Done");
        }

        static void compress(string file)
        {
            Console.WriteLine("Start");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\darks\Downloads\zstd-1.5.0\build\VS2010\bin\x64_Debug\zstd.exe";
            startInfo.Arguments = $"--compress \"{file}\" -o \"{file}.clone\" -15";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;

            try
            {
                using (Process proc = Process.Start(startInfo))
                {
                    proc.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }

            Console.WriteLine("Done");
        }

        public static void Create32X32Blocks()
        {
            int width = 100;
            int height = 100;
            int[,] input = new int[height, width];

            int value = 1;
            for (int i = 0; i < height; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    input[i, x] = value;
                    value++;
                }
            }

            // START

            var modWidth = width % 32;
            var timeWidth = (width - modWidth) / 32;

            var modHeight = height % 32;
            var timeHeight = (height - modHeight) / 32;

            int[,] output = new int[height, width];

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
                            output[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(input, width);
                        }

                for (var positionY = 0; positionY < lineH; positionY++)
                    for (var positionX = 0; positionX < modWidth; positionX++)
                    {
                        offsetX = timeWidth * 32;
                        offsetY = timeH * 32;
                        output[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(input, width);
                    }
            }
            _countGcfpxaH = 0;
            _countGcfpxa = 0;

            //for (int i = 0; i < height; i++)
            //{
            //    Console.WriteLine($"Array {i}:");
            //    for (int x = 0; x < width; x++)
            //    {
            //        if (x == 0 || x + 1 == width)
            //        {
            //            if (x !< width && x != 0)
            //            {
            //                Console.Write(", ");
            //            }
            //            Console.Write(output[i, x]);
            //        }
            //    }

            //    Console.WriteLine("\n");
            //}

            int[,] outputRev = new int[height, width];

            int totalRemainder = height % 32; // 4
            int totalBlocks = (height - totalRemainder) / 32; // 3


            //int originalH = 0;

            //for (int heightIndex = 0; heightIndex < height; heightIndex++)
            //{
            //    int originalW = 0;

            //    for (int blockIndex = 0; blockIndex < totalBlocks; blockIndex++)
            //    {
            //        int selectedBlock = blockIndex * 32;


            //        for (int widthPosition = 0; widthPosition < 32; widthPosition++)
            //        {
            //            outputRev[selectedBlock, widthPosition + ] = output[originalH, originalW];
            //            originalW++;
            //        }

            //    }

            //    originalH++;
            //}


            int originalH = 0;

            for (int heightIndex = 0; heightIndex < height; heightIndex++)
            {
                int originalW = 0;

                for (int blockIndex = 0; blockIndex < totalBlocks; blockIndex++)
                {
                    int selectedBlock = blockIndex * 32;


                    for (int widthPosition = 0; widthPosition < 32; widthPosition++)
                    {
                        //outputRev[selectedBlock, widthPosition + ] = output[originalH, originalW];
                        originalW++;
                    }

                }

                originalH++;
            }


            Console.WriteLine("Done");
        }

        private static int _countGcfpxa;
        private static int _countGcfpxaH;

        public static int GetColorFromPxArray(int[,] pixelArrayOLD, int width)
        {
            if (_countGcfpxa > pixelArrayOLD.GetLength(1) - 1)
            {
                _countGcfpxa = 0;
                _countGcfpxaH += 1;
            }

            int goodColor = pixelArrayOLD[_countGcfpxaH, _countGcfpxa];

            _countGcfpxa += 1;
            
            return goodColor;
        }
    }
}
