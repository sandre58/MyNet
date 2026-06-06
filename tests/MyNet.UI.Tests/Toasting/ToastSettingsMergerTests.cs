// -----------------------------------------------------------------------
// <copyright file="ToastSettingsMergerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Primitives;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Toasting.Settings;
using Xunit;

namespace MyNet.UI.Tests.Toasting;

public sealed class ToastSettingsMergerTests
{
    [Fact]
    public void Merge_UsesGlobalDefaultsWhenNotificationHasNoOverrides()
    {
        var options = new ToastManagerOptions
        {
            DefaultClosingStrategy = ToastClosingStrategy.Both,
            DefaultFreezeOnMouseEnter = false
        };

        var settings = ToastSettingsMerger.Merge(new MessageNotification("hello"), options);

        settings.ClosingStrategy.Should().Be(ToastClosingStrategy.Both);
        settings.FreezeOnMouseEnter.Should().BeFalse();
        settings.Duration.Should().BeNull();
    }

    [Fact]
    public void Merge_AppliesNotificationOverridesWithoutResettingOtherValues()
    {
        var notification = new MessageNotification("hello")
        {
            ToastSettings = new() { Duration = 2.Seconds() }
        };

        var settings = ToastSettingsMerger.Merge(notification, new()
        {
            DefaultClosingStrategy = ToastClosingStrategy.AutoClose,
            DefaultFreezeOnMouseEnter = true
        });

        settings.ClosingStrategy.Should().Be(ToastClosingStrategy.AutoClose);
        settings.FreezeOnMouseEnter.Should().BeTrue();
        settings.Duration.Should().Be(2.Seconds());
    }

    [Fact]
    public void Merge_UpgradesAutoCloseToBothForClosableNotifications()
    {
        var settings = ToastSettingsMerger.Merge(
            new ClosableNotification("message", "title", NotificationSeverity.Information),
            new() { DefaultClosingStrategy = ToastClosingStrategy.AutoClose });

        settings.ClosingStrategy.Should().Be(ToastClosingStrategy.Both);
    }

    [Fact]
    public void Merge_UpgradesNoneToCloseButtonForClosableNotifications()
    {
        var settings = ToastSettingsMerger.Merge(
            new ClosableNotification("message", "title", NotificationSeverity.Information),
            new() { DefaultClosingStrategy = ToastClosingStrategy.None });

        settings.ClosingStrategy.Should().Be(ToastClosingStrategy.CloseButton);
    }

    [Fact]
    public void Merge_RespectsExplicitClosingStrategyOverrideForClosableNotifications()
    {
        var notification = new ClosableNotification("message", "title", NotificationSeverity.Information)
        {
            ToastSettings = new() { ClosingStrategy = ToastClosingStrategy.CloseButton }
        };

        var settings = ToastSettingsMerger.Merge(notification);

        settings.ClosingStrategy.Should().Be(ToastClosingStrategy.CloseButton);
    }
}
