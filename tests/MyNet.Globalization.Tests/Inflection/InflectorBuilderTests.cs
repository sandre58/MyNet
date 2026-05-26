// -----------------------------------------------------------------------
// <copyright file="InflectorBuilderTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Inflection;
using Xunit;

namespace MyNet.Globalization.Tests.Inflection;

public sealed class InflectorBuilderTests
{
    [Fact]
    public void Build_WithCustomPluralRule_AppliesRule()
    {
        var inflector = new InflectorBuilder(CultureInfo.InvariantCulture)
            .AddPluralRule("ox$", "oxen")
            .Build();

        Assert.Equal("oxen", inflector.Pluralize("ox"));
    }

    [Fact]
    public void Build_WithUncountableWord_ReturnsOriginalForPluralize()
    {
        var inflector = new InflectorBuilder(CultureInfo.InvariantCulture)
            .AddPluralRule("$", "s")
            .AddUncountable("equipment")
            .Build();

        Assert.Equal("equipment", inflector.Pluralize("equipment"));
    }

    [Fact]
    public void Build_WithIrregularRule_SingularizesCorrectly()
    {
        var inflector = new InflectorBuilder(CultureInfo.InvariantCulture)
            .AddIrregular("person", "people", matchEnding: false)
            .Build();

        Assert.Equal("person", inflector.Singularize("people"));
    }
}
