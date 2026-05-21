// -----------------------------------------------------------------------
// <copyright file="NavigationContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Represents the context of a navigation operation, including the source and destination pages, navigation mode, parameters, and timestamp.
/// </summary>
/// <param name="From">The source page of the navigation.</param>
/// <param name="To">The destination page of the navigation.</param>
/// <param name="Mode">The mode of the navigation.</param>
/// <param name="Parameters">The parameters associated with the navigation.</param>
/// <param name="Timestamp">The timestamp when the navigation occurred.</param>
public sealed record NavigationContext(
    INavigationPage? From,
    INavigationPage To,
    NavigationMode Mode,
    INavigationParameters? Parameters,
    DateTimeOffset Timestamp)
{
    /// <summary>
    /// Creates a new instance of the NavigationContext record with the specified parameters and the current timestamp.
    /// </summary>
    /// <param name="from">The source page of the navigation.</param>
    /// <param name="to">The destination page of the navigation.</param>
    /// <param name="mode">The mode of the navigation.</param>
    /// <param name="parameters">The parameters associated with the navigation.</param>
    /// <returns>A new instance of the NavigationContext record with the specified parameters and the current timestamp.</returns>
    public static NavigationContext Create(
        INavigationPage? from,
        INavigationPage to,
        NavigationMode mode,
        INavigationParameters? parameters)
        => new(from, to, mode, parameters, DateTimeOffset.UtcNow);
}
