// -----------------------------------------------------------------------
// <copyright file="TranslationServiceTests.cs" company="Stéphane ANDRE">
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

public class TranslationServiceTests
{
    [Fact]
    public void Translate_WithDefaultCulture_UsesContextCulture()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var result = service.Translate("Greeting", TranslationOptionsPresets.Default);

        Assert.Equal("Hello", result);
    }

    [Fact]
    public void Translate_WithDifferentContextCulture_UsesContextCulture()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("fr-FR")));

        var result = service.Translate("Greeting", TranslationOptionsPresets.Default);

        Assert.Equal("Bonjour", result);
    }

    [Fact]
    public void Translate_WithOptions_AppliesOptions()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var options = new TranslationOptionsBuilder()
            .WithArgument("name", "Alice")
            .Build();

        var result = service.Translate("Welcome", options);

        Assert.Contains("Alice", result, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void Translate_WithStyle_AppliesStyle()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var options = new TranslationOptionsBuilder()
            .WithStyle(DisplayStyle.Abbreviation)
            .Build();

        var result = service.Translate("Status", options);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithQuantity_AppliesQuantity()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var options = new TranslationOptionsBuilder()
            .WithQuantity(5)
            .Build();

        var result = service.Translate("Items", options);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_ComplexScenario_AllFeaturesWork()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var options = new TranslationOptionsBuilder()
            .WithStyle(DisplayStyle.Default)
            .WithQuantity(10)
            .WithArgument("location", "warehouse")
            .WithArgument("user", "Bob")
            .Build();

        var result = service.Translate("ComplexMessage", options);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_WithoutKeyFallback_ReturnsEmptyIfNotFound()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var options = new TranslationOptionsBuilder()
            .WithoutKeyFallback()
            .Build();

        var result = service.Translate("NonExistent", options);

        Assert.Empty(result);
    }

    [Fact]
    public void Translate_MultipleCallsSameCulture_ConsistentResults()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var result1 = service.Translate("Greeting", TranslationOptionsPresets.Default);
        var result2 = service.Translate("Greeting", TranslationOptionsPresets.Default);
        var result3 = service.Translate("Greeting", TranslationOptionsPresets.Default);

        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    [Fact]
    public void Translate_DifferentServices_DifferentCultures()
    {
        var translator = CreateTranslator();

        var enService = new TranslationService(translator, new FixedCultureContext(new("en-US")));
        var frService = new TranslationService(translator, new FixedCultureContext(new("fr-FR")));

        var enResult = enService.Translate("Greeting", TranslationOptionsPresets.Default);
        var frResult = frService.Translate("Greeting", TranslationOptionsPresets.Default);

        Assert.NotEqual(enResult, frResult);
        Assert.Equal("Hello", enResult);
        Assert.Equal("Bonjour", frResult);
    }

    [Fact]
    public void Translate_WithChangingContext_ReflectsNewCulture()
    {
        var translator = CreateTranslator();
        var context = new DynamicCultureContext(new("en-US"));
        var service = new TranslationService(translator, context);

        var result1 = service.Translate("Greeting", TranslationOptionsPresets.Default);
        Assert.Equal("Hello", result1);

        // Change culture
        context.CurrentCulture = new("fr-FR");
        var result2 = service.Translate("Greeting", TranslationOptionsPresets.Default);
        Assert.Equal("Bonjour", result2);
    }

    [Fact]
    public void Translate_InflectionFallback_Respected()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var optionsWithInflection = new TranslationOptionsBuilder()
            .WithQuantity(2)
            .UseInflectionFallback()
            .Build();

        var resultWithInflection = service.Translate("Person", optionsWithInflection);
        Assert.NotEmpty(resultWithInflection);
    }

    [Fact]
    public void Translate_WithAllDisplayStyles_AllSupported()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var styles = new[] { DisplayStyle.Default, DisplayStyle.Short, DisplayStyle.Abbreviation, DisplayStyle.Symbol, DisplayStyle.Narrow };

        foreach (var style in styles)
        {
            var options = new TranslationOptionsBuilder().WithStyle(style).Build();
            var result = service.Translate("Currency", options);
            Assert.NotEmpty(result);
        }
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("de-DE")]
    public void Translate_VariousCultures_AllSupported(string cultureName)
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new(cultureName)));

        var result = service.Translate("Greeting", TranslationOptionsPresets.Default);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_TemplateWithMultipleArguments_AllRenderedCorrectly()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var options = new TranslationOptionsBuilder()
            .WithQuantity(42)
            .WithArgument("name", "John")
            .WithArgument("location", "NYC")
            .Build();

        var result = service.Translate("DetailedMsg", options);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void Translate_VeryLongKey_Handled()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var longKey = new string('K', 500);
        var result = service.Translate(longKey, TranslationOptionsPresets.Default);

        Assert.Equal(longKey, result); // Fallback to key
    }

    [Fact]
    public void Translate_BuiltOptionsReused_Works()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        // Build options once
        var options = new TranslationOptionsBuilder()
            .WithStyle(DisplayStyle.Short)
            .WithQuantity(10)
            .Build();

        // Reuse options multiple times
        var result1 = service.Translate("Items", options);
        var result2 = service.Translate("Items", options);
        var result3 = service.Translate("Items", options);

        Assert.NotEmpty(result1);
        Assert.NotEmpty(result2);
        Assert.NotEmpty(result3);
    }

    [Fact]
    public void Translate_WithPresetOptions_Works()
    {
        var translator = CreateTranslator();
        var service = new TranslationService(translator, new FixedCultureContext(new("en-US")));

        var result = service.Translate("Greeting", TranslationOptionsPresets.Default);

        Assert.NotEmpty(result);
    }

    private static Translator CreateTranslator()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["Greeting"] = "Hello",
                ["Welcome"] = "Welcome {name}!",
                ["Items"] = "item",
                ["Status"] = "Status",
                ["Status_abbreviation"] = "Sts",
                ["Currency"] = "Dollar",
                ["Currency_short"] = "USD",
                ["Currency_abbreviation"] = "USD",
                ["Currency_symbol"] = "$",
                ["Currency_narrow"] = "$",
                ["Person"] = "person",
                ["ComplexMessage"] = "Message",
                ["DetailedMsg"] = "Detailed message"
            },
            ["fr-FR"] = new()
            {
                ["Greeting"] = "Bonjour",
                ["Welcome"] = "Bienvenue {name}!",
                ["Items"] = "article",
                ["Status"] = "Statut"
            },
            ["de-DE"] = new() { ["Greeting"] = "Hallo" }
        }));

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

    private sealed class DynamicCultureContext(CultureInfo initialCulture) : ICultureContext
    {
        public CultureInfo CurrentCulture { get; set; } = initialCulture;
    }

    private sealed class SimpleKeyResolver : ITranslationKeyResolver
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
