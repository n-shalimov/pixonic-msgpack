namespace Pixonic.MsgPack
{
    public class MsgPackException : System.Exception
    {
        public MsgPackException(string format, params object[] args)
            : base(string.Format(format, args))
        {}
    }

    public sealed class MsgPackSerializationException : System.Exception
    {
        public string Trace { get; private set; }

        public MsgPackSerializationException(string message, string trace) : base(message)
        {
            Trace = trace;
        }
    }

    public sealed class MsgPackCodeException : MsgPackException
    {
        public MsgPackCodeException(byte code)
            : base("Code is invalid. Format mame: {0}", CodeToString(code))
        {}

        private static string CodeToString(byte code)
        {
            switch (FormatMask.Match(code))
            {
                case FormatMask.PositiveFixInt: return "positive fixint";
                case FormatMask.NegativeFixInt: return "negative fixint";
                case FormatMask.FixString: return "fixstr";
                case FormatMask.FixArray: return "fixarray";
                case FormatMask.FixMap: return "fixmap";
            }

            switch (code)
            {
                case FormatCode.Nil: return "nil";
                case FormatCode.False: return "bool (false)";
                case FormatCode.True: return "bool (true)";
                case FormatCode.Int8: return "int 8";
                case FormatCode.UInt8: return "uint 8";
                case FormatCode.Int16: return "int 16";
                case FormatCode.UInt16: return "uint 16";
                case FormatCode.Int32: return "int 32";
                case FormatCode.UInt32: return "uint 32";
                case FormatCode.Int64: return "int 64";
                case FormatCode.UInt64: return "uint 64";
                case FormatCode.Float32: return "float 32";
                case FormatCode.Float64: return "float 64";
                case FormatCode.Str8: return "str 8";
                case FormatCode.Str16: return "str 16";
                case FormatCode.Str32: return "str 32";
                case FormatCode.Bin8: return "bin 8";
                case FormatCode.Bin16: return "bin 16";
                case FormatCode.Bin32: return "bin 32";
                case FormatCode.Array16: return "array 16";
                case FormatCode.Array32: return "array 32";
                case FormatCode.Map16: return "map 16";
                case FormatCode.Map32: return "map 32";
                case FormatCode.FixExt1: return "fixext 1";
                case FormatCode.FixExt2: return "fixext 2";
                case FormatCode.FixExt4: return "fixext 4";
                case FormatCode.FixExt8: return "fixext 8";
                case FormatCode.FixExt16: return "fixext 16";
                case FormatCode.Ext8: return "ext 8";
                case FormatCode.Ext16: return "ext 16";
                case FormatCode.Ext32: return "ext 32";
                default: return string.Format("unknown 0x{0:x02}", code);
            }
        }
    }

    public sealed class MsgPackExtensionException : MsgPackException
    {
        public MsgPackExtensionException(sbyte actualCode, sbyte expectedCode)
            : base("Extension type code is invalid. Actual: {0}, expected: {1}", actualCode, expectedCode)
        {}
    }
}
