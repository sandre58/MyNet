// -----------------------------------------------------------------------
// <copyright file="BusyScope.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using MyNet.UI.Loading.Models;

namespace MyNet.UI.Loading;

/// <summary>
/// Disposable scope created by <see cref="BusyService"/>.
/// </summary>
public sealed class BusyScope : IBusyScope
{
    private readonly BusyService _owner;
    private int _disposed;

    internal BusyScope(BusyService owner, IBusy busy, CancellationToken cancellationToken)
    {
        _owner = owner;
        Busy = busy;
        CancellationToken = cancellationToken;
    }

    /// <inheritdoc />
    public IBusy Busy { get; }

    /// <inheritdoc />
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _owner.Pop(this);
    }
}
