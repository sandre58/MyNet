// -----------------------------------------------------------------------
// <copyright file="MailSmtpServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Mail.Smtp;
using Xunit;
using Attachment = MyNet.Mail.Models.Attachment;

namespace MyNet.Mail.Tests;

public sealed class MailSmtpServiceTests
{
    [Fact]
    public void CreateMailMessage_HtmlWithPlainAlternative_BuildsMultipartAlternative()
    {
        var email = Email.From("from@test.com", "From")
            .To("to@test.com", "To")
            .Subject("Subject")
            .Body("<p>HTML</p>", isHtml: true)
            .PlaintextAlternativeBody("Plain");

        using var message = MailSmtpService.CreateMailMessage(email);

        Assert.Equal("Subject", message.Subject);
        Assert.NotNull(message.From);
        Assert.Equal("from@test.com", message.From.Address);
        Assert.False(message.IsBodyHtml);
        Assert.Equal("Plain", message.Body);
        Assert.Single(message.AlternateViews);
        Assert.Equal("text/html; charset=UTF-8", message.AlternateViews[0].ContentType.ToString());
    }

    [Fact]
    public void CreateMailMessage_PlainHtmlBody_SetsHtmlFlag()
    {
        var email = Email.From("from@test.com")
            .To("to@test.com")
            .Body("<b>Hi</b>", isHtml: true);

        using var message = MailSmtpService.CreateMailMessage(email);

        Assert.True(message.IsBodyHtml);
        Assert.Equal("<b>Hi</b>", message.Body);
    }

    [Fact]
    public void CreateMailMessage_WithRecipientsHeadersAndAttachment_MapsAllFields()
    {
        var email = Email.From("from@test.com")
            .To("to@test.com")
            .Cc("cc@test.com")
            .Bcc("bcc@test.com")
            .ReplyTo("reply@test.com")
            .Header("X-Custom", "value")
            .LowPriority()
            .Attach(new Attachment
            {
                Filename = "doc.txt",
                ContentType = "text/plain",
                ContentId = "cid-1",
                Data = new MemoryStream("content"u8.ToArray())
            })
            .Attach(new Attachment { Filename = "empty.bin", Data = null });

        using var message = MailSmtpService.CreateMailMessage(email);

        Assert.Equal("to@test.com", message.To[0].Address);
        Assert.Equal("cc@test.com", message.CC[0].Address);
        Assert.Equal("bcc@test.com", message.Bcc[0].Address);
        Assert.Equal("reply@test.com", message.ReplyToList[0].Address);
        Assert.Equal("value", message.Headers["X-Custom"]);
        Assert.Equal(MailPriority.Low, message.Priority);
        Assert.Single(message.Attachments);
        Assert.Equal("cid-1", message.Attachments[0].ContentId);
    }

    [Fact]
    public void CreateMailMessage_HighPriority_SetsHighMailPriority()
    {
        var email = Email.From("from@test.com").To("to@test.com").HighPriority();

        using var message = MailSmtpService.CreateMailMessage(email);

        Assert.Equal(MailPriority.High, message.Priority);
    }

    [Fact]
    public async Task SendAsync_PickupDirectory_WritesMailFileAsync()
    {
        var pickupDir = CreateTempDirectory();
        try
        {
            var options = new SmtpClientOptions
            {
                UsePickupDirectory = true,
                MailPickupDirectory = pickupDir
            };

            using var service = new MailSmtpService(options);
            var email = Email.From("from@test.com")
                .To("to@test.com")
                .Subject("Pickup test")
                .Body("Hello");

            var response = await service.SendAsync(email);

            Assert.True(response.Successful);
            Assert.NotEmpty(Directory.GetFiles(pickupDir));
        }
        finally
        {
            DeleteDirectory(pickupDir);
        }
    }

    [Fact]
    public async Task SendAsync_WhenCanceled_AddsErrorMessageAsync()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        using var service = new MailSmtpService(new SmtpClient("localhost", 25));
        var response = await service.SendAsync(Email.From("from@test.com").To("to@test.com"), cts.Token);

        Assert.Contains("Send canceled.", response.ErrorMessages);
    }

    [Fact]
    public void Send_WhenCanceled_AddsErrorMessage()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        using var service = new MailSmtpService(new SmtpClient("localhost", 25));
        var response = service.Send(Email.From("from@test.com").To("to@test.com"), cts.Token);

        Assert.Contains("Send canceled.", response.ErrorMessages);
    }

    [Fact]
    public void CanConnect_WithUnreachableHost_ReturnsFalse()
    {
        using var client = new SmtpClient("127.0.0.1", 1);
        using var service = new MailSmtpService(client);

        Assert.False(service.CanConnect());
    }

    [Fact]
    public async Task CanConnectAsync_WithUnreachableHost_ReturnsFalseAsync()
    {
        using var client = new SmtpClient("127.0.0.1", 1);
        using var service = new MailSmtpService(client);

        Assert.False(await service.CanConnectAsync());
    }

    private static string CreateTempDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
