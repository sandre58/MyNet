// -----------------------------------------------------------------------
// <copyright file="IDomainFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Fakers.Internet;

/// <summary>
/// Provides methods for generating fake domains.
/// </summary>
public interface IDomainFaker
{
    /// <summary>
    /// Generates a random domain name based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture for which to generate the domain name.</param>
    /// <returns>A random domain name.</returns>
    string Domain(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random host name based on the specified culture.
    /// </summary>
    /// <param name="culture">The culture for which to generate the host name.</param>
    /// <returns>A random host name.</returns>
    string Host(CultureInfo? culture = null);
}
