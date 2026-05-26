// -----------------------------------------------------------------------
// <copyright file="WeakEventSourceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
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

        sut.Raise(this, EventArgs.Empty);

        Assert.Equal(0, TransientHandler.LastCount);
    }

    private static void SubscribeTransientHandler(WeakEventSource<EventArgs> source)
    {
        subscribe();

        void subscribe()
        {
            var handler = new TransientHandler();
            TransientHandler.LastCount = 0;
            source.Subscribe(handler.Handle);
        }
    }

    private sealed class TransientHandler
    {
        public static int LastCount { get; set; }

        [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local", Justification = "Instance method to test instance handler collection.")]
        public void Handle(object? sender, EventArgs e) => LastCount++;
    }

    private sealed class CountingHandler
    {
        public static int StaticCount { get; set; }

        public int Count { get; private set; }

        public static void HandleStatic(object? sender, EventArgs e) => StaticCount++;

        public void Handle(object? sender, EventArgs e) => Count++;
    }
}
