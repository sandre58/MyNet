// -----------------------------------------------------------------------
// <copyright file="ICultureFallbackPolicy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Localization.Policies;

/// <summary>
/// Defines a policy for resolving culture fallback chains.
/// Used by both the translation pipeline and localization provider factories
/// to determine which culture to try next when a resource or provider is not found.
/// </summary>
public interface ICultureFallbackPolicy
{
    /// <summary>
    /// Gets the next culture to try as a fallback for the given culture.
    /// Returns <c>null</c> when there is no further fallback.
    /// </summary>
    /// <param name="culture">The culture for which a fallback is requested.</param>
    /// <returns>The fallback culture, or <c>null</c> if no fallback is available.</returns>
    CultureInfo? GetFallback(CultureInfo culture);
}
