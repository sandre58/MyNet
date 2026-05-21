// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using MyNet.Humanizer.Display;
using MyNet.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Contains extension methods for humanizing Enums.
/// </summary>
public static class EnumExtensions
{
    extension(Enum value)
    {
        /// <summary>
        /// Gets the display name using an explicit provider resolved from DI.
        /// </summary>
        /// <param name="provider">The display name provider.</param>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The display name of the enum value.</returns>
        public string HumanizeWith(IDisplayTextStrategy<Enum> provider, CultureInfo? culture = null)
            => value.HumanizeWith(provider, DisplayTextOptions.Default, culture);

        /// <summary>
        /// Gets the display name using an explicit provider resolved from DI.
        /// </summary>
        /// <param name="provider">The display name provider.</param>
        /// <param name="options">The options to use when retrieving the display name.</param>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The display name of the enum value.</returns>
        public string HumanizeWith(IDisplayTextStrategy<Enum> provider, DisplayTextOptions options, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(provider);
            return provider.GetDisplayText(value, options, culture.OrCurrent());
        }

        /// <summary>
        /// Gets the description of the enum value, if it has a <see cref="DescriptionAttribute"/>. Otherwise, returns null.
        /// </summary>
        /// <returns>The description of the enum value, or null if not defined.</returns>
        public string? GetDescription() => value.GetAttribute<DescriptionAttribute>()?.Description;
    }
}
