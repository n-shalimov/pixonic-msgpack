namespace Pixonic.MsgPack.Formatters
{
    public static class BuiltInFormatters
    {
        public static void Register(Serializer serializer)
        {
            Register(serializer, new BoolFormatter());
            Register(serializer, new Int8Formatter());
            Register(serializer, new Int16Formatter());
            Register(serializer, new UInt16Formatter());
            Register(serializer, new Int32Formatter());
            Register(serializer, new UInt32Formatter());
            Register(serializer, new Int64Formatter());
            Register(serializer, new UInt64Formatter());
            Register(serializer, new SingleFormatter());
            Register(serializer, new DoubleFormatter());
            Register(serializer, new DateTimeFormatter());

            var byteFormatter = new UInt8Formatter();
            serializer.RegisterFormatter(byteFormatter);
            serializer.RegisterFormatter(new NullableFormatter<byte>(byteFormatter));
            serializer.RegisterFormatter(new ListFormatter<byte>(byteFormatter));
            serializer.RegisterFormatter(new ByteArrayFormatter());

            var stringFormatter = new StringFormatter();
            serializer.RegisterFormatter(stringFormatter);
            serializer.RegisterFormatter(new ArrayFormatter<string>(stringFormatter));
            serializer.RegisterFormatter(new ListFormatter<string>(stringFormatter));

            // "dynamic" types
            Register(serializer, new DecimalFormatter());

            var objectFormatter = new ObjectFormatter();
            serializer.RegisterFormatter(objectFormatter);
            serializer.RegisterFormatter(new ArrayFormatter<object>(objectFormatter));
            serializer.RegisterFormatter(new ListFormatter<object>(objectFormatter));
        }

        private static void Register<T>(Serializer serializer, IFormatter<T> baseFormatter)
            where T : struct
        {
            serializer.RegisterFormatter(baseFormatter);
            serializer.RegisterFormatter(new NullableFormatter<T>(baseFormatter));
            serializer.RegisterFormatter(new ArrayFormatter<T>(baseFormatter));
            serializer.RegisterFormatter(new ListFormatter<T>(baseFormatter));
        }
    }
}
