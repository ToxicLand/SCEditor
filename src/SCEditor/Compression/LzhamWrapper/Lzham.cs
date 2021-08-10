namespace LzhamWrapper
{
    using LzhamWrapper.Enums;

    public static class Lzham
    {
        public static uint GetVersion()
        {
            return LzhamInterop.GetVersion();
        }

        public static DecompressStatus DecompressMemory(DecompressionParameters parameters, byte[] inBuf, int inBufSize, int inBufOffset, byte[] outBuf, ref int outBufSize,
            int outBufOffset, ref uint adler32)
        {
            return LzhamInterop.DecompressMemory(parameters, outBuf, ref outBufSize, outBufOffset, inBuf, inBufSize, inBufOffset, ref adler32);
        }

        public static CompressStatus CompressMemory(CompressionParameters parameters, byte[] inBuf, int inBufSize, int inBufOffset, byte[] outBuf, ref int outBufSize,
            int outBufOffset, ref uint adler32)
        {
            return LzhamInterop.CompressMemory(parameters, outBuf, ref outBufSize, outBufOffset, inBuf, inBufSize, inBufOffset, ref adler32);
        }
    }
}