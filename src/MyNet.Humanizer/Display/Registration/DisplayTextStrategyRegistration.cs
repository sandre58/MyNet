// -----------------------------------------------------------------------
// <copyright file="DisplayTextStrategyRegistration.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Humanizer.Display.Registration;

/// <summary>
/// Represents a registration of a display text strategy for a specific target type T.
/// </summary>
/// <param name="strategy">The display text strategy instance.</param>
/// <typeparam name="T">The target type for which the display text strategy is registered.</typeparam>
public sealed class DisplayTextStrategyRegistration<T>(IDisplayTextStrategy<T> strategy) : IDisplayTextStrategyRegistration
    where T : notnull
{
    /// <inheritdoc/>
    public Type TargetType => typeof(T);

    /// <inheritdoc/>
    public IDisplayTextStrategy Strategy => strategy;
}
