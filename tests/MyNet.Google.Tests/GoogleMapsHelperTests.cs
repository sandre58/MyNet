// -----------------------------------------------------------------------
// <copyright file="GoogleMapsHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Geography;
using MyNet.Google.Maps;
using Xunit;

namespace MyNet.Google.Tests;

public class GoogleMapsHelperTests
{
    [Fact]
    public void GetGoogleMapsUrl_WithNoSettings_ReturnsBaseUrl()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new GoogleMapsSettings());

        Assert.Equal(GoogleMapsHelper.Url, url);
    }

    [Fact]
    public void GetGoogleMapsUrl_WithAddress_EncodesSpacesAsPlus()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new GoogleMapsSettings { Address = "10 rue de Paris" });

        Assert.StartsWith($"{GoogleMapsHelper.Url}?", url, StringComparison.Ordinal);
        Assert.Contains("q=10+rue+de+Paris", url, StringComparison.Ordinal);
    }

    [Fact]
    public void GetGoogleMapsUrl_WithCoordinates_IncludesLatLong()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new GoogleMapsSettings
        {
            Coordinates = new Coordinates(48.8566, 2.3522)
        });

        Assert.Contains("ll=48.8566,2.3522", url, StringComparison.Ordinal);
    }

    [Fact]
    public void GetGoogleMapsUrl_WithHideLeftPanel_AddsEmbedOutput()
    {
        var url = GoogleMapsHelper.GetGoogleMapsUrl(new GoogleMapsSettings
        {
            Address = "Paris",
            HideLeftPanel = true
        });

        Assert.Contains("output=embed", url, StringComparison.Ordinal);
        Assert.Contains("q=Paris", url, StringComparison.Ordinal);
    }
}
