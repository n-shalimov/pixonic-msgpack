using System.Collections.Generic;

namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public sealed class DictionaryFormatter<TKey, TValue> : IFormatter<Dictionary<TKey, TValue>>
    {
        void IFormatter<Dictionary<TKey, TValue>>.Write(Dictionary<TKey, TValue> value, MsgPackStream stream,
            IContext context)
        {
            if (value == null)
            {
                StreamWriter.WriteNil(stream);
                return;
            }

            var keyFormatter = context.ResolveFormatter<TKey>();
            var valueFormatter = context.ResolveFormatter<TValue>();
            var count = unchecked((uint)value.Count);

            StreamWriter.WriteMapHeader(count, stream);

            var enumerator = value.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                keyFormatter.Write(pair.Key, stream, context);
                valueFormatter.Write(pair.Value, stream, context);
            }
        }

        Dictionary<TKey, TValue> IFormatter<Dictionary<TKey, TValue>>.Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return null;
            }

            var keyFormatter = context.ResolveFormatter<TKey>();
            var valueFormatter = context.ResolveFormatter<TValue>();
            var count = StreamReader.ReadMapHeader(stream);

            var dict = new Dictionary<TKey, TValue>(unchecked((int)count));

            for (int i = 0; i < count; ++i)
            {
                var key = keyFormatter.Read(stream, context);
                var value = valueFormatter.Read(stream, context);
                dict.Add(key, value);
            }

            return dict;
        }
    }
}
