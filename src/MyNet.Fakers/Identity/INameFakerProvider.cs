// -----------------------------------------------------------------------
// <copyright file="INameFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers.Identity;

/// <summary>
/// Interface for providing name data to the NameFaker. Implementations can source data from various origins, such as embedded resources, external files, or databases.
/// </summary>
public interface INameFakerProvider : ICultureScoped
{
    /// <summary>
    /// Gets an array of common male first names. The NameFaker will use this data to generate realistic male first names when requested. Implementations should provide a diverse set of names to ensure variability in generated data.
    /// </summary>
    IReadOnlyList<string> MaleFirstNames { get; }

    /// <summary>
    /// Gets an array of common female first names. The NameFaker will use this data to generate realistic female first names when requested. Implementations should provide a diverse set of names to ensure variability in generated data.
    /// </summary>
    IReadOnlyList<string> FemaleFirstNames { get; }

    /// <summary>
    /// Gets an array of common last names. The NameFaker will use this data to generate realistic last names when requested. Implementations should provide a diverse set of names to ensure variability in generated data.
    /// </summary>
    IReadOnlyList<string> LastNames { get; }

    /// <summary>
    /// Gets an array of common name prefixes. The NameFaker will use this data to generate realistic name prefixes when requested. Implementations should provide a diverse set of prefixes to ensure variability in generated data.
    /// </summary>
    IReadOnlyList<string> Prefixes { get; }

    /// <summary>
    /// Gets an array of common name suffixes. The NameFaker will use this data to generate realistic name suffixes when requested. Implementations should provide a diverse set of suffixes to ensure variability in generated data.
    /// </summary>
    IReadOnlyList<string> Suffixes { get; }
}
