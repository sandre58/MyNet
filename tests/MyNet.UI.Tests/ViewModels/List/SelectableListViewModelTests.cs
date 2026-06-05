// -----------------------------------------------------------------------
// <copyright file="SelectableListViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Selection;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Paging;
using MyNet.UI.ViewModels.List.Selection;
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

    [Fact]
    public void Paging_Should_ExposeOnlyCurrentPageInItems()
    {
        using var vm = ListViewModelFactory.CreateSelection(
            Enumerable.Range(1, 250).Select(x => x.ToString(CultureInfo.CurrentCulture)),
            new() { Paging = new PagingViewModel(100) });

        vm.Paging!.TotalItems.Should().Be(250);
        vm.FilteredCount.Should().Be(250);
        vm.FilteredItems.Should().HaveCount(250);
        vm.Items.Should().HaveCount(100);
        vm.Items.Should().Equal(Enumerable.Range(1, 100).Select(x => x.ToString(CultureInfo.CurrentCulture)));

        vm.Paging.MoveToPage(2);

        vm.Paging.CurrentPage.Should().Be(2);
        vm.Items.Should().HaveCount(100);
        vm.Items.First().Should().Be("101");
    }

    [Fact]
    public void Paging_FromReadOnlyLargeCollection_Should_ExposeOnlyCurrentPageInItems()
    {
        var collection = ExtendedCollection.FromReadOnly(Enumerable.Range(1, 7461).Select(x => x.ToString(CultureInfo.CurrentCulture)));
        using var vm = ListViewModelFactory.CreateSelection(
            collection,
            new() { Paging = new PagingViewModel(100) });

        vm.Paging!.TotalItems.Should().Be(7461);
        vm.Items.Should().HaveCount(100);
    }

    [Fact]
    public void RequestPipelineRefresh_WithAdHocFilter_Should_PreserveFilterAndPaging()
    {
        using var vm = new AdHocFilterListViewModel(
            ExtendedCollection.From(["A1", "A2", "B1", "B2", "B3"]),
            new() { Paging = new PagingViewModel(2) });

        vm.SetPrefixFilter("B");
        vm.RequestPipelineRefresh();

        vm.Paging!.TotalItems.Should().Be(3);
        vm.Items.Should().Equal("B1", "B2");
        vm.Paging.CurrentPage.Should().Be(1);
    }

    [Fact]
    public void RequestPipelineRefresh_WhenCurrentPageExceedsTotalPages_Should_ClampPageAndItems()
    {
        using var vm = new AdHocFilterListViewModel(
            ExtendedCollection.From(Enumerable.Range(1, 250).Select(x => x.ToString(CultureInfo.CurrentCulture))),
            new() { Paging = new PagingViewModel(100) });

        vm.Paging!.MoveToPage(3);
        vm.Paging.CurrentPage.Should().Be(3);

        vm.SetNumericMaxFilter(50);
        vm.RequestPipelineRefresh();

        vm.Paging.TotalItems.Should().Be(50);
        vm.Paging.CurrentPage.Should().Be(1);
        vm.Items.Should().HaveCount(50);
        vm.Items.First().Should().Be("1");
        vm.Items.Last().Should().Be("50");
    }

    private sealed class AdHocFilterListViewModel(
        ExtendedCollection<string> collection,
        ListViewModelOptions<string>? options)
        : SelectableListViewModel<string>(collection, SelectionMode.Single, options)
    {
        public void SetPrefixFilter(string prefix)
            => DataProvider.SetFilter(new ExpressionFilter<string>(x => x.StartsWith(prefix, StringComparison.Ordinal)));

        public void SetNumericMaxFilter(int max)
            => DataProvider.SetFilter(new ExpressionFilter<string>(x => int.Parse(x, CultureInfo.InvariantCulture) <= max));

        public new void RequestPipelineRefresh() => base.RequestPipelineRefresh();
    }
}
