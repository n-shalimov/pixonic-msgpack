namespace Pixonic.MsgPack.Formatters
{
    public sealed class IgnoreFormatter<T> : IFormatter<T>
    {
        void IFormatter<T>.Write(T value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteNil(stream);
        }

        T IFormatter<T>.Read(MsgPackStream stream, IContext context)
        {
            StreamReader.Skip(stream);
            return default(T);
        }
    }
}
