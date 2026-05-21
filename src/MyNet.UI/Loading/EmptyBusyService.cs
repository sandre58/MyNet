// -----------------------------------------------------------------------
// <copyright file="EmptyBusyService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Loading;

/// <summary>
/// Provides a no-op implementation of the IBusyService interface, useful for scenarios where busy state management is not needed or should be disabled.
/// </summary>
public class EmptyBusyService : IBusyService
{
    /// <summary>
    /// Gets a singleton instance of the EmptyBusyService, which can be used throughout the application to provide a consistent no-op busy service implementation without the need for multiple instances.
    /// </summary>
    public static readonly EmptyBusyService Instance = new();

    /// <inheritdoc />
    public bool IsBusy => false;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public Task RunAsync<TBusy>(Func<TBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where TBusy : class, IBusy, new()
        => action(new(), cancellationToken);

    /// <inheritdoc />
    public Task<TResult> RunAsync<TBusy, TResult>(Func<TBusy, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
        where TBusy : Busy, new()
        => action(new(), cancellationToken);

    /// <inheritdoc />
    public IBusyScope Begin<TBusy>(CancellationToken cancellationToken = default)
        where TBusy : Busy, new()
        => new EmptyBusyScope();

    /// <inheritdoc />
    public TBusy? GetCurrent<TBusy>()
        where TBusy : class, IBusy => null;
}

/// <summary>
/// Initializes a new instance of the <see cref="NoopBusyScope"/> class.
/// </summary>
public sealed class EmptyBusyScope(CancellationToken cancellationToken = default) : IBusyScope
{
    private static readonly IBusy EmptyBusy = new IndeterminateBusy();

    /// <inheritdoc />
    public IBusy Busy => EmptyBusy;

    /// <inheritdoc />
    public CancellationToken CancellationToken { get; } = cancellationToken;

    /// <inheritdoc />
    public void Dispose()
    {
        // Intentionally does nothing
    }
}
