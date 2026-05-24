// -----------------------------------------------------------------------
// <copyright file="SubscriberLists.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Utilities.Execution;

/// <summary>
/// Helper for managing lists of items associated to subscribers (e.g. for tracking subscriptions of a subscriber to multiple observables).
/// </summary>
internal static class SubscriberLists
{
    /// <summary>
    /// Adds an item to the list associated to the specified subscriber in the dictionary, creating a new list if the subscriber is not already present.
    /// </summary>
    /// <param name="dictionary">The dictionary mapping subscribers to their lists of items.</param>
    /// <param name="subscriber">The subscriber for whom the item is being added.</param>
    /// <param name="item">The item to add to the subscriber's list.</param>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public static void Add<T>(Dictionary<object, List<T>> dictionary, object subscriber, T item)
    {
        if (!dictionary.TryGetValue(subscriber, out var list))
        {
            list = [];
            dictionary[subscriber] = list;
        }

        list.Add(item);
    }
}
