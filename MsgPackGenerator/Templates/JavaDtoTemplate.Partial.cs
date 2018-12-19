using Analysis;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Templates
{
    public partial class JavaDtoTemplate
    {
        public interface IOptions
        {
            string Namespace { get; }
            string Package { get; }
        }

        private static readonly Dictionary<string, string> TypeMap = new Dictionary<string, string>
        {
            { "short", "short" },
            { "int", "int" },
            { "long", "long" },
            { "float", "float" },
            { "double", "double" },
            { "bool", "boolean" },
            { "byte", "char" },
            { "sbyte", "byte" },
            { "char", "char" },
            { "string", "String" },

            { "short?", "Short" },
            { "int?", "Integer" },
            { "long?", "Long" },
            { "float?", "Float" },
            { "double?", "Double" },
            { "bool?", "Boolean" },
            { "byte?", "Character" },
            { "sbyte?", "Byte" },
            { "char?", "Character" },
            { "global::System.DateTime", "java.util.Date" },

            { "decimal", null },
            { "ushort", null },
            { "uint", null },
            { "ulong", null }
        };

        private static readonly Dictionary<string, string> GenericArgumentsTypeMap = new Dictionary<string, string>
        {
            { "short", "Short" },
            { "int", "Integer" },
            { "long", "Long" },
            { "float", "Float" },
            { "double", "Double" },
            { "bool", "Boolean" },
            { "byte", "Character" },
            { "sbyte", "Byte" },
            { "char", "Character" }
        };

        private static readonly Dictionary<string, string> GenericTypeMap = new Dictionary<string, string>
        {
            { "System.Collections.Generic.List<>", "java.util.List<{0}>" },
            { "BattleMechs.Client.Message.Collection<>", "java.util.List<{0}>" },
            { "System.Collections.Generic.Dictionary<,>", "java.util.Map<{0},{1}>" },
        };

        private readonly IOptions _options;
        private readonly ObjectDefinition _definition;
        private readonly string _package;

        public readonly string Output;

        public JavaDtoTemplate(ObjectDefinition definition, IOptions options)
        {
            _options = options;
            _definition = definition;
            _package = GetPackageName(_definition.Type);

            var path = string.Join(Path.DirectorySeparatorChar, _package.Remove(0, _options.Package.Length).Split('.'));
            Output = Path.Join(path, _definition.Name) + ".java";
        }

        private static string Capitalize(string value) =>
            string.IsNullOrEmpty(value)
                ? value
                : $"{char.ToUpper(value[0])}{value.Substring(1)}";

        private string GetPackageName(ITypeSymbol typeSymbol)
        {
            var @namespace = typeSymbol.ContainingNamespace.ToDisplayString();
            if (@namespace.StartsWith(_options.Namespace, System.StringComparison.Ordinal))
            {
                return $"{_options.Package}{@namespace.Remove(0, _options.Namespace.Length).ToLower()}";
            }

            throw new System.Exception($"Wrong namespace root '{typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}'");
        }

        private string ConvertType(ITypeSymbol typeSymbol, bool genericArgument = false)
        {
            var type = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            if (genericArgument && GenericArgumentsTypeMap.TryGetValue(type, out string genericArgumentType))
            {
                return genericArgumentType;
            }

            if (TypeMap.TryGetValue(type, out string javaType))
            {
                if (javaType == null) { throw new System.Exception(); }
                return javaType;
            }

            if (typeSymbol.TypeKind == TypeKind.Array)
            {
                var arraySymbol = (IArrayTypeSymbol)typeSymbol;
                return $"java.util.List<{ConvertType(arraySymbol.ElementType, true)}>";
            }

            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType)
                {
                    var genericType = namedTypeSymbol
                        .ConstructUnboundGenericType()
                        .ToDisplayString();

                    if (!GenericTypeMap.TryGetValue(genericType, out string javaGenericType))
                    {
                        throw new System.Exception($"Unsupported type {namedTypeSymbol}");
                    }

                    return string.Format(
                        javaGenericType,
                        namedTypeSymbol.TypeArguments
                            .Select(t => ConvertType(t, true))
                            .ToArray()
                    );
                }

                return $"{GetPackageName(typeSymbol)}.{typeSymbol.Name}";
            }

            System.Console.WriteLine($"Unmapped type {typeSymbol}");

            return type;
        }
    }
}
