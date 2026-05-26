// -----------------------------------------------------------------------
// <copyright file="FrenchOrdinalizerTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Ordinalizing;
using MyNet.Humanizer.Ordinalizing.Cultures;
using MyNet.Text;
using Xunit;

namespace MyNet.Humanizer.Tests.Ordinalizing;

public sealed class FrenchOrdinalizerTests
{
    [Fact]
    public void Ordinalize_OneWithFeminineGender_ReturnsPremiereForm()
    {
        var options = new OrdinalizationOptions { Gender = GrammaticalGender.Feminine };

        var result = Ordinalizers.French.Ordinalize(1, options);

        Assert.Equal("1re", result);
    }

    [Fact]
    public void Ordinalize_OneWithMasculineGender_ReturnsStandardForm()
    {
        var options = new OrdinalizationOptions { Gender = GrammaticalGender.Masculine };

        var result = Ordinalizers.French.Ordinalize(1, options);

        Assert.Equal("1er", result);
    }
}
