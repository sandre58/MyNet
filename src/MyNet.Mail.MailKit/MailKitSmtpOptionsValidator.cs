// -----------------------------------------------------------------------
// <copyright file="MailKitSmtpOptionsValidator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Mail.MailKit.Exceptions;
using MyNet.Mail.Smtp;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Validates <see cref="SmtpClientOptions"/> before connecting or sending mail.
/// </summary>
public static class MailKitSmtpOptionsValidator
{
    /// <summary>
    /// Validates SMTP client options and throws if configuration is invalid.
    /// </summary>
    public static void Validate(SmtpClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.UsePickupDirectory)
        {
            if (string.IsNullOrWhiteSpace(options.MailPickupDirectory))
            {
                throw new ArgumentException("Pickup directory must be defined when UsePickupDirectory is enabled.");
            }

            return;
        }

        if (string.IsNullOrWhiteSpace(options.Server))
        {
            throw new UndefinedServerException();
        }

        if (options.Port is < 1 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(options), options.Port, "SMTP port must be between 1 and 65535.");
        }

        switch (options.RequiresAuthentication)
        {
            case true when string.IsNullOrWhiteSpace(options.User):
                throw new ArgumentException("SMTP user must be defined when authentication is required.");
            case true when options.Password is null:
                throw new ArgumentException("SMTP password must be defined when authentication is required.");
        }
    }
}
