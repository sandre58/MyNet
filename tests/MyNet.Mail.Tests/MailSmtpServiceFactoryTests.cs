// -----------------------------------------------------------------------
// <copyright file="MailSmtpServiceFactoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.Tests;

public sealed class MailSmtpServiceFactoryTests
{
    [Fact]
    public void Create_ReturnsMailSmtpService()
    {
        var factory = new MailSmtpServiceFactory();
        var options = new SmtpClientOptions { UsePickupDirectory = true, MailPickupDirectory = @"C:\pickup" };

        var service = factory.Create(options);

        Assert.IsType<MailSmtpService>(service);
        (service as IDisposable)?.Dispose();
    }
}
