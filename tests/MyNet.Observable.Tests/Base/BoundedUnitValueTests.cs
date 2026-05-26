// -----------------------------------------------------------------------
// <copyright file="BoundedUnitValueTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class BoundedUnitValueTests
{
    [Fact]
    public void Normalize_1024Bytes_BecomesKilobyte()
    {
        var size = new FileSizeValue { Value = 1024 };

        var normalized = size.Normalize();

        Assert.Equal(1, normalized.Value);
        Assert.Equal(DataSizeUnit.Kilobyte, normalized.Unit);
    }

    [Fact]
    public void Convert_MetersToKilometers_ReturnsScaledValue()
    {
        var length = new LengthValue { Value = 1500 };

        var kilometers = length.Convert(LengthUnit.Kilometer);

        Assert.Equal(1.5, kilometers);
    }
}
