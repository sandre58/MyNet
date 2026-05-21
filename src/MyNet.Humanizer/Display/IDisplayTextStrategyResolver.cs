// -----------------------------------------------------------------------
// <copyright file="IDisplayTextStrategyResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace MyNet.Humanizer.Display;

/// <summary>
/// Resolver for display name providers, allowing retrieval of providers by their registered target type.
/// </summary>
public interface IDisplayTextStrategyResolver
{
    /// <summary>
    /// Gets the display name strategy registered for the specified target type, or null if no strategy is registered.
    /// </summary>
    /// <typeparam name="T">The target type for which to retrieve the display name strategy.</typeparam>
    /// <param name="strategy">When this method returns, contains the display name strategy for the specified type, if found; otherwise, null.</param>
    /// <returns>True if a display name strategy is registered for the specified type; otherwise, false.</returns>
    bool TryGet<T>([NotNullWhen(true)] out IDisplayTextStrategy<T>? strategy)
        where T : notnull;

    /// <summary>
    /// Gets the display name strategy registered for the specified target type, or throws an exception if no strategy is registered.
    /// </summary>
    /// <typeparam name="T">The target type for which to retrieve the display name strategy.</typeparam>
    /// <returns>The display name strategy for the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if no display name strategy is registered for the specified type.</exception>
    IDisplayTextStrategy<T> GetRequired<T>()
        where T : notnull;
}
