// -----------------------------------------------------------------------
// <copyright file="IconManifest.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Tools.PackageIconGenerator;

internal sealed class IconManifest
{
    public List<IconDefinition> Icons { get; init; } = [];
}
