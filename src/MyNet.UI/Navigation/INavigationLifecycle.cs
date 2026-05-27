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
/// Navigation lifecycle hooks for pages.
/// </summary>
public interface INavigationLifecycle
{
    /// <summary>Called on the target page before the navigation is committed.</summary>
    Task OnNavigatingToAsync(NavigationContext context, CancellationToken cancellationToken);

    /// <summary>Called on the target page after the navigation is committed.</summary>
    Task OnNavigatedAsync(NavigationContext context, CancellationToken cancellationToken);

    /// <summary>Called on the source page before leaving.</summary>
    Task OnNavigatingFromAsync(NavigationContext context, CancellationToken cancellationToken);
}
