// -----------------------------------------------------------------------
// <copyright file="FilterGroupTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Collections.Filters;
using MyNet.Primitives;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Filters;

public sealed class FilterGroupTests
{
    [Fact]
    public void ProvideExpression_OrGroup_MatchesAnyChild()
    {
        var group = new FilterGroup<int>(
            LogicalOperator.Or,
            [
                new ExpressionFilter<int>(x => x > 10),
                new ExpressionFilter<int>(x => x < 0)
            ]);

        var predicate = group.ProvideExpression().Compile();

        Assert.True(predicate(15));
        Assert.True(predicate(-1));
        Assert.False(predicate(5));
    }

    [Fact]
    public void ProvideExpression_AndGroup_RequiresAllChildren()
    {
        var group = new FilterGroup<int>(
            LogicalOperator.And,
            [
                new ExpressionFilter<int>(x => x > 0),
                new ExpressionFilter<int>(x => x % 2 == 0)
            ]);

        var predicate = group.ProvideExpression().Compile();

        Assert.True(predicate(4));
        Assert.False(predicate(3));
        Assert.False(predicate(-2));
    }

    [Fact]
    public void ProvideExpression_EmptyGroup_MatchesAll()
    {
        var group = new FilterGroup<int>(LogicalOperator.And, []);

        var predicate = group.ProvideExpression().Compile();

        Assert.True(predicate(0));
        Assert.True(predicate(99));
    }
}
