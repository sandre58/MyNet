// -----------------------------------------------------------------------
// <copyright file="MailKitServiceFactoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging.Abstractions;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public sealed class MailKitServiceFactoryTests
{
    [Fact]
    public void Create_ReturnsMailKitService()
    {
        var factory = new MailKitServiceFactory(NullLoggerFactory.Instance);
        var options = new SmtpClientOptions { UsePickupDirectory = true, MailPickupDirectory = @"C:\pickup" };

        var service = factory.Create(options);

        Assert.IsType<MailKitService>(service);
        (service as IDisposable)?.Dispose();
    }
}
