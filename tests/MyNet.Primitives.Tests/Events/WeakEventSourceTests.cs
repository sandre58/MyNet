// -----------------------------------------------------------------------
// <copyright file="WeakEventSourceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Events;
using Xunit;

namespace MyNet.Primitives.Tests.Events;

public sealed class WeakEventSourceTests
{
    [Fact]
    public void Raise_WithInstanceHandler_InvokesHandler()
    {
        var sut = new WeakEventSource<EventArgs>();
        var handler = new CountingHandler();
        sut.Subscribe(handler.Handle);

        sut.Raise(this, EventArgs.Empty);

        Assert.Equal(1, handler.Count);
    }

    [Fact]
    public void Raise_WithStaticHandler_InvokesHandler()
    {
        var sut = new WeakEventSource<EventArgs>();
        CountingHandler.StaticCount = 0;
        sut.Subscribe(CountingHandler.HandleStatic);

        sut.Raise(this, EventArgs.Empty);

        Assert.Equal(1, CountingHandler.StaticCount);
    }

    [Fact]
    public void Unsubscribe_RemovesHandler()
    {
        var sut = new WeakEventSource<EventArgs>();
        var handler = new CountingHandler();
        EventHandler<EventArgs> callback = handler.Handle;
        sut.Subscribe(callback);
        sut.Unsubscribe(callback);

        sut.Raise(this, EventArgs.Empty);

        Assert.Equal(0, handler.Count);
    }

    [Fact]
    public void Raise_AfterSubscriberCollected_DoesNotInvokeHandler()
    {
        var sut = new WeakEventSource<EventArgs>();
        SubscribeTransientHandler(sut);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var ex = Record.Exception(() => sut.Raise(this, EventArgs.Empty));

        Assert.Null(ex);
        Assert.Equal(0, TransientHandler.LastCount);
    }

    private static void SubscribeTransientHandler(WeakEventSource<EventArgs> source)
    {
        TransientHandler.LastCount = 0;
        source.Subscribe(TransientHandler.Handle);
    }

    private static class TransientHandler
    {
        public static int LastCount { get; set; }

        public static void Handle(object? sender, EventArgs e) => LastCount++;
    }

    private sealed class CountingHandler
    {
        public static int StaticCount { get; set; }

        public int Count { get; private set; }

        public static void HandleStatic(object? sender, EventArgs e) => StaticCount++;

        public void Handle(object? sender, EventArgs e) => Count++;
    }
}
