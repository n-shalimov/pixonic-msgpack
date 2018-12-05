using System.Text;
using NUnit.Framework;

namespace Pixonic.MsgPack.Tests
{
    [TestFixture]
    public class ReadWriteTests
    {
        [Test]
        public void TestPrimitive()
        {
            var stream = new MsgPackStream();
            StreamWriter.WriteNil(stream);
            StreamWriter.WriteBool(true, stream);
            StreamWriter.WriteBool(false, stream);
            StreamWriter.WriteInt8(sbyte.MinValue, stream);
            StreamWriter.WriteUInt8(byte.MaxValue, stream);
            StreamWriter.WriteUInt8(17, stream);
            StreamWriter.WriteInt16(short.MaxValue, stream);
            StreamWriter.WriteUInt16(ushort.MaxValue, stream);
            StreamWriter.WriteInt32(short.MinValue, stream);
            StreamWriter.WriteUInt32(0, stream);
            StreamWriter.WriteInt64(long.MinValue, stream);
            StreamWriter.WriteUInt64(ulong.MaxValue, stream);

            stream.Position = 0;
            Assert.IsTrue(StreamReader.TryReadNil(stream));
            Assert.IsFalse(StreamReader.TryReadNil(stream));
            Assert.IsTrue(StreamReader.ReadBool(stream));
            Assert.IsFalse(StreamReader.ReadBool(stream));
            Assert.AreEqual(sbyte.MinValue, StreamReader.ReadInt8(stream));
            Assert.AreEqual(byte.MaxValue, StreamReader.ReadUInt8(stream));
            Assert.AreEqual(17, StreamReader.ReadUInt8(stream));
            Assert.AreEqual(short.MaxValue, StreamReader.ReadInt16(stream));
            Assert.AreEqual(ushort.MaxValue, StreamReader.ReadUInt16(stream));
            Assert.AreEqual(short.MinValue, StreamReader.ReadInt32(stream));
            Assert.AreEqual(0, StreamReader.ReadUInt32(stream));
            Assert.AreEqual(long.MinValue, StreamReader.ReadInt64(stream));
            Assert.AreEqual(ulong.MaxValue, StreamReader.ReadUInt64(stream));
        }

        [Test]
        public void TestIntRanges()
        {
            var stream = new MsgPackStream();
            StreamWriter.WriteUInt64(ulong.MaxValue, stream);
            StreamWriter.WriteUInt64(uint.MaxValue, stream);
            StreamWriter.WriteUInt64(ushort.MaxValue, stream);
            StreamWriter.WriteUInt64(byte.MaxValue, stream);
            StreamWriter.WriteUInt64(FormatRange.MaxFixPositiveInt, stream);
            StreamWriter.WriteInt64(long.MinValue, stream);
            StreamWriter.WriteInt64(int.MinValue, stream);
            StreamWriter.WriteInt64(short.MinValue, stream);
            StreamWriter.WriteInt64(sbyte.MinValue, stream);
            StreamWriter.WriteInt64(FormatRange.MinFixNegativeInt, stream);

            stream.Position = 0;
            Assert.AreEqual(ulong.MaxValue, StreamReader.ReadUInt64(stream));
            Assert.AreEqual(uint.MaxValue, StreamReader.ReadUInt64(stream));
            Assert.AreEqual(ushort.MaxValue, StreamReader.ReadUInt64(stream));
            Assert.AreEqual(byte.MaxValue, StreamReader.ReadUInt64(stream));
            Assert.AreEqual(FormatRange.MaxFixPositiveInt, StreamReader.ReadUInt64(stream));
            Assert.AreEqual(long.MinValue, StreamReader.ReadInt64(stream));
            Assert.AreEqual(int.MinValue, StreamReader.ReadInt64(stream));
            Assert.AreEqual(short.MinValue, StreamReader.ReadInt64(stream));
            Assert.AreEqual(sbyte.MinValue, StreamReader.ReadInt64(stream));
            Assert.AreEqual(FormatRange.MinFixNegativeInt, StreamReader.ReadInt64(stream));
        }

        [Test]
        public void TestFloat()
        {
            var stream = new MsgPackStream();
            StreamWriter.WriteSingle(-198832.1f, stream);
            StreamWriter.WriteDouble(198832.1, stream);

            stream.Position = 0;
            Assert.AreEqual(-198832.1f, StreamReader.ReadSingle(stream));
            Assert.AreEqual(198832.1, StreamReader.ReadDouble(stream));
        }

        [Test]
        public void TestString()
        {
            var stream = new MsgPackStream();
            var nullStr = new BufferSegment();
            var emptyStr = new BufferSegment(new byte[0]);
            var fixStr = CreateUtf8String(FormatRange.MaxFixStringLength);
            var smallStr = CreateUtf8String(byte.MaxValue);
            var mediumStr = CreateUtf8String(ushort.MaxValue);
            var largeStr = CreateUtf8String(ushort.MaxValue + 1);

            StreamWriter.WriteUtf8(nullStr, stream);
            StreamWriter.WriteUtf8(emptyStr, stream);
            StreamWriter.WriteUtf8(fixStr, stream);
            StreamWriter.WriteUtf8(smallStr, stream);
            StreamWriter.WriteUtf8(mediumStr, stream);
            StreamWriter.WriteUtf8(largeStr, stream);

            stream.Position = 0;
            Assert.AreEqual(nullStr, StreamReader.ReadUtf8(stream));
            Assert.AreEqual(emptyStr, StreamReader.ReadUtf8(stream));
            Assert.AreEqual(fixStr, StreamReader.ReadUtf8(stream));
            Assert.AreEqual(smallStr, StreamReader.ReadUtf8(stream));
            Assert.AreEqual(mediumStr, StreamReader.ReadUtf8(stream));
            Assert.AreEqual(largeStr, StreamReader.ReadUtf8(stream));
        }

        [Test]
        public void TestUtf8String()
        {
            var stream = new MsgPackStream();
            var utf8String = new BufferSegment(Encoding.UTF8.GetBytes("utf8 test string"));

            StreamWriter.WriteUtf8(utf8String, stream);
            StreamWriter.WriteString("test string", stream);

            stream.Position = 0;
            Assert.AreEqual("utf8 test string", StreamReader.ReadString(stream));
            Assert.AreEqual("test string", StreamReader.ReadString(stream));

            stream.Position = 0;
            Assert.AreEqual(utf8String, StreamReader.ReadUtf8(stream));
            Assert.AreEqual(new BufferSegment(Encoding.UTF8.GetBytes("test string")), StreamReader.ReadUtf8(stream));
        }

        [Test]
        public void TestBinary()
        {
            var stream = new MsgPackStream();
            var nullArray = new BufferSegment();
            var emptyArray = new BufferSegment(new byte[0]);
            var smallArray = CreateByteArray(byte.MaxValue / 2);
            var mediumArray = CreateByteArray(short.MaxValue / 2);
            var largeArray = CreateByteArray(ushort.MaxValue);

            StreamWriter.WriteBytes(nullArray, stream);
            StreamWriter.WriteBytes(emptyArray, stream);
            StreamWriter.WriteBytes(smallArray, stream);
            StreamWriter.WriteBytes(mediumArray, stream);
            StreamWriter.WriteBytes(largeArray, stream);

            stream.Position = 0;
            Assert.AreEqual(nullArray, StreamReader.ReadBytes(stream));
            Assert.AreEqual(emptyArray, StreamReader.ReadBytes(stream));
            Assert.AreEqual(smallArray, StreamReader.ReadBytes(stream));
            Assert.AreEqual(mediumArray, StreamReader.ReadBytes(stream));
            Assert.AreEqual(largeArray, StreamReader.ReadBytes(stream));
        }

        [Test]
        public void TestArrayHeader()
        {
            var stream = new MsgPackStream();
            StreamWriter.WriteArrayHeader(5, stream);
            StreamWriter.WriteArrayHeader(150, stream);
            StreamWriter.WriteArrayHeader(uint.MaxValue, stream);

            stream.Position = 0;
            Assert.AreEqual(5, StreamReader.ReadArrayHeader(stream));
            Assert.AreEqual(150, StreamReader.ReadArrayHeader(stream));
            Assert.AreEqual(uint.MaxValue, StreamReader.ReadArrayHeader(stream));
        }

        [Test]
        public void TestMapHeader()
        {
            var stream = new MsgPackStream();
            StreamWriter.WriteMapHeader(15, stream);
            StreamWriter.WriteMapHeader(16, stream);
            StreamWriter.WriteMapHeader(uint.MaxValue, stream);

            stream.Position = 0;
            Assert.AreEqual(15, StreamReader.ReadMapHeader(stream));
            Assert.AreEqual(16, StreamReader.ReadMapHeader(stream));
            Assert.AreEqual(uint.MaxValue, StreamReader.ReadMapHeader(stream));
        }

        [Test]
        public void TestExtensionHeader()
        {
            var stream = new MsgPackStream();
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(-6, 0), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(-5, 1), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(-4, 2), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(-3, 4), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(-2, 8), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(-1, 16), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(0, byte.MaxValue), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(1, ushort.MaxValue), stream);
            StreamWriter.WriteExtensionHeader(new ExtensionHeader(2, uint.MaxValue), stream);

            stream.Position = 0;
            var ext0 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(-6, ext0.TypeCode);
            Assert.AreEqual(0, ext0.Length);
            var ext1 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(-5, ext1.TypeCode);
            Assert.AreEqual(1, ext1.Length);
            var ext2 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(-4, ext2.TypeCode);
            Assert.AreEqual(2, ext2.Length);
            var ext3 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(-3, ext3.TypeCode);
            Assert.AreEqual(4, ext3.Length);
            var ext4 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(-2, ext4.TypeCode);
            Assert.AreEqual(8, ext4.Length);
            var ext5 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(-1, ext5.TypeCode);
            Assert.AreEqual(16, ext5.Length);
            var ext6 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(0, ext6.TypeCode);
            Assert.AreEqual(byte.MaxValue, ext6.Length);
            var ext7 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(1, ext7.TypeCode);
            Assert.AreEqual(ushort.MaxValue, ext7.Length);
            var ext8 = StreamReader.ReadExtensionHeader(stream);
            Assert.AreEqual(2, ext8.TypeCode);
            Assert.AreEqual(uint.MaxValue, ext8.Length);
        }

        private BufferSegment CreateByteArray(int length)
        {
            var bytes = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                bytes[i] = unchecked((byte)i);
            }

            return new BufferSegment(bytes);
        }

        private BufferSegment CreateUtf8String(int length)
        {
            var begin = 0x41; // A
            var end = 0x5a; // Z
            var bytes = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                bytes[i] = unchecked((byte)(begin + i % (end - begin)));
            }

            return new BufferSegment(bytes);
        }
    }
}
