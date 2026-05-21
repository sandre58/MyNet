// -----------------------------------------------------------------------
// <copyright file="IIdentityFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Fakers.Identity;

/// <summary>
/// Provides fake identity data for applications.
/// </summary>
public interface IIdentityFaker
{
    /// <summary>
    /// Generates a random username.
    /// </summary>
    /// <returns>A random username.</returns>
    string Username();

    /// <summary>
    /// Generates a random password with a specified length.
    /// </summary>
    /// <param name="length">The length of the password. Current is 12.</param>
    /// <returns>A random password.</returns>
    string Password(int length = 12);
}
