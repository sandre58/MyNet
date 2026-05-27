// -----------------------------------------------------------------------
// <copyright file="INavigationClient.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Provides a high-level, developer-friendly API on top of <see cref="INavigationService"/>.
/// </summary>
public interface INavigationClient
{
    /// <inheritdoc cref="INavigationService.StateChanged"/>
    event EventHandler<NavigationStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Gets the current navigation context.
    /// </summary>
    NavigationContext? CurrentContext { get; }

    /// <summary>
    /// Gets a value indicating whether backward navigation is available.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets a value indicating whether forward navigation is available.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Starts building a navigation request toward the specified page type.
    /// </summary>
    /// <typeparam name="TPage">Target page type.</typeparam>
    /// <returns>A request builder.</returns>
    INavigationRequestBuilder To<TPage>()
        where TPage : class, INavigationPage;

    /// <summary>
    /// Navigates directly to the specified page type.
    /// </summary>
    /// <typeparam name="TPage">Target page type.</typeparam>
    /// <param name="parameters">Optional parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The navigation result.</returns>
    Task<NavigationResult> NavigateToAsync<TPage>(object? parameters = null, CancellationToken cancellationToken = default)
        where TPage : class, INavigationPage;

    /// <summary>
    /// Navigates back.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The navigation result.</returns>
    Task<NavigationResult> GoBackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates forward.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The navigation result.</returns>
    Task<NavigationResult> GoForwardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the navigation state.
    /// </summary>
    /// <returns>A task that completes when the state has been reset.</returns>
    Task ResetAsync();
}
