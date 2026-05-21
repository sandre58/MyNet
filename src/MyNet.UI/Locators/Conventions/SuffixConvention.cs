// -----------------------------------------------------------------------
// <copyright file="SuffixConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Implements a naming convention that resolves view and view model types based on common suffixes.
/// </summary>
public sealed class SuffixConvention : ITypeNamingConvention
{
    /// <summary>
    /// Resolves a view or view model type based on the naming convention. If the source type is a view model (ends with "ViewModel"), it attempts to find a corresponding view type by replacing "ViewModels" with "Views" in the namespace and removing the "ViewModel" suffix, trying each known view suffix in order. If the source type is a view (ends with one of the defined view suffixes), it attempts to find a corresponding view model type by replacing "Views" with "ViewModels" in the namespace and adding the "ViewModel" suffix. If no matching type is found, it returns null.
    /// </summary>
    /// <param name="source">The source type to resolve from.</param>
    /// <returns>The resolved target type if found; otherwise, null.</returns>
    public Type? Resolve(Type source)
    {
        var assembly = source.Assembly;
        var ns = source.Namespace;

        if (ns is null)
            return null;

        if (source.Name.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
            var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(ns, "ViewModels", "Views");

            foreach (var suffix in NamingConventionHelpers.ViewSuffixes)
            {
                var fullName = $"{targetNs}.{baseName}{suffix}";
                var type = assembly.GetType(fullName);

                if (type != null)
                    return type;
            }
        }

        if (NamingConventionHelpers.IsViewType(source))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
            var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(ns, "Views", "ViewModels");
            var fullName = $"{targetNs}.{baseName}ViewModel";
            return assembly.GetType(fullName);
        }

        return null;
    }
}
