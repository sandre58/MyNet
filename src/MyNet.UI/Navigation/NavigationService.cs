// -----------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

public sealed class NavigationService(
    INavigationJournal journal,
    INavigationLifecycle lifecycle,
    IEnumerable<INavigationGuard> guards,
    IEnumerable<INavigationMiddleware> middlewares)
    : INavigationService, IDisposable
{
    private readonly IReadOnlyList<INavigationMiddleware> _middlewares = [.. middlewares];
    private readonly SemaphoreSlim _navigationLock = new(1, 1);

    /// <inheritdoc />
    public event EventHandler<NavigationStateChangedEventArgs>? StateChanged;

    /// <inheritdoc />
    public NavigationContext? CurrentContext { get; private set; }

    /// <inheritdoc />
    public bool CanGoBack => journal.BackStack.Any();

    /// <inheritdoc />
    public bool CanGoForward => journal.ForwardStack.Any();

    #region Public API

    /// <inheritdoc />
    public async Task<NavigationResult> NavigateToAsync(INavigationPage page, INavigationParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);

        await _navigationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            return await NavigateInternalAsync(page, NavigationMode.Normal, parameters, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<NavigationResult> GoBackAsync(CancellationToken cancellationToken = default)
    {
        await _navigationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var context = journal.PeekBack();

            return context is null
                ? new(NavigationStatus.NotFound)
                : await NavigateInternalAsync(context.To, NavigationMode.Back, context.Parameters, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<NavigationResult> GoForwardAsync(CancellationToken cancellationToken = default)
    {
        await _navigationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var context = journal.PeekForward();

            return context is null
                ? new(NavigationStatus.NotFound)
                : await NavigateInternalAsync(context.To, NavigationMode.Forward, context.Parameters, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task ResetAsync()
    {
        await _navigationLock.WaitAsync().ConfigureAwait(false);

        try
        {
            CurrentContext = null;
            journal.Clear();
            RaiseStateChanged();
        }
        finally
        {
            _navigationLock.Release();
        }
    }

    #endregion

    #region Core Navigation Pipeline

    private async Task<NavigationResult> NavigateInternalAsync(
        INavigationPage to,
        NavigationMode mode,
        INavigationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(to);

        var previousContext = CurrentContext;
        var context = NavigationContext.Create(previousContext?.To, to, mode, parameters);

        try
        {
            // 1. Guards (decision layer)
            var guardResult = await ExecuteGuardsAsync(previousContext, context, cancellationToken).ConfigureAwait(false);
            if (!guardResult)
            {
                return new(NavigationStatus.Cancelled, "Navigation blocked by guard.");
            }

            // 2. Lifecycle: leaving the current page (source only)
            if (previousContext is not null)
            {
                await lifecycle.OnNavigatingFromAsync(context, cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();

            // 3. Middleware pipeline (includes core execution; no target lifecycle yet)
            var pipeline = BuildMiddlewarePipeline(previousContext, context, core, cancellationToken);

            var result = await pipeline().ConfigureAwait(false);

            if (result.Status != NavigationStatus.Succeeded)
                return result;

            // 4. Lifecycle: entering the target page (after a successful pipeline)
            await lifecycle.OnNavigatingToAsync(context, cancellationToken).ConfigureAwait(false);

            // 5. Commit navigation atomically
            UpdateJournal(mode, previousContext);
            CurrentContext = context;

            // 6. Lifecycle: navigation committed
            await lifecycle.OnNavigatedAsync(context, cancellationToken).ConfigureAwait(false);

            RaiseStateChanged();

            return result;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return new(NavigationStatus.Cancelled);
        }
        catch (Exception exception)
        {
            return new(NavigationStatus.Failed, exception.Message, exception);
        }

        static Task<NavigationResult> core() => Task.FromResult(new NavigationResult(NavigationStatus.Succeeded));
    }

    #endregion

    #region Guards

    private async Task<bool> ExecuteGuardsAsync(NavigationContext? previousContext, NavigationContext context, CancellationToken cancellationToken)
    {
        foreach (var guard in guards)
        {
            var canNavigate = await guard.CanNavigateAsync(previousContext, context, cancellationToken).ConfigureAwait(false);

            if (!canNavigate)
                return false;
        }

        return true;
    }

    #endregion

    #region Middleware Pipeline

    private Func<Task<NavigationResult>> BuildMiddlewarePipeline(
        NavigationContext? from,
        NavigationContext to,
        Func<Task<NavigationResult>> core,
        CancellationToken cancellationToken)
    {
        var pipeline = core;

        foreach (var middleware in _middlewares.Reverse())
        {
            var next = pipeline;

            pipeline = () => middleware.InvokeAsync(
                from,
                to,
                next,
                cancellationToken);
        }

        return pipeline;
    }

    #endregion

    #region Journal

    private void UpdateJournal(NavigationMode mode, NavigationContext? previousContext)
    {
        switch (mode)
        {
            case NavigationMode.Normal:
                if (previousContext is not null)
                    journal.PushBack(previousContext);
                journal.ClearForward();
                break;

            case NavigationMode.Back:
                _ = journal.PopBack();
                if (previousContext is not null)
                    journal.PushForward(previousContext);
                break;

            case NavigationMode.Forward:
                _ = journal.PopForward();
                if (previousContext is not null)
                    journal.PushBack(previousContext);
                break;
        }
    }

    private void RaiseStateChanged()
        => StateChanged?.Invoke(
            this,
            new(CurrentContext, CanGoBack, CanGoForward));

    /// <inheritdoc />
    public void Dispose() => _navigationLock.Dispose();

    #endregion
}
