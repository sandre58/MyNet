// -----------------------------------------------------------------------
// <copyright file="SmtpClientOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Mail.Smtp;

public class SmtpClientOptions
{
    public string? Server { get; set; }

    public int Port { get; set; } = 25;

    public string? User { get; set; }

    public string? Password { get; set; }

    public bool UseSsl { get; set; }

    /// <summary>
    /// Gets or sets the explicit TLS mode for MailKit. When <see cref="SmtpSecurityMode.Auto"/>, port 465 uses SSL on connect and <see cref="UseSsl"/> enables required STARTTLS.
    /// </summary>
    public SmtpSecurityMode SecurityMode { get; set; } = SmtpSecurityMode.Auto;

    public bool RequiresAuthentication { get; set; }

    public string? PreferredEncoding { get; set; }

    public bool UsePickupDirectory { get; set; }

    public string? MailPickupDirectory { get; set; }
}
