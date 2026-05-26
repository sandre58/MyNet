// -----------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Collections;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        /// Filters out null elements from the sequence and returns a non-nullable enumerable of the remaining items.
        /// </summary>
        /// <returns>A non-nullable enumerable containing the non-null elements.</returns>
        public IEnumerable<T> NotNull() => source.Where(e => e is not null).Select(e => e!);

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> to an <see cref="ObservableCollection{T}"/>, reusing the source when possible.
        /// </summary>
        /// <returns>An observable collection containing the sequence elements.</returns>
        public ObservableCollection<T> ToObservableCollection()
        {
            ArgumentNullException.ThrowIfNull(source);

            return source as ObservableCollection<T> ?? new(source);
        }

        /// <summary>
        /// Iterates the sequence and invokes the provided action for each element.
        /// </summary>
        /// <param name="action">The action to invoke for each element.</param>
        public void ForEach(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(action);

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Iterates the sequence and invokes the provided action for each element including the element index.
        /// </summary>
        /// <param name="action">The action to invoke for each element receiving the element and its zero-based index.</param>
        public void ForEach(Action<T, int> action)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(action);

            var i = 0;
            foreach (var item in source)
            {
                action(item, i);
                i++;
            }
        }

        /// <summary>
        /// Sums a sequence of <see cref="TimeSpan"/> values projected from the source.
        /// </summary>
        /// <param name="selector">A selector that returns a <see cref="TimeSpan"/> for each element.</param>
        /// <returns>The aggregated <see cref="TimeSpan"/>.</returns>
        public TimeSpan Sum(Func<T, TimeSpan> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return source.Aggregate(TimeSpan.Zero, (current, item) => current + selector(item));
        }

        /// <summary>
        /// Rotates the sequence by the specified offset, moving the first <paramref name="offset"/> elements to the end.
        /// </summary>
        /// <param name="offset">The number of elements to skip from the start.</param>
        /// <returns>A rotated sequence.</returns>
        public IEnumerable<T> Rotate(int offset)
        {
            var list = source as IList<T> ?? [.. source];

            var count = list.Count;
            if (count == 0)
                yield break;

            offset %= count;
            if (offset < 0)
                offset += count;

            for (var i = 0; i < count; i++)
                yield return list[(i + offset) % count];
        }

        /// <summary>
        /// Returns the average of a projected integer sequence or <paramref name="defaultValue"/> when the sequence is empty.
        /// </summary>
        public double AverageOrDefault(Func<T, int> selector, int defaultValue = 0)
            => source.AverageOrDefault(x => (double)selector(x), defaultValue);

        /// <summary>
        /// Returns the average of a projected double sequence or <paramref name="defaultValue"/> when the sequence is empty.
        /// </summary>
        public double AverageOrDefault(Func<T, double> selector, double defaultValue = 0)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            var sum = 0.0d;
            var count = 0;

            foreach (var item in source)
            {
                sum += selector(item);
                count++;
            }

            return count == 0 ? defaultValue : sum / count;
        }

        /// <summary>
        /// Returns the average of a projected decimal sequence or <paramref name="defaultValue"/> when the sequence is empty.
        /// </summary>
        public decimal AverageOrDefault(Func<T, decimal> selector, decimal defaultValue = 0)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            var sum = 0.0m;
            var count = 0;

            foreach (var item in source)
            {
                sum += selector(item);
                count++;
            }

            return count == 0 ? defaultValue : sum / count;
        }

        /// <summary>
        /// Returns the maximum projected value from the sequence or <paramref name="defaultValue"/> when the sequence is empty.
        /// </summary>
        public TResult MaxOrDefault<TResult>(Func<T, TResult> selector, TResult defaultValue = default)
            where TResult : struct, IComparable
        {
            ArgumentNullException.ThrowIfNull(source);

            using var e = source.GetEnumerator();
            if (!e.MoveNext())
                return defaultValue;

            var max = selector(e.Current);

            while (e.MoveNext())
            {
                var current = selector(e.Current);
                if (current.CompareTo(max) > 0)
                    max = current;
            }

            return max;
        }

        /// <summary>
        /// Returns the minimum projected value from the sequence or <paramref name="defaultValue"/> when the sequence is empty.
        /// </summary>
        public TResult MinOrDefault<TResult>(Func<T, TResult> selector, TResult defaultValue = default)
            where TResult : struct, IComparable
        {
            ArgumentNullException.ThrowIfNull(source);

            using var e = source.GetEnumerator();
            if (!e.MoveNext())
                return defaultValue;

            var min = selector(e.Current);

            while (e.MoveNext())
            {
                var current = selector(e.Current);
                if (current.CompareTo(min) < 0)
                    min = current;
            }

            return min;
        }

        /// <summary>
        /// Generates round-robin pairings for the sequence items. If the number of items is odd, a default placeholder is added to make pairing possible.
        /// </summary>
        /// <returns>A collection of rounds, each round being a sequence of pairs.</returns>
        public IEnumerable<IEnumerable<(T Item1, T Item2)>> RoundRobin()
        {
            var list = source.ToList();

            if (list.Count % 2 != 0)
                list.Add(default!);

            var n = list.Count;
            var rounds = n - 1;

            for (var r = 0; r < rounds; r++)
            {
                var round = new List<(T, T)>(n / 2);

                for (var i = 0; i < n / 2; i++)
                {
                    var a = list[i];
                    var b = list[n - 1 - i];

                    if (a is not null && b is not null)
                        round.Add((a, b));
                }

                yield return round;

                // rotate (circle method)
                var last = list[^1];

                list.RemoveAt(list.Count - 1);
                list.Insert(1, last);
            }
        }
    }

    extension<T, TId>(IEnumerable<T> source)
        where T : IIdentifiable<TId>
    {
        /// <summary>
        /// Finds an item by its identifier or returns the default when not found.
        /// </summary>
        /// <param name="id">The identifier to search for.</param>
        /// <returns>The matching element or <c>null</c> if not found.</returns>
        public T? GetByIdOrDefault(TId? id)
        {
            ArgumentNullException.ThrowIfNull(source);

            foreach (var item in source)
            {
                if (Equals(item.Id, id))
                    return item;
            }

            return default;
        }

        /// <summary>
        /// Finds an item by its identifier or throws when not found.
        /// </summary>
        public T GetById(TId id)
        {
            var result = source.GetByIdOrDefault(id);

            return result ?? throw new KeyNotFoundException($"Id '{id}' not found.");
        }

        /// <summary>
        /// Determines whether any element in the sequence has the specified identifier.
        /// </summary>
        public bool HasId(TId id)
        {
            ArgumentNullException.ThrowIfNull(source);

            return source.Any(item => Equals(item.Id, id));
        }
    }

    extension(IEnumerable<string> source)
    {
        /// <summary>
        /// Filters out null or empty string elements from the sequence and returns a non-nullable enumerable of the remaining items.
        /// </summary>
        /// <returns>A non-nullable enumerable containing the non-null and non-empty string elements.</returns>
        public IEnumerable<string> NotNullOrEmpty() => source.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => e);
    }

    /// <summary>
    /// Returns the average of a sequence of integers or 0 when the sequence is empty.
    /// </summary>
    public static double AverageOrDefault(this IEnumerable<int> source) => source.AverageOrDefault(x => x);

    /// <summary>
    /// Returns the average of a sequence of doubles or 0 when the sequence is empty.
    /// </summary>
    public static double AverageOrDefault(this IEnumerable<double> source) => source.AverageOrDefault(x => x);

    /// <summary>
    /// Returns the average of a sequence of decimals or 0 when the sequence is empty.
    /// </summary>
    public static decimal AverageOrDefault(this IEnumerable<decimal> source) => source.AverageOrDefault(x => x);
}
