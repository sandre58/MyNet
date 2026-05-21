// -----------------------------------------------------------------------
// <copyright file="TranslatorTests.cs" company="Stéphane ANDRE">
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

public class TranslatorTests
{
    [Fact]
    public void Translate_UnknownKey_ReturnsKeyByDefault()
    {
        var translator = CreateTranslator(
            new StubKeyResolver(new("Unknown", TranslationFallbackPolicy.Flexible)));

        var result = translator.Translate("Unknown", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("Unknown", result);
    }

    [Fact]
    public void Translate_WithResourceKey_UsesOnlyTargetResource()
    {
        var translator = CreateTranslator(
            new StubKeyResolver(new("Label", TranslationFallbackPolicy.Flexible)), ("A", new InMemoryResourceManager(new() { ["en-US"] = new() { ["Label"] = "A value" } })), ("B", new InMemoryResourceManager(new() { ["en-US"] = new() { ["Label"] = "B value" } })));

        var result = translator.Translate("Label", TranslationOptionsPresets.Default, new("en-US"), "B");

        Assert.Equal("B value", result);
    }

    [Fact]
    public void Translate_WithCultureFallback_UsesParentCulture()
    {
        var translator = CreateTranslator(
            new StubKeyResolver(new("Greeting", TranslationFallbackPolicy.Flexible)), ("Main", new InMemoryResourceManager(new() { ["en"] = new() { ["Greeting"] = "Hello from parent" } })));

        var result = translator.Translate("Greeting", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("Hello from parent", result);
    }

    [Fact]
    public void Translate_FlexiblePolicy_AppliesInflectionFallback()
    {
        var options = new TranslationOptionsBuilder().WithQuantity(2).Build();
        var translator = CreateTranslator(
            new StubKeyResolver(new("Word", TranslationFallbackPolicy.Flexible)), ("Main", new InMemoryResourceManager(new() { ["en-US"] = new() { ["Word"] = "person" } })));

        var result = translator.Translate("Word", options, new("en-US"));

        Assert.Equal("people", result);
    }

    [Fact]
    public void Translate_StrictPolicy_DoesNotApplyInflectionFallback()
    {
        var options = new TranslationOptionsBuilder().WithQuantity(2).Build();
        var translator = CreateTranslator(
            new StubKeyResolver(new("Word", TranslationFallbackPolicy.Strict)), ("Main", new InMemoryResourceManager(new() { ["en-US"] = new() { ["Word"] = "person" } })));

        var result = translator.Translate("Word", options, new("en-US"));

        Assert.Equal("person", result);
    }

    [Fact]
    public void Translate_RendersTemplateArguments()
    {
        var options = new TranslationOptionsBuilder().WithQuantity(1234).Build();
        var translator = CreateTranslator(
            new StubKeyResolver(new("Template", TranslationFallbackPolicy.Flexible)), ("Main", new InMemoryResourceManager(new() { ["fr-FR"] = new() { ["Template"] = "Valeur {quantity:N0}" } })));

        var result = translator.Translate("Template", options, new("fr-FR"));

        var normalized = result.Replace('\u202F', ' ').Replace('\u00A0', ' ');
        Assert.StartsWith("Valeur 1 234", normalized, StringComparison.Ordinal);
    }

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
        var resolver = new LocalizationServiceResolver(registry);

        return new(catalog, keyResolver, new PluralizationService(resolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class StubKeyResolver(TranslationKeyCandidate candidate) : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture) => [candidate];

        public string ResolvePluralizedKey(string baseKey, decimal count, CultureInfo culture) => throw new NotSupportedException();

        public string ResolveStyledKey(string baseKey, DisplayStyle style, CultureInfo culture) => throw new NotSupportedException();
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
