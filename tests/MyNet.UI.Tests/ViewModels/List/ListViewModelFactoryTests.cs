// -----------------------------------------------------------------------
// <copyright file="ListViewModelFactoryTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using FluentAssertions;
using Moq;
using MyNet.Observable.Collections.Grouping;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Factories;
using MyNet.UI.ViewModels.List.Services;
using Xunit;
using static System.Reactive.Linq.Observable;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class ListViewModelFactoryTests
{
    [Fact]
    public void CreateCrud_WithProvider_Should_ReturnCrudListViewModel()
    {
        var providerMock = CreateProvider(["A", "B"]);
        var crudMock = new Mock<ICrudService<string>>();

        var vm = ListViewModelFactory.CreateCrud(providerMock.Object, crudMock.Object);

        vm.Should().BeOfType<CrudListViewModel<string>>();
        vm.Items.Should().HaveCount(2);
        vm.Dispose();
    }

    [Fact]
    public void CreateCrud_WithItems_Should_ReturnCrudListViewModel()
    {
        var crudService = new NoOpCrudService<string>();

        var vm = ListViewModelFactory.CreateCrud(["A", "B", "C"], crudService);

        vm.Should().BeOfType<CrudListViewModel<string>>();
        vm.Source.Should().HaveCount(3);
        vm.Dispose();
    }

    [Fact]
    public void CreateCrud_WithNullProvider_Should_Throw()
    {
        var action = () => ListViewModelFactory.CreateCrud(provider: null!, new NoOpCrudService<string>());

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateCrud_WithNullItems_Should_Throw()
    {
        var action = () => ListViewModelFactory.CreateCrud(items: null!, new NoOpCrudService<string>());

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateCrud_WithNullCrudService_Should_Throw()
    {
        var providerMock = CreateProvider(["A"]);

        var action = () => ListViewModelFactory.CreateCrud(providerMock.Object, crudService: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateVirtualized_WithProvider_Should_ReturnVirtualizedListViewModel()
    {
        var providerMock = CreateProvider(["A", "B"]);

        var vm = ListViewModelFactory.CreateVirtualized(providerMock.Object);

        vm.Should().BeOfType<VirtualizedListViewModel<string>>();
        vm.Items.Should().HaveCount(2);
        vm.Dispose();
    }

    [Fact]
    public void CreateVirtualized_WithItems_Should_ReturnVirtualizedListViewModel()
    {
        var vm = ListViewModelFactory.CreateVirtualized(["A", "B", "C"]);

        vm.Should().BeOfType<VirtualizedListViewModel<string>>();
        vm.Source.Should().HaveCount(3);
        vm.Dispose();
    }

    [Fact]
    public void CreateVirtualized_WithNullProvider_Should_Throw()
    {
        var action = () => ListViewModelFactory.CreateVirtualized<string>(provider: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateVirtualized_WithNullItems_Should_Throw()
    {
        var action = () => ListViewModelFactory.CreateVirtualized<string>(items: null!);

        action.Should().Throw<ArgumentNullException>();
    }

    private static Mock<IListDataProvider<string>> CreateProvider(IEnumerable<string> items)
    {
        var source = new ObservableCollection<string>(items);
        var providerMock = new Mock<IListDataProvider<string>>();
        providerMock.Setup(p => p.Source).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.Items).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.Connect()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.ConnectGroups()).Returns(Return<IReadOnlyList<CollectionGroup<string>>>([]));
        return providerMock;
    }
}
