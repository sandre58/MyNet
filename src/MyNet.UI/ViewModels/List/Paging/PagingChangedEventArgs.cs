// -----------------------------------------------------------------------
// <copyright file="PagingChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.List.Paging;

/// <summary>
/// Provides data for the PagingChanged event, containing the new page number and page size.
/// </summary>
/// <param name="page">The target page number (1-based).</param>
/// <param name="pageSize">The number of items per page.</param>
public class PagingChangedEventArgs(int page, int pageSize) : EventArgs
{
    /// <summary>
    /// Gets the target page number (1-based index).
    /// </summary>
    public int Page { get; } = page;

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int PageSize { get; } = pageSize;
}
