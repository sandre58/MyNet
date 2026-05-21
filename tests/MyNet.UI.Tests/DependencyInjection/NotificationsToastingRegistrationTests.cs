// -----------------------------------------------------------------------
// <copyright file="NotificationsToastingRegistrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Notifications;
using MyNet.UI.Threading;
using MyNet.UI.Toasting;
using MyNet.UI.Toasting.Filters;
using Xunit;

namespace MyNet.UI.Tests.DependencyInjection;

public class NotificationsToastingRegistrationTests
{
    [Fact]
    public void AddMyNetNotifications_RegistersCoreServices()
    {
        var services = new ServiceCollection();
        services.AddNotifications();
        using var provider = services.BuildServiceProvider();

        provider.GetService<INotificationService>().Should().NotBeNull();
        provider.GetService<INotificationsManager>().Should().NotBeNull();
        provider.GetService<ISchedulerProvider>().Should().NotBeNull();
    }

    [Fact]
    public void AddMyNetToasting_RegistersToastServices()
    {
        var services = new ServiceCollection();
        services.AddToasting();
        using var provider = services.BuildServiceProvider();

        provider.GetService<IToastManager>().Should().NotBeNull();
        provider.GetService<IToastFactory>().Should().NotBeNull();
        provider.GetService<IToastFilter>().Should().BeOfType<AllToastsFilter>();
        provider.GetService<INotificationService>().Should().NotBeNull();
    }
}
