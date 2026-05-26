// -----------------------------------------------------------------------
// <copyright file="LocalizedEnumTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class LocalizedEnumTests
{
    [Fact]
    public void Value_ReturnsProvidedEnum()
    {
        var sut = new LocalizedEnum<DayOfWeek>(DayOfWeek.Tuesday);

        Assert.Equal(DayOfWeek.Tuesday, sut.Value);
    }

    [Fact]
    public void Equals_SameEnumValue_IsEqual()
    {
        var left = new LocalizedEnum<DayOfWeek>(DayOfWeek.Friday);
        var right = new LocalizedEnum<DayOfWeek>(DayOfWeek.Friday);

        Assert.True(left.Equals(right));
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void CultureChange_RaisesPropertyChangedForDisplayAndDescription()
    {
        var sut = new LocalizedEnum<DayOfWeek>(DayOfWeek.Monday);
        var changed = new List<string>();
        sut.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        sut.OnEvent(new(new("fr-FR")));

        Assert.Contains(nameof(LocalizedEnum<>.Display), changed);
        Assert.Contains(nameof(LocalizedEnum<>.Description), changed);
    }

    private enum SampleDescribedEnum
    {
        [Description("Sample description")]
        Value = 1
    }

    [Fact]
    public void Description_ReturnsDescriptionAttributeText()
    {
        var sut = new LocalizedEnum<SampleDescribedEnum>(SampleDescribedEnum.Value);

        Assert.Equal("Sample description", sut.Description);
    }
}
