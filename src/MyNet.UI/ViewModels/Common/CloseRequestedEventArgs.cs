// -----------------------------------------------------------------------
// <copyright file="CloseRequestedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.Common;

/// <summary>
/// Provides data for the CloseRequested event, including whether the close request should be forced.
/// </summary>
public class CloseRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Gets a value indicating whether the close request should be forced.
    /// </summary>
    public bool Force { get; init; }
}
