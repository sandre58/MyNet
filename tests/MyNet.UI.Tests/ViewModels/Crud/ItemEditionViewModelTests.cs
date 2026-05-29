// -----------------------------------------------------------------------
// <copyright file="ItemEditionViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using MyNet.UI.ViewModels.Crud;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Crud;

public class ItemEditionViewModelTests
{
    [Fact]
    public void SetOriginalItem_ThenModifyItem_SetsIsDirty()
    {
        var sut = new TestItemEditionViewModel();
        sut.SetOriginalItem(10);

        sut.SetItem(20);

        sut.IsDirty.Should().BeTrue();
        sut.ApplyCommand.CanExecute(null).Should().BeTrue();
        sut.ResetCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public async Task ApplyAsync_UpdatesOriginal_AndClearsDirty()
    {
        var sut = new TestItemEditionViewModel();
        sut.SetOriginalItem(1);
        sut.SetItem(2);

        await sut.ApplyAsync();

        sut.OriginalItem.Should().Be(2);
        sut.Item.Should().Be(2);
        sut.IsDirty.Should().BeFalse();
    }

    [Fact]
    public async Task ResetAsync_RestoresOriginalItem()
    {
        var sut = new TestItemEditionViewModel();
        sut.SetOriginalItem(5);
        sut.SetItem(99);

        await sut.ResetAsync();

        sut.Item.Should().Be(5);
        sut.IsDirty.Should().BeFalse();
    }

    private sealed class TestItemEditionViewModel : ItemEditionViewModel<int>;
}
