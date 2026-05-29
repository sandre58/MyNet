// -----------------------------------------------------------------------
// <copyright file="CharHelperTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Primitives.Helpers;
using Xunit;

namespace MyNet.Primitives.Tests.Helpers;

public sealed class CharHelperTests
{
    [Fact]
    public void Alphabet_Contains26Letters()
    {
        Assert.Equal(26, CharHelper.Alphabet.Length);
        Assert.Equal('a', CharHelper.Alphabet[0]);
    }

    [Fact]
    public void Numbers_ContainsTenDigits()
    {
        Assert.Equal(10, CharHelper.Numbers.Length);
        Assert.Equal('0', CharHelper.Numbers[0]);
    }

    [Fact]
    public void HexDigits_ContainsSixteenCharacters() => Assert.Equal(16, CharHelper.HexDigits.Length);

    [Fact]
    public void AlphaNumbers_ContainsLettersAndDigits() => Assert.Equal(36, CharHelper.AlphaNumbers.Length);
}
