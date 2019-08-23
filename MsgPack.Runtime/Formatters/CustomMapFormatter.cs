using System.Collections.Generic;
using Pixonic.MsgPack.Formatters;

namespace Pixonic.MsgPack.Runtime.Formatters
{
    public abstract class CustomMapFormatter<TMap, TKey, TValue> : IFormatter<TMap>
    {
        void IFormatter<TMap>.Write(TMap value, MsgPackStream stream, IContext context)
        {
            if (!typeof(TMap).IsValueType && object.Equals(value, default(TMap)))
            {
                StreamWriter.WriteNil(stream);
                return;
            }

            var keyFormatter = context.ResolveFormatter<TKey>();
            var valueFormatter = context.ResolveFormatter<TValue>();
            var length = GetLength(value);

            StreamWriter.WriteMapHeader(length, stream);

            var enumerator = Enumerate(value);
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                keyFormatter.Write(pair.Key, stream, context);
                valueFormatter.Write(pair.Value, stream, context);
            }
        }

        TMap IFormatter<TMap>.Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return GetNil();
            }

            var keyFormatter = context.ResolveFormatter<TKey>();
            var valueFormatter = context.ResolveFormatter<TValue>();
            var length = StreamReader.ReadMapHeader(stream);
            var map = Create(length);

            for (int i = 0; i < length; ++i)
            {
                var key = keyFormatter.Read(stream, context);
                var value = valueFormatter.Read(stream, context);
                Set(map, key, value);
            }

            return map;
        }

        protected virtual TMap GetNil()
        {
            return default(TMap);
        }

        protected abstract TMap Create(uint length);
        protected abstract uint GetLength(TMap map);
        protected abstract IEnumerator<KeyValuePair<TKey, TValue>> Enumerate(TMap map);
        protected abstract void Set(TMap map, TKey key, TValue value);
    }
}
