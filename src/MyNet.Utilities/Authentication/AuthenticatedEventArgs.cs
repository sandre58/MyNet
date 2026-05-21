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
/// <param name="isAuthenticated">Indicates whether the new state is authenticated.</param>
public sealed class AuthenticatedEventArgs(bool isAuthenticated) : EventArgs
{
    /// <summary>
    /// Gets a value indicating whether the new authentication state is authenticated.
    /// </summary>
    public bool IsAuthenticated { get; } = isAuthenticated;
}
