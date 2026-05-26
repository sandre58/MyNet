// -----------------------------------------------------------------------
// <copyright file="TextTruncationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Text.Truncation;
using Xunit;

namespace MyNet.Text.Tests;

public class TextTruncationExtensionsTests
{
    [Fact]
    public void TextTruncationOptions_NegativeLength_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => new TextTruncationOptions
    {
        Length = -1,
        TruncationString = "...",
        Direction = TruncateFrom.Right
    });

    [Fact]
    public void TextTruncationOptions_NullTruncationString_Throws() => Assert.Throws<ArgumentNullException>(() => new TextTruncationOptions
    {
        Length = 5,
        TruncationString = null!,
        Direction = TruncateFrom.Right
    });

    [Fact]
    public void Truncate_StringExtension_WithNullTransform_Throws()
    {
        const string input = "abcdef";
        ITextTruncator nullTransform = null!;

        Assert.Throws<ArgumentNullException>(() => input.Truncate(nullTransform));
    }

    [Fact]
    public void Truncate_PipelineExtension_WithNullTransform_Throws()
    {
        var pipeline = new TextPipeline("abcdef", CultureInfo.InvariantCulture);
        ITextTruncator nullTransform = null!;

        Assert.Throws<ArgumentNullException>(() => pipeline.Truncate(nullTransform));
    }

    [Fact]
    public void Apply_WithNullCulture_Throws()
    {
        var truncator = new FixedLengthTextTruncator(new() { Length = 3 });

        Assert.Throws<ArgumentNullException>(() => truncator.Apply("abcdef", null!));
    }

    [Fact]
    public void Truncate_WithValidOptions_ReturnsExpectedValue()
    {
        var result = "abcdef".Truncate(4, "...");

        Assert.Equal("a...", result);
    }
}
