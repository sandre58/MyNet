// -----------------------------------------------------------------------
// <copyright file="CultureInfoNameComparer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace MyNet.Utilities.Comparers;

/// <summary>
/// Compares <see cref="CultureInfo"/> instances using their culture name.
/// </summary>
public sealed class CultureInfoNameComparer : IEqualityComparer<CultureInfo>
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static CultureInfoNameComparer Instance { get; } = new();

    /// <inheritdoc />
    public bool Equals(CultureInfo? x, CultureInfo? y) => StringComparer.OrdinalIgnoreCase.Equals(x?.Name, y?.Name);

    /// <inheritdoc />
    public int GetHashCode(CultureInfo obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
    }
}
