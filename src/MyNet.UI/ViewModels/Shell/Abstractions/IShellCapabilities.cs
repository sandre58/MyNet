// -----------------------------------------------------------------------
// <copyright file="IShellCapabilities.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Optional features exposed by the active shell host.
/// </summary>
public interface IShellCapabilities
{
    /// <summary>
    /// Gets a value indicating whether a file menu is configured.
    /// </summary>
    bool HasFileMenu { get; }
}
