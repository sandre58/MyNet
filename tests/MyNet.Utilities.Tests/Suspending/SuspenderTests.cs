// -----------------------------------------------------------------------
// <copyright file="SuspenderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Suspending;
using Xunit;

namespace MyNet.Utilities.Tests.Suspending;

/// <summary>
/// Tests for Suspender class covering nested scopes, LIFO order validation, and state integrity.
/// </summary>
public class SuspenderTests
{
    [Fact]
    public void Suspend_ShouldSetIsSuspended()
    {
        // Arrange
        var suspender = new Suspender();

        // Act
        Assert.False(suspender.IsSuspended);

        using (suspender.Suspend())
        {
            // Assert
            Assert.True(suspender.IsSuspended);
        }

        // Assert
        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void Resume_ShouldClearIsSuspended()
    {
        // Arrange
        var suspender = new Suspender();

        // Act
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);

            using (suspender.Resume())
            {
                // Assert
                Assert.False(suspender.IsSuspended);
            }
        }

        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void NestedSuspends_ShouldRequireLIFODisposal()
    {
        // Arrange
        var suspender = new Suspender();

        // Act
        var scope1 = suspender.Suspend();
        Assert.True(suspender.IsSuspended);

        var scope2 = suspender.Suspend();
        Assert.True(suspender.IsSuspended);

        // Dispose in LIFO order should work
        scope2.Dispose();
        Assert.True(suspender.IsSuspended); // scope1 still active

        scope1.Dispose();
        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void OutOfOrderDisposal_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var suspender = new Suspender();

        // Act & Assert
        var scope1 = suspender.Suspend();
        var scope2 = suspender.Suspend();

        // Trying to dispose scope1 before scope2 should throw
        var ex = Assert.Throws<InvalidOperationException>(scope1.Dispose);
        Assert.Contains("reverse order", ex.Message, StringComparison.InvariantCultureIgnoreCase);

        // Verify state is still valid
        Assert.True(suspender.IsSuspended);

        // Cleanup
        scope2.Dispose();
        Assert.True(suspender.IsSuspended);
        scope1.Dispose();
        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void SuspendResumePattern_ShouldWorkCorrectly()
    {
        // Arrange
        var suspender = new Suspender();

        // Act & Assert
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);

            using (suspender.Resume())
            {
                Assert.False(suspender.IsSuspended);

                using (suspender.Suspend())
                {
                    Assert.True(suspender.IsSuspended);
                }

                Assert.False(suspender.IsSuspended);
            }

            Assert.True(suspender.IsSuspended);
        }

        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void ThreeNestedSuspends_RequireLIFODisposal()
    {
        // Arrange
        var suspender = new Suspender();

        // Act
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);

            using (suspender.Suspend())
            {
                Assert.True(suspender.IsSuspended);

                using (suspender.Suspend())
                {
                    Assert.True(suspender.IsSuspended);
                }

                Assert.True(suspender.IsSuspended);
            }

            Assert.True(suspender.IsSuspended);
        }

        // Assert
        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void DoubleSuspend_WithoutResume_IsAllowed()
    {
        // Arrange
        var suspender = new Suspender();

        // Act & Assert
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);

            using (suspender.Suspend())
            {
                Assert.True(suspender.IsSuspended);
            }

            Assert.True(suspender.IsSuspended);
        }

        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void DisposeTwice_ShouldBeIdempotent()
    {
        // Arrange
        var suspender = new Suspender();
        var scope = suspender.Suspend();

        // Act
        scope.Dispose();

        // Assert - second dispose is ignored
        scope.Dispose();
        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void MultipleIndependentScopes_ShouldWorkCorrectly()
    {
        // Arrange
        var suspender = new Suspender();

        // Act & Assert - first scope
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);
        }

        Assert.False(suspender.IsSuspended);

        // second scope
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);
        }

        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void ResumeWhenNotSuspended_ShouldClearSuspension()
    {
        // Arrange
        var suspender = new Suspender();

        // Act
        Assert.False(suspender.IsSuspended);

        using (suspender.Resume())
        {
            // Resume when not suspended: suspension state becomes False (already False)
            Assert.False(suspender.IsSuspended);
        }

        Assert.False(suspender.IsSuspended);
    }

    [Fact]
    public void ComplexNestingPattern_ResumeInsideSuspend()
    {
        // Arrange
        var suspender = new Suspender();

        // Act & Assert
        using (suspender.Suspend())
        {
            Assert.True(suspender.IsSuspended);

            using (suspender.Resume())
            {
                Assert.False(suspender.IsSuspended);

                using (suspender.Suspend())
                {
                    Assert.True(suspender.IsSuspended);
                }

                Assert.False(suspender.IsSuspended);
            }

            Assert.True(suspender.IsSuspended);
        }

        Assert.False(suspender.IsSuspended);
    }
}
