namespace LzhamWrapper.Enums
{
    using System;

    [Flags]
    public enum DecompressionFlag
    {
        OutputUnbuffered = 1,
        ComputeAdler32 = 2,
        ReadZlibStream = 4
    }
}