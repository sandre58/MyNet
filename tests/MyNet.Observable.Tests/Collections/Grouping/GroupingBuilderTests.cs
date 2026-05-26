// -----------------------------------------------------------------------
// <copyright file="GroupingBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Collections.Grouping;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Grouping;

public sealed class GroupingBuilderTests
{
    [Fact]
    public void Build_ReturnsConfiguredGroupingProperties()
    {
        var grouping = GroupingBuilder<string>.Create()
            .ThenBy(x => x[0])
            .ThenBy(x => x.Length)
            .Build();

        Assert.Equal(2, grouping.Count);
    }
}
