// -----------------------------------------------------------------------
// <copyright file="MailToHelper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Primitives.Helpers;

namespace MyNet.Mail;

/// <summary>
/// Helper class for sending emails using the default mail client on the user's system. This class provides methods to construct "mailto:" URIs with specified email addresses, subject, and body, and then opens them using the default application associated with "mailto:" URIs. Note that the functionality of this class relies on the user's system configuration and may not work if no mail client is set up or if the mail client does not support "mailto:" URIs.
/// </summary>
public static class MailToHelper
{
    /// <summary>
    /// Sends an email to a single recipient using the default mail client on the user's system. This method is a convenience overload that calls the main SendMail method with a single email address. It constructs a "mailto:" URI with the provided email address, title, and body, and then opens it using the default application associated with "mailto:" URIs. Note that this method relies on the user's system configuration and may not work if no mail client is set up or if the mail client does not support "mailto:" URIs.
    /// </summary>
    /// <param name="address">The email address of the recipient.</param>
    /// <param name="title">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <returns>True if the email was successfully sent, otherwise false.</returns>
    public static bool SendMail(string address, string title, string body) => SendMail([address], title, body);

    /// <summary>
    /// Sends an email using the default mail client on the user's system. This method constructs a "mailto:" URI with the provided email addresses, title, and body, and then opens it using the default application associated with "mailto:" URIs. Note that this method relies on the user's system configuration and may not work if no mail client is set up or if the mail client does not support "mailto:" URIs.
    /// </summary>
    /// <param name="addresses">The email addresses of the recipients.</param>
    /// <param name="title">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <returns>True if the email was successfully sent, otherwise false.</returns>
    public static bool SendMail(IEnumerable<string> addresses, string title, string body)
    {
        var recipients = addresses
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToArray();

        if (recipients.Length == 0)
        {
            return false;
        }

        var values = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(title))
        {
            values.Add("subject", title);
        }

        if (!string.IsNullOrEmpty(body))
        {
            values.Add("body", body);
        }

        var command = $"mailto:{string.Join(";", recipients)}";

        if (values.Count != 0)
        {
            command += $"?{string.Join("&", values.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"))}";
        }

        ProcessHelper.Start(command);
        return true;
    }
}
