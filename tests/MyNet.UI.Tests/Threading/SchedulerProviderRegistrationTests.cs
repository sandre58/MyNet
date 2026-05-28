// -----------------------------------------------------------------------
// <copyright file="SchedulerProviderRegistrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Notifications;
using MyNet.UI.Threading;
using MyNet.UI.Toasting;
using Xunit;

namespace MyNet.UI.Tests.Threading;

public sealed class SchedulerProviderRegistrationTests
{
    [Fact]
    public void AddSchedulerProvider_RegistersDefaultImplementation()
    {
        var services = new ServiceCollection();
        services.AddSchedulerProvider();
        using var provider = services.BuildServiceProvider();

        provider.GetService<ISchedulerProvider>().Should().BeOfType<DefaultSchedulerProvider>();
    }

    [Fact]
    public void AddSchedulerProvider_IsIdempotent()
    {
        var services = new ServiceCollection();
        services.AddSchedulerProvider();
        services.AddSchedulerProvider();
        using var provider = services.BuildServiceProvider();

        provider.GetServices<ISchedulerProvider>().Should().ContainSingle();
    }

    [Fact]
    public void AddNotificationsAndToasting_ShareSingleSchedulerProvider()
    {
        var services = new ServiceCollection();
        services.AddNotifications();
        services.AddToasting();
        using var provider = services.BuildServiceProvider();

        var schedulers = provider.GetServices<ISchedulerProvider>().ToList();
        schedulers.Should().ContainSingle().Which.Should().BeOfType<DefaultSchedulerProvider>();
    }
}
