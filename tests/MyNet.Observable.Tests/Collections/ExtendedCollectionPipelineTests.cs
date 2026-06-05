// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionPipelineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Sorting;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class ExtendedCollectionPipelineTests
{
    [Fact]
    public void FilterAndSort_AppliesBothOperations()
    {
        using var collection = ExtendedCollection.From([5, 1, 4, 2, 3]);

        collection.SetFilter(new ExpressionFilter<int>(x => x % 2 == 0));
        collection.SetSorting(new ExpressionSortingProperty<int>(x => x, ListSortDirection.Descending));

        collection.Count.Should().Be(2);
        collection.Should().Equal(4, 2);
        collection.SourceCount.Should().Be(5);
    }

    [Fact]
    public void InvalidateSort_ReordersFilteredView()
    {
        using var collection = ExtendedCollection.From([3, 1, 2]);

        collection.SetSorting(new ExpressionSortingProperty<int>(x => x));
        collection.Should().Equal(1, 2, 3);

        collection.SetSorting(new ExpressionSortingProperty<int>(x => x, ListSortDirection.Descending));

        collection.Should().Equal(3, 2, 1);
    }

    [Fact]
    public void Paging_ExposesOnlyCurrentPageInItems()
    {
        using var collection = ExtendedCollection.FromReadOnly(Enumerable.Range(1, 250));

        collection.SetPaging(1, 100);

        collection.Count.Should().Be(250);
        collection.FilteredItems.Should().HaveCount(250);
        collection.FilteredItems.Should().Equal(Enumerable.Range(1, 250));
        collection.Items.Should().HaveCount(100);
        collection.Items.Should().Equal(Enumerable.Range(1, 100));

        collection.SetPaging(2, 100);

        collection.FilteredItems.Should().HaveCount(250);
        collection.Items.Should().HaveCount(100);
        collection.Items.Should().Equal(Enumerable.Range(101, 100));
    }

    [Fact]
    public void ClearPaging_ExposesFullFilteredCollectionInItems()
    {
        using var collection = ExtendedCollection.FromReadOnly(Enumerable.Range(1, 250));

        collection.SetPaging(1, 100);
        collection.ClearPaging();

        collection.Items.Should().HaveCount(250);
    }
}
