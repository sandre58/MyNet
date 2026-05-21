// -----------------------------------------------------------------------
// <copyright file="GroupingEngineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MyNet.Observable.Collections.Grouping;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Grouping;

public sealed class GroupingEngineTests
{
    [Fact]
    public void ComputeGroups_NoProperties_ShouldReturnEmpty()
    {
        var result = GroupingEngine<string>.ComputeGroups(["a", "b", "c"], []);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ComputeGroups_SingleProperty_ShouldGroupCorrectly()
    {
        var items = new[] { "apple", "ant", "banana", "cherry", "avocado" };
        var properties = new IGroupingProperty<string>[] { new ExpressionGroupingProperty<string>(x => x[0]) };

        var result = GroupingEngine<string>.ComputeGroups(items, properties);

        result.Should().HaveCount(3);
        result.Select(g => g.Key).Should().BeEquivalentTo(["a", "b", "c"]);
        result.Single(g => g.Key == "a").Items.Should().BeEquivalentTo(["apple", "ant", "avocado"]);
    }

    [Fact]
    public void ComputeGroups_MultipleProperties_ShouldUsePipeDelimitedKey()
    {
        var items = new[] { "aa", "ab", "ba", "bb" };
        var properties = new IGroupingProperty<string>[] { new ExpressionGroupingProperty<string>(x => x[0]), new ExpressionGroupingProperty<string>(x => x[1]) };

        var result = GroupingEngine<string>.ComputeGroups(items, properties);

        result.Should().HaveCount(4);
        result.Select(g => g.Key).Should().BeEquivalentTo(["a|a", "a|b", "b|a", "b|b"]);
    }

    [Fact]
    public void Set_ShouldUpdateCurrentAndNotifySubscribers()
    {
        using var engine = new GroupingEngine<string>();
        var emitted = new List<IGroupingProperty<string>[]>();
        using var sub = engine.Grouping.Subscribe(onNext);

        var prop = new ExpressionGroupingProperty<string>(x => x[0]);
        engine.Set([prop]);

        emitted.Should().HaveCount(2); // initial empty + new
        engine.Current.Should().HaveCount(1);
        return;
        void onNext(IGroupingProperty<string>[] g) => emitted.Add(g);
    }

    [Fact]
    public void Clear_ShouldResetCurrentAndNotifySubscribers()
    {
        using var engine = new GroupingEngine<string>();
        engine.Set([new ExpressionGroupingProperty<string>(x => x[0])]);

        var emitted = new List<IGroupingProperty<string>[]>();
        using var sub = engine.Grouping.Subscribe(onNext);

        engine.Clear();

        engine.Current.Should().BeEmpty();
        emitted[^1].Should().BeEmpty();
        return;
        void onNext(IGroupingProperty<string>[] g) => emitted.Add(g);
    }
}
