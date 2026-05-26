// -----------------------------------------------------------------------
// <copyright file="DataSizeConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class DataSizeConverterTests
{
    private readonly DataSizeConverter _sut = new();

    [Fact]
    public void Convert_SameUnit_ReturnsOriginalValue() => Assert.Equal(512, _sut.Convert(512, DataSizeUnit.Kilobyte, DataSizeUnit.Kilobyte));

    [Fact]
    public void Convert_BytesToKilobytes_DividesBy1024() => Assert.Equal(1, _sut.Convert(1024, DataSizeUnit.Byte, DataSizeUnit.Kilobyte));

    [Fact]
    public void Convert_MegabytesToKilobytes_MultipliesBy1024() => Assert.Equal(2048, _sut.Convert(2, DataSizeUnit.Megabyte, DataSizeUnit.Kilobyte));

    [Fact]
    public void Convert_GigabytesToBytes_UsesPowerOf1024() => Assert.Equal(1024 * 1024 * 1024, _sut.Convert(1, DataSizeUnit.Gigabyte, DataSizeUnit.Byte));
}
