// -----------------------------------------------------------------------
// <copyright file="ShellHostProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.Shell;

/// <inheritdoc />
public sealed class ShellHostProvider : IShellHostProvider
{
    /// <inheritdoc />
    public IShellHost? Current { get; private set; }

    /// <inheritdoc />
    public void Attach(IShellHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        Current = host;
    }

    /// <inheritdoc />
    public void Detach(IShellHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        if (ReferenceEquals(Current, host))
            Current = null;
    }
}
