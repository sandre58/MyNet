// -----------------------------------------------------------------------
// <copyright file="IToastManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using MyNet.UI.Toasting.Models;

namespace MyNet.UI.Toasting;

/// <summary>
/// Defines the contract for a manager that handles toasts and their lifecycle.
/// </summary>
public interface IToastManager : IDisposable
{
    /// <summary>
    /// Gets the collection of toasts managed by this manager.
    /// </summary>
    ReadOnlyObservableCollection<IToast> Toasts { get; }

    /// <summary>
    /// Clears all toasts managed by this manager.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes a specific toast from the manager.
    /// </summary>
    /// <param name="toast">The toast to remove.</param>
    void Remove(IToast toast);
}
