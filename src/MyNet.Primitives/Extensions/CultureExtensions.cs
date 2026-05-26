// -----------------------------------------------------------------------
// <copyright file="CultureExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class CultureExtensions
{
    extension(CultureInfo? culture)
    {
        /// <summary>
        /// Determines whether the provided culture is the current culture. If <paramref name="culture"/> is null, it is considered to be the current culture, and the method returns true. Otherwise, it compares <paramref name="culture"/> with the current culture and returns true if they are equal, or false if they are different. This method provides a convenient way to check if a specific culture matches the current culture, while also treating a null value as a match to the current culture.
        /// </summary>
        public bool IsCurrent() => culture?.Equals(CultureInfo.CurrentCulture) != false;

        /// <summary>
        /// Returns the current culture if <paramref name="culture"/> is null, otherwise returns <paramref name="culture"/>. This is a convenient method to ensure that a non-null culture is always returned, defaulting to the current culture when no specific culture is provided.
        /// </summary>
        /// <returns>The current culture if <paramref name="culture"/> is null, otherwise the provided culture.</returns>
        public CultureInfo OrCurrent() => culture.Or(CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns the invariant culture if <paramref name="culture"/> is null, otherwise returns <paramref name="culture"/>. This is useful for scenarios where a culture-agnostic behavior is desired when no specific culture is provided.
        /// </summary>
        /// <returns>The invariant culture if <paramref name="culture"/> is null, otherwise the provided culture.</returns>
        public CultureInfo OrInvariant() => culture.Or(CultureInfo.InvariantCulture);
    }
}
