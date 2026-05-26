// -----------------------------------------------------------------------
// <copyright file="LocalizedSmartEnumTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Primitives;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class LocalizedSmartEnumTests
{
    [Fact]
    public void Value_ReturnsProvidedSmartEnum()
    {
        var sut = new LocalizedSmartEnum<StatusSmartEnum>(StatusSmartEnum.Active);

        Assert.Same(StatusSmartEnum.Active, sut.Value);
    }

    [Fact]
    public void Equals_SameSmartEnumValue_IsEqual()
    {
        var left = new LocalizedSmartEnum<StatusSmartEnum>(StatusSmartEnum.Active);
        var right = new LocalizedSmartEnum<StatusSmartEnum>(StatusSmartEnum.Active);

        Assert.True(left.Equals(right));
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void CultureChange_RaisesPropertyChangedForDisplay()
    {
        var sut = new LocalizedSmartEnum<StatusSmartEnum>(StatusSmartEnum.Active);
        var changed = new List<string>();
        sut.PropertyChanged += (_, e) => changed.Add(e.PropertyName ?? string.Empty);

        sut.OnEvent(new(new("fr-FR")));

        Assert.Contains(nameof(LocalizedSmartEnum<>.Display), changed);
    }

    private sealed class StatusSmartEnum(int value) : SmartEnum<StatusSmartEnum, int>(value)
    {
        public static readonly StatusSmartEnum Active = new(1);
    }
}
