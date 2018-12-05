namespace Pixonic.MsgPack
{
    public struct ExtensionHeader
    {
        public readonly sbyte TypeCode;
        public readonly uint Length;

        public ExtensionHeader(sbyte typeCode, uint length)
        {
            TypeCode = typeCode;
            Length = length;
        }
    }
}
