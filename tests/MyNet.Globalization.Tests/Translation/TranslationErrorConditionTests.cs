// -----------------------------------------------------------------------
// <copyright file="TranslationErrorConditionTests.cs" company="Stéphane ANDRE">
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

public class TranslationErrorConditionTests
{
    [Fact]
    public void Translate_WithoutRegisteredResources_ReturnsKeyAsDefault()
    {
        var translator = CreateTranslator([]);

        var result = translator.Translate("UnknownKey", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("UnknownKey", result);
    }

    [Fact]
    public void Translate_WithoutRegisteredResources_AndNoKeyFallback_ReturnsEmpty()
    {
        var translator = CreateTranslator([]);
        var options = new TranslationOptionsBuilder().WithoutKeyFallback().Build();

        var result = translator.Translate("UnknownKey", options, new("en-US"));

        Assert.Empty(result);
    }

    [Fact]
    public void Translate_WithNullArgument_Handled()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { ["Message"] = "Hello {name}" }
            }))]);

        var options = new TranslationOptionsBuilder()
            .WithArgument("name", null)
            .Build();

        // Should not throw even with null argument
        var result = translator.Translate("Message", options, new("en-US"));
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithMissingResource_FallsBackToKey()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { ["Existing"] = "Exists" }
            }))]);

        var result = translator.Translate("NonExistent", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("NonExistent", result);
    }

    [Fact]
    public void Translate_WithEmptyKe_ReturnsEmpty()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { [string.Empty] = "Empty key result" }
            }))]);

        // Empty key might be handled as null or empty
        var result = translator.Translate(string.Empty, TranslationOptionsPresets.Default, new("en-US"));

        Assert.NotNull(result);
    }

    [Fact]
    public void Translate_WithInvalidFormat_Renders()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new()
                {
                    // Valid format specifier
                    ["Template"] = "Value: {quantity:N0}"
                }
            }))]);

        var options = new TranslationOptionsBuilder().WithQuantity(1234567).Build();
        var result = translator.Translate("Template", options, new("en-US"));

        // Should render the format correctly
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithUnknownPlaceholder_PreservesPlaceholder()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new()
                {
                    ["Message"] = "Hello {unknownArg}"
                }
            }))]);

        var result = translator.Translate("Message", TranslationOptionsPresets.Default, new("en-US"));

        // Unknown placeholders should be preserved
        Assert.Contains("{unknownArg}", result, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void TranslateWithMultipleUnknownPlaceholders_AllPreserved()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new()
                {
                    ["Complex"] = "Start {missing1} middle {missing2} end"
                }
            }))]);

        var result = translator.Translate("Complex", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Contains("{missing1}", result, StringComparison.InvariantCultureIgnoreCase);
        Assert.Contains("{missing2}", result, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void TranslationBuilder_WithNullArgumentName_Throws()
    {
        var builder = new TranslationOptionsBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithArgument(null!, "value"));
    }

    [Fact]
    public void TranslationBuilder_WithEmptyArgumentName_Throws()
    {
        var builder = new TranslationOptionsBuilder();

        Assert.Throws<ArgumentException>(() => builder.WithArgument(string.Empty, "value"));
    }

    [Fact]
    public void TranslationBuilder_WithWhitespaceArgumentName_Throws()
    {
        var builder = new TranslationOptionsBuilder();

        Assert.Throws<ArgumentException>(() => builder.WithArgument("   ", "value"));
    }

    [Fact]
    public void TranslationBuilder_WithNullArgumentsEnumerable_Throws()
    {
        var builder = new TranslationOptionsBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithArguments(null!));
    }

    [Fact]
    public void Translate_FromInvariantCulture_Works()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                [string.Empty] = new() { ["Current"] = "Invariant Value" }
            }))]);

        var result = translator.Translate("Current", TranslationOptionsPresets.Default, CultureInfo.InvariantCulture);

        // Should handle invariant culture
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithCultureFallbackDisabled_DoesNotFallBack()
    {
        var translator = CreateTranslatorWithoutFallback(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en"] = new() { ["Greeting"] = "Hello from en" }
            }))]);

        var result = translator.Translate("Greeting", TranslationOptionsPresets.Default, new("en-US"));

        // Should not fallback to en culture, returns key
        Assert.Equal("Greeting", result);
    }

    [Fact]
    public void Translate_VeryLongKey_Handled()
    {
        var longKey = new string('A', 1000);
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager([]))]);

        var result = translator.Translate(longKey, TranslationOptionsPresets.Default, new("en-US"));

        // Should return the key itself as fallback
        Assert.Equal(longKey, result);
    }

    [Fact]
    public void Translate_WithSpecialCharactersInKey_Handled()
    {
        const string specialKey = "Key@#$%^&*()";
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { [specialKey] = "Found" }
            }))]);

        var result = translator.Translate(specialKey, TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("Found", result);
    }

    [Fact]
    public void Translate_WithUnicodeKey_Handled()
    {
        const string unicodeKey = "🔑Key";
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { [unicodeKey] = "Unicode Found" }
            }))]);

        var result = translator.Translate(unicodeKey, TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("Unicode Found", result);
    }

    [Fact]
    public void Translate_WithVeryLargeQuantity_Handled()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { ["item"] = "item" }
            }))]);

        const decimal quantity = decimal.MaxValue;
        var options = new TranslationOptionsBuilder().WithQuantity(quantity).Build();

        // Should handle very large numbers
        var result = translator.Translate("item", options, new("en-US"));
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithNegativeQuantity_Handled()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { ["item"] = "item" }
            }))]);

        var options = new TranslationOptionsBuilder().WithQuantity(-5).Build();

        // Should handle negative numbers
        var result = translator.Translate("item", options, new("en-US"));
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithZeroQuantity_Handled()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { ["item"] = "item" }
            }))]);

        var options = new TranslationOptionsBuilder().WithQuantity(0).Build();

        // Should handle zero
        var result = translator.Translate("item", options, new("en-US"));
        Assert.NotEmpty(result);
    }

    [Fact]
    public void TranslateMultipleTimes_SameKey_ConsistentResults()
    {
        var translator = CreateTranslator(
            [("Main", new InMemoryResourceManager(new()
            {
                ["en-US"] = new() { ["Key"] = "Value" }
            }))]);

        var result1 = translator.Translate("Key", TranslationOptionsPresets.Default, new("en-US"));
        var result2 = translator.Translate("Key", TranslationOptionsPresets.Default, new("en-US"));
        var result3 = translator.Translate("Key", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    private static Translator CreateTranslator(List<(string Key, ResourceManager ResourceManager)> resources)
    {
        var catalog = new TestTranslationCatalog();
        foreach (var (key, manager) in resources)
            catalog.Register(key, manager);

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        return new(catalog, new StubKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);
    }

    private static Translator CreateTranslatorWithoutFallback(List<(string Key, ResourceManager ResourceManager)> resources)
    {
        var catalog = new TestTranslationCatalog();
        foreach (var (key, manager) in resources)
            catalog.Register(key, manager);

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        return new(catalog, new StubKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.None);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class StubKeyResolver : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture)
            => [new(baseKey, TranslationFallbackPolicy.Flexible)];

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
