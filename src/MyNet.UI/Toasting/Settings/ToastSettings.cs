// -----------------------------------------------------------------------
// <copyright file="ToastSettings.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Toasting.Settings;

/// <summary>
/// Settings for configuring the behavior of an individual toast notification.
/// </summary>
public class ToastSettings
{
    /// <summary>
    /// Gets the default toast settings.
    /// </summary>
    public static ToastSettings Default => new();

    /// <summary>
    /// Gets or sets the strategy for closing the toast notification.
    /// </summary>
    public ToastClosingStrategy ClosingStrategy { get; set; } = ToastClosingStrategy.AutoClose;

    /// <summary>
    /// Gets or sets a value indicating whether the toast notification should freeze when the mouse enters it.
    /// </summary>
    public bool FreezeOnMouseEnter { get; set; } = true;

    /// <summary>
    /// Gets or sets the duration for which the toast notification should be displayed before it is automatically closed.
    /// </summary>
    public TimeSpan? Duration { get; set; }
}
