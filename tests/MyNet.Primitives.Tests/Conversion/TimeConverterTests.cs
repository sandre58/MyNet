// -----------------------------------------------------------------------
// <copyright file="TimeConverterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Conversion;
using Xunit;

namespace MyNet.Primitives.Tests.Conversion;

public sealed class TimeConverterTests
{
    private readonly TimeConverter _sut = new();

    [Fact]
    public void Convert_MinutesToSeconds_MultipliesBySixty()
        => Assert.Equal(120, _sut.Convert(2, TimeUnit.Minute, TimeUnit.Second));

    [Fact]
    public void Convert_HoursToMinutes_MultipliesBySixty()
        => Assert.Equal(180, _sut.Convert(3, TimeUnit.Hour, TimeUnit.Minute));

    [Fact]
    public void Convert_SecondsToMilliseconds_MultipliesByThousand()
        => Assert.Equal(5000, _sut.Convert(5, TimeUnit.Second, TimeUnit.Millisecond));
}
