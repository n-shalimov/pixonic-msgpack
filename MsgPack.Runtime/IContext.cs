namespace Pixonic.MsgPack
{
    public interface IContext
    {
        IFormatter<T> ResolveFormatter<T>();
        IFormatter ResolveFormatter(System.Type type);
        void Trace(string message);
    }
}
