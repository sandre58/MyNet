// -----------------------------------------------------------------------
// <copyright file="CrudListViewModelTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using FluentAssertions;
using Moq;
using MyNet.Observable.Collections.Grouping;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.List.Services;
using Xunit;
using static System.Reactive.Linq.Observable;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class CrudListViewModelTests
{
    [Fact]
    public async Task Add_Should_CallService_And_InvokeAddedHookAsync()
    {
        var (vm, crud) = CreateSut();
        crud.Setup(x => x.CreateAsync(CancellationToken.None)).ReturnsAsync("Created");

        await vm.InvokeAddAsync();

        crud.Verify(x => x.CreateAsync(CancellationToken.None), Times.Once);
        vm.AddedItems.Should().ContainSingle("Created");
    }

    [Fact]
    public async Task Edit_Should_CallService_And_InvokeEditedHook_WhenSuccessAsync()
    {
        var (vm, crud) = CreateSut();
        crud.Setup(x => x.UpdateAsync("A", CancellationToken.None)).ReturnsAsync(true);

        await vm.InvokeEditAsync("A");

        crud.Verify(x => x.UpdateAsync("A", CancellationToken.None), Times.Once);
        vm.EditedItems.Should().ContainSingle("A");
    }

    [Fact]
    public async Task Delete_Should_CallService_And_InvokeDeletedHook_WhenSuccessAsync()
    {
        var (vm, crud) = CreateSut();
        crud.Setup(x => x.DeleteAsync("A", CancellationToken.None)).ReturnsAsync(true);

        await vm.InvokeDeleteAsync("A");

        crud.Verify(x => x.DeleteAsync("A", CancellationToken.None), Times.Once);
        vm.DeletedItems.Should().ContainSingle("A");
    }

    [Fact]
    public async Task DeleteRange_Should_CallService_Once_And_InvokeDeletedHook_ForEachItemAsync()
    {
        var (vm, crud) = CreateSut();
        var items = new[] { "A", "B" };
        crud.Setup(x => x.DeleteRangeAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None)).ReturnsAsync(true);

        await vm.InvokeDeleteRangeAsync(items);

        crud.Verify(x => x.DeleteRangeAsync(It.Is<IEnumerable<string>>(_ => true), CancellationToken.None), Times.Once);
        vm.DeletedItems.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task DefaultCrudHooks_Should_RequestPipelineRefreshAsync()
    {
        var source = new ObservableCollection<string>(["A", "B", "C"]);
        var providerMock = new Mock<IListDataProvider<string>>();
        providerMock.Setup(p => p.Source).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.FilteredItems).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.Items).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.FilteredCount).Returns(source.Count);
        providerMock.Setup(p => p.ConnectFiltered()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.Connect()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.ConnectGroups()).Returns(Return<IReadOnlyList<CollectionGroup<string>>>([]));

        var refreshCalls = 0;
        providerMock.Setup(p => p.ClearFilter()).Callback(() => refreshCalls++);
        providerMock.Setup(p => p.ClearSorting()).Callback(() => refreshCalls++);
        providerMock.Setup(p => p.ClearGrouping()).Callback(() => refreshCalls++);

        var crudMock = new Mock<ICrudService<string>>();
        crudMock.Setup(x => x.CreateAsync(CancellationToken.None)).ReturnsAsync("Created");

        var vm = new DefaultHookCrudListViewModel(providerMock.Object, crudMock.Object);
        refreshCalls = 0;

        await vm.InvokeAddAsync();

        refreshCalls.Should().BeGreaterThan(0);
        vm.Dispose();
    }

    private static (TestableCrudListViewModel Vm, Mock<ICrudService<string>> Crud) CreateSut()
    {
        var source = new ObservableCollection<string>(["A", "B", "C"]);
        var providerMock = new Mock<IListDataProvider<string>>();
        providerMock.Setup(p => p.Source).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.FilteredItems).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.Items).Returns(new ReadOnlyObservableCollection<string>(source));
        providerMock.Setup(p => p.FilteredCount).Returns(source.Count);
        providerMock.Setup(p => p.ConnectFiltered()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.Connect()).Returns(Empty<IChangeSet<string>>());
        providerMock.Setup(p => p.ConnectGroups()).Returns(Return<IReadOnlyList<CollectionGroup<string>>>([]));

        var crudMock = new Mock<ICrudService<string>>();
        var vm = new TestableCrudListViewModel(providerMock.Object, crudMock.Object);

        return (vm, crudMock);
    }

    private sealed class TestableCrudListViewModel(IListDataProvider<string> provider, ICrudService<string> crudService)
        : CrudListViewModel<string>(provider, crudService)
    {
        public List<string> AddedItems { get; } = [];

        public List<string> EditedItems { get; } = [];

        public List<string> DeletedItems { get; } = [];

        public Task InvokeAddAsync() => AddAsync();

        public Task InvokeEditAsync(string item) => EditAsync(item);

        public Task InvokeDeleteAsync(string item) => DeleteAsync(item);

        public Task InvokeDeleteRangeAsync(IEnumerable<string> items) => DeleteRangeAsync(items);

        protected override Task OnItemAddedAsync(string item, CancellationToken cancellationToken = default)
        {
            AddedItems.Add(item);
            return Task.CompletedTask;
        }

        protected override Task OnItemEditedAsync(string item, CancellationToken cancellationToken = default)
        {
            EditedItems.Add(item);
            return Task.CompletedTask;
        }

        protected override Task OnItemDeletedAsync(string item, CancellationToken cancellationToken = default)
        {
            DeletedItems.Add(item);
            return Task.CompletedTask;
        }
    }

    private sealed class DefaultHookCrudListViewModel(IListDataProvider<string> provider, ICrudService<string> crudService)
        : CrudListViewModel<string>(provider, crudService)
    {
        public Task InvokeAddAsync() => AddAsync();
    }
}
