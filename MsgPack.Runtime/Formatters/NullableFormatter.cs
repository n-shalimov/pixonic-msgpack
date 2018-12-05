namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public sealed class NullableFormatter<T> : IFormatter<T?>
        where T : struct
    {
        private readonly IFormatter<T> _underlyingFormatter;

        public NullableFormatter() : this(null) {}

        public NullableFormatter(IFormatter<T> underlyingFormatter)
        {
            _underlyingFormatter = underlyingFormatter;
        }

        void IFormatter<T?>.Write(T? value, MsgPackStream stream, IContext context)
        {
            if (value.HasValue)
            {
                (_underlyingFormatter ?? context.ResolveFormatter<T>()).Write(value.Value, stream, context);
            }
            else
            {
                StreamWriter.WriteNil(stream);
            }
        }

        T? IFormatter<T?>.Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return null;
            }

            return (_underlyingFormatter ?? context.ResolveFormatter<T>()).Read(stream, context);
        }
    }
}
