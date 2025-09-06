// -----------------------------------------------------------------------
// <copyright file="IAuthenticationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Security.Principal;

namespace MyNet.Utilities.Authentication;

/// <summary>
/// Defines a service that manages authentication state and exposes the current principal.
/// </summary>
/// <typeparam name="TPrincipal">The type of principal returned by the service.</typeparam>
public interface IAuthenticationService<out TPrincipal>
    where TPrincipal : IPrincipal
{
    /// <summary>
    /// Occurs when the authentication state changes.
    /// </summary>
    event EventHandler<AuthenticatedEventArgs>? Authenticated;

    /// <summary>
    /// Gets a value indicating whether the current thread has an authenticated principal.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current principal associated with the executing thread.
    /// When no principal is set, implementations typically return an anonymous principal.
    /// </summary>
    TPrincipal CurrentPrincipal { get; }

    /// <summary>
    /// Performs authentication and sets the current principal.
    /// The concrete behavior depends on the implementation.
    /// </summary>
    void Authenticate();

    /// <summary>
    /// Removes or resets the current authentication and sets an anonymous principal.
    /// </summary>
    void Unauthenticate();
}
