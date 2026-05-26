// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Collections;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CollectionExtensions
{
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// Replaces the content of the collection with the specified items.
        /// </summary>
        /// <param name="items">The items to set in the collection.</param>
        public void Set(IEnumerable<T>? items)
        {
            collection.Clear();
            collection.AddRange(items);
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="items">The items to add to the collection.</param>
        public void AddRange(IEnumerable<T>? items)
        {
            ArgumentNullException.ThrowIfNull(collection);

            var materialized = items?.ToArray() ?? [];
            foreach (var item in materialized)
            {
                collection.Add(item);
            }
        }
    }
}
