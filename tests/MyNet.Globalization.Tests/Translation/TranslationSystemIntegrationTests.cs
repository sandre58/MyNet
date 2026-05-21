// -----------------------------------------------------------------------
// <copyright file="TranslationSystemIntegrationTests.cs" company="Stéphane ANDRE">
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

public class TranslationSystemIntegrationTests
{
    [Fact]
    public void CompleteWorkflow_TranslateWithAllComponents()
    {
        // Setup all components
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["Welcome"] = "Welcome {name}!",
                ["Items"] = "item",
                ["ItemCount"] = "You have {quantity} items"
            },
            ["fr-FR"] = new()
            {
                ["Welcome"] = "Bienvenue {name}!",
                ["Items"] = "article",
                ["ItemCount"] = "Vous avez {quantity} articles"
            }
        }));
        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        // Test 1: Simple translation with argument
        var result1 = translator.Translate(
            "Welcome",
            new TranslationOptionsBuilder().WithArgument("name", "Alice").Build(),
            new("en-US"));

        Assert.Equal("Welcome Alice!", result1);

        // Test 2: Translation with quantity
        var result2 = translator.Translate(
            "Items",
            new TranslationOptionsBuilder().WithQuantity(3).Build(),
            new("en-US"));

        Assert.NotEmpty(result2);

        // Test 3: Complex template with quantity
        var result3 = translator.Translate(
            "ItemCount",
            new TranslationOptionsBuilder().WithQuantity(42).Build(),
            new("en-US"));

        Assert.Contains("42", result3, StringComparison.InvariantCultureIgnoreCase);

        // Test 4: French translation
        var result4 = translator.Translate(
            "Welcome",
            new TranslationOptionsBuilder().WithArgument("name", "Marie").Build(),
            new("fr-FR"));

        Assert.Equal("Bienvenue Marie!", result4);
    }

    [Fact]
    public void IntegrationWithDisplayStyles_AllStylesWork()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["Currency"] = "Dollar",
                ["Currency_short"] = "USD",
                ["Currency_abbreviation"] = "USD",
                ["Currency_symbol"] = "$",
                ["Currency_narrow"] = "$"
            }
        }));

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new StyleAwareKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        // Test each display style
        var styles = new[] { DisplayStyle.Default, DisplayStyle.Short, DisplayStyle.Abbreviation, DisplayStyle.Symbol, DisplayStyle.Narrow };

        foreach (var style in styles)
        {
            var result = translator.Translate(
                "Currency",
                new TranslationOptionsBuilder().WithStyle(style).Build(),
                new("en-US"));

            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public void IntegrationCatalogModification_DynamicResourceRegistration()
    {
        var catalog = new TestTranslationCatalog();

        // Initially empty
        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        // Translate when no resources exist
        var result1 = translator.Translate("Missing", TranslationOptionsPresets.Default, new("en-US"));
        Assert.Equal("Missing", result1);

        // Register new resources
        catalog.Register("Dynamic", new InMemoryResourceManager(new()
        {
            ["en-US"] = new() { ["NewKey"] = "New Value" }
        }));

        // Translate after registration
        var result2 = translator.Translate("NewKey", TranslationOptionsPresets.Default, new("en-US"));
        Assert.Equal("New Value", result2);
    }

    [Fact]
    public void IntegrationProviderResolver_MultiCultureProvider()
    {
        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        // Test culture-specific provider resolution
        providerResolver.TryGet<IInflector>(new("en-US"), out var enProvider);
        providerResolver.TryGet<IInflector>(new("fr-FR"), out var frProvider);
        providerResolver.TryGet<IInflector>(new("de-DE"), out var deProvider);

        Assert.NotNull(enProvider);
        Assert.NotNull(frProvider);
        Assert.NotNull(deProvider);
    }

    [Fact]
    public void IntegrationPluralization_EnglishVsFrench()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["person"] = "person" // English pluralizes to "people"
            },
            ["fr-FR"] = new()
            {
                ["personne"] = "personne" // French just adds "s": "personnes"
            }
        }));

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        // English pluralization: person -> people
        var enResult = translator.Translate(
            "person",
            new TranslationOptionsBuilder().WithQuantity(5).Build(),
            new("en-US"));

        Assert.Equal("people", enResult);

        // French pluralization: personne -> personnes (via inflection)
        var frResult = translator.Translate(
            "personne",
            new TranslationOptionsBuilder().WithQuantity(5).Build(),
            new("fr-FR"));

        Assert.Equal("personnes", frResult);
    }

    [Fact]
    public void IntegrationCultureFallback_ChainedResolution()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            // Only have English (en) translation, not en-US
            ["en"] = new() { ["Hello"] = "Hello from en" }
        }));

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        // Request en-US but only have en -> should fallback
        var result = translator.Translate("Hello", TranslationOptionsPresets.Default, new("en-US"));

        Assert.Equal("Hello from en", result);
    }

    [Fact]
    public void IntegrationComplexTemplate_MultipleArguments()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["ComplexMsg"] = "User {username} has {quantity:N0} items of type {itemType} in {location}"
            }
        }));

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        var options = new TranslationOptionsBuilder()
            .WithQuantity(1000)
            .WithArgument("username", "alice")
            .WithArgument("itemType", "books")
            .WithArgument("location", "library")
            .Build();

        var result = translator.Translate("ComplexMsg", options, new("en-US"));

        Assert.Contains("alice", result, StringComparison.InvariantCultureIgnoreCase);
        Assert.Contains("1,000", result, StringComparison.InvariantCultureIgnoreCase);
        Assert.Contains("books", result, StringComparison.InvariantCultureIgnoreCase);
        Assert.Contains("library", result, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void IntegrationMultipleResources_ResolvesCorrectly()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("LoginModule", new InMemoryResourceManager(new()
        {
            ["en-US"] = new() { ["Login"] = "Login" }
        }));

        catalog.Register("UserModule", new InMemoryResourceManager(new()
        {
            ["en-US"] = new() { ["Profile"] = "User Profile" }
        }));

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var translator = new Translator(catalog, new SimpleKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);

        // Specific resource lookup
        var result1 = translator.Translate("Login", TranslationOptionsPresets.Default, new("en-US"), "LoginModule");
        Assert.Equal("Login", result1);

        var result2 = translator.Translate("Profile", TranslationOptionsPresets.Default, new("en-US"), "UserModule");
        Assert.Equal("User Profile", result2);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class SimpleKeyResolver : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture)
            => [new(baseKey, TranslationFallbackPolicy.Flexible)];

        public string ResolvePluralizedKey(string baseKey, decimal count, CultureInfo culture) => throw new NotSupportedException();

        public string ResolveStyledKey(string baseKey, DisplayStyle style, CultureInfo culture) => throw new NotSupportedException();
    }

    private sealed class StyleAwareKeyResolver : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture)
        {
            var candidates = new List<TranslationKeyCandidate>();

            // Add style-specific key
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

            // Add base key
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
