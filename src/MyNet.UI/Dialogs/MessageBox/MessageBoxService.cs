// -----------------------------------------------------------------------
// <copyright file="MessageBoxService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Dialogs.ContentDialogs;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Provides a service for displaying message boxes to the user.
/// Uses <see cref="IContentDialogService"/> directly to avoid a circular dependency
/// with <see cref="IDialogService"/>.
/// </summary>
/// <param name="factory">The factory used to create message box view models.</param>
/// <param name="contentDialogService">The content dialog service used to display the message box.</param>
public sealed class MessageBoxService(IMessageBoxFactory factory, IContentDialogService contentDialogService) : IMessageBoxService
{
    /// <inheritdoc />
    public Task<MessageBoxResult> ShowAsync(
        string message,
        string? title = null,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        MessageBoxResult defaultResult = MessageBoxResult.Ok,
        CancellationToken cancellationToken = default)
    {
        var options = new MessageBoxOptions
        {
            Message = message,
            Title = title,
            Severity = severity,
            Buttons = buttons,
            DefaultResult = defaultResult
        };

        return ShowAsync(options, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MessageBoxResult> ShowAsync(
        MessageBoxOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var messageBox = factory.Create(options);

        var dialogOptions = new DialogOptions
        {
            Dialog = messageBox,
            IsModal = true
        };

        var result = await contentDialogService
            .ShowAsync<MessageBoxResult>(messageBox, dialogOptions, cancellationToken)
            .ConfigureAwait(false);

        return MapResult(result, options.DefaultResult);
    }

    /// <inheritdoc />
    public IMessageBoxBuilder Create() => new MessageBoxBuilder(this);

    private static MessageBoxResult MapResult(DialogResult<MessageBoxResult> result, MessageBoxResult defaultResult)
        => result.Outcome switch
        {
            DialogOutcome.Success when result.Value is MessageBoxResult value => value,
            DialogOutcome.Cancelled => MessageBoxResult.Cancel,
            _ => defaultResult
        };
}
