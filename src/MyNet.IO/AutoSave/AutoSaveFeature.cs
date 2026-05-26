// -----------------------------------------------------------------------
// <copyright file="AutoSaveFeature.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.IO.AutoSave;

/// <summary>
/// Represents the auto-save feature, allowing to enable or disable it and notifying about changes in its state.
/// </summary>
public sealed class AutoSaveFeature : IAutoSaveFeature
{
    /// <summary>
    /// Gets a value indicating whether the auto-save feature is currently enabled.
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Occurs when the state of the auto-save feature changes. The event handler receives a boolean value indicating the new state of the feature (enabled or disabled).
    /// </summary>
    public event EventHandler<AutoSaveEnabledChangedEventArgs>? Changed;

    /// <summary>
    /// Enables the auto-save feature. If the feature is already enabled, this method does nothing. If the state changes to enabled, it raises the <see cref="Changed"/> event with the new state (true).
    /// </summary>
    public void Enable() => Set(true);

    /// <summary>
    /// Disables the auto-save feature. If the feature is already disabled, this method does nothing. If the state changes to disabled, it raises the <see cref="Changed"/> event with the new state (false).
    /// </summary>
    public void Disable() => Set(false);

    /// <summary>
    /// Sets the state of the auto-save feature to the specified value. If the new value is the same as the current state, this method does nothing. If the state changes, it updates the <see cref="IsEnabled"/> property and raises the <see cref="Changed"/> event with the new state.
    /// </summary>
    /// <param name="value">The new state of the auto-save feature.</param>
    private void Set(bool value)
    {
        if (IsEnabled == value)
            return;

        IsEnabled = value;
        Changed?.Invoke(this, new(value));
    }
}
