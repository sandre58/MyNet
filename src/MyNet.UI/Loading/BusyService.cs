// -----------------------------------------------------------------------
// <copyright file="BusyService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Observable;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Loading;

/// <summary>
/// Default <see cref="IBusyService"/> implementation with nested scopes and cancellation support.
/// </summary>
public sealed class BusyService : ObservableObject, IBusyService
{
    private readonly Lock _gate = new();
    private readonly Stack<BusyStackEntry> _stack = new();

    /// <inheritdoc />
    public bool IsBusy
    {
        get
        {
            lock (_gate)
                return _stack.Count > 0;
        }
    }

    /// <inheritdoc />
    public IBusy? CurrentBusy
    {
        get
        {
            lock (_gate)
                return _stack.Count > 0 ? _stack.Peek().Busy : null;
        }
    }

    /// <inheritdoc />
    public async Task RunAsync<TBusy>(Func<TBusy, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where TBusy : IBusy, new()
    {
        ArgumentNullException.ThrowIfNull(action);

        using var scope = BeginScope<TBusy>(cancellationToken);
        await action((TBusy)scope.Busy, scope.CancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<TResult> RunAsync<TBusy, TResult>(Func<TBusy, CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
        where TBusy : IBusy, new()
    {
        ArgumentNullException.ThrowIfNull(action);

        using var scope = BeginScope<TBusy>(cancellationToken);
        return await action((TBusy)scope.Busy, scope.CancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IBusyScope Begin<TBusy>(CancellationToken cancellationToken = default)
        where TBusy : IBusy, new()
        => BeginScope<TBusy>(cancellationToken);

    /// <inheritdoc />
    public TBusy? GetCurrent<TBusy>()
        where TBusy : class, IBusy
    {
        lock (_gate)
            return _stack.Count > 0 ? _stack.Peek().Busy as TBusy : null;
    }

    internal void Pop(BusyScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        lock (_gate)
        {
            if (_stack.Count == 0 || !ReferenceEquals(_stack.Peek().Scope, scope))
                return;

            var entry = _stack.Pop();
            entry.DisposeResources();
            RaiseStateChanged();
        }
    }

    private static CancellationToken Bind<TBusy>(TBusy busy, CancellationToken cancellationToken, out CancellationTokenSource? linkedSource)
        where TBusy : IBusy
    {
        ArgumentNullException.ThrowIfNull(busy);

        if (busy is not Busy bindable)
        {
            linkedSource = null;
            return cancellationToken;
        }

        linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        bindable.BindCancellation(linkedSource);
        return linkedSource.Token;
    }

    private BusyScope BeginScope<TBusy>(CancellationToken cancellationToken)
        where TBusy : IBusy, new()
    {
        var busy = new TBusy();
        var token = Bind(busy, cancellationToken, out var linkedSource);
        var scope = new BusyScope(this, busy, token);

        lock (_gate)
        {
            _stack.Push(new BusyStackEntry(busy, linkedSource, scope));
            RaiseStateChanged();
        }

        return scope;
    }

    private void RaiseStateChanged()
    {
        NotifyPropertyChanged(nameof(IsBusy));
        NotifyPropertyChanged(nameof(CurrentBusy));
    }

    private sealed class BusyStackEntry(IBusy busy, CancellationTokenSource? linkedSource, BusyScope scope)
    {
        public IBusy Busy { get; } = busy;

        public BusyScope Scope { get; } = scope;

        public void DisposeResources() => linkedSource?.Dispose();
    }
}
