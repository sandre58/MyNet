// -----------------------------------------------------------------------
// <copyright file="AuthenticatedEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Authentication;

/// <summary>
/// Provides data for authentication state change events.
/// </summary>
/// <param name="success">Indicates whether the authentication succeeded.</param>
public class AuthenticatedEventArgs(bool success) : EventArgs
{
    /// <summary>
    /// Gets a value indicating whether authentication was successful.
    /// </summary>
    public bool Success { get; } = success;
}
