// -----------------------------------------------------------------------
// <copyright file="IColumnOptionsDisplayModeViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Input;
using MyNet.Collections;
using MyNet.Observable;
using MyNet.UI.ViewModels.Display.Options;

namespace MyNet.UI.ViewModels.Display;

/// <summary>
/// Represents a display mode that supports column configuration.
/// </summary>
public interface IColumnOptionsDisplayModeViewModel : IDisplayModeViewModel
{
    /// <summary>
    /// Gets the column layout options.
    /// </summary>
    ColumnOptionsViewModel ColumnOptions { get; }

    /// <summary>
    /// Gets predefined column presets.
    /// </summary>
    ObservableRangeCollection<LabeledValue<string[]>> PresetColumns { get; }

    /// <summary>
    /// Gets the command that applies the selected displayed columns.
    /// </summary>
    ICommand SetDisplayedColumnsCommand { get; }

    /// <summary>
    /// Applies the specified columns as displayed columns.
    /// </summary>
    /// <param name="columns">The columns to display.</param>
    void SetDisplayedColumns(IEnumerable<string> columns);
}
