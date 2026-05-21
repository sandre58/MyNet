// -----------------------------------------------------------------------
// <copyright file="IAutoSaveFeature.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.IO.AutoSave;

/// <summary>
/// Defines the contract for a feature that can enable or disable the auto-save service. This interface allows for controlling whether the auto-save functionality is active, providing methods to enable or disable the service as needed. When enabled, the auto-save service will operate according to its configured behavior, while when disabled, it will not perform any auto-saving operations and will stop any active timers if necessary.
/// </summary>
public interface IAutoSaveFeature
{
    /// <summary>
    /// Gets a value indicating whether the auto-save service is enabled. When enabled, the service will automatically save data at regular intervals as configured. When disabled, the service will not perform any auto-saving operations, and the timer will be stopped if it is currently running.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>Enables the auto-save service and starts the timer if not already running.</summary>
    void Enable();

    /// <summary>Disables the auto-save service and stops the timer.</summary>
    void Disable();
}
