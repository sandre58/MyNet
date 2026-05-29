// -----------------------------------------------------------------------
// <copyright file="SendResponseTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Mail.Models;
using Xunit;

namespace MyNet.Mail.Tests;

public sealed class SendResponseTests
{
    [Fact]
    public void Successful_WhenNoErrors_ReturnsTrue()
    {
        var response = new SendResponse();

        Assert.True(response.Successful);
    }

    [Fact]
    public void Successful_WhenErrorsPresent_ReturnsFalse()
    {
        var response = new SendResponse();
        response.ErrorMessages.Add("failed");

        Assert.False(response.Successful);
    }
}
