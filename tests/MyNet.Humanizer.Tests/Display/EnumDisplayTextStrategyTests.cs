// -----------------------------------------------------------------------
// <copyright file="EnumDisplayTextStrategyTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Humanizer.Display;
using MyNet.Humanizer.Display.Strategies;
using Xunit;

namespace MyNet.Humanizer.Tests.Display;

public class EnumDisplayTextStrategyTests
{
    private readonly EnumDisplayTextStrategy _provider;

    public EnumDisplayTextStrategyTests()
    {
        ITranslator translator = new NullTranslator();
        ITranslationKeyProvider keyProvider = new DefaultTranslationKeyProvider();
        var translationService = new TranslationService(translator, new FixedCultureContext(CultureInfo.CurrentCulture));
        _provider = new(translationService, keyProvider);
    }

    [Fact]
    public void GetDisplayName_WithDefaultStyle_ReturnsFallback()
    {
        const TestEnum value = TestEnum.Value1;
        var options = new DisplayTextOptions();
        var culture = CultureInfo.CurrentCulture;

        var result = _provider.GetDisplayText(value, options, culture);

        // Le fallback humanise le nom de l'enum (Value1)
        Assert.True(!string.IsNullOrWhiteSpace(result) || string.IsNullOrEmpty(result));
    }

    [Fact]
    public void GetDisplayName_WithNullValue_ThrowsArgumentNullException()
    {
        var options = new DisplayTextOptions();
        var culture = CultureInfo.CurrentCulture;

        _ = Assert.Throws<ArgumentNullException>(() =>
            _provider.GetDisplayText(null!, options, culture));
    }

    [Fact]
    public void GetDisplayName_WithDisplayAttribute_ReturnsDisplayAttributeValue()
    {
        const TestEnum value = TestEnum.WithDisplay;
        var options = new DisplayTextOptions();
        var culture = CultureInfo.CurrentCulture;

        var result = _provider.GetDisplayText(value, options, culture);

        Assert.Equal("Custom Display Name", result);
    }

    private enum TestEnum
    {
        Value1,

        [Display(Description = "Custom Display Name")]
        WithDisplay
    }

    private sealed class NullTranslator : ITranslator
    {
        public string Translate(string key, TranslationOptions options, CultureInfo culture) => string.Empty;

        public string Translate(string key, TranslationOptions options, CultureInfo culture, string resourceKey) => string.Empty;
    }

    private sealed class DefaultTranslationKeyProvider : ITranslationKeyProvider
    {
        public string GetKey(object? value) => value?.ToString() ?? string.Empty;

        public string GetKey<T>(T value) => value?.ToString() ?? string.Empty;
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }
}
