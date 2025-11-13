// -----------------------------------------------------------------------
// <copyright file="PagingResponse.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.List.Paging;

/// <summary>
/// Represents the response from a paging operation containing pagination metadata.
/// This is a lightweight readonly struct optimized for performance.
/// </summary>
/// <param name="currentPage">The current page number (1-based index).</param>
/// <param name="totalPages">The total number of pages.</param>
/// <param name="totalItems">The total number of items across all pages.</param>
public readonly struct PagingResponse(int currentPage, int totalPages, int totalItems) : IEquatable<PagingResponse>
{
    /// <summary>
    /// Gets the current page number (1-based index).
    /// </summary>
    public int CurrentPage { get; } = currentPage;

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; } = totalPages;

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    public int TotalItems { get; } = totalItems;

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="PagingResponse"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>true if the specified object is a <see cref="PagingResponse"/> with the same values; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is PagingResponse pagingResponse && Equals(pagingResponse);

    /// <summary>
    /// Indicates whether the current <see cref="PagingResponse"/> is equal to another <see cref="PagingResponse"/>.
    /// </summary>
    /// <param name="other">A <see cref="PagingResponse"/> to compare with this instance.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    bool IEquatable<PagingResponse>.Equals(PagingResponse other) => Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for the current <see cref="PagingResponse"/>.</returns>
    public override int GetHashCode() => HashCode.Combine(CurrentPage, TotalPages, TotalItems);

    /// <summary>
    /// Determines whether two <see cref="PagingResponse"/> instances are equal.
    /// </summary>
    public static bool operator ==(PagingResponse left, PagingResponse right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PagingResponse"/> instances are not equal.
    /// </summary>
    public static bool operator !=(PagingResponse left, PagingResponse right) => !(left == right);
}
