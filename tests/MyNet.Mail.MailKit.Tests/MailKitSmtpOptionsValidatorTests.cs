// -----------------------------------------------------------------------
// <copyright file="MailKitSmtpOptionsValidatorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Mail.MailKit.Exceptions;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public class MailKitSmtpOptionsValidatorTests
{
    [Fact]
    public void Validate_MissingServer_ThrowsUndefinedServerException()
    {
        var options = new SmtpClientOptions { Server = null, Port = 25 };

        Assert.Throws<UndefinedServerException>(() => MailKitSmtpOptionsValidator.Validate(options));
    }

    [Fact]
    public void Validate_InvalidPort_ThrowsArgumentOutOfRangeException()
    {
        var options = new SmtpClientOptions { Server = "smtp.test", Port = 0 };

        Assert.Throws<ArgumentOutOfRangeException>(() => MailKitSmtpOptionsValidator.Validate(options));
    }

    [Fact]
    public void Validate_PickupWithoutDirectory_ThrowsArgumentException()
    {
        var options = new SmtpClientOptions { UsePickupDirectory = true, MailPickupDirectory = null };

        Assert.Throws<ArgumentException>(() => MailKitSmtpOptionsValidator.Validate(options));
    }

    [Fact]
    public void Validate_ValidNetworkOptions_DoesNotThrow()
    {
        var options = new SmtpClientOptions
        {
            Server = "smtp.test",
            Port = 587,
            RequiresAuthentication = true,
            User = "user",
            Password = "pass"
        };

        var exception = Record.Exception(() => MailKitSmtpOptionsValidator.Validate(options));

        Assert.Null(exception);
    }
}
