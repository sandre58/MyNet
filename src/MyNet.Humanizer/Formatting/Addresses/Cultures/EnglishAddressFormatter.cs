// -----------------------------------------------------------------------
// <copyright file="EnglishAddressFormatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Culture;

namespace MyNet.Humanizer.Formatting.Addresses.Cultures;

/// <summary>
/// Provides address formatting for French culture.
/// </summary>
public class EnglishAddressFormatter() : AddressFormatterBase(SupportedCultures.English)
{
    /// <inheritdoc/>
    protected override string Template { get; } = "{Street}\n{City}\n{PostalCode}\n{Country}";
}
