// -----------------------------------------------------------------------
// <copyright file="GoogleMapsHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Google.Maps;
using Xunit;

namespace MyNet.Google.Tests;

public class GoogleMapsHelperTests
{
    [Fact]
    public void GetGoogleMapsUrl_WithNoSettings_ReturnsBaseUrl()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new());

        Assert.Equal(GoogleMapsHelper.Url, url);
    }

    [Fact]
    public void GetGoogleMapsUrl_WithAddress_EncodesSpacesAsPlus()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new() { Address = "10 rue de Paris" });

        Assert.StartsWith($"{GoogleMapsHelper.Url}?", url, StringComparison.Ordinal);
        Assert.Contains("q=10+rue+de+Paris", url, StringComparison.Ordinal);
    }

    [Fact]
    public void GetGoogleMapsUrl_WithCoordinates_IncludesLatLong()
    {
        const double latitude = 48.8566;
        const double longitude = 2.3522;
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new()
        {
            Coordinates = new(latitude, longitude)
        });

        Assert.Contains("ll=", url, StringComparison.Ordinal);
        Assert.Contains(latitude.ToString(System.Globalization.CultureInfo.CurrentCulture), url, StringComparison.Ordinal);
        Assert.Contains(longitude.ToString(System.Globalization.CultureInfo.CurrentCulture), url, StringComparison.Ordinal);
    }

    [Fact]
    public void GetGoogleMapsUrl_WithHideLeftPanel_AddsEmbedOutput()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new()
        {
            Address = "Paris",
            HideLeftPanel = true
        });

        Assert.Contains("output=embed", url, StringComparison.Ordinal);
        Assert.Contains("q=Paris", url, StringComparison.Ordinal);
    }
}
