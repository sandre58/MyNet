// -----------------------------------------------------------------------
// <copyright file="CultureExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using MyNet.Utilities.Geography;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CultureExtensions
{
    extension(CultureInfo culture)
    {
        /// <summary>
        /// Gets the country associated with the specified culture, if available.
        /// </summary>
        /// <returns>The country associated with the specified culture, or <c>null</c> if not available.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="culture"/> is <c>null</c>.</exception>
        public Country? GetCountry()
        {
            ArgumentNullException.ThrowIfNull(culture);

            if (string.IsNullOrEmpty(culture.Name))
                return null;

            try
            {
                var specificCulture = culture.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(culture.Name) : culture;

                var alpha2 = new RegionInfo(specificCulture.Name).TwoLetterISORegionName;

                return Country.All.FirstOrDefault(c => string.Equals(c.Alpha2, alpha2, StringComparison.OrdinalIgnoreCase));
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
