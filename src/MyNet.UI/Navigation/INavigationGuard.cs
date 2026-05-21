// -----------------------------------------------------------------------
// <copyright file="INavigationGuard.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Defines a contract for navigation guards that can intercept and evaluate navigation operations before they are executed, allowing for cancellation or modification of the navigation process.
/// </summary>
public interface INavigationGuard
{
    /// <summary>
    /// Determines whether navigation from one context to another is allowed.
    /// </summary>
    /// <param name="from">The current navigation context.</param>
    /// <param name="to">The target navigation context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether navigation is allowed.</returns>
    Task<bool> CanNavigateAsync(NavigationContext? from, NavigationContext to, CancellationToken cancellationToken);
}
