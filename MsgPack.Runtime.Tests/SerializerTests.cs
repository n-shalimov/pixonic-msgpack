using System.Collections.Generic;
using NUnit.Framework;

namespace Pixonic.MsgPack.Tests
{
    [TestFixture]
    public class SerializerTests
    {
        [Test]
        public void TestPrimitives()
        {
            var serializer = new Serializer();

            var bytes = serializer.Serialize(true);
            Assert.IsTrue(serializer.Deserialize<bool>(bytes));

            bytes = serializer.Serialize(byte.MaxValue);
            Assert.AreEqual(byte.MaxValue, serializer.Deserialize<byte>(bytes));

            bytes = serializer.Serialize(sbyte.MinValue);
            Assert.AreEqual(sbyte.MinValue, serializer.Deserialize<sbyte>(bytes));

            bytes = serializer.Serialize(uint.MaxValue);
            Assert.AreEqual(uint.MaxValue, serializer.Deserialize<uint>(bytes));

            bytes = serializer.Serialize(int.MinValue);
            Assert.AreEqual(int.MinValue, serializer.Deserialize<int>(bytes));

            bytes = serializer.Serialize(ulong.MaxValue);
            Assert.AreEqual(ulong.MaxValue, serializer.Deserialize<ulong>(bytes));

            bytes = serializer.Serialize(long.MinValue);
            Assert.AreEqual(long.MinValue, serializer.Deserialize<long>(bytes));
        }

        [Test]
        public void TestFloat()
        {
            var serializer = new Serializer();

            var bytes = serializer.Serialize(0.5f);
            Assert.AreEqual(0.5f, serializer.Deserialize<float>(bytes));

            bytes = serializer.Serialize(77.13);
            Assert.AreEqual(77.13, serializer.Deserialize<double>(bytes));
        }

        [Test]
        public void TestString()
        {
            var serializer = new Serializer();

            var bytes = serializer.Serialize("test string");
            Assert.AreEqual("test string", serializer.Deserialize<string>(bytes));
        }

        [Test]
        public void TestBinary()
        {
            var serializer = new Serializer();
            var value = new byte[] { 0, 1, 2, 3, 4, 5 };

            var bytes = serializer.Serialize(value);
            Assert.AreEqual(value, serializer.Deserialize<byte[]>(bytes));
        }

        [Test]
        public void TestArray()
        {
            var serializer = new Serializer();
            var floats = new float[] { 0.1f, 1.2f, 2.3f, 3.4f, 4.5f, 5.6f, 6.7f };
            var ints = new int[] { 8, 90, -73, 255, 0, 14, 15 };

            var bytes = serializer.Serialize(floats);
            Assert.AreEqual(floats, serializer.Deserialize<float[]>(bytes));

            bytes = serializer.Serialize(ints);
            Assert.AreEqual(ints, serializer.Deserialize<int[]>(bytes));
        }

        [Test]
        public void TestList()
        {
            var serializer = new Serializer();
            var strings = new List<string> { "aa", "b", "ccc", "", "dd", null, "ffff" };
            var bools = new List<bool> { true, true, false, true, false, false, false, true };

            var bytes = serializer.Serialize(strings);
            Assert.AreEqual(strings, serializer.Deserialize<List<string>>(bytes));

            bytes = serializer.Serialize(bools);
            Assert.AreEqual(bools, serializer.Deserialize<List<bool>>(bytes));
        }

        [Test]
        public void TestNullable()
        {
            var serializer = new Serializer();

            short? v1 = 2;
            var bytes = serializer.Serialize(v1);
            Assert.AreEqual(2, serializer.Deserialize<short?>(bytes));

            short? v2 = null;
            bytes = serializer.Serialize(v2);
            Assert.AreEqual(null, serializer.Deserialize<short?>(bytes));
        }

        [Test]
        public void TestDateTime()
        {
            var serializer = new Serializer();
            var now = System.DateTime.Now;

            var bytes = serializer.Serialize(System.DateTime.MinValue);
            Assert.AreEqual(System.DateTime.MinValue, serializer.Deserialize<System.DateTime>(bytes));

            bytes = serializer.Serialize(System.DateTime.MaxValue);
            Assert.AreEqual(System.DateTime.MaxValue, serializer.Deserialize<System.DateTime>(bytes));

            bytes = serializer.Serialize(now);
            Assert.AreEqual(now, serializer.Deserialize<System.DateTime>(bytes));
        }

        [Test]
        public void TestObject()
        {
            var serializer = new Serializer();

            var boolObject = (object)true;
            var bytes = serializer.Serialize(boolObject);
            Assert.AreEqual(boolObject, serializer.Deserialize<object>(bytes));

            var intObject = (object)19;
            bytes = serializer.Serialize(intObject);
            Assert.AreEqual(intObject, serializer.Deserialize<object>(bytes));

            var doubleObject = (object)10032.72;
            bytes = serializer.Serialize(doubleObject);
            Assert.AreEqual(doubleObject, serializer.Deserialize<object>(bytes));

            var dateTimeObject = (object)System.DateTime.FromBinary(1234562);
            bytes = serializer.Serialize(dateTimeObject);
            Assert.AreEqual(dateTimeObject, serializer.Deserialize<object>(bytes));

            var array = new object[] { intObject, dateTimeObject, boolObject };
            bytes = serializer.Serialize(array);
            var unpackedArray = serializer.Deserialize<object>(bytes);
            Assert.IsInstanceOf<object[]>(unpackedArray);

            array = (object[])unpackedArray;
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(intObject, array[0]);
            Assert.AreEqual(dateTimeObject, array[1]);
            Assert.AreEqual(boolObject, array[2]);
        }
    }
}
