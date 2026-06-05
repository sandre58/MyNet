// -----------------------------------------------------------------------
// <copyright file="VirtualizedListViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using FluentAssertions;
using Moq;
using MyNet.Observable.Collections.Grouping;
using MyNet.UI.ViewModels.List;
using Xunit;
using static System.Reactive.Linq.Observable;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class VirtualizedListViewModelTests
{
    [Fact]
    public void VisibleItems_Should_ReturnWindow_FromStartAndCount()
    {
        var vm = CreateSut(out _);
        vm.VisibleStartIndex = 2;
        vm.VisibleCount = 3;

        vm.VisibleItems.Should().ContainInOrder("C", "D", "E");
    }

    [Fact]
    public void VisibleItems_WhenStartBeyondCount_Should_ClampToLastItem()
    {
        var vm = CreateSut(out _);
        vm.VisibleStartIndex = 50;
        vm.VisibleCount = 10;

        vm.VisibleItems.Should().ContainSingle("F");
    }

    [Fact]
    public void VisibleItems_Should_ReflectUnderlyingItemsCollectionChanges()
    {
        var vm = CreateSut(out var source);
        vm.VisibleStartIndex = 0;
        vm.VisibleCount = 10;

        source.Add("G");

        vm.VisibleItems.Should().Contain("G");
    }

    private static VirtualizedListViewModel<string> CreateSut(out ObservableCollection<string> source)
    {
        source = new(["A", "B", "C", "D", "E", "F"]);

        var providerMock = new Mock<IListDataProvider<string>>();
        providerMock.Setup(p => p.Source).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.FilteredItems).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.Items).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.FilteredCount).Returns(source.Count);
        providerMock.Setup(p => p.ConnectFiltered()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.Connect()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.ConnectGroups()).Returns(Return<IReadOnlyList<CollectionGroup<string>>>([]));

        return new(providerMock.Object);
    }
}
