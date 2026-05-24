// -----------------------------------------------------------------------
// <copyright file="SingleTaskDeferrerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyNet.Utilities.Threading;
using Xunit;

namespace MyNet.Utilities.Tests.Threading;

public class SingleTaskDeferrerTests
{
    [Fact]
    public async Task Request_ExecutesActionWhenNotDeferredAsync()
    {
        var executed = false;

        using var deferrer = new SingleTaskDeferrer(_ =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        deferrer.Request();
        await WaitUntilAsync(() => executed);

        Assert.True(executed);
    }

    [Fact]
    public async Task Request_ExecutesOnceAfterDeferScopeEndsAsync()
    {
        var executionCount = 0;

        using var deferrer = new SingleTaskDeferrer(_ =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        using (deferrer.Defer())
        {
            deferrer.Request();
            deferrer.Request();
            Assert.Equal(0, executionCount);
        }

        await WaitUntilAsync(() => executionCount == 1).ConfigureAwait(true);
        Assert.Equal(1, executionCount);
    }

    [Fact]
    public void Request_IsIgnoredWhileSuspended()
    {
        var executionCount = 0;

        using var deferrer = new SingleTaskDeferrer(_ =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        using (deferrer.Suspend())
            deferrer.Request();

        Assert.Equal(0, executionCount);
    }

    [Fact]
    public async Task Request_CancelsRunningTaskAndRerunsAsync()
    {
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var executionCount = 0;

        using var deferrer = new SingleTaskDeferrer(async cancellationToken =>
        {
            executionCount++;

            if (executionCount == 1)
            {
                await release.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
        });

        deferrer.Request();
        await WaitUntilAsync(() => executionCount == 1);

        deferrer.Request();
        release.SetResult();
        await WaitUntilAsync(() => executionCount == 2);

        Assert.Equal(2, executionCount);
    }

    [Fact]
    public async Task Request_ThrottlesExecutionAsync()
    {
        var executionCount = 0;

        using var deferrer = new SingleTaskDeferrer(
            _ =>
            {
                executionCount++;
                return Task.CompletedTask;
            },
            throttleMilliseconds: 50);

        deferrer.Request();
        deferrer.Request();
        deferrer.Request();

        await Task.Delay(120);

        Assert.Equal(1, executionCount);
    }

    private static async Task WaitUntilAsync(Func<bool> condition, int timeoutMilliseconds = 2000)
    {
        var deadline = Environment.TickCount64 + timeoutMilliseconds;

        while (!condition())
        {
            if (Environment.TickCount64 >= deadline)
                throw new TimeoutException("Condition was not met before timeout.");

            await Task.Delay(10).ConfigureAwait(false);
        }
    }
}
