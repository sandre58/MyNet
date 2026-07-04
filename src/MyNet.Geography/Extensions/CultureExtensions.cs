// -----------------------------------------------------------------------
// <copyright file="CultureExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Geography;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CultureExtensions
{
    extension(CultureInfo culture)
    {
        /// <summary>
        /// Gets the country associated with the specified culture, if available.
        /// </summary>
        /// <returns>The country associated with the specified culture, or <c>null</c> if not available.</returns>
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
