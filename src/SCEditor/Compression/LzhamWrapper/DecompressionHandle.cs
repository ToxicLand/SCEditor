namespace LzhamWrapper
{
    using System;
    using Microsoft.Win32.SafeHandles;

    public class DecompressionHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public DecompressionHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            this.Finish();
            return true;
        }

        public uint Finish()
        {
            this.handle = IntPtr.Zero;
            return LzhamInterop.DecompressDeinit(this.handle);
        }
    }
}