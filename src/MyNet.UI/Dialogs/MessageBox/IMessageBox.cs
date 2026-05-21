// -----------------------------------------------------------------------
// <copyright file="IMessageBox.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Dialogs.ContentDialogs;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Interface representing a message box dialog.
/// Extends <see cref="IDialog{TResult}"/> of <see cref="MessageBoxResult"/> so that
/// the platform strategy can show and retrieve the result uniformly through
/// <see cref="ContentDialogs.IContentDialogService"/>.
/// </summary>
public interface IMessageBox : IDialog<MessageBoxResult>
{
    /// <summary>Gets the message displayed in the message box.</summary>
    string Message { get; }

    /// <summary>Gets the severity of the message (Info, Warning, Error, etc.).</summary>
    MessageSeverity Severity { get; }

    /// <summary>Gets the buttons displayed in the message box.</summary>
    MessageBoxResultOption Buttons { get; }

    /// <summary>Gets the default selected result.</summary>
    MessageBoxResult DefaultResult { get; }
}
