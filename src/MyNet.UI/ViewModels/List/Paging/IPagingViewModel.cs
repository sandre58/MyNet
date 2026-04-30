// -----------------------------------------------------------------------
// <copyright file="IPagingViewModel.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

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
    /// Moves to the specified page number (1-based).
    /// </summary>
    /// <param name="page">The target page number.</param>
    void MoveToPage(int page);

    /// <summary>
    /// Moves to the next page if not on the last page.
    /// </summary>
    void MoveNext();

    /// <summary>
    /// Moves to the previous page if not on the first page.
    /// </summary>
    void MovePrevious();

    /// <summary>
    /// Moves to the first page.
    /// </summary>
    void MoveFirst();

    /// <summary>
    /// Moves to the last page.
    /// </summary>
    void MoveLast();

    /// <summary>
    /// Updates the paging state based on a response from a paging operation.
    /// </summary>
    /// <param name="totalItems">The total number of items across all pages.</param>
    /// <param name="currentPage">The current page number (1-based index).</param>
    void Update(int totalItems, int currentPage);
}
