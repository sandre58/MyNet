// -----------------------------------------------------------------------
// <copyright file="IDateNavigableDisplayModeViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Represents a display mode that exposes date navigation options.
/// </summary>
public interface IDateNavigableDisplayModeViewModel : IDisplayModeViewModel
{
    /// <summary>
    /// Gets the currently displayed date.
    /// </summary>
    DateTime DisplayDate { get; }

    /// <summary>
    /// Gets the command that navigates to the previous date range.
    /// </summary>
    ICommand MoveToPreviousDateCommand { get; }

    /// <summary>
    /// Gets the command that navigates to the next date range.
    /// </summary>
    ICommand MoveToNextDateCommand { get; }

    /// <summary>
    /// Gets the command that navigates to today.
    /// </summary>
    ICommand MoveToTodayCommand { get; }
}
