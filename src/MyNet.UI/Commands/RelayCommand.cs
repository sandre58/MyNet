// -----------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Reactive.Concurrency;
using System.Windows.Input;
using MyNet.UI.Threading;

namespace MyNet.UI.Commands;

/// <summary>
/// A command that relays its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
/// </summary>
/// <param name="execute">The action to execute.</param>
/// <param name="canExecute">A function that determines whether the command can execute.</param>
/// <param name="schedulerProvider">Optional scheduler provider used to raise CanExecuteChanged on the UI scheduler.</param>
public class RelayCommand(Action execute, Func<bool>? canExecute = null, ISchedulerProvider? schedulerProvider = null) : ICommand, IRaiseCanExecuteChanged
{
    private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly IScheduler _uiScheduler = schedulerProvider?.Ui ?? CurrentThreadScheduler.Instance;

    /// <summary>
    /// Gets the function that determines whether the command can execute. If null, the command can always execute.
    /// </summary>
    public virtual event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute in its current state. If the canExecute function is null, this method returns true.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the canExecute function.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    public virtual bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    /// <summary>
    /// Executes the command. If the command cannot execute, this method does nothing.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the execute action.</param>
    public virtual void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        _execute();
    }

    /// <summary>
    /// Raises the CanExecuteChanged event to indicate that the return value of the CanExecute method has changed. This method should be called whenever the conditions that determine whether the command can execute have changed.
    /// </summary>
    public virtual void RaiseCanExecuteChanged() => _uiScheduler.Schedule(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
}

/// <summary>
/// A command that relays its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'. The command can execute if the parameter is of type T or is null.
/// </summary>
/// <param name="execute">The action to execute.</param>
/// <param name="canExecute">A function that determines whether the command can execute.</param>
/// <param name="schedulerProvider">Optional scheduler provider used to raise CanExecuteChanged on the UI scheduler.</param>
/// <typeparam name="T">The type of the parameter.</typeparam>
public class RelayCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null, ISchedulerProvider? schedulerProvider = null) : ICommand, IRaiseCanExecuteChanged
{
    private readonly Action<T?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly IScheduler _uiScheduler = schedulerProvider?.Ui ?? CurrentThreadScheduler.Instance;

    /// <summary>
    /// Gets the function that determines whether the command can execute. If null, the command can always execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Determines whether the command can execute in its current state. If the canExecute function is null, this method returns true. The command can execute if the parameter is of type T or is null.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the canExecute function.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    public bool CanExecute(object? parameter) => parameter is T or null && (canExecute?.Invoke((T?)parameter) ?? true);

    /// <summary>
    /// Executes the command. If the command cannot execute, this method does nothing. The command can execute if the parameter is of type T or is null.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the execute action.</param>
    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        _execute((T?)parameter);
    }

    /// <summary>
    /// Raises the CanExecuteChanged event to indicate that the return value of the CanExecute method has changed. This method should be called whenever the conditions that determine whether the command can execute have changed.
    /// </summary>
    public virtual void RaiseCanExecuteChanged() => _uiScheduler.Schedule(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
}
