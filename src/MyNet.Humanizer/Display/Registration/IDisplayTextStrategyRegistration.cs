// -----------------------------------------------------------------------
// <copyright file="IDisplayTextStrategyRegistration.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Humanizer.Display.Registration;

/// <summary>
/// Represents a registration of a display text strategy for a specific target type.
/// </summary>
public interface IDisplayTextStrategyRegistration
{
    /// <summary>
    /// Gets the target type associated with this display text strategy registration.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Gets the display text strategy instance associated with this registration.
    /// </summary>
    IDisplayTextStrategy Strategy { get; }
}
