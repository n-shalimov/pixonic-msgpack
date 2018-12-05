using System.Collections.Generic;

namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public sealed class ListFormatter<T> : IFormatter<List<T>>
    {
        private readonly IFormatter<T> _itemFormatter;

        public ListFormatter() : this(null) { }

        public ListFormatter(IFormatter<T> itemFormatter)
        {
            _itemFormatter = itemFormatter;
        }

        void IFormatter<List<T>>.Write(List<T> value, MsgPackStream stream, IContext context)
        {
            if (value == null)
            {
                StreamWriter.WriteNil(stream);
                return;
            }

            var formatter = _itemFormatter ?? context.ResolveFormatter<T>();
            var count = unchecked((uint)value.Count);
            StreamWriter.WriteArrayHeader(count, stream);
            for (var i = 0; i < count; ++i)
            {
                formatter.Write(value[i], stream, context);
            }
        }

        List<T> IFormatter<List<T>>.Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return null;
            }

            var formatter = _itemFormatter ?? context.ResolveFormatter<T>();
            var count = checked((int)StreamReader.ReadArrayHeader(stream));
            var list = new List<T>(count);
            for (var i = 0; i < count; ++i)
            {
                list.Add(formatter.Read(stream, context));
            }

            return list;
        }
    }
}
