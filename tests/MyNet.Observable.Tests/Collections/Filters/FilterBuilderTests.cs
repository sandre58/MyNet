// -----------------------------------------------------------------------
// <copyright file="FilterBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Collections.Filters;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Filters;

public sealed class FilterBuilderTests
{
    [Fact]
    public void Build_Empty_ReturnsNull()
    {
        var filter = FilterBuilder<int>.Create().Build();

        Assert.Null(filter);
    }

    [Fact]
    public void Where_And_Or_CombinesWithOrLogic()
    {
        var filter = FilterBuilder<int>.Create()
            .Where(x => x > 10)
            .Or(x => x < 0)
            .Build();

        var predicate = filter!.ProvideExpression().Compile();

        Assert.True(predicate(15));
        Assert.True(predicate(-1));
        Assert.False(predicate(5));
    }

    [Fact]
    public void And_Group_BuildsNestedCriteria()
    {
        var filter = FilterBuilder<int>.Create()
            .Where(x => x > 0)
            .And(b => b.Where(x => x % 2 == 0))
            .Build();

        var predicate = filter!.ProvideExpression().Compile();

        Assert.True(predicate(4));
        Assert.False(predicate(3));
        Assert.False(predicate(-2));
    }
}
