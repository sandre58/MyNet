// -----------------------------------------------------------------------
// <copyright file="Directions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Google.Maps;

public class Directions
{
    public enum Status
    {
        Ok,
        Failed
    }

    public IList<Step> Steps { get; } = [];

    public string? Duration { get; set; }

    public string? Distance { get; set; }

    public Status StatusCode { get; set; }
}
