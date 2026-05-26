// -----------------------------------------------------------------------
// <copyright file="Attachment.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;

namespace MyNet.Mail.Models;

public class Attachment
{
    /// <summary>
    /// Gets or sets a value indicating whether gets or sets whether the attachment is intended to be used for inline images (changes the paramater name for providers such as MailGun).
    /// </summary>
    public bool IsInline { get; set; }

    public string Filename { get; set; } = string.Empty;

    public Stream? Data { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public string ContentId { get; set; } = string.Empty;
}
