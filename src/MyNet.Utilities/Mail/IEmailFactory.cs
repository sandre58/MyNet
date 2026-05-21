// -----------------------------------------------------------------------
// <copyright file="IEmailFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Mail;

/// <summary>
/// Factory for creating IEmail instances. This is useful for dependency injection and allows for easier testing and flexibility in email creation.
/// </summary>
public interface IEmailFactory
{
    /// <summary>
    /// Creates a new instance of an IEmail.
    /// </summary>
    /// <returns>A new instance of an IEmail.</returns>
    IEmail Create();
}
