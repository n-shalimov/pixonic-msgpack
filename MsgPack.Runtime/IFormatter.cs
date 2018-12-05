namespace Pixonic.MsgPack
{
    public interface IFormatter {}

    public interface IFormatter<T> : IFormatter
    {
        void Write(T value, MsgPackStream stream, IContext context);
        T Read(MsgPackStream stream, IContext context);
    }
}
