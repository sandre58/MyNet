// -----------------------------------------------------------------------
// <copyright file="EmailFactoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace MyNet.Mail.Tests;

public class EmailFactoryTests
{
    [Fact]
    public void Create_UsesConfiguredSender()
    {
        var factory = new EmailFactory("noreply@test.com", "No Reply");

        var email = factory.Create();

        Assert.Equal("noreply@test.com", email.Data.From.Address);
        Assert.Equal("No Reply", email.Data.From.Name);
    }
}
