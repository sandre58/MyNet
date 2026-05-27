// -----------------------------------------------------------------------
// <copyright file="NavigationStateChangedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Provides data for <see cref="INavigationService.StateChanged"/>.
/// </summary>
public sealed class NavigationStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationStateChangedEventArgs"/> class.
    /// </summary>
    /// <param name="currentContext">The current navigation context.</param>
    /// <param name="canGoBack">Whether backward navigation is available.</param>
    /// <param name="canGoForward">Whether forward navigation is available.</param>
    public NavigationStateChangedEventArgs(
        NavigationContext? currentContext,
        bool canGoBack,
        bool canGoForward)
    {
        CurrentContext = currentContext;
        CanGoBack = canGoBack;
        CanGoForward = canGoForward;
    }

    /// <summary>
    /// Gets the current navigation context after the state change.
    /// </summary>
    public NavigationContext? CurrentContext { get; }

    /// <summary>
    /// Gets a value indicating whether backward navigation is available.
    /// </summary>
    public bool CanGoBack { get; }

    /// <summary>
    /// Gets a value indicating whether forward navigation is available.
    /// </summary>
    public bool CanGoForward { get; }
}
