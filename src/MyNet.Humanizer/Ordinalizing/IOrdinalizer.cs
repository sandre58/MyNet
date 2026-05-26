// -----------------------------------------------------------------------
// <copyright file="IOrdinalizer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Localization.Providers;

namespace MyNet.Humanizer.Ordinalizing;

/// <summary>
/// Defines a contract for ordinalizing numbers, which is the process of converting a cardinal number (e.g., 1, 2, 3) into its corresponding ordinal form (e.g., 1st, 2nd, 3rd). The IOrdinalizer interface provides a method for performing this transformation, allowing for optional customization through the use of OrdinalizationOptions. Implementations of this interface can handle different languages and cultures, taking into account specific rules for ordinalization based on grammatical gender, pluralization, and other linguistic factors that may influence the correct ordinal form of a number in a given context.
/// </summary>
public interface IOrdinalizer : ICultureScoped
{
    /// <summary>
    /// Returns the ordinalized form of the specified number, which is the representation of the number in its ordinal form (e.g., "1st" for 1, "2nd" for 2, "3rd" for 3). The method takes an optional parameter of type OrdinalizationOptions, which allows for customization of the ordinalization process based on specific linguistic rules or preferences. This can include considerations such as grammatical gender, which may affect the suffix or form of the ordinal in certain languages. The method should return a string that represents the ordinalized version of the input number according to the rules of the language and culture being handled by the implementation of the IOrdinalizer interface.
    /// </summary>
    /// <param name="number">The number to ordinalize.</param>
    /// <param name="options">Optional parameters for customizing the ordinalization process.</param>
    /// <returns>The ordinalized form of the number.</returns>
    string Ordinalize(long number, OrdinalizationOptions? options = null);
}
