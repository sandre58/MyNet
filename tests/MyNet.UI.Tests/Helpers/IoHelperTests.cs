// -----------------------------------------------------------------------
// <copyright file="IoHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using FluentAssertions;
using Moq;
using MyNet.UI.Helpers;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using Xunit;

namespace MyNet.UI.Tests.Helpers;

public sealed class IoHelperTests
{
    [Fact]
    public void OpenFolderLocation_WhenDirectoryExists_ReturnsTrue()
    {
        var existingFile = Path.Combine(Path.GetTempPath(), "mynet-ui-helper-test.txt");
        File.WriteAllText(existingFile, "test");

        try
        {
            IoHelper.OpenFolderLocation(existingFile).Should().BeTrue();
        }
        finally
        {
            File.Delete(existingFile);
        }
    }

    [Fact]
    public void OpenFolderLocation_WhenDirectoryMissing_PublishesErrorAndReturnsFalse()
    {
        var publisher = new Mock<INotificationPublisher>();
        var missingFile = Path.Combine(Path.GetTempPath(), "missing-folder", "file.txt");

        var opened = IoHelper.OpenFolderLocation(missingFile, publisher.Object);

        opened.Should().BeFalse();
        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n => n.Severity == NotificationSeverity.Error)),
            Times.Once);
    }
}
