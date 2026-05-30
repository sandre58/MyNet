// -----------------------------------------------------------------------
// <copyright file="ExtendedObservableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Specialized;
using System.Linq;
using MyNet.Observable.Collections;
using Xunit;

namespace MyNet.Observable.Tests.Collections;

public sealed class ExtendedObservableCollectionTests
{
    [Fact]
    public void Constructor_FromEnumerable_CopiesItems()
    {
        var sut = new ExtendedObservableCollection<int>([1, 2, 3]);

        Assert.Equal(3, sut.Count);
        Assert.Equal([1, 2, 3], [.. sut]);
    }

    [Fact]
    public void Add_RaisesCollectionChanged_WithAddedItem()
    {
        var sut = new ExtendedObservableCollection<int>();

        NotifyCollectionChangedEventArgs? last = null;
        sut.CollectionChanged += (_, e) => last = e;

        sut.Add(42);

        Assert.NotNull(last);
        Assert.Equal(NotifyCollectionChangedAction.Add, last!.Action);
        Assert.NotNull(last.NewItems);
        Assert.Equal([42], [.. last.NewItems.Cast<int>()]);
    }
}
