// -----------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ListExtensions
{
    extension(IList list)
    {
        /// <summary>
        /// Swaps the elements at the specified indices in the list. If either index is out of range or both indices are the same, the list remains unchanged.
        /// </summary>
        /// <param name="firstIndex">The index of the first element to swap.</param>
        /// <param name="secondIndex">The index of the second element to swap.</param>
        /// <exception cref="ArgumentNullException">Thrown if the list is null.</exception>
        public void Swap(int firstIndex, int secondIndex)
        {
            ArgumentNullException.ThrowIfNull(list);

            if ((uint)firstIndex >= (uint)list.Count || (uint)secondIndex >= (uint)list.Count || firstIndex == secondIndex)
                return;

            (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
        }
    }

    extension<T>(IList<T> list)
    {
        /// <summary>
        /// Sorts the collection in place using the specified key selector.
        /// </summary>
        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(keySelector);

            if (list is List<T> concrete)
            {
                concrete.Sort((a, b) => Comparer<TKey>.Default.Compare(keySelector(a), keySelector(b)));
                return;
            }

            var sorted = list.OrderBy(keySelector).ToArray();
            Replace(list, sorted);
        }

        /// <summary>
        /// Sorts the collection in place using the specified key selector and comparer.
        /// </summary>
        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(keySelector);
            ArgumentNullException.ThrowIfNull(comparer);

            if (list is List<T> concrete)
            {
                concrete.Sort((a, b) => comparer.Compare(keySelector(a), keySelector(b)));
                return;
            }

            var sorted = list.OrderBy(keySelector, comparer).ToArray();
            Replace(list, sorted);
        }

        /// <summary>
        /// Sorts the collection in descending order using the specified key selector.
        /// </summary>
        public void SortDescending<TKey>(Func<T, TKey> keySelector) => list.Sort(keySelector, Comparer<TKey>.Default.Reverse());

        /// <summary>
        /// Sorts the collection in descending order using the specified key selector.
        /// </summary>
        public void SortDescending<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer) => list.Sort(keySelector, comparer.Reverse());

        /// <summary>
        /// Gets the element at the specified index or returns a default value if the index is out of range.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <param name="defaultValue">The default value to return if the index is out of range.</param>
        /// <returns>The element at the specified index or the default value if the index is out of range.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the list is null.</exception>
        public T? GetByIndex(int index, T? defaultValue = default)
        {
            ArgumentNullException.ThrowIfNull(list);
            return index >= 0 && index < list.Count ? list[index] : defaultValue;
        }

        public void UpdateFrom<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> sourceKey,
            Func<T, TKey> destKey,
            Action<TSource> add,
            Action<T> remove,
            Action<T, TSource> update)
            where TKey : notnull
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(sourceKey);
            ArgumentNullException.ThrowIfNull(destKey);
            ArgumentNullException.ThrowIfNull(add);
            ArgumentNullException.ThrowIfNull(remove);
            ArgumentNullException.ThrowIfNull(update);

            var sourceList = source as IList<TSource> ?? [.. source];

            var sourceMap = new Dictionary<TKey, TSource>(sourceList.Count);
            foreach (var s in sourceList)
                sourceMap[sourceKey(s)] = s;

            var destMap = new Dictionary<TKey, T>(list.Count);
            foreach (var d in list)
                destMap[destKey(d)] = d;

            // REMOVE
            foreach (var (key, dest) in destMap)
            {
                if (!sourceMap.ContainsKey(key))
                    remove(dest);
            }

            // UPDATE
            foreach (var (key, dest) in destMap)
            {
                if (sourceMap.TryGetValue(key, out var src))
                    update(dest, src);
            }

            // ADD
            foreach (var (key, src) in sourceMap)
            {
                if (!destMap.ContainsKey(key))
                    add(src);
            }
        }
    }

    /// <summary>
    /// Replaces the contents of the list with the specified items. The list is cleared and the items are added in order. If the list is read-only, an exception is thrown.
    /// </summary>
    /// <param name="list">The list to replace the contents of.</param>
    /// <param name="items">The items to replace the list with.</param>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    private static void Replace<T>(IList<T> list, T[] items)
    {
        list.Clear();
        foreach (var item in items)
            list.Add(item);
    }
}
