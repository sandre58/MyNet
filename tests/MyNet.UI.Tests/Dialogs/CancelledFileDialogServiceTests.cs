// -----------------------------------------------------------------------
// <copyright file="CancelledFileDialogServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.Dialogs.FileDialogs;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public sealed class CancelledFileDialogServiceTests
{
    private readonly CancelledFileDialogService _sut = new();

    [Fact]
    public async Task ShowOpenFileDialogAsync_ReturnsCancelledResultAsync()
    {
        var result = await _sut.ShowOpenFileDialogAsync(OpenFileDialogSettings.Default);

        result.IsCancelled.Should().BeTrue();
        result.Files.Should().BeEmpty();
    }

    [Fact]
    public async Task ShowSaveFileDialogAsync_ReturnsCancelledResultAsync()
    {
        var result = await _sut.ShowSaveFileDialogAsync(SaveFileDialogSettings.Default);

        result.IsCancelled.Should().BeTrue();
    }

    [Fact]
    public async Task ShowFolderDialogAsync_ReturnsCancelledResultAsync()
    {
        var result = await _sut.ShowFolderDialogAsync(OpenFolderDialogSettings.Default);

        result.IsCancelled.Should().BeTrue();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "Testing cancellation behavior")]
    public async Task ShowOpenFileDialogAsync_WhenCancelled_ThrowsOperationCanceledExceptionAsync()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await _sut.ShowOpenFileDialogAsync(OpenFileDialogSettings.Default, cts.Token).ConfigureAwait(false);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
