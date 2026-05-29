// -----------------------------------------------------------------------
// <copyright file="GroupingModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using MyNet.Observable.Collections.Grouping;
using MyNet.UI.ViewModels.List.Grouping;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class GroupingModelTests
{
    [Fact]
    public void Group_ExposesKeyAndItems()
    {
        var items = new[] { 1, 2, 3 };
        var group = new Group<int>("key", items);

        group.Key.Should().Be("key");
        group.Items.Should().BeSameAs(items);
    }

    [Fact]
    public void Group_WithNullItems_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => new Group<int>("key", null!));

    [Fact]
    public void GroupingChangedEventArgs_ExposesProperties()
    {
        IReadOnlyList<IGroupingProperty<string>> properties =
        [
            new TestGroupingProperty()
        ];

        var args = new GroupingChangedEventArgs<string>(properties);

        args.Grouping.Should().BeSameAs(properties);
    }

    [Fact]
    public void ItemChangedEventArgs_ExposesOldAndNewValues()
    {
        var args = new MyNet.UI.ViewModels.Crud.ItemChangedEventArgs<int>(1, 2);

        args.OldItem.Should().Be(1);
        args.NewItem.Should().Be(2);
    }

    private sealed class TestGroupingProperty : IGroupingProperty<string>
    {
        public Expression<Func<string, object?>> ProvideExpression() => x => x;
    }
}
