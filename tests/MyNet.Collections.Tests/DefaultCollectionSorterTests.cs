// -----------------------------------------------------------------------
// <copyright file="DefaultCollectionSorterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class DefaultCollectionSorterTests
{
    [Fact]
    public void Sort_Ascending_OrdersBySelector()
    {
        var source = new[] { 3, 1, 2 };

        var sorted = DefaultCollectionSorter<int>.Default.Sort(source, x => x);

        Assert.Equal([1, 2, 3], sorted);
    }

    [Fact]
    public void Sort_Descending_OrdersBySelectorDescending()
    {
        var source = new[] { 3, 1, 2 };

        var sorted = DefaultCollectionSorter<int>.Default.Sort(source, x => x, ListSortDirection.Descending);

        Assert.Equal([3, 2, 1], sorted);
    }

    [Fact]
    public void FindInsertIndex_ReturnsStableInsertionPoint()
    {
        var source = new[] { 1, 3, 5 };

        var index = DefaultCollectionSorter<int>.Default.FindInsertIndex(source, 4, x => x);

        Assert.Equal(2, index);
    }
}
