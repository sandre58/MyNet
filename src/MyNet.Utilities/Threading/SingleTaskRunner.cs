// -----------------------------------------------------------------------
// <copyright file="SingleTaskRunner.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Utilities.Logging;

namespace MyNet.Utilities.Threading;

public class SingleTaskRunner(
    Action<CancellationToken> action,
    Action<bool>? onRunningChanged = null,
    Action? onCancelled = null,
    ILogger? logger = null) : IDisposable
{
    private readonly Lock _lock = new();
    private volatile bool _isRunning;

    private CancellationTokenSource? _tokenSource;
    private bool _disposedValue;

    public bool IsRunning
    {
        get
        {
            lock (_lock)
            {
                return _isRunning;
            }
        }
    }

    public void Cancel() => _tokenSource?.Cancel();

    public void Run()
    {
        lock (_lock)
        {
            if (_isRunning) return;

            _isRunning = true;
            onRunningChanged?.Invoke(_isRunning);
        }

        _ = Task.Run(() =>
        {
            try
            {
                RunSimpleTask();
            }
            catch (Exception ex)
            {
                logger?.Error(ex);
            }
            finally
            {
                lock (_lock)
                {
                    _isRunning = false;
                    onRunningChanged?.Invoke(_isRunning);
                }
            }
        });
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _tokenSource?.Dispose();
        }

        _disposedValue = true;
    }

    private void RunSimpleTask()
    {
        _tokenSource = new CancellationTokenSource();
        try
        {
            action(_tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            onCancelled?.Invoke();
        }
    }
}
