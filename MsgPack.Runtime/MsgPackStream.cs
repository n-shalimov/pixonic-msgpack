using System.Runtime.InteropServices;
using System.Text;

namespace Pixonic.MsgPack
{
    public sealed class MsgPackStream
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);
        private const int ChunkSize = 1024;

        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly byte[] _tempBuffer = new byte[ChunkSize];

        private byte[] _buffer;
        private bool _isExternal;
        private long _position;

        public BufferSegment Output
        {
            get { return new BufferSegment(_buffer, 0, _position); }
        }

        public long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public MsgPackStream() {}

        public MsgPackStream(long initialCapacity)
        {
            BeginWrite(initialCapacity);
        }

        public MsgPackStream(byte[] input, long offset)
        {
            BeginRead(input, offset);
        }

        public void BeginWrite(long initialCapacity)
        {
            _buffer = new byte[initialCapacity];
            _isExternal = false;
            _position = 0;
        }

        public void BeginRead(byte[] input, long offset)
        {
            _buffer = input;
            _isExternal = true;
            _position = offset;
        }

        public void Reset()
        {
            _buffer = null;
            _position = 0;
        }

        public byte Peek()
        {
            return _buffer[_position];
        }

        public sbyte ReadInt8()
        {
            unchecked
            {
                Load(_tempBuffer, 1);
                return (sbyte)_tempBuffer[0];
            }
        }

        public void WriteInt8(sbyte value)
        {
            unchecked
            {
                _tempBuffer[0] = (byte)value;
                Flush(_tempBuffer, 0, 1);
            }
        }

        public byte ReadUInt8()
        {
            Load(_tempBuffer, 1);
            return _tempBuffer[0];
        }

        public void WriteUInt8(byte value)
        {
            _tempBuffer[0] = value;
            Flush(_tempBuffer, 0, 1);
        }

        public short ReadInt16()
        {
            unchecked
            {
                Load(_tempBuffer, 2);
                return (short)((_tempBuffer[0] << 8) | _tempBuffer[1]);
            }
        }

        public void WriteInt16(short value)
        {
            unchecked
            {
                _tempBuffer[0] = (byte)(value >> 8);
                _tempBuffer[1] = (byte)value;
                Flush(_tempBuffer, 0, 2);
            }
        }

        public ushort ReadUInt16()
        {
            return unchecked((ushort)ReadInt16());
        }

        public void WriteUInt16(ushort value)
        {
            WriteInt16(unchecked((short)value));
        }

        public int ReadInt32()
        {
            Load(_tempBuffer, 4);
            return (_tempBuffer[0] << 24) | (_tempBuffer[1] << 16) | (_tempBuffer[2] << 8) | _tempBuffer[3];
        }

        public void WriteInt32(int value)
        {
            unchecked
            {
                _tempBuffer[0] = (byte)(value >> 24);
                _tempBuffer[1] = (byte)(value >> 16);
                _tempBuffer[2] = (byte)(value >> 8);
                _tempBuffer[3] = (byte)value;
                Flush(_tempBuffer, 0, 4);
            }
        }

        public uint ReadUInt32()
        {
            return unchecked((uint)ReadInt32());
        }

        public void WriteUInt32(uint value)
        {
            WriteInt32(unchecked((int)value));
        }

        public long ReadInt64()
        {
            Load(_tempBuffer, 8);
            return
                ((long)_tempBuffer[0] << 56) |
                ((long)_tempBuffer[1] << 48) |
                ((long)_tempBuffer[2] << 40) |
                ((long)_tempBuffer[3] << 32) |
                ((long)_tempBuffer[4] << 24) |
                ((long)_tempBuffer[5] << 16) |
                ((long)_tempBuffer[6] << 8) |
                (long)_tempBuffer[7];
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct LongULong
        {
            [FieldOffset(0)]
            public long LongValue;
            [FieldOffset(1)]
            public ulong ULongValue;
        }

        public void WriteInt64(long value)
        {
            unchecked
            {
                _tempBuffer[0] = (byte)(value >> 56);
                _tempBuffer[1] = (byte)(value >> 48);
                _tempBuffer[2] = (byte)(value >> 40);
                _tempBuffer[3] = (byte)(value >> 32);
                _tempBuffer[4] = (byte)(value >> 24);
                _tempBuffer[5] = (byte)(value >> 16);
                _tempBuffer[6] = (byte)(value >> 8);
                _tempBuffer[7] = (byte)value;
                Flush(_tempBuffer, 0, 8);
            }
        }

        public ulong ReadUInt64()
        {
            return unchecked((ulong)ReadInt64());
        }

        public void WriteUInt64(ulong value)
        {
            WriteInt64(unchecked((long)value));
        }

        public float ReadSingle()
        {
            Load(_tempBuffer, 4);
            return FloatBytes.Read(_tempBuffer);
        }

        public void WriteSingle(float value)
        {
            FloatBytes.Write(value, _tempBuffer);
            Flush(_tempBuffer, 0, 4);
        }

        public double ReadDouble()
        {
            Load(_tempBuffer, 8);
            return DoubleBytes.Read(_tempBuffer);
        }

        public void WriteDouble(double value)
        {
            DoubleBytes.Write(value, _tempBuffer);
            Flush(_tempBuffer, 0, 8);
        }

        public string ReadString(uint length)
        {
            unchecked
            {
                if (length <= ChunkSize)
                {
                    Load(_tempBuffer, (int)length);
                    return Utf8.GetString(_tempBuffer, 0, (int)length);
                }

                _stringBuilder.Length = 0;
                while (length > 0)
                {
                    var chunkSize = length > ChunkSize ? ChunkSize : (int)length;

                    Load(_tempBuffer, chunkSize);
                    _stringBuilder.Append(Utf8.GetString(_tempBuffer, 0, chunkSize));

                    length -= (uint)chunkSize;
                }

                return _stringBuilder.ToString();
            }
        }

        public void WriteString(string value)
        {
            var length = value.Length;
            var maxChunkSize = Utf8.GetMaxCharCount(ChunkSize);

            for (var offset = 0; length > 0;)
            {
                var chunkSize = length > maxChunkSize ? maxChunkSize : length;
                var bytesCount = Utf8.GetBytes(value, offset, chunkSize, _tempBuffer, 0);

                Flush(_tempBuffer, 0, bytesCount);

                offset += chunkSize;
                length -= chunkSize;
            }
        }

        public BufferSegment ReadBytes(uint length)
        {
            var segment = new BufferSegment(_buffer, _position, length);
            _position += length;
            return segment;
        }

        public void WriteBytes(BufferSegment value)
        {
            Flush(value.Array, value.Offset, value.Length);
        }

        public void Skip(uint count)
        {
            _position += count;
        }

        private void Load(byte[] destination, int count)
        {
            if (_buffer == null)
            {
                throw new System.InvalidOperationException("Buffer not in read mode");
            }

            System.Array.Copy(_buffer, _position, destination, 0, count);
            _position += count;
        }

        private void Flush(byte[] source, long offset, long count)
        {
            if (_buffer == null)
            {
                _buffer = new byte[GetCapacityFor(count)];
            }
            else
            {
                var newPosition = _position + count;
                if (newPosition > uint.MaxValue || newPosition < 0)
                {
                    throw new System.IndexOutOfRangeException("Unable to expand external buffer");
                }

                if (newPosition >= _buffer.LongLength)
                {
                    if (_isExternal)
                    {
                        throw new System.IndexOutOfRangeException("Unable to expand external buffer");
                    }

                    var capacity = _buffer.LongLength;
                    while (_position + count > capacity)
                    {
                        capacity *= 2;
                    }

                    // TODO: do something with max size
                    /*if (capacity > uint.MaxValue)
                    {
                        capacity = int.MaxValue;
                    }*/

                    if (_buffer.LongLength != capacity)
                    {
                        var newOutput = new byte[capacity];
                        System.Array.Copy(_buffer, 0, newOutput, 0, _position);
                        _buffer = newOutput;
                    }
                }
            }

            System.Array.Copy(source, offset, _buffer, _position, count);
            _position += count;
        }

        private static long GetCapacityFor(long count)
        {
            return ChunkSize * ((count / ChunkSize) + 1);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatBytes
        {
            [FieldOffset(0)] private float _value;

            [FieldOffset(0)] private byte _byte0;
            [FieldOffset(1)] private byte _byte1;
            [FieldOffset(2)] private byte _byte2;
            [FieldOffset(3)] private byte _byte3;

            public static float Read(byte[] bytes)
            {
                var floatBytes = new FloatBytes();
                if (System.BitConverter.IsLittleEndian)
                {
                    floatBytes._byte0 = bytes[3];
                    floatBytes._byte1 = bytes[2];
                    floatBytes._byte2 = bytes[1];
                    floatBytes._byte3 = bytes[0];
                }
                else
                {
                    floatBytes._byte0 = bytes[0];
                    floatBytes._byte1 = bytes[1];
                    floatBytes._byte2 = bytes[2];
                    floatBytes._byte3 = bytes[3];
                }

                return floatBytes._value;
            }

            public static void Write(float value, byte[] bytes)
            {
                var floatBytes = new FloatBytes { _value = value };
                if (System.BitConverter.IsLittleEndian)
                {
                    bytes[0] = floatBytes._byte3;
                    bytes[1] = floatBytes._byte2;
                    bytes[2] = floatBytes._byte1;
                    bytes[3] = floatBytes._byte0;
                }
                else
                {
                    bytes[0] = floatBytes._byte0;
                    bytes[1] = floatBytes._byte1;
                    bytes[2] = floatBytes._byte2;
                    bytes[3] = floatBytes._byte3;
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleBytes
        {
            [FieldOffset(0)] private double _value;

            [FieldOffset(0)] private byte _byte0;
            [FieldOffset(1)] private byte _byte1;
            [FieldOffset(2)] private byte _byte2;
            [FieldOffset(3)] private byte _byte3;
            [FieldOffset(4)] private byte _byte4;
            [FieldOffset(5)] private byte _byte5;
            [FieldOffset(6)] private byte _byte6;
            [FieldOffset(7)] private byte _byte7;

            public static double Read(byte[] bytes)
            {
                var doubleBytes = new DoubleBytes();
                if (System.BitConverter.IsLittleEndian)
                {
                    doubleBytes._byte0 = bytes[7];
                    doubleBytes._byte1 = bytes[6];
                    doubleBytes._byte2 = bytes[5];
                    doubleBytes._byte3 = bytes[4];
                    doubleBytes._byte4 = bytes[3];
                    doubleBytes._byte5 = bytes[2];
                    doubleBytes._byte6 = bytes[1];
                    doubleBytes._byte7 = bytes[0];
                }
                else
                {
                    doubleBytes._byte0 = bytes[0];
                    doubleBytes._byte1 = bytes[1];
                    doubleBytes._byte2 = bytes[2];
                    doubleBytes._byte3 = bytes[3];
                    doubleBytes._byte4 = bytes[4];
                    doubleBytes._byte5 = bytes[5];
                    doubleBytes._byte6 = bytes[6];
                    doubleBytes._byte7 = bytes[7];
                }

                return doubleBytes._value;
            }

            public static void Write(double value, byte[] bytes)
            {
                var doubleBytes = new DoubleBytes { _value = value };
                if (System.BitConverter.IsLittleEndian)
                {
                    bytes[0] = doubleBytes._byte7;
                    bytes[1] = doubleBytes._byte6;
                    bytes[2] = doubleBytes._byte5;
                    bytes[3] = doubleBytes._byte4;
                    bytes[4] = doubleBytes._byte3;
                    bytes[5] = doubleBytes._byte2;
                    bytes[6] = doubleBytes._byte1;
                    bytes[7] = doubleBytes._byte0;
                }
                else
                {
                    bytes[0] = doubleBytes._byte0;
                    bytes[1] = doubleBytes._byte1;
                    bytes[2] = doubleBytes._byte2;
                    bytes[3] = doubleBytes._byte3;
                    bytes[4] = doubleBytes._byte4;
                    bytes[5] = doubleBytes._byte5;
                    bytes[6] = doubleBytes._byte6;
                    bytes[7] = doubleBytes._byte7;
                }
            }
        }
    }
}
