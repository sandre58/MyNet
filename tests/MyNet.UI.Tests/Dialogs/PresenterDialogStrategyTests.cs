// -----------------------------------------------------------------------
// <copyright file="PresenterDialogStrategyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.Dialogs.ContentDialogs;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public class PresenterDialogStrategyTests
{
    [Fact]
    public void CanHandle_ReturnsFalse_WhenNoPresenterMatches()
    {
        var strategy = new PresenterDialogStrategy([]);

        strategy.CanHandle(new SimpleDialog(), null).Should().BeFalse();
    }

    [Fact]
    public async Task ShowAsync_DelegatesToHighestPriorityPresenterAsync()
    {
        var low = new CountingPresenter(priority: 1);
        var high = new CountingPresenter(priority: 5);
        var strategy = new PresenterDialogStrategy([low, high]);
        var dialog = new SimpleDialog();

        await strategy.ShowAsync(dialog, new() { Dialog = dialog }).ConfigureAwait(true);

        high.PresentCount.Should().Be(1);
        low.PresentCount.Should().Be(0);
    }

    private sealed class SimpleDialog : IDialog
    {
        public string? Title => null;

#pragma warning disable CS0067
        public event EventHandler<CloseRequestedEventArgs>? CloseRequested;
#pragma warning restore CS0067

        public Task<bool> CanCloseAsync() => Task.FromResult(true);

        public Task OnOpenedAsync() => Task.CompletedTask;

        public Task OnClosedAsync() => Task.CompletedTask;
    }

    private sealed class CountingPresenter(int priority) : IDialogPresenter
    {
        public int PresentCount { get; private set; }

        public int Priority => priority;

        public bool CanPresent(IDialog dialog, DialogOptions? options) => true;

        public Task<DialogResult<bool>> PresentAsync(IDialog dialog, DialogOptions options, CancellationToken cancellationToken)
        {
            PresentCount++;
            return Task.FromResult(DialogResult.Ok());
        }

        public Task CloseAsync(IDialog dialog) => Task.CompletedTask;
    }
}
