// -----------------------------------------------------------------------
// <copyright file="CollectionGroup.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Observable.Collections.Grouping;

/// <summary>
/// Represents a group of items produced by the core grouping pipeline, identified by a composite string key.
/// This is a core (UI-agnostic) type. UI layers may map it to their own presentation model.
/// </summary>
/// <typeparam name="T">The type of the items in the group.</typeparam>
/// <param name="Key">The composite key identifying the group.</param>
/// <param name="Items">The items belonging to this group.</param>
public readonly record struct CollectionGroup<T>(string Key, IReadOnlyList<T> Items)
    where T : notnull;
