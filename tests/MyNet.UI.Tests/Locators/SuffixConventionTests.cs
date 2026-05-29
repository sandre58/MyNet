// -----------------------------------------------------------------------
// <copyright file="SuffixConventionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Locators.Conventions;
using MyNet.UI.Tests.ViewModels;
using MyNet.UI.Tests.Views;
using Xunit;

namespace MyNet.UI.Tests.Locators;

public class SuffixConventionTests
{
    private readonly SuffixConvention _sut = new();

    // --- ViewModel → View ---
    [Fact]
    public void Resolve_ViewModel_ReturnsView()
    {
        var result = _sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
    }

    [Fact]
    public void Resolve_ViewModel_ReturnsControl_WhenNoViewExists()
    {
        // DashboardControl/Window/Page/Fragment exist — suffix iteration finds the first available one
        var result = _sut.Resolve(typeof(DashboardViewModel));

        result.Should().NotBeNull();
        var expected = new[] { typeof(DashboardControl), typeof(DashboardWindow), typeof(DashboardPage), typeof(DashboardFragment) };
        expected.Should().Contain(result);
    }

    [Fact]
    public void Resolve_ViewModel_ReturnsNull_WhenNoViewFound()
    {
        var result = _sut.Resolve(typeof(ItemViewModel));

        // ItemView exists → should be resolved
        result.Should().Be<ItemView>();
    }

    // --- View → ViewModel ---
    [Fact]
    public void Resolve_View_ReturnsViewModel()
    {
        var result = _sut.Resolve(typeof(PersonView));

        result.Should().Be<PersonViewModel>();
    }

    [Fact]
    public void Resolve_View_Control_ReturnsViewModel()
    {
        var result = _sut.Resolve(typeof(DashboardControl));

        result.Should().Be<DashboardViewModel>();
    }

    [Fact]
    public void Resolve_NonViewModelNonView_ReturnsNull()
    {
        var result = _sut.Resolve(typeof(string));

        result.Should().BeNull();
    }

    [Fact]
    public void Resolve_TypeWithoutNamespace_ReturnsNull()
    {
        // A type with no namespace should not throw, just return null
        var result = _sut.Resolve(typeof(NoNamespaceType));

        result.Should().BeNull();
    }
}

/// <summary>Type at global namespace to test null-namespace path.</summary>
internal sealed class NoNamespaceType;
