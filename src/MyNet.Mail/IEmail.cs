// -----------------------------------------------------------------------
// <copyright file="IEmail.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Mail.Models;

namespace MyNet.Mail;

public interface IEmail
{
    EmailData Data { get; }

    /// <summary>
    /// Set the send from email address.
    /// </summary>
    /// <param name="emailAddress">Email address of sender.</param>
    /// <param name="name">Name of sender.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail SetFrom(string emailAddress, string name = "");

    /// <summary>
    /// Adds a reciepient to the email, Splits name and address on ';'.
    /// </summary>
    /// <param name="emailAddress">Email address of recipeient.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail To(string emailAddress, string name = "");

    /// <summary>
    /// Adds all reciepients in list to email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail To(IList<EmailAddress> mailAddresses);

    /// <summary>
    /// Adds a Carbon Copy to the email.
    /// </summary>
    /// <param name="emailAddress">Email address of recipeient.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Cc(string emailAddress, string name = "");

    /// <summary>
    /// Adds a Carbon Copy to the email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Cc(IList<EmailAddress> mailAddresses);

    /// <summary>
    /// Adds a blind carbon copy to the email.
    /// </summary>
    /// <param name="emailAddress">Email address of recipeient.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Bcc(string emailAddress, string name = "");

    /// <summary>
    /// Adds a blind carbon copy to the email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Bcc(IList<EmailAddress> mailAddresses);

    /// <summary>
    /// Sets the ReplyTo address on the email.
    /// </summary>
    /// <param name="emailAddress">Email address of recipeient.</param>
    /// <param name="name">Name of recipient.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail ReplyTo(string emailAddress, string name = "");

    /// <summary>
    /// Sets the ReplyTo address on the email.
    /// </summary>
    /// <param name="mailAddresses">List of recipients.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail ReplyTo(IList<EmailAddress> mailAddresses);

    /// <summary>
    /// Sets the subject of the email.
    /// </summary>
    /// <param name="subject">email subject.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Subject(string subject);

    /// <summary>
    /// Adds a Body to the Email.
    /// </summary>
    /// <param name="body">The content of the body.</param>
    /// <param name="isHtml">True if Body is HTML, false for plain text (Optional).</param>
    IEmail Body(string body, bool isHtml = false);

    /// <summary>
    /// Marks the email as High Priority.
    /// </summary>
    IEmail HighPriority();

    /// <summary>
    /// Marks the email as Low Priority.
    /// </summary>
    IEmail LowPriority();

    /// <summary>
    /// Adds an Attachment to the Email.
    /// </summary>
    /// <param name="attachment">The Attachment to add.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Attach(Attachment attachment);

    /// <summary>
    /// Adds Multiple Attachments to the Email.
    /// </summary>
    /// <param name="attachments">The List of Attachments to add.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Attach(IList<Attachment> attachments);

    IEmail AttachFromFilename(string filename, string contentType = "", string attachmentName = "");

    /// <summary>
    /// Adds a Plaintext alternative Body to the Email. Used in conjunction with an HTML email,
    /// this allows for email readers without html capability, and also helps avoid spam filters.
    /// </summary>
    /// <param name="body">The content of the body.</param>
    IEmail PlaintextAlternativeBody(string body);

    /// <summary>
    /// Adds header to the Email.
    /// </summary>
    /// <param name="header">Header name, only printable ASCII allowed.</param>
    /// <param name="body">value of the header.</param>
    /// <returns>Instance of the Email class.</returns>
    IEmail Header(string header, string body);
}
