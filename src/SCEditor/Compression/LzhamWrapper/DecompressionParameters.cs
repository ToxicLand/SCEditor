namespace LzhamWrapper
{
    using LzhamWrapper.Enums;

    public class DecompressionParameters
    {
        public uint DictionarySize { get; set; }
        public TableUpdateRate UpdateRate { get; set; }
        public DecompressionFlag Flags { get; set; }
        public byte[] SeedBytes { get; set; }
        public uint MaxUpdateInterval { get; set; }
        public uint UpdateIntervalSlowRate { get; set; }
    }
}