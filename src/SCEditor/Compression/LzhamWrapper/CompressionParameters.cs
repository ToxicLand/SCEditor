namespace LzhamWrapper
{
    using LzhamWrapper.Enums;

    public class CompressionParameters
    {
        public uint DictionarySize { get; set; }
        public CompressionLevel Level { get; set; }
        public TableUpdateRate UpdateRate { get; set; }
        public int HelperThreads { get; set; }
        public CompressionFlag Flags { get; set; }
        public byte[] SeedBytes { get; set; }
        public uint MaxUpdateInterval { get; set; }
        public uint UpdateIntervalSlowRate { get; set; }
    }
}