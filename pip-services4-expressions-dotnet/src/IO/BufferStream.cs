using System;
using System.IO;

namespace PipServices4.Expressions.IO
{
    /// <summary>
    /// This class is used for buffered read/write to the other stream.
    /// This class is used at least in FieldComm for reading/writing messages to the NetworkStream.
    /// This class is a port for .Net Compact Framework from System.IO.BufferedStream (Full .Net Framework).
    /// </summary>
    public class BufferStream : Stream
    {
        // Fields
        private const int DefaultBufferSize = 0x1000;
        private readonly int _bufferSize;
        private byte[] _buffer;
        private int _readLength;
        private int _readPosition;
        private Stream _stream;
        private int _writePosition;

        public BufferStream(Stream stream)
            : this(stream, DefaultBufferSize)
        {
        }

        public BufferStream(Stream stream, int bufferSize)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            _stream = stream;
            _bufferSize = bufferSize;
            if (!_stream.CanRead && !_stream.CanWrite)
            {
                Errors.StreamIsClosed();
            }
        }

        public override bool CanRead
        {
            get { return ((_stream != null) && _stream.CanRead); }
        }


        public override bool CanSeek
        {
            get { return ((_stream != null) && _stream.CanSeek); }
        }

        public override bool CanWrite
        {
            get { return ((_stream != null) && _stream.CanWrite); }
        }

        public override long Length
        {
            get
            {
                if (_stream == null)
                {
                    Errors.StreamIsClosed();
                }

                if (_writePosition > 0)
                {
                    FlushWrite();
                }

                return _stream.Length;
            }
        }


        public override long Position
        {
            get
            {
                if (_stream == null)
                {
                    Errors.StreamIsClosed();
                }
                if (!_stream.CanSeek)
                {
                    Errors.SeekNotSupported();
                }
                return (_stream.Position + ((_readPosition - _readLength) + _writePosition));
            }
            set
            {
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (_stream == null)
                {
                    Errors.StreamIsClosed();
                }
                if (!_stream.CanSeek)
                {
                    Errors.SeekNotSupported();
                }
                if (_writePosition > 0)
                {
                    FlushWrite();
                }
                _readPosition = 0;
                _readLength = 0;
                _stream.Seek(value, SeekOrigin.Begin);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (_stream != null))
                {
                    try
                    {
                        Flush();
                    }
                    finally
                    {
                        _stream.Close();
                    }
                }
            }
            finally
            {
                _stream = null;
                _buffer = null;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            if (_writePosition > 0)
            {
                FlushWrite();
            }
            else if ((_readPosition < _readLength) && _stream.CanSeek)
            {
                FlushRead();
            }
            _readPosition = 0;
            _readLength = 0;
        }

        private void FlushRead()
        {
            if ((_readPosition - _readLength) != 0 && _stream.CanSeek)
            {
                _stream.Seek(_readPosition - _readLength, SeekOrigin.Current);
            }
            _readPosition = 0;
            _readLength = 0;
        }


        private void FlushWrite()
        {
            _stream.Write(_buffer, 0, _writePosition);
            _writePosition = 0;
            _stream.Flush();
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            int num = _readLength - _readPosition;
            if (num == 0)
            {
                if (!_stream.CanRead)
                {
                    Errors.ReadNotSupported();
                }
                if (_writePosition > 0)
                {
                    FlushWrite();
                }
                if (count >= _bufferSize)
                {
                    num = _stream.Read(array, offset, count);
                    _readPosition = 0;
                    _readLength = 0;
                    return num;
                }
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
                num = _stream.Read(_buffer, 0, _bufferSize);
                if (num == 0)
                {
                    return 0;
                }
                _readPosition = 0;
                _readLength = num;
            }
            if (num > count)
            {
                num = count;
            }

            Buffer.BlockCopy(_buffer, _readPosition, array, offset, num);
            _readPosition += num;
            if (num < count)
            {

                int num2 = _stream.Read(array, offset + num, count - num);

                num += num2;
                _readPosition = 0;
                _readLength = 0;
            }
            return num;
        }

        public override int ReadByte()
        {
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            if ((_readLength == 0) && !_stream.CanRead)
            {
                Errors.ReadNotSupported();
            }
            if (_readPosition == _readLength)
            {
                if (_writePosition > 0)
                {
                    FlushWrite();
                }
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
                _readLength = _stream.Read(_buffer, 0, _bufferSize);
                _readPosition = 0;
            }
            if (_readPosition == _readLength)
            {
                return -1;
            }
            return _buffer[_readPosition++];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            if (!_stream.CanSeek)
            {
                Errors.SeekNotSupported();
            }
            if (_writePosition > 0)
            {
                FlushWrite();
            }
            else if (origin == SeekOrigin.Current)
            {
                offset -= _readLength - _readPosition;
            }
            long num = _stream.Position + (_readPosition - _readLength);
            long num2 = _stream.Seek(offset, origin);
            if (_readLength > 0)
            {
                if (num == num2)
                {
                    if (_readPosition > 0)
                    {
                        Buffer.BlockCopy(_buffer, _readPosition, _buffer, 0, _readLength - _readPosition);
                        _readLength -= _readPosition;
                        _readPosition = 0;
                    }
                    if (_readLength > 0)
                    {
                        _stream.Seek(_readLength, SeekOrigin.Current);
                    }
                    return num2;
                }
                if (((num - _readPosition) < num2) && (num2 < ((num + _readLength) - _readPosition)))
                {
                    int num3 = (int)(num2 - num);
                    Buffer.BlockCopy(_buffer, _readPosition + num3, _buffer, 0, _readLength - (_readPosition + num3));
                    _readLength -= _readPosition + num3;
                    _readPosition = 0;
                    if (_readLength > 0)
                    {
                        _stream.Seek(_readLength, SeekOrigin.Current);
                    }
                    return num2;
                }
                _readPosition = 0;
                _readLength = 0;
            }
            return num2;
        }

        public override void SetLength(long value)
        {
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException("value");
            }
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            if (!_stream.CanSeek)
            {
                Errors.SeekNotSupported();
            }
            if (!_stream.CanWrite)
            {
                Errors.WriteNotSupported();
            }
            if (_writePosition > 0)
            {
                FlushWrite();
            }
            else if (_readPosition < _readLength && _stream.CanSeek)
            {
                FlushRead();
            }
            _readPosition = 0;
            _readLength = 0;
            _stream.SetLength(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if ((array.Length - offset) < count)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            if (_writePosition == 0)
            {
                if (!_stream.CanWrite)
                {
                    Errors.WriteNotSupported();
                }
                if (_readPosition < _readLength && _stream.CanSeek)
                {
                    FlushRead();
                }
                else
                {
                    _readPosition = 0;
                    _readLength = 0;
                }
            }
            if (_writePosition > 0)
            {
                int num = _bufferSize - _writePosition;
                if (num > 0)
                {
                    if (num > count)
                    {
                        num = count;
                    }
                    Buffer.BlockCopy(array, offset, _buffer, _writePosition, num);
                    _writePosition += num;
                    if (count == num)
                    {
                        return;
                    }
                    offset += num;
                    count -= num;
                }
                _stream.Write(_buffer, 0, _writePosition);
                _writePosition = 0;
            }
            if (count >= _bufferSize)
            {
                _stream.Write(array, offset, count);
            }
            else if (count != 0)
            {
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
                Buffer.BlockCopy(array, offset, _buffer, 0, count);
                _writePosition = count;
            }
        }

        public override void WriteByte(byte value)
        {
            if (_stream == null)
            {
                Errors.StreamIsClosed();
            }
            if (_writePosition == 0)
            {
                if (!_stream.CanWrite)
                {
                    Errors.WriteNotSupported();
                }
                if (_readPosition < _readLength && _stream.CanSeek)
                {
                    FlushRead();
                }
                else
                {
                    _readPosition = 0;
                    _readLength = 0;
                }
                if (_buffer == null)
                {
                    _buffer = new byte[_bufferSize];
                }
            }
            if (_writePosition == _bufferSize)
            {
                FlushWrite();
            }
            _buffer[_writePosition++] = value;
        }

        private static class Errors
        {
            public static void StreamIsClosed()
            {
                throw new IOException("Stream is closed");
            }

            public static void SeekNotSupported()
            {
                throw new NotSupportedException("Operation 'Seek' is not supported");
            }

            public static void ReadNotSupported()
            {
                throw new NotSupportedException("Operation 'Read' is not supported");
            }

            public static void WriteNotSupported()
            {
                throw new NotSupportedException("Operation 'Write' is not supported");
            }
        }

    }
}
