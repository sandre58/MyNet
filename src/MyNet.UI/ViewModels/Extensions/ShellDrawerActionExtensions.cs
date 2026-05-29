// -----------------------------------------------------------------------
// <copyright file="ShellDrawerActionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.UI.ViewModels.Shell;

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels;
#pragma warning restore IDE0130

/// <summary>
/// Helpers for <see cref="ShellDrawerAction"/>.
/// </summary>
public static class ShellDrawerActionExtensions
{
    extension(ShellDrawerAction action)
    {
        /// <summary>
        /// Returns whether the action opens the drawer from the given state.
        /// </summary>
        public bool WillOpen(bool isOpen)
            => action switch
            {
                ShellDrawerAction.Show => true,
                ShellDrawerAction.Hide => false,
                ShellDrawerAction.Toggle => !isOpen,
                _ => false,
            };

        /// <summary>
        /// Applies a drawer visibility action to the current open state.
        /// </summary>
        public bool ApplyToOpenState(bool isOpen)
            => action switch
            {
                ShellDrawerAction.Show => true,
                ShellDrawerAction.Hide => false,
                ShellDrawerAction.Toggle => !isOpen,
                _ => isOpen,
            };
    }
}
