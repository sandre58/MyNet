// -----------------------------------------------------------------------
// <copyright file="Toast.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Windows.Input;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Settings;

namespace MyNet.UI.Toasting.Models;

/// <summary>
/// Default immutable toast implementation used by the built-in toast factory.
/// </summary>
public sealed class Toast(
    INotification notification,
    ToastSettings? settings = null,
    ICommand? clickCommand = null,
    ICommand? closeCommand = null) : IToast
{
    /// <inheritdoc />
    public INotification Notification { get; } = notification;

    /// <inheritdoc />
    public ToastSettings Settings { get; } = settings ?? ToastSettings.Default;

    /// <inheritdoc />
    public bool IsVisible { get; } = true;

    /// <inheritdoc />
    public ICommand? ClickCommand { get; } = clickCommand;

    /// <inheritdoc />
    public ICommand? CloseCommand { get; } = closeCommand;
}
