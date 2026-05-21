// -----------------------------------------------------------------------
// <copyright file="IDisplayTextStrategyRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyNet.Humanizer.Display.Registration;

/// <summary>
/// Registry for display text providers keyed by their registered target type.
/// </summary>
public interface IDisplayTextStrategyRegistry
{
    /// <summary>
    /// Tries to resolve the strategy registered for the specified target type.
    /// </summary>
    /// <param name="targetType">Target type associated with the strategy.</param>
    /// <param name="strategy">Resolved strategy instance.</param>
    /// <returns>True if a strategy is registered for <paramref name="targetType"/>; otherwise false.</returns>
    bool TryGetStrategy(Type targetType, [NotNullWhen(true)] out IDisplayTextStrategy? strategy);

    /// <summary>
    /// Gets all registered strategies.
    /// </summary>
    IReadOnlyDictionary<Type, IDisplayTextStrategy> Strategies { get; }
}
