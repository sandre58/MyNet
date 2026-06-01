// -----------------------------------------------------------------------
// <copyright file="CountryExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Geography.Resources;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CountryExtensions
{
    private static readonly EmbeddedCountryFlagProvider DefaultFlagProvider = new();

    extension(Country country)
    {
        /// <summary>
        /// Retrieves the flag of the specified country in the given size as a MemoryStream. The flag is obtained using the default embedded flag provider.
        /// </summary>
        /// <param name="size">The size of the flag to retrieve.</param>
        /// <returns>A MemoryStream containing the flag image data.</returns>
        public MemoryStream GetFlag(FlagSize size) => country.GetFlag(DefaultFlagProvider, size);
    }
}
