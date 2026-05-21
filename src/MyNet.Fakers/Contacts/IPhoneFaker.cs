// -----------------------------------------------------------------------
// <copyright file="IPhoneFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Fakers.Contacts;

/// <summary>
/// Interface for generating fake phone numbers.
/// </summary>
public interface IPhoneFaker
{
    /// <summary>
    /// Generates a random phone number.
    /// </summary>
    /// <param name="culture">The culture used to generate the phone number.</param>
    /// <returns>A fake phone number.</returns>
    string Number(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random mobile phone number.
    /// </summary>
    /// <param name="culture">The culture used to generate the phone number.</param>
    /// <returns>A fake mobile phone number.</returns>
    string MobileNumber(CultureInfo? culture = null);

    /// <summary>
    /// Generates a random international phone number.
    /// </summary>
    /// <param name="culture">The culture used to generate the phone number.</param>
    /// <returns>A fake international phone number.</returns>
    string InternationalNumber(CultureInfo? culture = null);
}
