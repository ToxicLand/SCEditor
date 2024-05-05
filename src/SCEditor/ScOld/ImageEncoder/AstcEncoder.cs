using System;
using System.Diagnostics;
using System.IO;

namespace SCEditor.ScOld.ImageEncoder
{
    public static class AstcEncoder
    {
        private static readonly string _astcencPath = "libs/astcenc-avx2.exe";

        public static byte[] Encode(Span<byte> colorBytes, string modes = "-cH", string options = "6x6 -medium")
        {
            // Create temporary input and output files
            var tempInputFile = Path.GetTempFileName();
            var tempOutputFile = Path.GetTempFileName();

            try
            {
                // Write colorBytes to tempInputFile
                File.WriteAllBytes(tempInputFile, colorBytes.ToArray());

                // Use Astcenc to encode the file
                EncodeFile(tempInputFile, tempOutputFile, modes, options);

                // Read the result back into a byte array
                var result = File.ReadAllBytes(tempOutputFile);

                return result;
            }
            finally
            {
                // Clean up temporary files
                File.Delete(tempInputFile);
                File.Delete(tempOutputFile);
            }
        }

        private static void EncodeFile(string inputPath, string outputPath, string modes, string options)
        {
            var process = new Process();

            process.StartInfo.FileName = _astcencPath;
            process.StartInfo.Arguments = $"{modes} \"{inputPath}\" \"{outputPath}\" {options}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Astcenc encoding failed: {error}");
            }

            Console.WriteLine(output);
        }
    }
}
