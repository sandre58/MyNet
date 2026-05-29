// -----------------------------------------------------------------------
// <copyright file="ShellOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130
namespace MyNet.UI.ViewModels.Shell;
#pragma warning restore IDE0130

/// <summary>
/// Shell host behavior options.
/// </summary>
public sealed class ShellOptions
{
    /// <summary>
    /// Gets the default options.
    /// </summary>
    public static ShellOptions Default { get; } = new();

    /// <summary>
    /// Gets a value indicating whether only one shell drawer may be open at a time.
    /// </summary>
    public bool MutuallyExclusiveDrawers { get; init; } = true;
}
