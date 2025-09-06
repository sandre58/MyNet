// -----------------------------------------------------------------------
// <copyright file="WindowsUserPrincipal.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Principal;

namespace MyNet.Utilities.Authentication.Windows;

/// <summary>
/// Represents a Windows user principal with convenient name and domain properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowsUserPrincipal"/> class.
/// </remarks>
/// <param name="identity">The identity associated with the principal.</param>
/// <param name="roles">The roles for the principal.</param>
public class WindowsUserPrincipal(IIdentity identity, string[] roles) : GenericPrincipal(identity, roles)
{
    /// <summary>
    /// Gets the user name extracted from the identity.
    /// </summary>
    public string Name { get; } = identity.GetName();

    /// <summary>
    /// Gets the domain extracted from the identity.
    /// </summary>
    public string Domain { get; } = identity.GetDomain();
}
