// -----------------------------------------------------------------------
// <copyright file="SelectableListViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Observable.Collections.Selection;
using MyNet.UI.ViewModels.List.Factories;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class SelectableListViewModelTests
{
    [Fact]
    public void CreateSelection_Should_ExposeItemsAndSelection()
    {
        using var vm = ListViewModelFactory.CreateSelection(["A", "B", "C"]);

        vm.Items.Should().HaveCount(3);
        vm.Select("B");
        vm.IsSelected("B").Should().BeTrue();
        vm.SelectedItems.Should().ContainSingle("B");
    }

    [Fact]
    public void SingleSelection_Should_KeepOneItem()
    {
        using var vm = ListViewModelFactory.CreateSelection(
            ["A", "B"],
            selectionMode: SelectionMode.Single);

        vm.Select("A");
        vm.Select("B");

        vm.SelectedCount.Should().Be(1);
        vm.SelectedItem.Should().Be("B");
    }

    [Fact]
    public void Dispose_Should_NotThrow()
    {
        var vm = ListViewModelFactory.CreateSelection(["A"]);
        var act = vm.Dispose;
        act.Should().NotThrow();
    }
}
