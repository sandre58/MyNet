// -----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MyNet.Mail.MailKit.Tests;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMailKitMailService_RegistersMailKitServiceFactory()
    {
        var services = new ServiceCollection();

        services.AddMailKitMailService();

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IMailServiceFactory>();

        Assert.IsType<MailKitServiceFactory>(factory);
    }
}
