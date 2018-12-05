using NUnit.Framework;

namespace Pixonic.MsgPack.Tests
{
    [TestFixture]
    public class BufferSegmentTests
    {
        private static readonly byte[] Bytes = { 0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4 };

        [Test]
        public void TestEquality()
        {
            var segment1 = new BufferSegment(Bytes, 0, 4);
            var segment2 = new BufferSegment(Bytes, 0, 5);
            var segment3 = new BufferSegment(Bytes);
            var segment4 = new BufferSegment(Bytes, 6, 4);
            var segment5 = new BufferSegment();
            var segment6 = new BufferSegment();

            Assert.AreNotEqual(segment1, segment2);
            Assert.AreNotEqual(segment1, segment3);
            Assert.AreNotEqual(segment1, segment5);
            Assert.AreEqual(segment1, segment1);
            Assert.AreEqual(segment1, segment4);
            Assert.AreEqual(segment5, segment6);
        }

        [Test]
        public void TestHashCode()
        {
            var segment1 = new BufferSegment(Bytes, 0, 4);
            var segment2 = new BufferSegment(Bytes);
            var segment3 = new BufferSegment(Bytes, 6, 4);
            var segment4 = new BufferSegment();
            var segment5 = new BufferSegment();

            Assert.AreNotEqual(segment1.GetHashCode(), segment2.GetHashCode());
            Assert.AreNotEqual(segment1.GetHashCode(), segment4.GetHashCode());
            Assert.AreEqual(segment1.GetHashCode(), segment3.GetHashCode());
            Assert.AreEqual(segment1.GetHashCode(), segment1.GetHashCode());
            Assert.AreEqual(segment4.GetHashCode(), segment5.GetHashCode());
        }
    }
}
