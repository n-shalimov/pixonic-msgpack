namespace Pixonic.MsgPack.Formatters
{
    [MsgPackFormatter]
    public sealed class BoolFormatter : IFormatter<bool>
    {
        void IFormatter<bool>.Write(bool value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteBool(value, stream);
        }

        bool IFormatter<bool>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadBool(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class Int8Formatter : IFormatter<sbyte>
    {
        void IFormatter<sbyte>.Write(sbyte value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteInt8(value, stream);
        }

        sbyte IFormatter<sbyte>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadInt8(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class UInt8Formatter : IFormatter<byte>
    {
        void IFormatter<byte>.Write(byte value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteUInt8(value, stream);
        }

        byte IFormatter<byte>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadUInt8(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class Int16Formatter : IFormatter<short>
    {
        void IFormatter<short>.Write(short value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteInt16(value, stream);
        }

        short IFormatter<short>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadInt16(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class UInt16Formatter : IFormatter<ushort>
    {
        void IFormatter<ushort>.Write(ushort value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteUInt16(value, stream);
        }

        ushort IFormatter<ushort>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadUInt16(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class Int32Formatter : IFormatter<int>
    {
        void IFormatter<int>.Write(int value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteInt32(value, stream);
        }

        int IFormatter<int>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadInt32(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class UInt32Formatter : IFormatter<uint>
    {
        void IFormatter<uint>.Write(uint value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteUInt32(value, stream);
        }

        uint IFormatter<uint>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadUInt32(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class Int64Formatter : IFormatter<long>
    {
        void IFormatter<long>.Write(long value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteInt64(value, stream);
        }

        long IFormatter<long>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadInt64(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class UInt64Formatter : IFormatter<ulong>
    {
        void IFormatter<ulong>.Write(ulong value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteUInt64(value, stream);
        }

        ulong IFormatter<ulong>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadUInt64(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class SingleFormatter : IFormatter<float>
    {
        void IFormatter<float>.Write(float value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteSingle(value, stream);
        }

        float IFormatter<float>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadSingle(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class DoubleFormatter : IFormatter<double>
    {
        void IFormatter<double>.Write(double value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteDouble(value, stream);
        }

        double IFormatter<double>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadDouble(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class StringFormatter : IFormatter<string>
    {
        void IFormatter<string>.Write(string value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteString(value, stream);
        }

        string IFormatter<string>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadString(stream);
        }
    }

    [MsgPackFormatter]
    public sealed class ByteArrayFormatter : IFormatter<byte[]>
    {
        void IFormatter<byte[]>.Write(byte[] value, MsgPackStream stream, IContext context)
        {
            StreamWriter.WriteBytes(new BufferSegment(value), stream);
        }

        byte[] IFormatter<byte[]>.Read(MsgPackStream stream, IContext context)
        {
            return StreamReader.ReadBytes(stream).ToArray();
        }
    }
}
