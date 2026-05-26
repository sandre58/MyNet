// -----------------------------------------------------------------------
// <copyright file="PatternFakerBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Generator;
using MyNet.Globalization.Localization.Providers;
using MyNet.Text.Randomize;

namespace MyNet.Fakers;

/// <summary>
/// Base class for pattern-based fakers, providing common functionality to generate randomized values from culture-specific patterns.
/// </summary>
/// <param name="patternGenerator">Text patternGenerator random.</param>
/// <param name="random">Random random.</param>
/// <param name="source">Culture-aware provider source.</param>
/// <typeparam name="TProvider">Type of the culture-aware provider.</typeparam>
public abstract class PatternFakerBase<TProvider>(ITextRandomGenerator patternGenerator, IRandomGenerator random, ICultureScopedServiceSource<TProvider> source)
    where TProvider : class, ICultureScoped
{
    /// <summary>
    /// Generates a randomized value from the specified patterns.
    /// </summary>
    /// <param name="selector">Pattern selector.</param>
    /// <param name="culture">Target culture.</param>
    /// <returns>A randomized string.</returns>
    protected string Randomize(Func<TProvider, IReadOnlyList<string>> selector, CultureInfo? culture = null)
    {
        var provider = source.Get(culture);

        return patternGenerator.RandomizeWithRandomPattern(random, selector(provider));
    }
}
