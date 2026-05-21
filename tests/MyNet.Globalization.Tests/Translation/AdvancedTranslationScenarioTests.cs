// -----------------------------------------------------------------------
// <copyright file="AdvancedTranslationScenarioTests.cs" company="Stéphane ANDRE">
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
using MyNet.Globalization.Localization.Translation.Catalog;
using MyNet.Globalization.Localization.Translation.KeyResolving;
using MyNet.Globalization.Tests.Providers;
using Xunit;

namespace MyNet.Globalization.Tests.Translation;

/// <summary>
/// Advanced test scenarios combining multiple complex features of the translation system.
/// </summary>
public class AdvancedTranslationScenarioTests
{
    [Fact]
    public void Scenario_InternationalECommerceProduct_ComplexTranslation()
    {
        // Simulate translating a product description with quantity pricing
        var catalog = new TestTranslationCatalog();
        catalog.Register("Products", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["price_template"] = "{currency} {amount:N2}",
                ["bulk_discount"] = "Buy {quantity} or more and save {percent_value}%",
                ["shipping"] = "item",
                ["estimated_delivery"] = "Estimated delivery: {quantity} business day" // Will pluralize
            },
            ["fr-FR"] = new()
            {
                ["price_template"] = "{amount:N2} {currency}",
                ["bulk_discount"] = "Achetez {quantity} articles ou plus et économisez {percent_value}%",
                ["shipping"] = "article",
                ["estimated_delivery"] = "Livraison estimée : {quantity} jour ouvrable"
            },
            ["de-DE"] = new()
            {
                ["price_template"] = "{amount:N2} {currency}",
                ["bulk_discount"] = "Kaufen Sie {quantity} oder mehr und sparen Sie {percent_value}%",
                ["shipping"] = "artikel",
                ["estimated_delivery"] = "Geschätzter Versand: {quantity} Geschäftstag"
            }
        }));

        var translator = CreateTranslator(catalog);

        // Test 1: Price formatting for different currencies
        var priceOptions = new TranslationOptionsBuilder()
            .WithArgument("currency", "USD")
            .WithArgument("amount", 29.99m)
            .Build();

        var enPrice = translator.Translate("price_template", priceOptions, new("en-US"));
        Assert.Contains("USD", enPrice, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("29.99", enPrice, StringComparison.CurrentCultureIgnoreCase);

        var frPrice = translator.Translate("price_template", priceOptions, new("fr-FR"));
        Assert.Contains("29,99", frPrice, StringComparison.CurrentCultureIgnoreCase);

        // Test 2: Bulk discount message
        var discountOptions = new TranslationOptionsBuilder()
            .WithQuantity(5)
            .WithArgument("percent_value", 15)
            .Build();

        var discount = translator.Translate("bulk_discount", discountOptions, new("en-US"));
        Assert.Contains("5", discount, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("15", discount, StringComparison.CurrentCultureIgnoreCase);

        // Test 3: Shipping with pluralization
        var shippingOptions = new TranslationOptionsBuilder()
            .WithQuantity(3)
            .Build();

        var shipping = translator.Translate("shipping", shippingOptions, new("en-US"));
        Assert.NotEmpty(shipping);
    }

    [Fact]
    public void Scenario_MultiLanguageDocumentation_WithCodeExamples()
    {
        // Simulate translating documentation with code blocks and special formatting
        var catalog = new TestTranslationCatalog();
        catalog.Register("Docs", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["tutorial_intro"] = "Welcome to {product_name}. This guide will teach you to {verb} in {quantity} minutes.",
                ["code_note"] = "Code example: {code}",
                ["section_title"] = "Section {number}"
            },
            ["fr-FR"] = new()
            {
                ["tutorial_intro"] = "Bienvenue à {product_name}. Ce guide vous apprendra à {verb} en {quantity} minutes.",
                ["code_note"] = "Exemple de code : {code}",
                ["section_title"] = "Section {number}"
            }
        }));

        var translator = CreateTranslator(catalog);

        var options = new TranslationOptionsBuilder()
            .WithQuantity(10)
            .WithArgument("product_name", "MyFramework")
            .WithArgument("verb", "get started")
            .WithArgument("code", "const x = 42;")
            .WithArgument("number", 3)
            .Build();

        var enTutorial = translator.Translate("tutorial_intro", options, new("en-US"));
        Assert.Contains("MyFramework", enTutorial, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("get started", enTutorial, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("10", enTutorial, StringComparison.CurrentCultureIgnoreCase);

        var frTutorial = translator.Translate("tutorial_intro", options, new("fr-FR"));
        Assert.Contains("MyFramework", frTutorial, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void Scenario_DynamicUserPreferences_StyleAndCulture()
    {
        // Simulate user preferences affecting translation output
        var catalog = new TestTranslationCatalog();
        catalog.Register("UI", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["menu"] = "Menu",
                ["menu_short"] = "...",
                ["menu_abbreviation"] = "MN",
                ["menu_symbol"] = "☰",
                ["settings"] = "Settings",
                ["settings_short"] = "Opts",
                ["settings_abbreviation"] = "SET",
                ["settings_symbol"] = "⚙"
            },
            ["fr-FR"] = new()
            {
                ["menu"] = "Menu",
                ["menu_short"] = "...",
                ["menu_abbreviation"] = "MN",
                ["menu_symbol"] = "☰",
                ["settings"] = "Paramètres",
                ["settings_short"] = "Opts",
                ["settings_abbreviation"] = "PAR",
                ["settings_symbol"] = "⚙"
            }
        }));

        var translator = CreateTranslator(catalog);

        // User prefers compact symbols in English
        var symbolOptions = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Symbol).Build();
        var enSymbol = translator.Translate("menu", symbolOptions, new("en-US"));
        Assert.Equal("☰", enSymbol);

        // User prefers abbreviations in French
        var abbrevOptions = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Abbreviation).Build();
        var frAbbrev = translator.Translate("settings", abbrevOptions, new("fr-FR"));
        Assert.Equal("PAR", frAbbrev);
    }

    [Fact]
    public void Scenario_TimeBasedContentGeneration_WithQuantity()
    {
        // Simulate generating messages based on time/quantity
        var catalog = new TestTranslationCatalog();
        catalog.Register("Time", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["ago"] = "{quantity} minute ago",
                ["version"] = "Version {quantity:N0}",
                ["changelog"] = "Released {quantity} update"
            },
            ["fr-FR"] = new()
            {
                ["ago"] = "Il y a {quantity} minute",
                ["version"] = "Version {quantity:N0}",
                ["changelog"] = "{quantity} mise à jour publiée"
            }
        }));

        var translator = CreateTranslator(catalog);

        // Time-based message
        var timeOptions = new TranslationOptionsBuilder().WithQuantity(30).Build();
        var timeMsg = translator.Translate("ago", timeOptions, new("en-US"));
        Assert.Contains("30", timeMsg, StringComparison.CurrentCultureIgnoreCase);

        // Version number formatting
        var versionOptions = new TranslationOptionsBuilder().WithQuantity(1000000).Build();
        var version = translator.Translate("version", versionOptions, new("en-US"));
        Assert.Contains("1,000,000", version, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void Scenario_ContextualMessageGeneration_PersonalizedContent()
    {
        // Simulate personalized messages with user context
        var catalog = new TestTranslationCatalog();
        catalog.Register("Messages", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["welcome_new"] = "Welcome {name}! You're our newest member.",
                ["order_summary"] = "{name}, you ordered {quantity} items worth {amount:C} on {date}.",
                ["achievement"] = "{name} reached level {level_value}!"
            },
            ["fr-FR"] = new()
            {
                ["welcome_new"] = "Bienvenue {name} ! Vous êtes notre nouveau membre.",
                ["order_summary"] = "{name}, vous avez commandé {quantity} articles pour {amount:C} le {date}.",
                ["achievement"] = "{name} a atteint le niveau {level_value} !"
            }
        }));

        var translator = CreateTranslator(catalog);

        // Personalized welcome
        var welcomeOptions = new TranslationOptionsBuilder()
            .WithArgument("name", "John Doe")
            .Build();

        var welcome = translator.Translate("welcome_new", welcomeOptions, new("en-US"));
        Assert.Contains("John Doe", welcome, StringComparison.CurrentCultureIgnoreCase);

        // Order summary with multiple arguments
        var orderOptions = new TranslationOptionsBuilder()
            .WithQuantity(3)
            .WithArgument("name", "Jane Smith")
            .WithArgument("amount", 150m)
            .WithArgument("date", "2023-12-01")
            .Build();

        var order = translator.Translate("order_summary", orderOptions, new("en-US"));
        Assert.Contains("Jane Smith", order, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("3", order, StringComparison.CurrentCultureIgnoreCase);

        // Achievement message
        var achievementOptions = new TranslationOptionsBuilder()
            .WithArgument("name", "Alex")
            .WithArgument("level_value", 42)
            .Build();

        var achievement = translator.Translate("achievement", achievementOptions, new("en-US"));
        Assert.Contains("Alex", achievement, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("42", achievement, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public void Scenario_FallbackChain_MissingTranslations()
    {
        // Test cascading fallback: en-GB -> en -> invariant
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            // Only English generic available
            ["en"] = new() { ["Greeting"] = "Hello" }

            // No en-GB or en-US specific
        }));

        var translator = CreateTranslator(catalog);

        // Request en-GB but only have en
        var result = translator.Translate("Greeting", TranslationOptionsPresets.Default, new("en-GB"));

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Scenario_MissingArgumentFallback_PartialTemplate()
    {
        // What happens when template references missing arguments
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new() { ["en-US"] = new() { ["partial"] = "Hello {name}, welcome to {platform}" } }));

        var translator = CreateTranslator(catalog);

        // Only provide one of two arguments
        var options = new TranslationOptionsBuilder()
            .WithArgument("name", "Alice")

            // Missing: platform
            .Build();

        var result = translator.Translate("partial", options, new("en-US"));

        Assert.Contains("Alice", result, StringComparison.CurrentCultureIgnoreCase);
        Assert.Contains("{platform}", result, StringComparison.CurrentCultureIgnoreCase); // Unknown placeholder preserved
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("en-GB")]
    [InlineData("en-AU")]
    [InlineData("fr-FR")]
    [InlineData("fr-CA")]
    [InlineData("de-DE")]
    [InlineData("de-AT")]
    public void Scenario_RegionalVariants_AllSupported(string cultureName)
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en"] = new() { ["Hello"] = "Hello" },
            ["fr"] = new() { ["Hello"] = "Bonjour" },
            ["de"] = new() { ["Hello"] = "Hallo" }
        }));

        var translator = CreateTranslator(catalog);

        var result = translator.Translate("Hello", TranslationOptionsPresets.Default, new(cultureName));

        Assert.NotEmpty(result);
    }

    private static Translator CreateTranslator(ITranslationCatalog catalog)
    {
        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        return new(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class SimpleKeyResolver : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture)
        {
            var candidates = new List<TranslationKeyCandidate>();

            if (options.Style != DisplayStyle.Default)
            {
                var styleSuffix = options.Style switch
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

            candidates.Add(new(baseKey, TranslationFallbackPolicy.Flexible));

            return candidates;
        }

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
