// -----------------------------------------------------------------------
// <copyright file="SortingBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using MyNet.Observable.Collections.Sorting;
using Xunit;

namespace MyNet.Observable.Tests.Collections.Sorting;

public sealed class SortingBuilderTests
{
    [Fact]
    public void Build_ReturnsConfiguredSortKeys()
    {
        var sorting = SortingBuilder<Item>.Create()
            .ThenByDescending(x => x.Score)
            .ThenBy(x => x.Name)
            .Build();

        Assert.Equal(2, sorting.Count);
        Assert.Equal(ListSortDirection.Descending, sorting[0].Direction);
        Assert.Equal(ListSortDirection.Ascending, sorting[1].Direction);
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used for testing purposes only.")]
    private sealed record Item(string Name, int Score);
}
