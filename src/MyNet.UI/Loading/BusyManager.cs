// -----------------------------------------------------------------------
// <copyright file="BusyManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Loading;

/// <summary>
/// Provides a global access point for the application's main <see cref="IBusyService"/>.
/// Allows initialization, creation, and usage of busy services and helpers for common busy scenarios.
/// </summary>
public static class BusyManager
{
    private static IBusyServiceFactory? _busyServiceFactory;
    private static IBusyService? _default;

    /// <summary>
    /// Gets the default <see cref="IBusyService"/> instance for the application.
    /// </summary>
    public static IBusyService Default
    {
        get
        {
            _default ??= Create();
            return _default;
        }
    }

    /// <summary>
    /// Initializes the <see cref="BusyManager"/> with the specified <see cref="IBusyServiceFactory"/>.
    /// </summary>
    /// <param name="busyServiceFactory">The factory used to create <see cref="IBusyService"/> instances.</param>
    public static void Initialize(IBusyServiceFactory busyServiceFactory) => _busyServiceFactory = busyServiceFactory;

    /// <summary>
    /// Creates a new <see cref="IBusyService"/> instance using the configured factory.
    /// </summary>
    /// <returns>A new <see cref="IBusyService"/> instance.</returns>
    public static IBusyService Create() => _busyServiceFactory!.Create();

    /// <summary>
    /// Executes an indeterminate busy operation using the default busy service and the specified action.
    /// </summary>
    /// <param name="action">The action to execute with an <see cref="IndeterminateBusy"/> indicator.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WaitIndeterminateAsync(Action<IndeterminateBusy> action)
        => await Default.RunAsync(action).ConfigureAwait(false);

    /// <summary>
    /// Executes an indeterminate busy operation using the default busy service and the specified action.
    /// </summary>
    /// <param name="action">The action to execute while busy.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WaitIndeterminateAsync(Action action)
        => await Default.RunAsync<IndeterminateBusy>(_ => action()).ConfigureAwait(false);

    /// <summary>
    /// Executes a determinate busy operation using the default busy service and the specified action.
    /// </summary>
    /// <param name="action">The action to execute with a <see cref="DeterminateBusy"/> indicator.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WaitDeterminateAsync(Action<DeterminateBusy> action)
        => await Default.RunAsync(action).ConfigureAwait(false);
}
