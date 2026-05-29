// -----------------------------------------------------------------------
// <copyright file="ObservableCollectionDecoratorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class ObservableCollectionDecoratorTests
{
    [Fact]
    public void Constructor_WithNullInner_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => new TestDecorator<int>(null!));

    [Fact]
    public void Operations_ForwardToInnerCollection()
    {
        using var decorator = new TestDecorator<int>(new ObservableRangeCollection<int>());

        decorator.AddRange([1, 2, 3]);
        decorator.Insert(0, 0);
        decorator[1] = 99;
        decorator.Move(1, 3);
        decorator.InsertRange([4, 5], 0);
        decorator.SetCapacity(16);
        decorator.Load([10, 20, 30]);
        Assert.Equal(1, decorator.RemoveAll(x => x == 30));
        Assert.Equal(2, decorator.Count);
        decorator.Remove(10);

        Assert.Single(decorator);
        Assert.Equal(20, decorator[0]);
        Assert.Contains(20, decorator);
        Assert.Equal(0, decorator.IndexOf(20));

        var array = new int[1];
        decorator.CopyTo(array, 0);
        Assert.Equal(20, array[0]);

        using var enumerator = decorator.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(20, enumerator.Current);
    }

    [Fact]
    public void CollectionChanged_IsForwardedFromInner()
    {
        var inner = new ObservableRangeCollection<int>();
        using var decorator = new TestDecorator<int>(inner);
        var changes = 0;

        decorator.CollectionChanged += (_, _) => changes++;
        inner.Add(1);

        Assert.Equal(1, changes);
    }

    [Fact]
    public void PropertyChanged_IsForwardedFromInner()
    {
        var inner = new ObservableRangeCollection<int>();
        using var decorator = new TestDecorator<int>(inner);
        var changes = 0;

        decorator.PropertyChanged += (_, _) => changes++;
        inner.Add(1);

        Assert.True(changes > 0);
    }

    [Fact]
    public void SuspendNotifications_SuppressesEventsUntilDisposed()
    {
        var inner = new ObservableRangeCollection<int>();
        using var decorator = new TestDecorator<int>(inner);
        var changes = 0;

        decorator.CollectionChanged += (_, _) => changes++;

        using (decorator.SuspendNotifications())
        {
            decorator.Add(1);
            decorator.Add(2);
            Assert.Equal(0, changes);
        }

        decorator.Add(3);
        Assert.True(changes >= 1);
    }

    [Fact]
    public void Dispose_UnsubscribesFromInnerEvents()
    {
        var inner = new ObservableRangeCollection<int>();
        var decorator = new TestDecorator<int>(inner);
        var changes = 0;

        decorator.CollectionChanged += (_, _) => changes++;
        decorator.Dispose();
        inner.Add(1);

        Assert.Equal(0, changes);
    }

    private sealed class TestDecorator<T>(IObservableRangeCollection<T> inner) : ObservableCollectionDecorator<T>(inner);
}
