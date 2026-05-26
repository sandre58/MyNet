// -----------------------------------------------------------------------
// <copyright file="FluentTimeSpanOperatorOverloadsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Xunit;

namespace MyNet.Primitives.Tests.Temporal;

public class FluentTimeSpanOperatorOverloadsTests
{
    [Fact]
    public void LessThan()
    {
        Assert.True(1.Seconds() < 2.Seconds());
        Assert.True(1.Seconds() < TimeSpan.FromSeconds(2));
        Assert.True(TimeSpan.FromSeconds(1) < 2.Seconds());
    }

    [Fact]
    public void LessThanOrEqualTo()
    {
        Assert.True(1.Seconds() <= 2.Seconds());
        Assert.True(1.Seconds() <= TimeSpan.FromSeconds(2));
        Assert.True(TimeSpan.FromSeconds(1) <= 2.Seconds());
    }

    [Fact]
    public void GreaterThan()
    {
        Assert.True(2.Seconds() > 1.Seconds());
        Assert.True(2.Seconds() > TimeSpan.FromSeconds(1));
        Assert.True(TimeSpan.FromSeconds(2) > 1.Seconds());
    }

    [Fact]
    public void GreaterThanOrEqualTo()
    {
        Assert.True(2.Seconds() >= 1.Seconds());
        Assert.True(2.Seconds() >= TimeSpan.FromSeconds(1));
        Assert.True(TimeSpan.FromSeconds(2) >= 1.Seconds());
    }

    [Fact]
    public void AreEquals()
    {
        Assert.True(2.Seconds() == TimeSpan.FromSeconds(2));
        Assert.True(TimeSpan.FromSeconds(2) == 2.Seconds());
    }

    [Fact]
    public void NotEquals()
    {
        Assert.True(2.Seconds() != 1.Seconds());
        Assert.True(2.Seconds() != TimeSpan.FromSeconds(1));
        Assert.True(TimeSpan.FromSeconds(2) != 1.Seconds());
    }

    [Fact]
    public void Add()
    {
        Assert.Equal(1.Seconds() + 2.Seconds(), 3.Seconds());
        Assert.Equal(1.Seconds() + TimeSpan.FromSeconds(2), 3.Seconds());
        Assert.Equal(TimeSpan.FromSeconds(1) + 2.Seconds(), 3.Seconds());
    }

    [Fact]
    public void Subtract()
    {
        Assert.Equal(1.Seconds() - 2.Seconds(), -1.Seconds());
        Assert.Equal(1.Seconds() - TimeSpan.FromSeconds(2), -1.Seconds());
        Assert.Equal(TimeSpan.FromSeconds(1) - 2.Seconds(), -1.Seconds());
    }
}
