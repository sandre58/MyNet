// -----------------------------------------------------------------------
// <copyright file="ExpirationPolicyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Caching.Policies;
using Xunit;

namespace MyNet.Utilities.Tests.Caching;

public sealed class ExpirationPolicyTests
{
    [Fact]
    public void Absolute_WhenExpirationIsInFuture_IsNotExpired()
    {
        var policy = ExpirationPolicy.Absolute(DateTime.UtcNow.AddMinutes(5), force: true);

        Assert.NotNull(policy);
        Assert.False(policy.IsExpired);
    }

    [Fact]
    public void Absolute_WhenExpirationIsInPast_ReturnsNullByDefault()
    {
        var policy = ExpirationPolicy.Absolute(DateTime.UtcNow.AddMinutes(-5));

        Assert.Null(policy);
    }

    [Fact]
    public void Sliding_CanReset_ExtendsExpiration()
    {
        var policy = ExpirationPolicy.Sliding(TimeSpan.FromMinutes(10), force: true);

        Assert.NotNull(policy);
        Assert.True(policy.CanReset);
        Assert.False(policy.IsExpired);

        policy.Reset();

        Assert.False(policy.IsExpired);
    }

    [Fact]
    public void Duration_WhenZeroDuration_ReturnsNull() => Assert.Null(ExpirationPolicy.Duration(TimeSpan.Zero));

    [Fact]
    public void Absolute_Reset_ThrowsWhenPolicyCannotReset()
    {
        var policy = ExpirationPolicy.Absolute(DateTime.UtcNow.AddMinutes(5), force: true)!;

        Assert.Throws<InvalidOperationException>(policy.Reset);
    }
}
