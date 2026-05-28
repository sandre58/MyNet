// -----------------------------------------------------------------------
// <copyright file="ColumnOptionsViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyNet.UI.ViewModels.Display.Options;

/// <summary>
/// Represents configurable column options for list display mode.
/// </summary>
public sealed class ColumnOptionsViewModel : ObservableCollection<DisplayColumnOptionViewModel>
{
    private readonly HashSet<string>? _defaultVisibleColumns;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnOptionsViewModel"/> class.
    /// </summary>
    /// <param name="defaultVisibleColumns">Optional default visible column identifiers.</param>
    public ColumnOptionsViewModel(IEnumerable<string>? defaultVisibleColumns = null)
    {
        if (defaultVisibleColumns is not null)
            _defaultVisibleColumns = new(defaultVisibleColumns, StringComparer.Ordinal);
    }

    /// <summary>
    /// Occurs when the list consumer should refresh column rendering.
    /// </summary>
    public event EventHandler? RefreshRequested;

    /// <summary>
    /// Resets all options and reapplies default visible columns when provided.
    /// </summary>
    public void Reset()
    {
        foreach (var item in this)
            item.Reset();

        if (_defaultVisibleColumns is not null)
            SetDisplayedColumns(_defaultVisibleColumns);

        RequestRefresh();
    }

    /// <summary>
    /// Sets visible columns by identifier.
    /// </summary>
    /// <param name="columns">Identifiers to display.</param>
    public void SetDisplayedColumns(IEnumerable<string> columns)
    {
        var displayed = new HashSet<string>(columns, StringComparer.Ordinal);

        foreach (var item in this)
            item.IsVisible = !item.CanBeHidden || displayed.Contains(item.Identifier);
    }

    /// <summary>
    /// Requests a visual refresh after column changes.
    /// </summary>
    public void RequestRefresh() => RefreshRequested?.Invoke(this, EventArgs.Empty);
}
