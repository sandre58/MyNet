// -----------------------------------------------------------------------
// <copyright file="NavigationStatus.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Defines the status of a navigation operation.
/// </summary>
public enum NavigationStatus
{
    /// <summary>
    /// Indicates that the navigation operation succeeded.
    /// </summary>
    Succeeded,

    /// <summary>
    /// Indicates that the navigation operation was canceled.
    /// </summary>
    Cancelled,

    /// <summary>
    /// Indicates that the navigation operation failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Indicates that the navigation target was not found.
    /// </summary>
    NotFound
}
