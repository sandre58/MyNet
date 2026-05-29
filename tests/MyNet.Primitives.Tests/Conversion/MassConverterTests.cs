// -----------------------------------------------------------------------
// <copyright file="MassConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class MassConverterTests
{
    private readonly MassConverter _sut = new();

    [Fact]
    public void Convert_KilogramsToGrams_MultipliesByThousand()
        => Assert.Equal(2500, _sut.Convert(2.5, MassUnit.Kilogram, MassUnit.Gram));

    [Fact]
    public void Convert_GramsToMilligrams_MultipliesByThousand()
        => Assert.Equal(5000, _sut.Convert(5, MassUnit.Gram, MassUnit.Milligram));
}
