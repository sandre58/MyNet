// -----------------------------------------------------------------------
// <copyright file="PagingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using MyNet.Observable;
using MyNet.UI.Collections;
using MyNet.UI.Commands;
using MyNet.Utilities;

namespace MyNet.UI.ViewModels.List.Paging;

/// <summary>
/// View model for managing pagination of a collection.
/// Provides commands for navigation (first, last, next, previous pages) and tracks paging state.
/// </summary>
/// <remarks>
/// This implementation supports:
/// <list type="bullet">
/// <item>Dynamic page size changes</item>
/// <item>Navigation commands with proper enabled states</item>
/// <item>Observable collection of available page numbers</item>
/// <item>Event notifications when paging changes</item>
/// </list>
/// </remarks>
public class PagingViewModel : ObservableObject, IPagingViewModel
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed in Cleanup")]
    private readonly UiObservableCollection<int> _pages = [];

    #region Properties

    /// <summary>
    /// Gets the total number of items across all pages.
    /// Updated via <see cref="Update"/> method.
    /// </summary>
    public int TotalItems { get; private set; }

    /// <summary>
    /// Gets the current page number (1-based index).
    /// Updated via <see cref="Update"/> method or navigation commands.
    /// </summary>
    public int CurrentPage { get; private set; } = 1;

    /// <summary>
    /// Gets the total number of pages based on <see cref="TotalItems"/> and <see cref="PageSize"/>.
    /// Updated via <see cref="Update"/> method.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// Changing this property triggers a <see cref="PagingChanged"/> event.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets the observable collection of available page numbers.
    /// Automatically updated when <see cref="TotalPages"/> changes.
    /// </summary>
    public ReadOnlyObservableCollection<int> Pages { get; }

    #endregion

    #region Commands

    /// <summary>
    /// Gets the command to move to the first page.
    /// Enabled when <see cref="CurrentPage"/> > 1.
    /// </summary>
    public ICommand MoveToFirstPageCommand { get; }

    /// <summary>
    /// Gets the command to move to the last page.
    /// Enabled when <see cref="CurrentPage"/> &lt; <see cref="TotalPages"/>.
    /// </summary>
    public ICommand MoveToLastPageCommand { get; }

    /// <summary>
    /// Gets the command to move to the previous page.
    /// Enabled when <see cref="CurrentPage"/> > 1.
    /// </summary>
    public ICommand MoveToPreviousPageCommand { get; }

    /// <summary>
    /// Gets the command to move to the next page.
    /// Enabled when <see cref="CurrentPage"/> &lt; <see cref="TotalPages"/>.
    /// </summary>
    public ICommand MoveToNextPageCommand { get; }

    /// <summary>
    /// Gets the command to move to a specific page.
    /// Parameter: int (page number, 1-based).
    /// Enabled when the target page is valid and different from <see cref="CurrentPage"/>.
    /// </summary>
    public ICommand MoveToPageCommand { get; }

    /// <summary>
    /// Gets the command to set the page size.
    /// Parameter: int (new page size).
    /// Enabled when the value is greater than 0.
    /// </summary>
    public ICommand SetPageSizeCommand { get; }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the paging configuration has changed (page number or page size).
    /// </summary>
    public event EventHandler<PagingChangedEventArgs>? PagingChanged;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingViewModel"/> class.
    /// </summary>
    /// <param name="pageSize">The initial page size. Default is 25.</param>
    public PagingViewModel(int pageSize = 25)
    {
        PageSize = pageSize;
        Pages = new(_pages);

        // Initialize navigation commands with proper enable conditions
        MoveToFirstPageCommand = CommandsManager.Create(MoveToFirstPage, () => CurrentPage > 1);
        MoveToLastPageCommand = CommandsManager.Create(MoveToLastPage, () => CurrentPage < TotalPages);
        MoveToPreviousPageCommand = CommandsManager.Create(MoveToPreviousPage, () => CurrentPage > 1);
        MoveToNextPageCommand = CommandsManager.Create(MoveToNextPage, () => CurrentPage < TotalPages);
        MoveToPageCommand = CommandsManager.Create<int>(MoveToPage, x => x >= 1 && x <= TotalPages && x != CurrentPage);
        SetPageSizeCommand = CommandsManager.Create<int>(x => PageSize = x, x => x > 0);
    }

    #endregion

    #region Navigation Methods

    /// <summary>
    /// Moves to the specified page number (1-based).
    /// Raises the <see cref="PagingChanged"/> event if the page is valid and different from the current page.
    /// </summary>
    /// <param name="value">The target page number (must be between 1 and <see cref="TotalPages"/>).</param>
    public void MoveToPage(int value)
    {
        // Validate page number
        if (value < 1 || value > TotalPages || value == CurrentPage)
            return;

        // Raise paging changed event - actual page change happens in Update()
        PagingChanged?.Invoke(this, new PagingChangedEventArgs(value, PageSize));
    }

    /// <summary>
    /// Moves to the next page if not on the last page.
    /// </summary>
    public void MoveToNextPage() => MoveToPage(CurrentPage + 1);

    /// <summary>
    /// Moves to the previous page if not on the first page.
    /// </summary>
    public void MoveToPreviousPage() => MoveToPage(CurrentPage - 1);

    /// <summary>
    /// Moves to the first page.
    /// </summary>
    public void MoveToFirstPage() => MoveToPage(1);

    /// <summary>
    /// Moves to the last page.
    /// </summary>
    public void MoveToLastPage() => MoveToPage(TotalPages);

    #endregion

    #region State Management

    /// <summary>
    /// Updates the paging state based on a response from a paging operation.
    /// Updates <see cref="TotalItems"/>, <see cref="CurrentPage"/>, <see cref="TotalPages"/>, and the <see cref="Pages"/> collection.
    /// </summary>
    /// <param name="response">The paging response containing total items, current page, and total pages.</param>
    /// <remarks>
    /// <para><strong>Performance:</strong> This method is optimized to minimize UI updates.</para>
    /// <list type="bullet">
    /// <item>Only updates the Pages collection if the count has changed</item>
    /// <item>Uses Set() instead of Clear() + AddRange() to minimize notifications</item>
    /// </list>
    /// </remarks>
    public void Update(PagingResponse response)
    {
        TotalItems = response.TotalItems;
        CurrentPage = response.CurrentPage;
        TotalPages = response.TotalPages;

        // Performance optimization: Only update pages if count changed
        // This avoids unnecessary UI updates when navigating between pages
        if (_pages.Count != TotalPages)
        {
            // Use Set() for single notification instead of Clear() + AddRange() (two notifications)
            _pages.Set(Enumerable.Range(1, TotalPages));
        }
    }

    /// <summary>
    /// Called when <see cref="PageSize"/> changes.
    /// Raises the <see cref="PagingChanged"/> event to trigger a refresh with the new page size.
    /// </summary>
    protected void OnPageSizeChanged()
        => PagingChanged?.Invoke(this, new PagingChangedEventArgs(CurrentPage, PageSize));

    #endregion

    protected override void Cleanup()
    {
        base.Cleanup();
        _pages.Dispose();
    }
}
