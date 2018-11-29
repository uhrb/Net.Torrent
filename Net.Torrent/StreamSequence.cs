using System;
using System.Collections.Generic;
using System.IO;

namespace Net.Torrent
{
    public class StreamSequence : Stream
    {
        private readonly IList<(Stream, long, bool)> _streams;
        private int _currentStreamIndex;
        private int _currentStreamReadedTotal;
        private long _currentPosition;
        public StreamSequence(IList<(Stream, long, bool)> streams)
        {
            _currentPosition = 0;
            _currentStreamIndex = 0;
            _currentStreamReadedTotal = 0;
            _streams = streams ?? throw new ArgumentNullException(nameof(streams));
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _currentPosition;
            set => throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (var (stream, len, close) in _streams)
                {
                    if (close)
                    {
                        try
                        {
                            stream.Dispose();
                        }
                        catch (Exception)
                        {
                            // do nothing
                        }
                    }
                }
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if(_currentStreamIndex >= _streams.Count)
            {
                return 0;
            }
            var current = _streams[_currentStreamIndex];
            var readed = _streams[_currentStreamIndex].Item1.Read(buffer, offset, count);
            offset += readed;
            _currentStreamReadedTotal += readed;
            _currentPosition += readed;
            if(readed < count)
            {
                if (current.Item3)
                {
                    current.Item1.Dispose();
                }
                if(_currentStreamReadedTotal < current.Item2)
                {
                    throw new IOException("Expected stream length was not reached");
                }
                _currentStreamReadedTotal = 0;
                _currentStreamIndex++;
                return readed + Read(buffer, offset, count - readed);
            }

            return readed;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
