// -----------------------------------------------------------------------
// <copyright file="ICommandFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNet.UI.Commands;

/// <summary>
/// Defines a factory interface for creating ICommand instances with various execution and can-execute logic.
/// </summary>
public interface ICommandFactory
{
    /// <summary>
    /// Creates an ICommand that executes the specified action.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create(Action execute);

    /// <summary>
    /// Creates an ICommand that executes the specified action and determines whether the command can execute.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">A function that determines whether the command can execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create(Action execute, Func<bool> canExecute);

    /// <summary>
    /// Creates an ICommand that executes the specified action with a parameter.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create<T>(Action<T?> execute);

    /// <summary>
    /// Creates an ICommand that executes the specified action with a parameter and determines whether the command can execute.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">A function that determines whether the command can execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create<T>(Action<T?> execute, Func<T?, bool> canExecute);

    /// <summary>
    /// Creates an ICommand that executes the specified asynchronous action.
    /// </summary>
    /// <param name="execute">The asynchronous action to execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create(Func<Task> execute);

    /// <summary>
    /// Creates an ICommand that executes the specified asynchronous action and determines whether the command can execute.
    /// </summary>
    /// <param name="execute">The asynchronous action to execute.</param>
    /// <param name="canExecute">A function that determines whether the command can execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create(Func<Task> execute, Func<bool> canExecute);

    /// <summary>
    /// Creates an ICommand that executes the specified asynchronous action with a parameter.
    /// </summary>
    /// <param name="execute">The asynchronous action to execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create<T>(Func<T?, Task> execute);

    /// <summary>
    /// Creates an ICommand that executes the specified asynchronous action with a parameter and determines whether the command can execute.
    /// </summary>
    /// <param name="execute">The asynchronous action to execute.</param>
    /// <param name="canExecute">A function that determines whether the command can execute.</param>
    /// <returns>An ICommand instance.</returns>
    ICommand Create<T>(Func<T?, Task> execute, Func<T?, bool> canExecute);
}
