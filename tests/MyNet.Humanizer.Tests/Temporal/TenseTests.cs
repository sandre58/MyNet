// -----------------------------------------------------------------------
// <copyright file="TenseTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer.Temporal;
using Xunit;

namespace MyNet.Humanizer.Tests.Temporal;

public class TenseTests
{
    [Fact]
    public void Tense_HasFutureValue() => Assert.Equal(Tense.Future, Tense.Future);

    [Fact]
    public void Tense_HasPastValue() => Assert.Equal(Tense.Past, Tense.Past);

    [Fact]
    public void Tense_FutureAndPast_AreNotEqual() => Assert.NotEqual(Tense.Future, Tense.Past);

    [Fact]
    public void Tense_CanCastToInt()
    {
        const int future = (int)Tense.Future;
        const int past = (int)Tense.Past;

        Assert.True(future >= 0);
        Assert.True(past >= 0);
    }

    [Theory]
    [InlineData(Tense.Future)]
    [InlineData(Tense.Past)]
    public void Tense_CanConvertFromInt(Tense tense)
    {
        var value = (int)tense;
        var converted = (Tense)value;

        Assert.Equal(tense, converted);
    }
}
