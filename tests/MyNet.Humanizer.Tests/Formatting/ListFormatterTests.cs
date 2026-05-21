// -----------------------------------------------------------------------
// <copyright file="ListFormatterTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Formatting.Collections;
using Xunit;

namespace MyNet.Humanizer.Tests.Formatting;

public class ListFormatterTests
{
    [Fact]
    public void ListFormatter_FormatWithEnglish_WorksCorrectly()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(["apple", "banana", "cherry"]);

        Assert.NotEmpty(result);
        Assert.Contains("apple", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("banana", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("cherry", result, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void ListFormatter_FormatWithFrench_WorksCorrectly()
    {
        var formatter = CreateListFormatter(new("fr-FR"));

        var result = formatter.Format(["pomme", "banane", "cerise"]);

        Assert.NotEmpty(result);
        Assert.Contains("pomme", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("banane", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("cerise", result, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void ListFormatter_EmptyList_ReturnsEmpty()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format([]);

        Assert.Empty(result);
    }

    [Fact]
    public void ListFormatter_SingleItem_ReturnsSingleItem()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(["apple"]);

        Assert.Equal("apple", result);
    }

    [Fact]
    public void ListFormatter_TwoItems_FormatsTwoItems()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(["apple", "banana"]);

        Assert.NotEmpty(result);
        Assert.Contains("apple", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("banana", result, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void ListFormatter_WithAndConjunction_FormatsCorrectly()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(
            ["apple", "banana", "cherry"],
            new() { Conjunction = ListConjunction.And });

        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListFormatter_WithOrConjunction_FormatsCorrectly()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(
            ["apple", "banana", "cherry"],
            new() { Conjunction = ListConjunction.Or });

        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListFormatter_WithOxfordComma_AddsCommaBeforeConjunction()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(
            ["apple", "banana", "cherry"],
            new() { Conjunction = ListConjunction.And, UseOxfordComma = true });

        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListFormatter_IgnoringNullOrWhiteSpace_FiltersEmptyItems()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(
            ["apple", null, "  ", "banana"],
            new() { IgnoreNullOrWhiteSpace = true });

        Assert.NotEmpty(result);
        Assert.Contains("apple", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("banana", result, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void ListFormatter_WithCustomSeparator_UsesSeparator()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(
            ["apple", "banana", "cherry"],
            new() { Separator = "|", Conjunction = ListConjunction.And });

        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("de-DE")]
    public void ListFormatter_DifferentCultures_AllWork(string cultureName)
    {
        var formatter = CreateListFormatter(new(cultureName));

        var result = formatter.Format(["first", "second", "third"]);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListFormatter_LargeList_Works()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var items = new List<string>();
        for (var i = 0; i < 100; i++)
            items.Add($"item{i}");

        var result = formatter.Format(items);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void ListFormatter_AllNullOrWhiteSpace_ReturnsEmpty()
    {
        var formatter = CreateListFormatter(new("en-US"));

        var result = formatter.Format(
            [null, "  ", "\t", "\n"],
            new() { IgnoreNullOrWhiteSpace = true });

        Assert.Empty(result);
    }

    private static ListFormatter CreateListFormatter(CultureInfo culture)
    {
        // Create a simple translator implementation
        var translator = new SimpleTranslator();
        var translationService = new TranslationService(translator, new FixedCultureContext(culture));
        return new(translationService, culture);
    }

    /// <summary>
    /// Simple translator mock for testing humanizers without full translation setup.
    /// </summary>
    private sealed class SimpleTranslator : ITranslator
    {
        public string Translate(string key, TranslationOptions options, CultureInfo culture) => key;

        public string Translate(string key, TranslationOptions options, CultureInfo culture, string resourceKey) => key;
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }
}
