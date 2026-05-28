// -----------------------------------------------------------------------
// <copyright file="DefaultMessageBoxFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Commands;

namespace MyNet.UI.Dialogs.MessageBox;

/// <summary>
/// Default factory that creates <see cref="MessageBoxViewModel"/> instances.
/// </summary>
/// <param name="commandFactory">Optional command factory passed to each message box instance.</param>
public sealed class DefaultMessageBoxFactory(ICommandFactory? commandFactory = null) : IMessageBoxFactory
{
    /// <inheritdoc />
    public IMessageBox Create(MessageBoxOptions options) => new MessageBoxViewModel(options, commandFactory);
}
