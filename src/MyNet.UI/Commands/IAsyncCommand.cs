// -----------------------------------------------------------------------
// <copyright file="IAsyncCommand.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNet.UI.Commands;

/// <summary>
/// Defines an interface for asynchronous commands, extending the standard ICommand interface to support asynchronous execution.
/// </summary>
public interface IAsyncCommand : ICommand
{
    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">The parameter to pass to the command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(object? parameter);
}
