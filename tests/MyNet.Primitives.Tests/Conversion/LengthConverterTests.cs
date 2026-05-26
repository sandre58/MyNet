// -----------------------------------------------------------------------
// <copyright file="LengthConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class LengthConverterTests
{
    private readonly LengthConverter _sut = new();

    [Fact]
    public void Convert_SameUnit_ReturnsOriginalValue() => Assert.Equal(42, _sut.Convert(42, LengthUnit.Meter, LengthUnit.Meter));

    [Fact]
    public void Convert_KilometersToMeters_MultipliesBy1000() => Assert.Equal(1500, _sut.Convert(1.5, LengthUnit.Kilometer, LengthUnit.Meter));

    [Fact]
    public void Convert_MetersToKilometers_DividesBy1000() => Assert.Equal(2, _sut.Convert(2000, LengthUnit.Meter, LengthUnit.Kilometer));

    [Fact]
    public void Convert_MillimetersToCentimeters_DividesBy10() => Assert.Equal(25, _sut.Convert(250, LengthUnit.Millimeter, LengthUnit.Centimeter));
}
