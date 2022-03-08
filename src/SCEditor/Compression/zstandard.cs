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
                    input.Position = 5;

                    int version = input.ReadByte();
                    long endOffset = -1;
                    MemoryStream v4Stream = null;

                    if (version == 4)
                    {
                        endOffset = Seek(input, "START", Encoding.UTF8);

                        if (endOffset == -1)
                            throw new Exception("SC Version 4 but could not find START of exports");

                        int v4BufferSize = (int)(endOffset - 30);
                        input.Position = 30;

                        v4Stream = new MemoryStream(v4BufferSize);

                        while (input.Position < endOffset)
                        {
                            v4Stream.WriteByte((byte)input.ReadByte());
                        }

                        int test = v4Stream.GetBuffer()[v4Stream.Length - 1];
                        v4Stream.Position = 0;
                    }
                    else
                    {
                        input.Position = 26;
                    }

                    using (var decompressionStream = (version != 4 ? new DecompressionStream(input) : new DecompressionStream(v4Stream)))
                    {
                        decompressionStream.CopyTo(output);
                        decompressionStream.Close();
                        decompressionStream.Dispose();
                    }
                    input.Close();
                    input.Dispose();

                    if (version == 4)
                    {
                        v4Stream.Close();
                        v4Stream.Dispose();
                    }
                }
                File.WriteAllBytes(file, output.ToArray());
                output.Close();
                output.Dispose();
            }
            
        }

        public static long Seek(Stream stream, string str, Encoding encoding)
        {
            var search = encoding.GetBytes(str);
            return Seek(stream, search);
        }

        public static long Seek(Stream stream, byte[] search)
        {
            int bufferSize = 1024;
            if (bufferSize < search.Length * 2) bufferSize = search.Length * 2;

            var buffer = new byte[bufferSize];
            var size = bufferSize;
            var offset = 0;
            var position = stream.Position;

            while (true)
            {
                var r = stream.Read(buffer, offset, size);

                // when no bytes are read -- the string could not be found
                if (r <= 0) return -1;

                // when less then size bytes are read, we need to slice
                // the buffer to prevent reading of "previous" bytes
                ReadOnlySpan<byte> ro = buffer;
                if (r < size)
                {
                    ro = ro.Slice(0, offset + size);
                }

                // check if we can find our search bytes in the buffer
                var i = ro.IndexOf(search);
                if (i > -1) return position + i;

                // when less then size was read, we are done and found nothing
                if (r < size) return -1;

                // we still have bytes to read, so copy the last search
                // length to the beginning of the buffer. It might contain
                // a part of the bytes we need to search for

                offset = search.Length;
                size = bufferSize - offset;
                Array.Copy(buffer, buffer.Length - offset, buffer, 0, offset);
                position += bufferSize - offset;
            }
        }
    }
}
