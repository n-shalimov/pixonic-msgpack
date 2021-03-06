﻿namespace Pixonic.MsgPack
{
    /// <summary>
    /// https://github.com/msgpack/msgpack/blob/master/spec.md#serialization-type-to-format-conversion
    /// </summary>
    public enum FormatType : byte
    {
        Unknown = 0,

        Integer = 1,
        Nil = 3,
        Boolean = 4,
        Float = 5,
        String = 6,
        Binary = 7,
        Array = 8,
        Map = 9,
        Extension = 10
    }

    public static class FormatMask
    {
        public const byte PositiveFixInt = 0x7f;
        public const byte NegativeFixInt = 0xe0;
        public const byte FixString = 0xa0;
        public const byte FixArray = 0x90;
        public const byte FixMap = 0x80;

        public static byte Match(byte code)
        {
            if ((code & 0x80) == 0) { return PositiveFixInt; }
            if ((code & 0xe0) == NegativeFixInt) { return NegativeFixInt; }
            if ((code & 0xe0) == FixString) { return FixString; }
            if ((code & 0xf0) == FixArray) { return FixArray; }
            if ((code & 0xf0) == FixMap) { return FixMap; }

            return 0;
        }
    }

    /// <summary>
    /// https://github.com/msgpack/msgpack/blob/master/spec.md#overview
    /// </summary>
    public static class FormatCode
    {
        public const byte MinFixInt = 0x00; // 0
        public const byte MaxFixInt = 0x7f; // 127
        public const byte MinFixMap = 0x80; // 128
        public const byte MaxFixMap = 0x8f; // 143
        public const byte MinFixArray = 0x90; // 144
        public const byte MaxFixArray = 0x9f; // 159
        public const byte MinFixStr = 0xa0; // 160
        public const byte MaxFixStr = 0xbf; // 191
        public const byte Nil = 0xc0;
        public const byte NeverUsed = 0xc1;
        public const byte False = 0xc2;
        public const byte True = 0xc3;
        public const byte Bin8 = 0xc4;
        public const byte Bin16 = 0xc5;
        public const byte Bin32 = 0xc6;
        public const byte Ext8 = 0xc7;
        public const byte Ext16 = 0xc8;
        public const byte Ext32 = 0xc9;
        public const byte Float32 = 0xca;
        public const byte Float64 = 0xcb;
        public const byte UInt8 = 0xcc;
        public const byte UInt16 = 0xcd;
        public const byte UInt32 = 0xce;
        public const byte UInt64 = 0xcf;
        public const byte Int8 = 0xd0;
        public const byte Int16 = 0xd1;
        public const byte Int32 = 0xd2;
        public const byte Int64 = 0xd3;
        public const byte FixExt1 = 0xd4;
        public const byte FixExt2 = 0xd5;
        public const byte FixExt4 = 0xd6;
        public const byte FixExt8 = 0xd7;
        public const byte FixExt16 = 0xd8;
        public const byte Str8 = 0xd9;
        public const byte Str16 = 0xda;
        public const byte Str32 = 0xdb;
        public const byte Array16 = 0xdc;
        public const byte Array32 = 0xdd;
        public const byte Map16 = 0xde;
        public const byte Map32 = 0xdf;
        public const byte MinNegativeFixInt = 0xe0; // 224
        public const byte MaxNegativeFixInt = 0xff; // 255

        public static bool IsSignedInteger(byte intCode)
        {
            return (intCode & 0xf0) == 0xd0
                || (intCode & 0xe0) == FormatMask.NegativeFixInt;
        }
    }

    public static class FormatRange
    {
        public const int MinFixNegativeInt = -32;
        public const int MaxFixNegativeInt = -1;
        public const int MaxFixPositiveInt = 127;
        public const int MinFixStringLength = 0;
        public const int MaxFixStringLength = 31;
        public const int MaxFixMapCount = 15;
        public const int MaxFixArrayCount = 15;
    }
}
