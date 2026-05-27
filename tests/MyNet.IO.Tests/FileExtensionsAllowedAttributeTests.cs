// -----------------------------------------------------------------------
// <copyright file="FileExtensionsAllowedAttributeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.IO.Attributes;
using MyNet.IO.FileExtensions;
using Xunit;

namespace MyNet.IO.Tests;

public sealed class FileExtensionsAllowedAttributeTests
{
    [Fact]
    public void Constructor_WithStringExtensions_NormalizesExtensions()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt", ".CSV");

        Assert.Contains(".txt", attribute.Extensions);
        Assert.Contains(".csv", attribute.Extensions);
    }

    [Fact]
    public void Constructor_WithFileExtensionObjects_UsesTheirValues()
    {
        var attribute = new FileExtensionsAllowedAttribute(new FileExtension("txt"), new FileExtension("csv"));

        Assert.Contains(".txt", attribute.Extensions);
        Assert.Contains(".csv", attribute.Extensions);
    }

    [Fact]
    public void Constructor_WithNullExtensions_ThrowsArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => new FileExtensionsAllowedAttribute((string[])null!));

    [Fact]
    public void Constructor_WithEmptyExtensions_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => new FileExtensionsAllowedAttribute(Array.Empty<string>()));

    [Fact]
    public void IsValid_WhenAllowEmptyIsTrue_AcceptsNullAndWhitespace()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt");

        Assert.True(attribute.IsValid(null));
        Assert.True(attribute.IsValid(" "));
    }

    [Fact]
    public void IsValid_WhenAllowEmptyIsFalse_RejectsNullAndWhitespace()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt") { AllowEmpty = false };

        Assert.False(attribute.IsValid(null));
        Assert.False(attribute.IsValid(" "));
    }

    [Fact]
    public void IsValid_ReturnsFalse_ForNonStringValue()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt");

        Assert.False(attribute.IsValid(42));
    }

    [Fact]
    public void IsValid_AcceptsMatchingExtension_IgnoringCase()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt", "csv");

        Assert.True(attribute.IsValid("report.TXT"));
        Assert.True(attribute.IsValid("report.csv"));
    }

    [Fact]
    public void IsValid_RejectsNonMatchingExtension()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt");

        Assert.False(attribute.IsValid("report.json"));
    }

    [Fact]
    public void Constructor_WithOnlyWhitespaceExtensions_ThrowsArgumentException()
        => Assert.Throws<ArgumentException>(() => new FileExtensionsAllowedAttribute(" ", "\t"));

    [Fact]
    public void IsValid_WithWildcard_AcceptsAnyNonEmptyPath()
    {
        var attribute = new FileExtensionsAllowedAttribute("*");

        Assert.True(attribute.IsValid("report.json"));
        Assert.True(attribute.IsValid("report"));
        Assert.True(attribute.IsValid("report.TXT"));
    }

    [Fact]
    public void FormatErrorMessage_ContainsFieldNameAndAllowedExtensions()
    {
        var attribute = new FileExtensionsAllowedAttribute("txt", "csv");

        var result = attribute.FormatErrorMessage("Document");

        Assert.Contains("Document", result, StringComparison.Ordinal);
        Assert.Contains(".txt", result, StringComparison.Ordinal);
        Assert.Contains(".csv", result, StringComparison.Ordinal);
    }
}
