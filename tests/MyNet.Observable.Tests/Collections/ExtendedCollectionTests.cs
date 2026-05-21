// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.Observable.Collections.Sorting;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class ExtendedCollectionTests
{
    // ── Factory / IsReadOnly ──────────────────────────────────────────────────
    [Fact]
    public void Create_ShouldBeWritable_AndEmpty()
    {
        using var col = ExtendedCollection.Create<int>();

        col.IsReadOnly.Should().BeFalse();
        col.Count.Should().Be(0);
    }

    [Fact]
    public void From_ShouldExposeItems()
    {
        using var col = ExtendedCollection.From([1, 2, 3]);

        col.Count.Should().Be(3);
        col.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void FromReadOnly_ShouldBeReadOnly()
    {
        using var col = ExtendedCollection.FromReadOnly([1, 2]);

        col.IsReadOnly.Should().BeTrue();
        col.Count.Should().Be(2);
    }

    // ── Add / Remove ──────────────────────────────────────────────────────────
    [Fact]
    public void Add_ShouldIncreaseBothCountAndSourceCount()
    {
        using var col = ExtendedCollection.Create<int>();

        col.Add(42);

        col.Count.Should().Be(1);
        col.SourceCount.Should().Be(1);
    }

    [Fact]
    public void Remove_ShouldDecreaseCount()
    {
        using var col = ExtendedCollection.From([1, 2, 3]);

        col.Remove(2);

        col.Count.Should().Be(2);
        col.Should().NotContain(2);
    }

    [Fact]
    public void Clear_ShouldEmptyCollection()
    {
        using var col = ExtendedCollection.From([1, 2, 3]);

        col.Clear();

        col.Count.Should().Be(0);
        col.SourceCount.Should().Be(0);
    }

    // ── Filter ────────────────────────────────────────────────────────────────
    [Fact]
    public void SetFilter_ShouldReduceVisibleItems()
    {
        using var col = ExtendedCollection.From([1, 2, 3, 4, 5]);

        col.SetFilter(new ExpressionFilter<int>(x => x % 2 == 0));

        col.Count.Should().Be(2);
        col.Should().BeEquivalentTo([2, 4]);
    }

    [Fact]
    public void SetFilter_SourceCount_ShouldRemainTotal()
    {
        using var col = ExtendedCollection.From([1, 2, 3, 4, 5]);

        col.SetFilter(new ExpressionFilter<int>(x => x > 3));

        col.SourceCount.Should().Be(5);
    }

    [Fact]
    public void ClearFilter_ShouldRestoreAllItems()
    {
        using var col = ExtendedCollection.From([1, 2, 3]);
        col.SetFilter(new ExpressionFilter<int>(x => x > 1));

        col.ClearFilter();

        col.Count.Should().Be(3);
    }

    // ── Sorting ───────────────────────────────────────────────────────────────
    [Fact]
    public void SetSorting_ShouldOrderItems()
    {
        using var col = ExtendedCollection.From([3, 1, 2]);

        col.SetSorting(new ExpressionSortingProperty<int>(x => x));

        col.Should().BeInAscendingOrder();
    }

    // ── Grouping ──────────────────────────────────────────────────────────────
    [Fact]
    public void SetGrouping_ShouldProduceGroups()
    {
        using var col = ExtendedCollection.From(["apple", "ant", "banana", "cherry", "avocado"]);

        var groups = new List<IReadOnlyList<CollectionGroup<string>>>();
        using var sub = col.ConnectGroups().Subscribe(onNext);

        col.SetGrouping(new ExpressionGroupingProperty<string>(x => x[0]));

        var latest = groups[^1];
        latest.Should().HaveCount(3); // 'a', 'b', 'c'
        latest.Select(g => g.Key).Should().BeEquivalentTo(["a", "b", "c"]);
        return;
        void onNext(IReadOnlyList<CollectionGroup<string>> g) => groups.Add(g);
    }

    [Fact]
    public void ClearGrouping_ShouldProduceEmptyGroups()
    {
        using var col = ExtendedCollection.From(["apple", "banana"]);
        col.SetGrouping(new ExpressionGroupingProperty<string>(x => x[0]));

        var groups = new List<IReadOnlyList<CollectionGroup<string>>>();
        using var sub = col.ConnectGroups().Subscribe(onNext);

        col.ClearGrouping();

        groups[^1].Should().BeEmpty();
        return;
        void onNext(IReadOnlyList<CollectionGroup<string>> g) => groups.Add(g);
    }

    // ── Dispose ───────────────────────────────────────────────────────────────
    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        var col = ExtendedCollection.From([1, 2, 3]);

        var act = col.Dispose;

        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_ShouldSetIsDisposed()
    {
        var col = ExtendedCollection.Create<int>();

        col.Dispose();

        col.IsDisposed.Should().BeTrue();
    }
}
