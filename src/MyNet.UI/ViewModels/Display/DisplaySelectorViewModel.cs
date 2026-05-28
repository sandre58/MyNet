// -----------------------------------------------------------------------
// <copyright file="DisplaySelectorViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MyNet.Observable;
using MyNet.UI.Commands;

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Provides a reusable selector implementation for display modes.
/// </summary>
public class DisplaySelectorViewModel : ObservableObject, IDisplaySelectorViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplaySelectorViewModel"/> class.
    /// </summary>
    /// <param name="allowedModes">The allowed display modes.</param>
    /// <param name="defaultMode">The optional default mode.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    public DisplaySelectorViewModel(
        IEnumerable<IDisplayModeViewModel>? allowedModes = null,
        IDisplayModeViewModel? defaultMode = null,
        ICommandFactory? commandFactory = null)
    {
        AllowedModes = [.. allowedModes ?? Enumerable.Empty<IDisplayModeViewModel>()];
        CurrentMode = defaultMode ?? AllowedModes.FirstOrDefault();

        var commands = commandFactory ?? RelayCommandFactory.Default;
        SetModeCommand = commands.Create<IDisplayModeViewModel>(SetMode, CanSetMode);
    }

    /// <inheritdoc />
    public ObservableCollection<IDisplayModeViewModel> AllowedModes { get; }

    /// <inheritdoc />
    public IDisplayModeViewModel? CurrentMode { get; private set => SetProperty(ref field, value); }

    /// <inheritdoc />
    public ICommand SetModeCommand { get; }

    /// <inheritdoc />
    public void SetMode<TMode>()
        where TMode : class, IDisplayModeViewModel
        => CurrentMode = AllowedModes.OfType<TMode>().FirstOrDefault();

    /// <inheritdoc />
    public void SetMode(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        CurrentMode = AllowedModes.FirstOrDefault(x => x.GetType() == type);
    }

    /// <inheritdoc />
    public void SetMode(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        CurrentMode = AllowedModes.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.Ordinal));
    }

    /// <summary>
    /// Adds a mode to <see cref="AllowedModes"/>.
    /// </summary>
    /// <param name="mode">The mode to add.</param>
    /// <param name="isDefault">Whether the added mode should become current mode.</param>
    /// <returns>The current selector instance.</returns>
    public DisplaySelectorViewModel AddMode(IDisplayModeViewModel mode, bool isDefault = false)
    {
        ArgumentNullException.ThrowIfNull(mode);

        AllowedModes.Add(mode);

        if (isDefault || CurrentMode is null)
            CurrentMode = mode;

        return this;
    }

    /// <summary>
    /// Adds a mode of type <typeparamref name="TMode"/> to <see cref="AllowedModes"/>.
    /// </summary>
    /// <typeparam name="TMode">The mode type.</typeparam>
    /// <param name="isDefault">Whether the added mode should become current mode.</param>
    /// <param name="configure">Optional mode configuration callback.</param>
    /// <returns>The current selector instance.</returns>
    public DisplaySelectorViewModel AddMode<TMode>(bool isDefault = false, Action<TMode>? configure = null)
        where TMode : class, IDisplayModeViewModel, new()
    {
        var mode = new TMode();
        configure?.Invoke(mode);
        return AddMode(mode, isDefault);
    }

    private bool CanSetMode(IDisplayModeViewModel? mode) => mode is not null && AllowedModes.Contains(mode);

    private void SetMode(IDisplayModeViewModel? mode)
    {
        if (mode is not null && AllowedModes.Contains(mode))
            CurrentMode = mode;
    }
}
