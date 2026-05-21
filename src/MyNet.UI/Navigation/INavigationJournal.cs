// -----------------------------------------------------------------------
// <copyright file="INavigationJournal.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Defines the interface for a navigation journal, which maintains a history of navigation contexts for back and forward navigation operations.
/// </summary>
public interface INavigationJournal
{
    /// <summary>
    /// Gets the list of navigation contexts in the back stack, representing the history of previously visited pages that can be navigated back to.
    /// </summary>
    IReadOnlyList<NavigationContext> BackStack { get; }

    /// <summary>
    /// Gets the list of navigation contexts in the forward stack, representing the history of pages that can be navigated forward to.
    /// </summary>
    IReadOnlyList<NavigationContext> ForwardStack { get; }

    /// <summary>
    /// Returns the next navigation context available in the back stack without removing it.
    /// </summary>
    /// <returns>The next back navigation context, or <see langword="null"/> when the stack is empty.</returns>
    NavigationContext? PeekBack();

    /// <summary>
    /// Returns the next navigation context available in the forward stack without removing it.
    /// </summary>
    /// <returns>The next forward navigation context, or <see langword="null"/> when the stack is empty.</returns>
    NavigationContext? PeekForward();

    /// <summary>
    /// Pushes a navigation context onto the back stack, indicating that the user has navigated to a new page or view. This method is typically called when performing a normal navigation operation, allowing the user to navigate back to the previous page or view if desired.
    /// </summary>
    /// <param name="context">The navigation context to push.</param>
    void PushBack(NavigationContext context);

    /// <summary>
    /// Pushes a navigation context onto the forward stack, indicating that the user has navigated forward to a new page or view. This method is typically called when performing a forward navigation operation after a backward navigation operation, allowing the user to navigate back to the previous page or view if desired.
    /// </summary>
    /// <param name="context">The navigation context to push.</param>
    void PushForward(NavigationContext context);

    /// <summary>
    /// Pops a navigation context from the back stack.
    /// </summary>
    /// <returns>The navigation context that was popped, or null if the back stack is empty.</returns>
    NavigationContext? PopBack();

    /// <summary>
    /// Pops a navigation context from the forward stack.
    /// </summary>
    /// <returns>The navigation context that was popped, or null if the forward stack is empty.</returns>
    NavigationContext? PopForward();

    /// <summary>
    /// Clears the navigation history.
    /// </summary>
    void Clear();

    /// <summary>
    /// Clears only the forward navigation history.
    /// </summary>
    void ClearForward();
}
