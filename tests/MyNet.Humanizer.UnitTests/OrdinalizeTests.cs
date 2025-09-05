// -----------------------------------------------------------------------
// <copyright file="OrdinalizeTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using Xunit;

namespace MyNet.Humanizer.UnitTests;

[Collection("UseCultureSequential")]
public class OrdinalizeTests
{
    [Theory]
    [UseCulture("en-US")]
    [InlineData("0", "0th")]
    [InlineData("1", "1st")]
    [InlineData("2", "2nd")]
    [InlineData("3", "3rd")]
    [InlineData("4", "4th")]
    [InlineData("5", "5th")]
    [InlineData("6", "6th")]
    [InlineData("7", "7th")]
    [InlineData("8", "8th")]
    [InlineData("9", "9th")]
    [InlineData("10", "10th")]
    [InlineData("11", "11th")]
    [InlineData("12", "12th")]
    [InlineData("13", "13th")]
    [InlineData("14", "14th")]
    [InlineData("20", "20th")]
    [InlineData("21", "21st")]
    [InlineData("22", "22nd")]
    [InlineData("23", "23rd")]
    [InlineData("24", "24th")]
    [InlineData("100", "100th")]
    [InlineData("101", "101st")]
    [InlineData("102", "102nd")]
    [InlineData("103", "103rd")]
    [InlineData("104", "104th")]
    [InlineData("110", "110th")]
    [InlineData("1000", "1000th")]
    [InlineData("1001", "1001st")]
    public void OrdinalizeString(string number, string ordinalized) => Assert.Equal(number.Ordinalize(), ordinalized);

    [Theory]
    [UseCulture("en-US")]
    [InlineData(0, "0th")]
    [InlineData(1, "1st")]
    [InlineData(2, "2nd")]
    [InlineData(3, "3rd")]
    [InlineData(4, "4th")]
    [InlineData(5, "5th")]
    [InlineData(6, "6th")]
    [InlineData(7, "7th")]
    [InlineData(8, "8th")]
    [InlineData(9, "9th")]
    [InlineData(10, "10th")]
    [InlineData(11, "11th")]
    [InlineData(12, "12th")]
    [InlineData(13, "13th")]
    [InlineData(14, "14th")]
    [InlineData(20, "20th")]
    [InlineData(21, "21st")]
    [InlineData(22, "22nd")]
    [InlineData(23, "23rd")]
    [InlineData(24, "24th")]
    [InlineData(100, "100th")]
    [InlineData(101, "101st")]
    [InlineData(102, "102nd")]
    [InlineData(103, "103rd")]
    [InlineData(104, "104th")]
    [InlineData(110, "110th")]
    [InlineData(1000, "1000th")]
    [InlineData(1001, "1001st")]
    public void OrdinalizeNumber(int number, string ordinalized) => Assert.Equal(number.Ordinalize(), ordinalized);

    [Theory]
    [UseCulture("fr-FR")]
    [InlineData(0, "0ème")]
    [InlineData(1, "1er")]
    [InlineData(2, "2ème")]
    [InlineData(3, "3ème")]
    [InlineData(4, "4ème")]
    [InlineData(5, "5ème")]
    [InlineData(6, "6ème")]
    [InlineData(7, "7ème")]
    [InlineData(8, "8ème")]
    [InlineData(9, "9ème")]
    [InlineData(10, "10ème")]
    [InlineData(11, "11ème")]
    [InlineData(12, "12ème")]
    [InlineData(13, "13ème")]
    [InlineData(14, "14ème")]
    [InlineData(20, "20ème")]
    [InlineData(21, "21ème")]
    [InlineData(22, "22ème")]
    [InlineData(23, "23ème")]
    [InlineData(24, "24ème")]
    public void OrdinalizeNumberFr(int number, string ordinalized) => Assert.Equal(number.Ordinalize(), ordinalized);

    [Theory]
    [UseCulture("fr-FR")]
    [InlineData("0", "0ème")]
    [InlineData("1", "1er")]
    [InlineData("2", "2ème")]
    [InlineData("3", "3ème")]
    [InlineData("4", "4ème")]
    [InlineData("5", "5ème")]
    [InlineData("6", "6ème")]
    [InlineData("7", "7ème")]
    [InlineData("8", "8ème")]
    [InlineData("9", "9ème")]
    [InlineData("10", "10ème")]
    [InlineData("11", "11ème")]
    [InlineData("12", "12ème")]
    [InlineData("13", "13ème")]
    [InlineData("14", "14ème")]
    [InlineData("20", "20ème")]
    [InlineData("21", "21ème")]
    [InlineData("22", "22ème")]
    [InlineData("23", "23ème")]
    [InlineData("24", "24ème")]
    public void OrdinalizeStringFr(string number, string ordinalized) => Assert.Equal(number.Ordinalize(), ordinalized);

    [Theory]
    [UseCulture("en-US")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(8)]
    public void OrdinalizeNumberGenderIsImmaterial(int number)
    {
        var masculineOrdinalized = number.Ordinalize(GrammaticalGender.Masculine);
        var feminineOrdinalized = number.Ordinalize(GrammaticalGender.Feminine);
        Assert.Equal(masculineOrdinalized, feminineOrdinalized);
    }

    [Theory]
    [UseCulture("fr-FR")]
    [InlineData(1, "1ère")]
    [InlineData(2, "2ème")]
    public void OrdinalizeNumberFeminineFr(int number, string ordinalized)
    {
        var feminineOrdinalized = number.Ordinalize(GrammaticalGender.Feminine);
        Assert.Equal(ordinalized, feminineOrdinalized);
    }

    [Theory]
    [InlineData("en-US", "1", "1st")]
    [InlineData("fr-FR", "1", "1er")]
    public void OrdinalizeStringWithCultureOverridesCurrentCulture(string cultureName, string number, string ordinalized)
    {
        var culture = new CultureInfo(cultureName);
        Assert.Equal(number.Ordinalize(culture), ordinalized);
    }

    [Theory]
    [InlineData("en-US", 1, "1st")]
    [InlineData("fr-FR", 1, "1er")]
    public void OrdinalizeNumberWithCultureOverridesCurrentCulture(string cultureName, int number, string ordinalized)
    {
        var culture = new CultureInfo(cultureName);
        Assert.Equal(number.Ordinalize(culture), ordinalized);
    }
}
