// -----------------------------------------------------------------------
// <copyright file="NavigationClient.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Default high-level navigation client.
/// </summary>
public sealed class NavigationClient(INavigationService navigationService, IServiceProvider serviceProvider) : INavigationClient
{
    /// <inheritdoc />
    public event EventHandler<NavigationStateChangedEventArgs>? StateChanged
    {
        add => navigationService.StateChanged += value;
        remove => navigationService.StateChanged -= value;
    }

    /// <inheritdoc />
    public NavigationContext? CurrentContext => navigationService.CurrentContext;

    /// <inheritdoc />
    public bool CanGoBack => navigationService.CanGoBack;

    /// <inheritdoc />
    public bool CanGoForward => navigationService.CanGoForward;

    /// <inheritdoc />
    public INavigationRequestBuilder To<TPage>()
        where TPage : class, INavigationPage
        => new NavigationRequestBuilder(
            () => ActivatorUtilities.GetServiceOrCreateInstance<TPage>(serviceProvider),
            navigationService);

    /// <inheritdoc />
    public Task<NavigationResult> NavigateToAsync<TPage>(object? parameters = null, CancellationToken cancellationToken = default)
        where TPage : class, INavigationPage
        => To<TPage>().With(parameters).GoAsync(cancellationToken);

    /// <inheritdoc />
    public Task<NavigationResult> GoBackAsync(CancellationToken cancellationToken = default)
        => navigationService.GoBackAsync(cancellationToken);

    /// <inheritdoc />
    public Task<NavigationResult> GoForwardAsync(CancellationToken cancellationToken = default)
        => navigationService.GoForwardAsync(cancellationToken);

    /// <inheritdoc />
    public Task ResetAsync() => navigationService.ResetAsync();
}
