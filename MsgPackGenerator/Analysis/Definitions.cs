using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Analysis
{
    public class ObjectDefinition
    {
        public ITypeSymbol Type { get; set; }

        public string Name
        {
            get
            {
                return Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace(".", "_");
            }
        }

        public string FullName
        {
            get
            {
                return Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            }
        }

        public string Namespace
        {
            get { return Type.ContainingNamespace.ToDisplayString(); }
        }

        public ObjectDefinition Base { get; set; }

        public MemberDefinition[] Members { get; set; }

        public MemberDefinition[] AllMembers =>
            (Base != null
                ? Base.AllMembers
                : Enumerable.Empty<MemberDefinition>()
            ).Concat(Members).ToArray();
    }

    public class EnumDefinition
    {
        public INamedTypeSymbol Type { get; set; }

        public bool IsStringEnum { get; set; }

        public INamedTypeSymbol UnderlyingType
        {
            get { return Type.EnumUnderlyingType; }
        }
    }

    public class MemberDefinition
    {
        public string Name { get; set; }
        public string Key { get; set; }

        public ITypeSymbol Type { get; set; }

        public string TypeName { get { return Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat); } }

        public bool IsPrimitive =>
            PrimitiveTypes.Contains(Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

        private static readonly HashSet<string> PrimitiveTypes = new HashSet<string>
        {
            "short",
            "int",
            "long",
            "ushort",
            "uint",
            "ulong",
            "float",
            "double",
            "bool",
            "byte",
            "sbyte"
        };
    }
}
