using Pixonic.MsgPack;

namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public class DecimalFormatter : IFormatter<decimal>
    {
        void IFormatter<decimal>.Write(decimal value, MsgPackStream stream, IContext context)
        {
            context.ResolveFormatter<double>().Write((double)value, stream, context);
        }

        decimal IFormatter<decimal>.Read(MsgPackStream stream, IContext context)
        {
            var code = stream.Peek();
            var type = StreamReader.GetType(stream);
            switch (type)
            {
                case FormatType.Integer:
                    return FormatCode.IsSignedInteger(code)
                        ? (decimal)context.ResolveFormatter<long>().Read(stream, context)
                        : context.ResolveFormatter<ulong>().Read(stream, context);

                case FormatType.Float:
                    return code == FormatCode.Float32
                        ? (decimal)context.ResolveFormatter<float>().Read(stream, context)
                        : (decimal)context.ResolveFormatter<double>().Read(stream, context);

                default:
                    throw new MsgPackException("Invalid decimal type code: {0} ({1})", code, type);
            }
        }
    }
}
