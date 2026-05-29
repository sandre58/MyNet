// -----------------------------------------------------------------------
// <copyright file="ITaskbarProgressSource.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Observable taskbar progress state for shell chrome binding and explicit updates (for example errors).
/// </summary>
public interface ITaskbarProgressSource : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the current taskbar progress state.
    /// </summary>
    TaskbarProgressState ProgressState { get; }

    /// <summary>
    /// Gets the current taskbar progress value.
    /// </summary>
    double ProgressValue { get; }

    /// <summary>
    /// Sets the taskbar progress state and optional value.
    /// </summary>
    void SetProgress(TaskbarProgressState progressState, double? progressValue = null);

    /// <summary>
    /// Sets the taskbar to an error state until the next busy/progress update clears it.
    /// </summary>
    void SetError(double progressValue = 1);
}
