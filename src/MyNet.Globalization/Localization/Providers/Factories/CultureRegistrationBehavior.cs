// -----------------------------------------------------------------------
// <copyright file="CultureRegistrationBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Globalization.Localization.Providers.Factories;

/// <summary>
/// Defines how a culture-specific provider registration behaves when a registration already exists for the same culture.
/// </summary>
public enum CultureRegistrationBehavior
{
    /// <summary>
    /// Replaces the existing registration for the culture.
    /// </summary>
    Replace = 0,

    /// <summary>
    /// Keeps the existing registration and ignores the new one.
    /// </summary>
    SkipIfExists = 1,

    /// <summary>
    /// Throws an exception when a registration already exists for the culture.
    /// </summary>
    ThrowIfExists = 2
}
