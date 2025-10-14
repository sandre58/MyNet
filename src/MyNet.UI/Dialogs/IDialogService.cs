// -----------------------------------------------------------------------
// <copyright file="IDialogService.cs" company="Stéphane ANDRE">
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
/// Provides methods and properties to display a window dialog.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Gets the collection of currently opened dialogs.
    /// </summary>
    IList<IDialogViewModel>? OpenedContentDialogs { get; }

    /// <summary>
    /// Gets a value indicating whether there are opened dialogs.
    /// </summary>
    bool HasOpenedContentDialogs { get; }

    #region Show

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    Task ShowAsync<T>(Action<T>? closeAction = null)
        where T : class, IDialogViewModel;

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    Task ShowAsync(Type typeViewModel, Action<IDialogViewModel>? closeAction = null);

    /// <summary>
    /// Displays a message dialog.
    /// </summary>
    Task ShowAsync<T>(T viewModel, Action<T>? closeAction = null)
        where T : IDialogViewModel;

    #endregion Show

    #region ShowDialog

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    Task<bool?> ShowDialogAsync<TViewModel>()
        where TViewModel : class, IDialogViewModel;

    /// <summary>
    /// Displays a modal dialog.
    /// </summary>
    /// <param name="typeViewModel">The view to include in workspace dialog.</param>
    Task<bool?> ShowDialogAsync(Type typeViewModel);

    /// <summary>
    /// Displays a message dialog.
    /// </summary>
    /// <param name="viewModel">The view to include in workspace dialog.</param>
    Task<bool?> ShowDialogAsync<T>(T viewModel)
        where T : IDialogViewModel;

    #endregion ShowDialog

    #region MessageBox

    /// <summary>
    /// Displays a success message dialog.
    /// </summary>
    Task<MessageBoxResult> ShowSuccessAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok);

    /// <summary>
    /// Displays an information message dialog.
    /// </summary>
    Task<MessageBoxResult> ShowInformationAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok);

    /// <summary>
    /// Displays an error message dialog.
    /// </summary>
    Task<MessageBoxResult> ShowErrorAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok);

    /// <summary>
    /// Displays a warning message dialog.
    /// </summary>
    Task<MessageBoxResult> ShowWarningAsync(string message, string? title = null, MessageBoxResultOption buttons = MessageBoxResultOption.Ok);

    /// <summary>
    /// Displays a question message dialog.
    /// </summary>
    Task<MessageBoxResult> ShowQuestionAsync(string message, string? title = null);

    /// <summary>
    /// Displays a question message dialog with cancel option.
    /// </summary>
    Task<MessageBoxResult> ShowQuestionWithCancelAsync(string message, string? title = null);

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="message">
    /// A <see cref="string"/> that specifies the text to display.
    /// </param>
    /// <param name="caption">
    /// A <see cref="string"/> that specifies the title bar caption to display. Default value
    /// is an empty string.
    /// </param>
    /// <param name="button">
    /// A MessageBoxButton value that specifies which button or buttons to
    /// display. Default value is MessageBoxButton.OK.
    /// </param>
    /// <param name="severity">
    /// A MessageBoxImage value that specifies the icon to display. Default value
    /// is MessageBoxImage.None.
    /// </param>
    /// <param name="defaultResult">
    /// A <see cref="MessageBoxResult"/> value that specifies the default result of the
    /// message box. Default value is <see cref="MessageBoxResult.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="MessageBoxResult"/> value that specifies which message box button is
    /// clicked by the user.
    /// </returns>
    Task<MessageBoxResult> ShowMessageAsync(
        string message,
        string? caption = "",
        MessageBoxResultOption button = MessageBoxResultOption.Ok,
        MessageSeverity severity = MessageSeverity.Information,
        MessageBoxResult defaultResult = MessageBoxResult.None);

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    /// <returns>
    /// A <see cref="MessageBoxResult"/> value that specifies which message box button is
    /// clicked by the user.
    /// </returns>
    Task<MessageBoxResult> ShowMessageBoxAsync(IMessageBox viewModel);

    #endregion MessageBox

    #region Files

    /// <summary>
    /// Displays the OpenFileDialog.
    /// </summary>
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<(bool? Result, string Filename)> ShowOpenFileDialogAsync();

    /// <summary>
    /// Displays the OpenFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the open file dialog.</param>
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<(bool? Result, string Filename)> ShowOpenFileDialogAsync(OpenFileDialogSettings settings);

    /// <summary>
    /// Displays the SaveFileDialog.
    /// </summary>
    /// <param name="settings">The settings for the save file dialog.</param>
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<(bool? Result, string Filename)> ShowSaveFileDialogAsync(SaveFileDialogSettings settings);

    /// <summary>
    /// Displays the FolderBrowserDialog.
    /// </summary>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <returns>
    /// If the user clicks the OK button of the dialog that is displayed, true is returned;
    /// otherwise false.
    /// </returns>
    Task<(bool? Result, string? SelectedPath)> ShowFolderBrowserDialogAsync(OpenFolderDialogSettings settings);

    #endregion Files
}
