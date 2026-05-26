// -----------------------------------------------------------------------
// <copyright file="SmtpSecureSocketOptionsResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MailKit.Security;
using MyNet.Mail.Smtp;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Maps <see cref="SmtpClientOptions"/> to MailKit <see cref="SecureSocketOptions"/>.
/// </summary>
public static class SmtpSecureSocketOptionsResolver
{
    /// <summary>
    /// Resolves the secure socket options for the given SMTP options.
    /// </summary>
    public static SecureSocketOptions Resolve(SmtpClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return options.SecurityMode != SmtpSecurityMode.Auto
            ? Map(options.SecurityMode)
            : options.Port == 465
            ? SecureSocketOptions.SslOnConnect
            : options.UseSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;
    }

    private static SecureSocketOptions Map(SmtpSecurityMode mode) =>
        mode switch
        {
            SmtpSecurityMode.None => SecureSocketOptions.None,
            SmtpSecurityMode.StartTls => SecureSocketOptions.StartTls,
            SmtpSecurityMode.StartTlsWhenAvailable => SecureSocketOptions.StartTlsWhenAvailable,
            SmtpSecurityMode.SslOnConnect => SecureSocketOptions.SslOnConnect,
            _ => SecureSocketOptions.Auto
        };
}
