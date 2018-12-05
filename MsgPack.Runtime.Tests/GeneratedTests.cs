using System.Collections.Generic;
using NUnit.Framework;

namespace Pixonic.MsgPack.Tests
{
    [TestFixture]
    public class GeneratedTests
    {
        public enum ValueEnum : short
        {
            FirstValue, SecondValue, ThirdValue
        }

        public enum StringEnum
        {
            FirstStringValue, SecondStringValue
        }

        [Test]
        public void TestFlat()
        {
            var serializer = new Serializer();
            GeneratedFormatters.Register(serializer);

            var message = new FlatMessage
            {
                Field1 = "test string",
                Field2 = -199,
                Field3 = null,
                Field4 = 24.6f,
                Field5 = new Dictionary<int, System.DateTime>
                {
                    { 1, System.DateTime.MaxValue },
                    { 8, System.DateTime.MinValue },
                    { -6, System.DateTime.FromBinary(134111234211) }
                },
                Field6 = ValueEnum.ThirdValue,
                Field7 = StringEnum.FirstStringValue
            };

            var bytes = serializer.Serialize(message);
            Assert.IsTrue(message.BeforeSerializeCalled);

            var restoredMessage = serializer.Deserialize<FlatMessage>(bytes);
            Assert.IsTrue(restoredMessage.AfterDeserializeCalled);

            Assert.AreEqual(message.Field1, restoredMessage.Field1);
            Assert.AreEqual(message.Field2, restoredMessage.Field2);
            Assert.AreEqual(message.Field3, restoredMessage.Field3);
            Assert.AreEqual(message.Field4, restoredMessage.Field4);
            Assert.AreEqual(3, restoredMessage.Field5.Count);
            Assert.AreEqual(System.DateTime.MaxValue, restoredMessage.Field5[1]);
            Assert.AreEqual(System.DateTime.MinValue, restoredMessage.Field5[8]);
            Assert.AreEqual(System.DateTime.FromBinary(134111234211), restoredMessage.Field5[-6]);
            Assert.AreEqual(message.Field6, restoredMessage.Field6);
            Assert.AreEqual(message.Field7, restoredMessage.Field7);
        }

        public class FlatMessage : IAfterDeserializeListener, IBeforeSerializeListener
        {
            public string Field1;
            public int Field2;
            public bool? Field3;
            public float Field4;
            public Dictionary<int, System.DateTime> Field5;
            public ValueEnum Field6;
            public StringEnum Field7;

            public bool AfterDeserializeCalled { get; private set; }
            public bool BeforeSerializeCalled { get; private set; }

            void IAfterDeserializeListener.OnAfterDeserialize()
            {
                AfterDeserializeCalled = true;
            }

            void IBeforeSerializeListener.OnBeforeSerialize()
            {
                BeforeSerializeCalled = true;
            }
        }

        public static class GeneratedFormatters
        {
            public static void Register(Serializer serializer)
            {
                serializer.RegisterFormatter(new Formatters.DictionaryFormatter<int, System.DateTime>());

                serializer.RegisterFormatter(new ValueEnumFormatter());
                serializer.RegisterFormatter(new Formatters.EnumStringFormatter<StringEnum>());

                serializer.RegisterFormatter(new FlatMessageFormatter());
            }
        }

        public sealed class ValueEnumFormatter : IFormatter<ValueEnum>
        {
            void IFormatter<ValueEnum>.Write(ValueEnum value, MsgPackStream stream, IContext context)
            {
                context.ResolveFormatter<short>().Write((short)value, stream, context);
            }

            ValueEnum IFormatter<ValueEnum>.Read(MsgPackStream stream, IContext context)
            {
                return (ValueEnum)context.ResolveFormatter<short>().Read(stream, context);
            }
        }

        public sealed class FlatMessageFormatter : IFormatter<FlatMessage>
        {
            private static readonly KeyIndexMap KeyIndexMap = new KeyIndexMap(
                "field1",
                "field2",
                "field3",
                "field4",
                "field5",
                "field6",
                "field7"
            );

            void IFormatter<FlatMessage>.Write(FlatMessage value, MsgPackStream stream, IContext context)
            {
                if (value == null)
                {
                    StreamWriter.WriteNil(stream);
                    return;
                }

                var beforeSerializeListener = value as IBeforeSerializeListener;
                if (beforeSerializeListener != null)
                {
                    beforeSerializeListener.OnBeforeSerialize();
                }

                StreamWriter.WriteMapHeader(7, stream);

                StreamWriter.WriteUtf8(KeyIndexMap[0], stream);
                context.ResolveFormatter<string>().Write(value.Field1, stream, context);

                StreamWriter.WriteUtf8(KeyIndexMap[1], stream);
                context.ResolveFormatter<int>().Write(value.Field2, stream, context);

                StreamWriter.WriteUtf8(KeyIndexMap[2], stream);
                context.ResolveFormatter<bool?>().Write(value.Field3, stream, context);

                StreamWriter.WriteUtf8(KeyIndexMap[3], stream);
                context.ResolveFormatter<float>().Write(value.Field4, stream, context);

                StreamWriter.WriteUtf8(KeyIndexMap[4], stream);
                context.ResolveFormatter<Dictionary<int, System.DateTime>>().Write(value.Field5, stream, context);

                StreamWriter.WriteUtf8(KeyIndexMap[5], stream);
                context.ResolveFormatter<ValueEnum>().Write(value.Field6, stream, context);

                StreamWriter.WriteUtf8(KeyIndexMap[6], stream);
                context.ResolveFormatter<StringEnum>().Write(value.Field7, stream, context);
            }

            FlatMessage IFormatter<FlatMessage>.Read(MsgPackStream stream, IContext context)
            {
                if (StreamReader.TryReadNil(stream))
                {
                    return null;
                }

                var __value0__ = default(string);
                var __value1__ = default(int);
                var __value2__ = default(bool?);
                var __value3__ = default(float);
                var __value4__ = default(Dictionary<int, System.DateTime>);
                var __value5__ = default(ValueEnum);
                var __value6__ = default(StringEnum);

                context.Trace("FlatMessage header");
                var length = StreamReader.ReadMapHeader(stream);
                for (var i = 0; i < length; ++i)
                {
                    context.Trace("FlatMessage next");
                    var key = StreamReader.ReadUtf8(stream);
                    int index;
                    if (!KeyIndexMap.TryGetIndex(key, out index))
                    {
                        StreamReader.Skip(stream);
                        continue;
                    }

                    switch (index)
                    {
                        case 0:
                            context.Trace("FlatMessage::Field1");
                            __value0__ = context.ResolveFormatter<string>().Read(stream, context);
                            break;

                        case 1:
                            context.Trace("FlatMessage::Field2");
                            __value1__ = context.ResolveFormatter<int>().Read(stream, context);
                            break;

                        case 2:
                            context.Trace("FlatMessage::Field3");
                            __value2__ = context.ResolveFormatter<bool?>().Read(stream, context);
                            break;

                        case 3:
                            context.Trace("FlatMessage::Field4");
                            __value3__ = context.ResolveFormatter<float>().Read(stream, context);
                            break;

                        case 4:
                            context.Trace("FlatMessage::Field5");
                            __value4__ = context.ResolveFormatter<Dictionary<int, System.DateTime>>().Read(stream, context);
                            break;

                        case 5:
                            context.Trace("FlatMessage::Field6");
                            __value5__ = context.ResolveFormatter<ValueEnum>().Read(stream, context);
                            break;

                        case 6:
                            context.Trace("FlatMessage::Field7");
                            __value6__ = context.ResolveFormatter<StringEnum>().Read(stream, context);
                            break;

                        default:
                            StreamReader.Skip(stream);
                            break;
                    }
                }

                var __result__ = new FlatMessage();
                __result__.Field1 = __value0__;
                __result__.Field2 = __value1__;
                __result__.Field3 = __value2__;
                __result__.Field4 = __value3__;
                __result__.Field5 = __value4__;
                __result__.Field6 = __value5__;
                __result__.Field7 = __value6__;

                var afterDeserializeListener = __result__ as IAfterDeserializeListener;
                if (afterDeserializeListener != null)
                {
                    afterDeserializeListener.OnAfterDeserialize();
                }

                return __result__;
            }
        }
    }
}
