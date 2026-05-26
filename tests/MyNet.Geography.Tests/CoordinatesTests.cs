// -----------------------------------------------------------------------
// <copyright file="CoordinatesTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Geography.Tests;

public sealed class CoordinatesTests
{
    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var left = new Coordinates(48.8566, 2.3522);
        var right = new Coordinates(48.8566, 2.3522);

        Assert.Equal(left, right);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var left = new Coordinates(48.8566, 2.3522);
        var right = new Coordinates(40.7128, -74.0060);

        Assert.NotEqual(left, right);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-33.8688, 151.2093)]
    public void Constructor_StoresLatitudeAndLongitude(double latitude, double longitude)
    {
        var sut = new Coordinates(latitude, longitude);

        Assert.Equal(latitude, sut.Latitude);
        Assert.Equal(longitude, sut.Longitude);
    }
}
