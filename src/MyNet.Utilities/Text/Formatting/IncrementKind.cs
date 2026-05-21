// -----------------------------------------------------------------------
// <copyright file="IncrementKind.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Formatting;

/// <summary>
/// Defines the available increment suffix strategies.
/// </summary>
public enum IncrementKind
{
    /// <summary>
    /// Appends numeric values (1, 2, 3, ...).
    /// </summary>
    Numeric,

    /// <summary>
    /// Appends alphabetic values (A, B, ..., Z, AA, AB, ...).
    /// </summary>
    Alpha
}
