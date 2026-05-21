// -----------------------------------------------------------------------
// <copyright file="IdentityHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Authentication;

/// <summary>
/// Provides helper methods for parsing identity names in the format "DOMAIN\Username".
/// </summary>
public static class IdentityHelper
{
    /// <summary>
    /// Extracts the domain part from the identity name, assuming the format "DOMAIN\Username".
    /// </summary>
    /// <param name="identity">The identity name, usually in the format "DOMAIN\Username".</param>
    /// <returns>The domain part of the identity name, or an empty string when no domain is present.</returns>
    public static string GetDomain(string? identity)
    {
        var (domain, _) = SplitIdentity(identity);
        return domain;
    }

    /// <summary>
    /// Extracts the username part from the identity name, assuming the format "DOMAIN\Username".
    /// </summary>
    /// <param name="identity">The identity name, usually in the format "DOMAIN\Username".</param>
    /// <returns>The username part of the identity name, or an empty string when unavailable.</returns>
    public static string GetName(string? identity)
    {
        var (_, name) = SplitIdentity(identity);
        return name;
    }

    private static (string Domain, string Name) SplitIdentity(string? identity)
    {
        if (string.IsNullOrWhiteSpace(identity))
            return (string.Empty, string.Empty);

        var separatorIndex = identity.AsSpan().IndexOf('\\');

        return separatorIndex switch
        {
            < 0 => (string.Empty, identity),
            0 => (string.Empty, identity[1..]),
            _ => separatorIndex == identity.Length - 1 ? (identity[..separatorIndex], string.Empty) : (identity[..separatorIndex], identity[(separatorIndex + 1)..])
        };
    }
}
