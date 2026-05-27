// -----------------------------------------------------------------------
// <copyright file="BusyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Loading.Models;

#pragma warning disable IDE0130
namespace MyNet.UI.Loading;
#pragma warning restore IDE0130

/// <summary>
/// Extension methods for <see cref="IBusyService"/>.
/// </summary>
public static class BusyExtensions
{
    /// <summary>
    /// Extension methods for <see cref="IBusyService"/>.
    /// </summary>
    extension(IBusyService busyService)
    {
        /// <summary>
        /// Runs an indeterminate busy operation.
        /// </summary>
        public Task RunIndeterminateAsync(Func<IndeterminateBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
            => busyService.RunAsync(action, cancellationToken);

        /// <summary>
        /// Runs an indeterminate busy operation without access to the busy instance.
        /// </summary>
        public Task RunIndeterminateAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
            => busyService.RunAsync<IndeterminateBusy>((_, ct) => action(ct), cancellationToken);

        /// <summary>
        /// Runs a determinate busy operation.
        /// </summary>
        public Task RunDeterminateAsync(Func<DeterminateBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
            => busyService.RunAsync(action, cancellationToken);

        /// <summary>
        /// Runs a progression busy operation (multi-step progress and message history).
        /// </summary>
        public Task RunProgressionAsync(Func<ProgressionBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
            => busyService.RunAsync(action, cancellationToken);

        /// <summary>
        /// Runs a progression busy operation without access to the cancellation token.
        /// </summary>
        public Task RunProgressionAsync(Func<ProgressionBusy, Task> action)
            => busyService.RunAsync<ProgressionBusy>((busy, _) => action(busy));

        /// <summary>
        /// Runs an indeterminate busy operation (synchronous action body).
        /// </summary>
        public Task RunIndeterminateAsync(Action<IndeterminateBusy, CancellationToken> action)
            => busyService.RunAsync<IndeterminateBusy>((busy, ct) =>
            {
                action(busy, ct);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Runs an indeterminate busy operation (synchronous action body).
        /// </summary>
        public Task RunIndeterminateAsync(Action<CancellationToken> action)
            => busyService.RunAsync<IndeterminateBusy>((_, ct) =>
            {
                action(ct);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Runs a determinate busy operation (synchronous action body).
        /// </summary>
        public Task RunDeterminateAsync(Action<DeterminateBusy, CancellationToken> action)
            => busyService.RunAsync<DeterminateBusy>((busy, ct) =>
            {
                action(busy, ct);
                return Task.CompletedTask;
            });

        /// <summary>
        /// Runs a progression busy operation (synchronous action body).
        /// </summary>
        public Task RunProgressionAsync(Action<ProgressionBusy, CancellationToken> action)
            => busyService.RunAsync<ProgressionBusy>((busy, ct) =>
            {
                action(busy, ct);
                return Task.CompletedTask;
            });
    }
}
