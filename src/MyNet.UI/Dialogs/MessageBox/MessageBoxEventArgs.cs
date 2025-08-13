// -----------------------------------------------------------------------
// <copyright file="MessageBoxEventArgs.cs" company="St�phane ANDRE">
// Copyright (c) St�phane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Provides data for message box events such as opening and closing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageBoxEventArgs"/> class.
/// </remarks>
/// <param name="messageBox">The message box view model associated with the event.</param>
public class MessageBoxEventArgs(IMessageBox messageBox) : EventArgs
{
    /// <summary>
    /// Gets the message box view model associated with the event.
    /// </summary>
    public IMessageBox MessageBox { get; } = messageBox;
}
