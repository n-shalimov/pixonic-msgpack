using Pixonic.MsgPack.Formatters;
using System.Globalization;
using System.IO;
using System.Text;

namespace Pixonic.MsgPack
{
    public static class Json
    {
        /// <summary>
        /// Dump message-pack binary to JSON string.
        /// </summary>
        public static string ToJson(byte[] bytes, long offset = 0)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var stream = new MsgPackStream(bytes, offset);
            ToJsonCore(stream, sb);

            return sb.ToString();
        }

        /// <summary>
        /// From Json String to MessagePack binary
        /// </summary>
        public static byte[] FromJson(string str)
        {
            using (var sr = new StringReader(str))
            {
                return FromJson(sr);
            }
        }

        /// <summary>
        /// From Json String to MessagePack binary
        /// </summary>
        public static byte[] FromJson(TextReader reader)
        {
            using (var jr = new TinyJsonReader(reader, false))
            {
                var stream = new MsgPackStream(1024);
                FromJsonCore(jr, stream);
                return stream.Output.ToArray();
            }
        }

        private static uint FromJsonCore(TinyJsonReader jr, MsgPackStream stream)
        {
            uint count = 0;
            while (jr.Read())
            {
                switch (jr.TokenType)
                {
                    case TinyJsonToken.None: break;
                    case TinyJsonToken.EndObject: return count;
                    case TinyJsonToken.EndArray: return count;

                    case TinyJsonToken.StartObject:
                        {
                            var innerStream = new MsgPackStream(1024);
                            var mapCount = FromJsonCore(jr, innerStream) / 2;
                            StreamWriter.WriteMapHeader(mapCount, stream);
                            stream.WriteBytes(innerStream.Output);

                            count++;
                            break;
                        }

                    case TinyJsonToken.StartArray:
                        {
                            var innerStream = new MsgPackStream(1024);
                            var arrayCount = FromJsonCore(jr, innerStream);
                            StreamWriter.WriteArrayHeader(arrayCount, stream);
                            stream.WriteBytes(innerStream.Output);
                            count++;
                            break;
                        }
                    
                    case TinyJsonToken.Number:
                        var v = jr.ValueType;
                        if (v == ValueType.Double)
                        {
                            StreamWriter.WriteDouble(jr.DoubleValue, stream);
                        }
                        else if (v == ValueType.Long)
                        {
                            StreamWriter.WriteInt64(jr.LongValue, stream);
                        }
                        else if (v == ValueType.ULong)
                        {
                            StreamWriter.WriteUInt64(jr.ULongValue, stream);
                        }
                        else if (v == ValueType.Decimal)
                        {
                            StreamWriter.WriteString(jr.DecimalValue.ToString(CultureInfo.InvariantCulture), stream);
                        }
                        count++;
                        break;

                    case TinyJsonToken.String:
                        StreamWriter.WriteString(jr.StringValue, stream);
                        count++;
                        break;

                    case TinyJsonToken.True:
                        StreamWriter.WriteBool(true, stream);
                        count++;
                        break;

                    case TinyJsonToken.False:
                        StreamWriter.WriteBool(false, stream);
                        count++;
                        break;

                    case TinyJsonToken.Null:
                        StreamWriter.WriteNil(stream);
                        count++;
                        break;
                }
            }

            return count;
        }

        static void ToJsonCore(MsgPackStream stream, StringBuilder builder)
        {
            var code = stream.Peek();
            var type = StreamReader.GetType(stream);

            switch (type)
            {
                case FormatType.Integer:
                    builder.Append(FormatCode.IsSignedInteger(code)
                        ? StreamReader.ReadInt64(stream).ToString(CultureInfo.InvariantCulture)
                        : StreamReader.ReadUInt64(stream).ToString(CultureInfo.InvariantCulture));
                    break;

                case FormatType.Boolean:
                    builder.Append(StreamReader.ReadBool(stream) ? "true" : "false");
                    break;

                case FormatType.Float:
                    builder.Append(code == FormatCode.Float32
                        ? StreamReader.ReadSingle(stream).ToString(CultureInfo.InvariantCulture)
                        : StreamReader.ReadDouble(stream).ToString(CultureInfo.InvariantCulture));
                    break;

                case FormatType.String:
                    WriteJsonString(StreamReader.ReadString(stream), builder);
                    break;

                case FormatType.Binary:
                    builder.Append("\"" + System.Convert.ToBase64String(StreamReader.ReadBytes(stream).ToArray()) + "\"");
                    break;

                case FormatType.Array:
                    {
                        var length = StreamReader.ReadArrayHeader(stream);

                        builder.Append("[");
                        for (int i = 0; i < length; i++)
                        {
                            ToJsonCore(stream, builder);
                            if (i != length - 1)
                            {
                                builder.Append(",");
                            }
                        }
                        builder.Append("]");
                        return;
                    }

                case FormatType.Map:
                    {
                        var length = StreamReader.ReadMapHeader(stream);
                        builder.Append("{");
                        for (int i = 0; i < length; i++)
                        {
                            var keyType = StreamReader.GetType(stream);
                            if (keyType == FormatType.String || keyType == FormatType.Binary)
                            {
                                ToJsonCore(stream, builder);
                            }
                            else
                            {
                                builder.Append("\"");
                                ToJsonCore(stream, builder);
                                builder.Append("\"");
                            }

                            builder.Append(":");
                            ToJsonCore(stream, builder);

                            if (i != length - 1)
                            {
                                builder.Append(",");
                            }
                        }
                        builder.Append("}");

                        return;
                    }

                case FormatType.Extension:
                    var header = StreamReader.ReadExtensionHeader(stream);
                    if (header.TypeCode == DateTimeFormatter.TypeCode)
                    {
                        var dt = DateTimeFormatter.Unpack(stream, header.Length);
                        builder.Append("\"");
                        builder.Append(dt.ToString("o", CultureInfo.InvariantCulture));
                        builder.Append("\"");
                    }
                    else
                    {
                        builder.Append("[");
                        builder.Append(header.TypeCode);
                        builder.Append(",");
                        builder.Append("\"");
                        builder.Append(System.Convert.ToBase64String(stream.ReadBytes(header.Length).ToArray()));
                        builder.Append("\"");
                        builder.Append("]");
                    }
                    break;

                default:
                    builder.Append("null");
                    break;
            }
        }

        // escape string
        private static void WriteJsonString(string value, StringBuilder builder)
        {
            builder.Append('\"');

            for (int i = 0, count = value.Length; i < count; ++i)
            {
                var c = value[i];
                switch (c)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default: builder.Append(c); break;
                }
            }

            builder.Append('\"');
        }
    }
}
