// -----------------------------------------------------------------------
// <copyright file="IShellHostProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Shell;

/// <summary>
/// Provides the current <see cref="IShellHost"/> registered by the application shell.
/// </summary>
public interface IShellHostProvider
{
    /// <summary>
    /// Gets the active shell host, if the main window view model has been attached.
    /// </summary>
    IShellHost? Current { get; }

    /// <summary>
    /// Attaches the main window as the active shell host.
    /// </summary>
    void Attach(IShellHost host);

    /// <summary>
    /// Detaches the host when it is disposed.
    /// </summary>
    void Detach(IShellHost host);
}
