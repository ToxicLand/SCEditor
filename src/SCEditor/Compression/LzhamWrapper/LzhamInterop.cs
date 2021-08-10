namespace LzhamWrapper
{
    using System;
    using System.Runtime.InteropServices;
    using LzhamWrapper.Enums;

    internal class LzhamInterop
    {
        private const string LzhamDll = "lzham.dll";

        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint lzham_get_version();

        // Initializes a compressor. Returns a pointer to the compressor's public state, or NULL on failure.
        // pParams cannot be NULL. Be sure to initialize the pParams->m_struct_size member to sizeof(lzham_compress_params) (along with the other members to reasonable values) before calling this function.
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe CompressionHandle lzham_compress_init(byte* parameters);

        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern CompressionHandle lzham_compress_reinit(CompressionHandle state);

        // Compresses an arbitrarily sized block of data, writing as much available compressed data as possible to the output buffer. 
        // This method may be called as many times as needed, but for best perf. try not to call it with tiny buffers.
        // pState - Pointer to public compression state, created by lzham_compress_init.
        // pIn_buf, pIn_buf_size - Pointer to input data buffer, and pointer to a size_t containing the number of bytes available in this buffer. 
        //                         On return, *pIn_buf_size will be set to the number of bytes read from the buffer.
        // pOut_buf, pOut_buf_size - Pointer to the output data buffer, and a pointer to a size_t containing the max number of bytes that can be written to this buffer.
        //                         On return, *pOut_buf_size will be set to the number of bytes written to this buffer.
        // no_more_input_bytes_flag - Set to true to indicate that no more input bytes are available to compress (EOF). Once you call this function with this param set to true, it must stay set to true in all future calls.
        //
        // Normal return status codes:
        //    LZHAM_COMP_STATUS_NOT_FINISHED - Compression can continue, but the compressor needs more input, or it needs more room in the output buffer.
        //    LZHAM_COMP_STATUS_NEEDS_MORE_INPUT - Compression can contintue, but the compressor has no more output, and has no input but we're not at EOF. Supply more input to continue.
        // Success/failure return status codes:
        //    LZHAM_COMP_STATUS_SUCCESS - Compression has completed successfully.
        //    LZHAM_COMP_STATUS_FAILED, LZHAM_COMP_STATUS_FAILED_INITIALIZING, LZHAM_COMP_STATUS_INVALID_PARAMETER - Something went wrong.
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int lzham_compress(CompressionHandle state, byte* inBuf, ref IntPtr inBufSize, byte* outBuf,
            ref IntPtr outBufSize, int noMoreInputBytesFlag);

        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int lzham_compress2(CompressionHandle state, byte* inBuf, ref IntPtr inBufSize, byte* outBuf,
            ref IntPtr outBufSize, Flush flushType);

        // Single function call compression interface.
        // Same return codes as lzham_compress, except this function can also return LZHAM_COMP_STATUS_OUTPUT_BUF_TOO_SMALL.
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int lzham_compress_memory(byte* parameters, byte* dstBuffer, ref IntPtr dstLength, byte* srcBuffer, int srcLenght, ref uint adler32);

        // Deinitializes a compressor, releasing all allocated memory.
        // returns adler32 of source data (valid only on success).
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int lzham_compress_deinit(IntPtr state);

        // Initializes a decompressor.
        // pParams cannot be NULL. Be sure to initialize the pParams->m_struct_size member to sizeof(lzham_decompress_params) (along with the other members to reasonable values) before calling this function.
        // Note: With large dictionaries this function could take a while (due to memory allocation). To serially decompress multiple streams, it's faster to init a compressor once and 
        // reuse it using by calling lzham_decompress_reinit().
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe DecompressionHandle lzham_decompress_init(byte* parameters);

        // Quickly re-initializes the decompressor to its initial state given an already allocated/initialized state (doesn't do any memory alloc unless necessary).
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe DecompressionHandle lzham_decompress_reinit(DecompressionHandle state, byte* parameters);

        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]

        // Decompresses an arbitrarily sized block of compressed data, writing as much available decompressed data as possible to the output buffer. 
        // This method is implemented as a coroutine so it may be called as many times as needed. However, for best perf. try not to call it with tiny buffers.
        // pState - Pointer to public decompression state, originally created by lzham_decompress_init.
        // pIn_buf, pIn_buf_size - Pointer to input data buffer, and pointer to a size_t containing the number of bytes available in this buffer. 
        //                         On return, *pIn_buf_size will be set to the number of bytes read from the buffer.
        // pOut_buf, pOut_buf_size - Pointer to the output data buffer, and a pointer to a size_t containing the max number of bytes that can be written to this buffer.
        //                         On return, *pOut_buf_size will be set to the number of bytes written to this buffer.
        // no_more_input_bytes_flag - Set to true to indicate that no more input bytes are available to compress (EOF). Once you call this function with this param set to true, it must stay set to true in all future calls.
        // Notes:
        // In unbuffered mode, the output buffer MUST be large enough to hold the entire decompressed stream. Otherwise, you'll receive the
        //  LZHAM_DECOMP_STATUS_FAILED_DEST_BUF_TOO_SMALL error (which is currently unrecoverable during unbuffered decompression).
        // In buffered mode, if the output buffer's size is 0 bytes, the caller is indicating that no more output bytes are expected from the
        //  decompressor. In this case, if the decompressor actually has more bytes you'll receive the LZHAM_DECOMP_STATUS_HAS_MORE_OUTPUT
        //  error (which is recoverable in the buffered case - just call lzham_decompress() again with a non-zero size output buffer).
        private static extern unsafe int lzham_decompress(DecompressionHandle state, byte* inBuf, ref IntPtr inBufSize, byte* outBuf,
            ref IntPtr outBufSize, int noMoreInputBytesFlag);

        // Single function call interface.
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int lzham_decompress_memory(byte* parameters, byte* dstBuffer, ref IntPtr dstLength, byte* srcBuffer, int srcLenght, ref uint adler32);

        // Deinitializes a decompressor.
        // returns adler32 of decompressed data if compute_adler32 was true, otherwise it returns the adler32 from the compressed stream.
        [DllImport(LzhamDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern int lzham_decompress_deinit(IntPtr state);


        // Compression parameters struct.
        // IMPORTANT: The values of m_dict_size_log2, m_table_update_rate, m_table_max_update_interval, and m_table_update_interval_slow_rate MUST
        // match during compression and decompression. The codec does not verify these values for you, if you don't use the same settings during
        // decompression it will fail (usually with a LZHAM_DECOMP_STATUS_FAILED_BAD_CODE error).
        // The seed buffer's contents and size must match the seed buffer used during decompression.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct CompressionParametersInternal
        {
            public uint m_struct_size; // set to sizeof(lzham_compress_params)

            public uint m_dict_size_log2;
            // set to the log2(dictionary_size), must range between [LZHAM_MIN_DICT_SIZE_LOG2, LZHAM_MAX_DICT_SIZE_LOG2_X86] for x86 LZHAM_MAX_DICT_SIZE_LOG2_X64 for x64

            public CompressionLevel m_level; // set to LZHAM_COMP_LEVEL_FASTEST, etc.

            public TableUpdateRate m_table_update_rate;
            // Controls tradeoff between ratio and decompression throughput. 0=default, or [1,LZHAM_MAX_TABLE_UPDATE_RATE], higher=faster but lower ratio.

            public int m_max_helper_threads;
            // max # of additional "helper" threads to create, must range between [-1,LZHAM_MAX_HELPER_THREADS], where -1=max practical

            public CompressionFlag m_compress_flags; // optional compression flags (see lzham_compress_flags enum)

            public uint m_num_seed_bytes;
            // for delta compression (optional) - number of seed bytes pointed to by m_pSeed_bytes

            public byte* m_pSeed_bytes;
            // for delta compression (optional) - pointer to seed bytes buffer, must be at least m_num_seed_bytes long

            // Advanced settings - set to 0 if you don't care.
            // m_table_max_update_interval/m_table_update_interval_slow_rate override m_table_update_rate and allow finer control over the table update settings.
            // If either are non-zero they will override whatever m_table_update_rate is set to. Just leave them 0 unless you are specifically customizing them for your data.

            // def=0, typical range 12-128 (LZHAM_DEFAULT_TABLE_UPDATE_RATE=64), controls the max interval between table updates, higher=longer max interval (faster decode/lower ratio). Was 16 in prev. releases.
            public uint m_table_max_update_interval;

            // def=0, 32 or higher (LZHAM_DEFAULT_TABLE_UPDATE_RATE=64), scaled by 32, controls the slowing of the update update freq, higher=more rapid slowing (faster decode/lower ratio). Was 40 in prev. releases.
            public uint m_table_update_interval_slow_rate;
        }


        // Decompression parameters structure.
        // Notes: 
        // m_dict_size_log2 MUST match the value used during compression!
        // If m_num_seed_bytes != 0, LZHAM_DECOMP_FLAG_OUTPUT_UNBUFFERED must not be set (i.e. static "seed" dictionaries are not compatible with unbuffered decompression).
        // The seed buffer's contents and size must match the seed buffer used during compression.
        // m_table_update_rate (or m_table_max_update_interval/m_table_update_interval_slow_rate) MUST match the values used for compression!
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct DecompressionParametersInternal
        {
            public uint m_struct_size; // set to sizeof(lzham_decompress_params)

            public uint m_dict_size_log2;
            // set to the log2(dictionary_size), must range between [LZHAM_MIN_DICT_SIZE_LOG2, LZHAM_MAX_DICT_SIZE_LOG2_X86] for x86 LZHAM_MAX_DICT_SIZE_LOG2_X64 for x64

            public TableUpdateRate m_table_update_rate;
            // Controls tradeoff between ratio and decompression throughput. 0=default, or [1,LZHAM_MAX_TABLE_UPDATE_RATE], higher=faster but lower ratio.

            public DecompressionFlag m_decompress_flags; // optional decompression flags (see lzham_decompress_flags enum)

            public uint m_num_seed_bytes;
            // for delta compression (optional) - number of seed bytes pointed to by m_pSeed_bytes

            public byte* m_pSeed_bytes;
            // for delta compression (optional) - pointer to seed bytes buffer, must be at least m_num_seed_bytes long

            // Advanced settings - set to 0 if you don't care.
            // m_table_max_update_interval/m_table_update_interval_slow_rate override m_table_update_rate and allow finer control over the table update settings.
            // If either are non-zero they will override whatever m_table_update_rate is set to. Just leave them 0 unless you are specifically customizing them for your data.

            // def=0, typical range 12-128 (LZHAM_DEFAULT_TABLE_UPDATE_RATE=64), controls the max interval between table updates, higher=longer max interval (faster decode/lower ratio). Was 16 in prev. releases.
            public uint m_table_max_update_interval;

            // def=0, 32 or higher (LZHAM_DEFAULT_TABLE_UPDATE_RATE=64), scaled by 32, controls the slowing of the update update freq, higher=more rapid slowing (faster decode/lower ratio). Was 40 in prev. releases.
            public uint m_table_update_interval_slow_rate;
        }


        public static unsafe CompressionHandle CompressInit(CompressionParameters parameters)
        {
            CompressionParametersInternal p;
            p.m_struct_size = (uint) sizeof(CompressionParametersInternal);
            p.m_compress_flags = parameters.Flags;
            p.m_dict_size_log2 = parameters.DictionarySize;
            p.m_level = parameters.Level;
            p.m_max_helper_threads = parameters.HelperThreads;
            p.m_table_max_update_interval = parameters.MaxUpdateInterval;
            p.m_table_update_interval_slow_rate = parameters.UpdateIntervalSlowRate;
            p.m_table_update_rate = parameters.UpdateRate;
            if (parameters.SeedBytes != null)
            {
                p.m_num_seed_bytes = (uint) parameters.SeedBytes.Length;
            }

            fixed (byte* seedBytes = parameters.SeedBytes)
            {
                p.m_pSeed_bytes = seedBytes;
                byte* pBytes = (byte*) &p;
                return lzham_compress_init(pBytes);
            }
        }

        public static CompressionHandle CompressReinit(CompressionHandle state)
        {
            return lzham_compress_reinit(state);
        }

        public static unsafe CompressStatus Compress(CompressionHandle state, byte[] inBuf, ref int inBufSize, int inBufOffset, byte[] outBuf, ref int outBufSize, int outBufOffset,
            bool noMoreInputBytesFlag)
        {
            if (inBufOffset + inBufSize > inBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(inBuf));
            }

            if (outBufOffset + outBufSize > outBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(outBuf));
            }

            fixed (byte* inBytes = inBuf)
            fixed (byte* outBytes = outBuf)
            {
                IntPtr inSize = new IntPtr(inBufSize);
                IntPtr outSize = new IntPtr(outBufSize);
                CompressStatus result =
                    (CompressStatus) lzham_compress(state, inBytes + inBufOffset, ref inSize, outBytes + outBufOffset, ref outSize, noMoreInputBytesFlag ? 1 : 0);
                inBufSize = inSize.ToInt32();
                outBufSize = outSize.ToInt32();
                return result;
            }
        }

        public static unsafe CompressStatus Compress2(CompressionHandle state, byte[] inBuf, ref int inBufSize, int inBufOffset, byte[] outBuf, ref int outBufSize,
            int outBufOffset, Flush flushType)
        {
            if (inBufOffset + inBufSize > inBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(inBuf));
            }

            if (outBufOffset + outBufSize > outBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(outBuf));
            }

            fixed (byte* inBytes = inBuf)
            fixed (byte* outBytes = outBuf)
            {
                IntPtr inSize = new IntPtr(inBufSize);
                IntPtr outSize = new IntPtr(outBufSize);
                CompressStatus result = (CompressStatus) lzham_compress2(state, inBytes + inBufOffset, ref inSize, outBytes + outBufOffset, ref outSize, flushType);
                inBufSize = inSize.ToInt32();
                outBufSize = outSize.ToInt32();
                return result;
            }
        }

        public static unsafe CompressStatus CompressMemory(CompressionParameters parameters, byte[] outBuf, ref int outBufSize, int outBufOffset, byte[] inBuf, int inBufSize,
            int inBufOffset, ref uint adler32)
        {
            if (outBufOffset + outBufSize > outBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(outBuf));
            }

            if (inBufOffset + inBufSize > inBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(inBuf));
            }

            CompressionParametersInternal p;
            p.m_struct_size = (uint) sizeof(CompressionParametersInternal);
            p.m_compress_flags = parameters.Flags;
            p.m_dict_size_log2 = parameters.DictionarySize;
            p.m_level = parameters.Level;
            p.m_max_helper_threads = parameters.HelperThreads;
            p.m_table_max_update_interval = parameters.MaxUpdateInterval;
            p.m_table_update_interval_slow_rate = parameters.UpdateIntervalSlowRate;
            p.m_table_update_rate = parameters.UpdateRate;
            if (parameters.SeedBytes != null)
            {
                p.m_num_seed_bytes = (uint) parameters.SeedBytes.Length;
            }

            p.m_struct_size = (uint) sizeof(CompressionParametersInternal);
            fixed (byte* seedBytes = parameters.SeedBytes)
            fixed (byte* outBytes = outBuf)
            fixed (byte* inBytes = inBuf)
            {
                p.m_pSeed_bytes = seedBytes;
                byte* pBytes = (byte*) &p;
                IntPtr outSize = new IntPtr(outBufSize);
                CompressStatus result = (CompressStatus) lzham_compress_memory(pBytes, outBytes + outBufOffset, ref outSize, inBytes + inBufOffset, inBufSize, ref adler32);
                outBufSize = outSize.ToInt32();
                return result;
            }
        }

        public static uint CompressDeinit(IntPtr state)
        {
            return (uint) lzham_compress_deinit(state);
        }

        public static unsafe DecompressionHandle DecompressInit(DecompressionParameters parameters)
        {
            DecompressionParametersInternal p;
            p.m_struct_size = (uint) sizeof(DecompressionParametersInternal);
            p.m_decompress_flags = parameters.Flags;
            p.m_dict_size_log2 = parameters.DictionarySize;
            p.m_table_max_update_interval = parameters.MaxUpdateInterval;
            p.m_table_update_interval_slow_rate = parameters.UpdateIntervalSlowRate;
            p.m_table_update_rate = parameters.UpdateRate;
            if (parameters.SeedBytes != null)
            {
                p.m_num_seed_bytes = (uint) parameters.SeedBytes.Length;
            }

            p.m_struct_size = (uint) sizeof(DecompressionParametersInternal);
            fixed (byte* seedBytes = parameters.SeedBytes)
            {
                p.m_pSeed_bytes = seedBytes;
                byte* pBytes = (byte*) &p;
                return lzham_decompress_init(pBytes);
            }
        }

        public static unsafe DecompressionHandle DecompressReinit(DecompressionHandle state, DecompressionParameters parameters)
        {
            DecompressionParametersInternal p;
            p.m_struct_size = (uint) sizeof(DecompressionParametersInternal);
            p.m_decompress_flags = parameters.Flags;
            p.m_dict_size_log2 = parameters.DictionarySize;
            p.m_table_max_update_interval = parameters.MaxUpdateInterval;
            p.m_table_update_interval_slow_rate = parameters.UpdateIntervalSlowRate;
            p.m_table_update_rate = parameters.UpdateRate;
            if (parameters.SeedBytes != null)
            {
                p.m_num_seed_bytes = (uint) parameters.SeedBytes.Length;
            }

            p.m_struct_size = (uint) sizeof(DecompressionParametersInternal);
            fixed (byte* seedBytes = parameters.SeedBytes)
            {
                p.m_pSeed_bytes = seedBytes;
                byte* pBytes = (byte*) &p;
                return lzham_decompress_reinit(state, pBytes);
            }
        }

        public static unsafe DecompressStatus Decompress(DecompressionHandle state, byte[] inBuf, ref int inBufSize, int inBufOffset, byte[] outBuf, ref int outBufSize,
            int outBufOffset, bool noMoreInputBytesFlag)
        {
            if (inBufOffset + inBufSize > inBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(inBuf));
            }

            if (outBufOffset + outBufSize > outBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(outBuf));
            }

            fixed (byte* inBytes = inBuf)
            fixed (byte* outBytes = outBuf)
            {
                IntPtr inSize = new IntPtr(inBufSize);
                IntPtr outSize = new IntPtr(outBufSize);
                DecompressStatus result =
                    (DecompressStatus) lzham_decompress(state, inBytes + inBufOffset, ref inSize, outBytes + outBufOffset, ref outSize, noMoreInputBytesFlag ? 1 : 0);
                inBufSize = inSize.ToInt32();
                outBufSize = outSize.ToInt32();
                return result;
            }
        }

        public static unsafe DecompressStatus Decompress(DecompressionHandle state, byte[] inBuf, ref int inBufSize, int inBufOffset, byte* outBuf, ref int outBufSize,
            bool noMoreInputBytesFlag)
        {
            if (inBufOffset + inBufSize > inBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(inBuf));
            }

            fixed (byte* inBytes = inBuf)
            {
                IntPtr inSize = new IntPtr(inBufSize);
                IntPtr outSize = new IntPtr(outBufSize);
                DecompressStatus result = (DecompressStatus) lzham_decompress(state, inBytes + inBufOffset, ref inSize, outBuf, ref outSize, noMoreInputBytesFlag ? 1 : 0);
                inBufSize = inSize.ToInt32();
                outBufSize = outSize.ToInt32();
                return result;
            }
        }

        public static unsafe DecompressStatus DecompressMemory(DecompressionParameters parameters, byte[] outBuf, ref int outBufSize, int outBufOffset, byte[] inBuf, int inBufSize,
            int inBufOffset, ref uint adler32)
        {
            if (outBufOffset + outBufSize > outBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(outBuf));
            }

            if (inBufOffset + inBufSize > inBuf.Length)
            {
                throw new ArgumentException("Offset plus count is larger than the length of array", nameof(inBuf));
            }

            DecompressionParametersInternal p;
            p.m_struct_size = (uint) sizeof(DecompressionParametersInternal);
            p.m_decompress_flags = parameters.Flags;
            p.m_dict_size_log2 = parameters.DictionarySize;
            p.m_table_max_update_interval = parameters.MaxUpdateInterval;
            p.m_table_update_interval_slow_rate = parameters.UpdateIntervalSlowRate;
            p.m_table_update_rate = parameters.UpdateRate;
            if (parameters.SeedBytes != null)
            {
                p.m_num_seed_bytes = (uint) parameters.SeedBytes.Length;
            }

            p.m_struct_size = (uint) sizeof(DecompressionParametersInternal);
            fixed (byte* seedBytes = parameters.SeedBytes)
            fixed (byte* outBytes = outBuf)
            fixed (byte* inBytes = inBuf)
            {
                p.m_pSeed_bytes = seedBytes;
                byte* pBytes = (byte*) &p;
                IntPtr outSize = new IntPtr(outBufSize);
                DecompressStatus result = (DecompressStatus) lzham_decompress_memory(pBytes, outBytes + outBufOffset, ref outSize, inBytes + inBufOffset, inBufSize, ref adler32);
                outBufSize = outSize.ToInt32();
                return result;
            }
        }

        public static uint DecompressDeinit(IntPtr state)
        {
            return (uint) lzham_decompress_deinit(state);
        }

        public static uint GetVersion()
        {
            return lzham_get_version();
        }
    }
}