// -----------------------------------------------------------------------
// <copyright file="SmtpHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Mail.Smtp;
using Xunit;

namespace MyNet.Mail.Tests;

public class SmtpHelperTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TestSmtpConnection_WithMissingServer_ReturnsFalse(string? server) => Assert.False(SmtpHelper.TestSmtpConnection(server, 25));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(70000)]
    public void TestSmtpConnection_WithInvalidPort_ReturnsFalse(int port) => Assert.False(SmtpHelper.TestSmtpConnection("smtp.test", port));
}
