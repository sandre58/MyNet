// -----------------------------------------------------------------------
// <copyright file="SourceEngineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using DynamicData;
using FluentAssertions;
using MyNet.Observable.Collections.Sources;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Sources;

public sealed class SourceEngineTests
{
    [Fact]
    public void Empty_ShouldBeWritable_AndCountZero()
    {
        using var engine = SourceEngine<int>.Empty();

        engine.IsReadOnly.Should().BeFalse();
        engine.Count.Should().Be(0);
    }

    [Fact]
    public void From_Writable_ShouldExposeItems()
    {
        using var engine = SourceEngine<string>.From(["a", "b", "c"], readOnly: false);

        engine.IsReadOnly.Should().BeFalse();
        engine.Count.Should().Be(3);
    }

    [Fact]
    public void From_ReadOnly_ShouldBeReadOnly_AndCountCorrect()
    {
        using var engine = SourceEngine<string>.From(["x", "y"], readOnly: true);

        engine.IsReadOnly.Should().BeTrue();
        engine.Count.Should().Be(2);
    }

    [Fact]
    public void Add_WhenWritable_ShouldIncrementCount()
    {
        using var engine = SourceEngine<int>.Empty();

        engine.Add(1);
        engine.Add(2);

        engine.Count.Should().Be(2);
    }

    [Fact]
    public void Add_WhenReadOnly_ShouldThrow()
    {
        using var engine = SourceEngine<int>.From([1, 2], readOnly: true);
        var act = () => engine.Add(99);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Remove_WhenWritable_ShouldDecrementCount()
    {
        using var engine = SourceEngine<int>.Empty();
        engine.Add(10);
        engine.Add(20);

        engine.Remove(10);

        engine.Count.Should().Be(1);
    }

    [Fact]
    public void Clear_WhenWritable_ShouldResetCountToZero()
    {
        using var engine = SourceEngine<int>.From([1, 2, 3], readOnly: false);

        engine.Clear();

        engine.Count.Should().Be(0);
    }

    [Fact]
    public void FromSnapshot_ShouldLoadItemsAndBeReadOnly()
    {
        using var engine = SourceEngine<int>.FromSnapshot(() => [100, 200, 300]);

        engine.IsReadOnly.Should().BeTrue();
        engine.Count.Should().Be(3);
    }

    [Fact]
    public void FromSnapshot_Refresh_ShouldReloadItems()
    {
        var items = new[] { 1, 2, 3 };
        using var engine = SourceEngine<int>.FromSnapshot(() => items);

        engine.Count.Should().Be(3);

        items = [10, 20, 30, 40];
        engine.Refresh();

        engine.Count.Should().Be(4);
    }

    [Fact]
    public void FromObservable_ShouldTrackExternalCount()
    {
        var source = new SourceList<string>();
        source.Edit(l =>
        {
            l.Add("a");
            l.Add("b");
        });

        using var engine = SourceEngine<string>.FromObservable(source.Connect());

        engine.IsReadOnly.Should().BeTrue();
        engine.Count.Should().Be(2);

        source.Edit(l => l.Add("c"));
        engine.Count.Should().Be(3);

        source.Edit(l => l.Remove("a"));
        engine.Count.Should().Be(2);
    }
}
