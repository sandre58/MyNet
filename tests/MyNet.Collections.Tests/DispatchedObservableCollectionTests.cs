// -----------------------------------------------------------------------
// <copyright file="DispatchedObservableCollectionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class DispatchedObservableCollectionTests
{
    [Fact]
    public void Add_WithImmediateDispatcher_RaisesCollectionChanged()
    {
        var changes = 0;
        var collection = new DispatchedObservableCollection<int>(ImmediateCollectionEventDispatcher.Default);
        collection.CollectionChanged += (_, _) => changes++;

        collection.Add(1);

        Assert.Equal(1, changes);
        Assert.Single(collection);
    }

    [Fact]
    public void Add_WithDeferredDispatcher_RaisesOnDispatch()
    {
        var dispatcher = new DeferredCollectionEventDispatcher();
        var changes = 0;
        var collection = new DispatchedObservableCollection<int>(dispatcher);
        collection.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                changes++;
        };

        collection.Add(7);
        Assert.Equal(0, changes);

        dispatcher.RunPending();

        Assert.Equal(1, changes);
    }

    private sealed class DeferredCollectionEventDispatcher : ICollectionEventDispatcher
    {
        private readonly List<Action> _pending = [];

        public bool CheckAccess() => false;

        public void Dispatch(Action action) => _pending.Add(action);

        public void RunPending()
        {
            foreach (var action in _pending)
                action();

            _pending.Clear();
        }
    }
}
