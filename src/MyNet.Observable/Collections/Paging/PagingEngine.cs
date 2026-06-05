// -----------------------------------------------------------------------
// <copyright file="PagingEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Subjects;

namespace MyNet.Observable.Collections.Paging;

/// <summary>
/// Maintains the active paging window applied after filtering and sorting.
/// </summary>
public sealed class PagingEngine : IDisposable
{
    private readonly BehaviorSubject<(int Page, int PageSize)?> _state = new(null);

    /// <summary>
    /// Gets an observable paging state consumed by the collection pipeline.
    /// A <see langword="null"/> value disables paging.
    /// </summary>
    public IObservable<(int Page, int PageSize)?> State => _state;

    /// <summary>
    /// Gets a value indicating whether paging is currently active.
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// Sets the active page window.
    /// </summary>
    /// <param name="page">The one-based page index.</param>
    /// <param name="pageSize">The page size.</param>
    public void Set(int page, int pageSize)
    {
        if (pageSize <= 0)
        {
            Clear();
            return;
        }

        IsEnabled = true;
        _state.OnNext((Math.Max(page, 1), pageSize));
    }

    /// <summary>
    /// Disables paging and exposes the full filtered collection through the paged view.
    /// </summary>
    public void Clear()
    {
        IsEnabled = false;
        _state.OnNext(null);
    }

    /// <inheritdoc />
    public void Dispose() => _state.Dispose();
}
