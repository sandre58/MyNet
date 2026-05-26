// -----------------------------------------------------------------------
// <copyright file="MailKitSmtpOptionsValidatorAuthTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public class MailKitSmtpOptionsValidatorAuthTests
{
    [Fact]
    public void Validate_RequiresAuthenticationWithoutUser_ThrowsArgumentException()
    {
        var options = new SmtpClientOptions
        {
            Server = "smtp.test",
            Port = 587,
            RequiresAuthentication = true,
            User = null,
            Password = "pass"
        };

        var exception = Assert.Throws<ArgumentException>(() => MailKitSmtpOptionsValidator.Validate(options));

        Assert.Contains("user", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_RequiresAuthenticationWithoutPassword_ThrowsArgumentException()
    {
        var options = new SmtpClientOptions
        {
            Server = "smtp.test",
            Port = 587,
            RequiresAuthentication = true,
            User = "user",
            Password = null
        };

        var exception = Assert.Throws<ArgumentException>(() => MailKitSmtpOptionsValidator.Validate(options));

        Assert.Contains("password", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ValidPickupDirectory_DoesNotThrow()
    {
        var options = new SmtpClientOptions
        {
            UsePickupDirectory = true,
            MailPickupDirectory = @"C:\pickup"
        };

        var exception = Record.Exception(() => MailKitSmtpOptionsValidator.Validate(options));

        Assert.Null(exception);
    }
}
