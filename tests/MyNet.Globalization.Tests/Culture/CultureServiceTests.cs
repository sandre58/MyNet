// -----------------------------------------------------------------------
// <copyright file="CultureServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Globalization.Culture;
using Xunit;

namespace MyNet.Globalization.Tests.Culture;

public sealed class CultureServiceTests
{
    [Fact]
    public void Constructor_WithCulture_SetsCurrentCulture()
    {
        var expected = CultureInfo.GetCultureInfo("fr-FR");
        var sut = new CultureService(expected);

        Assert.Equal(expected, sut.CurrentCulture);
    }

    [Fact]
    public void Constructor_WithGlobalizationOptions_UsesDefaultCulture()
    {
        var expected = CultureInfo.GetCultureInfo("de-DE");
        var options = new GlobalizationOptions { DefaultCulture = expected };
        var sut = new CultureService(options);

        Assert.Equal(expected, sut.CurrentCulture);
    }

    [Fact]
    public void SetCulture_UpdatesCurrentCultureAndThreadCultures()
    {
        var sut = new CultureService(CultureInfo.InvariantCulture);
        var target = CultureInfo.GetCultureInfo("es-ES");

        sut.SetCulture(target);

        Assert.Equal(target, sut.CurrentCulture);
        Assert.Equal(target, CultureInfo.CurrentCulture);
        Assert.Equal(target, CultureInfo.CurrentUICulture);
    }

    [Fact]
    public void SetCulture_WithCultureCode_ResolvesCulture()
    {
        var sut = new CultureService(CultureInfo.InvariantCulture);

        sut.SetCulture("it-IT");

        Assert.Equal("it-IT", sut.CurrentCulture.Name);
    }

    [Fact]
    public void SetCulture_SameCulture_DoesNotRaiseCultureChanged()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var sut = new CultureService(culture);
        var eventCount = 0;
        sut.CultureChanged += (_, _) => eventCount++;

        sut.SetCulture(culture);

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void SetCulture_DifferentCulture_RaisesCultureChangedWithArgs()
    {
        var oldCulture = CultureInfo.GetCultureInfo("en-US");
        var newCulture = CultureInfo.GetCultureInfo("fr-FR");
        var sut = new CultureService(oldCulture);
        CultureChangedEventArgs? captured = null;
        sut.CultureChanged += (_, args) => captured = args;

        sut.SetCulture(newCulture);

        Assert.NotNull(captured);
        Assert.Equal(oldCulture, captured.OldCulture);
        Assert.Equal(newCulture, captured.NewCulture);
    }

    [Fact]
    public void SetCulture_NullCulture_ThrowsArgumentNullException()
    {
        var sut = new CultureService(CultureInfo.InvariantCulture);

        Assert.Throws<ArgumentNullException>(() => sut.SetCulture((CultureInfo)null!));
    }
}
