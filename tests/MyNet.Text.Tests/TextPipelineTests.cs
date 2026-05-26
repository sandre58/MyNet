// -----------------------------------------------------------------------
// <copyright file="TextPipelineTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Text.Normalization;
using MyNet.Text.TextCasing;
using Xunit;
using TextPortal = MyNet.Text.Text;

namespace MyNet.Text.Tests;

public sealed class TextPipelineTests
{
    [Fact]
    public void For_AppliesTransformsInOrder()
    {
        var result = TextPortal
            .For("  café  ", CultureInfo.InvariantCulture)
            .Apply(Casing.TitleCase, new DiacriticsRemovalTransform())
            .Value;

        Assert.Equal("  Cafe  ", result);
    }

    [Fact]
    public void Transform_ChainsMultipleTransforms()
    {
        var result = TextPortal
            .For("hello world", CultureInfo.InvariantCulture)
            .Transform(Casing.UpperCase, new UnicodeNormalizationTransform(System.Text.NormalizationForm.FormC));

        Assert.Equal("HELLO WORLD", result);
    }
}
