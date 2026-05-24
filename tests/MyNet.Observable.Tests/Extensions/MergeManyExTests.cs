// -----------------------------------------------------------------------
// <copyright file="MergeManyExTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using FluentAssertions;
using Xunit;

namespace MyNet.Observable.Tests.Extensions;

public sealed class MergeManyExTests
{
    [Fact]
    public void MergeManyEx_WhenOuterItemRemoved_RemovesInnerItemsFromBoundList()
    {
        var parentA = CreateParent("a1", "a2");
        var parentB = CreateParent("b1");
        using var parents = new SourceList<Parent>();
        parents.Add(parentA);
        parents.Add(parentB);

        using var subscription = parents.Connect()
            .MergeManyEx(p => p.Children.Connect())
            .Bind(out var merged)
            .Subscribe();

        merged.Count.Should().Be(3);

        parents.Remove(parentA);

        merged.Count.Should().Be(1);
        merged.Select(c => c.Name).Should().Equal("b1");
    }

    [Fact]
    public void MergeManyEx_WhenOuterItemRemoved_EmitsRemoveRangeForInnerItems()
    {
        var parent = CreateParent("a1", "a2");
        using var parents = new SourceList<Parent>();
        parents.Add(parent);

        var changeSets = new List<IChangeSet<Child>>();
        using var subscription = parents.Connect()
            .MergeManyEx(p => p.Children.Connect())
            .Subscribe(changeSets.Add);

        changeSets.Clear();
        parents.Remove(parent);

        var removed = changeSets.SelectMany(cs => cs.GetRemovedItems()).Select(c => c.Name).ToList();
        removed.Should().BeEquivalentTo("a1", "a2");
    }

    [Fact]
    public void MergeManyEx_StandardMergeMany_DoesNotRemoveInnerItemsWhenOuterRemoved()
    {
        var parentA = CreateParent("a1", "a2");
        using var parents = new SourceList<Parent>();
        parents.Add(parentA);

        using var subscription = parents.Connect()
            .MergeMany(p => p.Children.Connect())
            .Bind(out var merged)
            .Subscribe();

        merged.Count.Should().Be(2);

        parents.Remove(parentA);

        merged.Count.Should().Be(2);
    }

    [Fact]
    public void MergeManyEx_Keyed_WhenOuterItemRemoved_EmitsRemovesForInnerItems()
    {
        var parentA = CreateKeyedParent("A", "a1", "a2");
        var parentB = CreateKeyedParent("B", "b1");
        using var parents = new SourceCache<KeyedParent, string>(p => p.Id);
        parents.AddOrUpdate(parentA);
        parents.AddOrUpdate(parentB);

        var changeSets = new List<IChangeSet<Child, string>>();
        using var subscription = parents.Connect()
            .MergeManyEx(
                p => p.Children.Connect(),
                c => c.Name)
            .Subscribe(changeSets.Add);

        changeSets.Clear();
        parents.RemoveKey("A");

        var removed = changeSets
            .SelectMany(cs => cs.GetRemovedItems())
            .Select(c => c.Name)
            .ToList();

        removed.Should().BeEquivalentTo("a1", "a2");
    }

    [Fact]
    public void MergeManyEx_Keyed_WhenOuterRemoveRange_EmitsRemovesForAllInnerItems()
    {
        var parentA = CreateKeyedParent("A", "a1");
        var parentB = CreateKeyedParent("B", "b1");
        using var parents = new SourceCache<KeyedParent, string>(p => p.Id);
        parents.AddOrUpdate(parentA);
        parents.AddOrUpdate(parentB);

        var changeSets = new List<IChangeSet<Child, string>>();
        using var subscription = parents.Connect()
            .MergeManyEx(
                p => p.Children.Connect(),
                c => c.Name)
            .Subscribe(changeSets.Add);

        changeSets.Clear();
        parents.Edit(updater =>
        {
            updater.RemoveKey("A");
            updater.RemoveKey("B");
        });

        var removed = changeSets
            .SelectMany(cs => cs.GetRemovedItems())
            .Select(c => c.Name)
            .ToList();

        removed.Should().BeEquivalentTo("a1", "b1");
    }

    [Fact]
    public void MergeManyEx_WhenSourceIsNull_Throws()
    {
        IObservable<IChangeSet<Parent>>? source = null;

        var act = () => source!.MergeManyEx(_ => System.Reactive.Linq.Observable.Empty<IChangeSet<Child>>());

        act.Should().Throw<ArgumentNullException>();
    }

    private static Parent CreateParent(params string[] childNames)
    {
        var parent = new Parent();
        foreach (var name in childNames)
        {
            parent.Children.Add(new(name));
        }

        return parent;
    }

    private static KeyedParent CreateKeyedParent(string id, params string[] childNames)
    {
        var parent = new KeyedParent(id);
        foreach (var name in childNames)
        {
            parent.Children.AddOrUpdate(new Child(name));
        }

        return parent;
    }

    private sealed class Parent
    {
        public SourceList<Child> Children { get; } = new();
    }

    private sealed class KeyedParent(string id)
    {
        public string Id { get; } = id;

        public SourceCache<Child, string> Children { get; } = new(c => c.Name);
    }

    private sealed class Child(string name)
    {
        public string Name { get; } = name;
    }
}
