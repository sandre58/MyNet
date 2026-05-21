// -----------------------------------------------------------------------
// <copyright file="AppBusyManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using MyNet.UI.Legacy.Loading;
using MyNet.UI.Legacy.Loading.Models;

namespace MyNet.UI.Services;

/// <summary>
/// Provides static methods to manage busy states in the application, including main and background busy services.
/// Supports progress reporting, indeterminate operations, and background tasks.
/// </summary>
public static class AppBusyManager
{
    private static IBusyServiceFactory? _busyServiceFactory;

    /// <summary>
    /// Initializes the busy service factory used to create busy services.
    /// </summary>
    /// <param name="busyServiceFactory">The factory to create busy services.</param>
    public static void Initialize(IBusyServiceFactory busyServiceFactory) => _busyServiceFactory = busyServiceFactory;

    /// <summary>
    /// Gets the main busy service, creating it if necessary.
    /// </summary>
    public static IBusyService MainBusyService => GetOrCreate(ref field);

    /// <summary>
    /// Gets the background busy service, creating it if necessary.
    /// </summary>
    public static IBusyService BackgroundBusyService => GetOrCreate(ref field);

    /// <summary>
    /// Gets or creates a busy service using the factory.
    /// </summary>
    /// <param name="busyService">Reference to the busy service instance.</param>
    /// <returns>The busy service instance.</returns>
    private static IBusyService GetOrCreate(ref IBusyService? busyService)
    {
        if (busyService is not null)
            return busyService;
        if (_busyServiceFactory is null)
            throw new InvalidOperationException("Busy Service has not been Initialized.");

        busyService = _busyServiceFactory.Create();

        return busyService;
    }

    /// <summary>
    /// Executes an action asynchronously with a background indeterminate busy indicator.
    /// </summary>
    /// <param name="action">The action to execute with the busy indicator.</param>
    public static async Task BackgroundAsync(Action<IndeterminateBusy> action) => await BackgroundBusyService.WaitAsync(action).ConfigureAwait(false);

    /// <summary>
    /// Executes an action asynchronously with a background indeterminate busy indicator.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static async Task BackgroundAsync(Action action) => await BackgroundBusyService.WaitAsync<IndeterminateBusy>(_ => action()).ConfigureAwait(false);

    /// <summary>
    /// Executes an action asynchronously with the main indeterminate busy indicator.
    /// </summary>
    /// <param name="action">The action to execute with the busy indicator.</param>
    public static async Task WaitAsync(Action<IndeterminateBusy> action) => await MainBusyService.WaitAsync(action).ConfigureAwait(false);

    /// <summary>
    /// Executes an action asynchronously with the main indeterminate busy indicator.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static async Task WaitAsync(Action action) => await MainBusyService.WaitAsync<IndeterminateBusy>(_ => action()).ConfigureAwait(false);

    /// <summary>
    /// Executes an action asynchronously with the main progression busy indicator.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static async Task ProgressAsync(Action action) => await MainBusyService.WaitAsync<ProgressionBusy>(_ => action()).ConfigureAwait(false);

    /// <summary>
    /// Executes a task asynchronously with the main progression busy indicator.
    /// </summary>
    /// <param name="task">The task to execute.</param>
    public static async Task ProgressAsync(Task task) => await MainBusyService.WaitAsync(new Func<ProgressionBusy, Task>(_ => task)).ConfigureAwait(false);

    /// <summary>
    /// Resumes normal operation by ending the main busy state.
    /// </summary>
    public static void Resume() => MainBusyService.Resume();

    /// <summary>
    /// Instantiates and marks the UI as busy with a progression busy indicator.
    /// </summary>
    public static void Progress() => _ = MainBusyService.Wait<ProgressionBusy>();
}
