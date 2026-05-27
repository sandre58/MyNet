// -----------------------------------------------------------------------
// <copyright file="SelectionManagerCompositionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using MyNet.Observable.Collections;
using MyNet.Observable.Collections.Sources;
using MyNet.UI.ViewModels.List.Selection;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.List;

/// <summary>
/// Validates that <see cref="SelectableListViewModel{T}"/> delegates selection to <see cref="ISelectionManager{T}"/>.
/// </summary>
public sealed class SelectionManagerCompositionTests : IDisposable
{
    private readonly Mock<ISelectionManager<string>> _managerMock;
    private readonly SelectableListViewModel<string> _vm;

    public SelectionManagerCompositionTests()
    {
        _managerMock = new();
        _managerMock.Setup(m => m.SelectedItems).Returns([]);
        _managerMock.Setup(m => m.SelectedCount).Returns(0);
        _managerMock.Setup(m => m.IsSelected(It.IsAny<string>())).Returns(false);

        var source = SourceEngine<string>.From(["Alpha", "Beta", "Gamma"], readOnly: false);
        var collection = new ExtendedCollection<string>(source);

        _vm = new(collection, _managerMock.Object);
    }

    [Fact]
    public void Select_Should_Delegate_To_Manager()
    {
        _vm.Select("Alpha");

        _managerMock.Verify(m => m.Select("Alpha"), Times.Once);
    }

    [Fact]
    public void Unselect_Should_Delegate_To_Manager()
    {
        _vm.Unselect("Beta");

        _managerMock.Verify(m => m.Unselect("Beta"), Times.Once);
    }

    [Fact]
    public void Toggle_Should_Delegate_To_Manager()
    {
        _vm.Toggle("Beta");

        _managerMock.Verify(m => m.Toggle("Beta"), Times.Once);
    }

    [Fact]
    public void ClearSelection_Should_Delegate_To_Manager()
    {
        _vm.ClearSelection();

        _managerMock.Verify(m => m.ClearSelection(), Times.Once);
    }

    [Fact]
    public void SetSelection_Should_Delegate_To_Manager()
    {
        var items = new List<string> { "Alpha", "Beta" };

        _vm.SetSelection(items);

        _managerMock.Verify(m => m.SetSelection(It.Is<IEnumerable<string>>(e => e.SequenceEqual(items))), Times.Once);
    }

    [Fact]
    public void IsSelected_Should_Delegate_To_Manager()
    {
        _managerMock.Setup(m => m.IsSelected("Alpha")).Returns(true);

        _vm.IsSelected("Alpha").Should().BeTrue();
    }

    [Fact]
    public void SelectedItems_Should_Come_From_Manager()
    {
        _managerMock.Setup(m => m.SelectedItems).Returns(["Alpha"]);

        _vm.SelectedItems.Should().ContainSingle("Alpha");
    }

    [Fact]
    public void SelectedCount_Should_Come_From_Manager()
    {
        _managerMock.Setup(m => m.SelectedCount).Returns(2);

        _vm.SelectedCount.Should().Be(2);
    }

    [Fact]
    public void SelectionChanged_RaisedByManager_Should_NotifySelectedItemsProperties()
    {
        var notified = new List<string?>();
        _vm.PropertyChanged += (_, e) => notified.Add(e.PropertyName);

        _managerMock.Raise(m => m.SelectionChanged += null, EventArgs.Empty);

        notified.Should().Contain(nameof(_vm.SelectedItems));
        notified.Should().Contain(nameof(_vm.SelectedCount));
        notified.Should().Contain(nameof(_vm.SelectedItem));
    }

    [Fact]
    public void Dispose_Should_Unsubscribe_And_Dispose_Manager()
    {
        _vm.Dispose();

        _managerMock.Verify(m => m.Dispose(), Times.Once);
    }

    public void Dispose() => _vm.Dispose();
}
