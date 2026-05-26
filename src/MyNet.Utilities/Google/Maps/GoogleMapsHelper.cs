// -----------------------------------------------------------------------
// <copyright file="GoogleMapsHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Primitives.Helpers;

namespace MyNet.Utilities.Google.Maps;

public static class GoogleMapsHelper
{
    public const string Url = "https://maps.google.com/maps";

    public static void OpenGoogleMaps(GoogleMapsSettings settings) => ProcessHelper.Start(GetGoogleMapsUrl(settings));

    public static string GetGoogleMapsUrl(GoogleMapsSettings settings)
    {
        var url = Url;
        var parameters = settings.ToDictionary();
        if (parameters.Count != 0)
        {
            url += $"?{string.Join("&", settings.ToDictionary().Select(x => $"{x.Key}={x.Value}"))}";
        }

        return url;
    }

    private static Dictionary<string, string> ToDictionary(this GoogleMapsSettings settings)
    {
        var result = new Dictionary<string, string>();

        if (settings.HideLeftPanel)
        {
            result.Add("output", "embed");
        }

        if (!string.IsNullOrEmpty(settings.Address))
        {
            result.Add("q", settings.Address.Replace(" ", "+", StringComparison.OrdinalIgnoreCase));
        }

        if (settings.Coordinates is not null)
        {
            result.Add("ll", $"{settings.Coordinates.Latitude},{settings.Coordinates.Longitude}");
        }

        return result;
    }
}
