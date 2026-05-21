// -----------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public Task ResetAsync()
    {
        CurrentContext = null;
        journal.Clear();
        return Task.CompletedTask;
    }

    #endregion

    #region Core Navigation Pipeline

    /// <summary>
    /// Executes the core navigation logic, which is responsible for performing the actual navigation between pages. This method is intended to be called within the middleware pipeline after all guards have been executed and lifecycle events have been triggered. The core navigation logic can include tasks such as updating the UI, loading resources, or performing any necessary operations to complete the navigation process.
    /// </summary>
    /// <param name="context">The navigation context containing information about the navigation operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating the navigation outcome.</returns>
    [SuppressMessage("Roslynator", "RCS1163:Unused parameter", Justification = "Core navigation logic is not implemented in this example.")]
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Core navigation logic is not implemented in this example.")]
    private static Task<NavigationResult> ExecuteCoreAsync(NavigationContext context, CancellationToken cancellationToken) => Task.FromResult(new NavigationResult(NavigationStatus.Succeeded));

    /// <summary>
    /// Executes the navigation process, which includes guards, lifecycle events, middleware pipeline, and journal updates. This method orchestrates the entire navigation flow, ensuring that all necessary steps are executed in the correct order to achieve a successful navigation operation. It handles the decision-making process through guards, triggers lifecycle events for both navigating from and navigating to pages, executes the middleware pipeline for any additional processing or modifications, and finally updates the navigation journal based on the navigation mode (normal, back, forward) to maintain an accurate history of navigation operations.
    /// </summary>
    /// <param name="to">The target page to navigate to.</param>
    /// <param name="mode">The navigation mode indicating the type of navigation (normal, back, forward).</param>
    /// <param name="parameters">Optional parameters to pass to the target page.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating the navigation outcome.</returns>
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

            // 2. Lifecycle BEFORE navigation
            if (previousContext is not null)
            {
                await lifecycle.OnNavigatingFromAsync(context, cancellationToken).ConfigureAwait(false);
            }

            await lifecycle.OnNavigatingToAsync(context, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            // 3. Middleware pipeline (around execution)
            var pipeline = BuildMiddlewarePipeline(previousContext, context, core, cancellationToken);

            var result = await pipeline().ConfigureAwait(false);

            if (result.Status != NavigationStatus.Succeeded)
                return result;

            // 4. Commit navigation atomically
            UpdateJournal(mode, previousContext);
            CurrentContext = context;

            // 5. Lifecycle AFTER navigation
            await lifecycle.OnNavigatedAsync(context, cancellationToken).ConfigureAwait(false);

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

        Task<NavigationResult> core() => ExecuteCoreAsync(context, cancellationToken);
    }

    #endregion

    #region Guards

    /// <summary>
    /// Executes all registered navigation guards to determine whether the navigation operation should proceed. Each guard is evaluated in sequence, and if any guard returns false, the navigation process is cancelled. This method ensures that all necessary conditions are met before allowing the navigation to occur, providing a mechanism for enforcing rules or restrictions on navigation operations based on the current and target navigation contexts.
    /// </summary>
    /// <param name="previousContext">The previous navigation context, if any.</param>
    /// <param name="context">The target navigation context containing information about the current navigation request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, with a result indicating whether the navigation is allowed.</returns>
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

    /// <summary>
    /// Builds the middleware pipeline by chaining the registered middleware components together, allowing each middleware to process the navigation operation in sequence. The pipeline is constructed in such a way that each middleware can perform its logic before and/or after invoking the next middleware in the chain, ultimately leading to the execution of the core navigation logic. This design enables a flexible and extensible architecture for handling navigation operations, where additional functionality can be easily added or modified by simply adding or updating middleware components without affecting the core navigation logic or other parts of the system.
    /// </summary>
    /// <param name="from">The current committed navigation context, if any.</param>
    /// <param name="to">The target navigation context currently being processed.</param>
    /// <param name="core">The core navigation logic to be executed at the end of the middleware pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A function representing the complete middleware pipeline, which can be invoked to execute the navigation operation.</returns>
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

    /// <summary>
    /// Updates the navigation journal based on the navigation mode and the previous navigation context. Depending on whether the navigation is a normal navigation, a back navigation, or a forward navigation, the method updates the journal's back and forward stacks accordingly to maintain an accurate history of navigation operations. This ensures that users can navigate back and forth through their navigation history seamlessly, with the journal reflecting the correct state of past and future navigation contexts based on the user's actions.
    /// </summary>
    /// <param name="mode">The navigation mode indicating the type of navigation operation (Normal, Back, or Forward).</param>
    /// <param name="previousContext">The previous navigation context representing the state before the current navigation operation.</param>
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

    /// <inheritdoc />
    public void Dispose() => _navigationLock.Dispose();

    #endregion
}
