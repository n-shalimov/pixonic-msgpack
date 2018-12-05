
namespace BattleMechs.Client
{
    public static class GeneratedFormatters
    {
        public static void Register(global::Pixonic.MsgPack.Serializer serializer)
        {
            serializer.RegisterFormatter(new global::Pixonic.MsgPack.Formatters.DictionaryFormatter<int, global::System.DateTime>());
            serializer.RegisterFormatter(new global::Pixonic.MsgPack.Formatters.ArrayFormatter<global::BattleMechs.Messages.InnerMessage>());

            serializer.RegisterFormatter(new BattleMechs_Messages_OneTwoThreeFormatter());

            serializer.RegisterFormatter(new BattleMechs_Messages_InnerMessageFormatter());
            serializer.RegisterFormatter(new BattleMechs_Messages_MessageFormatter());
        }
        
        private sealed class BattleMechs_Messages_OneTwoThreeFormatter
            : global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.OneTwoThree>
        {
            void global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.OneTwoThree>.Write(
                global::BattleMechs.Messages.OneTwoThree value,
                global::Pixonic.MsgPack.MsgPackStream stream,
                global::Pixonic.MsgPack.IContext context)
            {
                context.ResolveFormatter<int>().Write((int)value, stream, context);
            }
            
            global::BattleMechs.Messages.OneTwoThree global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.OneTwoThree>.Read(
                global::Pixonic.MsgPack.MsgPackStream stream,
                global::Pixonic.MsgPack.IContext context)
            {
                return (global::BattleMechs.Messages.OneTwoThree)context.ResolveFormatter<int>().Read(stream, context);
            }
        }

        private sealed class BattleMechs_Messages_InnerMessageFormatter
            : global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.InnerMessage>
        {
            private static readonly KeyIndexMap KeyIndexMap = new KeyIndexMap(
                "x",
                "oneTwoThree"
            );

            void global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.InnerMessage>.Write(
                global::BattleMechs.Messages.InnerMessage value,
                global::Pixonic.MsgPack.MsgPackStream stream,
                global::Pixonic.MsgPack.IContext context)
            {
                if (value == null)
                {
                    global::Pixonic.MsgPack.StreamWriter.WriteNil(stream);
                    return;
                }
                
                var beforeSerializeListener = value as global::Pixonic.MsgPack.IBeforeSerializeListener;
                if (beforeSerializeListener != null)
                {
                    beforeSerializeListener.OnBeforeSerialize();
                }

                context.Trace("global::BattleMechs.Messages.InnerMessage header>");
                global::Pixonic.MsgPack.StreamWriter.WriteMapHeader(2, stream);

                context.Trace("global::BattleMechs.Messages.InnerMessage::X key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[0], stream);
                context.Trace("global::BattleMechs.Messages.InnerMessage::X value");
                context.ResolveFormatter<float>().Write(value.X, stream, context);

                context.Trace("global::BattleMechs.Messages.InnerMessage::OneTwoThree key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[1], stream);
                context.Trace("global::BattleMechs.Messages.InnerMessage::OneTwoThree value");
                context.ResolveFormatter<global::BattleMechs.Messages.OneTwoThree>().Write(value.OneTwoThree, stream, context);
            }

            global::BattleMechs.Messages.InnerMessage global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.InnerMessage>.Read(
                global::Pixonic.MsgPack.MsgPackStream stream,
                global::Pixonic.MsgPack.IContext context)
            {
                if (global::Pixonic.MsgPack.StreamReader.TryReadNil(stream))
                {
                    return null;
                }
                
                var __value0__ = default(float);
                var __value1__ = default(global::BattleMechs.Messages.OneTwoThree);

                context.Trace("global::BattleMechs.Messages.InnerMessage header");
                var length = global::Pixonic.MsgPack.StreamReader.ReadMapHeader(stream);
                for (var i = 0; i < length; ++i)
                {
                    context.Trace("global::BattleMechs.Messages.InnerMessage next");
                    var key = global::Pixonic.MsgPack.StreamReader.ReadUtf8(stream);
                    int index;
                    if (!KeyIndexMap.TryGetIndex(key, out index))
                    {
                        global::Pixonic.MsgPack.StreamReader.Skip(stream);
                        continue;
                    }

                    switch (index)
                    {
                        case 0:
                            context.Trace("global::BattleMechs.Messages.InnerMessage::X>");
                            __value0__ = context.ResolveFormatter<float>().Read(stream, context);
                            break;

                        case 1:
                            context.Trace("global::BattleMechs.Messages.InnerMessage::OneTwoThree>");
                            __value1__ = context.ResolveFormatter<global::BattleMechs.Messages.OneTwoThree>().Read(stream, context);
                            break;

                        default:
                            global::Pixonic.MsgPack.StreamReader.Skip(stream);
                            break;
                    }
                }

                var __result__ = new global::BattleMechs.Messages.InnerMessage();
                __result__.X = __value0__;
                __result__.OneTwoThree = __value1__;

                var afterDeserializeListener = __result__ as global::Pixonic.MsgPack.IAfterDeserializeListener;
                if (afterDeserializeListener != null)
                {
                    afterDeserializeListener.OnAfterDeserialize();
                }

                return __result__;
            }
        }

        private sealed class BattleMechs_Messages_MessageFormatter
            : global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.Message>
        {
            private static readonly KeyIndexMap KeyIndexMap = new KeyIndexMap(
                "times",
                "innerMessages",
                "floatValue",
                "nullableFloatValue",
                "bytes",
                "itemList",
                "secondItemList"
            );

            void global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.Message>.Write(
                global::BattleMechs.Messages.Message value,
                global::Pixonic.MsgPack.MsgPackStream stream,
                global::Pixonic.MsgPack.IContext context)
            {
                if (value == null)
                {
                    global::Pixonic.MsgPack.StreamWriter.WriteNil(stream);
                    return;
                }
                
                var beforeSerializeListener = value as global::Pixonic.MsgPack.IBeforeSerializeListener;
                if (beforeSerializeListener != null)
                {
                    beforeSerializeListener.OnBeforeSerialize();
                }

                context.Trace("global::BattleMechs.Messages.Message header>");
                global::Pixonic.MsgPack.StreamWriter.WriteMapHeader(7, stream);

                context.Trace("global::BattleMechs.Messages.Message::Times key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[0], stream);
                context.Trace("global::BattleMechs.Messages.Message::Times value");
                context.ResolveFormatter<global::System.Collections.Generic.Dictionary<int, global::System.DateTime>>().Write(value.Times, stream, context);

                context.Trace("global::BattleMechs.Messages.Message::InnerMessages key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[1], stream);
                context.Trace("global::BattleMechs.Messages.Message::InnerMessages value");
                context.ResolveFormatter<global::BattleMechs.Messages.InnerMessage[]>().Write(value.InnerMessages, stream, context);

                context.Trace("global::BattleMechs.Messages.Message::FloatValue key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[2], stream);
                context.Trace("global::BattleMechs.Messages.Message::FloatValue value");
                context.ResolveFormatter<float>().Write(value.FloatValue, stream, context);

                context.Trace("global::BattleMechs.Messages.Message::NullableFloatValue key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[3], stream);
                context.Trace("global::BattleMechs.Messages.Message::NullableFloatValue value");
                context.ResolveFormatter<float?>().Write(value.NullableFloatValue, stream, context);

                context.Trace("global::BattleMechs.Messages.Message::Bytes key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[4], stream);
                context.Trace("global::BattleMechs.Messages.Message::Bytes value");
                context.ResolveFormatter<byte[]>().Write(value.Bytes, stream, context);

                context.Trace("global::BattleMechs.Messages.Message::ItemList key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[5], stream);
                context.Trace("global::BattleMechs.Messages.Message::ItemList value");
                context.ResolveFormatter<string[]>().Write(value.ItemList, stream, context);

                context.Trace("global::BattleMechs.Messages.Message::SecondItemList key");
                global::Pixonic.MsgPack.StreamWriter.WriteUtf8(KeyIndexMap[6], stream);
                context.Trace("global::BattleMechs.Messages.Message::SecondItemList value");
                context.ResolveFormatter<string[]>().Write(value.SecondItemList, stream, context);
            }

            global::BattleMechs.Messages.Message global::Pixonic.MsgPack.IFormatter<global::BattleMechs.Messages.Message>.Read(
                global::Pixonic.MsgPack.MsgPackStream stream,
                global::Pixonic.MsgPack.IContext context)
            {
                if (global::Pixonic.MsgPack.StreamReader.TryReadNil(stream))
                {
                    return null;
                }
                
                var __value0__ = default(global::System.Collections.Generic.Dictionary<int, global::System.DateTime>);
                var __value1__ = default(global::BattleMechs.Messages.InnerMessage[]);
                var __value2__ = default(float);
                var __value3__ = default(float?);
                var __value4__ = default(byte[]);
                var __value5__ = default(string[]);
                var __value6__ = default(string[]);

                context.Trace("global::BattleMechs.Messages.Message header");
                var length = global::Pixonic.MsgPack.StreamReader.ReadMapHeader(stream);
                for (var i = 0; i < length; ++i)
                {
                    context.Trace("global::BattleMechs.Messages.Message next");
                    var key = global::Pixonic.MsgPack.StreamReader.ReadUtf8(stream);
                    int index;
                    if (!KeyIndexMap.TryGetIndex(key, out index))
                    {
                        global::Pixonic.MsgPack.StreamReader.Skip(stream);
                        continue;
                    }

                    switch (index)
                    {
                        case 0:
                            context.Trace("global::BattleMechs.Messages.Message::Times>");
                            __value0__ = context.ResolveFormatter<global::System.Collections.Generic.Dictionary<int, global::System.DateTime>>().Read(stream, context);
                            break;

                        case 1:
                            context.Trace("global::BattleMechs.Messages.Message::InnerMessages>");
                            __value1__ = context.ResolveFormatter<global::BattleMechs.Messages.InnerMessage[]>().Read(stream, context);
                            break;

                        case 2:
                            context.Trace("global::BattleMechs.Messages.Message::FloatValue>");
                            __value2__ = context.ResolveFormatter<float>().Read(stream, context);
                            break;

                        case 3:
                            context.Trace("global::BattleMechs.Messages.Message::NullableFloatValue>");
                            __value3__ = context.ResolveFormatter<float?>().Read(stream, context);
                            break;

                        case 4:
                            context.Trace("global::BattleMechs.Messages.Message::Bytes>");
                            __value4__ = context.ResolveFormatter<byte[]>().Read(stream, context);
                            break;

                        case 5:
                            context.Trace("global::BattleMechs.Messages.Message::ItemList>");
                            __value5__ = context.ResolveFormatter<string[]>().Read(stream, context);
                            break;

                        case 6:
                            context.Trace("global::BattleMechs.Messages.Message::SecondItemList>");
                            __value6__ = context.ResolveFormatter<string[]>().Read(stream, context);
                            break;

                        default:
                            global::Pixonic.MsgPack.StreamReader.Skip(stream);
                            break;
                    }
                }

                var __result__ = new global::BattleMechs.Messages.Message();
                __result__.Times = __value0__;
                __result__.InnerMessages = __value1__;
                __result__.FloatValue = __value2__;
                __result__.NullableFloatValue = __value3__;
                __result__.Bytes = __value4__;
                __result__.ItemList = __value5__;
                __result__.SecondItemList = __value6__;

                var afterDeserializeListener = __result__ as global::Pixonic.MsgPack.IAfterDeserializeListener;
                if (afterDeserializeListener != null)
                {
                    afterDeserializeListener.OnAfterDeserialize();
                }

                return __result__;
            }
        }
    }
}
