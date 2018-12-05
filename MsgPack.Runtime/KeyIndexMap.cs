using System.Collections.Generic;

namespace Pixonic.MsgPack
{
    public sealed class KeyIndexMap
    {
        private readonly BufferSegment[] _keys;
        private readonly Dictionary<BufferSegment, int> _keyMap;

        public KeyIndexMap(params string[] keys)
        {
            int count = keys.Length;
            _keys = new BufferSegment[count];
            _keyMap = new Dictionary<BufferSegment, int>(count, BufferSegment.EqualityComparer);

            for (int i = 0; i < count; ++i)
            {
                _keys[i] = new BufferSegment(System.Text.Encoding.UTF8.GetBytes(keys[i]));
                _keyMap[_keys[i]] = i;
            }
        }

        public BufferSegment this[int index]
        {
            get { return _keys[index]; }
        }

        public bool TryGetIndex(BufferSegment key, out int index)
        {
            return _keyMap.TryGetValue(key, out index);
        }
    }
}
