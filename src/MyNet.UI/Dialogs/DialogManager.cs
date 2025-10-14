// -----------------------------------------------------------------------
// <copyright file="DialogManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Dialogs.MessageBox;

namespace MyNet.UI.Dialogs;

/// <summary>
/// Static facade for accessing the default dialog manager instance.
/// Provides backward compatibility with legacy code.
/// </summary>
public static class DialogManager
{
    private static IDialogService? _defaultInstance;

    /// <summary>
    /// Gets the default dialog manager instance.
    /// </summary>
    public static IDialogService Default => _defaultInstance ?? throw new InvalidOperationException("DialogService has not been initialized. Call Initialize() first.");

    /// <summary>
    /// Gets a value indicating whether the default instance has been set.
    /// </summary>
    public static bool IsInitialized => _defaultInstance is not null;

    /// <summary>
    /// Initializes the <see cref="DialogManager"/> with the specified <see cref="IDialogService"/>.
    /// </summary>
    public static void Initialize(IDialogService dialogService) => _defaultInstance = dialogService;

    /// <summary>
    /// Gets the collection of currently opened dialogs.
    /// </summary>
    public static IList<IDialogViewModel>? OpenedDialogs => Default.OpenedContentDialogs;

    /// <summary>
    /// Gets a value indicating whether there are opened dialogs.
    /// </summary>
    public static bool HasOpenedDialogs => Default.HasOpenedContentDialogs;

    #region Show

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    public static async Task ShowAsync<T>(Action<T>? closeAction = null)
        where T : class, IDialogViewModel
        => await Default.ShowAsync(closeAction).ConfigureAwait(false);

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    public static async Task ShowAsync(Type typeViewModel, Action<IDialogViewModel>? closeAction = null)
        => await Default.ShowAsync(typeViewModel, closeAction).ConfigureAwait(false);

    /// <summary>
    /// Displays a message dialog.
    /// </summary>
    public static async Task ShowAsync<T>(T viewModel, Action<T>? closeAction = null)
        where T : IDialogViewModel
        => await Default.ShowAsync(viewModel, closeAction).ConfigureAwait(false);

    #endregion Show

    #region ShowDialog

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    public static async Task<bool?> ShowDialogAsync<TViewModel>()
        where TViewModel : class, IDialogViewModel
        => await Default.ShowDialogAsync<TViewModel>().ConfigureAwait(false);

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    /// <param name="typeViewModel">The view to include in workspace dialog.</param>
    public static async Task<bool?> ShowDialogAsync(Type typeViewModel)
        => await Default.ShowDialogAsync(typeViewModel).ConfigureAwait(false);

    /// <summary>
    /// Displays a message dialog.
    /// </summary>
    /// <param name="viewModel">The view to include in workspace dialog.</param>
    public static async Task<bool?> ShowDialogAsync<T>(T viewModel)
        where T : IDialogViewModel
        => await Default.ShowDialogAsync(viewModel).ConfigureAwait(false);

    #endregion ShowDialog

    #region MessageBox

    /// <summary>
    /// Displays a success message dialog.
    /// </summary>
    public static async Task<MessageBoxResult> ShowSuccessAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok)
        => await Default.ShowSuccessAsync(message, title, buttons).ConfigureAwait(false);

    /// <summary>
    /// Displays an information message dialog.
    /// </summary>
    public static async Task<MessageBoxResult> ShowInformationAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok)
        => await Default.ShowInformationAsync(message, title, buttons).ConfigureAwait(false);

    /// <summary>
    /// Displays an error message dialog.
    /// </summary>
    public static async Task<MessageBoxResult> ShowErrorAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok)
        => await Default.ShowErrorAsync(message, title, buttons).ConfigureAwait(false);

    /// <summary>
    /// Displays a warning message dialog.
    /// </summary>
    public static async Task<MessageBoxResult> ShowWarningAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok)
        => await Default.ShowWarningAsync(message, title, buttons).ConfigureAwait(false);

    /// <summary>
    /// Displays a question message dialog.
    /// </summary>
    public static async Task<MessageBoxResult> ShowQuestionAsync(string message, string? title = null)
        => await Default.ShowQuestionAsync(message, title).ConfigureAwait(false);

    /// <summary>
    /// Displays a question message dialog with cancel option.
    /// </summary>
    public static async Task<MessageBoxResult> ShowQuestionWithCancelAsync(string message, string? title = null)
        => await Default.ShowQuestionWithCancelAsync(message, title).ConfigureAwait(false);

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    public static async Task<MessageBoxResult> ShowMessageAsync(
        string message,
        string? caption = "",
        MessageBoxResultOption button = MessageBoxResultOption.Ok,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResult defaultResult = MessageBoxResult.None)
        => await Default.ShowMessageAsync(message, caption, button, severity, defaultResult).ConfigureAwait(false);

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    public static async Task<MessageBoxResult> ShowMessageBoxAsync(IMessageBox viewModel)
        => await Default.ShowMessageBoxAsync(viewModel).ConfigureAwait(false);

    #endregion MessageBox

    #region Files

    /// <summary>
    /// Displays the OpenFileDialog.
    /// </summary>
    public static async Task<(bool? Result, string Filename)> ShowOpenFileDialogAsync()
        => await Default.ShowOpenFileDialogAsync().ConfigureAwait(false);

    /// <summary>
    /// Displays the OpenFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the open file dialog.</param>
    public static async Task<(bool? Result, string Filename)> ShowOpenFileDialogAsync(OpenFileDialogSettings settings)
        => await Default.ShowOpenFileDialogAsync(settings).ConfigureAwait(false);

    /// <summary>
    /// Displays the SaveFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the save file dialog.</param>
    public static async Task<(bool? Result, string Filename)> ShowSaveFileDialogAsync(SaveFileDialogSettings settings)
        => await Default.ShowSaveFileDialogAsync(settings).ConfigureAwait(false);

    /// <summary>
    /// Displays the FolderBrowserDialog.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    public static async Task<(bool? Result, string? SelectedPath)> ShowFolderBrowserDialogAsync(OpenFolderDialogSettings settings)
        => await Default.ShowFolderBrowserDialogAsync(settings).ConfigureAwait(false);

    #endregion Files
}
