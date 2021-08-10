namespace LzhamWrapper.Enums
{
    using System;

    [Flags]
    public enum CompressionFlag
    {
        ExtremeParsing = 2,

        // Improves ratio by allowing the compressor's parse graph to grow "higher" (up to 4 parent nodes per output node), but is much slower.
        DeterministicParsing = 4,

        // Guarantees that the compressed output will always be the same given the same input and parameters (no variation between runs due to kernel threading scheduling).
        // If enabled, the compressor is free to use any optimizations which could lower the decompression rate (such
        // as adaptively resetting the Huffman table update rate to maximum frequency, which is costly for the decompressor).
        TradeIffDecompressionForCompressionRatio = 16,
        WriteZlibStream = 32
    }
}