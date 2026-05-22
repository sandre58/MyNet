// -----------------------------------------------------------------------
// <copyright file="MetadataProviderGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MyNet.Observable.Metadata.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class MetadataProviderGenerator : IIncrementalGenerator
{
    private const string IgnoreModificationTrackingAttribute = "MyNet.Observable.Behaviors.Metadata.Attributes.IgnoreModificationTrackingAttribute";
    private const string UpdateOnCultureChangedAttribute = "MyNet.Observable.Behaviors.Metadata.Attributes.UpdateOnCultureChangedAttribute";
    private const string UpdateOnTimeZoneChangedAttribute = "MyNet.Observable.Behaviors.Metadata.Attributes.UpdateOnTimeZoneChangedAttribute";
    private const string AlsoValidateAttribute = "MyNet.Observable.Behaviors.Metadata.Attributes.AlsoValidateAttribute";

    // Behavior-related property attributes
    private const string ForwardPropertyAttribute = "MyNet.Observable.Behaviors.Metadata.Attributes.ForwardPropertyAttribute";
    private const string EnforceGeneratedMetadataAttribute = "MyNet.Observable.Behaviors.Metadata.Attributes.EnforceGeneratedMetadataAttribute";
    private const string ObservableObjectType = "MyNet.Observable.ObservableObject";

    private static readonly DiagnosticDescriptor MissingProviderDescriptor = new(
        id: "MNETMETA001",
        title: "Generated metadata configuration is missing",
        messageFormat: "Type '{0}' derives from ObservableObject but has no generated metadata configuration. Add metadata attributes to at least one property or disable strict mode.",
        category: "MyNet.Observable.Metadata.Generator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Check for strict mode assembly attribute
        var enforceMetadata = context.CompilationProvider
            .Select(static (compilation, _) => HasEnforceGeneratedMetadataAttribute(compilation));

        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) => GetTypeModel(ctx))
            .Where(static model => model is not null)
            .Select(static (model, _) => model!);

        context.RegisterSourceOutput(
            candidates.Collect().Combine(enforceMetadata).Combine(context.CompilationProvider),
            static (spc, data) => Emit(spc, data.Left.Left, data.Left.Right, data.Right));
    }

    private static bool HasEnforceGeneratedMetadataAttribute(Compilation compilation)
        => compilation.Assembly.GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == EnforceGeneratedMetadataAttribute);

    private static TypeModel? GetTypeModel(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classSyntax)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol typeSymbol)
            return null;

        if (typeSymbol.IsAbstract)
            return null;

        var properties =
            (from property in typeSymbol.GetMembers().OfType<IPropertySymbol>()
             where !property.IsStatic
             let model = BuildPropertyModel(property)
             where model is not null
             select model)
            .ToImmutableArray();

        if (properties.Length == 0)
            return null;

        var fullyQualifiedTypeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new(fullyQualifiedTypeName, properties);
    }

    [SuppressMessage("ReSharper", "MergeIntoPattern", Justification = "Improves readability in this case")]
    private static PropertyModel? BuildPropertyModel(IPropertySymbol property)
    {
        var ignoreTracking = false;
        var reactsToCulture = false;
        var reactsToTimeZone = false;
        var propertyChangedForwarding = false;
        var concatenateForwardedPropertyName = true;
        var validationDependents = ImmutableArray<string>.Empty;

        foreach (var attributeName in from attribute in property.GetAttributes() let attributeName = attribute.AttributeClass?.ToDisplayString() where !TryApplyFeatureAttribute(attributeName, attribute, ref ignoreTracking, ref reactsToCulture, ref reactsToTimeZone, ref validationDependents) where !TryApplyBehaviorAttribute(attributeName, attribute, ref propertyChangedForwarding, ref concatenateForwardedPropertyName) select attributeName)
        {
            switch (attributeName)
            {
                default:
                    continue;
            }
        }

        return !ignoreTracking && !reactsToCulture && !reactsToTimeZone && !propertyChangedForwarding && validationDependents.Length == 0
            ? null
            : new(property.Name, ignoreTracking, reactsToCulture, reactsToTimeZone, propertyChangedForwarding, concatenateForwardedPropertyName, validationDependents);
    }

    [SuppressMessage("ReSharper", "MergeIntoPattern", Justification = "Improves readability in this case")]
    private static bool TryApplyFeatureAttribute(
        string? attributeName,
        AttributeData attribute,
        ref bool ignoreTracking,
        ref bool reactsToCulture,
        ref bool reactsToTimeZone,
        ref ImmutableArray<string> validationDependents)
    {
        switch (attributeName)
        {
            case IgnoreModificationTrackingAttribute:
                ignoreTracking = true;
                return true;
            case UpdateOnCultureChangedAttribute:
                reactsToCulture = true;
                return true;
            case UpdateOnTimeZoneChangedAttribute:
                reactsToTimeZone = true;
                return true;
            case AlsoValidateAttribute:
                if (attribute.ConstructorArguments.Length == 1 &&
                    attribute.ConstructorArguments[0].Value is string dependent &&
                    !string.IsNullOrWhiteSpace(dependent))
                {
                    validationDependents = validationDependents.Add(dependent);
                }

                return true;
            default:
                return false;
        }
    }

    private static bool TryApplyBehaviorAttribute(string? attributeName, AttributeData attribute, ref bool propertyChangedForwarding, ref bool concatenateForwardedPropertyName)
    {
        switch (attributeName)
        {
            case ForwardPropertyAttribute:
                propertyChangedForwarding = true;

                if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is bool ctorValue)
                    concatenateForwardedPropertyName = ctorValue;

                foreach (var namedArgument in attribute.NamedArguments)
                {
                    if (string.Equals(namedArgument.Key, "ConcatenatePropertyName", StringComparison.Ordinal) && namedArgument.Value.Value is bool namedValue)
                    {
                        concatenateForwardedPropertyName = namedValue;
                        break;
                    }
                }

                return true;
            default:
                return false;
        }
    }

    private static void Emit(SourceProductionContext context, ImmutableArray<TypeModel> models, bool enforceGeneratedMetadata, Compilation compilation)
    {
        var distinctModels = models
            .GroupBy(static x => x.FullyQualifiedTypeName, StringComparer.Ordinal)
            .Select(static x => x.First())
            .OrderBy(static x => x.FullyQualifiedTypeName, StringComparer.Ordinal)
            .ToArray();

        var source = new StringBuilder();
        source.AppendLine("// <auto-generated />");
        source.AppendLine("#nullable enable");
        source.AppendLine("using System;");
        source.AppendLine("using System.Runtime.CompilerServices;");
        source.AppendLine("using MyNet.Observable.Behaviors;");
        source.AppendLine("using MyNet.Utilities;");
        source.AppendLine("using MyNet.Utilities.Metadata;");
        source.AppendLine("using MyNet.Observable;");
        source.AppendLine();
        source.AppendLine("namespace MyNet.Utilities.Metadata.Generated");
        source.AppendLine("{");

        // Generate fluent metadata configuration class
        if (distinctModels.Length > 0)
        {
            source.AppendLine();
            source.AppendLine("    internal static class MetadataFluentConfigurator");
            source.AppendLine("    {");
            source.AppendLine("        [ModuleInitializer]");
            source.AppendLine("        internal static void Configure()");
            source.AppendLine("        {");

            foreach (var t in distinctModels)
            {
                EmitFluentConfiguration(source, t);
            }

            source.AppendLine("        }");
            source.AppendLine("    }");
        }

        source.AppendLine("}");

        context.AddSource("GeneratedMetadataProviders.g.cs", SourceText.From(source.ToString(), Encoding.UTF8));

        if (enforceGeneratedMetadata)
            ReportMissingMetadata(context, compilation, distinctModels);
    }

    private static void ReportMissingMetadata(SourceProductionContext context, Compilation compilation, TypeModel[] generatedModels)
    {
        var observableObjectSymbol = compilation.GetTypeByMetadataName(ObservableObjectType);
        if (observableObjectSymbol is null)
            return;

        var generatedTypeNames = new HashSet<string>(
            generatedModels.Select(x => x.FullyQualifiedTypeName),
            StringComparer.Ordinal);

        foreach (var type in EnumerateNamedTypes(compilation.Assembly.GlobalNamespace))
        {
            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
                continue;

            if (!InheritsFrom(type, observableObjectSymbol))
                continue;

            var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (generatedTypeNames.Contains(typeName))
                continue;

            var location = type.Locations.FirstOrDefault(static l => l.IsInSource);
            if (location is null)
                continue;

            context.ReportDiagnostic(Diagnostic.Create(MissingProviderDescriptor, location, type.ToDisplayString()));
        }
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

    private static IEnumerable<INamedTypeSymbol> EnumerateNamedTypes(INamespaceSymbol ns)
    {
        foreach (var type in ns.GetTypeMembers())
        {
            yield return type;

            foreach (var nested in EnumerateNestedTypes(type))
                yield return nested;
        }

        foreach (var child in ns.GetNamespaceMembers())
        {
            foreach (var type in EnumerateNamedTypes(child))
                yield return type;
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateNestedTypes(INamedTypeSymbol type)
    {
        foreach (var nested in type.GetTypeMembers())
        {
            yield return nested;

            foreach (var child in EnumerateNestedTypes(nested))
                yield return child;
        }
    }

    private static void EmitFluentConfiguration(StringBuilder source, TypeModel model)
    {
        source.AppendLine("            // Configure metadata fluently for " + model.FullyQualifiedTypeName);

        foreach (var property in model.Properties.Where(property => property.IgnoreModificationTracking || property.ReactsToCulture || property.ReactsToTimeZone || property.PropertyChangedForwarding || property.ValidationDependents.Length > 0))
        {
            EmitFluentPropertyConfiguration(source, model, property);
        }

        source.AppendLine();
    }

    private static void EmitFluentPropertyConfiguration(StringBuilder source, TypeModel model, PropertyModel property)
    {
        var emitsMetadataConfiguration = property.IgnoreModificationTracking || property.ReactsToCulture || property.ReactsToTimeZone || property.ValidationDependents.Length > 0;
        if (emitsMetadataConfiguration)
        {
            // Generates code that directly configures metadata features for the property
            source.Append("            var metadata_");
            source.Append(property.Name);
            source.Append(" = MetadataRegistry.Get(typeof(");
            source.Append(model.FullyQualifiedTypeName);
            source.Append(")).GetProperty(\"");
            source.Append(property.Name);
            source.AppendLine("\");");

            EmitFeatureConfiguration(source, property);

            if (property.ValidationDependents.Length > 0)
            {
                source.Append("            metadata_");
                source.Append(property.Name);
                source.Append(".Validates(");
                for (var i = 0; i < property.ValidationDependents.Length; i++)
                {
                    if (i > 0)
                        source.Append(", ");
                    source.Append('"');
                    source.Append(property.ValidationDependents[i].Replace("\"", "\\\""));
                    source.Append('"');
                }

                source.AppendLine(");");
            }
        }

        EmitBehaviorConfiguration(source, model, property);
    }

    private static void EmitBehaviorConfiguration(StringBuilder source, TypeModel model, PropertyModel property)
    {
        if (!property.PropertyChangedForwarding)
            return;

        source.Append("            GeneratedPropertyBehaviorRegistry.RegisterForwardProperty(typeof(");
        source.Append(model.FullyQualifiedTypeName);
        source.Append("), \"");
        source.Append(property.Name);
        source.Append("\", ");
        source.Append(property.ConcatenateForwardedPropertyName ? "true" : "false");
        source.AppendLine(");");
    }

    private static void EmitFeatureConfiguration(StringBuilder source, PropertyModel property)
    {
        if (property.IgnoreModificationTracking)
        {
            source.Append("            metadata_");
            source.Append(property.Name);
            source.AppendLine(".IgnoreModificationTracking();");
        }

        if (property.ReactsToCulture)
        {
            source.Append("            metadata_");
            source.Append(property.Name);
            source.AppendLine(".UpdateOnCultureChanged();");
        }

        if (property.ReactsToTimeZone)
        {
            source.Append("            metadata_");
            source.Append(property.Name);
            source.AppendLine(".UpdateOnTimeZoneChanged();");
        }
    }

    private sealed class TypeModel(string fullyQualifiedTypeName, ImmutableArray<PropertyModel> properties)
    {
        public string FullyQualifiedTypeName { get; } = fullyQualifiedTypeName;

        public ImmutableArray<PropertyModel> Properties { get; } = properties;
    }

    private sealed class PropertyModel(string name, bool ignoreModificationTracking, bool reactsToCulture, bool reactsToTimeZone, bool propertyChangedForwarding, bool concatenateForwardedPropertyName, ImmutableArray<string> validationDependents)
    {
        public string Name { get; } = name;

        public bool IgnoreModificationTracking { get; } = ignoreModificationTracking;

        public bool ReactsToCulture { get; } = reactsToCulture;

        public bool ReactsToTimeZone { get; } = reactsToTimeZone;

        public bool PropertyChangedForwarding { get; } = propertyChangedForwarding;

        public bool ConcatenateForwardedPropertyName { get; } = concatenateForwardedPropertyName;

        public ImmutableArray<string> ValidationDependents { get; } = validationDependents;
    }
}
