// -----------------------------------------------------------------------
// <copyright file="AutoSaveTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using MyNet.IO.AutoSave;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class AutoSaveTests
{
    [Fact]
    public void AutoSaveFeature_EnableAndDisable_RaisesChangedOnlyWhenStateChanges()
    {
        var feature = new AutoSaveFeature();
        var changedCount = 0;
        var lastState = false;
        feature.Changed += (_, e) =>
        {
            changedCount++;
            lastState = e.IsEnabled;
        };

        feature.Enable();
        feature.Enable();
        feature.Disable();

        Assert.False(feature.IsEnabled);
        Assert.Equal(2, changedCount);
        Assert.False(lastState);
    }

    [Fact]
    public void AutoSaveEnabledChangedEventArgs_ExposesIsEnabled()
        => Assert.True(new AutoSaveEnabledChangedEventArgs(true).IsEnabled);

    [Fact]
    public async Task AutoSaveEngine_TriggerSaveAsync_ExecutesSaveCoreAsync()
    {
        using var engine = new TestAutoSaveEngine(TimeSpan.FromMinutes(1));

        await engine.TriggerSaveAsync();

        Assert.Equal(1, engine.SaveCount);
        Assert.False(engine.IsSaving);
    }

    [Fact]
    public void AutoSaveEngine_SetInterval_WithNonPositiveValue_ThrowsArgumentOutOfRangeException()
    {
        using var engine = new TestAutoSaveEngine(TimeSpan.FromSeconds(1));

        Assert.Throws<ArgumentOutOfRangeException>(() => engine.SetInterval(TimeSpan.Zero));
    }

    [Fact]
    public async Task AutoSaveEngine_Start_PerformsPeriodicSavesUntilStoppedAsync()
    {
        using var engine = new TestAutoSaveEngine(TimeSpan.FromMilliseconds(20));

        engine.Start();
        await engine.WaitForSavesAsync(2);
        engine.Stop();
        await WaitUntilAsync(() => !engine.IsRunning);

        Assert.True(engine.SaveCount >= 2);
    }

    [Fact]
    public async Task AutoSaveEngine_TriggerSaveAsync_DoesNotRunConcurrentlyAsync()
    {
        using var engine = new TestAutoSaveEngine(TimeSpan.FromMinutes(1));
        engine.BlockSave();

        var first = engine.TriggerSaveAsync();
        await engine.WaitForSaveEnteredAsync();

        var second = engine.TriggerSaveAsync();
        engine.ReleaseSave();

        await Task.WhenAll(first, second);

        Assert.Equal(1, engine.SaveCount);
    }

    private static async Task WaitUntilAsync(Func<bool> predicate, int timeoutMs = 1000)
    {
        var start = Environment.TickCount64;

        while (!predicate())
        {
            if (Environment.TickCount64 - start > timeoutMs)
                throw new TimeoutException("Condition was not met in time.");

            await Task.Delay(10).ConfigureAwait(false);
        }
    }

    private sealed class TestAutoSaveEngine(TimeSpan interval) : AutoSaveEngine(interval, NullLogger.Instance)
    {
        private readonly TaskCompletionSource _saveEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _releaseSave = CreateReleaseSource();
        private TaskCompletionSource _targetReached = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private volatile bool _blockSave;

        public int SaveCount { get; private set; }

        public void BlockSave() => _blockSave = true;

        public void ReleaseSave()
        {
            _blockSave = false;
            _releaseSave.TrySetResult();
        }

        public Task WaitForSaveEnteredAsync() => _saveEntered.Task;

        public Task WaitForSavesAsync(int count)
        {
            if (SaveCount >= count) return Task.CompletedTask;

            _targetReached = new(TaskCreationOptions.RunContinuationsAsynchronously);
            _expectedCount = count;
            return _targetReached.Task;
        }

        private int _expectedCount;

        protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
        {
            SaveCount++;
            _saveEntered.TrySetResult();

            if (_blockSave)
                await _releaseSave.Task.WaitAsync(cancellationToken).ConfigureAwait(false);

            if (_expectedCount > 0 && SaveCount >= _expectedCount)
                _targetReached.TrySetResult();
        }

        private static TaskCompletionSource CreateReleaseSource() => new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
