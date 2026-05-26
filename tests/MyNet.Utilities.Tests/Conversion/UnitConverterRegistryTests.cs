// -----------------------------------------------------------------------
// <copyright file="UnitConverterRegistryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Utilities.Tests.Conversion;

public class UnitConverterRegistryTests
{
    [Fact]
    public void GetReturnsRegisteredLengthConverter()
        => Assert.IsType<LengthConverter>(UnitConverterRegistry.Get<LengthUnit>());

    [Fact]
    public void GetReturnsRegisteredTimeConverter()
        => Assert.IsType<TimeConverter>(UnitConverterRegistry.Get<TimeUnit>());

    [Fact]
    public void GetReturnsRegisteredMassConverter()
        => Assert.IsType<MassConverter>(UnitConverterRegistry.Get<MassUnit>());

    [Fact]
    public void GetReturnsRegisteredTemperatureConverter()
        => Assert.IsType<TemperatureConverter>(UnitConverterRegistry.Get<TemperatureUnit>());

    [Fact]
    public void GetThrowsForUnregisteredType()
    {
        var ex = Assert.Throws<InvalidOperationException>(UnitConverterRegistry.Get<UnknownUnit>);

        Assert.Contains(nameof(UnknownUnit), ex.Message, StringComparison.Ordinal);
    }

    private enum UnknownUnit;
}
