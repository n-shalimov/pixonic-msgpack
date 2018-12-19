using System;

namespace MessagePack
{
    [Obsolete("Use Pixonic.MsgPack.MsgPackObjectAttribute instead")]
    [AttributeUsage(AttributeTargets.Class/* | AttributeTargets.Struct*/, AllowMultiple = false, Inherited = true)]
    public class MessagePackObjectAttribute : Attribute
    {
        public bool KeyAsPropertyName { get; private set; }

        public MessagePackObjectAttribute(bool keyAsPropertyName = false)
        {
            this.KeyAsPropertyName = keyAsPropertyName;
        }
    }

    [Obsolete("Use Pixonic.MsgPack.MsgPackKeyAttribute instead")]
    [AttributeUsage(/*AttributeTargets.Property | */AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public string StringKey { get; private set; }

        public KeyAttribute(string x)
        {
            this.StringKey = x;
        }
    }

    [Obsolete("Use Pixonic.MsgPack.MsgPackIgnoreAttribute instead")]
    [AttributeUsage(/*AttributeTargets.Property | */AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
