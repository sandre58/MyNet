// -----------------------------------------------------------------------
// <copyright file="EnumExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyNet.Globalization.Localization.Translation;
using MyNet.Humanizer.Static;
using MyNet.Utilities;
using Xunit;

namespace MyNet.Humanizer.Tests.Extensions;

[Collection("UseCultureSequential")]
public class EnumExtensionsTests
{
    private enum DescriptionEnum
    {
        [Description("Custom description")]
        WithDescription
    }

    private enum Dummy
    {
        // ReSharper disable once UnusedMember.Local
        First,

        // ReSharper disable once UnusedMember.Local
        Second
    }

    [Fact]
    public void Humanize_UsesDisplayDescription() => Assert.Equal(EnumTestsResources.MemberWithDisplayAttribute, EnumUnderTest.MemberWithDisplayAttribute.Humanize());

    [Fact]
    public void Humanize_UsesDisplayNameWhenDescriptionMissing() => Assert.Equal(EnumTestsResources.MemberWithDisplayAttributeWithoutDescription, EnumUnderTest.MemberWithDisplayAttributeWithoutDescription.Humanize());

    [Fact]
    public void ThrowsForEnumNoMatch() => _ = Assert.Throws<KeyNotFoundException>(() => "does not exist".DehumanizeTo<Dummy>());

    [Fact]
    public void ThrowsForNullOrWhiteSpaceInput() => _ = Assert.Throws<ArgumentException>(() => " ".DehumanizeTo<EnumUnderTest>());

    [Fact]
    public void DehumanizeMembersWithoutDescriptionAttribute() => Assert.Equal(EnumUnderTest.MemberWithoutDescriptionAttribute, nameof(EnumUnderTest.MemberWithoutDescriptionAttribute).DehumanizeTo<EnumUnderTest>());

    [Fact]
    public void AllCapitalMembersAreReturnedAsIs() => Assert.Equal(EnumUnderTest.ALLCAPITALS, nameof(EnumUnderTest.ALLCAPITALS).DehumanizeTo<EnumUnderTest>());

    [Fact]
    public void HonorsDisplayAttribute() => Assert.Equal(EnumUnderTest.MemberWithDisplayAttribute, EnumTestsResources.MemberWithDisplayAttribute.DehumanizeTo<EnumUnderTest>());

    [Fact]
    public void EnumHumanize_WithAbbreviationStyle_UsesTranslationKeySuffix()
    {
        var result = TimeUnit.Minute.Humanize(new() { Style = DisplayStyle.Abbreviation }, new("en-US"));

        Assert.Equal("min", result);
    }

    [Fact]
    public void EnumHumanize_WithDefaultStyle_UsesLocalizedTranslation()
    {
        var result = TimeUnit.Month.Humanize(culture: new("fr-FR"));

        Assert.Equal("Mois", result);
    }

    [Fact]
    public void GetDescription_ReturnsDescriptionAttributeValue()
        => Assert.Equal("Custom description", DescriptionEnum.WithDescription.GetDescription(), StringComparer.Ordinal);

    [Fact]
    public void TryDehumanizeTo_WithKnownValue_ReturnsTrueAndValue()
    {
        var success = nameof(EnumUnderTest.MemberWithoutDescriptionAttribute)
            .TryDehumanizeTo<EnumUnderTest>(out var value);

        Assert.True(success);
        Assert.Equal(EnumUnderTest.MemberWithoutDescriptionAttribute, value);
    }

    [Fact]
    public void TryDehumanizeTo_WithUnknownValue_ReturnsFalseAndNullValue()
    {
        var success = "unknown-value".TryDehumanizeTo<EnumUnderTest>(out var value);

        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void DehumanizeToType_WithNonEnumType_ThrowsArgumentException()
        => _ = Assert.Throws<ArgumentException>(() => MyNet.Humanizer.Static.EnumExtensions.DehumanizeTo("value", typeof(string)));
}

internal enum EnumUnderTest
{
    MemberWithDescriptionAttributeSubclass,
    [CustomDescription(EnumTestsResources.MemberWithCustomDescriptionAttribute)]
    MemberWithCustomDescriptionAttribute,
    [ImposterDescription(42)]
    MemberWithImposterDescriptionAttribute,
    [CustomProperty(EnumTestsResources.MemberWithCustomPropertyAttribute)]
    MemberWithCustomPropertyAttribute,
    MemberWithoutDescriptionAttribute,

    // ReSharper disable once InconsistentNaming
    ALLCAPITALS,
    [Display(Description = EnumTestsResources.MemberWithDisplayAttribute)]
    MemberWithDisplayAttribute,
    [Display(Description = "MemberWithLocalizedDisplayAttribute", ResourceType = typeof(EnumTestsResources))]
    MemberWithLocalizedDisplayAttribute,
    [Display(Name = EnumTestsResources.MemberWithDisplayAttributeWithoutDescription)]
    MemberWithDisplayAttributeWithoutDescription
}

internal abstract class EnumTestsResources
{
    public const string MemberWithDescriptionAttribute = "Some Description";
    public const string MemberWithDescriptionAttributeSubclass = "Description in Description subclass";
    public const string MemberWithCustomDescriptionAttribute = "Description in custom Description attribute";
    public const string MemberWithImposterDescriptionAttribute = "Member with imposter description attribute";
    public const string MemberWithCustomPropertyAttribute = "Description in custom property attribute";
    public const string MemberWithoutDescriptionAttributeSentence = "Member without description attribute";
    public const string MemberWithoutDescriptionAttributeTitle = "Member Without Description Attribute";
    public const string MemberWithoutDescriptionAttributeLowerCase = "member without description attribute";
    public const string MemberWithDisplayAttribute = "Description from Display attribute";
    public const string MemberWithDisplayAttributeWithoutDescription = "Displayattribute without description";

    public static string MemberWithLocalizedDisplayAttribute => "Localized description from Display attribute";
}

[AttributeUsage(AttributeTargets.All)]
internal sealed class ImposterDescriptionAttribute(int description) : Attribute
{
    public int Description { get; } = description;
}

[AttributeUsage(AttributeTargets.All)]
internal sealed class CustomDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}

[AttributeUsage(AttributeTargets.All)]
internal sealed class CustomPropertyAttribute(string info) : Attribute
{
    public string Info { get; } = info;
}
