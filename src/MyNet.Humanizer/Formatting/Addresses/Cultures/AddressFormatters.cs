// -----------------------------------------------------------------------
// <copyright file="AddressFormatters.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Formatting.Addresses.Cultures;

/// <summary>
/// Provides access to various address formatters for different cultures. These formatters can be used to format addresses according to the conventions of specific cultures, ensuring that the output is culturally appropriate and correctly structured.
/// </summary>
public static class AddressFormatters
{
    /// <summary>
    /// Gets the default address formatter, which is the invariant address formatter.
    /// </summary>
    public static IAddressFormatter Invariant { get; } = new AddressFormatter();

    /// <summary>
    /// Gets an address formatter that uses the English culture and its specific formatting rules. This formatter will correctly format addresses according to the conventions of the English-speaking world.
    /// </summary>
    public static IAddressFormatter English { get; } = new EnglishAddressFormatter();

    /// <summary>
    /// Gets an address formatter that uses the French culture and its specific formatting rules. This formatter will correctly format addresses according to the conventions of the French-speaking world.
    /// </summary>
    public static IAddressFormatter French { get; } = new FrenchAddressFormatter();
}
