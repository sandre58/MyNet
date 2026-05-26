// -----------------------------------------------------------------------
// <copyright file="MailEncodingResolver.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using MyNet.Mail.Smtp;

namespace MyNet.Mail.MailKit;

/// <summary>
/// Resolves the text encoding used for MIME bodies from SMTP options.
/// </summary>
public static class MailEncodingResolver
{
    /// <summary>
    /// Returns UTF-8 when <paramref name="preferredEncoding"/> is null or empty; otherwise attempts to resolve the encoding by name.
    /// </summary>
    public static Encoding Resolve(string? preferredEncoding)
    {
        if (string.IsNullOrWhiteSpace(preferredEncoding))
        {
            return Encoding.UTF8;
        }

        try
        {
            return Encoding.GetEncoding(preferredEncoding);
        }
        catch (ArgumentException)
        {
            return Encoding.UTF8;
        }
    }

    /// <summary>
    /// Resolves encoding from <see cref="SmtpClientOptions.PreferredEncoding"/>.
    /// </summary>
    public static Encoding Resolve(SmtpClientOptions options) =>
        Resolve(options.PreferredEncoding);
}
