// -----------------------------------------------------------------------
// <copyright file="CultureFallbackPolicies.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Localization.Policies;

/// <summary>
/// Provides built-in <see cref="ICultureFallbackPolicy"/> implementations.
/// </summary>
public static class CultureFallbackPolicies
{
    /// <summary>
    /// Gets a policy that never falls back: returns <c>null</c> for any culture.
    /// </summary>
    public static ICultureFallbackPolicy None { get; } = new NonePolicy();

    /// <summary>
    /// Gets a policy that walks up the culture hierarchy:
    /// <c>en-US</c> → <c>en</c> → <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    public static ICultureFallbackPolicy ParentCulture { get; } = new ParentCulturePolicy();

    private sealed class NonePolicy : ICultureFallbackPolicy
    {
        public CultureInfo? GetFallback(CultureInfo culture) => null;
    }

    private sealed class ParentCulturePolicy : ICultureFallbackPolicy
    {
        public CultureInfo? GetFallback(CultureInfo culture) => culture.Equals(CultureInfo.InvariantCulture) ? null : culture.Parent;
    }
}
