// -----------------------------------------------------------------------
// <copyright file="ObservableObjectSetterAnalyzer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyNet.Observable.Metadata.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObservableObjectSetterAnalyzer : DiagnosticAnalyzer
{
    private const string ObservableObjectMetadataName = "MyNet.Observable.ObservableObject";

    private static readonly DiagnosticDescriptor SetterMustUseSetPropertyDescriptor = new(
        id: "MNETOBS004",
        title: "Observable property setter must use SetProperty or the changing pipeline",
        messageFormat: "Property setter for '{0}' should call SetProperty(ref field, value) or guard assignment with OnPropertyChanging / ProcessPropertyChanging",
        category: "MyNet.Observable.Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Assigning a backing field without SetProperty bypasses cancellation, behaviors, and coalesced suspension.");

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        [SetterMustUseSetPropertyDescriptor];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSetter, SyntaxKind.SetAccessorDeclaration);
    }

    private static void AnalyzeSetter(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AccessorDeclarationSyntax setter)
            return;

        if (setter.Body is null && setter.ExpressionBody is null)
            return;

        if (IsGeneratedFile(setter.SyntaxTree.FilePath))
            return;

        var propertyDeclaration = setter.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
        if (propertyDeclaration is null)
            return;

        if (context.SemanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken) is not { } propertySymbol)
            return;

        if (propertySymbol.IsIndexer)
            return;

        var containingType = propertySymbol.ContainingType;
        if (!InheritsFromObservableObject(containingType, context.Compilation))
            return;

        if (UsesSetProperty(setter, context.SemanticModel))
            return;

        if (UsesManualChangingPipeline(setter, context.SemanticModel))
            return;

        if (!AssignsBackingMember(setter, propertySymbol, context.SemanticModel))
            return;

        var location = setter.Body?.OpenBraceToken.GetLocation() ?? setter.ExpressionBody?.GetLocation() ?? setter.GetLocation();
        context.ReportDiagnostic(Diagnostic.Create(SetterMustUseSetPropertyDescriptor, location, propertySymbol.Name));
    }

    private static bool InheritsFromObservableObject(INamedTypeSymbol type, Compilation compilation)
    {
        var observableObject = compilation.GetTypeByMetadataName(ObservableObjectMetadataName);
        if (observableObject is null)
            return false;

        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, observableObject))
                return true;
        }

        return false;
    }

    private static bool UsesSetProperty(AccessorDeclarationSyntax setter, SemanticModel semanticModel)
        => setter.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Any(invocation => IsSetPropertyInvocation(invocation, semanticModel));

    private static bool IsSetPropertyInvocation(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        return symbol is { Name: "SetProperty" }
               && symbol.ContainingType.ToDisplayString() == ObservableObjectMetadataName;
    }

    private static bool UsesManualChangingPipeline(AccessorDeclarationSyntax setter, SemanticModel semanticModel)
        => setter.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Any(invocation =>
            {
                var name = semanticModel.GetSymbolInfo(invocation).Symbol?.Name;
                return name is "OnPropertyChanging" or "ProcessPropertyChanging";
            });

    private static bool AssignsBackingMember(AccessorDeclarationSyntax setter, IPropertySymbol property, SemanticModel semanticModel)
    {
        foreach (var assignment in setter.DescendantNodes().OfType<AssignmentExpressionSyntax>())
        {
            if (assignment.Kind() != SyntaxKind.SimpleAssignmentExpression)
                continue;

            var assignedSymbol = semanticModel.GetSymbolInfo(assignment.Left).Symbol;
            switch (assignedSymbol)
            {
                case IFieldSymbol field when IsBackingFieldForProperty(field, property):
                case IPropertySymbol assignedProperty
                    when SymbolEqualityComparer.Default.Equals(assignedProperty, property):
                    return true;
            }
        }

        return false;
    }

    private static bool IsBackingFieldForProperty(IFieldSymbol field, IPropertySymbol property)
    {
        if (field.AssociatedSymbol is not null
            && SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, property))
        {
            return true;
        }

        var fieldName = field.Name.TrimStart('_');
        if (fieldName.StartsWith("m_", System.StringComparison.Ordinal))
            fieldName = fieldName.Substring(2);

        return string.Equals(fieldName, property.Name, System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsGeneratedFile(string? filePath) => !string.IsNullOrEmpty(filePath) && (filePath!.EndsWith(".g.cs", System.StringComparison.OrdinalIgnoreCase)
                                                                                                 || filePath.IndexOf("Generated", System.StringComparison.OrdinalIgnoreCase) >= 0);
}
