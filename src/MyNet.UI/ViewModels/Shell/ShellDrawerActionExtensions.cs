// -----------------------------------------------------------------------
// <copyright file="ShellDrawerActionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Helpers for <see cref="ShellDrawerAction"/>.
/// </summary>
public static class ShellDrawerActionExtensions
{
    /// <summary>
    /// Applies a drawer visibility action to the current open state.
    /// </summary>
    public static bool ApplyToOpenState(this ShellDrawerAction action, bool isOpen)
        => action switch
        {
            ShellDrawerAction.Show => true,
            ShellDrawerAction.Hide => false,
            ShellDrawerAction.Toggle => !isOpen,
            _ => isOpen,
        };
}
