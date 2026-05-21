// -----------------------------------------------------------------------
// <copyright file="ColorFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Generator;

namespace MyNet.Fakers.Media;

/// <summary>
/// Current implementation of <see cref="IColorFaker"/>.
/// </summary>
public sealed class ColorFaker(IRandomGenerator random) : IColorFaker
{
    private static readonly string[] NamedColors =
    [
        "red",
        "blue",
        "green",
        "yellow",
        "purple",
        "orange",
        "pink",
        "black",
        "white",
        "gray",
        "cyan",
        "magenta",
        "lime",
        "teal",
        "navy",
        "maroon"
    ];

    /// <inheritdoc />
    public string Hex()
    {
        var r = random.Byte();
        var g = random.Byte();
        var b = random.Byte();

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <inheritdoc />
    public (byte R, byte G, byte B) Rgb() => (random.Byte(), random.Byte(), random.Byte());

    /// <inheritdoc />
    public (byte A, byte R, byte G, byte B) Argb() => (random.Byte(), random.Byte(), random.Byte(), random.Byte());

    /// <inheritdoc />
    public string CssRgb()
    {
        var (r, g, b) = Rgb();
        return $"rgb({r}, {g}, {b})";
    }

    /// <inheritdoc />
    public string CssRgba()
    {
        var (a, r, g, b) = Argb();
        return $"rgba({r}, {g}, {b}, {a / 255.0:F2})";
    }

    /// <inheritdoc />
    public string Name() => random.Item(NamedColors);
}
