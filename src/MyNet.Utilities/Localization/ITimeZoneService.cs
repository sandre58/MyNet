// -----------------------------------------------------------------------
// <copyright file="ITimeZoneService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Localization;

public interface ITimeZoneService
{
    TimeZoneInfo TimeZone { get; }
}
