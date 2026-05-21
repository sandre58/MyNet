// -----------------------------------------------------------------------
// <copyright file="GlobalizationOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Localization.Policies;

namespace MyNet.Globalization;

/// <summary>
/// Configuration options for globalization services including default culture and time zone.
/// </summary>
public sealed class GlobalizationOptions
{
    /// <summary>
    /// Gets the default globalization options instance with default culture and time zone.
    /// </summary>
    public static GlobalizationOptions Default { get; } = new();

    /// <summary>
    /// Gets the default culture used when no culture is explicitly set.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when setting to null.</exception>
    public CultureInfo DefaultCulture
    {
        get;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    }

        = CultureInfo.CurrentCulture;

    /// <summary>
    /// Gets the default time zone used when no time zone is explicitly set.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when setting to null.</exception>
    public TimeZoneInfo DefaultTimeZone
    {
        get;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    }

        = TimeZoneInfo.Local;

    /// <summary>
    /// Gets the culture fallback policy used by translators and provider resolvers.
    /// Defaults to <see cref="CultureFallbackPolicies.ParentCulture"/> (walks up the culture hierarchy).
    /// Use <see cref="CultureFallbackPolicies.None"/> to disable fallback.
    /// </summary>
    public ICultureFallbackPolicy CultureFallbackPolicy { get; init; } = CultureFallbackPolicies.ParentCulture;
}
