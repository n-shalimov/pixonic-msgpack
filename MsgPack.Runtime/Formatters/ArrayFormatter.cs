namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public sealed class ArrayFormatter<T> : IFormatter<T[]>
    {
        private readonly IFormatter<T> _itemFormatter;

        public ArrayFormatter() : this(null) {}

        public ArrayFormatter(IFormatter<T> itemFormatter)
        {
            _itemFormatter = itemFormatter;
        }

        void IFormatter<T[]>.Write(T[] value, MsgPackStream stream, IContext context)
        {
            if (value == null)
            {
                StreamWriter.WriteNil(stream);
                return;
            }

            var formatter = _itemFormatter ?? context.ResolveFormatter<T>();
            var count = value.LongLength;
            StreamWriter.WriteArrayHeader(unchecked((uint)count), stream);
            for (var i = 0; i < count; ++i)
            {
                formatter.Write(value[i], stream, context);
            }
        }

        T[] IFormatter<T[]>.Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return null;
            }

            var formatter = _itemFormatter ?? context.ResolveFormatter<T>();
            var count = StreamReader.ReadArrayHeader(stream);
            var array = new T[count];
            for (var i = 0; i < count; ++i)
            {
                array[i] = formatter.Read(stream, context);
            }

            return array;
        }
    }
}
