// -----------------------------------------------------------------------
// <copyright file="TypeNamingConventionBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Base class for view model ↔ view naming conventions. Subclass and override the two resolve methods.
/// </summary>
public abstract class TypeNamingConventionBase : ITypeNamingConvention
{
    /// <inheritdoc />
    public Type? Resolve(Type source) => NamingConventionHelpers.IsViewModelType(source)
        ? ResolveViewFromViewModel(source)
        : NamingConventionHelpers.IsViewType(source) ? ResolveViewModelFromView(source) : null;

    /// <summary>
    /// Loads a type from the same assembly as <paramref name="source"/>.
    /// </summary>
    protected static Type? GetType(Type source, string fullTypeName) =>
        NamingConventionHelpers.GetAssemblyType(source, fullTypeName);

    /// <summary>
    /// Returns the first type that exists for the given fully qualified names.
    /// </summary>
    protected static Type? GetFirstMatchingType(Type source, IEnumerable<string> fullTypeNames) =>
        fullTypeNames
            .Select(fullName => GetType(source, fullName))
            .OfType<Type>()
            .FirstOrDefault();

    /// <summary>
    /// Resolves a view type from a view model type.
    /// </summary>
    protected abstract Type? ResolveViewFromViewModel(Type source);

    /// <summary>
    /// Resolves a view model type from a view type.
    /// </summary>
    protected abstract Type? ResolveViewModelFromView(Type source);
}
