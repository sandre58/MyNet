// -----------------------------------------------------------------------
// <copyright file="MailKitServiceSendTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public sealed class MailKitServiceSendTests
{
    [Fact]
    public async Task SendAsync_PickupDirectory_WritesEmlFileAsync()
    {
        var pickupDir = CreateTempDirectory();
        try
        {
            var options = new SmtpClientOptions
            {
                UsePickupDirectory = true,
                MailPickupDirectory = pickupDir
            };

            using var service = new MailKitService(options);
            var email = Email.From("from@test.com")
                .To("to@test.com")
                .Subject("Pickup")
                .Body("Hello");

            var response = await service.SendAsync(email);

            Assert.True(response.Successful);
            Assert.NotEmpty(Directory.GetFiles(pickupDir, "*.eml"));
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

        using var service = new MailKitService(new() { Server = "smtp.test", Port = 587 });
        var response = await service.SendAsync(Email.From("from@test.com").To("to@test.com"), cts.Token);

        Assert.Contains("Send canceled.", response.ErrorMessages);
    }

    [Fact]
    public async Task SendAsync_EmptySender_AddsErrorMessageAsync()
    {
        var pickupDir = CreateTempDirectory();
        try
        {
            var options = new SmtpClientOptions
            {
                UsePickupDirectory = true,
                MailPickupDirectory = pickupDir
            };

            using var service = new MailKitService(options);
            var email = Email.From(string.Empty).To("to@test.com");

            var response = await service.SendAsync(email);

            Assert.False(response.Successful);
            Assert.NotEmpty(response.ErrorMessages);
        }
        finally
        {
            DeleteDirectory(pickupDir);
        }
    }

    [Fact]
    public async Task CanConnectAsync_PickupDirectory_ExistingDirectory_ReturnsTrueAsync()
    {
        var pickupDir = CreateTempDirectory();
        try
        {
            var options = new SmtpClientOptions
            {
                UsePickupDirectory = true,
                MailPickupDirectory = pickupDir
            };

            using var service = new MailKitService(options);

            Assert.True(await service.CanConnectAsync());
        }
        finally
        {
            DeleteDirectory(pickupDir);
        }
    }

    [Fact]
    public async Task CanConnectAsync_PickupDirectory_MissingDirectory_ReturnsFalseAsync()
    {
        var options = new SmtpClientOptions
        {
            UsePickupDirectory = true,
            MailPickupDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
        };

        using var service = new MailKitService(options);

        Assert.False(await service.CanConnectAsync());
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var service = new MailKitService(new() { Server = "smtp.test", Port = 587 });

        service.Dispose();
        service.Dispose();
    }

    [Fact]
    public async Task SendAsync_AfterDispose_ThrowsObjectDisposedExceptionAsync()
    {
        var service = new MailKitService(new() { Server = "smtp.test", Port = 587 });
        service.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            service.SendAsync(Email.From("from@test.com").To("to@test.com")));
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
