// -----------------------------------------------------------------------
// <copyright file="DeferredOperationsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Utilities.Deferring;
using Xunit;

namespace MyNet.Utilities.Tests.Deferring;

/// <summary>
/// Tests for DeferredOperations class covering multiple deferred operations, nesting, and exception handling.
/// </summary>
public class DeferredOperationsTests
{
    [Fact]
    public void Execute_WithoutDefer_ShouldExecuteImmediately()
    {
        // Arrange
        var executed = false;
        var deferrer = new DeferredOperations();

        // Act
        deferrer.Execute(() => executed = true);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void Execute_WithinDefer_ShouldDeferExecution()
    {
        // Arrange
        var executed = false;
        var deferrer = new DeferredOperations();

        // Act
        using (deferrer.Defer())
        {
            deferrer.Execute(() => executed = true);
            Assert.False(executed, "Should not execute while deferred");
        }

        // Assert
        Assert.True(executed, "Should execute after deferred scope ends");
    }

    [Fact]
    public void MultipleExecutions_WithinDefer_ShouldExecuteAllInOrder()
    {
        // Arrange
        var executionOrder = new List<int>();
        var deferrer = new DeferredOperations();

        // Act
        using (deferrer.Defer())
        {
            deferrer.Execute(() => executionOrder.Add(1));
            deferrer.Execute(() => executionOrder.Add(2));
            deferrer.Execute(() => executionOrder.Add(3));
        }

        // Assert
        Assert.Equal(new[] { 1, 2, 3 }, executionOrder);
    }

    [Fact]
    public void NestedDefers_ShouldExecuteOnlyWhenOutermostScopeEnds()
    {
        // Arrange
        var executionOrder = new List<int>();
        var deferrer = new DeferredOperations();

        // Act
        using (deferrer.Defer())
        {
            deferrer.Execute(() => executionOrder.Add(1));

            using (deferrer.Defer())
            {
                deferrer.Execute(() => executionOrder.Add(2));
                Assert.Empty(executionOrder);
            }

            Assert.Empty(executionOrder);
        }

        // Assert
        Assert.Equal(new[] { 1, 2 }, executionOrder);
    }

    [Fact]
    public void ExceptionInOperation_ShouldPreventFollowingOperations_ButNotCorruptState()
    {
        // Arrange
        var executedBeforeException = false;
        var executedAfterException = false;
        var deferrer = new DeferredOperations();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using (deferrer.Defer())
            {
                deferrer.Execute(() => executedBeforeException = true);
                deferrer.Execute(() => throw new InvalidOperationException("Test error"));
                deferrer.Execute(() => executedAfterException = true);
            }
        });

        Assert.Equal("Test error", ex.Message);
        Assert.True(executedBeforeException, "Operation before exception should execute");
        Assert.False(executedAfterException, "Operation after exception should not execute");

        // Verify that a new deferred scope works correctly after exception
        var newExecuted = false;
        using (deferrer.Defer())
        {
            deferrer.Execute(() => newExecuted = true);
        }

        Assert.True(newExecuted, "New deferred scope should work after previous exception");
    }

    [Fact]
    public void IsDeferred_ShouldReflectCurrentState()
    {
        // Arrange
        var deferrer = new DeferredOperations();

        // Act & Assert
        Assert.False(deferrer.IsDeferred, "Should not be deferred initially");

        using (deferrer.Defer())
        {
            Assert.True(deferrer.IsDeferred, "Should be deferred within scope");
        }

        Assert.False(deferrer.IsDeferred, "Should not be deferred after scope ends");
    }

    [Fact]
    public void DisposeTwice_ShouldBeIdempotent()
    {
        // Arrange
        var executionCount = 0;
        var deferrer = new DeferredOperations();

        // Act
        var scope1 = deferrer.Defer();
        deferrer.Execute(() => executionCount++);
        scope1.Dispose();
        scope1.Dispose(); // Second dispose should be safe

        // Assert
        Assert.Equal(1, executionCount);
    }

    [Fact]
    public void ThreeNestedScopes_ShouldExecuteAllWhenOutermostEnds()
    {
        // Arrange
        var executionCount = 0;
        var deferrer = new DeferredOperations();

        // Act
        using (deferrer.Defer())
        {
            deferrer.Execute(() => executionCount++);

            using (deferrer.Defer())
            {
                deferrer.Execute(() => executionCount++);

                using (deferrer.Defer())
                {
                    deferrer.Execute(() => executionCount++);
                    Assert.Equal(0, executionCount);
                }

                Assert.Equal(0, executionCount);
            }

            Assert.Equal(0, executionCount);
        }

        // Assert
        Assert.Equal(3, executionCount);
    }

    [Fact]
    public void Execute_ManyOperations_ShouldExecuteAllInOrder()
    {
        // Arrange
        const int operationCount = 100;
        var executionOrder = new List<int>();
        var deferrer = new DeferredOperations();

        // Act
        using (deferrer.Defer())
        {
            for (var i = 0; i < operationCount; i++)
            {
                var capturedI = i; // Capture for closure
                deferrer.Execute(() => executionOrder.Add(capturedI));
            }
        }

        // Assert
        Assert.Equal(operationCount, executionOrder.Count);
        for (var i = 0; i < operationCount; i++)
        {
            Assert.Equal(i, executionOrder[i]);
        }
    }

    [Fact]
    public void MixedImmediateAndDeferredExecution_ShouldFollowRules()
    {
        // Arrange
        var executionOrder = new List<string>();
        var deferrer = new DeferredOperations();

        // Act
        executionOrder.Add("before-defer");
        deferrer.Execute(() => executionOrder.Add("immediate"));

        using (deferrer.Defer())
        {
            executionOrder.Add("during-defer");
            deferrer.Execute(() => executionOrder.Add("deferred"));
        }

        executionOrder.Add("after-defer");

        // Assert
        Assert.Equal(
            new[] { "before-defer", "immediate", "during-defer", "deferred", "after-defer" },
            executionOrder);
    }
}
