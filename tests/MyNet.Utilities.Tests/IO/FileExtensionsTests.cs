// -----------------------------------------------------------------------
// <copyright file="FileExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using MyNet.Utilities.IO.FileExtensions;
using Xunit;

namespace MyNet.Utilities.Tests.IO;

public sealed class FileExtensionsTests
{
    [Fact]
    public void FileExtension_NormalizesValue()
    {
        var extension = new FileExtension(" TXT ");

        Assert.Equal(".txt", extension.Value, StringComparer.Ordinal);
        Assert.Equal(".txt", extension.ToString(), StringComparer.Ordinal);
    }

    [Fact]
    public void FileExtension_WithEquivalentValues_AreEqual()
    {
        var left = new FileExtension("txt");
        var right = new FileExtension(".TXT");

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void FileExtensionGroup_Create_BuildsNormalizedExtensions()
    {
        var group = FileExtensionGroup.Create("Documents", "txt", ".csv");

        Assert.Equal("Documents", group.Key, StringComparer.Ordinal);
        Assert.Equal([".txt", ".csv"], [.. group.Extensions.Select(x => x.Value)]);
        Assert.Equal(".txt", group.GetDefaultExtension()!.Value, StringComparer.Ordinal);
    }

    [Fact]
    public void FileExtensionFilter_AllExtensions_ReturnsDistinctExtensions()
    {
        var filter = new FileExtensionFilter([
            FileExtensionGroup.Create("First", ".txt", ".csv"),
            FileExtensionGroup.Create("Second", ".csv", ".json")
        ]);

        Assert.Equal([".txt", ".csv", ".json"], [.. filter.AllExtensions]);
    }

    [Fact]
    public void FileExtensionFilter_ToFilterString_UsesLocalizedNamesWhenProvided()
    {
        var filter = new FileExtensionFilter([FileExtensionGroup.Create("Docs", ".txt", ".csv")]);

        var result = filter.ToFilterString(key => "Localized " + key);

        Assert.Equal("Localized Docs (*.txt;*.csv)", result, StringComparer.Ordinal);
    }

    [Fact]
    public void FileExtensionFilterBuilder_AddRange_And_Add_BuildsCombinedFilter()
    {
        var filter = new FileExtensionFilterBuilder()
            .AddRange([FileExtensionGroup.Create("Docs", ".txt")])
            .Add(FileExtensionGroup.Create("Data", ".csv"))
            .Build();

        Assert.Equal(2, filter.Groups.Count);
    }

    [Fact]
    public void FileExtensionGroup_ToFilter_CreatesTitleAndPattern()
    {
        var group = FileExtensionGroup.Create("Docs", ".txt", ".csv");

        var result = group.ToFilter(_ => "Documents");

        Assert.Equal("Documents (*.txt;*.csv)", result.Title, StringComparer.Ordinal);
        Assert.Equal("*.txt;*.csv", result.Pattern, StringComparer.Ordinal);
    }

    [Fact]
    public void FileExtensionGroup_IsValidFile_ReturnsTrueForMatchingExtension()
    {
        var group = FileExtensionGroup.Create("Docs", ".txt");

        Assert.True(group.IsValidFile("report.TXT"));
        Assert.False(group.IsValidFile("report.csv"));
        Assert.False(group.IsValidFile(string.Empty));
    }

    [Fact]
    public void FileExtensionGroup_Concat_MergesExtensionsAndUsesProvidedKey()
    {
        var left = FileExtensionGroup.Create("Docs", ".txt");
        var right = FileExtensionGroup.Create("Data", ".csv");

        var result = left.Concat(right, "Combined");

        Assert.Equal("Combined", result.Key, StringComparer.Ordinal);
        Assert.Equal([".txt", ".csv"], [.. result.Extensions.Select(x => x.Value)]);
    }

    [Fact]
    public void FileExtensionGroups_ToFilterString_ProducesDialogCompatibleString()
    {
        var result = new[]
        {
            FileExtensionGroup.Create("Docs", ".txt"),
            FileExtensionGroup.Create("Data", ".csv")
        }.ToFilterString(key => key.ToUpperInvariant());

        Assert.Equal("DOCS (*.txt)|*.txt|DATA (*.csv)|*.csv", result, StringComparer.Ordinal);
    }

    [Fact]
    public void Files_FilterByExtensions_ReturnsOnlyMatchingFiles()
    {
        var files = new[] { "a.txt", "b.csv", "c.TXT", "d.json" };
        var group = FileExtensionGroup.Create("Docs", ".txt");

        var result = files.FilterByExtensions(group).ToArray();

        Assert.Equal(["a.txt", "c.TXT"], result);
    }

    [Fact]
    public void PredefinedFilters_ExposeExpectedExtensions()
    {
        Assert.Contains(".xlsx", FileFilters.Excel().AllExtensions);
        Assert.Contains(".jpg", FileFilters.Images().AllExtensions);
        Assert.Contains(".*", FileFilters.All().AllExtensions);
        Assert.Contains(".png", CommonFileFilters.Images.Extensions.Select(x => x.Value));
        Assert.Equal(".csv", FileExtensionGroups.Csv.GetDefaultExtension()!.Value, StringComparer.Ordinal);
    }
}
