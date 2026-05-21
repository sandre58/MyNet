// -----------------------------------------------------------------------
// <copyright file="ITypeNamingConvention.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Defines the contract for a type naming convention that can resolve a source type to a target type based on specific naming rules.
/// </summary>
public interface ITypeNamingConvention
{
    /// <summary>
    /// Resolves the target type for the given source type based on the naming convention. Returns null if the source type does not match the convention or if the target type cannot be determined.
    /// </summary>
    /// <param name="source">The source type to resolve.</param>
    /// <returns>The resolved target type if a mapping exists; otherwise, null.</returns>
    Type? Resolve(Type source);
}
