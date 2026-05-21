// -----------------------------------------------------------------------
// <copyright file="INavigationLifecycle.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Defines the lifecycle methods for navigation events, allowing pages to respond to navigation actions such as navigating to, navigating from, and after navigation has occurred, enabling them to perform necessary initialization, cleanup, or state management based on the navigation context and parameters.
/// </summary>
public interface INavigationLifecycle
{
    /// <summary>
    /// Asynchronously called when navigating to the page, allowing the page to perform any necessary initialization or data loading based on the provided navigation context and parameters.
    /// </summary>
    /// <param name="context">The navigation context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnNavigatingToAsync(NavigationContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously called when the navigation to the page is completed, allowing the page to perform any necessary actions after the navigation has occurred, such as updating the UI or starting animations.
    /// </summary>
    /// <param name="context">The navigation context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnNavigatedAsync(NavigationContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously called when navigating away from the page, allowing the page to perform any necessary cleanup or state preservation based on the provided navigation context and parameters.
    /// </summary>
    /// <param name="context">The navigation context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task OnNavigatingFromAsync(NavigationContext context, CancellationToken cancellationToken);
}
