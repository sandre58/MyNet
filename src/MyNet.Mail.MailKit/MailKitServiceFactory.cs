// -----------------------------------------------------------------------
// <copyright file="MailKitServiceFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using MyNet.Mail.Smtp;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Creates <see cref="MailKitService"/> instances with optional structured logging.
/// </summary>
/// <param name="loggerFactory">Optional logger factory passed to each created service.</param>
public sealed class MailKitServiceFactory(ILoggerFactory? loggerFactory = null) : IMailServiceFactory
{
    /// <inheritdoc />
    public IMailService Create(SmtpClientOptions options) =>
        new MailKitService(options, loggerFactory?.CreateLogger<MailKitService>());
}
