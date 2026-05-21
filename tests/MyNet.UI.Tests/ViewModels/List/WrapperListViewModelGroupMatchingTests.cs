// -----------------------------------------------------------------------
// <copyright file="WrapperListViewModelGroupMatchingTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.Observable;
using MyNet.Observable.Collections.Wrappers;
using MyNet.UI.ViewModels.List.Wrappers;
using MyNet.Utilities;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class WrapperListViewModelGroupMatchingTests
{
    [Fact]
    public void TwoGenericWrapperViewModel_Should_ExposeWrappers()
    {
        var collection = ExtendedWrapperCollection.From<string, TestWrapper>(["A", "B"], x => new(x), scheduler: null);
        var vm = new WrapperListViewModel<string, TestWrapper>(collection);

        vm.Wrappers.Should().HaveCount(2);
        vm.Wrappers[0].Item.Should().Be("A");
        vm.Wrappers[1].Item.Should().Be("B");

        vm.Dispose();
    }

    [Fact]
    public void GetWrapper_Should_ReturnCachedWrapper_ForItem()
    {
        var collection = ExtendedWrapperCollection.From<string, TestWrapper>(["A"], x => new(x), scheduler: null);
        var vm = new WrapperListViewModel<string, TestWrapper>(collection);

        var wrapper = vm.GetWrapper("A");

        wrapper.Should().NotBeNull();
        wrapper.Item.Should().Be("A");

        vm.Dispose();
    }

    private sealed class TestWrapper(string item) : IWrapper<string>
    {
        public string Item { get; } = item;
    }
}
