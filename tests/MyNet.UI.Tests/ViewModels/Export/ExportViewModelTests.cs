// -----------------------------------------------------------------------
// <copyright file="ExportViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Moq;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.ViewModels.Export;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Export;

public class ExportViewModelTests
{
    [Fact]
    public async Task ExportAndCloseCommand_WhenNoItems_PublishesValidationErrors()
    {
        var publisher = new Mock<INotificationPublisher>();
        var sut = new TestExportViewModel(publisher.Object);
        await sut.OnOpenedAsync();

        await ExecuteCommandAsync(sut.ExportAndCloseCommand);

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n => n.Severity == NotificationSeverity.Error)),
            Times.AtLeastOnce);
        sut.HasErrors.Should().BeTrue();
        sut.GetResultAsync().IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task ExportAndCloseCommand_WhenValid_ExportsAndClosesWithSuccess()
    {
        var sut = new TestExportViewModel();
        sut.Load(["a", "b"]);
        await sut.OnOpenedAsync();

        await ExecuteCommandAsync(sut.ExportAndCloseCommand);

        sut.ExportCalled.Should().BeTrue();
        var result = await sut.GetResultAsync();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task CancelCommand_SetsCancelledResult()
    {
        var sut = new TestExportViewModel();
        sut.Load(["item"]);
        await sut.OnOpenedAsync();

        sut.CancelCommand.Execute(null);

        (await sut.GetResultAsync()).IsCancelled.Should().BeTrue();
    }

    [Fact]
    public async Task ExportAndCloseCommand_WhenExportFails_DoesNotCloseWithSuccess()
    {
        var sut = new TestExportViewModel { ExportReturnValue = false };
        sut.Load(["item"]);
        await sut.OnOpenedAsync();

        await ExecuteCommandAsync(sut.ExportAndCloseCommand);

        sut.ExportCalled.Should().BeTrue();
        sut.GetResultAsync().IsCompleted.Should().BeFalse();
    }

    private sealed class TestExportViewModel : ExportViewModelBase<string>
    {
        public TestExportViewModel(INotificationPublisher? notificationPublisher = null)
            : base(notificationPublisher)
        {
        }

        public bool ExportCalled { get; private set; }

        public bool ExportReturnValue { get; init; } = true;

        protected override Task<bool> ExportItemsAsync(
            ICollection<string> items,
            CancellationToken cancellationToken = default)
        {
            ExportCalled = true;
            items.Should().NotBeEmpty();
            return Task.FromResult(ExportReturnValue);
        }
    }

    private static async Task ExecuteCommandAsync(ICommand command)
    {
        if (command is IAsyncCommand asyncCommand)
        {
            await asyncCommand.ExecuteAsync(null);
            return;
        }

        command.Execute(null);
        await Task.CompletedTask;
    }
}
