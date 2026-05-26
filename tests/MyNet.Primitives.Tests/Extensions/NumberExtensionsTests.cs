// -----------------------------------------------------------------------
// <copyright file="NumberExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Primitives.Tests.Extensions;

public class NumberExtensionsTests
{
    [Theory]
    [InlineData(1024, DataSizeUnit.Byte, 1, DataSizeUnit.Kilobyte)]
    [InlineData(1048576, DataSizeUnit.Byte, 1, DataSizeUnit.Megabyte)]
    [InlineData(1073741824, DataSizeUnit.Byte, 1, DataSizeUnit.Gigabyte)]
    [InlineData(1099511627776, DataSizeUnit.Byte, 1, DataSizeUnit.Terabyte)]
    [InlineData(244587587, DataSizeUnit.Byte, 238855.0654296875, DataSizeUnit.Kilobyte)]
    [InlineData(244587587, DataSizeUnit.Byte, 233.2568998336792, DataSizeUnit.Megabyte)]
    [InlineData(244587587, DataSizeUnit.Byte, 0.22778994124382734, DataSizeUnit.Gigabyte)]
    [InlineData(244587587, DataSizeUnit.Byte, 0.00022245111449592514, DataSizeUnit.Terabyte)]
    [InlineData(21454545, DataSizeUnit.Kilobyte, 21969454080, DataSizeUnit.Byte)]
    [InlineData(21454545, DataSizeUnit.Kilobyte, 20951.7041015625, DataSizeUnit.Megabyte)]
    [InlineData(21454545, DataSizeUnit.Kilobyte, 20.46064853668213, DataSizeUnit.Gigabyte)]
    [InlineData(21454545, DataSizeUnit.Kilobyte, 0.01998110208660364, DataSizeUnit.Terabyte)]
    [InlineData(124124, DataSizeUnit.Megabyte, 130153447424, DataSizeUnit.Byte)]
    [InlineData(124124, DataSizeUnit.Megabyte, 127102976, DataSizeUnit.Kilobyte)]
    [InlineData(124124, DataSizeUnit.Megabyte, 121.21484375, DataSizeUnit.Gigabyte)]
    [InlineData(124124, DataSizeUnit.Megabyte, 0.11837387084960938, DataSizeUnit.Terabyte)]
    [InlineData(14212, DataSizeUnit.Gigabyte, 15260018802688, DataSizeUnit.Byte)]
    [InlineData(14212, DataSizeUnit.Gigabyte, 14902362112, DataSizeUnit.Kilobyte)]
    [InlineData(14212, DataSizeUnit.Gigabyte, 14553088, DataSizeUnit.Megabyte)]
    [InlineData(14212, DataSizeUnit.Gigabyte, 13.87890625, DataSizeUnit.Terabyte)]
    public void DoubleToFileSize(double from, DataSizeUnit fromUnit, double result, DataSizeUnit toUnit) => Assert.Equal(result, from.To(fromUnit, toUnit));

    [Fact]
    public void IntToTens()
    {
        const int number = 1;
        Assert.Equal(10, number.Tens());
    }

    [Fact]
    public void LongToTens()
    {
        const long number = 1;
        Assert.Equal(10L, number.Tens());
    }

    [Fact]
    public void DoubleToTens()
    {
        const double number = 1;
        Assert.Equal(10d, number.Tens());
    }

    [Fact]
    public void IntToHundreds()
    {
        const int number = 2;
        Assert.Equal(200, number.Hundreds());
    }

    [Fact]
    public void LongToHundreds()
    {
        const long number = 2;
        Assert.Equal(200L, number.Hundreds());
    }

    [Fact]
    public void DoubleToHundreds()
    {
        const double number = 2;
        Assert.Equal(200d, number.Hundreds());
    }

    [Fact]
    public void IntToThousands()
    {
        const int number = 3;
        Assert.Equal(3000, number.Thousands());
    }

    [Fact]
    public void LongToThousands()
    {
        const long number = 3;
        Assert.Equal(3000L, number.Thousands());
    }

    [Fact]
    public void DoubleToThousands()
    {
        const double number = 3;
        Assert.Equal(3000d, number.Thousands());
    }

    [Fact]
    public void IntToMillions()
    {
        const int number = 4;
        Assert.Equal(4000000, number.Millions());
    }

    [Fact]
    public void LongToMillions()
    {
        const long number = 4;
        Assert.Equal(4000000L, number.Millions());
    }

    [Fact]
    public void DoubleToMillions()
    {
        const double number = 4;
        Assert.Equal(4000000d, number.Millions());
    }

    [Fact]
    public void IntToBillions()
    {
        const int number = 1;
        Assert.Equal(1000000000, number.Billions());
    }

    [Fact]
    public void LongToBillions()
    {
        const long number = 1;
        Assert.Equal(1000000000L, number.Billions());
    }

    [Fact]
    public void DoubleToBillions()
    {
        const double number = 1;
        Assert.Equal(1000000000d, number.Billions());
    }
}
