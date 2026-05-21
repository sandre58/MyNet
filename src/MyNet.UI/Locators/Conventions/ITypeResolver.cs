// -----------------------------------------------------------------------
// <copyright file="ITypeResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Defines the contract for a type resolver that can register and resolve type mappings. This is useful for scenarios where you want to map one type to another, such as mapping view models to views or interfaces to implementations.
/// </summary>
public interface ITypeResolver
{
    /// <summary>
    /// Registers a mapping between a source type and a target type. When resolving, the resolver will return the target type associated with the given source type.
    /// </summary>
    /// <param name="source">The source type to register.</param>
    /// <param name="target">The target type to associate with the source type.</param>
    void Register(Type source, Type target);

    /// <summary>
    /// Resolves the target type for the given source type. Returns null if no mapping exists.
    /// </summary>
    /// <param name="source">The source type to resolve.</param>
    /// <returns>The target type associated with the source type, or null if no mapping exists.</returns>
    Type? Resolve(Type source);

    /// <summary>
    /// Clears the internal resolution cache. Manually registered mappings are preserved. Use this when assemblies are loaded dynamically or after modifying conventions at runtime.
    /// </summary>
    void ClearCache();
}
