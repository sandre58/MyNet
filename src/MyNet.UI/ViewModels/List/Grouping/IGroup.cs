// -----------------------------------------------------------------------
// <copyright file="IGroup.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.UI.ViewModels.List.Grouping;

/// <summary>
/// Defines a group of items of type T, identified by a key. This interface is used to represent grouped items in the list view model.
/// </summary>
/// <typeparam name="T">The type of the items in the group.</typeparam>
public interface IGroup<out T>
{
    /// <summary>
    /// Gets the key that identifies the group.
    /// </summary>
    object? Key { get; }

    /// <summary>
    /// Gets the items in the group.
    /// </summary>
    IReadOnlyList<T> Items { get; }
}
