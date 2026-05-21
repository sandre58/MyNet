// -----------------------------------------------------------------------
// <copyright file="CountryDisplayTextStrategyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using FluentAssertions;
using Moq;
using MyNet.Geography.Providers;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Humanizer.Display;
using MyNet.Utilities.Geography;
using Xunit;

namespace MyNet.Geography.Tests;

public sealed class CountryDisplayTextStrategyTests
{
    [Fact]
    public void GetDisplayName_ShouldReturnFrenchResourceValue()
    {
        var translationService = new Mock<ITranslationService>(MockBehavior.Strict);
        var keyProvider = new Mock<ITranslationKeyProvider>(MockBehavior.Strict);
        var sut = new CountryDisplayTextStrategy(translationService.Object, keyProvider.Object);

        var result = sut.GetDisplayText(Country.Germany, DisplayTextOptions.Default, CultureInfo.GetCultureInfo("fr-FR"));

        result.Should().Be("Allemagne");
        translationService.VerifyNoOtherCalls();
        keyProvider.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetDisplayName_WithSymbolStyle_ShouldFallbackToDisplayName()
    {
        var translationService = new Mock<ITranslationService>(MockBehavior.Strict);
        var keyProvider = new Mock<ITranslationKeyProvider>(MockBehavior.Strict);
        var sut = new CountryDisplayTextStrategy(translationService.Object, keyProvider.Object);

        var options = new DisplayTextOptions { Style = DisplayStyle.Symbol };
        var result = sut.GetDisplayText(Country.Germany, options, CultureInfo.GetCultureInfo("fr-FR"));

        result.Should().Be("Allemagne");
        translationService.VerifyNoOtherCalls();
        keyProvider.VerifyNoOtherCalls();
    }

    [Fact]
    public void GetDisplayName_ShouldReturnEnglishResourceValue()
    {
        var translationService = new Mock<ITranslationService>(MockBehavior.Strict);
        var keyProvider = new Mock<ITranslationKeyProvider>(MockBehavior.Strict);
        var sut = new CountryDisplayTextStrategy(translationService.Object, keyProvider.Object);

        var result = sut.GetDisplayText(Country.Germany, DisplayTextOptions.Default, CultureInfo.GetCultureInfo("en-US"));

        result.Should().Be("Germany");
    }
}
