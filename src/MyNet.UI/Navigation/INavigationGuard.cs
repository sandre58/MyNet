// -----------------------------------------------------------------------
// <copyright file="INavigationGuard.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Navigation.Models;

namespace MyNet.UI.Navigation;

/// <summary>
/// Evaluates whether a navigation request may proceed.
/// </summary>
public interface INavigationGuard
{
    /// <summary>Returns <see langword="false"/> to cancel navigation.</summary>
    Task<bool> CanNavigateAsync(NavigationContext? from, NavigationContext to, CancellationToken cancellationToken);
}
