namespace LzhamWrapper.Enums
{
    public enum CompressStatus
    {
        NotFinished = 0,
        NeedsMoreInput,
        HasMoreOutput,

        // All the following enums must indicate failure/success.
        FirstSuccessOrFailureCode,
        Success = FirstSuccessOrFailureCode,
        Failure,
        Failed = Failure,
        FailedInitializing,
        InvalidParameter,
        OutputBufferTooSmall,
        Force = -1
    }
}