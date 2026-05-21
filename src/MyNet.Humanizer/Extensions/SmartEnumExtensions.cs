// -----------------------------------------------------------------------
// <copyright file="SmartEnumExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Humanizer.Display;
using MyNet.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Contains extension methods for humanizing Enums.
/// </summary>
public static class SmartEnumExtensions
{
    extension(ISmartEnum value)
    {
        /// <summary>
        /// Returns the humanized display name using an explicit provider resolved from DI.
        /// </summary>
        /// <param name="provider">The display name provider.</param>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The humanized display name of the SmartEnum value.</returns>
        public string HumanizeWith(IDisplayTextStrategy<ISmartEnum> provider, CultureInfo? culture = null)
            => value.HumanizeWith(provider, DisplayTextOptions.Default, culture);

        /// <summary>
        /// Returns the humanized display name using an explicit provider resolved from DI.
        /// </summary>
        /// <param name="provider">The display name provider.</param>
        /// <param name="options">The options to use when retrieving the display name.</param>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The humanized display name of the SmartEnum value.</returns>
        public string HumanizeWith(IDisplayTextStrategy<ISmartEnum> provider, DisplayTextOptions options, CultureInfo? culture = null)
        {
            ArgumentNullException.ThrowIfNull(provider);
            return provider.GetDisplayText(value, options, culture.OrCurrent());
        }
    }
}
