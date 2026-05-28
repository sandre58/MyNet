// -----------------------------------------------------------------------
// <copyright file="CloseRequestedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

#pragma warning disable IDE0130
namespace MyNet.UI;
#pragma warning restore IDE0130

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
