using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ZstdSharp;

namespace SCEditor.Compression
{
    public class zstandard
    {
        internal static void Compress(string file, string outputLocation)
        {
            File.Copy(file, file += ".clone");
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(File.ReadAllBytes(file));
            }


            using (var output = new FileStream(outputLocation, FileMode.Create, FileAccess.Write))
            {
                output.Write(Encoding.UTF8.GetBytes("SC"), 0, 2);
                output.Write(BitConverter.GetBytes(3).Reverse().ToArray(), 0, 4);
                output.Write(BitConverter.GetBytes(hash.Length).Reverse().ToArray(), 0, 4);
                output.Write(hash, 0, hash.Length);

                using var compressor = new Compressor(Compressor.MaxCompressionLevel);

                output.Write(compressor.Wrap(File.ReadAllBytes(file)));

                output.Flush();
                output.Dispose();
            }

            File.Delete(file);
        }

        internal static void decompress(string file)
        {

            using (MemoryStream output = new MemoryStream())
            {
                using (FileStream input = new FileStream(file, FileMode.Open))
                {
                    input.Position = 26;
                    using (var decompressionStream = new DecompressionStream(input))
                    {
                        decompressionStream.CopyTo(output);
                        decompressionStream.Close();
                        decompressionStream.Dispose();
                    }
                    input.Close();
                    input.Dispose();
                }
                File.WriteAllBytes(file, output.ToArray());
                output.Close();
                output.Dispose();
            }
            
        }
    }
}
