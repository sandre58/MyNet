// -----------------------------------------------------------------------
// <copyright file="IImportSourceViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Represents a source that can provide importable items.
/// </summary>
/// <typeparam name="T">The import item type.</typeparam>
public interface IImportSourceViewModel<out T>
    where T : ImportItemViewModel
{
    /// <summary>
    /// Gets the source key.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the source display name.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Initializes the source.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides items from this source.
    /// </summary>
    IReadOnlyCollection<T> ProvideItems();

    /// <summary>
    /// Reloads source data.
    /// </summary>
    void Reload();
}
