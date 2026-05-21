// -----------------------------------------------------------------------
// <copyright file="FrenchAddressFormatter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Culture;

namespace MyNet.Humanizer.Formatting.Addresses.Cultures;

/// <summary>
/// Provides address formatting for French culture.
/// </summary>
public class FrenchAddressFormatter() : AddressFormatterBase(SupportedCultures.French)
{
    /// <inheritdoc/>
    protected override string Template { get; } = "{Street}\n{PostalCode} {City}\n{Country}";
}
