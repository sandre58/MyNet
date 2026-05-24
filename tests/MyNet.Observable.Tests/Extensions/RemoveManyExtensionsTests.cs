// -----------------------------------------------------------------------
// <copyright file="RemoveManyExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using MyNet.Observable;
using MyNet.Observable.Collections.Sources;
using Xunit;

namespace MyNet.Observable.Tests.Extensions;

public sealed class RemoveManyExtensionsTests
{
    [Fact]
    public void RemoveMany_RemovesSpecifiedItems()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };

        list.RemoveMany([2, 4]);

        list.Should().Equal(1, 3, 5);
    }

    [Fact]
    public void RemoveMany_WhenSourceContainsDuplicateValues_RemovesAllMatchingInstances()
    {
        var list = new List<int> { 1, 1, 2, 3 };

        list.RemoveMany([1, 1]);

        list.Should().Equal(2, 3);
    }

    [Fact]
    public void RemoveMany_WhenItemsToRemoveIsEmpty_DoesNotChangeSource()
    {
        var list = new List<string> { "a", "b" };

        list.RemoveMany([]);

        list.Should().Equal("a", "b");
    }

    [Fact]
    public void RemoveMany_WhenSourceIsNonListCollection_RemovesItems()
    {
        ICollection<int> collection = new HashSet<int> { 1, 2, 3 };

        collection.RemoveMany([2]);

        collection.Should().Equal(1, 3);
    }

    [Fact]
    public void RemoveMany_WhenSourceIsNull_Throws()
    {
        List<int>? list = null;

        var act = () => list!.RemoveMany([1]);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RemoveMany_WhenItemsToRemoveIsNull_Throws()
    {
        var list = new List<int> { 1 };

        var act = () => list.RemoveMany(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SourceEngine_RemoveMany_RemovesFromUnderlyingList()
    {
        using var engine = SourceEngine<int>.From([1, 2, 3, 4], readOnly: false);

        engine.RemoveMany([2, 4]);

        engine.Count.Should().Be(2);
        engine.Remove(1).Should().BeTrue();
        engine.Remove(3).Should().BeTrue();
        engine.Count.Should().Be(0);
    }
}
