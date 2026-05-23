// -----------------------------------------------------------------------
// <copyright file="ExtendedCollectionBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Selection;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class ExtendedCollectionBuilderTests
{
    [Fact]
    public void Build_WithFilterAndSort_ShouldApplyPipeline()
    {
        using var collection = new ExtendedCollectionBuilder<int>()
            .From([3, 1, 4, 2])
            .Where(x => x % 2 == 0)
            .OrderBy(x => x)
            .Build();

        collection.Count.Should().Be(2);
        collection.Should().Equal(2, 4);
        collection.SourceCount.Should().Be(4);
    }

    [Fact]
    public void BuildSelectable_ShouldExposeSelection()
    {
        using var selectable = new ExtendedCollectionBuilder<string>()
            .From(["a", "b", "c"])
            .BuildSelectable(SelectionMode.Multiple);

        selectable.Select("a");
        selectable.Select("c");

        selectable.SelectedCount.Should().Be(2);
        selectable.SelectedItems.Should().BeEquivalentTo(["a", "c"]);
    }

    [Fact]
    public void BuildSelectable_ShouldDisposeOwnedCollection()
    {
        var selectable = new ExtendedCollectionBuilder<int>()
            .From([1, 2])
            .BuildSelectable();

        selectable.Dispose();

        selectable.Collection.IsDisposed.Should().BeTrue();
    }
}
