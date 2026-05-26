// -----------------------------------------------------------------------
// <copyright file="MailToHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Mail.Tests;

public class MailToHelperTests
{
    [Fact]
    public void SendMail_WithNoRecipients_ReturnsFalse() => Assert.False(MailToHelper.SendMail([], "title", "body"));

    [Fact]
    public void SendMail_WithWhitespaceRecipients_ReturnsFalse() => Assert.False(MailToHelper.SendMail([" ", "\t"], "title", "body"));
}
