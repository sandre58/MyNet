// -----------------------------------------------------------------------
// <copyright file="BusyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Loading;

public static class BusyExtensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="IBusyService"/> interface, allowing for simplified execution of busy operations with proper state management and cancellation support.
    /// </summary>
    extension(IBusyService busyService)
    {
        /// <summary>
        /// Runs an indeterminate busy operation with the provided action, ensuring proper busy state management and cancellation support.
        /// </summary>
        /// <param name="action">The action to execute while the busy state is active.</param>
        public Task RunIndeterminateAsync(Func<IndeterminateBusy, CancellationToken, Task> action) => busyService.RunAsync(action);

        /// <summary>
        /// Runs an indeterminate busy operation with the provided action, ensuring proper busy state management and cancellation support.
        /// </summary>
        /// <param name="action">The action to execute while the busy state is active.</param>
        public Task RunIndeterminateAsync(Func<CancellationToken, Task> action)
            => busyService.RunAsync<IndeterminateBusy>((_, ct) => action(ct));

        /// <summary>
        /// Runs a determinate busy operation with the provided action, ensuring proper busy state management and cancellation support.
        /// </summary>
        /// <param name="action">The action to execute while the busy state is active.</param>
        public Task RunDeterminateAsync(Func<DeterminateBusy, CancellationToken, Task> action)
            => busyService.RunAsync(action);

        /// <summary>
        /// Runs an indeterminate busy operation with the provided action, ensuring proper busy state management and cancellation support.
        /// </summary>
        /// <param name="action">The action to execute while the busy state is active.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RunIndeterminateAsync(Action<IndeterminateBusy, CancellationToken> action)
            => busyService.RunAsync<IndeterminateBusy>((busy, ct) =>
            {
                action(busy, ct);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Runs an indeterminate busy operation with the provided action, ensuring proper busy state management and cancellation support.
        /// </summary>
        /// <param name="action">The action to execute while the busy state is active.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RunIndeterminateAsync(Action<CancellationToken> action)
            => busyService.RunAsync<IndeterminateBusy>((_, ct) =>
            {
                action(ct);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Runs a determinate busy operation with the provided action, ensuring proper busy state management and cancellation support.
        /// </summary>
        /// <param name="action">The action to execute while the busy state is active.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RunDeterminateAsync(Action<DeterminateBusy, CancellationToken> action)
            => busyService.RunAsync<DeterminateBusy>((busy, ct) =>
            {
                action(busy, ct);
                return Task.CompletedTask;
            });
    }
}
