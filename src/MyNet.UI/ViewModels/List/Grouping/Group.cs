// -----------------------------------------------------------------------
// <copyright file="Group.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Represents a concrete group of items.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="Group{T}"/> class.
/// </remarks>
/// <param name="key">The group key.</param>
/// <param name="items">The grouped items.</param>
public sealed class Group<T>(object? key, IReadOnlyList<T> items) : IGroup<T>
{
    /// <inheritdoc />
    public object? Key { get; } = key;

    /// <inheritdoc />
    public IReadOnlyList<T> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));
}
