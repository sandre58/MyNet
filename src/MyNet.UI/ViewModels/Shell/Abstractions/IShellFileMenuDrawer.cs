// -----------------------------------------------------------------------
// <copyright file="IShellFileMenuDrawer.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// File menu drawer open state controlled by the shell host.
/// </summary>
public interface IShellFileMenuDrawer
{
    /// <summary>
    /// Gets or sets a value indicating whether the file menu drawer is open.
    /// </summary>
    bool IsFileMenuOpen { get; set; }
}
