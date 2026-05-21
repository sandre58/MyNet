// -----------------------------------------------------------------------
// <copyright file="DefaultToastFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.UI.Commands;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Models;
using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Toasting;

/// <summary>
/// Creates default toast instances from notifications.
/// </summary>
public sealed class DefaultToastFactory(ICommandFactory? commandFactory = null) : IToastFactory
{
    private readonly ICommandFactory _commandFactory = commandFactory ?? RelayCommandFactory.Default;

    /// <inheritdoc />
    public IToast Create(INotification notification)
    {
        var settings = ToastSettings.Default;

        var closeCommand = notification is ClosableNotification closable
            ? _commandFactory.Create((Action)(() => closable.RequestClose()))
            : null;

        return new Toast(notification, settings, clickCommand: null, closeCommand: closeCommand);
    }
}
