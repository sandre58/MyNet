// -----------------------------------------------------------------------
// <copyright file="IMessageBoxBuilder.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Defines a builder interface for constructing and displaying message boxes with customizable options such as message, title, severity, buttons, and default result.
/// </summary>
public interface IMessageBoxBuilder
{
    /// <summary>
    /// Sets the message to be displayed in the message box.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <returns>The current instance of <see cref="IMessageBoxBuilder"/>.</returns>
    IMessageBoxBuilder WithMessage(string message);

    /// <summary>
    /// Sets the title of the message box.
    /// </summary>
    /// <param name="title">The title to display.</param>
    /// <returns>The current instance of <see cref="IMessageBoxBuilder"/>.</returns>
    IMessageBoxBuilder WithTitle(string title);

    /// <summary>
    /// Sets the severity of the message box.
    /// </summary>
    /// <param name="severity">The severity to display.</param>
    /// <returns>The current instance of <see cref="IMessageBoxBuilder"/>.</returns>
    IMessageBoxBuilder WithSeverity(MessageSeverity severity);

    /// <summary>
    /// Sets the buttons to be displayed in the message box.
    /// </summary>
    /// <param name="buttons">The buttons to display.</param>
    /// <returns>The current instance of <see cref="IMessageBoxBuilder"/>.</returns>
    IMessageBoxBuilder WithButtons(MessageBoxResultOption buttons);

    /// <summary>
    /// Sets the default result of the message box.
    /// </summary>
    /// <param name="result">The default result to display.</param>
    /// <returns>The current instance of <see cref="IMessageBoxBuilder"/>.</returns>
    IMessageBoxBuilder WithDefaultResult(MessageBoxResult result);

    /// <summary>
    /// Displays the message box asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user's response.</returns>
    Task<MessageBoxResult> ShowAsync(CancellationToken cancellationToken = default);
}
