// -----------------------------------------------------------------------
// <copyright file="IShellHostProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Provides the current <see cref="IShellHost"/> registered by the application shell.
/// </summary>
public interface IShellHostProvider
{
    /// <summary>
    /// Gets the active shell host, if the shell host view model has been attached.
    /// </summary>
    IShellHost? Current { get; }

    /// <summary>
    /// Attaches the shell host view model as the active shell host.
    /// </summary>
    void Attach(IShellHost host);

    /// <summary>
    /// Detaches the host when it is disposed.
    /// </summary>
    void Detach(IShellHost host);
}
