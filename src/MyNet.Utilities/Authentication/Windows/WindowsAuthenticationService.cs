// -----------------------------------------------------------------------
// <copyright file="WindowsAuthenticationService.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;

namespace MyNet.Utilities.Authentication.Windows;

/// <summary>
/// Windows-specific authentication service using <see cref="WindowsUserPrincipal"/>.
/// </summary>
[SupportedOSPlatform("windows")]
public class WindowsAuthenticationService : WindowsAuthenticationService<WindowsUserPrincipal>
{
    /// <summary>
    /// Shared anonymous principal used when no authenticated principal is available.
    /// </summary>
    public static readonly WindowsUserPrincipal Anonymous = new(new GenericIdentity(string.Empty), []);

    /// <inheritdoc />
    protected override WindowsUserPrincipal CreatePrincipal(IIdentity identity) => new(identity, []);

    /// <inheritdoc />
    protected override WindowsUserPrincipal GetAnonymous() => Anonymous;
}

/// <summary>
/// Base implementation of a Windows authentication service.
/// </summary>
/// <typeparam name="TPrincipal">The type of principal used by the service.</typeparam>
[SupportedOSPlatform("windows")]
public abstract class WindowsAuthenticationService<TPrincipal> : IAuthenticationService<TPrincipal>
    where TPrincipal : IPrincipal
{
    /// <inheritdoc />
    public event EventHandler<AuthenticatedEventArgs>? Authenticated;

    /// <inheritdoc />
    public bool IsAuthenticated => Thread.CurrentPrincipal?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc />
    public TPrincipal CurrentPrincipal => Thread.CurrentPrincipal is TPrincipal principal ? principal : GetAnonymous();

    /// <summary>
    /// Authenticates using the current Windows identity and sets the principal.
    /// </summary>
    public virtual void Authenticate() => Authenticate(CreatePrincipal(WindowsIdentity.GetCurrent()));

    /// <summary>
    /// Unauthenticates and resets to an anonymous principal.
    /// </summary>
    public virtual void Unauthenticate() => Authenticate(GetAnonymous());

    /// <summary>
    /// Sets the provided principal as the current thread principal and raises the <see cref="Authenticated"/> event.
    /// </summary>
    /// <param name="principal">The principal to set for the current thread.</param>
    protected virtual void Authenticate(TPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var wasAuthenticated = IsAuthenticated;

        Thread.CurrentPrincipal = principal;

        var isAuthenticated = IsAuthenticated;

        if (wasAuthenticated != isAuthenticated)
            Authenticated?.Invoke(this, new(isAuthenticated));
    }

    /// <summary>
    /// Returns an anonymous principal instance used when no authenticated principal is available.
    /// </summary>
    protected abstract TPrincipal GetAnonymous();

    /// <summary>
    /// Creates a principal instance from an identity.
    /// </summary>
    /// <param name="identity">The identity to create the principal from.</param>
    protected abstract TPrincipal CreatePrincipal(IIdentity identity);
}
