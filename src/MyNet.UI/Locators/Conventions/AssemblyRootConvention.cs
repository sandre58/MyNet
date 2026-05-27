// -----------------------------------------------------------------------
// <copyright file="AssemblyRootConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Maps view models to views under <c>{AssemblyName}.{subNamespace}.{Name}View</c>.
/// Opt-in via <see cref="ServiceCollectionExtensions.AddAssemblyRootConvention"/>.
/// </summary>
/// <param name="subNamespace">The sub-namespace segment for views (default: <c>UI.Views</c>).</param>
public sealed class AssemblyRootConvention(string subNamespace = "UI.Views") : TypeNamingConventionBase
{
    /// <inheritdoc />
    protected override Type? ResolveViewFromViewModel(Type source)
    {
        var assemblyName = source.Assembly.GetName().Name;
        if (assemblyName is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
        return GetType(source, $"{assemblyName}.{subNamespace}.{baseName}View");
    }

    /// <inheritdoc />
    protected override Type? ResolveViewModelFromView(Type source)
    {
        var assemblyName = source.Assembly.GetName().Name;
        if (assemblyName is null)
            return null;

        var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
        return GetType(source, $"{assemblyName}.ViewModels.{baseName}ViewModel");
    }
}
