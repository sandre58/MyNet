// -----------------------------------------------------------------------
// <copyright file="QuantityExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class QuantityExtensionsTests
{
    [Fact]
    public void Of_CreatesQuantityFromNumericValues()
    {
        Assert.Equal(5, 5.Of(LengthUnit.Meter).Value);
        Assert.Equal(LengthUnit.Meter, 5.Of(LengthUnit.Meter).Unit);
        Assert.Equal(2.5, 2.5d.Of(LengthUnit.Kilometer).Value);
        Assert.Equal(3, 3.0m.Of(LengthUnit.Centimeter).Value);
        Assert.Equal(3, Quantity.Of(3.0, LengthUnit.Centimeter).Value);
    }

    [Fact]
    public void To_ConvertsQuantityToTargetUnit()
    {
        var quantity = 1.Of(LengthUnit.Kilometer);

        var meters = quantity.To(LengthUnit.Meter);

        Assert.Equal(1000, meters.Value);
        Assert.Equal(LengthUnit.Meter, meters.Unit);
    }

    [Fact]
    public void Add_And_Subtract_ConvertUnitsBeforeOperation()
    {
        var left = 1000.Of(LengthUnit.Meter);
        var right = 1.Of(LengthUnit.Kilometer);

        Assert.Equal(2000, left.Add(right).Value);
        Assert.Equal(0, left.Subtract(right).Value);
    }
}
