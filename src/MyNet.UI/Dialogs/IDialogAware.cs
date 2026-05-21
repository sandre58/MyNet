// -----------------------------------------------------------------------
// <copyright file="IDialogAware.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MyNet.UI.Dialogs;

/// <summary>
/// Defines callbacks invoked by the dialog infrastructure when a dialog is opened or closed.
/// </summary>
public interface IDialogAware
{
    /// <summary>
    /// Called after the dialog is opened.
    /// </summary>
    Task OnOpenedAsync();

    /// <summary>
    /// Called after the dialog is closed.
    /// </summary>
    Task OnClosedAsync();
}
