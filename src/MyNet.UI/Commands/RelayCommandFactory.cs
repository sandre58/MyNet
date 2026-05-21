// -----------------------------------------------------------------------
// <copyright file="RelayCommandFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MyNet.UI.Threading;

namespace MyNet.UI.Commands;

/// <summary>
/// A factory for creating RelayCommand and AsyncRelayCommand instances, optionally using a provided scheduler for command execution.
/// </summary>
/// <param name="schedulerProvider">The scheduler provider for UI thread scheduling.</param>
public class RelayCommandFactory(ISchedulerProvider? schedulerProvider = null) : ICommandFactory
{
    /// <summary>
    /// Gets a default instance of the <see cref="RelayCommandFactory"/> that uses the default scheduler provider.
    /// </summary>
    public static readonly RelayCommandFactory Default = new();

    // <inheritdoc />
    public ICommand Create(Action execute) => new RelayCommand(execute, schedulerProvider: schedulerProvider);

    // <inheritdoc />
    public ICommand Create(Action execute, Func<bool> canExecute) => new RelayCommand(execute, canExecute, schedulerProvider);

    // <inheritdoc />
    public ICommand Create<T>(Action<T?> execute) => new RelayCommand<T>(execute, schedulerProvider: schedulerProvider);

    // <inheritdoc />
    public ICommand Create<T>(Action<T?> execute, Func<T?, bool> canExecute) => new RelayCommand<T>(execute, canExecute, schedulerProvider);

    // <inheritdoc />
    public ICommand Create(Func<Task> execute) => new AsyncRelayCommand(execute, schedulerProvider: schedulerProvider);

    // <inheritdoc />
    public ICommand Create(Func<Task> execute, Func<bool> canExecute) => new AsyncRelayCommand(execute, canExecute, schedulerProvider);

    // <inheritdoc />
    public ICommand Create<T>(Func<T?, Task> execute) => new AsyncRelayCommand<T>(execute, schedulerProvider: schedulerProvider);

    // <inheritdoc />
    public ICommand Create<T>(Func<T?, Task> execute, Func<T?, bool> canExecute) => new AsyncRelayCommand<T>(execute, canExecute, schedulerProvider);
}
