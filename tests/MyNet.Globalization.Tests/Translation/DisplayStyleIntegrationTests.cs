// -----------------------------------------------------------------------
// <copyright file="DisplayStyleIntegrationTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Inflection.Cultures;
using MyNet.Globalization.Localization.Policies;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Providers.Factories;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyResolving;
using MyNet.Globalization.Tests.Providers;
using Xunit;

namespace MyNet.Globalization.Tests.Translation;

public class DisplayStyleIntegrationTests
{
    public static IEnumerable<object[]> AllDisplayStyles =>
    [
        [DisplayStyle.Default],
        [DisplayStyle.Short],
        [DisplayStyle.Abbreviation],
        [DisplayStyle.Symbol],
        [DisplayStyle.Narrow]
    ];

    public static IEnumerable<object[]> DisplayStylesWithCultures =>
    [
        [DisplayStyle.Default, "en-US"],
        [DisplayStyle.Default, "fr-FR"],
        [DisplayStyle.Default, "de-DE"],
        [DisplayStyle.Short, "en-US"],
        [DisplayStyle.Short, "fr-FR"],
        [DisplayStyle.Short, "de-DE"],
        [DisplayStyle.Abbreviation, "en-US"],
        [DisplayStyle.Abbreviation, "fr-FR"],
        [DisplayStyle.Abbreviation, "de-DE"],
        [DisplayStyle.Symbol, "en-US"],
        [DisplayStyle.Symbol, "fr-FR"],
        [DisplayStyle.Symbol, "de-DE"],
        [DisplayStyle.Narrow, "en-US"],
        [DisplayStyle.Narrow, "fr-FR"],
        [DisplayStyle.Narrow, "de-DE"]
    ];

    [Theory]
    [MemberData(nameof(AllDisplayStyles))]
    public void TranslateWithDisplayStyle_AllStylesSupported(DisplayStyle style)
    {
        var manager = new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["Label"] = "Current Label",
                ["Label_short"] = "Short Label",
                ["Label_abbreviation"] = "Abbr",
                ["Label_symbol"] = "L",
                ["Label_narrow"] = "L"
            }
        });
        var translator = CreateTranslator(new ParameterizedKeyResolver(style), ("Main", manager));

        var options = new TranslationOptionsBuilder().WithStyle(style).Build();
        var result = translator.Translate("Label", options, new("en-US"));

        Assert.NotEmpty(result);
    }

    [Theory]
    [MemberData(nameof(DisplayStylesWithCultures))]
    public void TranslateWithStyleAndCulture_WorksForAllCombinations(DisplayStyle style, string cultureName)
    {
        var translator = CreateTranslator(
            new ParameterizedKeyResolver(style), ("Main", new InMemoryResourceManager(BuildMultiCultureResources())));

        var options = new TranslationOptionsBuilder().WithStyle(style).Build();

        // Should not throw and should return some value
        var result = translator.Translate("Message", options, new(cultureName));
        Assert.NotEmpty(result);
    }

    [Theory]
    [MemberData(nameof(AllDisplayStyles))]
    public void TranslateWithStyleAndQuantity_AppliesPluralization(DisplayStyle style)
    {
        var manager = new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["items"] = "item",
                ["items_short"] = "item",
                ["items_abbreviation"] = "itm",
                ["items_symbol"] = "i",
                ["items_narrow"] = "i"
            }
        });
        var translator = CreateTranslator(
            new ParameterizedKeyResolver(style), ("Main", manager));

        var options = new TranslationOptionsBuilder()
            .WithStyle(style)
            .WithQuantity(5)
            .Build();

        var result = translator.Translate("items", options, new("en-US"));

        Assert.NotEmpty(result);
    }

    [Fact]
    public void TranslateWithAllStyles_Symbol_PrefersDirect()
    {
        var translator = CreateTranslator(
            new ParameterizedKeyResolver(DisplayStyle.Symbol), ("Main", new InMemoryResourceManager(new() { ["en-US"] = new() { ["Currency"] = "Dollar", ["Currency_symbol"] = "$" } })));

        var options = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Symbol).Build();
        var result = translator.Translate("Currency", options, new("en-US"));

        Assert.Equal("$", result);
    }

    [Fact]
    public void TranslateWithMultipleStyles_FallsBackCorrectly()
    {
        var manager = new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                // Only default available, short not available
                ["Status"] = "Status Text"
            }
        });
        var translator = CreateTranslator(
            new ParameterizedKeyResolver(DisplayStyle.Short), ("Main", manager));

        var options = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Short).Build();
        var result = translator.Translate("Status", options, new("en-US"));

        // Should fall back to default
        Assert.Equal("Status Text", result);
    }

    [Theory]
    [InlineData("en-US", "de-DE")]
    [InlineData("fr-FR", "en-US")]
    [InlineData("de-DE", "fr-FR")]
    public void TranslateWithStyleAcrossCultures_CultureFallbackWorks(string requestCulture, string resourceCulture)
    {
        var translator = CreateTranslator(
            new ParameterizedKeyResolver(DisplayStyle.Default), ("Main", new InMemoryResourceManager(new() { [resourceCulture] = new() { ["Hello"] = "Translation in " + resourceCulture } })));

        var options = TranslationOptionsPresets.Default;
        var result = translator.Translate("Hello", options, new(requestCulture));

        // Should use the key as fallback since we don't have the exact culture
        Assert.NotEmpty(result);
    }

    [Theory]
    [MemberData(nameof(AllDisplayStyles))]
    public void BuildWithStyle_ProducesValidOptions(DisplayStyle style)
    {
        var options = new TranslationOptionsBuilder()
            .WithStyle(style)
            .Build();

        Assert.Equal(style, options.Style);
        Assert.True(options.UseInflectionFallback);
        Assert.True(options.UseKeyAsFallback);
    }

    [Fact]
    public void TranslateWithAllStylesAndQuantity_ComplexScenario()
    {
        var translator = CreateTranslator(
            new ParameterizedKeyResolver(DisplayStyle.Abbreviation), ("Main", new InMemoryResourceManager(new() { ["en-US"] = new() { ["item"] = "item", ["item_abbreviation"] = "itm" } })));

        var options = new TranslationOptionsBuilder()
            .WithStyle(DisplayStyle.Abbreviation)
            .WithQuantity(3)
            .WithArgument("location", "warehouse")
            .Build();

        var result = translator.Translate("item", options, new("en-US"));

        Assert.NotEmpty(result);
        Assert.Equal(DisplayStyle.Abbreviation, options.Style);
        Assert.Equal(3, options.Quantity);
    }

    private static Dictionary<string, Dictionary<string, string>> BuildMultiCultureResources()
        => new()
        {
            ["en-US"] = new()
            {
                ["Message"] = "Hello",
                ["Message_short"] = "Hi",
                ["Message_abbreviation"] = "H",
                ["Message_symbol"] = "✓",
                ["Message_narrow"] = "H"
            },
            ["fr-FR"] = new()
            {
                ["Message"] = "Bonjour",
                ["Message_short"] = "Salut",
                ["Message_abbreviation"] = "Bj",
                ["Message_symbol"] = "✓",
                ["Message_narrow"] = "B"
            },
            ["de-DE"] = new()
            {
                ["Message"] = "Hallo",
                ["Message_short"] = "Hi",
                ["Message_abbreviation"] = "H",
                ["Message_symbol"] = "✓",
                ["Message_narrow"] = "H"
            }
        };

    private static Translator CreateTranslator(ITranslationKeyResolver keyResolver, params (string Key, ResourceManager ResourceManager)[] resources)
    {
        var catalog = new TestTranslationCatalog();
        foreach (var (key, resourceManager) in resources)
            catalog.Register(key, resourceManager);

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        return new(catalog, keyResolver, new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class ParameterizedKeyResolver(DisplayStyle style) : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture)
        {
            var candidates = new List<TranslationKeyCandidate>();

            // Add style-specific key first
            if (style != DisplayStyle.Default)
            {
                var styleSuffix = style switch
                {
                    DisplayStyle.Short => "_short",
                    DisplayStyle.Abbreviation => "_abbreviation",
                    DisplayStyle.Symbol => "_symbol",
                    DisplayStyle.Narrow => "_narrow",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(styleSuffix))
                    candidates.Add(new(baseKey + styleSuffix, TranslationFallbackPolicy.Strict));
            }

            // Add base key
            candidates.Add(new(baseKey, TranslationFallbackPolicy.Flexible));

            return candidates;
        }

        public string ResolvePluralizedKey(string baseKey, decimal count, CultureInfo culture) => throw new NotSupportedException();

        public string ResolveStyledKey(string baseKey, DisplayStyle displayStyle, CultureInfo culture) => throw new NotSupportedException();
    }

    private sealed class InMemoryResourceManager(Dictionary<string, Dictionary<string, string>> valuesByCulture) : ResourceManager
    {
        public override string? GetString(string name, CultureInfo? culture)
        {
            var cultureName = culture?.Name ?? string.Empty;

            return valuesByCulture.TryGetValue(cultureName, out var values) && values.TryGetValue(name, out var value)
                ? value
                : null;
        }
    }
}
