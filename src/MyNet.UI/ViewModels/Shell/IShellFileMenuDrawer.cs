// -----------------------------------------------------------------------
// <copyright file="IShellFileMenuDrawer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// File menu drawer open state controlled by the shell host (for example <see cref="MainWindowViewModel.IsFileMenuOpen"/>).
/// </summary>
public interface IShellFileMenuDrawer
{
    /// <summary>
    /// Gets or sets a value indicating whether the file menu drawer is open.
    /// </summary>
    bool IsFileMenuOpen { get; set; }
}
