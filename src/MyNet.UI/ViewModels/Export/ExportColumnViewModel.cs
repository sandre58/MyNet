// -----------------------------------------------------------------------
// <copyright file="ExportColumnViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// Represents a selectable export column.
/// </summary>
/// <typeparam name="TColumn">The column metadata type.</typeparam>
public sealed class ExportColumnViewModel<TColumn> : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExportColumnViewModel{TColumn}"/> class.
    /// </summary>
    /// <param name="column">The column metadata.</param>
    /// <param name="key">The unique column key.</param>
    /// <param name="isSelected">Whether this column is selected for export.</param>
    public ExportColumnViewModel(TColumn column, string key, bool isSelected)
    {
        Column = column;
        Key = key;
        IsSelected = isSelected;
    }

    /// <summary>
    /// Gets the column metadata.
    /// </summary>
    public TColumn Column { get; }

    /// <summary>
    /// Gets the unique column key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the column is selected.
    /// </summary>
    public bool IsSelected { get; set => SetProperty(ref field, value); }
}
