// -----------------------------------------------------------------------
// <copyright file="ContentDialogLifecycleEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Event data for content dialog lifecycle notifications.
/// </summary>
public sealed class ContentDialogLifecycleEventArgs(IDialog dialog) : EventArgs
{
    /// <summary>
    /// Gets the dialog involved in the lifecycle transition.
    /// </summary>
    public IDialog Dialog { get; } = dialog ?? throw new ArgumentNullException(nameof(dialog));
}
