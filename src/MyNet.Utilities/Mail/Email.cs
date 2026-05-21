// -----------------------------------------------------------------------
// <copyright file="Email.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyNet.Utilities.Mail.Models;

namespace MyNet.Utilities.Mail;

public class Email(string fromAddress, string displayNameFrom = "") : IEmail
{
    public EmailData Data { get; } = new(new(fromAddress, displayNameFrom));

    /// <summary>
    /// Creates a new Email instance and sets the from property.
    /// </summary>
    /// <param name="emailAddress">Email address to send from.</param>
    /// <param name="name">Name to send from.</param>
    /// <returns>Instance of the Email class.</returns>
    public static IEmail From(string emailAddress, string name = "") => new Email(emailAddress, name);

    /// <summary>
    /// Set the send from email address.
    /// </summary>
    /// <param name="emailAddress">Email address of sender.</param>
    /// <param name="name">Name of sender.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail SetFrom(string emailAddress, string name = "")
    {
        Data.From = new(emailAddress, name);
        return this;
    }

    /// <summary>
    /// Adds a recipients to the email, Splits name and address on ';'.
    /// </summary>
    /// <param name="emailAddress">Email address of recipients.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail To(string emailAddress, string name = "") => AddAddresses(Data.To, emailAddress, name);

    /// <summary>
    /// Adds all recipients in list to email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail To(IList<EmailAddress> mailAddresses) => AddAddresses(Data.To, mailAddresses);

    /// <summary>
    /// Adds a Carbon Copy to the email.
    /// </summary>
    /// <param name="emailAddress">Email address of recipients.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Cc(string emailAddress, string name = "") => AddAddresses(Data.Cc, emailAddress, name);

    /// <summary>
    /// Adds a Carbon Copy to the email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Cc(IList<EmailAddress> mailAddresses) => AddAddresses(Data.Cc, mailAddresses);

    /// <summary>
    /// Adds a blind carbon copy to the email.
    /// </summary>
    /// <param name="emailAddress">Email address of recipient.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Bcc(string emailAddress, string name = "") => AddAddresses(Data.Bcc, emailAddress, name);

    /// <summary>
    /// Adds a blind carbon copy to the email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Bcc(IList<EmailAddress> mailAddresses) => AddAddresses(Data.Bcc, mailAddresses);

    /// <summary>
    /// Sets the ReplyTo address on the email.
    /// </summary>
    /// <param name="emailAddress">Email address of recipient.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail ReplyTo(string emailAddress, string name = "") => AddAddresses(Data.ReplyTo, emailAddress, name);

    /// <summary>
    /// Sets the ReplyTo address on the email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail ReplyTo(IList<EmailAddress> mailAddresses) => AddAddresses(Data.ReplyTo, mailAddresses);

    /// <summary>
    /// Sets the subject of the email.
    /// </summary>
    /// <param name="subject">email subject.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Subject(string subject)
    {
        Data.Subject = subject;
        return this;
    }

    /// <summary>
    /// Adds a Body to the Email.
    /// </summary>
    /// <param name="body">The content of the body.</param>
    /// <param name="isHtml">True if Body is HTML, false for plain text (default).</param>
    public IEmail Body(string body, bool isHtml = false)
    {
        Data.IsHtml = isHtml;
        Data.Body = body;
        return this;
    }

    /// <summary>
    /// Adds a Plaintext alternative Body to the Email. Used in conjunction with an HTML email,
    /// this allows for email readers without html capability, and also helps avoid spam filters.
    /// </summary>
    /// <param name="body">The content of the body.</param>
    public IEmail PlaintextAlternativeBody(string body)
    {
        Data.PlaintextAlternativeBody = body;
        return this;
    }

    /// <summary>
    /// Marks the email as High Priority.
    /// </summary>
    public IEmail HighPriority()
    {
        Data.Priority = Priority.High;
        return this;
    }

    /// <summary>
    /// Marks the email as Low Priority.
    /// </summary>
    public IEmail LowPriority()
    {
        Data.Priority = Priority.Low;
        return this;
    }

    /// <summary>
    /// Adds an Attachment to the Email.
    /// </summary>
    /// <param name="attachment">The Attachment to add.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Attach(Attachment attachment)
    {
        if (!Data.Attachments.Contains(attachment))
            Data.Attachments.Add(attachment);

        return this;
    }

    /// <summary>
    /// Adds Multiple Attachments to the Email.
    /// </summary>
    /// <param name="attachments">The List of Attachments to add.</param>
    /// <returns>Instance of the Email class.</returns>
    public IEmail Attach(IList<Attachment> attachments)
    {
        foreach (var attachment in attachments.Where(attachment => !Data.Attachments.Contains(attachment)))
            Data.Attachments.Add(attachment);

        return this;
    }

    public IEmail AttachFromFilename(string filename, string contentType = "", string attachmentName = "")
    {
        if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename must be provided.", nameof(filename));

        var stream = File.OpenRead(filename);
        _ = Attach(new Attachment
        {
            Data = stream,
            Filename = !string.IsNullOrWhiteSpace(attachmentName) ? attachmentName : Path.GetFileName(filename),
            ContentType = contentType
        });

        return this;
    }

    public IEmail Header(string header, string body)
    {
        if (!string.IsNullOrWhiteSpace(header))
            Data.Headers[header] = body;

        return this;
    }

    public override string ToString() => $"From {Data.From.Address} To {string.Join(", ", Data.To.Select(x => x.Address))} - Subject : {Data.Subject}";

    /// <summary>
    /// Adds a recipient to the email, Splits name and address on ';'.
    /// </summary>
    /// <returns>Instance of the Email class.</returns>
    private Email AddAddresses(IList<EmailAddress> destination, string emailAddress, string name)
    {
        if (string.IsNullOrWhiteSpace(emailAddress)) return this;

        if (emailAddress.Contains(';', StringComparison.OrdinalIgnoreCase))
        {
            // email address has semicolon, try split
            var nameSplit = name.Split(';', StringSplitOptions.TrimEntries);
            var addressSplit = emailAddress.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            for (var i = 0; i < addressSplit.Length; i++)
            {
                var currentName = string.Empty;
                if (nameSplit.Length - 1 >= i)
                    currentName = nameSplit[i];

                if (!string.IsNullOrWhiteSpace(addressSplit[i]))
                    destination.Add(new(addressSplit[i], currentName.Trim()));
            }
        }
        else
        {
            destination.Add(new(emailAddress.Trim(), name.Trim()));
        }

        return this;
    }

    /// <summary>
    /// Adds all recipients in list to email.
    /// </summary>
    /// <returns>Instance of the Email class.</returns>
    private Email AddAddresses(IList<EmailAddress> addresses, IList<EmailAddress> mailAddresses)
    {
        addresses.AddRange(mailAddresses.Where(x => !string.IsNullOrWhiteSpace(x.Address)));

        return this;
    }
}
