// -----------------------------------------------------------------------
// <copyright file="PagingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.Utilities.Deferring;
using PropertyChanged;

namespace MyNet.UI.ViewModels.List.Paging;

/// <summary>
/// ViewModel for managing paging state and navigation in a paginated list or collection.
/// </summary>
public class PagingViewModel : EditableObject, IPagingViewModel
{
    private readonly DeferredAction _deferredAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingViewModel"/> class with an optional page size.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="commandFactory">Optional command factory used to create commands.</param>
    public PagingViewModel(int pageSize = 25, ICommandFactory? commandFactory = null)
    {
        var commands = commandFactory ?? RelayCommandFactory.Default;

        _deferredAction = new(RaisePagingChanged);

        MoveNextCommand = commands.Create(MoveNext, () => HasNextPage);
        MovePreviousCommand = commands.Create(MovePrevious, () => HasPreviousPage);
        MoveFirstCommand = commands.Create(MoveFirst, () => HasPreviousPage);
        MoveLastCommand = commands.Create(MoveLast, () => HasNextPage);

        PageSize = pageSize;
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

    /// <inheritdoc />
    public ICommand MoveNextCommand { get; }

    /// <inheritdoc />
    public ICommand MovePreviousCommand { get; }

    /// <inheritdoc />
    public ICommand MoveFirstCommand { get; }

    /// <inheritdoc />
    public ICommand MoveLastCommand { get; }

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
        TotalPages = PageSize <= 0 ? 0 : (int)Math.Ceiling((double)totalItems / PageSize);
        CurrentPage = TotalPages == 0 ? 1 : Math.Min(Math.Max(1, currentPage), TotalPages);
    }

    /// <summary>
    /// Requests a page change to the specified page number.
    /// </summary>
    /// <param name="page">The target page number (must be between 1 and <see cref="TotalPages"/>).</param>
    private void RequestPage(int page)
    {
        CurrentPage = page;
        _deferredAction.Request();
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
