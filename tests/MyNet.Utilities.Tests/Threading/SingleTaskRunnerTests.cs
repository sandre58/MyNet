// -----------------------------------------------------------------------
// <copyright file="SingleTaskRunnerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Threading;
using Xunit;

namespace MyNet.Utilities.Tests.Threading;

[SuppressMessage("Performance", "CA1849:Call async methods when in an async method", Justification = "Test code")]
public class SingleTaskRunnerTests
{
    [Fact]
    public async Task Run_WhileAlreadyRunning_ReturnsFalse_AndNotifiesStartStopAsync()
    {
        var started = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var states = new ConcurrentQueue<bool>();

        using var runner = new SingleTaskRunner(async _ =>
            {
                started.TrySetResult(null);
                await release.Task.ConfigureAwait(true);
            },
            onRunningChanged: states.Enqueue);

        Assert.True(runner.Run());
        await started.Task.ConfigureAwait(true);
        Assert.False(runner.Run());

        var currentTask = runner.CurrentTask;
        Assert.NotNull(currentTask);
        release.TrySetResult(null);
        await currentTask.ConfigureAwait(true);

        Assert.Equal([true, false], [.. states]);
        Assert.False(runner.IsRunning);
    }

    [Fact]
    public async Task RunAsync_WhenAlreadyRunning_ReturnsNullAsync()
    {
        var release = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var runner = new SingleTaskRunner(_ => release.Task);

        var firstTask = runner.RunAsync();
        var secondTask = runner.RunAsync();

        Assert.NotNull(firstTask);
        Assert.Null(secondTask);

        release.TrySetResult(null);
        await firstTask.ConfigureAwait(true);
    }

    [Fact]
    public async Task Cancel_TriggersOnCancelledCallbackAsync()
    {
        var cancelled = false;
        using var runner = new SingleTaskRunner(
            token => Task.Delay(Timeout.InfiniteTimeSpan, token),
            onCancelled: () => cancelled = true);

        Assert.True(runner.Run());
        var currentTask = runner.CurrentTask;
        Assert.NotNull(currentTask);
        runner.Cancel();
        await currentTask.ConfigureAwait(true);

        Assert.True(cancelled);
        Assert.False(runner.IsRunning);
    }

    [Fact]
    public async Task ActionThrows_IsSwallowed_AndLoggedAsync()
    {
        var logger = new CaptureLogger();

        using var runner = new SingleTaskRunner(
            _ => throw new InvalidOperationException("boom"),
            logger: logger);

        Assert.True(runner.Run());
        var currentTask = runner.CurrentTask;
        Assert.NotNull(currentTask);
        await currentTask.ConfigureAwait(true);

        Assert.Contains(logger.Entries, e => e is { Level: LogLevel.Error, Exception: InvalidOperationException });
    }

    [Fact]
    public void Run_AfterDispose_ThrowsObjectDisposedException()
    {
        var runner = new SingleTaskRunner(_ => Task.CompletedTask);
        runner.Dispose();

        Assert.Throws<ObjectDisposedException>(() => runner.Run());
    }

    private sealed class CaptureLogger : ILogger
    {
        public List<(LogLevel Level, string Message, Exception? Exception)> Entries { get; } = [];

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception), exception));

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
