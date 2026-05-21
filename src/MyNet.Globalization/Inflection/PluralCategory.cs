// -----------------------------------------------------------------------
// <copyright file="PluralCategory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Globalization.Inflection;

/// <summary>
/// Represents the plural category of a quantity, used for inflection rules.
/// </summary>
public enum PluralCategory
{
    /// <summary>
    /// Represents a quantity of zero, which may have a distinct plural form in some languages.
    /// </summary>
    Zero,

    /// <summary>
    /// Represents a singular quantity, typically one.
    /// </summary>
    Singular,

    /// <summary>
    /// Represents a dual quantity, typically two.
    /// </summary>
    Dual,

    /// <summary>
    /// Represents a few quantity, typically three or four.
    /// </summary>
    Few,

    /// <summary>
    /// Represents a many quantity, typically five or more.
    /// </summary>
    Many,

    /// <summary>
    /// Represents any other quantity not covered by the other categories.
    /// </summary>
    Other
}
