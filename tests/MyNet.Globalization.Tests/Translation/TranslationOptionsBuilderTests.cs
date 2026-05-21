// -----------------------------------------------------------------------
// <copyright file="TranslationOptionsBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MyNet.Globalization.Localization.Translation;
using Xunit;

namespace MyNet.Globalization.Tests.Translation;

public class TranslationOptionsBuilderTests
{
    [Fact]
    public void Build_DefaultValues_ReturnsDefaultOptions()
    {
        var options = new TranslationOptionsBuilder().Build();

        Assert.Equal(DisplayStyle.Default, options.Style);
        Assert.Null(options.Quantity);
        Assert.Empty(options.Arguments);
        Assert.True(options.UseInflectionFallback);
        Assert.True(options.UseKeyAsFallback);
    }

    [Theory]
    [InlineData(DisplayStyle.Default)]
    [InlineData(DisplayStyle.Short)]
    [InlineData(DisplayStyle.Abbreviation)]
    [InlineData(DisplayStyle.Symbol)]
    [InlineData(DisplayStyle.Narrow)]
    public void WithStyle_SetsDifferentStyles(DisplayStyle style)
    {
        var options = new TranslationOptionsBuilder()
            .WithStyle(style)
            .Build();

        Assert.Equal(style, options.Style);
    }

    [Fact]
    public void WithQuantity_SetsQuantityAndAddsCountArgument()
    {
        var options = new TranslationOptionsBuilder()
            .WithQuantity(42)
            .Build();

        Assert.Equal(42, options.Quantity);
        Assert.Empty(options.Arguments);
    }

    [Fact]
    public void WithQuantity_OverridesExistingCountArgument()
    {
        var options = new TranslationOptionsBuilder()
            .WithArgument("quantity", 10)
            .WithQuantity(42)
            .Build();

        Assert.Equal(10, options.Arguments["quantity"]);
        Assert.Equal(42, options.Quantity);
    }

    [Fact]
    public void WithArgument_AddsNamedArgument()
    {
        var options = new TranslationOptionsBuilder()
            .WithArgument("name", "Alice")
            .WithArgument("age", 30)
            .Build();

        Assert.Equal("Alice", options.Arguments["name"]);
        Assert.Equal(30, options.Arguments["age"]);
    }

    [Fact]
    public void WithArgument_CaseInsensitive()
    {
        var options = new TranslationOptionsBuilder()
            .WithArgument("Name", "Alice")
            .WithArgument("NAME", "Bob")
            .Build();

        // Last write wins
        Assert.Equal("Bob", options.Arguments["name"]);
        Assert.Single(options.Arguments);
    }

    [Fact]
    public void WithArgument_NullOrWhiteSpaceName_Throws()
    {
        var builder = new TranslationOptionsBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithArgument(null!, "value"));
        Assert.Throws<ArgumentException>(() => builder.WithArgument(string.Empty, "value"));
        Assert.Throws<ArgumentException>(() => builder.WithArgument("  ", "value"));
    }

    [Fact]
    public void WithArgument_NullValue_Allowed()
    {
        var options = new TranslationOptionsBuilder()
            .WithArgument("value", null)
            .Build();

        Assert.True(options.Arguments.ContainsKey("value"));
        Assert.Null(options.Arguments["value"]);
    }

    [Fact]
    public void WithArguments_AddsMultipleArguments()
    {
        var args = new Dictionary<string, object?>
        {
            ["firstName"] = "John",
            ["lastName"] = "Doe",
            ["count"] = 5
        };

        var options = new TranslationOptionsBuilder()
            .WithArguments(args)
            .Build();

        Assert.Equal(3, options.Arguments.Count);
        Assert.Equal("John", options.Arguments["firstName"]);
        Assert.Equal("Doe", options.Arguments["lastName"]);
        Assert.Equal(5, options.Arguments["count"]);
    }

    [Fact]
    public void WithArguments_NullEnumerable_Throws()
    {
        var builder = new TranslationOptionsBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithArguments(null!));
    }

    [Fact]
    public void WithArguments_OverwritesPreviousValues()
    {
        var args = new Dictionary<string, object?> { ["name"] = "Bob" };

        var options = new TranslationOptionsBuilder()
            .WithArgument("name", "Alice")
            .WithArguments(args)
            .Build();

        Assert.Equal("Bob", options.Arguments["name"]);
    }

    [Fact]
    public void UseInflectionFallback_EnablesInflectionFallback()
    {
        var options = new TranslationOptionsBuilder()
            .WithoutInflectionFallback()
            .UseInflectionFallback()
            .Build();

        Assert.True(options.UseInflectionFallback);
    }

    [Fact]
    public void WithoutInflectionFallback_DisablesInflectionFallback()
    {
        var options = new TranslationOptionsBuilder()
            .WithoutInflectionFallback()
            .Build();

        Assert.False(options.UseInflectionFallback);
    }

    [Fact]
    public void UseKeyFallback_EnablesKeyFallback()
    {
        var options = new TranslationOptionsBuilder()
            .WithoutKeyFallback()
            .UseKeyFallback()
            .Build();

        Assert.True(options.UseKeyAsFallback);
    }

    [Fact]
    public void WithoutKeyFallback_DisablesKeyFallback()
    {
        var options = new TranslationOptionsBuilder()
            .WithoutKeyFallback()
            .Build();

        Assert.False(options.UseKeyAsFallback);
    }

    [Fact]
    public void ChainedBuilding_CombinesAllOptions()
    {
        var options = new TranslationOptionsBuilder()
            .WithStyle(DisplayStyle.Short)
            .WithQuantity(10)
            .WithArgument("name", "Test")
            .WithoutInflectionFallback()
            .WithoutKeyFallback()
            .Build();

        Assert.Equal(DisplayStyle.Short, options.Style);
        Assert.Equal(10, options.Quantity);
        Assert.Contains(options.Arguments, kvp => kvp.Key == "name" && (string?)kvp.Value == "Test");
        Assert.False(options.UseInflectionFallback);
        Assert.False(options.UseKeyAsFallback);
    }

    [Fact]
    public void MultipleBuiltOptions_AreIndependent()
    {
        var builder1 = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Abbreviation);
        var builder2 = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Symbol);

        var options1 = builder1.Build();
        var options2 = builder2.Build();

        Assert.Equal(DisplayStyle.Abbreviation, options1.Style);
        Assert.Equal(DisplayStyle.Symbol, options2.Style);
    }

    [Fact]
    public void BuildMultipleTimes_ProducesIndependentInstances()
    {
        var builder = new TranslationOptionsBuilder().WithArgument("value", 1);

        var options1 = builder.Build();
        builder.WithArgument("value", 2);
        var options2 = builder.Build();

        // After building, further modifications don't affect previous builds
        Assert.Equal(1, options1.Arguments["value"]);
        Assert.Equal(2, options2.Arguments["value"]);
    }

    [Fact]
    public void WithArguments_EmptyCollection_Allowed()
    {
        var options = new TranslationOptionsBuilder()
            .WithArguments([])
            .Build();

        Assert.Empty(options.Arguments);
    }

    [Fact]
    public void ComplexScenario_MultipleStylesAndArguments()
    {
        var args = new[]
        {
            new KeyValuePair<string, object?>("item1", "First"),
            new KeyValuePair<string, object?>("item2", "Second"),
            new KeyValuePair<string, object?>("count", 100)
        };

        var builder = new TranslationOptionsBuilder();

        foreach (var arg in args)
            builder.WithArgument(arg.Key, arg.Value);

        var options = builder
            .WithStyle(DisplayStyle.Narrow)
            .WithQuantity(50)
            .Build();

        Assert.Equal(DisplayStyle.Narrow, options.Style);
        Assert.Equal(50, options.Quantity);

        // quantity is now carried by options.Quantity rather than injected as argument
        Assert.Equal(100, options.Arguments["count"]);
    }
}
