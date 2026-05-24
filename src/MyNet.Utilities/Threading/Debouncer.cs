// -----------------------------------------------------------------------
// <copyright file="Debouncer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace MyNet.Utilities.Threading;

/// <summary>
/// Coalesces rapid repeated signals into a single callback after a quiet period.
/// </summary>
internal sealed class Debouncer : IDisposable
{
    private readonly Lock _lock = new();
    private readonly Timer _timer;
    private readonly int _delayMilliseconds;
    private Action? _pending;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Debouncer"/> class with the specified delay in milliseconds. The delay represents the quiet period after which the pending action will be invoked if no new signals are received.
    /// </summary>
    /// <param name="delayMilliseconds">The delay in milliseconds.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the delay is negative.</exception>
    public Debouncer(int delayMilliseconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(delayMilliseconds);
        _delayMilliseconds = delayMilliseconds;
        _timer = new(static s => ((Debouncer)s!).Fire(), this, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// Schedules the specified action to be invoked after the delay. If called again before the delay elapses, the previous pending action is discarded and replaced with the new one, effectively resetting the timer.
    /// </summary>
    /// <param name="action">The action to be invoked after the delay.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the debouncer has been disposed.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the action is null.</exception>
    public void Schedule(Action action)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(action);

        lock (_lock)
        {
            _pending = action;
            _ = _timer.Change(_delayMilliseconds, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Invokes the pending action if any, and clears it. This method is called by the timer callback when the delay elapses.
    /// </summary>
    private void Fire()
    {
        Action? action;

        lock (_lock)
        {
            action = _pending;
            _pending = null;
        }

        action?.Invoke();
    }

    /// <summary>
    /// Disposes the debouncer, preventing any pending or future scheduled actions from being invoked.
    /// </summary>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
                return;

            _disposed = true;
            _pending = null;
            _timer.Dispose();
        }
    }
}
