// -----------------------------------------------------------------------
// <copyright file="SortEngineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Observable.Collections.Sorting;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Sorting;

public sealed class SortEngineTests
{
    private sealed record Item(string Name, int Age);

    [Fact]
    public void Set_SortsByConfiguredKeys()
    {
        using var engine = new SortEngine<Item>();

        var sorting = SortingBuilder<Item>.Create()
            .ThenBy(i => i.Age)
            .ThenBy(i => i.Name)
            .Build();

        engine.Set(sorting);

        var comparer = new SortingComparer<Item>(engine.Current);
        var items = new List<Item>
        {
            new("Bob", 30),
            new("Alice", 30),
            new("Zoe", 20)
        };
        items.Sort(comparer);

        Assert.Equal(["Zoe", "Alice", "Bob"], items.ConvertAll(i => i.Name));
    }

    [Fact]
    public void Clear_PreservesSourceOrder()
    {
        using var engine = new SortEngine<Item>();

        engine.Set(SortingBuilder<Item>.Create().ThenBy(i => i.Name).Build());
        engine.Clear();

        var comparer = new SortingComparer<Item>(engine.Current);
        var left = new Item("B", 1);
        var right = new Item("A", 2);

        Assert.Equal(0, comparer.Compare(left, right));
    }
}
