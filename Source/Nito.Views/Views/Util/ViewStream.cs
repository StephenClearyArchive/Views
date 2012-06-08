using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Views.Util
{
    /// <summary>
    /// Provides a read-only stream for a view of bytes. This is similar to <c>MemoryStream</c>.
    /// </summary>
    public sealed class ViewStream : Stream
    {
        /// <summary>
        /// The underlying view.
        /// </summary>
        private readonly IView<byte> view;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewStream"/> class.
        /// </summary>
        /// <param name="view">The underlying view.</param>
        public ViewStream(IView<byte> view)
        {
            this.view = view;
        }

        /// <summary>
        /// Gets the underlying view.
        /// </summary>
        public IView<byte> View
        {
            get { return this.view; }
        }

        /// <summary>
        /// Gets a value indicating whether this stream permits read operations. Always returns <c>true</c>.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether this stream permits seek operations. Always returns <c>true</c>.
        /// </summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether this stream permits write operations. Always returns <c>false</c>.
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Gets the length of the stream. This is the number of elements in the underlying view.
        /// </summary>
        public override long Length
        {
            get { return this.view.Count; }
        }

        /// <summary>
        /// Gets or sets the current position of the stream.
        /// </summary>
        public override long Position { get; set; }

        /// <summary>
        /// Flushes the stream. This is a noop.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Reads a sequence of bytes from the stream and advances the stream position.
        /// </summary>
        /// <param name="buffer">The buffer into which the bytes are read from the stream.</param>
        /// <param name="offset">The offset in the buffer to which to begin storing the bytes read from the stream.</param>
        /// <param name="count">The maximum number of bytes to read into the buffer.</param>
        /// <returns>The number of bytes actually read into the buffer.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesToTransfer = Math.Min(count, checked((int)(this.view.Count - this.Position)));
            for (int i = 0; i != bytesToTransfer; ++i)
                buffer[offset + i] = this.view[checked((int)this.Position + i)];
            this.Position += bytesToTransfer;
            return bytesToTransfer;
        }

        /// <summary>
        /// Sets the stream position relative to an origin.
        /// </summary>
        /// <param name="offset">The offset to apply to the origin.</param>
        /// <param name="origin">The origin of the new stream position.</param>
        /// <returns>The new stream position.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = checked((int)offset);
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.view.Count + offset;
                    break;
            }

            return this.Position;
        }

        /// <summary>
        /// Sets the length of the stream. Always throws <c>NotSupportedException</c>.
        /// </summary>
        /// <param name="value">The new length of the stream.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes bytes to the stream. Always throws <c>NotSupportedException</c>.
        /// </summary>
        /// <param name="buffer">The buffer containing bytes to write.</param>
        /// <param name="offset">The offset in the buffer at which to begin writing.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
