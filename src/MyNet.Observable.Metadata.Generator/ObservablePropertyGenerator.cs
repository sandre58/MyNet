// -----------------------------------------------------------------------
// <copyright file="ObservablePropertyGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MyNet.Observable.Metadata.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class ObservablePropertyGenerator : IIncrementalGenerator
{
    private const string ObservablePropertyAttribute = "MyNet.Observable.Metadata.ObservablePropertyAttribute";
    private const string ObservableObjectType = "MyNet.Observable.ObservableObject";

    private static readonly DiagnosticDescriptor NotPartialDescriptor = new(
        id: "MNETOBS001",
        title: "Observable property host type must be partial",
        messageFormat: "Type '{0}' must be declared partial to use [ObservableProperty]",
        category: "MyNet.Observable.Generator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor NotObservableObjectDescriptor = new(
        id: "MNETOBS002",
        title: "Observable property host type must derive from ObservableObject",
        messageFormat: "Type '{0}' must derive from ObservableObject to use [ObservableProperty]",
        category: "MyNet.Observable.Generator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor NestedTypeNotSupportedDescriptor = new(
        id: "MNETOBS003",
        title: "Nested types are not supported for [ObservableProperty]",
        messageFormat: "Type '{0}' is nested; move [ObservableProperty] fields to a non-nested partial ObservableObject",
        category: "MyNet.Observable.Generator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var fields = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is FieldDeclarationSyntax { AttributeLists.Count: > 0 },
            static (ctx, _) => GetFieldModels(ctx))
            .SelectMany(static (models, _) => models);

        context.RegisterSourceOutput(fields.Collect(), static (spc, models) => Emit(spc, models));
    }

    private static ImmutableArray<FieldModel?> GetFieldModels(GeneratorSyntaxContext context)
    {
        if (context.Node is not FieldDeclarationSyntax fieldDeclaration)
            return ImmutableArray<FieldModel?>.Empty;

        var builder = ImmutableArray.CreateBuilder<FieldModel?>();

        foreach (var variable in fieldDeclaration.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSymbol)
                continue;

            var model = GetFieldModel(context, fieldSymbol);
            if (model is not null)
                builder.Add(model);
        }

        return builder.ToImmutable();
    }

    [SuppressMessage("ReSharper", "ConvertTypeCheckPatternToNullCheck", Justification = "Explicitly checking for nullability here for clarity in the generator context.")]
    private static FieldModel? GetFieldModel(GeneratorSyntaxContext context, IFieldSymbol fieldSymbol)
    {
        if (fieldSymbol.IsStatic || fieldSymbol.IsConst)
            return null;

        if (!HasObservablePropertyAttribute(fieldSymbol))
            return null;

        if (fieldSymbol.ContainingType is not INamedTypeSymbol containingType)
            return null;

        var typeDeclaration = context.Node.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();

        if (typeDeclaration is null)
            return null;

        var isPartial = typeDeclaration.Modifiers.Any(static m => m.IsKind(SyntaxKind.PartialKeyword));
        var typeName = containingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var location = typeDeclaration.Identifier.GetLocation();

        if (!isPartial)
            return FieldModel.WithDiagnostic(NotPartialDescriptor, location, typeName);

        var observableObject = context.SemanticModel.Compilation.GetTypeByMetadataName(ObservableObjectType);
        if (observableObject is null || !InheritsFrom(containingType, observableObject))
            return FieldModel.WithDiagnostic(NotObservableObjectDescriptor, location, typeName);

        if (containingType.ContainingType is not null)
            return FieldModel.WithDiagnostic(NestedTypeNotSupportedDescriptor, location, typeName);

        var propertyName = ToPropertyName(fieldSymbol.Name);
        if (string.IsNullOrEmpty(propertyName))
            return null;

        var propertyType = fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var fieldName = fieldSymbol.Name;

        var namespaceName = containingType.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        return new(namespaceName, containingType.Name, propertyName, propertyType, fieldName, null);
    }

    private static bool HasObservablePropertyAttribute(IFieldSymbol fieldSymbol)
    {
        foreach (var attribute in fieldSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == ObservablePropertyAttribute)
                return true;
        }

        return false;
    }

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;
        }

        return false;
    }

    private static string ToPropertyName(string fieldName)
    {
        if (fieldName.StartsWith("_", StringComparison.Ordinal))
            fieldName = fieldName.Substring(1);

        if (fieldName.StartsWith("m_", StringComparison.Ordinal))
            fieldName = fieldName.Substring(2);

        if (fieldName.Length == 0)
            return string.Empty;

        if (fieldName.Length == 1)
            return fieldName.ToUpperInvariant();

        return char.ToUpperInvariant(fieldName[0]) + fieldName.Substring(1);
    }

    private static void Emit(SourceProductionContext context, ImmutableArray<FieldModel?> models)
    {
        var valid = models.Where(static m => m is { Diagnostic: null }).Cast<FieldModel>().ToArray();

        foreach (var diagnostic in models.Where(static m => m?.Diagnostic is not null).Select(static m => m!.Diagnostic!))
            context.ReportDiagnostic(diagnostic);

        foreach (var group in valid.GroupBy(static m => m.TypeKey, StringComparer.Ordinal))
        {
            var first = group.First();
            var source = new StringBuilder();
            source.AppendLine("// <auto-generated />");
            source.AppendLine("#nullable enable");
            source.AppendLine();

            if (!string.IsNullOrEmpty(first.NamespaceName))
            {
                source.Append("namespace ");
                source.Append(first.NamespaceName);
                source.AppendLine(";");
                source.AppendLine();
            }

            source.AppendLine($"partial class {first.TypeName}");
            source.AppendLine("{");

            foreach (var field in group.OrderBy(static f => f.PropertyName, StringComparer.Ordinal))
            {
                source.AppendLine($"    public {field.PropertyType} {field.PropertyName}");
                source.AppendLine("    {");
                source.AppendLine($"        get => {field.FieldName};");
                source.AppendLine($"        set => SetProperty(ref {field.FieldName}, value);");
                source.AppendLine("    }");
                source.AppendLine();
            }

            source.AppendLine("}");

            var fileName = $"{SanitizeFileName(first.TypeName)}.ObservableProperties.g.cs";
            context.AddSource(fileName, SourceText.From(source.ToString(), Encoding.UTF8));
        }
    }

    private static string SanitizeFileName(string typeName)
        => typeName.Replace('<', '_').Replace('>', '_');

    private sealed class FieldModel(
        string namespaceName,
        string typeName,
        string propertyName,
        string propertyType,
        string fieldName,
        Diagnostic? diagnostic)
    {
        public string NamespaceName { get; } = namespaceName;

        public string TypeName { get; } = typeName;

        public string TypeKey => string.IsNullOrEmpty(NamespaceName) ? TypeName : NamespaceName + "." + TypeName;

        public string PropertyName { get; } = propertyName;

        public string PropertyType { get; } = propertyType;

        public string FieldName { get; } = fieldName;

        public Diagnostic? Diagnostic { get; } = diagnostic;

        public static FieldModel WithDiagnostic(DiagnosticDescriptor descriptor, Location location, string typeName)
        {
            var diagnostic = Diagnostic.Create(descriptor, location, typeName);
            return new(string.Empty, typeName, string.Empty, string.Empty, string.Empty, diagnostic);
        }
    }
}
