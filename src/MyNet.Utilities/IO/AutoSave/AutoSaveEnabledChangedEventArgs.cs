// -----------------------------------------------------------------------
// <copyright file="AutoSaveEnabledChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.IO.AutoSave;

/// <summary>
/// Initializes a new instance of the <see cref="AutoSaveEnabledChangedEventArgs"/> class with the specified state.
/// </summary>
/// <param name="isEnabled">The new state of the auto-save feature.</param>
public class AutoSaveEnabledChangedEventArgs(bool isEnabled) : EventArgs
{
    /// <summary>
    /// Gets a value indicating whether the auto-save feature is enabled.
    /// </summary>
    public bool IsEnabled { get; } = isEnabled;
}
