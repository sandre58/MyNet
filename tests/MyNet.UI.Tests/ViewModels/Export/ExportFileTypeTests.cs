// -----------------------------------------------------------------------
// <copyright file="ExportFileTypeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MyNet.UI.ViewModels.Export;
using Xunit;

namespace MyNet.UI.Tests.ViewModels.Export;

public sealed class ExportFileTypeTests
{
    [Fact]
    public void Constructor_NormalizesExtensions()
    {
        var fileType = new ExportFileType("csv", "CSV files|*.csv", ["csv", ".txt"]);

        fileType.DefaultExtension.Should().Be(".csv");
        fileType.AllowedExtensions.Should().BeEquivalentTo(".csv", ".txt");
    }

    [Theory]
    [InlineData("report.csv", true)]
    [InlineData("report.CSV", true)]
    [InlineData("report.pdf", false)]
    [InlineData("", false)]
    public void IsValidPath_ValidatesAllowedExtensions(string path, bool expected)
    {
        var fileType = new ExportFileType(".csv", "CSV files|*.csv");

        fileType.IsValidPath(path).Should().Be(expected);
    }
}
