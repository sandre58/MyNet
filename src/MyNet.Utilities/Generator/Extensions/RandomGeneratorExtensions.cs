// -----------------------------------------------------------------------
// <copyright file="RandomGeneratorExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class RandomGeneratorExtensions
{
    extension(IRandomGenerator random)
    {
        /// <summary>
        /// Generates a random subset of the provided list with a specified count. The method first checks if the input list is null and throws an ArgumentNullException if it is. Then, it calculates a safe count by clamping the requested count between 0 and the total number of items in the list, ensuring that it does not exceed the list's bounds. Finally, it uses the random generator to select a subset of items from the list based on the calculated safe count, providing a way to retrieve a random selection of items without risking an out-of-range error.
        /// </summary>
        /// <param name="list">The list from which to generate a random subset.</param>
        /// <param name="count">The number of items to include in the subset.</param>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <returns>A random subset of the list with the specified count.</returns>
        public IReadOnlyList<T> SafeSubset<T>(IReadOnlyCollection<T> list, int count)
        {
            ArgumentNullException.ThrowIfNull(list);

            var safeCount = count.SafeClamp(0, list.Count);

            return random.Subset(list, safeCount);
        }

        /// <summary>
        /// Attempts to retrieve a random item from the provided list. If the list is empty, it returns the default value for the type T (which is null for reference types and zero for value types). This method ensures that it does not throw an exception when trying to access an item from an empty list, providing a safe way to handle such scenarios.
        /// </summary>
        /// <param name="list">The list from which to retrieve a random item.</param>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <returns>A random item from the list, or the default value if the list is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the list is null.</exception>
        public T? TryItem<T>(IReadOnlyCollection<T> list)
        {
            ArgumentNullException.ThrowIfNull(list);

            return list.Count == 0 ? default : random.Item(list);
        }

        /// <summary>
        /// Generates a random DateTime value that falls within the specified interval. The method extracts the start and end values from the provided interval and generates a random date between them. If the interval does not have defined start or end values, it defaults to DateTime.MinValue and DateTime.MaxValue respectively, ensuring that the generated date is valid and falls within the expected range.
        /// </summary>
        /// <param name="interval">The interval specifying the start and end DateTime values.</param>
        /// <returns>A random DateTime value within the specified interval.</returns>
        public DateTime Date(IInterval<DateTime> interval) => random.Date(interval.Start?.Value, interval.End?.Value);

        /// <summary>
        /// Generates a random DateTime value that falls within the specified minimum and maximum range. If the minimum or maximum values are not provided, it defaults to DateTime.MinValue and DateTime.MaxValue respectively, ensuring that the generated date is valid and does not exceed the provided bounds.
        /// </summary>
        /// <param name="min">The minimum DateTime value.</param>
        /// <param name="max">The maximum DateTime value.</param>
        /// <returns>A random DateTime value within the specified range.</returns>
        public DateTime Date(DateTime? min = null, DateTime? max = null) => random.SafeDate(min ?? DateTime.MinValue, max ?? DateTime.MaxValue);

        /// <summary>
        /// Generates a random DateTime value that falls within the specified minimum and maximum range, ensuring that the generated date is valid and does not exceed the provided bounds.
        /// </summary>
        /// <param name="min">The minimum DateTime value.</param>
        /// <param name="max">The maximum DateTime value.</param>
        /// <returns>A random DateTime value within the specified range.</returns>
        public DateTime SafeDate(DateTime? min = null, DateTime? max = null)
        {
            var minMax = min.MinMax(max);
            return random.Date(minMax.Min, minMax.Max);
        }
    }
}
