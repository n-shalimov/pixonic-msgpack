using System.Collections.Generic;
using Analysis;
using Microsoft.CodeAnalysis;

namespace Templates
{
    public partial class FormattersTemplate
    {
        public interface IOptions
        {
            string Namespace { get; }
            string ClassName { get; }
        }

        private const string DefaultNamespace = "MsgPack.Generated";

        private static readonly HashSet<string> BuiltInTypes = new HashSet<string>
        {
            "bool", "bool?", "bool[]",
            "sbyte", "sbyte?", "sbyte[]",
            "byte", "byte?", "byte[]",
            "ushort", "ushort?", "ushort[]",
            "short", "short?", "short[]",
            "int", "int?", "int[]",
            "uint", "uint?", "uint[]",
            "long", "ulong?", "ulong[]",
            "ulong", "long?", "long[]",
            "float", "float?", "float[]",
            "double", "double?", "double[]",
            "string", "string[]",
            "System.DateTime", "System.DateTime?", "System.DateTime[]",
            "object",
            "decimal"
        };

        private readonly Collector _collector;

        private string Namespace { get; }

        private string ClassName { get; }

        private IEnumerable<string> Formatters
        {
            get
            {
                foreach (var pair in _collector.Formatters)
                {
                    if (BuiltInTypes.Contains(pair.Key.ToString()))
                    {
                        continue;
                    }

                    yield return pair.Value.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
            }
        }

        private IEnumerable<ObjectDefinition> ObjectDefinitions
            => _collector.ObjectDefinitions;

        private IEnumerable<EnumDefinition> EnumDefinitions
            => _collector.EnumDefinitions;

        public FormattersTemplate(Collector collector, IOptions options)
        {
            Namespace = options.Namespace;
            if (string.IsNullOrEmpty(Namespace))
            {
                Namespace = DefaultNamespace;
            }

            ClassName = options.ClassName;
            _collector = collector;
        }

        private static string GetFormatterName(ITypeSymbol type)
        {
            return $"{type.ContainingNamespace.ToString().Replace(".", "_")}_{type.Name}Formatter";
        }

        private static string GetFullTypeName(ITypeSymbol type)
        {
            return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
    }
}
