// -----------------------------------------------------------------------
// <copyright file="IRaiseCanExecuteChanged.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Commands;

/// <summary>
/// Defines an interface for objects that can raise the CanExecuteChanged event, allowing them to notify the UI when the ability to execute a command has changed.
/// </summary>
public interface IRaiseCanExecuteChanged
{
    /// <summary>
    /// Raises the CanExecuteChanged event.
    /// </summary>
    void RaiseCanExecuteChanged();
}
