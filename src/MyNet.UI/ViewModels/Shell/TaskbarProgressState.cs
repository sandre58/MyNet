// -----------------------------------------------------------------------
// <copyright file="TaskbarProgressState.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Specifies the state of the progress indicator in the Windows taskbar.
/// </summary>
public enum TaskbarProgressState
{
    /// <summary>No progress indicator is displayed in the taskbar button.</summary>
    None = 0,

    /// <summary>A pulsing green indicator is displayed in the taskbar button.</summary>
    Indeterminate = 1,

    /// <summary>A green progress indicator is displayed in the taskbar button.</summary>
    Normal = 2,

    /// <summary>A red progress indicator is displayed in the taskbar button.</summary>
    Error = 3,

    /// <summary>A yellow progress indicator is displayed in the taskbar button.</summary>
    Paused = 4
}
