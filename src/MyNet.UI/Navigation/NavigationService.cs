// -----------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.UI.Navigation.Models;
using MyNet.Utilities.Suspending;

namespace MyNet.UI.Navigation;

/// <summary>
/// Provides a base implementation of <see cref="INavigationService"/> for managing navigation between pages and history.
/// Handles navigation events, history stacks, and context updates.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Stack<NavigationContext> _backStack = new();
    private readonly Stack<NavigationContext> _forwardStack = new();

    /// <inheritdoc/>
    public event EventHandler<NavigatingEventArgs>? Navigating;

    /// <inheritdoc/>
    public event EventHandler<NavigationEventArgs>? Navigated;

    /// <inheritdoc/>
    public event EventHandler? HistoryCleared;

    /// <inheritdoc/>
    public event EventHandler? Cleared;

    /// <inheritdoc/>
    public NavigationContext? CurrentContext { get; private set; }

    /// <inheritdoc/>
    public Suspender JournalSuspender { get; } = new();

    /// <summary>
    /// Raises the <see cref="Navigating"/> event and updates cancellation state.
    /// </summary>
    /// <param name="navigatingContext">The navigating context.</param>
    private void RaiseNavigating(NavigatingContext navigatingContext)
    {
        var args = new NavigatingEventArgs(navigatingContext.OldPage, navigatingContext.Page, navigatingContext.Mode, navigatingContext.Parameters);
        Navigating?.Invoke(this, args);
        navigatingContext.Cancel = args.Cancel;
    }

    /// <summary>
    /// Raises the <see cref="Navigated"/> event.
    /// </summary>
    /// <param name="navigationContext">The navigation context.</param>
    private void RaiseNavigated(NavigationContext navigationContext)
        => Navigated?.Invoke(this, new NavigationEventArgs(navigationContext.OldPage, navigationContext.Page, navigationContext.Mode, navigationContext.Parameters));

    /// <summary>
    /// Raises the <see cref="HistoryCleared"/> event.
    /// </summary>
    private void RaiseHistoryCleared() => HistoryCleared?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Raises the <see cref="Cleared"/> event.
    /// </summary>
    private void RaiseCleared() => Cleared?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc/>
    public virtual IEnumerable<NavigationContext> GetBackJournal() => _backStack;

    /// <inheritdoc/>
    public virtual IEnumerable<NavigationContext> GetForwardJournal() => _forwardStack;

    /// <inheritdoc/>
    public virtual bool GoBack()
    {
        if (!CanGoBack()) return false;

        var previousContext = _backStack.Peek();
        return Navigate(CurrentContext?.Page, previousContext.Page, NavigationMode.Back, previousContext.Parameters);
    }

    /// <inheritdoc/>
    public virtual bool CanGoBack() => GetBackJournal().Any();

    /// <inheritdoc/>
    public virtual bool GoForward()
    {
        if (!CanGoForward()) return false;

        var nextContext = _forwardStack.Peek();
        return Navigate(CurrentContext?.Page, nextContext.Page, NavigationMode.Forward, nextContext.Parameters);
    }

    /// <inheritdoc/>
    public virtual bool CanGoForward() => GetForwardJournal().Any();

    /// <summary>
    /// Adds a navigation context to the back stack.
    /// </summary>
    /// <param name="navigatingContext">The navigation context to add.</param>
    protected virtual void AddBackEntry(NavigationContext navigatingContext) => _backStack.Push(navigatingContext);

    /// <summary>
    /// Adds a navigation context to the forward stack.
    /// </summary>
    /// <param name="navigatingContext">The navigation context to add.</param>
    protected virtual void AddForwardEntry(NavigationContext navigatingContext) => _forwardStack.Push(navigatingContext);

    /// <summary>
    /// Removes a navigation context from the back stack.
    /// </summary>
    /// <param name="context">The context to remove.</param>
    protected virtual void RemoveBackEntry(NavigationContext context)
    {
        if (_backStack.Count == 0) return;

        // Fast path: if the item to remove is at the top
        if (ReferenceEquals(_backStack.Peek(), context))
        {
            _ = _backStack.Pop();
            return;
        }

        // Use a list for efficient removal in the middle
        var items = _backStack.ToList();
        var index = items.FindIndex(x => ReferenceEquals(x, context));

        if (index >= 0)
        {
            items.RemoveAt(index);
            _backStack.Clear();

            // Rebuild stack in reverse order to maintain LIFO behavior
            for (var i = items.Count - 1; i >= 0; i--)
                _backStack.Push(items[i]);
        }
    }

    /// <summary>
    /// Removes a navigation context from the forward stack.
    /// </summary>
    /// <param name="context">The context to remove.</param>
    protected virtual void RemoveForwardEntry(NavigationContext context)
    {
        if (_forwardStack.Count == 0) return;

        // Fast path: if the item to remove is at the top
        if (ReferenceEquals(_forwardStack.Peek(), context))
        {
            _ = _forwardStack.Pop();
            return;
        }

        // Use a list for efficient removal in the middle
        var items = _forwardStack.ToList();
        var index = items.FindIndex(x => ReferenceEquals(x, context));

        if (index >= 0)
        {
            items.RemoveAt(index);
            _forwardStack.Clear();

            // Rebuild stack in reverse order to maintain LIFO behavior
            for (var i = items.Count - 1; i >= 0; i--)
                _forwardStack.Push(items[i]);
        }
    }

    /// <summary>
    /// Updates the navigation journal based on the navigation mode.
    /// </summary>
    /// <param name="mode">The navigation mode.</param>
    /// <param name="navigatingContext">The navigation context.</param>
    protected virtual void UpdateJournal(NavigationMode mode, NavigationContext navigatingContext)
    {
        switch (mode)
        {
            case NavigationMode.Normal:
                if (!JournalSuspender.IsSuspended)
                    AddBackEntry(navigatingContext);
                _forwardStack.Clear();
                break;
            case NavigationMode.Back:
                if (_backStack.Count > 0)
                    _ = _backStack.Pop();
                if (!JournalSuspender.IsSuspended)
                    AddForwardEntry(navigatingContext);
                break;
            case NavigationMode.Forward:
                if (_forwardStack.Count > 0)
                    _ = _forwardStack.Pop();
                if (!JournalSuspender.IsSuspended)
                    AddBackEntry(navigatingContext);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    /// <summary>
    /// Updates the current navigation context.
    /// </summary>
    /// <param name="navigationContext">The navigation context.</param>
    protected virtual void UpdateCurrentContext(NavigationContext navigationContext) => CurrentContext = navigationContext;

    /// <inheritdoc/>
    public virtual void ClearJournal()
    {
        _backStack.Clear();
        _forwardStack.Clear();

        RaiseHistoryCleared();
    }

    /// <inheritdoc/>
    public virtual void Clear()
    {
        CurrentContext = null;

        RaiseCleared();
    }

    /// <inheritdoc/>
    public virtual bool NavigateTo(INavigationPage page, NavigationParameters? navigationParameters = null) => Navigate(CurrentContext?.Page, page, NavigationMode.Normal, navigationParameters);

    /// <summary>
    /// Performs navigation between pages, handling events and updating context and history.
    /// </summary>
    /// <param name="oldPage">The previous page before navigation.</param>
    /// <param name="newPage">The destination page for navigation.</param>
    /// <param name="mode">The navigation mode.</param>
    /// <param name="navigationParameters">The navigation parameters.</param>
    /// <returns>True if navigation succeeded; otherwise, false.</returns>
    protected virtual bool Navigate(INavigationPage? oldPage, INavigationPage newPage, NavigationMode mode, NavigationParameters? navigationParameters)
    {
        var navigatingContext = new NavigatingContext(oldPage, newPage, mode, navigationParameters);

        OnNavigatingFrom(navigatingContext);

        if (navigatingContext.Cancel) return false;

        OnNavigatingTo(navigatingContext);

        if (navigatingContext.Cancel) return false;

        RaiseNavigating(navigatingContext);

        if (navigatingContext.Cancel) return false;

        if (CurrentContext is not null)
            UpdateJournal(mode, CurrentContext);

        UpdateCurrentContext(new NavigationContext(oldPage, newPage, mode, navigationParameters));

        OnNavigated(CurrentContext!);

        RaiseNavigated(CurrentContext!);

        return true;
    }

    /// <summary>
    /// Called when navigating from the current page.
    /// </summary>
    /// <param name="navigatingContext">The navigating context.</param>
    protected virtual void OnNavigatingFrom(NavigatingContext navigatingContext) => navigatingContext.OldPage?.OnNavigatingFrom(navigatingContext);

    /// <summary>
    /// Called when navigating to the new page.
    /// </summary>
    /// <param name="navigatingContext">The navigating context.</param>
    protected virtual void OnNavigatingTo(NavigatingContext navigatingContext) => navigatingContext.Page.OnNavigatingTo(navigatingContext);

    /// <summary>
    /// Called after navigation is completed.
    /// </summary>
    /// <param name="navigatingContext">The navigation context.</param>
    protected virtual void OnNavigated(NavigationContext navigatingContext) => navigatingContext.Page.OnNavigated(navigatingContext);
}
