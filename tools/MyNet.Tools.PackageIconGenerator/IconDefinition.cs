// -----------------------------------------------------------------------
// <copyright file="IconDefinition.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Tools.PackageIconGenerator;

internal sealed class IconDefinition
{
    public required string File { get; init; }

    public required string Label { get; init; }

    /// <summary>Gets sVG file under icon-svgs/ (omit when <see cref="Glyph"/> is set).</summary>
    public string? Svg { get; init; }

    /// <summary>Gets bold text drawn as the pictogram (e.g. HTTP) when SVG is not clear enough.</summary>
    public string? Glyph { get; init; }

    public required string Base { get; init; }

    public required string Accent { get; init; }

    /// <summary>Gets optional scale multiplier for the pictogram (default 1).</summary>
    public float? IconScale { get; init; }
}
