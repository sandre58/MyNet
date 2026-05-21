// -----------------------------------------------------------------------
// <copyright file="IMailFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Fakers.Contacts;

/// <summary>
/// Provides fake email data for applications.
/// </summary>
public interface IMailFaker
{
    /// <summary>
    /// Generates a random email address.
    /// </summary>
    /// <param name="host">The domain for the email address. If null, a random domain will be used.</param>
    /// <param name="culture">The culture for the email address. When null, uses the current culture.</param>
    /// <returns>A random email address.</returns>
    string Email(string? host = null, CultureInfo? culture = null);
}
