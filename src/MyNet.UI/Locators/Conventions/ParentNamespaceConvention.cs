// -----------------------------------------------------------------------
// <copyright file="ParentNamespaceConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Maps view models to views under <c>{ParentNamespace}.Views</c> and <c>{ParentNamespace}.ViewModels</c>.
/// Opt-in via <see cref="ServiceCollectionExtensions.AddParentNamespaceConvention"/>.
/// </summary>
public sealed class ParentNamespaceConvention : TypeNamingConventionBase
{
    /// <inheritdoc />
    protected override Type? ResolveViewFromViewModel(Type source)
    {
        var parentNs = NamingConventionHelpers.GetParentNamespace(source.Namespace);
        if (parentNs is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
        return GetType(source, $"{parentNs}.Views.{baseName}View");
    }

    /// <inheritdoc />
    protected override Type? ResolveViewModelFromView(Type source)
    {
        var parentNs = NamingConventionHelpers.GetParentNamespace(source.Namespace);
        if (parentNs is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
        return GetType(source, $"{parentNs}.ViewModels.{baseName}ViewModel");
    }
}
