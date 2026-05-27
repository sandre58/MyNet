// -----------------------------------------------------------------------
// <copyright file="SuffixConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Maps view models to views by swapping the <c>ViewModels</c>/<c>Views</c> namespace segment and trying multiple view suffixes.
/// Registered by default via <see cref="ServiceCollectionExtensions.AddViewLocators"/>.
/// </summary>
public sealed class SuffixConvention : TypeNamingConventionBase
{
    /// <inheritdoc />
    protected override Type? ResolveViewFromViewModel(Type source)
    {
        if (source.Namespace is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
        var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(source.Namespace, "ViewModels", "Views");

        return GetFirstMatchingType(
            source,
            NamingConventionHelpers.ViewSuffixes.Select(suffix => $"{targetNs}.{baseName}{suffix}"));
    }

    /// <inheritdoc />
    protected override Type? ResolveViewModelFromView(Type source)
    {
        if (source.Namespace is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
        var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(source.Namespace, "Views", "ViewModels");

        return GetType(source, $"{targetNs}.{baseName}ViewModel");
    }
}
