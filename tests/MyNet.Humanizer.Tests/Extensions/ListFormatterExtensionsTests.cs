// -----------------------------------------------------------------------
// <copyright file="ListFormatterExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using MyNet.Humanizer.Formatting.Collections;
using MyNet.Humanizer.Static;
using Xunit;

namespace MyNet.Humanizer.Tests.Extensions;

[UseCulture("en-US")]
[Collection("UseCultureSequential")]
public partial class ListFormatterExtensionsTests
{
    private static readonly Func<string, string> DummyFormatter = input => input;

    private readonly List<SomeClass> _testCollection =
    [
        new() { SomeInt = 1, SomeString = "One" },
        new() { SomeInt = 2, SomeString = "Two" },
        new() { SomeInt = 3, SomeString = "Three" }
    ];

    [Fact]
    public void EmptyListReturnsEmpty()
    {
        var result = Array.Empty<string>().Humanize();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void TwoItems_UsesAndSeparator()
    {
        var result = new[] { "Apple", "Banana" }.Humanize();

        Assert.Equal("Apple and Banana", NormalizeSpaces(result));
    }

    [Fact]
    public void ThreeItems_WithOrConjunction_ContainsExpectedOrder()
    {
        var result = new[] { "Apple", "Banana", "Cherry" }.Humanize(ListConjunction.Or);

        Assert.Equal("Apple, Banana or Cherry", NormalizeSpaces(result));
    }

    [Fact]
    public void HumanizeWithPrefixAndSuffix_WrapsFormattedContent()
    {
        var result = new[] { "Apple", "Banana" }.HumanizeWith("Items: ", ".");

        Assert.Equal("Items: Apple and Banana.", NormalizeSpaces(result));
    }

    [Fact]
    public void HumanizeInParentheses_WrapsFormattedContent()
    {
        var result = new[] { "Apple", "Banana" }.HumanizeInParentheses();

        Assert.Equal("(Apple and Banana)", NormalizeSpaces(result));
    }

    [Fact]
    public void HumanizeInBrackets_WrapsFormattedContent()
    {
        var result = new[] { "Apple", "Banana" }.HumanizeInBrackets();

        Assert.Equal("[Apple and Banana]", NormalizeSpaces(result));
    }

    [Fact]
    public void HumanizeInBraces_WrapsFormattedContent()
    {
        var result = new[] { "Apple", "Banana" }.HumanizeInBraces();

        Assert.Equal("{Apple and Banana}", NormalizeSpaces(result));
    }

    [Fact]
    public void HumanizeWithFormatter_AppliesFormatterAndAffixes()
    {
        var result = new[] { 1, 2 }.HumanizeWith(x => $"#{x}", "Items: ", ".");

        Assert.Equal("Items: #1 and #2.", NormalizeSpaces(result));
    }

    [Fact]
    public void HumanizeNonNull_AlwaysIgnoresNullOrWhiteSpaceItems()
    {
        var result = new[] { "A", null, " ", "C" }.HumanizeNonNull(new() { IgnoreNullOrWhiteSpace = false });

        Assert.Equal("A and C", NormalizeSpaces(result));
    }

    [Fact]
    public void BuildOptions_CreatesNewBuilderInstance()
        => Assert.NotNull(ListFormatterExtensions.BuildOptions());

    [Fact]
    public void HumanizeReturnsOnlyNameWhenCollectionContainsOneItem()
    {
        var collection = new List<string> { "A String" };

        AssertHumanized("A String", collection.Humanize());
    }

    [Fact]
    public void HumanizeUsesSeparatorWhenMoreThanOneItemIsInCollection()
    {
        var collection = new List<string> { "A String", "Another String" };

        AssertHumanized("A String or Another String", collection.Humanize(ListConjunction.Or));
    }

    [Fact]
    public void HumanizeDefaultsSeparatorToAnd()
    {
        var collection = new List<string> { "A String", "Another String" };

        AssertHumanized("A String and Another String", collection.Humanize());
    }

    [Fact]
    public void HumanizeUsesOxfordComma()
    {
        var collection = new List<string>
        {
            "A String",
            "Another String",
            "A Third String"
        };

        AssertHumanized("A String, Another String or A Third String", collection.Humanize(ListConjunction.Or));
    }

    [Fact]
    public void HumanizeDefaultsToToString() => AssertHumanized("ToString, ToString or ToString", _testCollection.Humanize(ListConjunction.Or));

    [Fact]
    public void HumanizeUsesStringDisplayFormatter()
    {
        var humanized = _testCollection.Humanize(sc => string.Format(CultureInfo.CurrentCulture, "SomeObject #{0} - {1}", sc.SomeInt, sc.SomeString));
        AssertHumanized("SomeObject #1 - One, SomeObject #2 - Two and SomeObject #3 - Three", humanized);
    }

    [Fact]
    public void HumanizeUsesObjectDisplayFormatter()
    {
        var humanized = _testCollection.Humanize(sc => sc.SomeInt.ToString(CultureInfo.CurrentCulture));
        AssertHumanized("1, 2 and 3", humanized);
    }

    [Fact]
    public void HumanizeUsesStringDisplayFormatterWhenSeparatorIsProvided()
    {
        var humanized = _testCollection.Humanize(sc => string.Format(CultureInfo.CurrentCulture, "SomeObject #{0} - {1}", sc.SomeInt, sc.SomeString), ListConjunction.Or);
        AssertHumanized("SomeObject #1 - One, SomeObject #2 - Two or SomeObject #3 - Three", humanized);
    }

    [Fact]
    public void HumanizeUsesObjectDisplayFormatterWhenSeparatorIsProvided()
    {
        var humanized = _testCollection.Humanize(sc => sc.SomeInt.ToString(CultureInfo.CurrentCulture), ListConjunction.Or);
        AssertHumanized("1, 2 or 3", humanized);
    }

    [Fact]
    public void HumanizeHandlesNullItemsWithoutAnException() => Assert.Null(Record.Exception(() => new object?[] { null, null }.Humanize()));

    [Fact]
    public void HumanizeHandlesNullStringDisplayFormatterReturnsWithoutAnException() => Assert.Null(Record.Exception(() => new[] { "A", "B", "C" }.Humanize(_ => null)));

    [Fact]
    public void HumanizeRunsStringDisplayFormatterOnNulls() => AssertHumanized("1, (null) and 3", new int?[] { 1, null, 3 }.Humanize(x => x?.ToString(CultureInfo.CurrentCulture) ?? "(null)"));

    [Fact]
    public void HumanizeRunsObjectDisplayFormatterOnNulls() => AssertHumanized("1, 2 and 3", new int?[] { 1, null, 3 }.Humanize(x => (x ?? 2).ToString(CultureInfo.CurrentCulture)));

    [Fact]
    public void HumanizeRemovesEmptyItemsByDefault() => AssertHumanized("A and C", new[] { "A", " ", "C" }.Humanize(DummyFormatter));

    [Fact]
    public void HumanizeTrimsItemsByDefault() => AssertHumanized("A, B and C", new[] { "A", "  B  ", "C" }.Humanize(DummyFormatter));

    private static string NormalizeSpaces(string value) => NormalizeRegex().Replace(value, " ").Trim();

    private static void AssertHumanized(string expected, string actual) => Assert.Equal(NormalizeSpaces(expected), NormalizeSpaces(actual), StringComparer.Ordinal);

    [GeneratedRegex("\\s+")]
    private static partial Regex NormalizeRegex();

    private sealed class SomeClass
    {
        public int SomeInt { get; init; }

        public string? SomeString { get; init; }

        public override string ToString() => nameof(ToString);
    }
}
