// -----------------------------------------------------------------------
// <copyright file="IDisplaySelectorViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Represents a selector that manages available display modes and the active mode.
/// </summary>
public interface IDisplaySelectorViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the available display modes.
    /// </summary>
    ObservableCollection<IDisplayModeViewModel> AllowedModes { get; }

    /// <summary>
    /// Gets the currently selected display mode.
    /// </summary>
    IDisplayModeViewModel? CurrentMode { get; }

    /// <summary>
    /// Gets the command that changes the current display mode.
    /// </summary>
    ICommand SetModeCommand { get; }

    /// <summary>
    /// Selects the first mode assignable to <typeparamref name="TMode"/>.
    /// </summary>
    void SetMode<TMode>()
        where TMode : class, IDisplayModeViewModel;

    /// <summary>
    /// Selects a mode by runtime type.
    /// </summary>
    void SetMode(Type type);

    /// <summary>
    /// Selects a mode by key.
    /// </summary>
    void SetMode(string key);
}
