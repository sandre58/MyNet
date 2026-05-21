// -----------------------------------------------------------------------
// <copyright file="Ordinalizers.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Ordinalizing.Cultures;

/// <summary>
/// Provides access to various ordinalizers for different cultures. This class serves as a centralized location to retrieve ordinalizer instances that are tailored to specific cultural rules for ordinalization. Each property returns an instance of an ordinalizer that can be used to convert numbers into their corresponding ordinal forms according to the conventions of the specified culture. For example, the English ordinalizer will convert "1" to "1st", while the French ordinalizer will convert "1" to "1er". This design allows for easy access and use of culturally appropriate ordinalizers throughout the application.
/// </summary>
public static class Ordinalizers
{
    /// <summary>
    /// Gets the default ordinalizer, which is the invariant ordinalizer.
    /// </summary>
    public static IOrdinalizer Invariant { get; } = new InvariantOrdinalizer();

    /// <summary>
    /// Gets an ordinalizer that uses the English culture and its specific ordinalization rules. This ordinalizer will correctly transform numbers into their English ordinal forms, such as "1st", "2nd", "3rd", and so on, following the standard English conventions for ordinal numbers.
    /// </summary>
    public static IOrdinalizer English { get; } = new EnglishOrdinalizer();

    /// <summary>
    /// Gets an ordinalizer that uses the French culture and its specific ordinalization rules. This ordinalizer will correctly transform numbers into their French ordinal forms, such as "1er" for 1, "2e" for 2, "3e" for 3, and so on, following the standard French conventions for ordinal numbers. In French, the first ordinal is unique ("1er"), while subsequent ordinals typically use the "e" suffix, making this ordinalizer essential for accurate representation of ordinals in the French language.
    /// </summary>
    public static IOrdinalizer French { get; } = new FrenchOrdinalizer();
}
