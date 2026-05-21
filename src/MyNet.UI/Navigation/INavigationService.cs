// -----------------------------------------------------------------------
// <copyright file="INavigationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Defines a service for managing navigation between pages in the application, including navigation history and events for navigation operations.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets a value indicating whether the navigation service can navigate back.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets a value indicating whether the navigation service can navigate forward.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Gets the current navigation context.
    /// </summary>
    NavigationContext? CurrentContext { get; }

    /// <summary>
    /// Navigates to the specified page asynchronously.
    /// </summary>
    /// <param name="page">The page to navigate to.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous navigation operation. The task result contains the navigation result.</returns>
    Task<NavigationResult> NavigateToAsync(INavigationPage page, INavigationParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates back to the previous page asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous navigation operation. The task result contains the navigation result.</returns>
    Task<NavigationResult> GoBackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates forward to the next page asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous navigation operation. The task result contains the navigation result.</returns>
    Task<NavigationResult> GoForwardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the navigation history asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous reset operation.</returns>
    Task ResetAsync();
}
