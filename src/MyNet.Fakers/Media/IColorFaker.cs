// -----------------------------------------------------------------------
// <copyright file="IColorFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Fakers.Media;

/// <summary>
/// Interface for generating fake color values in various formats, such as hexadecimal and RGB.
/// </summary>
public interface IColorFaker
{
    /// <summary>
    /// Generates a random color in hexadecimal format (e.g. #A1B2C3).
    /// </summary>
    string Hex();

    /// <summary>
    /// Generates a random RGB color tuple.
    /// </summary>
    (byte R, byte G, byte B) Rgb();

    /// <summary>
    /// Generates a random ARGB color tuple.
    /// </summary>
    (byte A, byte R, byte G, byte B) Argb();

    /// <summary>
    /// Generates a random CSS rgb() string.
    /// </summary>
    string CssRgb();

    /// <summary>
    /// Generates a random CSS rgba() string.
    /// </summary>
    string CssRgba();

    /// <summary>
    /// Generates a random known color name (e.g. "red", "blue").
    /// </summary>
    string Name();
}
