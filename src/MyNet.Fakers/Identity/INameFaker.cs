// -----------------------------------------------------------------------
// <copyright file="INameFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Fakers.Identity;

/// <summary>
/// Interface for generating fake names, including first names, last names, prefixes, suffixes, and full names.
/// </summary>
public interface INameFaker
{
    /// <summary>
    /// Generates a random first name based on the specified gender.
    /// </summary>
    /// <param name="gender">The gender for which to generate a first name.</param>
    /// <param name="culture">The culture for which to generate a first name.</param>
    /// <returns>A random first name.</returns>
    string FirstName(GenderType gender, CultureInfo? culture = null);

    /// <summary>
    /// Generates a random last name.
    /// </summary>
    /// <returns>A random last name.</returns>
    string LastName(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random prefix.
    /// </summary>
    /// <returns>A random prefix.</returns>
    string Prefix(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random suffix.
    /// </summary>
    /// <returns>A random suffix.</returns>
    string Suffix(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random full name based on the specified gender and format.
    /// </summary>
    /// <param name="gender">The gender for which to generate a full name.</param>
    /// <param name="format">The format in which to generate the full name.</param>
    /// <param name="culture">The culture for which to generate the full name.</param>
    /// <returns>A random full name.</returns>
    string FullName(GenderType gender, NameFormat format = NameFormat.Standard, CultureInfo? culture = null);
}
