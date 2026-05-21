// -----------------------------------------------------------------------
// <copyright file="IToastFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Models;

namespace MyNet.UI.Toasting;

/// <summary>
/// Defines the contract for a factory that creates toast notifications based on a given notification model.
/// </summary>
public interface IToastFactory
{
    /// <summary>
    /// Creates a toast notification based on the specified notification model.
    /// </summary>
    /// <param name="notification">The notification model to create the toast from.</param>
    /// <returns>A toast notification instance.</returns>
    IToast Create(INotification notification);
}
