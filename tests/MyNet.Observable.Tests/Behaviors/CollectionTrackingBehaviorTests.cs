// -----------------------------------------------------------------------
// <copyright file="CollectionTrackingBehaviorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using MyNet.Observable.Behaviors;
using Xunit;

namespace MyNet.Observable.Tests.Behaviors;

public sealed class CollectionTrackingBehaviorTests
{
    [Fact]
    public void Constructor_TracksExistingObservableCollection()
    {
        var owner = new OwnerWithItems();
        NotifyCollectionChangedEventArgs? args = null;
        var behavior = new CollectionTrackingBehavior(owner);
        behavior.CollectionChanged += (_, e) => args = e;

        owner.Items.Add("alpha");

        Assert.NotNull(args);
        Assert.Equal(NotifyCollectionChangedAction.Add, args!.Action);
    }

    [Fact]
    public void Track_ManualCollection_RaisesCollectionChanged()
    {
        var owner = new EmptyOwner();
        var behavior = new CollectionTrackingBehavior(owner);
        var manual = new ObservableCollection<int>();
        var changeCount = 0;
        behavior.CollectionChanged += (_, _) => changeCount++;

        behavior.Track(manual);
        manual.Add(1);

        Assert.Equal(1, changeCount);
    }

    [Fact]
    public void Untrack_StopsReceivingChanges()
    {
        var owner = new EmptyOwner();
        var behavior = new CollectionTrackingBehavior(owner);
        var manual = new ObservableCollection<int>();
        var changeCount = 0;
        behavior.CollectionChanged += (_, _) => changeCount++;

        behavior.Track(manual);
        behavior.Untrack(manual);
        manual.Add(1);

        Assert.Equal(0, changeCount);
    }

    private sealed class EmptyOwner : ObservableObject;

    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local", Justification = "Used for testing purposes only.")]
    private sealed class OwnerWithItems : ObservableObject
    {
        public ObservableCollection<string> Items { get; } = [];
    }
}
