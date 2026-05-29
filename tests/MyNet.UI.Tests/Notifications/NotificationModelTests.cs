// -----------------------------------------------------------------------
// <copyright file="NotificationModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Dialogs.FileDialogs;
using MyNet.UI.Loading.Models;
using MyNet.UI.Notifications.Models;
using MyNet.UI.Notifications.Processors;
using Xunit;

namespace MyNet.UI.Tests.Notifications;

public sealed class NotificationModelTests
{
    [Fact]
    public void ActionNotification_ExposesConfiguredAction()
    {
        var invoked = false;
        var notification = new ActionNotification(
            "message",
            "title",
            NotificationSeverity.Information,
            action: _ => invoked = true);

        notification.Action.Should().NotBeNull();
        notification.Action!(notification);
        invoked.Should().BeTrue();
    }

    [Fact]
    public void IndeterminateBusy_StoresMessage()
    {
        var busy = new IndeterminateBusy { Message = "Working..." };

        busy.Message.Should().Be("Working...");
    }

    [Fact]
    public void DeterminateBusy_StoresProgressValues()
    {
        var busy = new DeterminateBusy
        {
            Message = "Loading",
            Minimum = 0,
            Maximum = 100,
            Value = 42
        };

        busy.Message.Should().Be("Loading");
        busy.Minimum.Should().Be(0);
        busy.Maximum.Should().Be(100);
        busy.Value.Should().Be(42);
    }

    [Fact]
    public void FileDialogResult_Filename_ReturnsFirstSelectedFile()
    {
        var result = new FileDialogResult { Files = ["C:\\temp\\one.txt", "C:\\temp\\two.txt"] };

        result.Filename.Should().Be("C:\\temp\\one.txt");
    }

    [Fact]
    public void FileDialogResult_Filename_ReturnsNullWhenNoFilesSelected() => new FileDialogResult().Filename.Should().BeNull();

    [Fact]
    public void OpenFileDialogSettings_Default_IsInitialized()
    {
        var settings = OpenFileDialogSettings.Default;

        settings.Should().NotBeNull();
        settings.Multiselect.Should().BeFalse();
    }

    [Fact]
    public void EmptyNotificationFilter_RemovesBlankMessages()
    {
        var filter = new EmptyNotificationFilter();

        filter.Process(new MessageNotification("   ")).Should().BeNull();
        filter.Process(new MessageNotification("visible")).Should().NotBeNull();
    }
}
