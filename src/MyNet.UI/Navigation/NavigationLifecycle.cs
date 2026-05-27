// -----------------------------------------------------------------------
// <copyright file="NavigationLifecycle.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Forwards lifecycle calls to <see cref="INavigationLifecycle"/> pages in the context.
/// </summary>
public sealed class NavigationLifecycle : INavigationLifecycle
{
    /// <inheritdoc />
    public async Task OnNavigatingFromAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        if (context.From is INavigationLifecycle from)
            await from.OnNavigatingFromAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task OnNavigatingToAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        if (context.To is INavigationLifecycle to)
            await to.OnNavigatingToAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task OnNavigatedAsync(NavigationContext context, CancellationToken cancellationToken)
    {
        if (context.To is INavigationLifecycle to)
            await to.OnNavigatedAsync(context, cancellationToken).ConfigureAwait(false);
    }
}
