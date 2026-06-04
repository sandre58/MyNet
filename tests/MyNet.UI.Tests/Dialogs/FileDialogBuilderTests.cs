// -----------------------------------------------------------------------
// <copyright file="FileDialogBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.FileDialogs;
using Xunit;

namespace MyNet.UI.Tests.Dialogs;

public sealed class FileDialogBuilderTests
{
    [Fact]
    public void SaveFile_Build_AppliesSettings()
    {
        var dialogService = new Mock<IDialogService>();

        var settings = dialogService.Object.SaveFile()
            .WithFileName("export.csv")
            .WithInitialDirectory(@"C:\temp")
            .WithFilters("CSV|*.csv")
            .WithDefaultExtension("csv")
            .WithOverwritePrompt()
            .Build();

        settings.FileName.Should().Be("export.csv");
        settings.InitialDirectory.Should().Be(@"C:\temp");
        settings.Filters.Should().Be("CSV|*.csv");
        settings.DefaultExtension.Should().Be("csv");
        settings.OverwritePrompt.Should().BeTrue();
    }

    [Fact]
    public async Task SaveFile_PickAsync_CallsDialogServiceAsync()
    {
        var dialogService = new Mock<IDialogService>();
        var expected = new FileDialogResult { Files = ["C:\\out.csv"] };

        dialogService
            .Setup(x => x.ShowSaveFileDialogAsync(It.IsAny<SaveFileDialogSettings>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await dialogService.Object.SaveFile()
            .WithFileName("out")
            .PickAsync();

        result.Should().BeSameAs(expected);
        dialogService.Verify(
            x => x.ShowSaveFileDialogAsync(
                It.Is<SaveFileDialogSettings>(s => s.FileName == "out"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void OpenFile_WithMultiselect_SetsSetting()
    {
        var dialogService = new Mock<IDialogService>();

        var settings = dialogService.Object.OpenFile()
            .WithMultiselect()
            .Build();

        settings.Multiselect.Should().BeTrue();
    }
}
