// -----------------------------------------------------------------------
// <copyright file="ImportDialogViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyNet.UI.Notifications;
using MyNet.UI.Notifications.Models;
using MyNet.UI.ViewModels.Import;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Import;

public class ImportDialogViewModelTests
{
    [Fact]
    public void ValidateCommand_CannotExecute_WhenNoItemsMarkedForImport()
    {
        var list = new ImportListViewModel<TestImportItem>();
        list.Load([new() { Import = false }]);

        var sut = new ImportDialogViewModel<TestImportItem>(list);

        sut.ValidateCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void ImportSelectionCommand_MarksSelectedItemsForImport()
    {
        var list = new ImportListViewModel<TestImportItem>();
        var item = new TestImportItem { IsSelected = true, Import = false };
        list.Load([item]);

        list.ImportSelectionCommand.Execute(null);

        item.Import.Should().BeTrue();
        list.ImportItems.Should().ContainSingle();
    }

    [Fact]
    public async Task ValidateCommand_WhenInvalid_PublishesErrors_AndDoesNotCloseAsync()
    {
        var publisher = new Mock<INotificationPublisher>();
        var list = new ImportListViewModel<TestImportItem>();
        list.Load([new() { Import = true, FailValidation = true }]);

        var sut = new ImportDialogViewModel<TestImportItem>(list, publisher.Object);
        await sut.OnOpenedAsync();

        sut.ValidateCommand.Execute(null);

        publisher.Verify(
            x => x.Publish(It.Is<MessageNotification>(n => n.Severity == NotificationSeverity.Error)),
            Times.AtLeastOnce);
        sut.HasErrors.Should().BeTrue();

        var resultTask = sut.GetResultAsync();
        resultTask.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateCommand_WhenValid_ClosesWithImportItemsAsync()
    {
        var list = new ImportListViewModel<TestImportItem>();
        var expected = new TestImportItem { Import = true };
        list.Load([expected]);

        var sut = new ImportDialogViewModel<TestImportItem>(list);
        await sut.OnOpenedAsync();

        sut.ValidateCommand.Execute(null);

        var result = await sut.GetResultAsync();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle().Which.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task CancelCommand_SetsCancelledResultAsync()
    {
        var list = new ImportListViewModel<TestImportItem>();
        list.Load([new() { Import = true }]);

        var sut = new ImportDialogViewModel<TestImportItem>(list);
        await sut.OnOpenedAsync();

        sut.CancelCommand.Execute(null);

        var result = await sut.GetResultAsync();
        result.IsCancelled.Should().BeTrue();
    }

    private sealed class TestImportItem : ImportItemViewModel
    {
        public bool FailValidation { get; init; }

        public override IReadOnlyCollection<string> ValidateForImport()
            => FailValidation ? ["import-error"] : [];
    }
}
