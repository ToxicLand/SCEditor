namespace LzhamWrapper.Enums
{
    public enum TableUpdateRate
    {
        InsanelySlow = 1, // 1=insanely slow decompression, here for reference, use 2!
        Slowest = 2,
        Default = 8,
        Fastest = 20
    }
}