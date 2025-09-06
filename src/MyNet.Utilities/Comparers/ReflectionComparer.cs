// -----------------------------------------------------------------------
// <copyright file="ReflectionComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyNet.Utilities.Comparers;

/// <summary>
/// Compares objects by reflecting on specified property paths and applying the provided sort descriptions.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
public class ReflectionComparer<T>(IList<ReflectionSortDescription> sortDescriptions) : IComparer, IComparer<T>
{
    /// <inheritdoc />
    public int Compare(object? x, object? y) => Compare((T?)x, (T?)y);

    /// <inheritdoc />
    public int Compare(T? x, T? y)
    {
        var result = 0;

        foreach (var item in sortDescriptions)
        {
            var obj1 = x?.GetDeepPropertyValue(item.Path);
            var obj2 = y?.GetDeepPropertyValue(item.Path);

            result = obj1 switch
            {
                IComparable toCompare1 => toCompare1.CompareTo(obj2),
                null => obj2 != null ? -1 : 0,
                _ => obj2 != null ? 1 : -1
            };
            result *= item.Direction == ListSortDirection.Descending ? -1 : 1;

            if (result != 0)
            {
                break;
            }
        }

        return result;
    }
}

/// <summary>
/// Describes a property path and direction to use when comparing via <see cref="ReflectionComparer{T}"/>.
/// </summary>
public class ReflectionSortDescription(string path, ListSortDirection direction = ListSortDirection.Ascending)
{
    /// <summary>
    /// Gets the property path used to locate the value on the object graph.
    /// </summary>
    public string Path { get; } = path;

    /// <summary>
    /// Gets the sort direction for this description.
    /// </summary>
    public ListSortDirection Direction { get; } = direction;
}
