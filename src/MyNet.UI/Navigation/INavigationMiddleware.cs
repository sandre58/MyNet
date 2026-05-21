// -----------------------------------------------------------------------
// <copyright file="INavigationMiddleware.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Defines a middleware component that can intercept and process navigation operations in the navigation pipeline.
/// </summary>
public interface INavigationMiddleware
{
    /// <summary>
    /// Invokes the middleware to process a navigation operation from one context to another, allowing for modification of the navigation result or cancellation of the navigation process.
    /// </summary>
    /// <param name="from">The current navigation context.</param>
    /// <param name="to">The target navigation context.</param>
    /// <param name="next">A delegate representing the next middleware in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the navigation result.</returns>
    Task<NavigationResult> InvokeAsync(NavigationContext? from, NavigationContext to, Func<Task<NavigationResult>> next, CancellationToken cancellationToken);
}
