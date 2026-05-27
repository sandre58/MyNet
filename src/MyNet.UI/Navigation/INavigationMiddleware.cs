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
/// Intercepts navigation around the core execution step.
/// </summary>
public interface INavigationMiddleware
{
    /// <summary>Processes the navigation and optionally invokes <paramref name="next"/>.</summary>
    Task<NavigationResult> InvokeAsync(
        NavigationContext? from,
        NavigationContext to,
        Func<Task<NavigationResult>> next,
        CancellationToken cancellationToken);
}
