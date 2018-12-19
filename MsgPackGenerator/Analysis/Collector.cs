using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Analysis
{
    public class Collector
    {
        private static readonly HashSet<string> BuiltInTypes = new HashSet<string>
        {
            "bool", "sbyte", "byte", "ushort", "short", "int", "uint",
            "long", "ulong", "float", "double", "string", "System.DateTime",
            "object", "decimal"
        };

        private readonly INamedTypeSymbol _objectAttributeType;
        private readonly INamedTypeSymbol _keyAttributeType;
        private readonly INamedTypeSymbol _ignoreAttributeType;
        private readonly INamedTypeSymbol _formatterAttributeType;
        private readonly INamedTypeSymbol _stringEnumAttribute;

        private readonly HashSet<ITypeSymbol> _collected = new HashSet<ITypeSymbol>();

        private readonly List<ObjectDefinition> _collectedObjects = new List<ObjectDefinition>();
        private readonly List<EnumDefinition> _collectedEnums = new List<EnumDefinition>();

        private readonly Dictionary<int, INamedTypeSymbol> _genericArrayFormatters = new Dictionary<int, INamedTypeSymbol>();
        private readonly Dictionary<INamedTypeSymbol, INamedTypeSymbol> _genericFromatters = new Dictionary<INamedTypeSymbol, INamedTypeSymbol>();

        private readonly Dictionary<ITypeSymbol, INamedTypeSymbol> _formatters = new Dictionary<ITypeSymbol, INamedTypeSymbol>();

        private static readonly SymbolDisplayFormat ShortNameDisplayFormat = new SymbolDisplayFormat(
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly
        );

        public IReadOnlyDictionary<ITypeSymbol, INamedTypeSymbol> Formatters => _formatters;

        public IEnumerable<ObjectDefinition> ObjectDefinitions => _collectedObjects;

        public IEnumerable<EnumDefinition> EnumDefinitions => _collectedEnums;

        public Collector(Compilation compilation)
        {
            _objectAttributeType = compilation.GetTypeByMetadataName("Pixonic.MsgPack.MsgPackObjectAttribute");
            _keyAttributeType = compilation.GetTypeByMetadataName("Pixonic.MsgPack.MsgPackKeyAttribute");
            _ignoreAttributeType = compilation.GetTypeByMetadataName("Pixonic.MsgPack.MsgPackIgnoreAttribute");
            _formatterAttributeType = compilation.GetTypeByMetadataName("Pixonic.MsgPack.MsgPackFormatterAttribute");
            _stringEnumAttribute = compilation.GetTypeByMetadataName("Pixonic.MsgPack.MsgPackStringEnumAttribute");

            var baseFormatter = compilation.GetTypeByMetadataName("Pixonic.MsgPack.IFormatter`1");

            foreach (var formatter in GetClasses(compilation, _formatterAttributeType))
            {
                var genericBase = formatter.AllInterfaces.FirstOrDefault(i => i.OriginalDefinition == baseFormatter);
                if (genericBase == null)
                {
                    continue;
                }

                var targetType = genericBase.TypeArguments[0];

                if (targetType is IArrayTypeSymbol arrayTargetType)
                {
                    var orig = targetType.OriginalDefinition;
                    var elementType = arrayTargetType.ElementType;
                    if (elementType.TypeKind == TypeKind.TypeParameter)
                    {
                        _genericArrayFormatters[arrayTargetType.Rank] = formatter;
                    }
                    else
                    {
                        _formatters[arrayTargetType] = formatter;
                    }
                }
                else if (targetType is INamedTypeSymbol namedTargetType)
                {
                    if (namedTargetType.IsGenericType)
                    {
                        _genericFromatters[namedTargetType.OriginalDefinition] = formatter;
                    }
                    else
                    {
                        _formatters[namedTargetType] = formatter;
                    }
                }
            }

            foreach (var message in GetClasses(compilation, _objectAttributeType))
            {
                Collect(message);
            }
        }

        private IEnumerable<INamedTypeSymbol> GetClasses(Compilation compilation, INamedTypeSymbol attributeType) =>
            compilation.SyntaxTrees
                .Select(st => compilation.GetSemanticModel(st))
                .SelectMany(sm => sm.SyntaxTree
                    .GetRoot()
                    .DescendantNodes()
                    .Select(n => sm.GetDeclaredSymbol(n))
                    .Where(n => n != null)
                    .OfType<INamedTypeSymbol>()
                    .Where(IsPublic)
                    .Where(s => s.TypeKind == TypeKind.Class && !s.IsAbstract)
                    .Where(s => GetAttribute(s, attributeType) != null)
                );

        private bool Collect(ITypeSymbol symbol)
        {
            if (_collected.Contains(symbol))
            {
                return true;
            }

            var typeSymbol = symbol as INamedTypeSymbol;
            if (BuiltInTypes.Contains(symbol.ToString()))
            {
                return true;
            }

            if (symbol.TypeKind == TypeKind.Array)
            {
                return CollectArray((IArrayTypeSymbol)symbol);
            }

            if (!IsPublic(symbol))
            {
                return false;
            }

            if (symbol.TypeKind == TypeKind.Enum)
            {
                return CollectEnum(typeSymbol);
            }

            if (typeSymbol.IsGenericType)
            {
                return CollectGeneric(typeSymbol);
            }

            if (typeSymbol.Locations[0].IsInMetadata)
            {
                return false;
            }

            return CollectObject(typeSymbol);
        }

        private bool CollectArray(IArrayTypeSymbol arraySymbol)
        {
            var elementType = arraySymbol.ElementType;
            Collect(elementType);

            if (!_formatters.TryGetValue(arraySymbol, out INamedTypeSymbol formatter)
                && _genericArrayFormatters.TryGetValue(arraySymbol.Rank, out formatter))
            {
                formatter = formatter.Construct(elementType);
                _formatters.Add(arraySymbol, formatter);
            }

            if (formatter == null)
            {
                throw new InvalidOperationException($"Not supported array type {arraySymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}");
            }

            _collected.Add(arraySymbol);
            return true;
        }

        private bool CollectEnum(INamedTypeSymbol symbol)
        {
            _collectedEnums.Add(new EnumDefinition
            {
                Type = symbol,
                IsStringEnum = GetAttribute(symbol, _stringEnumAttribute) != null,
            });

            _collected.Add(symbol);
            return true;
        }

        private bool CollectGeneric(INamedTypeSymbol symbol)
        {
            if (!_formatters.TryGetValue(symbol, out INamedTypeSymbol formatter)
                && _genericFromatters.TryGetValue(symbol.OriginalDefinition, out formatter))
            {
                formatter = formatter.Construct(symbol.TypeArguments.ToArray());
                _formatters.Add(symbol, formatter);
            }

            if (formatter == null)
            {
                throw new Exception($"Unable to find suitable generic formatter. Type: {symbol}");
            }

            _collected.Add(symbol);

            var success = true;
            foreach (var item in symbol.TypeArguments)
            {
                success = Collect(item) && success;
            }

            return success;
        }

        private bool CollectObject(INamedTypeSymbol symbol)
        {
            var objectAttribute = GetAttribute(symbol, _objectAttributeType);
            if (objectAttribute == null)
            {
                throw new Exception($"Serialization Object must be marked by {_objectAttributeType.Name}. Type: {symbol}");
            }

            ObjectDefinition collectedBase = null;

            if (!BuiltInTypes.Contains(symbol.BaseType.ToString()))
            {
                if (!_collected.Contains(symbol.BaseType) && !CollectObject(symbol.BaseType))
                {
                    throw new InvalidOperationException($"Not serializable base type. Type {symbol}");
                }

                collectedBase = _collectedObjects.Find(d => d.Type == symbol.BaseType);
            }

            var members = new List<MemberDefinition>();
            foreach (var field in GetPublicMembers(symbol).OfType<IFieldSymbol>())
            {
                if (field.IsReadOnly) { continue; }
                if (GetAttribute(field, _ignoreAttributeType) != null) { continue; }
                if (!Collect(field.Type)) { continue; }

                var key = GetAttribute(field, _keyAttributeType)?.ConstructorArguments[0].Value.ToString() ?? ToSerializedName(field.Name);

                members.Add(new MemberDefinition
                {
                    Name = field.Name,
                    Key = key,
                    Type = field.Type
                });
            }

            foreach (var property in GetPublicMembers(symbol).OfType<IPropertySymbol>())
            {
                if (GetAttribute(property, _ignoreAttributeType, _legacyIgnoreAttributeType) != null) { continue; }
                if (!IsPublic(property.GetMethod)) { continue; }
                if (!IsPublic(property.SetMethod)) { continue; }
                if (!Collect(property.Type)) { continue; }

                var key = GetAttribute(property, _keyAttributeType, _legacyKeyAttributeType);
                members.Add(new MemberDefinition
                {
                    Name = property.Name,
                    Key = (key?.ConstructorArguments[0].Value.ToString()) ?? ToSerializedName(property.Name),
                    Type = property.Type
                });
            }

            var collectedObject = new ObjectDefinition
            {
                Type = symbol,
                Base = collectedBase,
                Members = members.ToArray()
            };

            _collectedObjects.Add(collectedObject);
            _collected.Add(symbol);

            return true;
        }

        private string ToSerializedName(string name)
        {
            if (string.IsNullOrEmpty(name)) { return string.Empty; }

            var chars = name.ToCharArray();
            chars[0] = char.ToLower(chars[0]);
            return new string(chars);
        }

        private static IEnumerable<ISymbol> GetPublicMembers(ITypeSymbol symbol) =>
            symbol.GetMembers().Where(s => IsPublic(s) && !s.IsStatic);

        private static AttributeData GetAttribute(ISymbol symbol, INamedTypeSymbol attribute) =>
            symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass == attribute);

        private static bool IsPublic(ISymbol symbol) =>
            symbol.DeclaredAccessibility == Accessibility.Public;
    }
}
