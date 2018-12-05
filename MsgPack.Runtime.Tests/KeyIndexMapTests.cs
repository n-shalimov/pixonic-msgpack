using System.Text;
using NUnit.Framework;

namespace Pixonic.MsgPack.Tests
{
    [TestFixture]
    public class KeyIndexMapTests
    {
        private static readonly string[] Keys = { "key1", "key2", "key3" };
        private static readonly BufferSegment[] Utf8Keys =
        {
            new BufferSegment(Encoding.UTF8.GetBytes(Keys[0])),
            new BufferSegment(Encoding.UTF8.GetBytes(Keys[1])),
            new BufferSegment(Encoding.UTF8.GetBytes(Keys[2])),
            new BufferSegment(Encoding.UTF8.GetBytes("extraKey"))
        };

        [Test]
        public void TestKeys()
        {
            var map = new KeyIndexMap(Keys);

            Assert.AreEqual(Utf8Keys[0], map[0]);
            Assert.AreEqual(Utf8Keys[1], map[1]);
            Assert.AreEqual(Utf8Keys[2], map[2]);
            Assert.Throws<System.IndexOutOfRangeException>(() => { var key = map[-1]; });
            Assert.Throws<System.IndexOutOfRangeException>(() => { var key = map[3]; });
        }

        [Test]
        public void TestIndices()
        {
            var map = new KeyIndexMap(Keys);

            int index;
            Assert.IsTrue(map.TryGetIndex(Utf8Keys[0], out index));
            Assert.AreEqual(0, index);
            Assert.IsTrue(map.TryGetIndex(Utf8Keys[2], out index));
            Assert.AreEqual(2, index);
            Assert.IsFalse(map.TryGetIndex(Utf8Keys[3], out index));
        }
    }
}
