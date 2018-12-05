namespace Pixonic.MsgPack
{
    public static class StreamReader
    {
        public static bool TryReadNil(MsgPackStream stream)
        {
            if (stream.Peek() == FormatCode.Nil)
            {
                stream.Skip(1);
                return true;
            }

            return false;
        }

        public static bool ReadBool(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (code)
            {
                case FormatCode.False: return false;
                case FormatCode.True: return true;
                default: throw new MsgPackCodeException(code);
            }
        }

        public static sbyte ReadInt8(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt:
                case FormatMask.NegativeFixInt:
                    return unchecked((sbyte)code);
            }
            
            switch (code)
            {
                case FormatCode.Int8: return stream.ReadInt8();
                case FormatCode.UInt8: return checked((sbyte)stream.ReadUInt8());
                default: throw new MsgPackCodeException(code);
            }
        }

        public static byte ReadUInt8(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.PositiveFixInt)
            {
                return code;
            }

            switch (code)
            {
                case FormatCode.UInt8: return stream.ReadUInt8();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static short ReadInt16(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return code;
                case FormatMask.NegativeFixInt: return unchecked((sbyte)code);
            }

            switch (code)
            {
                case FormatCode.Int8: return stream.ReadInt8();
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.Int16: return stream.ReadInt16();
                case FormatCode.UInt16: return checked((short)stream.ReadUInt16());
                default: throw new MsgPackCodeException(code);
            }
        }

        public static ushort ReadUInt16(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.PositiveFixInt)
            {
                return code;
            }

            switch (code)
            {
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.UInt16: return stream.ReadUInt16();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static int ReadInt32(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return code;
                case FormatMask.NegativeFixInt: return unchecked((sbyte)code);
            }

            switch (code)
            {
                case FormatCode.Int8: return stream.ReadInt8();
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.Int16: return stream.ReadInt16();
                case FormatCode.UInt16: return stream.ReadUInt16();
                case FormatCode.Int32: return stream.ReadInt32();
                case FormatCode.UInt32: return checked((int)stream.ReadUInt32());
                default: throw new MsgPackCodeException(code);
            }
        }

        public static uint ReadUInt32(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.PositiveFixInt)
            {
                return code;
            }

            switch (code)
            {
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.UInt16: return stream.ReadUInt16();
                case FormatCode.UInt32: return stream.ReadUInt32();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static long ReadInt64(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return code;
                case FormatMask.NegativeFixInt: return unchecked((sbyte)code);
            }

            switch (code)
            {
                case FormatCode.Int8: return stream.ReadInt8();
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.Int16: return stream.ReadInt16();
                case FormatCode.UInt16: return stream.ReadUInt16();
                case FormatCode.Int32: return stream.ReadInt32();
                case FormatCode.UInt32: return stream.ReadUInt32();
                case FormatCode.Int64: return stream.ReadInt64();
                case FormatCode.UInt64: return checked((long)stream.ReadUInt64());
                default: throw new MsgPackCodeException(code);
            }
        }

        public static ulong ReadUInt64(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.PositiveFixInt)
            {
                return code;
            }

            switch (code)
            {
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.UInt16: return stream.ReadUInt16();
                case FormatCode.UInt32: return stream.ReadUInt32();
                case FormatCode.UInt64: return stream.ReadUInt64();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static float ReadSingle(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return code;
                case FormatMask.NegativeFixInt: return unchecked((sbyte)code);
            }

            switch (code)
            {
                case FormatCode.Float32: return stream.ReadSingle();
                case FormatCode.Int8: return stream.ReadInt8();
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.Int16: return stream.ReadInt16();
                case FormatCode.UInt16: return stream.ReadUInt16();
                case FormatCode.Int32: return stream.ReadInt32();
                case FormatCode.UInt32: return stream.ReadUInt32();
                case FormatCode.Int64: return stream.ReadInt64();
                case FormatCode.UInt64: return stream.ReadUInt64();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static double ReadDouble(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return code;
                case FormatMask.NegativeFixInt: return unchecked((sbyte)code);
            }

            switch (code)
            {
                case FormatCode.Float64: return stream.ReadDouble();
                case FormatCode.Float32: return stream.ReadSingle();
                case FormatCode.Int8: return stream.ReadInt8();
                case FormatCode.UInt8: return stream.ReadUInt8();
                case FormatCode.Int16: return stream.ReadInt16();
                case FormatCode.UInt16: return stream.ReadUInt16();
                case FormatCode.Int32: return stream.ReadInt32();
                case FormatCode.UInt32: return stream.ReadUInt32();
                case FormatCode.Int64: return stream.ReadUInt64();
                case FormatCode.UInt64: return stream.ReadUInt64();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static string ReadString(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.FixString)
            {
                return stream.ReadString(code & 0x1Fu);
            }

            switch (code)
            {
                case FormatCode.Str8: return stream.ReadString(stream.ReadUInt8());
                case FormatCode.Str16: return stream.ReadString(stream.ReadUInt16());
                case FormatCode.Str32: return stream.ReadString(stream.ReadUInt32());
                case FormatCode.Nil: return null;
                default: throw new MsgPackCodeException(code);
            }
        }

        public static BufferSegment ReadUtf8(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.FixString)
            {
                return stream.ReadBytes(code & 0x1Fu);
            }

            switch (code)
            {
                case FormatCode.Str8: return stream.ReadBytes(stream.ReadUInt8());
                case FormatCode.Str16: return stream.ReadBytes(stream.ReadUInt16());
                case FormatCode.Str32: return stream.ReadBytes(stream.ReadUInt32());
                case FormatCode.Nil: return new BufferSegment();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static BufferSegment ReadBytes(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (code)
            {
                case FormatCode.Bin8: return stream.ReadBytes(stream.ReadUInt8());
                case FormatCode.Bin16: return stream.ReadBytes(stream.ReadUInt16());
                case FormatCode.Bin32: return stream.ReadBytes(stream.ReadUInt32());
                case FormatCode.Nil: return new BufferSegment();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static uint ReadArrayHeader(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.FixArray)
            {
                return code & 0xfu;
            }

            switch (code)
            {
                case FormatCode.Array16: return stream.ReadUInt16();
                case FormatCode.Array32: return stream.ReadUInt32();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static uint ReadMapHeader(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            if (FormatMask.Match(code) == FormatMask.FixMap)
            {
                return code & 0xfu;
            }

            switch (code)
            {
                case FormatCode.Map16: return stream.ReadUInt16();
                case FormatCode.Map32: return stream.ReadUInt32();
                default: throw new MsgPackCodeException(code);
            }
        }

        public static ExtensionHeader ReadExtensionHeader(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();
            uint length;

            switch (code)
            {
                case FormatCode.FixExt1: length = 1u; break;
                case FormatCode.FixExt2: length = 2u; break;
                case FormatCode.FixExt4: length = 4u; break;
                case FormatCode.FixExt8: length = 8u; break;
                case FormatCode.FixExt16: length = 16u; break;
                case FormatCode.Ext8: length = stream.ReadUInt8(); break;
                case FormatCode.Ext16: length = stream.ReadUInt16(); break;
                case FormatCode.Ext32: length = stream.ReadUInt32(); break;
                default: throw new MsgPackCodeException(code);
            }

            return new ExtensionHeader(stream.ReadInt8(), length);
        }

        public static void Skip(MsgPackStream stream)
        {
            var code = stream.ReadUInt8();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return;
                case FormatMask.NegativeFixInt: return;
                case FormatMask.FixString: stream.Skip(code & 0x1Fu); return;
                case FormatMask.FixArray: SkipElements(stream, code & 0xFu); return;
                case FormatMask.FixMap: SkipElements(stream, 2u * (code & 0xFu)); return;
            }

            switch (code)
            {
                case FormatCode.Nil: return;
                case FormatCode.False: return;
                case FormatCode.True: return;
                case FormatCode.Int8: stream.Skip(1u); break;
                case FormatCode.UInt8: stream.Skip(1u); break;
                case FormatCode.Int16: stream.Skip(2u); break;
                case FormatCode.UInt16: stream.Skip(2u); break;
                case FormatCode.Int32: stream.Skip(4u); break;
                case FormatCode.UInt32: stream.Skip(4u); break;
                case FormatCode.Int64: stream.Skip(8u); break;
                case FormatCode.UInt64: stream.Skip(8u); break;
                case FormatCode.Float32: stream.Skip(4u); break;
                case FormatCode.Float64: stream.Skip(8u); break;
                case FormatCode.Str8: stream.Skip(stream.ReadUInt8()); break;
                case FormatCode.Str16: stream.Skip(stream.ReadUInt16()); break;
                case FormatCode.Str32: stream.Skip(stream.ReadUInt32()); break;
                case FormatCode.Bin8: stream.Skip(stream.ReadUInt8()); break;
                case FormatCode.Bin16: stream.Skip(stream.ReadUInt16()); break;
                case FormatCode.Bin32: stream.Skip(stream.ReadUInt32()); break;
                case FormatCode.Array16: SkipElements(stream, stream.ReadUInt16()); break;
                case FormatCode.Array32: SkipElements(stream, stream.ReadUInt32()); break;
                case FormatCode.Map16: SkipElements(stream, 2u * stream.ReadUInt16()); break;
                case FormatCode.Map32: SkipElements(stream, 2u * stream.ReadUInt32()); break;
                case FormatCode.FixExt1: stream.Skip(2u); break;
                case FormatCode.FixExt2: stream.Skip(3u); break;
                case FormatCode.FixExt4: stream.Skip(5u); break;
                case FormatCode.FixExt8: stream.Skip(9u); break;
                case FormatCode.FixExt16: stream.Skip(17u); break;
                case FormatCode.Ext8: stream.Skip(1u + stream.ReadUInt8()); break;
                case FormatCode.Ext16: stream.Skip(1u + stream.ReadUInt16()); break;
                case FormatCode.Ext32: stream.Skip(1u + stream.ReadUInt32()); break;
                default: throw new MsgPackCodeException(code);
            }
        }

        public static FormatType GetType(MsgPackStream stream)
        {
            var code = stream.Peek();

            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return FormatType.Integer;
                case FormatMask.NegativeFixInt: return FormatType.Integer;
                case FormatMask.FixString: return FormatType.String;
                case FormatMask.FixArray: return FormatType.Array;
                case FormatMask.FixMap: return FormatType.Map;
            }

            switch (code)
            {
                case FormatCode.Nil: return FormatType.Nil;
                case FormatCode.NeverUsed: return FormatType.Unknown;
                case FormatCode.False: return FormatType.Boolean;
                case FormatCode.True: return FormatType.Boolean;
                case FormatCode.Int8: return FormatType.Integer;
                case FormatCode.UInt8: return FormatType.Integer;
                case FormatCode.Int16: return FormatType.Integer;
                case FormatCode.UInt16: return FormatType.Integer;
                case FormatCode.Int32: return FormatType.Integer;
                case FormatCode.UInt32: return FormatType.Integer;
                case FormatCode.Int64: return FormatType.Integer;
                case FormatCode.UInt64: return FormatType.Integer;
                case FormatCode.Float32: return FormatType.Float;
                case FormatCode.Float64: return FormatType.Float;
                case FormatCode.Str8: return FormatType.String;
                case FormatCode.Str16: return FormatType.String;
                case FormatCode.Str32: return FormatType.String;
                case FormatCode.Bin8: return FormatType.Binary;
                case FormatCode.Bin16: return FormatType.Binary;
                case FormatCode.Bin32: return FormatType.Binary;
                case FormatCode.Array16: return FormatType.Array;
                case FormatCode.Array32: return FormatType.Array;
                case FormatCode.Map16: return FormatType.Map;
                case FormatCode.Map32: return FormatType.Map;
                case FormatCode.FixExt1: return FormatType.Extension;
                case FormatCode.FixExt2: return FormatType.Extension;
                case FormatCode.FixExt4: return FormatType.Extension;
                case FormatCode.FixExt8: return FormatType.Extension;
                case FormatCode.FixExt16: return FormatType.Extension;
                case FormatCode.Ext8: return FormatType.Extension;
                case FormatCode.Ext16: return FormatType.Extension;
                case FormatCode.Ext32: return FormatType.Extension;
                default: return FormatType.Unknown;
            }
        }

        private static void SkipElements(MsgPackStream stream, uint count)
        {
            for (uint i = 0; i < count; ++i)
            {
                Skip(stream);
            }
        }
    }
}
