// -----------------------------------------------------------------------
// <copyright file="DialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Dialogs.MessageBox;

namespace MyNet.UI.Dialogs;

/// <summary>
/// Default implementation of <see cref="IDialogService"/>.
/// Delegates to <see cref="IContentDialogService"/>, <see cref="IMessageBoxService"/>,
/// and <see cref="IFileDialogService"/>.
/// </summary>
public class DialogService(
    IContentDialogService contentDialogService,
    IMessageBoxService messageBoxService,
    IFileDialogService fileDialogService) : IDialogService
{
    private readonly IContentDialogService _contentDialogService = contentDialogService ?? throw new ArgumentNullException(nameof(contentDialogService));
    private readonly IMessageBoxService _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
    private readonly IFileDialogService _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));

    #region Content dialogs

    /// <inheritdoc />
    public IReadOnlyList<IDialog> OpenedDialogs => _contentDialogService.OpenedDialogs;

    /// <inheritdoc />
    public bool HasOpenedDialogs => _contentDialogService.HasOpenedDialogs;

    /// <inheritdoc />
    public Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions? options = null, CancellationToken cancellationToken = default)
        => _contentDialogService.ShowAsync(dialog, options, cancellationToken);

    /// <inheritdoc />
    public Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, DialogOptions? options = null, CancellationToken cancellationToken = default)
        => _contentDialogService.ShowAsync(dialog, options, cancellationToken);

    /// <inheritdoc />
    public IDialogBuilder Create(IDialog dialog) => _contentDialogService.Create(dialog);

    /// <inheritdoc />
    public IDialogBuilder<TResult> Create<TResult>(IDialog<TResult> dialog) => _contentDialogService.Create(dialog);

    /// <inheritdoc />
    public Task CloseAsync(IDialog dialog) => _contentDialogService.CloseAsync(dialog);

    #endregion

    #region MessageBox

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowMessageAsync(
        string message,
        string? caption = null,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        MessageBoxResult defaultResult = MessageBoxResult.Ok,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(message, caption, severity, buttons, defaultResult, cancellationToken);

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowSuccessAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(message, title, MessageSeverity.Success, buttons, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowInformationAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(message, title, MessageSeverity.Information, buttons, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowErrorAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(message, title, MessageSeverity.Error, buttons, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowWarningAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(message, title, MessageSeverity.Warning, buttons, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowQuestionAsync(
        string message,
        string? title = null,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(
            message,
            title,
            MessageSeverity.Question,
            MessageBoxResultOption.YesNo,
            MessageBoxResult.Yes,
            cancellationToken);

    /// <inheritdoc />
    public Task<MessageBoxResult> ShowQuestionWithCancelAsync(
        string message,
        string? title = null,
        CancellationToken cancellationToken = default)
        => _messageBoxService.ShowAsync(
            message,
            title,
            MessageSeverity.Question,
            MessageBoxResultOption.YesNoCancel,
            MessageBoxResult.Yes,
            cancellationToken);

    #endregion

    #region File dialogs

    /// <inheritdoc />
    public Task<FileDialogResult> ShowOpenFileDialogAsync(
        OpenFileDialogSettings? settings = null,
        CancellationToken cancellationToken = default)
        => _fileDialogService.ShowOpenFileDialogAsync(settings ?? new OpenFileDialogSettings(), cancellationToken);

    /// <inheritdoc />
    public Task<FileDialogResult> ShowSaveFileDialogAsync(
        SaveFileDialogSettings settings,
        CancellationToken cancellationToken = default)
        => _fileDialogService.ShowSaveFileDialogAsync(settings, cancellationToken);

    /// <inheritdoc />
    public Task<FileDialogResult> ShowFolderDialogAsync(
        OpenFolderDialogSettings settings,
        CancellationToken cancellationToken = default)
        => _fileDialogService.ShowFolderDialogAsync(settings, cancellationToken);

    #endregion
}
