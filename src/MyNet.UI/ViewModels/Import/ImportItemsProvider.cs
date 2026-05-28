// -----------------------------------------------------------------------
// <copyright file="ImportItemsProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyNet.UI.ViewModels.Import;

/// <summary>
/// Maintains import items loaded from one source at a time.
/// </summary>
/// <typeparam name="T">The import item type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ImportItemsProvider{T}"/> class.
/// </remarks>
/// <param name="sources">The available import sources.</param>
public sealed class ImportItemsProvider<T>(IReadOnlyCollection<IImportSourceViewModel<T>> sources)
    where T : ImportItemViewModel
{
    private IImportSourceViewModel<T>? _lastSourceLoaded;

    /// <summary>
    /// Gets available import sources.
    /// </summary>
    public IReadOnlyCollection<IImportSourceViewModel<T>> Sources { get; } = sources ?? throw new ArgumentNullException(nameof(sources));

    /// <summary>
    /// Gets loaded items.
    /// </summary>
    public ObservableCollection<T> Items { get; } = [];

    /// <summary>
    /// Loads items from the specified source.
    /// </summary>
    public void LoadSource(IImportSourceViewModel<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _lastSourceLoaded = source;
        ReplaceItems(source.ProvideItems());
    }

    /// <summary>
    /// Reloads items from the last loaded source.
    /// </summary>
    public void Reload() => _lastSourceLoaded?.Reload();

    /// <summary>
    /// Clears loaded items.
    /// </summary>
    public void Clear() => Items.Clear();

    private void ReplaceItems(IEnumerable<T> items)
    {
        Items.Clear();

        foreach (var item in items)
            Items.Add(item);
    }
}
