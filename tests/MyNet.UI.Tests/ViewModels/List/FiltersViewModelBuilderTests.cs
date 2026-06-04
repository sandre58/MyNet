// -----------------------------------------------------------------------
// <copyright file="FiltersViewModelBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MyNet.Primitives;
using MyNet.UI.ViewModels.List.Filtering;
using MyNet.UI.ViewModels.List.Filtering.Filters;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.List;

public sealed class FiltersViewModelBuilderTests
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used as a simple data structure for testing purposes.")]
    private sealed record Item(string Name, string Path);

    [Fact]
    public void Create_BuildsOrGroupWithStringFilters()
    {
        var filters = FiltersViewModelBuilder<Item>.Create(builder => builder.OrGroup(group =>
            {
                group.AddStringFilter(nameof(Item.Name), x => x.Name);
                group.AddStringFilter(nameof(Item.Path), x => x.Path);
            }));

        filters.Root.Children.Should().HaveCount(1);
        var orGroup = filters.Root.Children[0].Should().BeOfType<FilterGroupViewModel<Item>>().Subject;
        orGroup.Operator.Should().Be(LogicalOperator.Or);
        orGroup.Children.Should().HaveCount(2);
        orGroup.Children.Should().AllBeOfType<StringFilterViewModel<Item>>();
    }
}
