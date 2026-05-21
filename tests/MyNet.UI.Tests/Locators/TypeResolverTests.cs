// -----------------------------------------------------------------------
// <copyright file="TypeResolverTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Moq;
using MyNet.UI.Locators.Conventions;
using MyNet.UI.Tests.ViewModels;
using MyNet.UI.Tests.Views;
using Xunit;

namespace MyNet.UI.Tests.Locators;

public class TypeResolverTests
{
    // --- Manual registration ---
    [Fact]
    public void Register_ManualMapping_OverridesConvention()
    {
        var sut = new TypeResolver([]);
        sut.Register(typeof(PersonViewModel), typeof(PersonView));

        var result = sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
    }

    [Fact]
    public void Register_ManualMapping_InvalidatesCache()
    {
        var sut = new TypeResolver([]);

        // First resolve → null (no convention, no mapping)
        sut.Resolve(typeof(PersonViewModel)).Should().BeNull();

        // Register manually
        sut.Register(typeof(PersonViewModel), typeof(PersonView));

        // Second resolve → should pick up the new manual mapping
        sut.Resolve(typeof(PersonViewModel)).Should().Be<PersonView>();
    }

    // --- Convention resolution ---
    [Fact]
    public void Resolve_UsesFirstMatchingConvention()
    {
        var convention = new SuffixConvention();
        var sut = new TypeResolver([convention]);

        var result = sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
    }

    [Fact]
    public void Resolve_ReturnsNull_WhenNoConventionMatches()
    {
        var sut = new TypeResolver([]);

        var result = sut.Resolve(typeof(string));

        result.Should().BeNull();
    }

    // --- Cache ---
    [Fact]
    public void Resolve_CachesResult_ConventionCalledOnce()
    {
        var mockConvention = new Mock<ITypeNamingConvention>();
        mockConvention.Setup(c => c.Resolve(It.IsAny<Type>())).Returns(typeof(PersonView));

        var sut = new TypeResolver([mockConvention.Object]);

        sut.Resolve(typeof(PersonViewModel));
        sut.Resolve(typeof(PersonViewModel));

        // Convention should be called only once due to caching
        mockConvention.Verify(c => c.Resolve(typeof(PersonViewModel)), Times.Once);
    }

    [Fact]
    public void Resolve_CachesNullResult_ConventionNotCalledAgain()
    {
        var mockConvention = new Mock<ITypeNamingConvention>();
        mockConvention.Setup(c => c.Resolve(It.IsAny<Type>())).Returns((Type?)null);

        var sut = new TypeResolver([mockConvention.Object]);

        sut.Resolve(typeof(string));
        sut.Resolve(typeof(string));

        mockConvention.Verify(c => c.Resolve(typeof(string)), Times.Once);
    }

    // --- ClearCache ---
    [Fact]
    public void ClearCache_ForcesReEvaluation()
    {
        var callCount = 0;
        var mockConvention = new Mock<ITypeNamingConvention>();
        mockConvention
            .Setup(c => c.Resolve(It.IsAny<Type>()))
            .Returns(() =>
            {
                callCount++;
                return callCount == 1 ? null : typeof(PersonView);
            });

        var sut = new TypeResolver([mockConvention.Object]);

        sut.Resolve(typeof(PersonViewModel)).Should().BeNull(); // first call: null, cached

        sut.ClearCache();

        sut.Resolve(typeof(PersonViewModel)).Should().Be<PersonView>(); // second call: view
    }

    [Fact]
    public void ClearCache_PreservesManualRegistrations()
    {
        var sut = new TypeResolver([]);
        sut.Register(typeof(PersonViewModel), typeof(PersonView));

        sut.ClearCache();

        // Manual mapping should still work after cache clear
        sut.Resolve(typeof(PersonViewModel)).Should().Be<PersonView>();
    }

    // --- Convention ordering ---
    [Fact]
    public void Resolve_UsesFirstConventionThatMatches_InOrder()
    {
        var firstConvention = new Mock<ITypeNamingConvention>();
        firstConvention.Setup(c => c.Resolve(typeof(PersonViewModel))).Returns(typeof(PersonView));

        var secondConvention = new Mock<ITypeNamingConvention>();
        secondConvention.Setup(c => c.Resolve(typeof(PersonViewModel))).Returns(typeof(ItemView));

        var sut = new TypeResolver([firstConvention.Object, secondConvention.Object]);

        var result = sut.Resolve(typeof(PersonViewModel));

        result.Should().Be<PersonView>();
        secondConvention.Verify(c => c.Resolve(It.IsAny<Type>()), Times.Never);
    }
}
