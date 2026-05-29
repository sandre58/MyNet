// -----------------------------------------------------------------------
// <copyright file="ContentDialogServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.ContentDialogs;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public class ContentDialogServiceTests
{
    [Fact]
    public async Task ShowAsync_UsesHighestPriorityStrategyAsync()
    {
        var low = new StubDialogStrategy(priority: 1, result: DialogResult.Cancel());
        var high = new StubDialogStrategy(priority: 10, result: DialogResult.Ok());
        var service = new ContentDialogService([low, high]);
        var dialog = new TestDialog();

        var result = await service.ShowAsync(dialog).ConfigureAwait(false);

        result.IsSuccess.Should().BeTrue();
        high.ShowCount.Should().Be(1);
        low.ShowCount.Should().Be(0);
    }

    [Fact]
    public async Task ShowAsyncTyped_ReturnsViewModelResultAsync()
    {
        var strategy = new BlockingDialogStrategy();
        var service = new ContentDialogService([strategy]);
        var dialog = new TestTypedDialog();

        var showTask = service.ShowAsync(dialog);
        await strategy.WaitUntilShowingAsync().ConfigureAwait(false);
        dialog.Complete("payload");
        strategy.Release();
        var result = await showTask.ConfigureAwait(false);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("payload");
    }

    [Fact]
    public async Task ShowAsync_RaisesDialogOpened()
    {
        var strategy = new BlockingDialogStrategy();
        var service = new ContentDialogService([strategy]);
        var dialog = new TestDialog();
        ContentDialogLifecycleEventArgs? opened = null;
        service.DialogOpened += (_, e) => opened = e;

        var showTask = service.ShowAsync(dialog);
        await strategy.WaitUntilShowingAsync().ConfigureAwait(false);

        opened.Should().NotBeNull();
        opened!.Dialog.Should().BeSameAs(dialog);

        strategy.Release();
        await showTask.ConfigureAwait(false);
    }

    [Fact]
    public async Task ShowAsync_TracksOpenedDialogsDuringDisplayAsync()
    {
        var strategy = new BlockingDialogStrategy();
        var service = new ContentDialogService([strategy]);
        var dialog = new TestDialog();

        var showTask = service.ShowAsync(dialog);
        await strategy.WaitUntilShowingAsync().ConfigureAwait(false);

        service.OpenedDialogs.Should().Contain(dialog);
        service.HasOpenedDialogs.Should().BeTrue();

        strategy.Release();
        await showTask.ConfigureAwait(false);

        service.HasOpenedDialogs.Should().BeFalse();
    }

    [Fact]
    public async Task CloseAsync_CompletesLifecycleAsync()
    {
        var strategy = new BlockingDialogStrategy();
        var service = new ContentDialogService([strategy]);
        var dialog = new TestDialog();

        var showTask = service.ShowAsync(dialog);
        await strategy.WaitUntilShowingAsync().ConfigureAwait(false);

        await service.CloseAsync(dialog).ConfigureAwait(false);
        await showTask.ConfigureAwait(false);

        dialog.ClosedCount.Should().Be(1);
        service.HasOpenedDialogs.Should().BeFalse();
    }

    [Fact]
    public void ShowAsync_ThrowsWhenNoStrategyMatches()
    {
        var service = new ContentDialogService([]);
        var act = () => service.ShowAsync(new TestDialog());

        act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShowAsync_DoesNotMutateCallerDialogOptionsAsync()
    {
        var strategy = new StubDialogStrategy(10, DialogResult.Ok());
        var service = new ContentDialogService([strategy]);
        var dialog = new TestDialog();
        var options = new DialogOptions { Dialog = new TestDialog(), IsModal = true, Title = "Original" };

        await service.ShowAsync(dialog, options).ConfigureAwait(false);

        options.Dialog.Should().NotBeSameAs(dialog);
        options.Title.Should().Be("Original");
    }

    private sealed class StubDialogStrategy(int priority, DialogResult<bool> result) : IDialogStrategy
    {
        public int ShowCount { get; private set; }

        public int Priority => priority;

        public bool CanHandle(IDialog dialog, DialogOptions? options) => true;

        public Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions options, CancellationToken ct = default)
        {
            ShowCount++;
            return Task.FromResult(result);
        }

        public Task CloseAsync(IDialog dialog) => Task.CompletedTask;
    }

    private sealed class BlockingDialogStrategy : IDialogStrategy
    {
        private readonly TaskCompletionSource _showing = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int Priority => 10;

        public bool CanHandle(IDialog dialog, DialogOptions? options) => true;

        public async Task<DialogResult<bool>> ShowAsync(IDialog dialog, DialogOptions options, CancellationToken ct = default)
        {
            _showing.TrySetResult();
            await _release.Task.ConfigureAwait(false);
            return DialogResult.Ok();
        }

        public Task WaitUntilShowingAsync() => _showing.Task;

        public void Release() => _release.TrySetResult();

        public Task CloseAsync(IDialog dialog)
        {
            Release();
            return Task.CompletedTask;
        }
    }

    private class TestDialog : IDialog
    {
        public int ClosedCount { get; private set; }

        public string? Title => "Test";

        public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

        public Task<bool> CanCloseAsync() => Task.FromResult(true);

        public Task OnOpenedAsync() => Task.CompletedTask;

        public Task OnClosedAsync()
        {
            ClosedCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class TestTypedDialog : TestDialog, IDialog<string>
    {
        private readonly TaskCompletionSource<DialogResult<string>> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<DialogResult<string>> GetResultAsync() => _tcs.Task;

        public void Complete(string value) => _tcs.TrySetResult(DialogResult<string>.Success(value));
    }
}
