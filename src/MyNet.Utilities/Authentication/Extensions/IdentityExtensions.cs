// -----------------------------------------------------------------------
// <copyright file="IdentityExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Principal;
using MyNet.Utilities.Authentication;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides convenience extension methods for reading name components from an <see cref="IIdentity"/>.
/// </summary>
public static class IdentityExtensions
{
    extension(IIdentity identity)
    {
        /// <summary>
        /// Extracts the domain part from the identity name.
        /// Returns an empty string when the identity does not contain a domain.
        /// </summary>
        public string GetDomain() => IdentityHelper.GetDomain(identity.Name);

        /// <summary>
        /// Extracts the user name part from the identity name.
        /// </summary>
        public string GetName() => IdentityHelper.GetName(identity.Name);
    }
}
