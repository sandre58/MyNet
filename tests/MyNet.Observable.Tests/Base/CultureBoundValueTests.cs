// -----------------------------------------------------------------------
// <copyright file="CultureBoundValueTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using MyNet.Globalization.Culture;
using Xunit;

namespace MyNet.Observable.Tests.Base;

public sealed class CultureBoundValueTests
{
    [Fact]
    public void Value_IsCachedUntilCultureChanges()
    {
        var cultureService = new FakeCultureService(CultureInfo.InvariantCulture);
        var computeCount = 0;
        var sut = new CultureBoundValue<string>(() =>
        {
            computeCount++;
            return cultureService.CurrentCulture.Name;
        },
            cultureService);

        _ = sut.Value;
        _ = sut.Value;
        Assert.Equal(1, computeCount);

        cultureService.Raise(CultureInfo.InvariantCulture, new("fr-FR"));
        _ = sut.Value;
        Assert.Equal(2, computeCount);
    }

    [Fact]
    public void CultureChange_RaisesPropertyChangedForValue()
    {
        var cultureService = new FakeCultureService(CultureInfo.InvariantCulture);
        var sut = new CultureBoundValue<string>(() => cultureService.CurrentCulture.Name, cultureService);

        var changedProperties = new List<string>();
        sut.PropertyChanged += (_, e) => changedProperties.Add(e.PropertyName ?? string.Empty);

        _ = sut.Value;
        cultureService.Raise(CultureInfo.InvariantCulture, new("fr-FR"));

        Assert.Contains(nameof(CultureBoundValue<>.Value), changedProperties);
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
