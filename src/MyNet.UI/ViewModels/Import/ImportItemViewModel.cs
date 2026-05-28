// -----------------------------------------------------------------------
// <copyright file="ImportItemViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Observable;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Base class for importable items.
/// </summary>
public class ImportItemViewModel : ObservableObject
{
    /// <summary>
    /// Gets the import mode for this item.
    /// </summary>
    public ImportMode Mode { get; init; } = ImportMode.Add;

    /// <summary>
    /// Gets or sets a value indicating whether this item is selected in the UI list.
    /// </summary>
    public bool IsSelected { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether this item should be imported.
    /// </summary>
    public bool Import { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Validates this item before import.
    /// </summary>
    /// <returns>A sequence of validation error messages. Empty means valid.</returns>
    public virtual IReadOnlyCollection<string> ValidateForImport() => [];
}
