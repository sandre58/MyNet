// -----------------------------------------------------------------------
// <copyright file="SmtpClientOptionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.Tests;

public sealed class SmtpClientOptionsTests
{
    [Fact]
    public void Defaults_UsePort25AndAutoSecurity()
    {
        var options = new SmtpClientOptions();

        Assert.Equal(25, options.Port);
        Assert.Equal(SmtpSecurityMode.Auto, options.SecurityMode);
        Assert.False(options.UseSsl);
        Assert.False(options.RequiresAuthentication);
    }
}
