namespace Pixonic.MsgPack.Formatters
{
    public abstract class CustomArrayFormatter<TArray, TItem> : IFormatter<TArray>
    {
        void IFormatter<TArray>.Write(TArray value, MsgPackStream stream, IContext context)
        {
            if (!typeof(TArray).IsValueType && value.Equals(default(TArray)))
            {
                StreamWriter.WriteNil(stream);
                return;
            }

            var formatter = context.ResolveFormatter<TItem>();
            var length = GetLength(value);
            StreamWriter.WriteArrayHeader(unchecked((uint)length), stream);
            for (var i = 0; i < length; ++i)
            {
                formatter.Write(GetItem(value, i), stream, context);
            }
        }

        TArray IFormatter<TArray>.Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return GetNil();
            }

            var formatter = context.ResolveFormatter<TItem>();
            var count = StreamReader.ReadArrayHeader(stream);
            var array = Create(count);
            for (var i = 0; i < count; ++i)
            {
                SetItem(array, i, formatter.Read(stream, context));
            }

            return array;
        }

        protected virtual TArray GetNil()
        {
            return default(TArray);
        }

        protected abstract TArray Create(long length);
        protected abstract long GetLength(TArray array);
        protected abstract TItem GetItem(TArray array, int index);
        protected abstract void SetItem(TArray array, int index, TItem value);
    }
}
