// -----------------------------------------------------------------------
// <copyright file="IMessageBoxService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Service for displaying message boxes.
/// </summary>
public interface IMessageBoxService
{
    /// <summary>
    /// Displays a message box with the specified options.
    /// </summary>
    /// <param name="options">The options for the message box.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A <see cref="MessageBoxResult"/> value that specifies which message box button is clicked by the user.</returns>
    Task<MessageBoxResult> ShowAsync(MessageBoxOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Displays a message box with the specified parameters.
    /// </summary>
    /// <param name="message">The message to display in the message box.</param>
    /// <param name="title">The title of the message box.</param>
    /// <param name="severity">The severity of the message.</param>
    /// <param name="buttons">The buttons to display in the message box.</param>
    /// <param name="defaultResult">The default result of the message box.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A <see cref="MessageBoxResult"/> value that specifies which message box button is clicked by the user.</returns>
    Task<MessageBoxResult> ShowAsync(
        string message,
        string? title = null,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        MessageBoxResult defaultResult = MessageBoxResult.Ok,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new instance of the <see cref="IMessageBoxBuilder"/> interface for building a message box.
    /// </summary>
    /// <returns>A new instance of the <see cref="IMessageBoxBuilder"/> interface.</returns>
    IMessageBoxBuilder Create();
}
