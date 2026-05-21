// -----------------------------------------------------------------------
// <copyright file="IMailServiceFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities.Mail.Smtp;

namespace MyNet.Utilities.Mail;

/// <summary>
/// Factory for creating instances of <see cref="IMailService"/>. This allows for different implementations of the mail service to be used, such as a mock service for testing or a real service for production.
/// </summary>
public interface IMailServiceFactory
{
    /// <summary>
    /// Creates an instance of <see cref="IMailService"/> using the provided SMTP client options. This method allows for the configuration of the mail service based on specific SMTP settings, such as server address, port, credentials, and security options.
    /// </summary>
    /// <param name="options">The SMTP client options to use for configuring the mail service.</param>
    /// <returns>An instance of <see cref="IMailService"/> configured with the provided options.</returns>
    IMailService Create(SmtpClientOptions options);
}
