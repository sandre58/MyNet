// -----------------------------------------------------------------------
// <copyright file="NamingConventionHelpersTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.Locators.Conventions;
using Xunit;

namespace MyNet.UI.Tests.Locators;

public class NamingConventionHelpersTests
{
    [Fact]
    public void ReplaceNamespaceSegment_ReplacesExactSegmentOnly()
    {
        var result = NamingConventionHelpers.ReplaceNamespaceSegment(
            "MyApp.ViewModels.Features",
            "ViewModels",
            "Views");

        result.Should().Be("MyApp.Views.Features");
    }

    [Fact]
    public void ReplaceNamespaceSegment_DoesNotReplaceSubstringInsideSegment()
    {
        var result = NamingConventionHelpers.ReplaceNamespaceSegment(
            "MyApp.SharedViewModels.Core",
            "ViewModels",
            "Views");

        result.Should().Be("MyApp.SharedViewModels.Core");
    }

    [Fact]
    public void ReplaceNamespaceSegment_ReplacesAllMatchingSegments()
    {
        var result = NamingConventionHelpers.ReplaceNamespaceSegment(
            "MyApp.ViewModels.Sub.ViewModels",
            "ViewModels",
            "Views");

        result.Should().Be("MyApp.Views.Sub.Views");
    }

    [Fact]
    public void ReplaceNamespaceSegment_ReturnsNull_WhenInputIsNull()
    {
        NamingConventionHelpers.ReplaceNamespaceSegment(null, "ViewModels", "Views")
            .Should().BeNull();
    }
}
