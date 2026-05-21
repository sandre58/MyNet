// -----------------------------------------------------------------------
// <copyright file="ListConjunction.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Humanizer.Formatting.Collections;

/// <summary>
/// Defines the conjunction to use when humanizing a list of items.
/// </summary>
public enum ListConjunction
{
    /// <summary>
    /// No conjunction will be used when humanizing a list of items. For example, "A, B, C".
    /// </summary>
    None,

    /// <summary>
    /// The conjunction "and" will be used when humanizing a list of items. For example, "A, B and C".
    /// </summary>
    And,

    /// <summary>
    /// The conjunction "or" will be used when humanizing a list of items. For example, "A, B or C".
    /// </summary>
    Or,

    /// <summary>
    /// The conjunction "&amp;" (ampersand) will be used when humanizing a list of items. For example, "A, B &amp; C".
    /// </summary>
    Ampersand
}
