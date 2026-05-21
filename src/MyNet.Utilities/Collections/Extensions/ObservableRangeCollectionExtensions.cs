// -----------------------------------------------------------------------
// <copyright file="ObservableRangeCollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using MyNet.Utilities.Collections;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ObservableRangeCollectionExtensions
{
    extension<T>(ObservableRangeCollection<T> collection)
    {
        /// <summary>
        /// Sorts the collection using an external sorting strategy.
        /// </summary>
        public void SortBy(Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending, ICollectionSorter<T>? sorter = null)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(selector);

            var sorted = (sorter ?? DefaultCollectionSorter<T>.Default).Sort(collection, selector, direction);
            collection.Load(sorted);
        }

        /// <summary>
        /// Wraps the collection in a synchronized adapter.
        /// </summary>
        public SynchronizedObservableCollection<T> Synchronized(ICollectionSynchronizer? synchronizer = null)
        {
            ArgumentNullException.ThrowIfNull(collection);
            return new(collection, synchronizer);
        }

        /// <summary>
        /// Wraps an observable range collection so notifications are dispatched with the specified dispatcher.
        /// </summary>
        public DispatchedObservableCollection<T> Dispatched(ICollectionEventDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(dispatcher);
            return new(collection, dispatcher);
        }
    }
}
