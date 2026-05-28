// -----------------------------------------------------------------------
// <copyright file="IDisplayModeViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Represents a display mode that can be selected by a display selector view model.
/// </summary>
public interface IDisplayModeViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the unique display mode key.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets a value indicating whether this mode overrides the empty source template.
    /// </summary>
    bool OverrideEmptySourceTemplate { get; }

    /// <summary>
    /// Gets a value indicating whether this mode overrides the empty items template.
    /// </summary>
    bool OverrideEmptyItemsTemplate { get; }

    /// <summary>
    /// Resets this mode options to their default values.
    /// </summary>
    void Reset();
}
