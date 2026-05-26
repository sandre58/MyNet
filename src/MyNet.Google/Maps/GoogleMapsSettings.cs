// -----------------------------------------------------------------------
// <copyright file="GoogleMapsSettings.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Geography;

namespace MyNet.Google.Maps;

public class GoogleMapsSettings
{
    public Coordinates? Coordinates { get; init; }

    public string Address { get; init; } = string.Empty;

    public bool HideLeftPanel { get; set; }
}
