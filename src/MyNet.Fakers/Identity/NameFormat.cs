// -----------------------------------------------------------------------
// <copyright file="NameFormat.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Fakers.Identity;

/// <summary>
/// Defines the various formats for generating names in the FakeDataGenerator library, including standard, inverse, and formats with prefixes or suffixes.
/// </summary>
public enum NameFormat
{
    /// <summary>
    /// The standard format for generating names, which typically includes a first name followed by a last name (e.g., "John Doe").
    /// </summary>
    Standard,

    /// <summary>
    /// An inverse format for generating names, which typically includes a last name followed by a first name (e.g., "Doe John").
    /// </summary>
    Inverse,

    /// <summary>
    /// A format for generating names with a prefix (e.g., "Dr. John Doe").
    /// </summary>
    WithPrefix,

    /// <summary>
    /// An inverse format for generating names with a prefix (e.g., "Doe John, Dr.").
    /// </summary>
    InverseWithPrefix,

    /// <summary>
    /// A format for generating names with a suffix (e.g., "John Doe, Jr.").
    /// </summary>
    WithSuffix,

    /// <summary>
    /// An inverse format for generating names with a suffix (e.g., "Doe John, Jr.").
    /// </summary>
    InverseWithSuffix
}
