// -----------------------------------------------------------------------
// <copyright file="PagingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Observable;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List.Paging;

public class PagingViewModel : EditableObject, IPagingViewModel
{
    private readonly Deferrer _deferrer;

    public PagingViewModel(int pageSize = 25)
    {
        PageSize = pageSize;
        _deferrer = new(RaisePagingChanged);
    }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// Changing this property triggers a <see cref="PagingChanged"/> event.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the current page number (1-based index).
    /// Updated via <see cref="Update"/> method or navigation commands.
    /// </summary>
    [AlsoNotifyFor(nameof(HasNextPage), nameof(HasPreviousPage))]
    public int CurrentPage { get; private set; } = 1;

    /// <summary>
    /// Gets the total number of pages based on <see cref="TotalItems"/> and <see cref="PageSize"/>.
    /// Updated via <see cref="Update"/> method.
    /// </summary>
    [AlsoNotifyFor(nameof(HasNextPage))]
    public int TotalPages { get; private set; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// Updated via <see cref="Update"/> method.
    /// </summary>
    public int TotalItems { get; private set; }

    /// <summary>
    /// Gets a value indicating whether there is a next page available.
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Gets a value indicating whether there is a previous page available.
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Occurs when the paging configuration has changed (page number or page size).
    /// </summary>
    public event EventHandler<PagingChangedEventArgs>? PagingChanged;

    /// <summary>
    /// Moves to the specified page number (1-based).
    /// Raises the <see cref="PagingChanged"/> event if the page is valid and different from the current page.
    /// </summary>
    /// <param name="page">The target page number (must be between 1 and <see cref="TotalPages"/>).</param>
    public void MoveToPage(int page)
    {
        if (page < 1 || page > TotalPages || page == CurrentPage)
            return;

        RequestPage(page);
    }

    /// <summary>
    /// Moves to the next page if not on the last page.
    /// </summary>
    public void MoveNext() => MoveToPage(CurrentPage + 1);

    /// <summary>
    /// Moves to the previous page if not on the first page.
    /// </summary>
    public void MovePrevious() => MoveToPage(CurrentPage - 1);

    /// <summary>
    /// Moves to the first page.
    /// </summary>
    public void MoveFirst() => MoveToPage(1);

    /// <summary>
    /// Moves to the last page.
    /// </summary>
    public void MoveLast() => MoveToPage(TotalPages);

    /// <summary>
    /// Updates the paging state based on a response from a paging operation.
    /// </summary>
    /// <param name="totalItems">The total number of items across all pages.</param>
    /// <param name="currentPage">The current page number (1-based index).</param>
    public void Update(int totalItems, int currentPage)
    {
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
        CurrentPage = currentPage;
    }

    /// <summary>
    /// Requests a page change to the specified page number.
    /// </summary>
    /// <param name="page">The target page number (must be between 1 and <see cref="TotalPages"/>).</param>
    private void RequestPage(int page)
    {
        CurrentPage = page;
        _deferrer.DeferOrExecute();
    }

    /// <summary>
    /// Invoked when the paging configuration has changed (page number or page size).
    /// </summary>
    private void RaisePagingChanged() => PagingChanged?.Invoke(this, new(CurrentPage, PageSize));

    /// <summary>
    /// Invoked when the page size has changed.
    /// </summary>
    protected virtual void OnPageSizeChanged() => RequestPage(1);
}
