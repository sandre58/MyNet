// -----------------------------------------------------------------------
// <copyright file="NavigationServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Navigation;
using MyNet.UI.Navigation.Models;
using Xunit;

namespace MyNet.UI.Tests.Navigation;

public class NavigationServiceTests
{
    [Fact]
    public async Task NavigateToAsync_UpdatesCurrentContext_AndCallsLifecycleAsync()
    {
        var sut = CreateService();
        var page = new TrackingPage();

        var result = await sut.NavigateToAsync(page);

        result.Status.Should().Be(NavigationStatus.Succeeded);
        sut.CurrentContext.Should().NotBeNull();
        sut.CurrentContext!.To.Should().BeSameAs(page);
        page.NavigatingToContext.Should().NotBeNull();
        page.NavigatedContext.Should().NotBeNull();
    }

    [Fact]
    public async Task GoBackAsync_DoesNotConsumeHistory_WhenGuardCancelsAsync()
    {
        var guard = new BlockingGuard(context => context.Mode != NavigationMode.Back);
        var sut = CreateService(guards: [guard]);
        var first = new TrackingPage();
        var second = new TrackingPage();

        await sut.NavigateToAsync(first);
        await sut.NavigateToAsync(second);

        var result = await sut.GoBackAsync();

        result.Status.Should().Be(NavigationStatus.Cancelled);
        sut.CanGoBack.Should().BeTrue();
        sut.CurrentContext!.To.Should().BeSameAs(second);
    }

    [Fact]
    public async Task GoForwardAsync_PreservesRemainingForwardHistoryAsync()
    {
        var sut = CreateService();
        var first = new TrackingPage();
        var second = new TrackingPage();
        var third = new TrackingPage();

        await sut.NavigateToAsync(first);
        await sut.NavigateToAsync(second);
        await sut.NavigateToAsync(third);
        await sut.GoBackAsync();
        await sut.GoBackAsync();

        var forwardResult = await sut.GoForwardAsync();

        forwardResult.Status.Should().Be(NavigationStatus.Succeeded);
        sut.CurrentContext!.To.Should().BeSameAs(second);
        sut.CanGoForward.Should().BeTrue();

        var secondForwardResult = await sut.GoForwardAsync();

        secondForwardResult.Status.Should().Be(NavigationStatus.Succeeded);
        sut.CurrentContext!.To.Should().BeSameAs(third);
    }

    [Fact]
    public async Task NavigateToAsync_PassesCorrectContexts_ToMiddlewareAsync()
    {
        var middleware = new CapturingMiddleware();
        var sut = CreateService(middlewares: [middleware]);
        var first = new TrackingPage();
        var second = new TrackingPage();
        using var cancellationTokenSource = new CancellationTokenSource();

        await sut.NavigateToAsync(first, cancellationToken: cancellationTokenSource.Token);
        await sut.NavigateToAsync(second, cancellationToken: cancellationTokenSource.Token);

        middleware.From.Should().NotBeNull();
        middleware.From!.To.Should().BeSameAs(first);
        middleware.To.Should().NotBeNull();
        middleware.To!.To.Should().BeSameAs(second);
        middleware.CancellationToken.CanBeCanceled.Should().BeTrue();
    }

    private static NavigationService CreateService(
        INavigationGuard[]? guards = null,
        INavigationMiddleware[]? middlewares = null)
        => new(
            new NavigationJournal(),
            new NavigationLifecycle(),
            guards ?? [],
            middlewares ?? []);

    private sealed class TrackingPage : INavigationPage
    {
        public NavigationContext? NavigatingToContext { get; private set; }

        public NavigationContext? NavigatedContext { get; private set; }

        public Task OnNavigatingToAsync(NavigationContext context, CancellationToken cancellationToken)
        {
            NavigatingToContext = context;
            return Task.CompletedTask;
        }

        public Task OnNavigatedAsync(NavigationContext context, CancellationToken cancellationToken)
        {
            NavigatedContext = context;
            return Task.CompletedTask;
        }

        public Task OnNavigatingFromAsync(NavigationContext context, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class BlockingGuard(System.Func<NavigationContext, bool> predicate) : INavigationGuard
    {
        public Task<bool> CanNavigateAsync(NavigationContext? from, NavigationContext to, CancellationToken cancellationToken)
            => Task.FromResult(predicate(to));
    }

    private sealed class CapturingMiddleware : INavigationMiddleware
    {
        public NavigationContext? From { get; private set; }

        public NavigationContext? To { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public async Task<NavigationResult> InvokeAsync(NavigationContext? from, NavigationContext to, System.Func<Task<NavigationResult>> next, CancellationToken cancellationToken)
        {
            From = from;
            To = to;
            CancellationToken = cancellationToken;
            return await next().ConfigureAwait(false);
        }
    }
}

public class NavigationClientTests
{
    [Fact]
    public async Task FluentBuilder_ResolvesPageFromDi_AndMapsAnonymousParametersAsync()
    {
        var services = new ServiceCollection();
        services.AddNavigation();
        services.AddTransient<TrackingNavigationPage>();
        await using var provider = services.BuildServiceProvider();

        var sut = provider.GetRequiredService<INavigationClient>();
        var result = await sut.To<TrackingNavigationPage>().With(new PlayerParameters(42)).GoAsync();

        result.Status.Should().Be(NavigationStatus.Succeeded);
        var currentPage = sut.CurrentContext!.To.Should().BeOfType<TrackingNavigationPage>().Subject;
        currentPage.NavigatingToContext.Should().NotBeNull();
        currentPage.NavigatingToContext!.Parameters.Get<int>(nameof(PlayerParameters.PlayerId)).Should().Be(42);
    }

    [Fact]
    public void TryAddNavigation_RegistersCoreServices()
    {
        var services = new ServiceCollection();
        services.AddNavigation();
        using var provider = services.BuildServiceProvider();

        provider.GetService<INavigationService>().Should().NotBeNull();
        provider.GetService<INavigationJournal>().Should().NotBeNull();
        provider.GetService<INavigationLifecycle>().Should().NotBeNull();
        provider.GetService<INavigationClient>().Should().NotBeNull();
    }

    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local",  Justification = "Test")]
    private sealed record PlayerParameters(int PlayerId);

    private sealed class TrackingNavigationPage : INavigationPage
    {
        public NavigationContext? NavigatingToContext { get; private set; }

        public Task OnNavigatingToAsync(NavigationContext context, CancellationToken cancellationToken)
        {
            NavigatingToContext = context;
            return Task.CompletedTask;
        }

        public Task OnNavigatedAsync(NavigationContext context, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task OnNavigatingFromAsync(NavigationContext context, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
