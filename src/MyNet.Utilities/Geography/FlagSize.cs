// -----------------------------------------------------------------------
// <copyright file="FlagSize.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Geography;

/// <summary>
/// Defines the size of a country flag in pixels.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Use as size in pixel.")]
public enum FlagSize
{
    Pixel16 = 16,

    Pixel24 = 24,

    Pixel32 = 32,

    Pixel48 = 48,

    Pixel64 = 64,

    Pixel128 = 128
}
