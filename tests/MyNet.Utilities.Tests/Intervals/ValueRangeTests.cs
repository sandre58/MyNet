// -----------------------------------------------------------------------
// <copyright file="ValueRangeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.Utilities.Intervals;
using Xunit;

namespace MyNet.Utilities.Tests.Intervals;

public class ValueRangeTests
{
    [Fact]
    public void Constructor_WithMinGreaterThanMax_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => new ValueRange<int>(10, 5));

    [Fact]
    public void Contains_RespectsInclusiveAndExclusiveBounds()
    {
        var range = new ValueRange<int>(1, 5, minInclusive: false, maxInclusive: true);

        Assert.False(range.Contains(1));
        Assert.True(range.Contains(2));
        Assert.True(range.Contains(5));
    }

    [Fact]
    public void Clamp_WithBoundedRange_ReturnsBoundariesWhenOutside()
    {
        var range = new ValueRange<int>(1, 5);

        Assert.Equal(1, range.Clamp(-10));
        Assert.Equal(5, range.Clamp(9));
        Assert.Equal(3, range.Clamp(3));
    }

    [Fact]
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument", Justification = "Testing the parameter name in the exception.")]
    public void EnsureInRange_WhenValueIsOutside_ThrowsArgumentOutOfRangeException()
    {
        var range = new ValueRange<int>(1, 5);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => range.EnsureInRange(10, "Age"));

        Assert.Equal("Age", exception.ParamName);
        Assert.Equal(10, exception.ActualValue);
    }

    [Fact]
    public void ToString_UsesExpectedBracketNotation()
    {
        var range = new ValueRange<int>(1, 5, minInclusive: false, maxInclusive: true);

        Assert.Equal("]1; 5]", range.ToString());
    }
}
