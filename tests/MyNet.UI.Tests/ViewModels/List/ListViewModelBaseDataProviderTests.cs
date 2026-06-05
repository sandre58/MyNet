// -----------------------------------------------------------------------
// <copyright file="ListViewModelBaseDataProviderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using FluentAssertions;
using Moq;
using MyNet.Observable.Collections.Filters;
using MyNet.Observable.Collections.Grouping;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Filtering;
using Xunit;
using static System.Reactive.Linq.Observable;

namespace MyNet.UI.Tests.ViewModels.List;

/// <summary>
/// P0 Test 1 – IListDataProvider abstraction.
/// Validates that ListViewModelBase works with any IListDataProvider implementation
/// (testability: allows mocking the pipeline).
/// </summary>
public sealed class ListViewModelBaseDataProviderTests : IDisposable
{
    private readonly Mock<IListDataProvider<string>> _providerMock;
    private readonly ObservableCollection<string> _sourceItems = ["A", "B", "C"];
    private readonly ObservableCollection<string> _filteredItems = ["A", "B"];
    private readonly ConcreteListViewModel _vm;

    public ListViewModelBaseDataProviderTests()
    {
        _providerMock = new();
        _providerMock.Setup(p => p.Source).Returns(new ReadOnlyObservableCollection<string>(_sourceItems));
        _providerMock.Setup(p => p.FilteredItems).Returns(new ReadOnlyObservableCollection<string>(_filteredItems));
        _providerMock.Setup(p => p.Items).Returns(new ReadOnlyObservableCollection<string>(_filteredItems));
        _providerMock.Setup(p => p.FilteredCount).Returns(_filteredItems.Count);
        _providerMock.Setup(p => p.ConnectFiltered()).Returns(Empty<IChangeSet<string>>());
        _providerMock.Setup(p => p.Connect()).Returns(Empty<IChangeSet<string>>());
        _providerMock.Setup(p => p.ConnectGroups()).Returns(Return<IReadOnlyList<CollectionGroup<string>>>([]));

        _vm = new(_providerMock.Object);
    }

    [Fact]
    public void Source_Should_ReturnProviderSource()
    {
        _vm.Source.Should().HaveCount(3);
        _vm.Source.Should().ContainInOrder("A", "B", "C");
    }

    [Fact]
    public void FilteredItems_Should_ReturnProviderFilteredItems() => _vm.FilteredItems.Should().HaveCount(2);

    [Fact]
    public void FilteredCount_Should_ReturnProviderFilteredCount() => _vm.FilteredCount.Should().Be(2);

    [Fact]
    public void Items_Should_ReturnProviderItems() => _vm.Items.Should().HaveCount(2);

    [Fact]
    public void TotalCount_Should_ReflectSourceCount() => _vm.TotalCount.Should().Be(3);

    [Fact]
    public void WhenFiltersViewModelIsNull_ClearFilter_Should_NotBeCalledOnRefresh()
        => _providerMock.Verify(p => p.ClearFilter(), Times.Never);

    [Fact]
    public void WhenFilterChanged_Provider_SetFilter_Should_BeCalled()
    {
        // Arrange
        var filtersMock = new Mock<IFiltersViewModel<string>>();
        var filter = new Mock<IFilter<string>>().Object;
        filtersMock.Setup(f => f.AutoApply).Returns(true);

        // Au début, pas de filtre
        IFilter<string>? current = null;
        filtersMock.SetupGet(f => f.CurrentFilter).Returns(() => current);

        // Création du ViewModel avec le mock de filtre (CurrentFilter = null)
        var vm = new ConcreteListViewModel(_providerMock.Object, filtersMock.Object);

        // On change le filtre juste avant de lever l'événement
        current = filter;
        IFilter<string>? calledFilter = null;
        _providerMock.Setup(p => p.SetFilter(It.IsAny<IFilter<string>>()))
            .Callback<IFilter<string>>(f => calledFilter = f);
        filtersMock.Raise(f => f.FiltersChanged += null, new FiltersChangedEventArgs<string>(filter));

        try
        {
            _providerMock.Verify(p => p.SetFilter(filter), Times.AtLeastOnce);
        }
        catch (MockException)
        {
            throw new Xunit.Sdk.XunitException($"SetFilter was appelé avec : {calledFilter}, attendu : {filter}");
        }

        vm.Dispose();
    }

    public void Dispose() => _vm.Dispose();

    // Minimal concrete subclass (ListViewModelBase<T> has protected constructor)
    private sealed class ConcreteListViewModel(IListDataProvider<string> provider, IFiltersViewModel<string>? filters = null)
        : ListViewModelBase<string>(provider, filters);
}
