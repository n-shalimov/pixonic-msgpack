MessagePack runtime and serialization formatters generation

#### Summary
[Specification](https://github.com/msgpack/msgpack/blob/master/spec.md).
The implementation presented here is intended for AoT-platforms such as Unity3D for mobile devices.

Library is divided into two parts:
- Runtime library which must be linked to your project;
- A code generator that uses your classes marked by attributes as a schema.

#### Runtime
Sample usage of built-in supported types:
```c#
var serializer = new Pixonic.MsgPack.Serializer();
byte[] bytes = serializer.Serialize("some string");
var unpackedString = serializer.Deserialize<string>(bytes)
```

#### Schema (Attributes)
Use `MsgPackObject` attribute to expose a class:
```c#
[MsgPackObject]
public class ClassToExpose
{
    public float Duration;

    [MsgPackKey("triangles")]
    public int[] Indices;

    [MsgPackIgnore]
    public bool DirtyFlag;
}
```
Instance of `ClassToExpose` will be serialized as map with
- "duration" with float32 value from `Duration` field;
  - Note that the uppercase letter at the beginning of the identifier will be changed to lower case.
- "triangles" with array of integer values from `Indices` array.
DirtyFlag will not be exposed because of `MsgPackIgnore` attribute.

To get serialization callbacks you need to implement interfaces `IBeforeSerializeListener` and/or `IAfterDeserializeListener`
- `void IBeforeSerializeListener.OnBeforeSerialize()` for writing phase (before packing);
- `void IAfterDeserializeListener.OnAfterDeserialize()` for reading phase (after unpacking).

Supported field types:
- Boolean;
- Integer: sbyte, byte, short, ushort, int, uint, long and ulong;
- Floating point: float, double, decimal;
- String;
- Raw data: byte[] (to get explicit collection of bytes use List<byte>);
- Array: plain C# arrays or System.Collections.Generic.List;
- Map: System.Collections.Generic.Dictionary;
- Timestamp: System.DateTime.

To manually extend the list above you could implement `IFormatter<T>` interface for desired type `T` and mark implementation with `MsgPackFormatter` attribute for compiler support or manually add formatter to serializer with `Serializer.RegisterFormatter`.
To add custom generic collection support you could extend `CustomArrayFormatter<TArray, TItem>` for sequenced collections and `CustomMapFormatter<TMap, TKey, TValue>` for key-value collections.

An enum value will be packed as it's underlying type by default. To change this behaviour and force enum values to serialize as strings use `MsgPackStringEnum` attribute:
```c#
[MsgPackStringEnum]
public enum StringEnum
{
    FirstValue,
    SecondValue,
    //...
}
```

#### Limitations
- Only public fields and properties could be exposed;
- Runtime library targeted to .NET 2.0 framework (for Unity3D compatibility).

#### Code generator
Usage:
```bash
dotnet mpg.dll formatters -i path-to-root-of-your-cs-files \
        -o output-directory \
        -n Desired.Namespace.CustomFormatters
```
Command above scans *.cs files in `path-to-root-of-cs-files` for `MsgPackObject` and `MsgPackFormatter` marks and outputs `output-directory/CustomFormatters.cs`.
Sample runtime usage of generated formatters:
```c#
var serializer = new Pixonic.MsgPack.Serializer();
Desired.Namespace.CustomFormatters.Register(serializer);
byte[] bytes = serializer.Serialize(new ClassToExpose { Duration = 13.0f });
...
ClassToExpose unpackedValue = serializer.Deserialize<ClassToExpose>(bytes);
...
```
