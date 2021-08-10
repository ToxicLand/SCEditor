namespace LzhamWrapper.Enums
{
    public enum DecompressStatus
    {
        // LZHAM_DECOMP_STATUS_NOT_FINISHED indicates that the decompressor is flushing its public buffer to the caller's output buffer. 
        // There may be more bytes available to decompress on the next call, but there is no guarantee.
        NotFinished = 0,

        // LZHAM_DECOMP_STATUS_HAS_MORE_OUTPUT indicates that the decompressor is trying to flush its public buffer to the caller's output buffer, 
        // but the caller hasn't provided any space to copy this data to the caller's output buffer. Call the lzham_decompress() again with a non-empty sized output buffer.
        HasMoreOutput,

        // LZHAM_DECOMP_STATUS_NEEDS_MORE_INPUT indicates that the decompressor has consumed all input bytes, has not encountered an "end of stream" code, 
        // and the caller hasn't set no_more_input_bytes_flag to true, so it's expecting more input to proceed.
        NeedsMoreInput,

        // All the following enums always (and MUST) indicate failure/success.
        FirstSuccessOrFailureCode,

        // LZHAM_DECOMP_STATUS_SUCCESS indicates decompression has successfully completed.
        Success = FirstSuccessOrFailureCode,

        // The remaining status codes indicate a failure of some sort. Most failures are unrecoverable. 
        Failure,
        FailedInitializing = Failure,
        DestinationBufferTooSmall,
        ExpectedMoreRawBytes,
        BadCode,
        Adler32,
        BadRawBlock,
        BadCompBlockSyncCheck,
        BadZlibHeader,
        NeedSeedBytes,
        BadSeedBytes,
        BadSyncBlock,
        InvalidParameter
    }
}