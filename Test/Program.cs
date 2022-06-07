using Newtonsoft.Json.Linq;
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
            jsontest();

            //decompress(@"C:\Users\darks\Downloads\zstd-1.5.0\build\VS2010\bin\Win32_Debug\test.sc");
            //compress(@"C:\Users\darks\Downloads\zstd-1.5.0\build\VS2010\bin\Win32_Debug\test.sc.clone");
            //byte[] test = new byte[] {0xE2, 0x12, 0x00, 0x00 };
            //byte[] test2 = new byte[] { 0x00, 0x10, 0x97, 0x45 };

            //Console.WriteLine(BitConverter.ToInt32(test));
            //Console.WriteLine(BitConverter.ToInt32(test2));

            //int width = 100;
            //int height = 100;
            //int[,] inputArray = new int[height, width];
            //int i, j;
            //int value = 1;
            //for (i = 0; i < height; i++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        inputArray[i, x] = value;
            //        value++;
            //    }
            //}

            //int[,] createdArray = Create32x32Blocks(width, height, inputArray);
            //int[,] solvedArray = Solve32X32Blocks(width, height, createdArray);

            //if (solvedArray.Equals(inputArray))
            //{
            //    Console.WriteLine("WORKS!");
            //}

            Console.WriteLine("Done!");
        }

        static void jsontest()
        {
            string jsonFileData = File.ReadAllText(@"C:\Users\darks\AppData\Local\Temp\sceditor\chunks\output\data-1.json");
            JObject jsonParsedData = JObject.Parse(jsonFileData);

            JArray test = (JArray)((JToken)jsonParsedData["meta"])["related_multi_packs"];

            JArray framesData = new JArray();

            JObject item = ((JObject)((JArray)jsonParsedData["frames"])[0]);
            item.Add("textureCount", 1);

            framesData.Add(item);

            int chunkTextureId = int.Parse((string)item["textureCount"]);

            Console.Write("x");
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

        public static int[,] Create32x32Blocks(int width, int height, int[,] arrayOld)
        {
            int[,] arrayNew = new int[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int totIdx = x + y * width;

                    int yBlockId = totIdx / (32 * width);
                    totIdx -= yBlockId * (32 * width);
                    int xBlockId;
                    if (yBlockId == (height - 1) / 32)
                    {
                        xBlockId = totIdx / (32 * (height % 32 > 0 ? height % 32 : 32));
                        totIdx -= xBlockId * (32 * (height % 32 > 0 ? height % 32 : 32));
                    }
                    else
                    {
                        xBlockId = totIdx / (32 * 32);
                        totIdx -= xBlockId * (32 * 32);
                    }

                    int xOffset, yOffset;
                    if (xBlockId == (width - 1) / 32)
                    {
                        yOffset = totIdx / (width % 32 > 0 ? width % 32 : 32);
                        xOffset = totIdx % (width % 32 > 0 ? width % 32 : 32);
                    }
                    else
                    {
                        yOffset = totIdx / 32;
                        xOffset = totIdx % 32;
                    }

                    int mapPosX = xBlockId * 32 + xOffset;
                    int mapPosY = yBlockId * 32 + yOffset;
                    arrayNew[y, x] = arrayOld[mapPosY, mapPosX];
                }
            }

            return arrayNew;
        }

        public static int[,] Solve32X32Blocks(int width, int height, int[,] arrayOld)
        {
            var modWidth = width % 32;
            var timeWidth = (width - modWidth) / 32;

            var modHeight = height % 32;
            var timeHeight = (height - modHeight) / 32;

            int[,] arrayNew = new int[height, width];

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
                            arrayNew[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(arrayOld, width);
                        }

                for (var positionY = 0; positionY < lineH; positionY++)
                    for (var positionX = 0; positionX < modWidth; positionX++)
                    {
                        offsetX = timeWidth * 32;
                        offsetY = timeH * 32;
                        arrayNew[positionY + offsetY, positionX + offsetX] = GetColorFromPxArray(arrayOld, width);
                    }
            }

            return arrayNew;
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
