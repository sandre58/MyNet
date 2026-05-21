// -----------------------------------------------------------------------
// <copyright file="ParentNamespaceConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Implements a naming convention that resolves view and view model types based on their parent namespace structure.
/// </summary>
public sealed class ParentNamespaceConvention : ITypeNamingConvention
{
    /// <summary>
    /// Resolves a view or view model type based on the parent namespace convention. The convention assumes that views and view models are organized in separate namespaces under a common parent namespace. For example, if the source type is a view model located in the "MyApp.ViewModels" namespace, the convention will look for a corresponding view in the "MyApp.Views" namespace with a matching base name. Conversely, if the source type is a view located in the "MyApp.Views" namespace, the convention will look for a corresponding view model in the "MyApp.ViewModels" namespace with a matching base name. If no matching type is found, the method returns null.
    /// </summary>
    /// <param name="source">The source type to resolve from.</param>
    /// <returns>The resolved target type if found; otherwise, null.</returns>
    public Type? Resolve(Type source)
    {
        var assembly = source.Assembly;
        var ns = NamingConventionHelpers.GetParentNamespace(source.Namespace);

        if (ns is null)
            return null;

        if (source.Name.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
            var fullName = $"{ns}.Views.{baseName}View";
            return assembly.GetType(fullName);
        }

        if (IsViewType(source))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
            var fullName = $"{ns}.ViewModels.{baseName}ViewModel";
            return assembly.GetType(fullName);
        }

        return null;
    }

    /// <summary>
    /// Determines whether the specified type is a view type based on its name. A type is considered a view type if its name ends with one of the recognized view suffixes.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a view type; otherwise, false.</returns>
    private static bool IsViewType(Type type) => NamingConventionHelpers.IsViewType(type);
}
