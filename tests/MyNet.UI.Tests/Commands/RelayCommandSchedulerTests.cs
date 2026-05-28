// -----------------------------------------------------------------------
// <copyright file="RelayCommandSchedulerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using MyNet.UI.Commands;
using MyNet.UI.Threading;
using Xunit;

namespace MyNet.UI.Tests.Commands;

public sealed class RelayCommandSchedulerTests
{
    [Fact]
    public void RelayCommand_RaiseCanExecuteChanged_WithoutProvider_RunsSynchronously()
    {
        var command = new RelayCommand(() => { });
        var raised = false;
        command.CanExecuteChanged += (_, _) => raised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(raised);
    }

    [Fact]
    public void RelayCommand_RaiseCanExecuteChanged_UsesUiSchedulerFromProvider()
    {
        var uiScheduler = new DeferredScheduler();
        var command = new RelayCommand(() => { }, schedulerProvider: new StubSchedulerProvider(uiScheduler));
        var raised = false;
        command.CanExecuteChanged += (_, _) => raised = true;

        command.RaiseCanExecuteChanged();

        Assert.False(raised);

        uiScheduler.RunPending();

        Assert.True(raised);
    }

    [Fact]
    public void RelayCommandOfT_RaiseCanExecuteChanged_UsesUiSchedulerFromProvider()
    {
        var uiScheduler = new DeferredScheduler();
        var command = new RelayCommand<int>(_ => { }, schedulerProvider: new StubSchedulerProvider(uiScheduler));
        var raised = false;
        command.CanExecuteChanged += (_, _) => raised = true;

        command.RaiseCanExecuteChanged();

        Assert.False(raised);

        uiScheduler.RunPending();

        Assert.True(raised);
    }

    [Fact]
    public void AsyncRelayCommand_RaiseCanExecuteChanged_UsesUiSchedulerFromProvider()
    {
        var uiScheduler = new DeferredScheduler();
        var command = new AsyncRelayCommand(() => Task.CompletedTask, schedulerProvider: new StubSchedulerProvider(uiScheduler));
        var raised = false;
        command.CanExecuteChanged += (_, _) => raised = true;

        command.RaiseCanExecuteChanged();

        Assert.False(raised);

        uiScheduler.RunPending();

        Assert.True(raised);
    }

    /// <summary>
    /// Queues scheduled work until <see cref="RunPending"/> is called.
    /// </summary>
    private sealed class DeferredScheduler : IScheduler
    {
        private readonly Queue<Action> _pending = new();
        private readonly object _gate = new();

        public DateTimeOffset Now => DateTimeOffset.UtcNow;

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            lock (_gate)
            {
                _pending.Enqueue(() => action(this, state));
            }

            return Disposable.Empty;
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            => Schedule(state, action);

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
            => Schedule(state, action);

        public void RunPending()
        {
            Action[] batch;
            lock (_gate)
            {
                batch = _pending.ToArray();
                _pending.Clear();
            }

            foreach (var action in batch)
                action();
        }
    }

    private sealed class StubSchedulerProvider(IScheduler ui) : ISchedulerProvider
    {
        public IScheduler Background { get; } = CurrentThreadScheduler.Instance;

        public IScheduler Ui { get; } = ui;
    }
}
