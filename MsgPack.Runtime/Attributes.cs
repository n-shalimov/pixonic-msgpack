using System;

namespace Pixonic.MsgPack
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MsgPackObjectAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MsgPackKeyAttribute : Attribute
    {
        public string Key { get; private set; }

        public MsgPackKeyAttribute(string key)
        {
            Key = key;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MsgPackIgnoreAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MsgPackFormatterForAttribute : Attribute
    {
        public Type Type { get; private set; }

        public MsgPackFormatterForAttribute(Type type)
        {
            Type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MsgPackFormatterAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class MsgPackStringEnumAttribute : Attribute {}
}
