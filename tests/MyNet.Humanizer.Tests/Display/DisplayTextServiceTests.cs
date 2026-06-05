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

    [Fact]
    public void GetDisplayText_WhenRuntimeTypeDiffersFromDeclaredType_UsesRuntimeTypeStrategy()
    {
        var culture = CultureInfo.InvariantCulture;
        var runtimeStrategy = new Mock<IDisplayTextStrategy>();
        runtimeStrategy
            .Setup(s => s.GetDisplayText(It.IsAny<object>(), It.IsAny<DisplayTextOptions>(), culture))
            .Returns("runtime");

        var resolver = new Mock<IDisplayTextStrategyResolver>();
        resolver.Setup(r => r.GetRequiredForType(typeof(DerivedType))).Returns(runtimeStrategy.Object);

        var cultureContext = new Mock<ICultureContext>();
        cultureContext.Setup(c => c.CurrentCulture).Returns(culture);

        var sut = new DisplayTextService(resolver.Object, cultureContext.Object);

        IContract value = new DerivedType();
        var result = sut.GetDisplayText(value, DisplayTextOptions.Default);

        Assert.Equal("runtime", result);
        resolver.Verify(r => r.GetRequiredForType(typeof(DerivedType)), Times.Once);
        resolver.Verify(r => r.GetRequired<IContract>(), Times.Never);
    }

    [Fact]
    public void GetDisplayText_WhenRuntimeTypeMatchesDeclaredType_UsesGenericStrategy()
    {
        var culture = CultureInfo.InvariantCulture;
        var strategy = new Mock<IDisplayTextStrategy<string>>();
        strategy
            .Setup(s => s.GetDisplayText(It.IsAny<string>(), It.IsAny<DisplayTextOptions>(), culture))
            .Returns("declared");

        var resolver = new Mock<IDisplayTextStrategyResolver>();
        resolver.Setup(r => r.GetRequired<string>()).Returns(strategy.Object);

        var cultureContext = new Mock<ICultureContext>();
        cultureContext.Setup(c => c.CurrentCulture).Returns(culture);

        var sut = new DisplayTextService(resolver.Object, cultureContext.Object);

        var result = sut.GetDisplayText("value", DisplayTextOptions.Default);

        Assert.Equal("declared", result);
        resolver.Verify(r => r.GetRequired<string>(), Times.Once);
        resolver.Verify(r => r.GetRequiredForType(It.IsAny<Type>()), Times.Never);
    }

    public interface IContract;

    public sealed class DerivedType : IContract;
}
