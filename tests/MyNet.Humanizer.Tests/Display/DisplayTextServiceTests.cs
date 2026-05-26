// -----------------------------------------------------------------------
// <copyright file="DisplayTextServiceTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Moq;
using MyNet.Globalization.Culture;
using MyNet.Humanizer.Display;
using Xunit;

namespace MyNet.Humanizer.Tests.Display;

public sealed class DisplayTextServiceTests
{
    [Fact]
    public void GetDisplayText_UsesResolverStrategyAndCultureContext()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var strategy = new Mock<IDisplayTextStrategy<DayOfWeek>>();
        strategy
            .Setup(s => s.GetDisplayText(DayOfWeek.Monday, It.IsAny<DisplayTextOptions>(), culture))
            .Returns("Monday");

        var resolver = new Mock<IDisplayTextStrategyResolver>();
        resolver.Setup(r => r.GetRequired<DayOfWeek>()).Returns(strategy.Object);

        var cultureContext = new Mock<ICultureContext>();
        cultureContext.Setup(c => c.CurrentCulture).Returns(culture);

        var sut = new DisplayTextService(resolver.Object, cultureContext.Object);

        var result = sut.GetDisplayText(DayOfWeek.Monday, DisplayTextOptions.Default);

        Assert.Equal("Monday", result);
        strategy.Verify(s => s.GetDisplayText(DayOfWeek.Monday, It.IsAny<DisplayTextOptions>(), culture), Times.Once);
    }

    [Fact]
    public void GetDisplayText_WithExplicitCulture_PassesCultureToStrategy()
    {
        var explicitCulture = CultureInfo.GetCultureInfo("fr-FR");
        var strategy = new Mock<IDisplayTextStrategy<DayOfWeek>>();
        strategy
            .Setup(s => s.GetDisplayText(DayOfWeek.Monday, It.IsAny<DisplayTextOptions>(), explicitCulture))
            .Returns("lundi");

        var resolver = new Mock<IDisplayTextStrategyResolver>();
        resolver.Setup(r => r.GetRequired<DayOfWeek>()).Returns(strategy.Object);

        var cultureContext = new Mock<ICultureContext>();
        cultureContext.Setup(c => c.CurrentCulture).Returns(CultureInfo.InvariantCulture);

        var sut = new DisplayTextService(resolver.Object, cultureContext.Object);

        var result = sut.GetDisplayText(DayOfWeek.Monday, DisplayTextOptions.Default, explicitCulture);

        Assert.Equal("lundi", result);
    }
}
