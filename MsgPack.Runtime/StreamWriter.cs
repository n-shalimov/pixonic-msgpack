using System.Text;

namespace Pixonic.MsgPack
{
    public static class StreamWriter
    {
        public static void WriteNil(MsgPackStream stream)
        {
            stream.WriteUInt8(FormatCode.Nil);
        }

        public static void WriteBool(bool value, MsgPackStream stream)
        {
            stream.WriteUInt8(value ? FormatCode.True : FormatCode.False);
        }

        public static void WriteInt8(sbyte value, MsgPackStream stream)
        {
            if (value < FormatRange.MinFixNegativeInt)
            {
                stream.WriteUInt8(FormatCode.Int8);
            }

            stream.WriteInt8(value);
        }

        public static void WriteUInt8(byte value, MsgPackStream stream)
        {
            if (value > FormatCode.MaxFixInt)
            {
                stream.WriteUInt8(FormatCode.UInt8);
            }

            stream.WriteUInt8(value);
        }

        public static void WriteInt16(short value, MsgPackStream stream)
        {
            if (value >= 0)
            {
                WriteUInt16(unchecked((ushort)value), stream);
                return;
            }

            if (value >= sbyte.MinValue)
            {
                WriteInt8(unchecked((sbyte)value), stream);
                return;
            }

            stream.WriteUInt8(FormatCode.Int16);
            stream.WriteInt16(value);
        }

        public static void WriteUInt16(ushort value, MsgPackStream stream)
        {
            if (value <= byte.MaxValue)
            {
                WriteUInt8(unchecked((byte)value), stream);
                return;
            }

            stream.WriteUInt8(FormatCode.UInt16);
            stream.WriteUInt16(value);
        }

        public static void WriteInt32(int value, MsgPackStream stream)
        {
            if (value >= 0)
            {
                WriteUInt32(unchecked((uint)value), stream);
                return;
            }

            if (value >= short.MinValue)
            {
                WriteInt16(unchecked((short)value), stream);
                return;
            }

            stream.WriteUInt8(FormatCode.Int32);
            stream.WriteInt32(value);
        }

        public static void WriteUInt32(uint value, MsgPackStream stream)
        {
            if (value <= ushort.MaxValue)
            {
                WriteUInt16(unchecked((ushort)value), stream);
                return;
            }

            stream.WriteUInt8(FormatCode.UInt32);
            stream.WriteUInt32(value);
        }

        public static void WriteInt64(long value, MsgPackStream stream)
        {
            if (value >= 0)
            {
                WriteUInt64(unchecked((ulong)value), stream);
                return;
            }

            if (value >= int.MinValue)
            {
                WriteInt32(unchecked((int)value), stream);
                return;
            }

            stream.WriteUInt8(FormatCode.Int64);
            stream.WriteInt64(value);
        }

        public static void WriteUInt64(ulong value, MsgPackStream stream)
        {
            if (value <= uint.MaxValue)
            {
                WriteUInt32(unchecked((uint)value), stream);
                return;
            }

            stream.WriteUInt8(FormatCode.UInt64);
            stream.WriteUInt64(value);
        }

        public static void WriteSingle(float value, MsgPackStream stream)
        {
            stream.WriteUInt8(FormatCode.Float32);
            stream.WriteSingle(value);
        }

        public static void WriteDouble(double value, MsgPackStream stream)
        {
            stream.WriteUInt8(FormatCode.Float64);
            stream.WriteDouble(value);
        }

        public static void WriteString(string value, MsgPackStream stream)
        {
            if (value == null)
            {
                stream.WriteUInt8(FormatCode.Nil);
                return;
            }

            var length = Encoding.UTF8.GetByteCount(value);

            if (length <= FormatRange.MaxFixStringLength)
            {
                stream.WriteUInt8(unchecked((byte)(FormatCode.MinFixStr | length)));
            }
            else if (length <= byte.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Str8);
                stream.WriteUInt8(unchecked((byte)length));
            }
            else if (length <= ushort.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Str16);
                stream.WriteUInt16(unchecked((ushort)length));
            }
            else
            {
                stream.WriteUInt8(FormatCode.Str32);
                stream.WriteInt32(length);
            }

            stream.WriteString(value);
        }

        public static void WriteUtf8(BufferSegment value, MsgPackStream stream)
        {
            if (value.Array == null)
            {
                stream.WriteUInt8(FormatCode.Nil);
                return;
            }

            var length = value.Length;

            if (length <= FormatRange.MaxFixStringLength)
            {
                stream.WriteUInt8(unchecked((byte)(FormatCode.MinFixStr | length)));
            }
            else if (length <= byte.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Str8);
                stream.WriteUInt8(unchecked((byte)length));
            }
            else if (length <= ushort.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Str16);
                stream.WriteUInt16(unchecked((ushort)length));
            }
            else if (length < uint.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Str32);
                stream.WriteUInt32(unchecked((uint)length));
            }
            else
            {
                throw new MsgPackException("Max string length exceeded");
            }

            stream.WriteBytes(value);
        }

        public static void WriteBytes(BufferSegment value, MsgPackStream stream)
        {
            if (value.Array == null)
            {
                stream.WriteUInt8(FormatCode.Nil);
                return;
            }

            var length = value.Length;

            if (length <= byte.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Bin8);
                stream.WriteUInt8(unchecked((byte)length));
            }
            else if (length <= ushort.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Bin16);
                stream.WriteUInt16(unchecked((ushort)length));
            }
            else if (length < uint.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Bin32);
                stream.WriteUInt32(unchecked((uint)length));
            }
            else
            {
                throw new MsgPackException("Max bin length exceeded");
            }

            stream.WriteBytes(value);
        }

        public static void WriteArrayHeader(uint length, MsgPackStream stream)
        {
            if (length <= FormatRange.MaxFixArrayCount)
            {
                stream.WriteUInt8(unchecked((byte)(FormatCode.MinFixArray | length)));
            }
            else if (length <= ushort.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Array16);
                stream.WriteUInt16(unchecked((ushort)length));
            }
            else
            {
                stream.WriteUInt8(FormatCode.Array32);
                stream.WriteUInt32(length);
            }
        }

        public static void WriteMapHeader(uint length, MsgPackStream stream)
        {
            if (length <= FormatRange.MaxFixMapCount)
            {
                stream.WriteUInt8(unchecked((byte)(FormatCode.MinFixMap | length)));
            }
            else if (length <= ushort.MaxValue)
            {
                stream.WriteUInt8(FormatCode.Map16);
                stream.WriteUInt16(unchecked((ushort)length));
            }
            else
            {
                stream.WriteUInt8(FormatCode.Map32);
                stream.WriteUInt32(length);
            }
        }

        public static void WriteExtensionHeader(ExtensionHeader header, MsgPackStream stream)
        {
            switch (header.Length)
            {
                case 1: stream.WriteUInt8(FormatCode.FixExt1); break;
                case 2: stream.WriteUInt8(FormatCode.FixExt2); break;
                case 4: stream.WriteUInt8(FormatCode.FixExt4); break;
                case 8: stream.WriteUInt8(FormatCode.FixExt8); break;
                case 16: stream.WriteUInt8(FormatCode.FixExt16); break;
                default:
                    if (header.Length <= byte.MaxValue)
                    {
                        stream.WriteUInt8(FormatCode.Ext8);
                        stream.WriteUInt8(unchecked((byte)header.Length));
                    }
                    else if (header.Length <= ushort.MaxValue)
                    {
                        stream.WriteUInt8(FormatCode.Ext16);
                        stream.WriteUInt16(unchecked((ushort)header.Length));
                    }
                    else
                    {
                        stream.WriteUInt8(FormatCode.Ext32);
                        stream.WriteUInt32(header.Length);
                    }

                    break;
            }

            stream.WriteInt8(header.TypeCode);
        }
    }
}
