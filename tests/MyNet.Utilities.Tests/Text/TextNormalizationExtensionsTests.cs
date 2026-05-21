// -----------------------------------------------------------------------
// <copyright file="TextNormalizationExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using MyNet.Utilities.Text.Normalization;
using Xunit;

namespace MyNet.Utilities.Tests.Text;

public class TextNormalizationExtensionsTests
{
    [Fact]
    public void RemoveDiacritics_RemovesAccents()
    {
        var result = "Crème brûlée".RemoveDiacritics();

        Assert.Equal("Creme brulee", result);
    }

    [Fact]
    public void NormalizeWhitespace_TrimsAndCollapses()
    {
        var result = "  hello\t  world \r\n test ".NormalizeWhitespace();

        Assert.Equal("hello world test", result);
    }

    [Fact]
    public void NormalizeText_AppliesUnicodeForm()
    {
        var result = "e\u0301".NormalizeUnicode(NormalizationForm.FormC);

        Assert.Equal("é", result);
    }

    [Fact]
    public void Normalize_Pipeline_AppliesTransform()
    {
        var result = Utilities.Text.Text.For("  Crème  brûlée ")
            .Normalize(Normalization.CleanWhitespace)
            .Normalize(Normalization.RemoveDiacritics)
            .Value;

        Assert.Equal("Creme brulee", result);
    }
}
