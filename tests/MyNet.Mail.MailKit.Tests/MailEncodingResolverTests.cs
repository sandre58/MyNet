// -----------------------------------------------------------------------
// <copyright file="MailEncodingResolverTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public class MailEncodingResolverTests
{
    [Fact]
    public void Resolve_NullOrEmpty_ReturnsUtf8()
    {
        Assert.Equal(Encoding.UTF8, MailEncodingResolver.Resolve((string?)null));
        Assert.Equal(Encoding.UTF8, MailEncodingResolver.Resolve(string.Empty));
    }

    [Fact]
    public void Resolve_FromOptions_UsesPreferredEncoding()
    {
        var options = new SmtpClientOptions { PreferredEncoding = "iso-8859-1" };

        Assert.Equal("iso-8859-1", MailEncodingResolver.Resolve(options).WebName);
    }

    [Fact]
    public void Resolve_InvalidName_FallsBackToUtf8() => Assert.Equal(Encoding.UTF8, MailEncodingResolver.Resolve("not-a-real-encoding-name-xyz"));
}
