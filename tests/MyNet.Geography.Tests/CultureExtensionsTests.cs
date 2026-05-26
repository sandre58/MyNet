// -----------------------------------------------------------------------
// <copyright file="CultureExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Xunit;

namespace MyNet.Geography.Tests;

public sealed class CultureExtensionsTests
{
    [Fact]
    public void GetCountry_ForFrenchCulture_ReturnsFrance()
    {
        var culture = CultureInfo.GetCultureInfo("fr-FR");

        var country = culture.GetCountry();

        Assert.Equal(Country.France, country);
    }

    [Fact]
    public void GetCountry_ForInvariantCulture_ReturnsNull()
    {
        var country = CultureInfo.InvariantCulture.GetCountry();

        Assert.Null(country);
    }

    [Fact]
    public void GetCountry_ForNeutralCulture_ResolvesSpecificRegion()
    {
        var culture = CultureInfo.GetCultureInfo("en");

        var country = culture.GetCountry();

        Assert.NotNull(country);
        Assert.Equal("us", country.Alpha2, StringComparer.OrdinalIgnoreCase);
    }
}
