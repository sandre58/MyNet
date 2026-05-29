// -----------------------------------------------------------------------
// <copyright file="MessageBoxServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.ContentDialogs;
using MyNet.UI.Dialogs.MessageBox;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public class MessageBoxServiceTests
{
    [Fact]
    public async Task ShowAsync_Headless_ReturnsDefaultResultForOkDialogAsync()
    {
        var services = new ServiceCollection();
        services.AddDialogs();
        await using var provider = services.BuildServiceProvider();

        var messageBox = provider.GetRequiredService<IMessageBoxService>();

        var result = await messageBox.ShowAsync(
            "Hello",
            "Title",
            MessageSeverity.Warning).ConfigureAwait(true);

        result.Should().Be(MessageBoxResult.Ok);
    }

    [Fact]
    public async Task ShowAsync_Headless_YesNoDialog_ReturnsDefaultYesAsync()
    {
        var services = new ServiceCollection();
        services.AddDialogs();
        await using var provider = services.BuildServiceProvider();

        var messageBox = provider.GetRequiredService<IMessageBoxService>();

        var result = await messageBox.ShowAsync(
            "Continue?",
            buttons: MessageBoxResultOption.YesNo,
            defaultResult: MessageBoxResult.Yes).ConfigureAwait(true);

        result.Should().Be(MessageBoxResult.Yes);
    }

    [Fact]
    public async Task ShowAsync_WithPresenter_ReturnsPresenterResultAsync()
    {
        var services = new ServiceCollection();
        services.AddDialogs(b => b.AddPresenter<StubMessageBoxPresenter>());
        await using var provider = services.BuildServiceProvider();

        var messageBox = provider.GetRequiredService<IMessageBoxService>();

        var result = await messageBox.ShowAsync("Confirm", buttons: MessageBoxResultOption.YesNo).ConfigureAwait(true);

        result.Should().Be(MessageBoxResult.No);
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a type parameter for testing.")]
    private sealed class StubMessageBoxPresenter : IDialogPresenter
    {
        public int Priority => 100;

        public bool CanPresent(IDialog dialog, DialogOptions? options) => true;

        public Task<DialogResult<bool>> PresentAsync(
            IDialog dialog,
            DialogOptions options,
            CancellationToken cancellationToken)
        {
            if (dialog is MessageBoxViewModel messageBox)
            {
                messageBox.ApplyResult(MessageBoxResult.No);
            }

            return Task.FromResult(DialogResult.Ok());
        }

        public Task CloseAsync(IDialog dialog) => Task.CompletedTask;
    }
}
