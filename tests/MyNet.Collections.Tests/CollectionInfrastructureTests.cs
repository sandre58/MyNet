// -----------------------------------------------------------------------
// <copyright file="CollectionInfrastructureTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using Xunit;

namespace MyNet.Collections.Tests;

public sealed class CollectionInfrastructureTests
{
    [Fact]
    public void ImmediateCollectionEventDispatcher_CheckAccess_ReturnsTrue()
    {
        var dispatcher = ImmediateCollectionEventDispatcher.Default;

        Assert.True(dispatcher.CheckAccess());
    }

    [Fact]
    public void ImmediateCollectionEventDispatcher_Dispatch_ExecutesAction()
    {
        var executed = false;

        ImmediateCollectionEventDispatcher.Default.Dispatch(() => executed = true);

        Assert.True(executed);
    }

    [Fact]
    public void ImmediateCollectionSynchronizer_ReadWrite_ExecuteWithoutLocking()
    {
        var synchronizer = ImmediateCollectionSynchronizer.Default;

        synchronizer.Write(() => { });
        synchronizer.Read(() => { });

        Assert.Equal(21, synchronizer.Read(() => 21));
        Assert.Equal(42, synchronizer.Write(() => 42));
    }

    [Fact]
    public void SynchronizationContextCollectionEventDispatcher_CaptureCurrentOrImmediate_WithoutContext_UsesImmediate()
    {
        SynchronizationContext.SetSynchronizationContext(null);

        var dispatcher = SynchronizationContextCollectionEventDispatcher.CaptureCurrentOrImmediate();

        Assert.Same(ImmediateCollectionEventDispatcher.Default, dispatcher);
    }

    [Fact]
    public void SynchronizationContextCollectionEventDispatcher_Dispatch_WithContext_PostsAction()
    {
        using var gate = new ManualResetEventSlim(false);
        var context = new TestSynchronizationContext(gate);
        var dispatcher = new SynchronizationContextCollectionEventDispatcher(context);
        var executed = false;

        dispatcher.Dispatch(() => executed = true);

        Assert.True(gate.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(executed);
    }

    [Fact]
    public void SynchronizationContextCollectionEventDispatcher_CheckAccess_WithMatchingContext_ReturnsTrue()
    {
        var context = new SynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(context);
        var dispatcher = new SynchronizationContextCollectionEventDispatcher(context);

        try
        {
            Assert.True(dispatcher.CheckAccess());
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(null);
        }
    }

    [Fact]
    public void SynchronizationContextCollectionEventDispatcher_WithNullContext_RunsImmediately()
    {
        var dispatcher = new SynchronizationContextCollectionEventDispatcher(null);
        var executed = false;

        dispatcher.Dispatch(() => executed = true);

        Assert.True(executed);
        Assert.True(dispatcher.CheckAccess());
    }

    private sealed class TestSynchronizationContext(ManualResetEventSlim gate) : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object? state)
        {
            d(state);
            gate.Set();
        }
    }
}
