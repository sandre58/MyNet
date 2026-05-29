// -----------------------------------------------------------------------
// <copyright file="DialogServiceExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Locators;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public class DialogServiceExtensionsTests
{
    [Fact]
    public async Task ShowAsync_WithLocator_ResolvesDialogAndDelegatesToServiceAsync()
    {
        var dialog = new TestDialog();
        var expected = DialogResult.Ok();

        var locator = new Mock<IViewModelLocator>();
        locator.Setup(x => x.Get<TestDialog>()).Returns(dialog);

        var service = new Mock<IDialogService>();
        service.Setup(x => x.ShowAsync(dialog, null, CancellationToken.None)).ReturnsAsync(expected);

        var result = await service.Object.ShowAsync<TestDialog>(locator.Object);

        result.Should().BeSameAs(expected);
        locator.Verify(x => x.Get<TestDialog>(), Times.Once);
        service.Verify(x => x.ShowAsync(dialog, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ShowAsyncTyped_WithLocator_ResolvesDialogAndDelegatesToTypedServiceAsync()
    {
        var dialog = new TestTypedDialog();
        var expected = DialogResult<string>.Success("ok");

        var locator = new Mock<IViewModelLocator>();
        locator.Setup(x => x.Get<TestTypedDialog>()).Returns(dialog);

        var service = new Mock<IDialogService>();
        service.Setup(x => x.ShowAsync(dialog, null, CancellationToken.None)).ReturnsAsync(expected);

        var result = await service.Object.ShowAsync<TestTypedDialog, string>(locator.Object);

        result.Should().BeSameAs(expected);
        locator.Verify(x => x.Get<TestTypedDialog>(), Times.Once);
        service.Verify(x => x.ShowAsync(dialog, null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void Create_WithLocator_ResolvesDialogAndDelegatesToService()
    {
        var dialog = new TestDialog();
        var builder = new Mock<IDialogBuilder>().Object;

        var locator = new Mock<IViewModelLocator>();
        locator.Setup(x => x.Get<TestDialog>()).Returns(dialog);

        var service = new Mock<IDialogService>();
        service.Setup(x => x.Create(dialog)).Returns(builder);

        var result = service.Object.Create<TestDialog>(locator.Object);

        result.Should().BeSameAs(builder);
        locator.Verify(x => x.Get<TestDialog>(), Times.Once);
        service.Verify(x => x.Create(dialog), Times.Once);
    }

    [Fact]
    public void CreateTyped_WithLocator_ResolvesDialogAndDelegatesToTypedService()
    {
        var dialog = new TestTypedDialog();
        var builder = new Mock<IDialogBuilder<string>>().Object;

        var locator = new Mock<IViewModelLocator>();
        locator.Setup(x => x.Get<TestTypedDialog>()).Returns(dialog);

        var service = new Mock<IDialogService>();
        service.Setup(x => x.Create(dialog)).Returns(builder);

        var result = service.Object.Create<TestTypedDialog, string>(locator.Object);

        result.Should().BeSameAs(builder);
        locator.Verify(x => x.Get<TestTypedDialog>(), Times.Once);
        service.Verify(x => x.Create(dialog), Times.Once);
    }

    [Fact]
    public async Task ShowAsync_WithLocator_Throws_WhenLocatorCannotResolveDialogAsync()
    {
        var locator = new Mock<IViewModelLocator>();
        locator.Setup(x => x.Get<TestDialog>()).Throws(new InvalidOperationException("missing"));

        var service = new Mock<IDialogService>();

        var act = async () => await service.Object.ShowAsync<TestDialog>(locator.Object).ConfigureAwait(false);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("missing");
    }

    [Fact]
    public void Create_WithNullLocator_ThrowsArgumentNullException()
    {
        var service = new Mock<IDialogService>();

        var act = () => service.Object.Create<TestDialog>((IViewModelLocator)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    private class TestDialog : IDialog
    {
        public string Title => "Test";

#pragma warning disable CS0067
        public event EventHandler<CloseRequestedEventArgs>? CloseRequested;
#pragma warning restore CS0067

        public Task<bool> CanCloseAsync() => Task.FromResult(true);

        public Task OnOpenedAsync() => Task.CompletedTask;

        public Task OnClosedAsync() => Task.CompletedTask;
    }

    private sealed class TestTypedDialog : TestDialog, IDialog<string>
    {
        public Task<DialogResult<string>> GetResultAsync() => Task.FromResult(DialogResult<string>.Success("value"));
    }
}
