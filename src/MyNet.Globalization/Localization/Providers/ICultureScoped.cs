// -----------------------------------------------------------------------
// <copyright file="ICultureScoped.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Localization.Providers;

/// <summary>
/// Interface for services that are aware of a specific culture, allowing them to provide localized resources or services based on that culture.
/// </summary>
public interface ICultureScoped
{
    /// <summary>
    /// Gets the culture supported by the provider.
    /// </summary>
    CultureInfo Culture { get; }
}
