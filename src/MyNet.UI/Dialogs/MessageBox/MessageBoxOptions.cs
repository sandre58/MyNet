// -----------------------------------------------------------------------
// <copyright file="MessageBoxOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Options for configuring the message box dialog.
/// </summary>
public sealed class MessageBoxOptions
{
    /// <summary>
    /// Gets the message displayed in the message box.
    /// </summary>
    public string Message { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the title of the message box.
    /// </summary>
    public string? Title { get; internal set; }

    /// <summary>
    /// Gets the severity of the message (Info, Warning, Error, etc.).
    /// </summary>
    public MessageSeverity Severity { get; internal set; } = MessageSeverity.Information;

    /// <summary>
    /// Gets the buttons displayed in the message box.
    /// </summary>
    public MessageBoxResultOption Buttons { get; internal set; } = MessageBoxResultOption.Ok;

    /// <summary>
    /// Gets the default selected result.
    /// </summary>
    public MessageBoxResult DefaultResult { get; internal set; } = MessageBoxResult.Ok;

    /// <summary>
    /// Creates a configured <see cref="MessageBoxOptions"/> instance for use with <see cref="IMessageBoxFactory"/>.
    /// </summary>
    public static MessageBoxOptions Create(
        string message,
        string? title = null,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        MessageBoxResult defaultResult = MessageBoxResult.Ok)
        => new()
        {
            Message = message,
            Title = title,
            Severity = severity,
            Buttons = buttons,
            DefaultResult = defaultResult
        };
}
