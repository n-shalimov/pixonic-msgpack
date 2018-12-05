using System.Collections.Generic;
using System.Reflection;

namespace Pixonic.MsgPack.Formatters
{
    public sealed class ObjectFormatter : IFormatter<object>
    {
        void IFormatter<object>.Write(object value, MsgPackStream stream, IContext context)
        {
            if (value == null)
            {
                StreamWriter.WriteNil(stream);
                return;
            }

            var valueType = value.GetType();
            var formatter = context.ResolveFormatter(valueType);
            var formatterType = formatter.GetType();

            var writeMethod = formatterType.GetMethod(
                string.Format("Pixonic.MsgPack.IFormatter<{0}>.Write", valueType.FullName),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (writeMethod == null)
            {
                var signature = new System.Type[] { valueType, typeof(MsgPackStream), typeof(IContext) };
                writeMethod = formatterType.GetMethod("Write", signature);
            }

            if (writeMethod == null)
            {
                throw new MsgPackException("Unable to resolve serialization method of '{0}'", formatterType.FullName);
            }

            var parameters = new object[] { value, stream, context };
            writeMethod.Invoke(formatter, parameters);
        }

        public object Read(MsgPackStream stream, IContext context)
        {
            if (StreamReader.TryReadNil(stream))
            {
                return null;
            }

            var code = stream.Peek();
            switch (StreamReader.GetType(stream))
            {
                case FormatType.Boolean:
                    return context.ResolveFormatter<bool>().Read(stream, context);

                case FormatType.Integer:
                    return FormatCode.IsSignedInteger(code)
                        ? (object)context.ResolveFormatter<long>().Read(stream, context)
                        : context.ResolveFormatter<ulong>().Read(stream, context);

                case FormatType.Float:
                    return code == FormatCode.Float32
                        ? (object)context.ResolveFormatter<float>().Read(stream, context)
                        : context.ResolveFormatter<double>().Read(stream, context);

                case FormatType.String:
                    return context.ResolveFormatter<string>().Read(stream, context);

                case FormatType.Binary:
                    return context.ResolveFormatter<byte[]>().Read(stream, context);

                case FormatType.Array:
                    var count = StreamReader.ReadArrayHeader(stream);
                    var array = new object[count];
                    for (var i = 0; i < count; ++i)
                    {
                        array[i] = Read(stream, context);
                    }
                    return array;

                case FormatType.Map:
                    var size = (int)StreamReader.ReadMapHeader(stream);
                    var map = new Dictionary<object, object>(size);
                    for (var i = 0; i < size; ++i)
                    {
                        var key = Read(stream, context);
                        var value = Read(stream, context);
                        map[key] = value;
                    }
                    return map;

                case FormatType.Extension:
                    var header = StreamReader.ReadExtensionHeader(stream);
                    if (header.TypeCode == DateTimeFormatter.TypeCode)
                    {
                        return DateTimeFormatter.Unpack(stream, header.Length);
                    }

                    return stream.ReadBytes(header.Length).ToArray();

                default:
                    throw new MsgPackException("Unsupported type code {0}", code);
            }
        }
    }
}
