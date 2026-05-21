// -----------------------------------------------------------------------
// <copyright file="ScreenMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.ViewModels.Workspace;

/// <summary>
/// Defines the mode of a screen, which can be used to determine the behavior and appearance of the screen based on its context, such as whether it is being created, edited, or viewed in read-only mode.
/// </summary>
public enum ScreenMode
{
    /// <summary>
    /// Default.
    /// </summary>
    Unknown,

    /// <summary>
    /// Mode Creation.
    /// </summary>
    Creation,

    /// <summary>
    /// Mode Edition.
    /// </summary>
    Edition,

    /// <summary>
    /// Mode Read Only.
    /// </summary>
    Read
}
