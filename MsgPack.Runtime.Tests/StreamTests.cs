using System.Text;
using NUnit.Framework;

namespace Pixonic.MsgPack.Tests
{
    [TestFixture]
    public class StreamTests
    {
        [Test]
        public void TestSigned()
        {
            var stream = new MsgPackStream();

            stream.WriteInt8(-11);
            Assert.AreEqual(stream.Position, 1);

            stream.WriteInt16(192);
            Assert.AreEqual(stream.Position, 3);

            stream.WriteInt32(-213);
            Assert.AreEqual(stream.Position, 7);

            stream.WriteInt64(64);
            Assert.AreEqual(stream.Position, 15);

            stream.Position = 0;
            Assert.AreEqual(-11, stream.ReadInt8());
            Assert.AreEqual(192, stream.ReadInt16());
            Assert.AreEqual(-213, stream.ReadInt32());
            Assert.AreEqual(64, stream.ReadInt64());
        }

        [Test]
        public void TestUnsigned()
        {
            var stream = new MsgPackStream();

            stream.WriteUInt8(255);
            Assert.AreEqual(stream.Position, 1);

            stream.WriteUInt16(12431);
            Assert.AreEqual(stream.Position, 3);

            stream.WriteUInt32(761);
            Assert.AreEqual(stream.Position, 7);

            stream.WriteUInt64(64);
            Assert.AreEqual(stream.Position, 15);

            stream.Position = 0;
            Assert.AreEqual(255, stream.ReadUInt8());
            Assert.AreEqual(12431, stream.ReadUInt16());
            Assert.AreEqual(761, stream.ReadUInt32());
            Assert.AreEqual(64, stream.ReadUInt64());
        }

        [Test]
        public void TestSignedMinMax()
        {
            var stream = new MsgPackStream();
            stream.WriteInt8(sbyte.MinValue);
            stream.WriteInt8(sbyte.MaxValue);
            stream.WriteInt16(short.MinValue);
            stream.WriteInt16(short.MaxValue);
            stream.WriteInt32(int.MinValue);
            stream.WriteInt32(int.MaxValue);
            stream.WriteInt64(long.MinValue);
            stream.WriteInt64(long.MaxValue);

            stream.Position = 0;
            Assert.AreEqual(sbyte.MinValue, stream.ReadInt8());
            Assert.AreEqual(sbyte.MaxValue, stream.ReadInt8());
            Assert.AreEqual(short.MinValue, stream.ReadInt16());
            Assert.AreEqual(short.MaxValue, stream.ReadInt16());
            Assert.AreEqual(int.MinValue, stream.ReadInt32());
            Assert.AreEqual(int.MaxValue, stream.ReadInt32());
            Assert.AreEqual(long.MinValue, stream.ReadInt64());
            Assert.AreEqual(long.MaxValue, stream.ReadInt64());
        }

        [Test]
        public void TestUnsignedMinMax()
        {
            var stream = new MsgPackStream();
            stream.WriteUInt8(byte.MinValue);
            stream.WriteUInt8(byte.MaxValue);
            stream.WriteUInt16(ushort.MinValue);
            stream.WriteUInt16(ushort.MaxValue);
            stream.WriteUInt32(uint.MinValue);
            stream.WriteUInt32(uint.MaxValue);
            stream.WriteUInt64(ulong.MinValue);
            stream.WriteUInt64(ulong.MaxValue);

            stream.Position = 0;
            Assert.AreEqual(byte.MinValue, stream.ReadUInt8());
            Assert.AreEqual(byte.MaxValue, stream.ReadUInt8());
            Assert.AreEqual(ushort.MinValue, stream.ReadUInt16());
            Assert.AreEqual(ushort.MaxValue, stream.ReadUInt16());
            Assert.AreEqual(uint.MinValue, stream.ReadUInt32());
            Assert.AreEqual(uint.MaxValue, stream.ReadUInt32());
            Assert.AreEqual(ulong.MinValue, stream.ReadUInt64());
            Assert.AreEqual(ulong.MaxValue, stream.ReadUInt64());
        }

        [Test]
        public void TestFloat()
        {
            var stream = new MsgPackStream();

            stream.WriteDouble(178.5);
            Assert.AreEqual(stream.Position, 8);

            stream.WriteSingle(-10.3f);
            Assert.AreEqual(stream.Position, 12);

            stream.WriteSingle(float.NaN);
            stream.WriteSingle(float.NegativeInfinity);
            stream.WriteSingle(float.PositiveInfinity);
            stream.WriteDouble(double.NaN);
            stream.WriteDouble(double.NegativeInfinity);
            stream.WriteDouble(double.PositiveInfinity);

            stream.Position = 0;
            Assert.AreEqual(178.5, stream.ReadDouble());
            Assert.AreEqual(-10.3f, stream.ReadSingle());
            Assert.IsTrue(float.IsNaN(stream.ReadSingle()));
            Assert.IsTrue(float.IsNegativeInfinity(stream.ReadSingle()));
            Assert.IsTrue(float.IsPositiveInfinity(stream.ReadSingle()));
            Assert.IsTrue(double.IsNaN(stream.ReadDouble()));
            Assert.IsTrue(double.IsNegativeInfinity(stream.ReadDouble()));
            Assert.IsTrue(double.IsPositiveInfinity(stream.ReadDouble()));
        }

        [Test]
        public void TestString()
        {
            var stream = new MsgPackStream();
            var testString = "Test string";
            var length = Encoding.UTF8.GetByteCount(testString);

            stream.WriteString(testString);
            Assert.AreEqual(stream.Position, length);

            stream.Position = 0;
            Assert.AreEqual(testString, stream.ReadString(unchecked((uint)length)));
        }

        [Test]
        public void TestBytes()
        {
            var stream = new MsgPackStream();
            var bytes = new BufferSegment(new byte[] { 1, 12, 23, 34, 45, 56 });

            stream.WriteBytes(bytes);
            Assert.AreEqual(stream.Position, bytes.Length);

            stream.Position = 0;

            Assert.True(
                BufferSegment.EqualityComparer.Equals(
                    bytes,
                    stream.ReadBytes(unchecked((uint)bytes.Length))
               )
            );
        }
    }
}
