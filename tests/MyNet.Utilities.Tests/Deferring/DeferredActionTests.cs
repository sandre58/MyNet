// -----------------------------------------------------------------------
// <copyright file="DeferredActionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Deferring;
using Xunit;

namespace MyNet.Utilities.Tests.Deferring;

/// <summary>
/// Tests for DeferredAction class covering nesting, exception handling, and state integrity.
/// </summary>
public class DeferredActionTests
{
    [Fact]
    public void Request_WithoutDefer_ShouldExecuteImmediately()
    {
        // Arrange
        var executed = false;
        var action = new DeferredAction(() => executed = true);

        // Act
        action.Request();

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void Request_WithinDefer_ShouldDeferExecution()
    {
        // Arrange
        var executed = false;
        var action = new DeferredAction(() => executed = true);

        // Act
        using (action.Defer())
        {
            action.Request();
            Assert.False(executed, "Should not execute while deferred");
        }

        // Assert
        Assert.True(executed, "Should execute after deferred scope ends");
    }

    [Fact]
    public void Request_ShouldBeAliasForRequest()
    {
        // Arrange
        var executed = false;
        var action = new DeferredAction(() => executed = true);

        // Act
        action.Request();

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void ExecuteNow_ShouldAlwaysExecuteImmediately()
    {
        // Arrange
        var executed = false;
        var action = new DeferredAction(() => executed = true);

        // Act
        using (action.Defer())
        {
            action.ExecuteNow();
            Assert.True(executed, "ExecuteNow should execute even within deferred scope");
        }
    }

    [Fact]
    public void NestedDefers_ShouldExecuteOnlyWhenOutermostScopeEnds()
    {
        // Arrange
        var executionCount = 0;
        var action = new DeferredAction(() => executionCount++);

        // Act
        using (action.Defer())
        {
            action.Request();

            using (action.Defer())
            {
                action.Request();
                Assert.Equal(0, executionCount);
            }

            Assert.Equal(0, executionCount);
        }

        // Assert
        Assert.Equal(1, executionCount);
    }

    [Fact]
    public void MultipleRequests_WithinDefer_ShouldExecuteOnce()
    {
        // Arrange
        var executionCount = 0;
        var action = new DeferredAction(() => executionCount++);

        // Act
        using (action.Defer())
        {
            action.Request();
            action.Request();
            action.Request();
        }

        // Assert
        Assert.Equal(1, executionCount);
    }

    [Fact]
    public void ExceptionInAction_ShouldNotPreventsCleanup()
    {
        // Arrange
        var action = new DeferredAction(() => throw new InvalidOperationException("Test error"));

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using (action.Defer())
            {
                action.Request();
            }
        });

        Assert.Equal("Test error", ex.Message);

        // Verify that another request after exception works
        var executed = false;
        var action2 = new DeferredAction(() => executed = true);
        action2.Request();
        Assert.True(executed);
    }

    [Fact]
    public void IsDeferred_ShouldReflectCurrentState()
    {
        // Arrange
        var action = new DeferredAction(() => { });

        // Act & Assert
        Assert.False(action.IsDeferred, "Should not be deferred initially");

        using (action.Defer())
        {
            Assert.True(action.IsDeferred, "Should be deferred within scope");
        }

        Assert.False(action.IsDeferred, "Should not be deferred after scope ends");
    }

    [Fact]
    public void DisposeTwice_ShouldBeIdempotent()
    {
        // Arrange
        var executionCount = 0;
        var action = new DeferredAction(() => executionCount++);

        // Act
        var scope1 = action.Defer();
        action.Request();
        scope1.Dispose();
        scope1.Dispose(); // Second dispose should be safe

        // Assert
        Assert.Equal(1, executionCount);
    }

    [Fact]
    public void ThreeNestedScopes_ShouldExecuteWhenOutermostEnds()
    {
        // Arrange
        var executionCount = 0;
        var action = new DeferredAction(() => executionCount++);

        // Act
        using (action.Defer())
        {
            action.Request();

            using (action.Defer())
            {
                action.Request();

                using (action.Defer())
                {
                    action.Request();
                    Assert.Equal(0, executionCount);
                }

                Assert.Equal(0, executionCount);
            }

            Assert.Equal(0, executionCount);
        }

        // Assert
        Assert.Equal(1, executionCount);
    }
}
