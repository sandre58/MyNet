// -----------------------------------------------------------------------
// <copyright file="MailKitServiceCreateMailMessageTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using MimeKit;
using MimeKit.Text;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public class MailKitServiceCreateMailMessageTests
{
    [Fact]
    public void CreateMailMessage_HtmlBody_SetsHtmlPart()
    {
        var email = Email.From("from@test.com", "From")
            .To("to@test.com")
            .Subject("Subject")
            .Body("<b>Hi</b>", isHtml: true);

        using var message = MailKitService.CreateMailMessage(email);

        Assert.Equal("Subject", message.Subject);
        Assert.Equal("from@test.com", message.From.Mailboxes.First().Address);
        Assert.Equal("to@test.com", message.To.Mailboxes.First().Address);
        Assert.IsType<TextPart>(message.Body);
        Assert.Equal(TextFormat.Html, ((TextPart)message.Body).Format);
    }

    [Fact]
    public void CreateMailMessage_PlainAndHtmlAlternative_BuildsMultipart()
    {
        var email = Email.From("from@test.com")
            .To("to@test.com")
            .Subject("Alt")
            .Body("<p>HTML</p>", isHtml: true)
            .PlaintextAlternativeBody("Plain");

        using var message = MailKitService.CreateMailMessage(email);

        Assert.IsType<MultipartAlternative>(message.Body);
    }

    [Fact]
    public void CreateMailMessage_PriorityHigh_SetsUrgentPriority()
    {
        var email = Email.From("from@test.com").To("to@test.com").HighPriority();

        using var message = MailKitService.CreateMailMessage(email);

        Assert.Equal(MessagePriority.Urgent, message.Priority);
    }

    [Fact]
    public void CreateMailMessage_PreferredEncoding_SetsCharsetOnTextParts()
    {
        var email = Email.From("from@test.com").To("to@test.com").Body("Hello", isHtml: false);
        var encoding = Encoding.GetEncoding("iso-8859-1");

        using var message = MailKitService.CreateMailMessage(email, encoding);

        var text = Assert.IsType<TextPart>(message.Body);
        Assert.Equal("iso-8859-1", text.ContentType.Charset);
    }

    [Fact]
    public void CreateMailMessage_CustomHeader_IsPresent()
    {
        var email = Email.From("from@test.com").To("to@test.com").Header("X-Custom", "value");

        using var message = MailKitService.CreateMailMessage(email);

        Assert.Equal("value", message.Headers["X-Custom"]);
    }

    [Fact]
    public void CreateMailMessage_PlainTextBody_SetsTextPart()
    {
        var email = Email.From("from@test.com").To("to@test.com").Body("Hello", isHtml: false);

        using var message = MailKitService.CreateMailMessage(email);

        var text = Assert.IsType<TextPart>(message.Body);
        Assert.Equal(TextFormat.Plain, text.Format);
    }

    [Fact]
    public void CreateMailMessage_CcBccAndReplyTo_AreMapped()
    {
        var email = Email.From("from@test.com")
            .To("to@test.com")
            .Cc("cc@test.com")
            .Bcc("bcc@test.com")
            .ReplyTo("reply@test.com");

        using var message = MailKitService.CreateMailMessage(email);

        Assert.Equal("cc@test.com", message.Cc.Mailboxes.First().Address);
        Assert.Equal("bcc@test.com", message.Bcc.Mailboxes.First().Address);
        Assert.Equal("reply@test.com", message.ReplyTo.Mailboxes.First().Address);
    }

    [Fact]
    public void CreateMailMessage_LowPriority_SetsNonUrgentPriority()
    {
        var email = Email.From("from@test.com").To("to@test.com").LowPriority();

        using var message = MailKitService.CreateMailMessage(email);

        Assert.Equal(MessagePriority.NonUrgent, message.Priority);
    }

    [Fact]
    public void CreateMailMessage_WithAttachment_AddsAttachmentPart()
    {
        var email = Email.From("from@test.com")
            .To("to@test.com")
            .Attach(new Models.Attachment
            {
                Filename = "file.txt",
                ContentType = "text/plain",
                ContentId = "cid-1",
                Data = new System.IO.MemoryStream("data"u8.ToArray())
            });

        using var message = MailKitService.CreateMailMessage(email);

        Assert.IsType<Multipart>(message.Body);
    }

    [Fact]
    public void CreateMailMessage_NullEmail_ThrowsArgumentNullException() => Assert.Throws<ArgumentNullException>(() => MailKitService.CreateMailMessage(null!));
}
