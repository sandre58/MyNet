// -----------------------------------------------------------------------
// <copyright file="ICultureService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Utilities.Localization;

public interface ICultureService
{
    CultureInfo Culture { get; }
}
