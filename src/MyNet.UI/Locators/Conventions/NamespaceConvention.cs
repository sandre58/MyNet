// -----------------------------------------------------------------------
// <copyright file="NamespaceConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Implements a naming convention that maps view models to views based on their namespaces and class names.
/// </summary>
public sealed class NamespaceConvention : ITypeNamingConvention
{
    /// <summary>
    /// Resolves the target type based on the source type's namespace and class name. It follows a convention where:
    /// - If the source type is a view model (ends with "ViewModel"), it looks for a corresponding view type by replacing "ViewModel" with "View" and changing the namespace segment from "ViewModels" to "Views".
    /// - If the source type is a view (ends with "View"), it looks for a corresponding view model type by replacing "View" with "ViewModel" and changing the namespace segment from "Views" to "ViewModels".
    /// </summary>
    /// <param name="source">The source type to resolve from.</param>
    /// <returns>The resolved target type if found; otherwise, null.</returns>
    public Type? Resolve(Type source)
    {
        var assembly = source.Assembly;
        var ns = source.Namespace;

        if (ns is null)
            return null;

        // ViewModel -> View
        if (source.Name.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
            var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(ns, "ViewModels", "Views");

            var fullName = $"{targetNs}.{baseName}View";
            return assembly.GetType(fullName);
        }

        // View -> ViewModel
        if (IsViewType(source))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
            var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(ns, "Views", "ViewModels");

            var fullName = $"{targetNs}.{baseName}ViewModel";
            return assembly.GetType(fullName);
        }

        return null;
    }

    /// <summary>
    /// Determines whether the specified type is a view type based on its name. A view type is identified by the presence of one of the recognized view suffixes in its name.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a view; otherwise, false.</returns>
    private static bool IsViewType(Type type) => NamingConventionHelpers.IsViewType(type);
}
