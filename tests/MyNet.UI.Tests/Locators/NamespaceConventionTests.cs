// -----------------------------------------------------------------------
// <copyright file="NamespaceConventionTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Locators.Conventions;
using MyNet.UI.Tests.ViewModels;
using MyNet.UI.Tests.Views;
using Xunit;

namespace MyNet.UI.Tests.Locators;

public class NamespaceConventionTests
{
    private readonly NamespaceConvention _sut = new();

    [Fact]
    public void Resolve_ViewModel_ReturnsView()
    {
        var result = _sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
    }

    [Fact]
    public void Resolve_View_ReturnsViewModel()
    {
        var result = _sut.Resolve(typeof(PersonView));

        result.Should().Be<PersonViewModel>();
    }

    [Fact]
    public void Resolve_NonViewModelNonView_ReturnsNull()
    {
        var result = _sut.Resolve(typeof(string));

        result.Should().BeNull();
    }
}

public class ParentNamespaceConventionTests
{
    private readonly ParentNamespaceConvention _sut = new();

    [Fact]
    public void Resolve_ViewModel_ReturnsView()
    {
        // Parent of "MyNet.UI.Tests.ViewModels" = "MyNet.UI.Tests"
        // Looks for "MyNet.UI.Tests.Views.PersonView"
        var result = _sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
    }

    [Fact]
    public void Resolve_View_ReturnsViewModel()
    {
        // Parent of "MyNet.UI.Tests.Views" = "MyNet.UI.Tests"
        // Looks for "MyNet.UI.Tests.ViewModels.PersonViewModel"
        var result = _sut.Resolve(typeof(PersonView));

        result.Should().Be<PersonViewModel>();
    }
}

public class AssemblyRootConventionTests
{
    // Assembly name = "MyNet.UI.Tests", subNamespace = "Views"
    // Looks for "MyNet.UI.Tests.Views.PersonView"
    private readonly AssemblyRootConvention _sut = new("Views");

    [Fact]
    public void Resolve_ViewModel_ReturnsView()
    {
        var result = _sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
    }

    [Fact]
    public void Resolve_View_ReturnsViewModel()
    {
        // Looks for "MyNet.UI.Tests.ViewModels.PersonViewModel"
        var result = _sut.Resolve(typeof(PersonView));

        result.Should().Be<PersonViewModel>();
    }
}
