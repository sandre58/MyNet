// -----------------------------------------------------------------------
// <copyright file="ProgresserBusyExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Loading.Models;
using MyNet.Utilities.Progress;

#pragma warning disable IDE0130
namespace MyNet.UI.Loading;
#pragma warning restore IDE0130

/// <summary>
/// Bridges the hierarchical progress API (<see cref="IProgresser"/>) onto the busy system:
/// a <see cref="ProgressionBusy"/> scope is opened for the duration of the operation and kept in
/// sync with every <see cref="ProgressReport{T}"/> emitted by the progresser (value, breadcrumb
/// message, cancellation availability and cancel action).
/// </summary>
public static class ProgresserBusyExtensions
{
    extension(IBusyService busyService)
    {
        /// <summary>
        /// Runs <paramref name="operation"/> inside a <see cref="ProgressionBusy"/> scope while mirroring
        /// the progress reported through <paramref name="progresser"/>. The operation receives the same
        /// <paramref name="progresser"/> so it can open steps with <c>Begin</c>/<c>StartStep</c>.
        /// </summary>
        /// <param name="progresser">The progresser whose reports drive the busy presentation.</param>
        /// <param name="operation">The asynchronous work to execute.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        public Task RunWithProgressAsync(
            IProgresser progresser,
            Func<IProgresser, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(progresser);
            ArgumentNullException.ThrowIfNull(operation);

            return busyService.RunAsync<ProgressionBusy>(
                (busy, token) => RunBridgedAsync(progresser, busy, () => operation(progresser, token), token),
                cancellationToken);
        }

        /// <summary>
        /// Runs <paramref name="operation"/> inside a <see cref="ProgressionBusy"/> scope, mirroring the
        /// progress reported through a freshly created <see cref="Progresser"/> handed to the operation.
        /// </summary>
        /// <param name="operation">The asynchronous work to execute.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        public Task RunWithProgressAsync(
            Func<IProgresser, CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
            => busyService.RunWithProgressAsync(new Progresser(), operation, cancellationToken);
    }

    private static async Task RunBridgedAsync(
        IProgresser progresser,
        ProgressionBusy busy,
        Func<Task> operation,
        CancellationToken token)
    {
        ProgressReport<ProgressMessage>? lastReport = null;
        string? lastMessage = null;

        // A cancel request coming from the busy UI (its CancellationToken) is forwarded to the
        // cancel action carried by the latest progress report, so the progress session stops too.
        await using var registration = token.Register(() => lastReport?.CancelAction?.Invoke()).ConfigureAwait(false);

        // Progress<T> marshals callbacks to the synchronization context captured here, so the busy
        // model (and its bound UI) is always updated on the originating thread.
        var sink = new Progress<ProgressReport<ProgressMessage>>(report =>
        {
            lastReport = report;
            busy.CanCancel = report.CanCancel;
            busy.Value = report.Progress;

            var message = report.Messages.Count > 0 ? report.Messages[^1].ToString() : null;
            if (string.IsNullOrWhiteSpace(message))
                return;

            busy.Title = message;

            // Only append a breadcrumb entry when the innermost message actually changes,
            // otherwise repeated reports for the same step would flood the history.
            if (!string.Equals(message, lastMessage, StringComparison.Ordinal))
            {
                busy.Messages.Add(message);
                lastMessage = message;
            }
        });

        progresser.Subscribe(sink);
        try
        {
            await operation().ConfigureAwait(false);
        }
        finally
        {
            progresser.Unsubscribe(sink);
        }
    }
}
