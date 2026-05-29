// -----------------------------------------------------------------------
// <copyright file="SynchronizedObservableCollectionOperationsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class SynchronizedObservableCollectionOperationsTests
{
    [Fact]
    public void Operations_WorkThroughSynchronizer()
    {
        var inner = new ObservableRangeCollection<int>();
        var sut = new SynchronizedObservableCollection<int>(inner, ImmediateCollectionSynchronizer.Default);

        sut.AddRange([1, 2, 3]);
        sut.Insert(0, 0);
        sut[2] = 99;
        sut.InsertRange([4, 5], 1);
        sut.SetCapacity(32);
        sut.Load([10, 20, 30]);
        Assert.Equal(1, sut.RemoveAll(x => x > 20));
        sut.RemoveRange(0, 1);

        Assert.Single(sut);
        Assert.Equal(20, sut[0]);
        Assert.Contains(20, sut);
        Assert.Equal(0, sut.IndexOf(20));
        Assert.False(sut.Contains(999));

        var array = new int[1];
        sut.CopyTo(array, 0);
        Assert.Equal(20, array[0]);
        Assert.Equal([20], sut.ToList());
    }

    [Fact]
    public void CopyTo_WithNullArray_ThrowsArgumentNullException()
    {
        var sut = new SynchronizedObservableCollection<int>([]);

        Assert.Throws<ArgumentNullException>(() => sut.CopyTo(null!, 0));
    }

    [Fact]
    public void DispatchedExtension_CreatesDispatchedCollection()
    {
        var collection = new ObservableRangeCollection<int>();
        using var dispatched = collection.Dispatched(ImmediateCollectionEventDispatcher.Default);

        dispatched.Add(1);

        Assert.Single(dispatched);
    }
}
