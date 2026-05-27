// -----------------------------------------------------------------------
// <copyright file="NamespaceConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Maps view models to views via <c>ViewModels</c>/<c>Views</c> namespace segment swap and a fixed <c>*View</c> suffix.
/// Opt-in via <see cref="ServiceCollectionExtensions.AddNamespaceConvention"/>.
/// </summary>
public sealed class NamespaceConvention : TypeNamingConventionBase
{
    /// <inheritdoc />
    protected override Type? ResolveViewFromViewModel(Type source)
    {
        if (source.Namespace is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
        var targetNs = NamingConventionHelpers.ReplaceNamespaceSegment(source.Namespace, "ViewModels", "Views");

        return GetType(source, $"{targetNs}.{baseName}View");
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
