// -----------------------------------------------------------------------
// <copyright file="NavigationJournal.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <inheritdoc />
public sealed class NavigationJournal : INavigationJournal
{
    private readonly Stack<NavigationContext> _back = new();
    private readonly Stack<NavigationContext> _forward = new();

    /// <inheritdoc />
    public IReadOnlyList<NavigationContext> BackStack => [.. _back];

    /// <inheritdoc />
    public IReadOnlyList<NavigationContext> ForwardStack => [.. _forward];

    /// <inheritdoc />
    public NavigationContext? PeekBack() => _back.Count > 0 ? _back.Peek() : null;

    /// <inheritdoc />
    public NavigationContext? PeekForward() => _forward.Count > 0 ? _forward.Peek() : null;

    /// <inheritdoc />
    public void PushBack(NavigationContext context) => _back.Push(context);

    /// <inheritdoc />
    public void PushForward(NavigationContext context) => _forward.Push(context);

    /// <inheritdoc />
    public NavigationContext? PopBack() => _back.Count > 0 ? _back.Pop() : null;

    /// <inheritdoc />
    public NavigationContext? PopForward() => _forward.Count > 0 ? _forward.Pop() : null;

    /// <inheritdoc />
    public void Clear()
    {
        _back.Clear();
        _forward.Clear();
    }

    /// <inheritdoc />
    public void ClearForward() => _forward.Clear();
}
