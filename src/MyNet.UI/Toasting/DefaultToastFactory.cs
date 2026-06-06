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
public sealed class DefaultToastFactory : IToastFactory
{
    private readonly ToastManagerOptions _options;
    private readonly ICommandFactory _commandFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultToastFactory"/> class.
    /// </summary>
    /// <param name="options">Global toast defaults used when resolving settings.</param>
    /// <param name="commandFactory">Optional command factory for toast interaction commands.</param>
    public DefaultToastFactory(ToastManagerOptions? options = null, ICommandFactory? commandFactory = null)
    {
        _options = options ?? new ToastManagerOptions();
        _commandFactory = commandFactory.GetOrDefault();
    }

    /// <inheritdoc />
    public IToast Create(INotification notification)
    {
        var settings = ToastSettingsMerger.Merge(notification, _options);

        var closeCommand = notification is IClosableNotification { IsClosable: true } closable
            ? _commandFactory.Create((Action)(() => closable.RequestClose()))
            : null;

        var clickCommand = notification is ActionNotification { Action: not null } actionNotification
            ? _commandFactory.Create(() => actionNotification.Action(actionNotification))
            : null;

        return new Toast(notification, settings, clickCommand: clickCommand, closeCommand: closeCommand);
    }
}
