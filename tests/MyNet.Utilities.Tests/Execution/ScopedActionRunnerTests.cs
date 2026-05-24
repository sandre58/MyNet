// -----------------------------------------------------------------------
// <copyright file="ScopedActionRunnerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Utilities.Execution;
using Xunit;

namespace MyNet.Utilities.Tests.Execution;

public class ScopedActionRunnerTests
{
    [Fact]
    public void Run_InvokesStartAndEndHandlers()
    {
        var startCount = 0;
        var endCount = 0;

        using var runner = new ScopedActionRunner(() => { });
        runner.RegisterOnStart(this, () => startCount++);
        runner.RegisterOnEnd(this, () => endCount++);

        runner.Run();

        Assert.Equal(1, startCount);
        Assert.Equal(1, endCount);
        Assert.False(runner.IsRunning);
    }

    [Fact]
    public void Run_CreatesAndDisposesRegisteredScopes()
    {
        var scopeCreated = false;
        var scopeDisposed = false;

        using var runner = new ScopedActionRunner(() => { });
        runner.RegisterScope(this, () => new Scope(() => scopeDisposed = true, () => scopeCreated = true));

        runner.Run();

        Assert.True(scopeCreated);
        Assert.True(scopeDisposed);
    }

    [Fact]
    public void Run_WaitsForManualCompletion()
    {
        var ended = false;

        using var runner = new ScopedActionRunner(complete =>
        {
            Assert.False(ended);
            complete();
        });

        runner.Run();

        ended = true;
        Assert.False(runner.IsRunning);
    }

    [Fact]
    public void Run_ThrowsWhenReenteredBeforeManualCompletion()
    {
        ScopedActionRunner? runner = null;
        runner = new(complete =>
        {
            Assert.Throws<InvalidOperationException>(() => runner!.Run());
            complete();
        });

        runner.Run();
        Assert.False(runner.IsRunning);
    }

    [Fact]
    public void GenericRun_PropagatesResultToHandlers()
    {
        string? startValue = null;
        string? endValue = null;

        using var runner = new ScopedActionRunner<int, string>(input => _ = input.ToString(CultureInfo.CurrentCulture));
        runner.RegisterOnStart(this, value => startValue = value);
        runner.RegisterOnEnd(this, value => endValue = value);

        runner.Run(42, () => "initial");

        Assert.Equal("initial", startValue);
        Assert.Equal("initial", endValue);
    }

    private sealed class Scope : IDisposable
    {
        private readonly Action _onDispose;

        public Scope(Action onDispose, Action onCreate)
        {
            _onDispose = onDispose;
            onCreate();
        }

        public void Dispose() => _onDispose();
    }
}
