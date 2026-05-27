// -----------------------------------------------------------------------
// <copyright file="INavigationJournal.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Back and forward navigation history.
/// </summary>
public interface INavigationJournal
{
    /// <summary>Gets contexts that can be reached via <see cref="INavigationService.GoBackAsync"/>.</summary>
    IReadOnlyList<NavigationContext> BackStack { get; }

    /// <summary>Gets contexts that can be reached via <see cref="INavigationService.GoForwardAsync"/>.</summary>
    IReadOnlyList<NavigationContext> ForwardStack { get; }

    /// <summary>Returns the next back entry without removing it.</summary>
    NavigationContext? PeekBack();

    /// <summary>Returns the next forward entry without removing it.</summary>
    NavigationContext? PeekForward();

    /// <summary>Pushes a context onto the back stack.</summary>
    void PushBack(NavigationContext context);

    /// <summary>Pushes a context onto the forward stack.</summary>
    void PushForward(NavigationContext context);

    /// <summary>Removes and returns the top back entry, if any.</summary>
    NavigationContext? PopBack();

    /// <summary>Removes and returns the top forward entry, if any.</summary>
    NavigationContext? PopForward();

    /// <summary>Clears both stacks.</summary>
    void Clear();

    /// <summary>Clears only the forward stack.</summary>
    void ClearForward();
}
