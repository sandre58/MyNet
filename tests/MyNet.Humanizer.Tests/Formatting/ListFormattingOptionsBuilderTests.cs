// -----------------------------------------------------------------------
// <copyright file="ListFormattingOptionsBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Formatting.Collections;
using Xunit;

namespace MyNet.Humanizer.Tests.Formatting;

public class ListFormattingOptionsBuilderTests
{
    [Fact]
    public void Build_ReturnsOptionsWithDefaultValues()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.Build();

        Assert.NotNull(options);
        Assert.Equal(", ", options.Separator);
        Assert.True(options.TrimItems);
        Assert.True(options.IgnoreNullOrWhiteSpace);
        Assert.Equal(ListConjunction.And, options.Conjunction);
        Assert.False(options.UseOxfordComma);
    }

    [Fact]
    public void WithSeparator_SetsSeparator()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.WithSeparator("|").Build();

        Assert.Equal("|", options.Separator);
    }

    [Fact]
    public void WithTrimming_SetsTrimItems()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.WithTrimming().Build();

        Assert.True(options.TrimItems);
    }

    [Fact]
    public void WithoutTrimming_SetsTrimItems()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.WithoutTrimming().Build();

        Assert.False(options.TrimItems);
    }

    [Fact]
    public void IgnoreNullOrWhiteSpace_SetsIgnoreNullOrWhiteSpace()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.IgnoreNullOrWhiteSpace().Build();

        Assert.True(options.IgnoreNullOrWhiteSpace);
    }

    [Fact]
    public void IncludeNullOrWhiteSpace_SetsIgnoreNullOrWhiteSpace()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.IncludeNullOrWhiteSpace().Build();

        Assert.False(options.IgnoreNullOrWhiteSpace);
    }

    [Fact]
    public void WithConjunction_SetsConjunction()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.WithConjunction(ListConjunction.Or).Build();

        Assert.Equal(ListConjunction.Or, options.Conjunction);
    }

    [Fact]
    public void WithOxfordComma_SetsUseOxfordComma()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.WithOxfordComma().Build();

        Assert.True(options.UseOxfordComma);
    }

    [Fact]
    public void WithoutOxfordComma_SetsUseOxfordComma()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder.WithoutOxfordComma().Build();

        Assert.False(options.UseOxfordComma);
    }

    [Fact]
    public void FluentBuilder_BuildsComplexOptions()
    {
        var builder = new ListFormattingOptionsBuilder();

        var options = builder
            .WithSeparator(";")
            .WithoutTrimming()
            .IncludeNullOrWhiteSpace()
            .WithOr()
            .WithOxfordComma()
            .Build();

        Assert.Equal(";", options.Separator);
        Assert.False(options.TrimItems);
        Assert.False(options.IgnoreNullOrWhiteSpace);
        Assert.Equal(ListConjunction.Or, options.Conjunction);
        Assert.True(options.UseOxfordComma);
    }
}
