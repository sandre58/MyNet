// -----------------------------------------------------------------------
// <copyright file="IMessageBoxFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Defines the contract for a factory that creates message box view models.
/// </summary>
public interface IMessageBoxFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="IMessageBox"/> interface based on the specified options.
    /// </summary>
    /// <param name="options">The options to configure the message box.</param>
    /// <returns>A new <see cref="IMessageBox"/> instance.</returns>
    IMessageBox Create(MessageBoxOptions options);
}
