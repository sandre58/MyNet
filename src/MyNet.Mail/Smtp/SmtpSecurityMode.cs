// -----------------------------------------------------------------------
// <copyright file="SmtpSecurityMode.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Mail.Smtp;

/// <summary>
/// Describes how the SMTP transport should secure the connection.
/// Used by MailKit implementations; <see cref="SmtpClientOptions.UseSsl"/> still applies when <see cref="Auto"/> is selected.
/// </summary>
public enum SmtpSecurityMode
{
    /// <summary>
    /// Infer from <see cref="SmtpClientOptions.UseSsl"/> and <see cref="SmtpClientOptions.Port"/> (port 465 uses SSL on connect; otherwise STARTTLS when <see cref="SmtpClientOptions.UseSsl"/> is true).
    /// </summary>
    Auto = 0,

    /// <summary>No TLS (not recommended on public networks).</summary>
    None = 1,

    /// <summary>Require STARTTLS upgrade after connect (typical for port 587).</summary>
    StartTls = 2,

    /// <summary>Use STARTTLS when advertised by the server.</summary>
    StartTlsWhenAvailable = 3,

    /// <summary>Implicit TLS from the first byte (typical for port 465).</summary>
    SslOnConnect = 4
}
