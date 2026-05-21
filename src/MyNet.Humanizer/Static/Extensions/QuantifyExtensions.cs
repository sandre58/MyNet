// -----------------------------------------------------------------------
// <copyright file="QuantifyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Display;
using MyNet.Utilities.Conversion;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer.Static;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extensions for formatting a <see cref="string"/> word as a quantity.
/// </summary>
public static class QuantifyExtensions
{
    extension<TUnit>(Quantity<TUnit> quantity)
        where TUnit : struct, Enum
    {
        /// <summary>
        /// Formats the quantity by humanizing the unit and quantifying the value. The method uses the default display style for humanizing the unit and formats the value using the provided format string. The resulting string combines the humanized unit with the quantified value to create a readable representation of the quantity.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <returns>The formatted quantity string.</returns>
        public string Humanize(string? format = null) => quantity.Humanize(DisplayStyle.Default, format);

        /// <summary>
        /// Formats the quantity by humanizing the unit and quantifying the value. The method first humanizes the unit using the specified display style, and then formats the value using the provided format string. The resulting string combines the humanized unit with the quantified value to create a readable representation of the quantity.
        /// </summary>
        /// <param name="style">The display style to use when humanizing the unit.</param>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <returns>The formatted quantity string.</returns>
        public string Humanize(DisplayStyle style, string? format = null) => quantity.Unit.Humanize(new DisplayTextOptions { Style = style }).Quantify(quantity.Value, format);
    }
}
