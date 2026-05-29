// -----------------------------------------------------------------------
// <copyright file="TemperatureConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class TemperatureConverterTests
{
    private readonly TemperatureConverter _sut = new();

    [Fact]
    public void Convert_CelsiusToFahrenheit_ConvertsCorrectly()
        => Assert.Equal(32, _sut.Convert(0, TemperatureUnit.Celsius, TemperatureUnit.Fahrenheit), precision: 5);

    [Fact]
    public void Convert_FahrenheitToCelsius_ConvertsCorrectly()
        => Assert.Equal(0, _sut.Convert(32, TemperatureUnit.Fahrenheit, TemperatureUnit.Celsius), precision: 5);

    [Fact]
    public void Convert_CelsiusToKelvin_AddsOffset()
        => Assert.Equal(273.15, _sut.Convert(0, TemperatureUnit.Celsius, TemperatureUnit.Kelvin), precision: 5);
}
