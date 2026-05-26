// -----------------------------------------------------------------------
// <copyright file="ConversionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Utilities.Tests.Conversion;

public class ConversionExtensionsTests
{
    [Theory]
    [InlineData(1d, LengthUnit.Kilometer, 1000d, LengthUnit.Meter)]
    [InlineData(250d, LengthUnit.Centimeter, 2.5d, LengthUnit.Meter)]
    [InlineData(3d, LengthUnit.Meter, 3000d, LengthUnit.Millimeter)]
    public void DoubleToLength(double fromValue, LengthUnit from, double expected, LengthUnit to)
        => Assert.Equal(expected, fromValue.To(from, to), 6);

    [Theory]
    [InlineData(2d, TimeUnit.Hour, 120d, TimeUnit.Minute)]
    [InlineData(1d, TimeUnit.Day, 24d, TimeUnit.Hour)]
    [InlineData(120d, TimeUnit.Second, 2d, TimeUnit.Minute)]
    public void DoubleToTime(double fromValue, TimeUnit from, double expected, TimeUnit to)
        => Assert.Equal(expected, fromValue.To(from, to), 6);

    [Theory]
    [InlineData(1d, MassUnit.Kilogram, 1000d, MassUnit.Gram)]
    [InlineData(2500d, MassUnit.Gram, 2.5d, MassUnit.Kilogram)]
    public void DoubleToMass(double fromValue, MassUnit from, double expected, MassUnit to)
        => Assert.Equal(expected, fromValue.To(from, to), 6);

    [Theory]
    [InlineData(0d, TemperatureUnit.Celsius, 32d, TemperatureUnit.Fahrenheit)]
    [InlineData(32d, TemperatureUnit.Fahrenheit, 0d, TemperatureUnit.Celsius)]
    [InlineData(0d, TemperatureUnit.Celsius, 273.15d, TemperatureUnit.Kelvin)]
    [InlineData(300d, TemperatureUnit.Kelvin, 80.33d, TemperatureUnit.Fahrenheit)]
    public void DoubleToTemperature(double fromValue, TemperatureUnit from, double expected, TemperatureUnit to)
        => Assert.Equal(expected, fromValue.To(from, to), 2);

    [Fact]
    public void SimplifyLengthUsesBestReadableUnit()
    {
        var (value, unit) = 1500d.Simplify(LengthUnit.Meter);

        Assert.Equal(1.5d, value, 6);
        Assert.Equal(LengthUnit.Kilometer, unit);
    }

    [Fact]
    public void QuantityCompareHandlesDifferentUnits()
    {
        var left = Quantity.Of(1, LengthUnit.Kilometer);
        var right = Quantity.Of(500, LengthUnit.Meter);

        Assert.True(left > right);
    }

    [Fact]
    public void QuantityAddConvertsBeforeSumming()
    {
        var left = Quantity.Of(1, LengthUnit.Kilometer);
        var right = Quantity.Of(250, LengthUnit.Meter);

        var result = left.Add(right);

        Assert.Equal(1.25d, result.Value, 6);
        Assert.Equal(LengthUnit.Kilometer, result.Unit);
    }

    [Fact]
    public void QuantityEqualValuesHaveSameHashCode()
    {
        var left = Quantity.Of(1, LengthUnit.Kilometer);
        var right = Quantity.Of(1000, LengthUnit.Meter);

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }
}
