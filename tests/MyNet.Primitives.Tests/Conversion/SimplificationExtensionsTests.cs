// -----------------------------------------------------------------------
// <copyright file="SimplificationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class SimplificationExtensionsTests
{
    [Fact]
    public void Simplify_ConvertsLargeValueToHigherUnit()
    {
        var (value, unit) = 1500.0.Simplify(LengthUnit.Meter);

        Assert.Equal(1.5, value);
        Assert.Equal(LengthUnit.Kilometer, unit);
    }

    [Fact]
    public void Simplify_OnQuantity_ReturnsSimplifiedQuantity()
    {
        var simplified = 2500.Of(LengthUnit.Meter).Simplify();

        Assert.Equal(2.5, simplified.Value);
        Assert.Equal(LengthUnit.Kilometer, simplified.Unit);
    }

    [Fact]
    public void Simplify_WithUnitRange_RespectsBounds()
    {
        var (value, unit) = 1500.0.Simplify(LengthUnit.Meter, min: LengthUnit.Meter, max: LengthUnit.Meter);

        Assert.Equal(1500, value);
        Assert.Equal(LengthUnit.Meter, unit);
    }
}
