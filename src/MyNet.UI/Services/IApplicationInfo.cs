// -----------------------------------------------------------------------
// <copyright file="IApplicationInfo.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Services;

/// <summary>
/// Application metadata for shell, about, and startup view models.
/// </summary>
public interface IApplicationInfo
{
    /// <summary>
    /// Gets the product name.
    /// </summary>
    string ProductName { get; }

    /// <summary>
    /// Gets the localized version string.
    /// </summary>
    string? Version { get; }

    /// <summary>
    /// Gets the copyright notice.
    /// </summary>
    string? Copyright { get; }

    /// <summary>
    /// Gets the company name.
    /// </summary>
    string? Company { get; }

    /// <summary>
    /// Gets the product description.
    /// </summary>
    string? Description { get; }
}
