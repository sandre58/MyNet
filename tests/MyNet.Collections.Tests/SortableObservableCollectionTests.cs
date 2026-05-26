// -----------------------------------------------------------------------
// <copyright file="SortableObservableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace MyNet.Collections.Tests;

public class SortableObservableCollectionTests
{
    [Fact]
    public void SortBy_ShouldSortAscending()
    {
        var collection = new ObservableRangeCollection<int>();
        collection.AddRange([5, 3, 8, 1, 9, 2]);

        collection.SortBy(x => x);

        Assert.Equal(new[] { 1, 2, 3, 5, 8, 9 }, collection);
    }

    [Fact]
    public void SortBy_ShouldSortDescending()
    {
        var collection = new ObservableRangeCollection<int>();
        collection.AddRange([5, 3, 8, 1, 9, 2]);

        collection.SortBy(x => x, ListSortDirection.Descending);

        Assert.Equal(new[] { 9, 8, 5, 3, 2, 1 }, collection);
    }

    [Fact]
    public void SortBy_ShouldSortComplexObjects()
    {
        var collection = new ObservableRangeCollection<Product>
        {
            new() { Name = "A", Price = 50 },
            new() { Name = "B", Price = 20 },
            new() { Name = "C", Price = 80 },
            new() { Name = "D", Price = 10 }
        };

        collection.SortBy(p => p.Price);

        Assert.Equal(10, collection[0].Price);
        Assert.Equal(20, collection[1].Price);
        Assert.Equal(50, collection[2].Price);
        Assert.Equal(80, collection[3].Price);
    }

    [Fact]
    public void DefaultSorter_FindInsertIndex_ShouldReturnExpectedPosition()
    {
        var sorter = DefaultCollectionSorter<int>.Default;
        IReadOnlyList<int> sorted = [1, 3, 5, 7, 9];

        var index = sorter.FindInsertIndex(sorted, 6, x => x);

        Assert.Equal(3, index);
    }

    [Fact]
    public void SortBy_WithCustomSorter_ShouldUseProvidedStrategy()
    {
        var collection = new ObservableRangeCollection<int>();
        collection.AddRange([1, 2, 3]);
        var sorter = new ReverseCollectionSorter<int>();

        collection.SortBy(x => x, sorter: sorter);

        Assert.Equal(new[] { 3, 2, 1 }, collection);
    }

    private sealed class Product
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Used for sorting tests")]
        public string Name { get; init; } = string.Empty;

        public decimal Price { get; init; }
    }

    private sealed class ReverseCollectionSorter<T> : ICollectionSorter<T>
    {
        public IReadOnlyList<T> Sort(IEnumerable<T> source, Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending)
            => [.. source.Reverse()];

        public int FindInsertIndex(IReadOnlyList<T> source, T item, Func<T, object> selector, ListSortDirection direction = ListSortDirection.Ascending)
            => 0;
    }
}
