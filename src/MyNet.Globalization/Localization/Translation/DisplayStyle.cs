// -----------------------------------------------------------------------
// <copyright file="DisplayStyle.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Globalization.Localization.Translation;

/// <summary>
/// Defines the display style to use when humanizing a value. This can be used to specify whether to use the default, short, abbreviation, symbol, narrow, or technical display style when humanizing a value.
/// </summary>
public enum DisplayStyle
{
    /// <summary>
    /// Use the default display style. The default display style is determined by the humanizer and may vary depending on the value being humanized and the culture being used.
    /// </summary>
    Default,

    /// <summary>
    /// Use the short display style. The short display style is typically a more concise version of the default display style.
    /// </summary>
    Short,

    /// <summary>
    /// Use the abbreviation display style. The abbreviation display style is typically a shortened version of the default display style, often using initials or acronyms.
    /// </summary>
    Abbreviation,

    /// <summary>
    /// Use the symbol display style. The symbol display style is typically a graphical representation of the value being humanized.
    /// </summary>
    Symbol,

    /// <summary>
    /// Use the narrow display style. The narrow display style is typically a more compact version of the default display style, often used in constrained spaces.
    /// </summary>
    Narrow
}
