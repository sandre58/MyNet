// -----------------------------------------------------------------------
// <copyright file="ToastSettingsOverrides.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Toasting.Settings;

/// <summary>
/// Optional per-notification toast setting overrides. Only non-null properties replace global defaults.
/// </summary>
public sealed class ToastSettingsOverrides
{
    /// <summary>
    /// Gets the closing strategy override.
    /// </summary>
    public ToastClosingStrategy? ClosingStrategy { get; init; }

    /// <summary>
    /// Gets the freeze-on-mouse-enter override.
    /// </summary>
    public bool? FreezeOnMouseEnter { get; init; }

    /// <summary>
    /// Gets the auto-close duration override.
    /// </summary>
    public TimeSpan? Duration { get; init; }
}
