// -----------------------------------------------------------------------
// <copyright file="CommandFactoryExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

#pragma warning disable IDE0130
namespace MyNet.UI.Commands;
#pragma warning restore IDE0130

/// <summary>
/// Provides convenience helpers around <see cref="ICommandFactory"/>.
/// </summary>
public static class CommandFactoryExtensions
{
    extension(ICommandFactory? factory)
    {
        /// <summary>
        /// Returns the factory instance or <see cref="RelayCommandFactory.Default"/> when null.
        /// </summary>
        [SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Method is more discoverable as an extension method.")]
        public ICommandFactory GetOrDefault() => factory ?? RelayCommandFactory.Default;
    }

    extension(ICommandFactory factory)
    {
        /// <summary>
        /// Creates a command that requires a non-null parameter of type T. If the parameter is null, the command will not execute.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <returns>A command that executes the action if the parameter is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the factory or execute action is null.</exception>
        public ICommand CreateRequired<T>(Action<T> execute)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(execute);

            return factory.Create<T>(x =>
            {
                if (x is null)
                    return;

                execute.Invoke(x);
            });
        }

        /// <summary>
        /// Creates a command that requires a non-null parameter of type T and a canExecute function. If the parameter is null, the command will not execute and canExecute will return false.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function that determines whether the command can execute.</param>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <returns>A command that executes the action if the parameter is not null and canExecute returns true.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the factory, execute action, or canExecute function is null.</exception>
        public ICommand CreateRequired<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(canExecute);

            return factory.Create<T>(x =>
                {
                    if (x is null)
                        return;

                    execute.Invoke(x);
                },
                x => x is not null && canExecute.Invoke(x));
        }

        /// <summary>
        /// Creates an asynchronous command that requires a non-null parameter of type T. If the parameter is null, the command will not execute.
        /// </summary>
        /// <param name="execute">The asynchronous action to execute.</param>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <returns>A command that executes the asynchronous action if the parameter is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the factory or execute action is null.</exception>
        public ICommand CreateRequired<T>(Func<T, Task> execute)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(execute);

            return factory.Create<T>(
                x => x is null ? Task.CompletedTask : execute(x));
        }

        /// <summary>
        /// Creates an asynchronous command that requires a non-null parameter of type T and a canExecute function. If the parameter is null, the command will not execute and canExecute will return false.
        /// </summary>
        /// <param name="execute">The asynchronous action to execute.</param>
        /// <param name="canExecute">The function that determines whether the command can execute.</param>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <returns>A command that executes the asynchronous action if the parameter is not null and canExecute returns true.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the factory, execute action, or canExecute function is null.</exception>
        public ICommand CreateRequired<T>(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(factory);
            ArgumentNullException.ThrowIfNull(execute);
            ArgumentNullException.ThrowIfNull(canExecute);

            return factory.Create<T>(
                x => x is null ? Task.CompletedTask : execute(x),
                x => x is not null && canExecute.Invoke(x));
        }
    }
}
