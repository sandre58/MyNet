// -----------------------------------------------------------------------
// <copyright file="AsyncRelayCommand.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.UI.Threading;

namespace MyNet.UI.Commands;

/// <summary>
/// An implementation of <see cref="IAsyncCommand"/> that relays its functionality to the provided asynchronous delegates. It also manages the execution state to prevent concurrent executions and provides a mechanism to raise CanExecuteChanged events on the UI thread.
/// </summary>
/// <param name="execute">The asynchronous delegate to execute when the command is invoked.</param>
/// <param name="canExecute">An optional delegate to determine whether the command can execute.</param>
/// <param name="schedulerProvider">An optional scheduler provider to specify the UI thread scheduler.</param>
public sealed class AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null, ISchedulerProvider? schedulerProvider = null) : IAsyncCommand, IRaiseCanExecuteChanged
{
    private readonly Func<Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly IScheduler _uiScheduler = schedulerProvider?.Ui ?? CurrentThreadScheduler.Instance;
    private int _isExecuting;

    /// <summary>
    /// Gets the event that is raised when the return value of the CanExecute method changes. This event should be raised whenever the conditions that determine whether the command can execute have changed.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute in its current state. The command can execute if it is not currently executing and if the canExecute delegate returns true (or is null). This method is thread-safe and can be called from any thread.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns><see langword="true"/> if the command can execute; otherwise, <see langword="false"/>.</returns>
    public bool CanExecute(object? parameter) => Volatile.Read(ref _isExecuting) == 0 && (canExecute?.Invoke() ?? true);

    /// <summary>
    /// Asynchronously executes the command. If the command cannot execute, this method does nothing. This method is thread-safe and can be called from any thread. It manages the execution state to prevent concurrent executions and raises CanExecuteChanged events before and after execution to update the command's availability in the UI.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    public async Task ExecuteAsync(object? parameter)
    {
        if (!CanExecute(parameter) || Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0)
            return;

        try
        {
            RaiseCanExecuteChanged();

            await _execute().ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref _isExecuting, 0);
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Asynchronously executes the command. This method is required by the ICommand interface and simply calls the ExecuteAsync method. It is marked as async void because ICommand.Execute does not return a Task, but it should be used with caution as it can lead to unobserved exceptions if the asynchronous operation fails. In this implementation, any exceptions thrown by the execute delegate will be unhandled and may crash the application, so it is recommended to handle exceptions within the execute delegate itself.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    [SuppressMessage("ReSharper", "AsyncVoidMethod", Justification = "Required by ICommand interface.")]
    async void ICommand.Execute(object? parameter) => await ExecuteAsync(parameter).ConfigureAwait(false);

    /// <summary>
    /// Raises the CanExecuteChanged event to indicate that the return value of the CanExecute method has changed. This method should be called whenever the conditions that determine whether the command can execute have changed.
    /// </summary>
    public void RaiseCanExecuteChanged() => _uiScheduler.Schedule(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
}

/// <summary>
/// An implementation of <see cref="IAsyncCommand"/> that relays its functionality to the provided asynchronous delegates. It also manages the execution state to prevent concurrent executions and provides a mechanism to raise CanExecuteChanged events on the UI thread. The command can execute if the parameter is of type T or is null, and if the canExecute delegate returns true (or is null).
/// </summary>
/// <param name="execute">The asynchronous delegate to execute when the command is invoked.</param>
/// <param name="canExecute">The delegate that determines whether the command can execute.</param>
/// <param name="schedulerProvider">The scheduler provider for UI thread scheduling.</param>
/// <typeparam name="T">The type of the command parameter.</typeparam>
public sealed class AsyncRelayCommand<T>(Func<T?, Task> execute, Func<T?, bool>? canExecute = null, ISchedulerProvider? schedulerProvider = null) : IAsyncCommand, IRaiseCanExecuteChanged
{
    private readonly Func<T?, Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly IScheduler _uiScheduler = schedulerProvider?.Ui ?? CurrentThreadScheduler.Instance;
    private int _isExecuting;

    /// <summary>
    /// Gets the event that is raised when the return value of the CanExecute method changes. This event should be raised whenever the conditions that determine whether the command can execute have changed.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute in its current state. The command can execute if it is not currently executing, if the parameter is of type T or is null, and if the canExecute delegate returns true (or is null). This method is thread-safe and can be called from any thread.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    public bool CanExecute(object? parameter)
        => Volatile.Read(ref _isExecuting) == 0
            && (parameter is T or null)
            && (canExecute?.Invoke((T?)parameter) ?? true);

    /// <summary>
    /// Asynchronously executes the command. If the command cannot execute, this method does nothing. This method is thread-safe and can be called from any thread. It manages the execution state to prevent concurrent executions and raises CanExecuteChanged events before and after execution to update the command's availability in the UI.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(object? parameter)
    {
        if (!CanExecute(parameter) || Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0)
            return;

        try
        {
            RaiseCanExecuteChanged();
            await _execute((T?)parameter).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref _isExecuting, 0);
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Asynchronously executes the command. This method is required by the ICommand interface and simply calls the ExecuteAsync method. It is marked as async void because ICommand.Execute does not return a Task, but it should be used with caution as it can lead to unobserved exceptions if the asynchronous operation fails. In this implementation, any exceptions thrown by the execute delegate will be unhandled and may crash the application, so it is recommended to handle exceptions within the execute delegate itself.
    /// </summary>
    /// <param name="parameter">The command parameter.</param>
    [SuppressMessage("ReSharper", "AsyncVoidMethod", Justification = "Required by ICommand interface.")]
    async void ICommand.Execute(object? parameter) => await ExecuteAsync(parameter).ConfigureAwait(false);

    /// <summary>
    /// Raises the CanExecuteChanged event to indicate that the return value of the CanExecute method has changed. This method should be called whenever the conditions that determine whether the command can execute have changed.
    /// </summary>
    public void RaiseCanExecuteChanged() => _uiScheduler.Schedule(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
}
