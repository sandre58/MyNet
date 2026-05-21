// -----------------------------------------------------------------------
// <copyright file="SupportedCultures.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Culture;

/// <summary>
/// Provides a set of predefined supported cultures for use in globalization and localization scenarios.
/// </summary>
public static class SupportedCultures
{
    /// <summary>
    /// Represents the French culture, which is commonly used in many countries around the world, including France, Canada, Belgium, Switzerland, and several African nations. The culture code for French is "fr". This culture is known for its rich history, art, cuisine, and contributions to literature and philosophy.
    /// </summary>
    public static readonly CultureInfo French = CultureInfo.GetCultureInfo("fr");

    /// <summary>
    /// Represents the English culture, which is widely used in many countries, including the United States, the United Kingdom, Canada, Australia, and several other nations. The culture code for English is "en". This culture is known for its global influence in business, science, technology, and popular culture.
    /// </summary>
    public static readonly CultureInfo English = CultureInfo.GetCultureInfo("en");
}
