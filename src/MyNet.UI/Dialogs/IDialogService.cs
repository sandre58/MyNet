// -----------------------------------------------------------------------
// <copyright file="IDialogService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Dialogs.MessageBox;

namespace MyNet.UI.Dialogs;

/// <summary>
/// Unified dialog service facade.
/// Aggregates <see cref="IContentDialogService"/>, <see cref="IMessageBoxService"/>,
/// and <see cref="IFileDialogService"/> into a single injectable surface.
/// </summary>
public interface IDialogService
{
    #region Content dialogs

    /// <summary>Gets the currently opened dialogs.</summary>
    IReadOnlyList<IDialog> OpenedDialogs { get; }

    /// <summary>Gets a value indicating whether at least one dialog is currently open.</summary>
    bool HasOpenedDialogs { get; }

    /// <summary>Displays a dialog and returns a boolean outcome.</summary>
    Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Displays a typed dialog and returns a strongly-typed result.</summary>
    Task<DialogResult<TResult>> ShowAsync<TResult>(IDialog<TResult> dialog, DialogOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a fluent builder to configure and display a dialog.</summary>
    IDialogBuilder Create(IDialog dialog);

    /// <summary>Creates a fluent typed builder to configure and display a typed dialog.</summary>
    IDialogBuilder<TResult> Create<TResult>(IDialog<TResult> dialog);

    /// <summary>Programmatically closes the specified open dialog.</summary>
    Task CloseAsync(IDialog dialog);

    #endregion

    #region MessageBox

    /// <summary>Displays a message box with the given parameters.</summary>
    Task<MessageBoxResult> ShowMessageAsync(
        string message,
        string? caption = null,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        MessageBoxResult defaultResult = MessageBoxResult.Ok,
        CancellationToken cancellationToken = default);

    /// <summary>Displays a success message box.</summary>
    Task<MessageBoxResult> ShowSuccessAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default);

    /// <summary>Displays an information message box.</summary>
    Task<MessageBoxResult> ShowInformationAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default);

    /// <summary>Displays an error message box.</summary>
    Task<MessageBoxResult> ShowErrorAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default);

    /// <summary>Displays a warning message box.</summary>
    Task<MessageBoxResult> ShowWarningAsync(
        string message,
        string? title = null,
        MessageBoxResultOption buttons = MessageBoxResultOption.Ok,
        CancellationToken cancellationToken = default);

    /// <summary>Displays a Yes/No question message box.</summary>
    Task<MessageBoxResult> ShowQuestionAsync(
        string message,
        string? title = null,
        CancellationToken cancellationToken = default);

    /// <summary>Displays a Yes/No/Cancel question message box.</summary>
    Task<MessageBoxResult> ShowQuestionWithCancelAsync(
        string message,
        string? title = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region File dialogs

    /// <summary>Displays the open-file dialog. Pass <see langword="null"/> to use default settings.</summary>
    Task<FileDialogResult> ShowOpenFileDialogAsync(
        OpenFileDialogSettings? settings = null,
        CancellationToken cancellationToken = default);

    /// <summary>Displays the save-file dialog.</summary>
    Task<FileDialogResult> ShowSaveFileDialogAsync(
        SaveFileDialogSettings settings,
        CancellationToken cancellationToken = default);

    /// <summary>Displays the folder-browser dialog.</summary>
    Task<FileDialogResult> ShowFolderDialogAsync(
        OpenFolderDialogSettings settings,
        CancellationToken cancellationToken = default);

    #endregion
}
