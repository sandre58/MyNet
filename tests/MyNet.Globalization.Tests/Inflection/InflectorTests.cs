// -----------------------------------------------------------------------
// <copyright file="InflectorTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Globalization.Inflection.Cultures;
using Xunit;

namespace MyNet.Globalization.Tests.Inflection;

public sealed class InflectorTests
{
    [Theory]
    [InlineData("child", "children")]
    [InlineData("person", "people")]
    [InlineData("fish", "fish")]
    public void English_Pluralize_AppliesIrregularRules(string singular, string expected) => Assert.Equal(expected, Inflectors.English.Pluralize(singular));

    [Theory]
    [InlineData("children", "child")]
    [InlineData("people", "person")]
    public void English_Singularize_ReversesIrregularRules(string plural, string expected) => Assert.Equal(expected, Inflectors.English.Singularize(plural));

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void English_IsPlural_FollowsEnglishRules(decimal count, bool expected) => Assert.Equal(expected, Inflectors.English.IsPlural(count));
}
