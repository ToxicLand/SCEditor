using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SCEditor.ScOld.ImageDecoders;

public static partial class AstcDecoder
{
    // bool decompress_astc_to_rgba(const uint8_t* astc_data, size_t astc_data_size,
    //      size_t width, size_t height, int block_width, int block_height,
    //      uint8_t* out_buffer, size_t out_buffer_size,
    //      size_t out_buffer_stride)
    [LibraryImport("libs/astc-codec.dll", EntryPoint = "decompress_astc_to_rgba")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static unsafe partial bool Decompress(
        [MarshalAs(UnmanagedType.LPArray)] byte[] astcData,
        nuint astcDataSize,
        nuint width,
        nuint height,
        int blockWidth,
        int blockHeight,
        [MarshalAs(UnmanagedType.LPArray)] byte[] outBuffer,
        UIntPtr outBufferSize,
        nuint outBufferStride
    );
}