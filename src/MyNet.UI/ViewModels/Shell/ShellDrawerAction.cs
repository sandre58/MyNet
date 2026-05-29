// -----------------------------------------------------------------------
// <copyright file="ShellDrawerAction.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Action to apply to a shell drawer or its content.
/// </summary>
public enum ShellDrawerAction
{
    /// <summary>
    /// Shows the drawer or content.
    /// </summary>
    Show,

    /// <summary>
    /// Hides the drawer or content.
    /// </summary>
    Hide,

    /// <summary>
    /// Toggles the drawer or content.
    /// </summary>
    Toggle,
}
