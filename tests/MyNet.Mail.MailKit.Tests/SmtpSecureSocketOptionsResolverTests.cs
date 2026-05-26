// -----------------------------------------------------------------------
// <copyright file="SmtpSecureSocketOptionsResolverTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MailKit.Security;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public class SmtpSecureSocketOptionsResolverTests
{
    [Fact]
    public void Resolve_Auto_Port465_UsesSslOnConnect()
    {
        var options = new SmtpClientOptions { Port = 465, UseSsl = false };

        Assert.Equal(SecureSocketOptions.SslOnConnect, SmtpSecureSocketOptionsResolver.Resolve(options));
    }

    [Fact]
    public void Resolve_Auto_UseSsl_UsesStartTls()
    {
        var options = new SmtpClientOptions { Port = 587, UseSsl = true };

        Assert.Equal(SecureSocketOptions.StartTls, SmtpSecureSocketOptionsResolver.Resolve(options));
    }

    [Fact]
    public void Resolve_Auto_NoSsl_UsesNone()
    {
        var options = new SmtpClientOptions { Port = 25, UseSsl = false };

        Assert.Equal(SecureSocketOptions.None, SmtpSecureSocketOptionsResolver.Resolve(options));
    }

    [Theory]
    [InlineData(SmtpSecurityMode.SslOnConnect, SecureSocketOptions.SslOnConnect)]
    [InlineData(SmtpSecurityMode.StartTls, SecureSocketOptions.StartTls)]
    [InlineData(SmtpSecurityMode.StartTlsWhenAvailable, SecureSocketOptions.StartTlsWhenAvailable)]
    [InlineData(SmtpSecurityMode.None, SecureSocketOptions.None)]
    public void Resolve_ExplicitMode_MapsCorrectly(SmtpSecurityMode mode, SecureSocketOptions expected)
    {
        var options = new SmtpClientOptions { SecurityMode = mode, Port = 25, UseSsl = false };

        Assert.Equal(expected, SmtpSecureSocketOptionsResolver.Resolve(options));
    }
}
