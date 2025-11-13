// -----------------------------------------------------------------------
// <copyright file="IPagingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MyNet.UI.ViewModels.List.Paging;

/// <summary>
/// Represents a view model for managing pagination of a collection.
/// Provides commands and properties for navigating through pages of data.
/// </summary>
public interface IPagingViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when the paging configuration has changed (page number or page size).
    /// </summary>
    event EventHandler<PagingChangedEventArgs>? PagingChanged;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// Changing this value triggers a <see cref="PagingChanged"/> event.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Gets the current page number (1-based index).
    /// </summary>
    int CurrentPage { get; }

    /// <summary>
    /// Gets the total number of pages based on <see cref="TotalItems"/> and <see cref="PageSize"/>.
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    int TotalItems { get; }

    /// <summary>
    /// Gets the command to move to the first page.
    /// Enabled when not on the first page.
    /// </summary>
    ICommand MoveToFirstPageCommand { get; }

    /// <summary>
    /// Gets the command to move to the last page.
    /// Enabled when not on the last page.
    /// </summary>
    ICommand MoveToLastPageCommand { get; }

    /// <summary>
    /// Gets the command to move to the previous page.
    /// Enabled when not on the first page.
    /// </summary>
    ICommand MoveToPreviousPageCommand { get; }

    /// <summary>
    /// Gets the command to move to the next page.
    /// Enabled when not on the last page.
    /// </summary>
    ICommand MoveToNextPageCommand { get; }

    /// <summary>
    /// Gets the command to move to a specific page.
    /// Parameter: int (page number, 1-based).
    /// Enabled when the target page is valid and different from current page.
    /// </summary>
    ICommand MoveToPageCommand { get; }

    /// <summary>
    /// Gets the command to set the page size.
    /// Parameter: int (new page size).
    /// Enabled when the value is greater than 0.
    /// </summary>
    ICommand SetPageSizeCommand { get; }

    /// <summary>
    /// Moves to the specified page number (1-based).
    /// </summary>
    /// <param name="value">The target page number.</param>
    void MoveToPage(int value);

    /// <summary>
    /// Moves to the next page if not on the last page.
    /// </summary>
    void MoveToNextPage();

    /// <summary>
    /// Moves to the previous page if not on the first page.
    /// </summary>
    void MoveToPreviousPage();

    /// <summary>
    /// Moves to the first page.
    /// </summary>
    void MoveToFirstPage();

    /// <summary>
    /// Moves to the last page.
    /// </summary>
    void MoveToLastPage();

    /// <summary>
    /// Updates the paging state based on a response from a paging operation.
    /// </summary>
    /// <param name="response">The paging response containing total items, current page, and total pages.</param>
    void Update(PagingResponse response);
}
