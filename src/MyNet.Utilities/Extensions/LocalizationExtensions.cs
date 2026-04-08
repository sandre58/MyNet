// -----------------------------------------------------------------------
// <copyright file="LocalizationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Localization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class LocalizationExtensions
{
    public const string AbbreviationSuffix = "Abbr";

    /// <summary>
    /// Gets the country associated with the specified culture, if available. This method attempts to determine the country by examining the culture's name and region information. If the culture is a neutral culture (e.g., "en"), it will try to create a specific culture (e.g., "en-US") to extract the region information. If the culture does not have an associated country or if any errors occur during this process, the method returns null.
    /// </summary>
    /// <param name="culture">The culture for which to retrieve the associated country.</param>
    /// <returns>The country associated with the specified culture, or null if no country is found.</returns>
    public static Country? GetCountry(this CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if (string.IsNullOrEmpty(culture.Name)) return null;

        try
        {
            var specificCulture = culture.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(culture.Name) : culture;

            var alpha2 = new RegionInfo(specificCulture.Name).TwoLetterISORegionName;
            return EnumClass.GetAll<Country>().FirstOrDefault(c => string.Equals(c.Alpha2, alpha2, StringComparison.OrdinalIgnoreCase));
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    public static string ToAbbreviationKey(this string key) => $"{key}{AbbreviationSuffix}";

    public static string Translate(this string key, CultureInfo? cultureInfo = null) => TranslationService.GetOrCurrent(cultureInfo)[key];

    public static string Translate(this string key, string filename, CultureInfo? cultureInfo = null) => TranslationService.GetOrCurrent(cultureInfo)[key, filename];

    public static string Translate(this CultureInfo culture, string key) => TranslationService.Get(culture).Translate(key);

    public static string Translate(this CultureInfo culture, string key, string filename) => TranslationService.Get(culture).Translate(key, filename);

    public static string TranslateAbbreviated(this string key, CultureInfo? cultureInfo = null) => key.ToAbbreviationKey().Translate(cultureInfo);

    public static string TranslateAbbreviated(this string key, string filename, CultureInfo? cultureInfo = null) => key.ToAbbreviationKey().Translate(filename, cultureInfo);

    public static string TranslateAbbreviated(this CultureInfo culture, string key) => culture.Translate(key.ToAbbreviationKey());

    public static string TranslateAbbreviated(this CultureInfo culture, string key, string filename) => culture.Translate(key.ToAbbreviationKey(), filename);

    public static T? GetProvider<T>(this CultureInfo culture) => LocalizationService.Get<T>(culture);
}
