// -----------------------------------------------------------------------
// <copyright file="VirtualizedListViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MyNet.UI.ViewModels.List.Factories;

namespace MyNet.UI.ViewModels.List;

/// <summary>
/// Provides a list view model exposing a virtualized visible window over list items.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class VirtualizedListViewModel<T> : ListViewModel<T>, IVirtualizedListViewModel<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VirtualizedListViewModel{T}"/> class.
    /// </summary>
    public VirtualizedListViewModel(IListDataProvider<T> dataProvider, ListViewModelOptions<T>? options = null)
        : base(dataProvider, options)
    {
        if (Items is INotifyCollectionChanged itemsNotify)
        {
            Disposables.Add(
                System.Reactive.Linq.Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => itemsNotify.CollectionChanged += h,
                        h => itemsNotify.CollectionChanged -= h)
                    .Subscribe(_ => NotifyPropertyChanged(nameof(VisibleItems))));
        }
    }

    /// <inheritdoc />
    public int VisibleStartIndex
    {
        get;
        set
        {
            if (!SetProperty(ref field, Math.Max(0, value)))
                return;

            NotifyPropertyChanged(nameof(VisibleItems));
        }
    }

    /// <inheritdoc />
    public int VisibleCount
    {
        get;
        set
        {
            if (!SetProperty(ref field, Math.Max(1, value)))
                return;

            NotifyPropertyChanged(nameof(VisibleItems));
        }
    }

        = 50;

    /// <inheritdoc />
    public IReadOnlyList<T> VisibleItems
    {
        get
        {
            if (Items.Count == 0)
                return [];

            var start = Math.Min(VisibleStartIndex, Items.Count - 1);
            var count = Math.Min(VisibleCount, Items.Count - start);

            return
            [
                .. Items
                    .Skip(start)
                    .Take(count)
            ];
        }
    }
}
