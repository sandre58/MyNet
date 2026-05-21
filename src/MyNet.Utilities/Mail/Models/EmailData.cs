// -----------------------------------------------------------------------
// <copyright file="EmailData.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyNet.Utilities.Mail.Models;

public class EmailData(EmailAddress from)
{
    public IList<EmailAddress> To { get; } = [];

    public IList<EmailAddress> Cc { get; } = [];

    public IList<EmailAddress> Bcc { get; } = [];

    public IList<EmailAddress> ReplyTo { get; } = [];

    public IList<Attachment> Attachments { get; } = [];

    public EmailAddress From { get; set; } = from;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string PlaintextAlternativeBody { get; set; } = string.Empty;

    public Priority Priority { get; set; }

    public bool IsHtml { get; set; }

    public Dictionary<string, string> Headers { get; } = [];
}
