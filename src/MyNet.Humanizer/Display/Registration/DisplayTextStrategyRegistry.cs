// -----------------------------------------------------------------------
// <copyright file="DisplayTextStrategyRegistry.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyNet.Humanizer.Display.Registration;

/// <summary>
/// Current implementation of <see cref="IDisplayTextStrategyRegistry"/> that stores strategies in a frozen dictionary for efficient retrieval.
/// </summary>
/// <param name="strategies">The strategies to be registered.</param>
public sealed class DisplayTextStrategyRegistry(IReadOnlyDictionary<Type, IDisplayTextStrategy> strategies) : IDisplayTextStrategyRegistry
{
    private readonly FrozenDictionary<Type, IDisplayTextStrategy> _strategies = strategies.ToFrozenDictionary();

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, IDisplayTextStrategy> Strategies => _strategies;

    /// <inheritdoc />
    public bool TryGetStrategy(Type targetType, [NotNullWhen(true)] out IDisplayTextStrategy? strategy) => _strategies.TryGetValue(targetType, out strategy);
}
