namespace Pixonic.MsgPack.Formatters
{
    public sealed class EnumStringFormatter<TEnum> : IFormatter<TEnum>
        where TEnum : struct
    {
        private readonly KeyIndexMap _keyIndexMap = new KeyIndexMap(System.Enum.GetNames(typeof(TEnum)));
        private readonly TEnum[] _values = (TEnum[])System.Enum.GetValues(typeof(TEnum));

        void IFormatter<TEnum>.Write(TEnum value, MsgPackStream stream, IContext context)
        {
            var index = System.Array.IndexOf(_values, value);

            if (index >= 0)
            {
                StreamWriter.WriteUtf8(_keyIndexMap[index], stream);
            }
            else
            {
                StreamWriter.WriteString(value.ToString(), stream);
            }
        }

        TEnum IFormatter<TEnum>.Read(MsgPackStream stream, IContext context)
        {
            var key = StreamReader.ReadUtf8(stream);
            int index;
            if (_keyIndexMap.TryGetIndex(key, out index))
            {
                return _values[index];
            }

            try
            {
                return (TEnum)System.Enum.Parse(typeof(TEnum), key.ToString());
            }
            catch (System.Exception ex)
            {
                throw new MsgPackException(ex.Message);
            }
        }
    }
}
