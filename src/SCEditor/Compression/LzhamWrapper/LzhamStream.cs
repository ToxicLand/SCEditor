namespace LzhamWrapper
{
    using System;
    using System.IO;
    using LzhamWrapper.Enums;

    public class LzhamStream : Stream
    {
        internal const int DefaultBufferSize = 8192;
        private Stream _stream;
        private readonly bool _leaveOpen;
        private readonly byte[] _buffer;
        private readonly CompressionHandle _compressionHandle;
        private readonly DecompressionHandle _decompressionHandle;

        private bool _readFinishing;
        private int _inputAvailable;
        private int _readOffset;


        public LzhamStream(Stream stream, CompressionParameters mode) : this(stream, mode, false)
        {
        }

        public LzhamStream(Stream stream, CompressionParameters mode, bool leaveOpen)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException("The base stream is not writeable", nameof(stream));
            this._stream = stream;
            this._leaveOpen = leaveOpen;
            this._buffer = new byte[DefaultBufferSize];
            this._compressionHandle = LzhamInterop.CompressInit(mode);
            if (this._compressionHandle.IsInvalid)
            {
                throw new ApplicationException("Could not initialize compression stream with specified parameters");
            }
        }

        public LzhamStream(Stream stream, DecompressionParameters mode) : this(stream, mode, false)
        {
        }

        public LzhamStream(Stream stream, DecompressionParameters mode, bool leaveOpen)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("The base stream is not readable", nameof(stream));
            this._stream = stream;
            this._leaveOpen = leaveOpen;
            this._buffer = new byte[DefaultBufferSize];
            this._decompressionHandle = LzhamInterop.DecompressInit(mode);
            if (this._decompressionHandle.IsInvalid)
            {
                throw new ApplicationException("Could not initialize Decompression stream with specified parameters");
            }

            this.ReadInput();
        }


        private void EnsureNotDisposed()
        {
            if (this._stream == null)
                throw new ObjectDisposedException(null, "Can not access a closed Stream");
        }

        private void EnsureDecompressionMode()
        {
            if (this._decompressionHandle == null || this._decompressionHandle.IsInvalid)
                throw new InvalidOperationException("Reading from the compression stream is not supported");
        }

        private void EnsureCompressionMode()
        {
            if (this._compressionHandle == null || this._compressionHandle.IsInvalid)
                throw new InvalidOperationException("Writing to the compression stream is not supported");
        }

        public override void Flush()
        {
            this.EnsureNotDisposed();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("This operation is not supported");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("This operation is not supported");
        }

        public override unsafe int Read(Span<byte> buffer)
        {
            this.EnsureDecompressionMode();
            this.EnsureNotDisposed();

            int count = buffer.Length;
            int totalWritten = 0;

            fixed (byte* pBuffer = buffer)
            {
                int writeOffset = 0;

                do
                {
                    DecompressStatus decompressionStatus;
                    do
                    {
                        int remaining = this._inputAvailable;
                        int outSize = count - totalWritten;
                        decompressionStatus = LzhamInterop.Decompress(this._decompressionHandle, this._buffer, ref remaining, this._readOffset, pBuffer + writeOffset, ref outSize,
                            this._readFinishing);
                        if (!(decompressionStatus == DecompressStatus.HasMoreOutput
                              || decompressionStatus == DecompressStatus.NeedsMoreInput
                              || decompressionStatus == DecompressStatus.NotFinished
                              || decompressionStatus == DecompressStatus.Success)
                        )
                        {
                            throw new InvalidOperationException($"Unexpected Decompress Status Code {decompressionStatus}");
                        }

                        int written = outSize;
                        int read = remaining;
                        totalWritten += written;
                        writeOffset += written;
                        this._inputAvailable = this._inputAvailable - read;
                        this._readOffset += read;
                    } while (decompressionStatus == DecompressStatus.HasMoreOutput && totalWritten < count);

                    if (this._inputAvailable == 0)
                    {
                        this.ReadInput();
                    }
                } while (totalWritten < count && this._inputAvailable > 0);
            }

            return totalWritten;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.EnsureDecompressionMode();
            this.ValidateParameters(buffer, offset, count);
            this.EnsureNotDisposed();
            int totalWritten = 0;
            int writeOffset = offset;

            do
            {
                DecompressStatus decompressionStatus;
                do
                {
                    int remaining = this._inputAvailable;
                    int outSize = count - totalWritten;
                    decompressionStatus = LzhamInterop.Decompress(this._decompressionHandle, this._buffer, ref remaining, this._readOffset, buffer, ref outSize, writeOffset,
                        this._readFinishing);
                    if (!(decompressionStatus == DecompressStatus.HasMoreOutput
                          || decompressionStatus == DecompressStatus.NeedsMoreInput
                          || decompressionStatus == DecompressStatus.NotFinished
                          || decompressionStatus == DecompressStatus.Success)
                    )
                    {
                        throw new InvalidOperationException($"Unexpected Decompress Status Code {decompressionStatus}");
                    }

                    int written = outSize;
                    int read = remaining;
                    totalWritten += written;
                    writeOffset += written;
                    this._inputAvailable = this._inputAvailable - read;
                    this._readOffset += read;
                } while (decompressionStatus == DecompressStatus.HasMoreOutput && totalWritten < count);

                if (this._inputAvailable == 0)
                {
                    this.ReadInput();
                }
            } while (totalWritten < count && this._inputAvailable > 0);

            return totalWritten;
        }

        private void ReadInput()
        {
            if (!this._readFinishing)
            {
                this._inputAvailable = this._stream.Read(this._buffer, 0, this._buffer.Length);
                this._readOffset = 0;
            }

            this._readFinishing = this._readFinishing || this._inputAvailable == 0;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.Write(buffer, offset, count, false);
        }

        private void Write(byte[] buffer, int offset, int count, bool finishing)
        {
            this.EnsureCompressionMode();
            this.ValidateParameters(buffer, offset, count);
            this.EnsureNotDisposed();
            int outSize = this._buffer.Length;
            int remaining = count;
            do
            {
                CompressStatus compressionStatus;
                do
                {
                    compressionStatus = LzhamInterop.Compress(this._compressionHandle, buffer, ref remaining, offset, this._buffer, ref outSize, 0, finishing);
                    if (!(compressionStatus == CompressStatus.HasMoreOutput
                          || compressionStatus == CompressStatus.NeedsMoreInput
                          || compressionStatus == CompressStatus.Success && finishing)
                    )
                    {
                        throw new InvalidOperationException($"Unexpected Compress Status Code {compressionStatus}");
                    }

                    int readBytes = remaining;
                    int writtenBytes = outSize;
                    if (writtenBytes > 0)
                    {
                        this._stream.Write(this._buffer, 0, writtenBytes);
                    }

                    remaining = remaining - readBytes;
                } while (compressionStatus == CompressStatus.HasMoreOutput);
            } while (remaining > 0);
        }

        private void ValidateParameters(byte[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (array.Length - offset < count)
                throw new ArgumentException("Offset plus count is larger than the length of target array");
        }

        public Stream BaseStream => this._stream;

        public override bool CanSeek => false;

        public override bool CanRead
        {
            get
            {
                if (this._stream == null)
                {
                    return false;
                }

                return this._decompressionHandle != null && !this._decompressionHandle.IsInvalid && this._stream.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (this._stream == null)
                {
                    return false;
                }

                return this._compressionHandle != null && !this._compressionHandle.IsInvalid && this._stream.CanWrite;
            }
        }

        public override long Length => throw new NotSupportedException("This operation is not supported");

        public uint Finish()
        {
            uint? result = null;
            if (this._compressionHandle != null && !this._compressionHandle.IsInvalid)
            {
                this.FinishCompression();
                result = this._compressionHandle.Finish();
            }

            if (this._decompressionHandle != null && !this._decompressionHandle.IsInvalid)
            {
                result = this._decompressionHandle.Finish();
            }

            if (!this._leaveOpen)
            {
                this._stream?.Dispose();
            }

            if (!result.HasValue)
            {
                throw new InvalidOperationException("It appears that operation has already finished");
            }

            return result.Value;
        }

        private void FinishCompression()
        {
            this.Write(new byte[0], 0, 0, true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                this.Finish();
            }
            finally
            {
                this._stream = null;
                try
                {
                    this._compressionHandle?.Dispose();
                    this._decompressionHandle?.Dispose();
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        public override long Position
        {
            get => throw new NotSupportedException("This operation is not supported");
            set => throw new NotSupportedException("This operation is not supported");
        }
    }
}