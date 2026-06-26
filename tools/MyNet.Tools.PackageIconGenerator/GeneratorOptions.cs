// -----------------------------------------------------------------------
// <copyright file="GeneratorOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Tools.PackageIconGenerator;

internal sealed class GeneratorOptions
{
    public bool ShowHelp { get; set; }

    public string? ManifestPath { get; set; }

    public string? SvgDirectory { get; set; }

    public string? OutputDirectory { get; set; }
}
