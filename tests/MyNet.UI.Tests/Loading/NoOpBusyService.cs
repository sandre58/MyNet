// -----------------------------------------------------------------------
// <copyright file="NoOpBusyService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Loading;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Tests.Loading;

/// <summary>
/// Test double for <see cref="IBusyService"/> without global busy state tracking.
/// </summary>
internal sealed class NoOpBusyService : IBusyService
{
    public bool IsBusy => false;

    public IBusy? CurrentBusy => null;

#pragma warning disable CS0067
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

    public Task RunAsync<TBusy>(Func<TBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where TBusy : IBusy, new()
        => action(new(), cancellationToken);

    public Task<TResult> RunAsync<TBusy, TResult>(Func<TBusy, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
        where TBusy : IBusy, new()
        => action(new(), cancellationToken);

    public IBusyScope Begin<TBusy>(CancellationToken cancellationToken = default)
        where TBusy : IBusy, new()
        => new NoOpBusyScope<TBusy>(cancellationToken);

    public TBusy? GetCurrent<TBusy>()
        where TBusy : class, IBusy
        => null;

    private sealed class NoOpBusyScope<TBusy>(CancellationToken cancellationToken) : IBusyScope
        where TBusy : IBusy, new()
    {
        public IBusy Busy { get; } = new TBusy();

        public CancellationToken CancellationToken { get; } = cancellationToken;

        public void Dispose()
        {
        }
    }
}
