// -----------------------------------------------------------------------
// <copyright file="AutoSaveEngine.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Logging;

namespace MyNet.Utilities.IO.AutoSave;

/// <summary>
/// Base class for implementing an auto-save engine that periodically saves data at a specified interval.
/// </summary>
/// <param name="interval">The interval at which the auto-save engine should save data.</param>
/// <param name="logger">An optional logger for logging messages and exceptions.</param>
public abstract class AutoSaveEngine(TimeSpan interval, ILogger? logger = null) : IAutoSaveEngine, IDisposable
{
    private readonly ILogger _logger = logger ?? Log.Create<AutoSaveEngine>();
    private readonly SemaphoreSlim _executionLock = new(1, 1);

    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    private bool _enabled;
    private bool _disposed;

    /// <summary>
    /// Gets a value indicating whether the auto-save engine is currently running. This property returns <c>true</c> if the engine is active and has not completed its loop task; otherwise, it returns <c>false</c>.
    /// </summary>
    public bool IsRunning => _loopTask is { IsCompleted: false };

    /// <summary>
    /// Gets a value indicating whether the auto-save engine is currently performing a save operation. This property is set to <c>true</c> when a save operation is in progress and is reset to <c>false</c> once the operation completes, allowing consumers to check if a save is currently underway.
    /// </summary>
    public bool IsSaving { get; private set; }

    /// <summary>
    /// Gets the interval at which the auto-save engine saves data. This property is initialized with the value provided in the constructor and can be updated using the SetInterval method. The interval must be a positive TimeSpan, and attempts to set it to zero or a negative value will result in an exception being thrown.
    /// </summary>
    public TimeSpan Interval { get; private set; } = interval;

    /// <summary>
    /// Sets the interval at which the auto-save engine saves data. The provided interval must be greater than zero; otherwise, an <see cref="ArgumentOutOfRangeException"/> will be thrown. This method allows for dynamically updating the save interval while the engine is running, enabling flexibility in how frequently data is saved based on runtime conditions or user preferences.
    /// </summary>
    /// <param name="interval">The new interval at which the auto-save engine should save data.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided interval is less than or equal to zero.</exception>
    public void SetInterval(TimeSpan interval)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

        Interval = interval;
    }

    /// <summary>
    /// Triggers an immediate save operation, allowing consumers to request a save outside of the regular interval. This method attempts to acquire the execution lock without waiting, and if it is already held (indicating that a save operation is in progress), it will simply return without performing a save. If the lock is acquired successfully, it will execute the save operation asynchronously, ensuring that the auto-save engine can respond to manual save requests while still maintaining its regular saving schedule.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the save operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public Task TriggerSaveAsync(CancellationToken ct = default) => ExecuteSaveAsync(ct);

    /// <summary>
    /// Starts the auto-save engine, initiating the periodic save loop. If the engine is already running or has been disposed, this method will return without taking any action. When started, a new cancellation token source is created to manage the lifecycle of the loop task, and the loop task is initiated to run the save operations at the specified intervals. The engine will continue to run until it is stopped or disposed, allowing for continuous auto-saving of data based on the configured interval.
    /// </summary>
    public void Start()
    {
        if (_disposed || _enabled)
            return;

        _enabled = true;

        _cts = new();
        _loopTask = RunLoopAsync(_cts.Token);
    }

    /// <summary>
    /// Stops the auto-save engine, signaling it to cease periodic save operations. If the engine is not currently running, this method will return without taking any action. When stopped, the cancellation token source is canceled and disposed of, which signals the loop task to exit gracefully. The engine will no longer perform auto-saving until it is started again, allowing for control over when data is automatically saved based on application state or user actions.
    /// </summary>
    public void Stop()
    {
        if (!_enabled)
            return;

        _enabled = false;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    /// <summary>
    /// Cancels the current save operation if it is in progress. This method can be used to interrupt an ongoing save operation, allowing for scenarios where a save may need to be aborted due to user actions or changes in application state. If no save operation is currently in progress, this method will have no effect.
    /// </summary>
    public void Cancel() => _cts?.Cancel();

    /// <summary>
    /// Suspends the auto-save engine, preventing it from performing save operations until the returned <see cref="IDisposable"/> is disposed. This can be useful for temporarily pausing auto-save during critical operations or when making multiple changes that should be saved together.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that, when disposed, will resume the auto-save engine.</returns>
    public IDisposable Suspend() => new SuspensionScope(this);

    /// <summary>
    /// Runs the main loop of the auto-save engine, which periodically executes save operations based on the configured interval. The loop continues to run until the provided cancellation token is canceled, at which point it will exit gracefully. During each iteration of the loop, it waits for the specified interval before attempting to execute a save operation. If an exception occurs during the save operation, it is logged using the provided logger, and the loop continues to run, allowing for resilience in the face of transient errors.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the loop.</param>
    private async Task RunLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(Interval, token).ConfigureAwait(false);

                await ExecuteSaveAsync(token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // expected
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
    }

    /// <summary>
    /// Executes the save operation, ensuring that only one save can occur at a time by using a semaphore to control access.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the save operation.</param>
    private async Task ExecuteSaveAsync(CancellationToken token)
    {
        if (!await _executionLock.WaitAsync(0, token).ConfigureAwait(false))
            return;

        try
        {
            IsSaving = true;

            await SaveCoreAsync(token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
        }
        finally
        {
            IsSaving = false;
            _executionLock.Release();
        }
    }

    /// <summary>
    /// When implemented in a derived class, performs the actual save operation. This method is called by the ExecuteSaveAsync method, which ensures that only one save operation can occur at a time and handles any exceptions that may arise during the save process. The implementation of this method should contain the logic for saving data, and it should respect the provided cancellation token to allow for graceful cancellation of the save operation when requested.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the save operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    protected abstract Task SaveCoreAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Releases all resources used by the auto-save engine. This method stops the engine if it is currently running and disposes of the execution lock. After calling this method, the auto-save engine will no longer be usable, and any attempts to start or trigger saves will have no effect. It is important to call this method when the auto-save engine is no longer needed to ensure that all resources are properly released and to prevent potential memory leaks.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the auto-save engine and optionally releases the managed resources. If disposing is true, the method releases both managed and unmanaged resources; if false, it releases only unmanaged resources. This method is called by the public Dispose method and can also be called by a finalizer if one is implemented in a derived class. The base implementation ensures that the engine is stopped and that the execution lock is disposed of, while derived classes can override this method to release any additional resources they may be using.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the method is being called from the Dispose method (true) or from a finalizer (false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Stop();
            _executionLock.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Represents a scope that temporarily suspends the auto-save engine when created and resumes it when disposed. This class is used by the Suspend method of the AutoSaveEngine to allow consumers to pause auto-saving during critical operations or when making multiple changes that should be saved together. When an instance of this class is created, it cancels any ongoing save operation and sets the engine's enabled state to false, effectively suspending auto-saving. When the instance is disposed, it sets the engine's enabled state back to true, allowing auto-saving to resume.
    /// </summary>
    private sealed class SuspensionScope : IDisposable
    {
        private readonly AutoSaveEngine _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionScope"/> class, which suspends the auto-save engine by canceling any ongoing save operation and setting the engine's enabled state to false. This constructor is called by the Suspend method of the AutoSaveEngine, and it ensures that auto-saving is paused while the scope is active. When the scope is disposed, it will resume auto-saving by setting the engine's enabled state back to true.
        /// </summary>
        /// <param name="engine">The auto-save engine to be suspended.</param>
        public SuspensionScope(AutoSaveEngine engine)
        {
            _engine = engine;
            _engine.Cancel();
            _engine._enabled = false;
        }

        /// <summary>
        /// Releases the resources used by the <see cref="SuspensionScope"/> and resumes the auto-save engine by setting its enabled state back to true. This method is called when the scope is disposed, allowing auto-saving to resume after being temporarily suspended. It is important to dispose of the scope properly to ensure that the auto-save engine can continue functioning as expected after the critical operations or changes that required suspension are completed.
        /// </summary>
        public void Dispose() => _engine._enabled = true;
    }
}
