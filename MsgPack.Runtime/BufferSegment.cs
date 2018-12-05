using System.Collections.Generic;

namespace Pixonic.MsgPack
{
    public struct BufferSegment : System.IEquatable<BufferSegment>
    {
        public static readonly IEqualityComparer<BufferSegment> EqualityComparer = new Comparer();

        private readonly byte[] _array;
        private readonly long _offset;
        private readonly long _length;

        public BufferSegment(byte[] array) : this(array, 0, array.LongLength) {}

        public BufferSegment(byte[] array, long offset, long length)
        {
            if (array != null && offset + length > array.LongLength)
            {
                throw new System.IndexOutOfRangeException("Buffer segment is out of range");
            }

            _array = array;
            _offset = offset;
            _length = length;
        }

        public byte[] Array
        {
            get { return _array; }
        }

        public long Offset
        {
            get { return _offset; }
        }

        public long Length
        {
            get { return _length; }
        }

        public bool Equals(BufferSegment other)
        {
            return EqualityComparer.Equals(this, other);
        }

        public override int GetHashCode()
        {
            return EqualityComparer.GetHashCode(this);
        }

        public override string ToString()
        {
            return _array == null
                ? null
                : System.Text.Encoding.UTF8.GetString(_array, (int)_offset, (int)_length);
        }

        public byte[] ToArray()
        {
            if (_array == null)
            {
                return null;
            }

            var result = new byte[_length];
            System.Array.Copy(_array, _offset, result, 0, _length);
            return result;
        }

        private class Comparer : IEqualityComparer<BufferSegment>
        {
            public bool Equals(BufferSegment x, BufferSegment y)
            {
                if (x._length != y._length)
                {
                    return false;
                }

                if (x._array == y._array && x._offset == y._offset)
                {
                    return true;
                }

                for (var i = 0; i < x._length; ++i)
                {
                    if (x._array[x._offset + i] != y._array[y._offset + i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(BufferSegment segment)
            {
                // Simple FNV-1a hash
                unchecked
                {
                    const int prime = 16777619;
                    int hash = (int)2166136261;

                    for (int i = 0; i < segment._length; ++i)
                    {
                        hash = (hash ^ segment._array[segment._offset + i]) * prime;
                    }

                    return hash;
                }
            }
        }
    }
}
