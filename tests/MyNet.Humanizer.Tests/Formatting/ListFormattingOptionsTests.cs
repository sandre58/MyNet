// -----------------------------------------------------------------------
// <copyright file="ListFormattingOptionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Formatting.Collections;
using Xunit;

namespace MyNet.Humanizer.Tests.Formatting;

public class ListFormattingOptionsTests
{
    [Fact]
    public void Constructor_CreatesNewInstanceWithDefaultValues()
    {
        var options = new ListFormattingOptions();

        Assert.NotNull(options);
        Assert.Equal(", ", options.Separator);
        Assert.True(options.TrimItems);
        Assert.True(options.IgnoreNullOrWhiteSpace);
        Assert.Equal(ListConjunction.And, options.Conjunction);
        Assert.False(options.UseOxfordComma);
    }

    [Fact]
    public void Constructor_WithSeparator_SetsSeparator()
    {
        var options = new ListFormattingOptions { Separator = "|" };

        Assert.Equal("|", options.Separator);
    }

    [Fact]
    public void Constructor_WithTrimItems_SetsTrimItems()
    {
        var options = new ListFormattingOptions { TrimItems = false };

        Assert.False(options.TrimItems);
    }

    [Fact]
    public void Constructor_WithIgnoreNullOrWhiteSpace_SetsIgnoreNullOrWhiteSpace()
    {
        var options = new ListFormattingOptions { IgnoreNullOrWhiteSpace = false };

        Assert.False(options.IgnoreNullOrWhiteSpace);
    }

    [Fact]
    public void Constructor_WithConjunction_SetsConjunction()
    {
        var options = new ListFormattingOptions { Conjunction = ListConjunction.Or };

        Assert.Equal(ListConjunction.Or, options.Conjunction);
    }

    [Fact]
    public void Constructor_WithUseOxfordComma_SetsUseOxfordComma()
    {
        var options = new ListFormattingOptions { UseOxfordComma = true };

        Assert.True(options.UseOxfordComma);
    }

    [Fact]
    public void Constructor_WithAllProperties_SetsAllProperties()
    {
        var options = new ListFormattingOptions
        {
            Separator = ";",
            TrimItems = false,
            IgnoreNullOrWhiteSpace = false,
            Conjunction = ListConjunction.Or,
            UseOxfordComma = true
        };

        Assert.Equal(";", options.Separator);
        Assert.False(options.TrimItems);
        Assert.False(options.IgnoreNullOrWhiteSpace);
        Assert.Equal(ListConjunction.Or, options.Conjunction);
        Assert.True(options.UseOxfordComma);
    }
}
