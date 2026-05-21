// -----------------------------------------------------------------------
// <copyright file="AssemblyRootConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Implements a naming convention that resolves view and view model types based on the assembly root namespace and a specified sub-namespace for views.
/// </summary>
/// <param name="subNamespace">The sub-namespace for views within the assembly.</param>
public sealed class AssemblyRootConvention(string subNamespace = "UI.Views") : ITypeNamingConvention
{
    /// <summary>
    /// Resolves the corresponding view or view model type based on the naming convention and assembly structure.
    /// </summary>
    /// <param name="source">The source type to resolve from.</param>
    /// <returns>The resolved target type if found; otherwise, null.</returns>
    public Type? Resolve(Type source)
    {
        var assembly = source.Assembly;
        var assemblyName = assembly.GetName().Name;

        if (assemblyName is null)
            return null;

        if (source.Name.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromViewModel(source);
            var fullName = $"{assemblyName}.{subNamespace}.{baseName}View";
            return assembly.GetType(fullName);
        }

        if (IsViewType(source))
        {
            var baseName = NamingConventionHelpers.GetBaseNameFromView(source);
            var fullName = $"{assemblyName}.ViewModels.{baseName}ViewModel";
            return assembly.GetType(fullName);
        }

        return null;
    }

    /// <summary>
    /// Determines whether the specified type is a view type based on its name. A view type is identified by the presence of one of the recognized view suffixes in its name.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a view type; otherwise, false.</returns>
    private static bool IsViewType(Type type) => NamingConventionHelpers.IsViewType(type);
}
