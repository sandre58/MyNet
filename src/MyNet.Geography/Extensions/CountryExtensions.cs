// -----------------------------------------------------------------------
// <copyright file="CountryExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Geography;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CountryExtensions
{
    extension(Country country)
    {
        /// <summary>
        /// Retrieves the flag of the specified country in the given size using the provided flag provider. The flag is returned as a MemoryStream containing the image data. If the flag cannot be found, an InvalidOperationException is thrown.
        /// </summary>
        /// <param name="provider">The flag provider to use for retrieving the flag.</param>
        /// <param name="size">The size of the flag to retrieve.</param>
        /// <returns>A MemoryStream containing the flag image data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the flag cannot be found.</exception>
        public MemoryStream GetFlag(ICountryFlagProvider provider, FlagSize size)
        {
            var bytes = provider.GetBytes(country, size);
            return bytes is not { Length: > 0 } ? throw new InvalidOperationException($"Flag not found for country '{country.Name}' and size '{size}'.") : new MemoryStream(bytes);
        }
    }
}
