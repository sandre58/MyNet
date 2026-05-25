// -----------------------------------------------------------------------
// <copyright file="LocalizedStringTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Globalization.Culture;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class LocalizedStringTests
{
    [Fact]
    public void Value_UsesKeyForEquality_NotTranslatedText()
    {
        var left = new LocalizedString("Same.Key");
        var right = new LocalizedString("same.key");

        Assert.True(left.Equals(right));
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void CultureChange_RaisesPropertyChangedForValue()
    {
        var cultureService = new FakeCultureService(CultureInfo.InvariantCulture);
        var sut = new LocalizedString("Test.Key", cultureService);

        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        _ = sut.Value;
        cultureService.Raise(CultureInfo.InvariantCulture, new("fr-FR"));

        Assert.Contains(nameof(LocalizedString.Value), changedProperties);
    }

    private sealed class FakeCultureService(CultureInfo initialCulture) : ICultureService
    {
        public CultureInfo CurrentCulture { get; private set; } = initialCulture;

        public event EventHandler<CultureChangedEventArgs>? CultureChanged;

        public void SetCulture(CultureInfo culture) => CurrentCulture = culture;

        public void SetCulture(string cultureCode) => CurrentCulture = CultureInfo.GetCultureInfo(cultureCode);

        public void Raise(CultureInfo oldCulture, CultureInfo newCulture)
        {
            CurrentCulture = newCulture;
            CultureChanged?.Invoke(this, new(oldCulture, newCulture));
        }
    }
}
