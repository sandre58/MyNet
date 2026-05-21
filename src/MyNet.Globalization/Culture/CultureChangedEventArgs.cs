// -----------------------------------------------------------------------
// <copyright file="CultureChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Globalization.Culture;

/// <summary>
/// Event arguments for culture change events, containing the old and new culture information.
/// </summary>
/// <param name="oldCulture">The previous culture.</param>
/// <param name="newCulture">The new culture.</param>
public sealed class CultureChangedEventArgs(CultureInfo oldCulture, CultureInfo newCulture) : EventArgs
{
    /// <summary>
    /// Gets the previous culture.
    /// </summary>
    public CultureInfo OldCulture { get; } = oldCulture;

    /// <summary>
    /// Gets the new culture.
    /// </summary>
    public CultureInfo NewCulture { get; } = newCulture;
}
