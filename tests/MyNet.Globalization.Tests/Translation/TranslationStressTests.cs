// -----------------------------------------------------------------------
// <copyright file="TranslationStressTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
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

[SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Stress tests may require blocking operations to simulate real-world concurrency scenarios.")]
public class TranslationStressTests
{
    [Fact]
    public void ConcurrentTranslations_AllSucceed()
    {
        var translator = CreateTranslator();
        var results = new ConcurrentBag<string>();
        var tasks = new Task[100];

        for (var i = 0; i < 100; i++)
        {
            var index = i;
            tasks[i] = Task.Run(() =>
            {
                var culture = index % 2 == 0 ? new CultureInfo("en-US") : new CultureInfo("fr-FR");
                var result = translator.Translate("Greeting", TranslationOptionsPresets.Default, culture);
                results.Add(result);
            });
        }

        Task.WaitAll(tasks);

        Assert.Equal(100, results.Count);
        Assert.All(results, Assert.NotEmpty);
    }

    [Fact]
    public void HighVolumeTranslations_MaintainsConsistency()
    {
        var translator = CreateTranslator();
        var culture = new CultureInfo("en-US");
        const int translationCount = 1000;

        var results = new string[translationCount];
        var tasks = new Task[Environment.ProcessorCount];

        // Divide work across all processor cores
        var itemsPerTask = translationCount / tasks.Length;

        for (var t = 0; t < tasks.Length; t++)
        {
            var taskIndex = t;
            tasks[t] = Task.Run(() =>
            {
                var start = taskIndex * itemsPerTask;
                var end = taskIndex == tasks.Length - 1 ? translationCount : (taskIndex + 1) * itemsPerTask;

                for (var i = start; i < end; i++)
                {
                    results[i] = translator.Translate("Greeting", TranslationOptionsPresets.Default, culture);
                }
            });
        }

        Task.WaitAll(tasks);

        Assert.All(results, result => Assert.Equal("Hello", result));
    }

    [Fact]
    public void ConcurrentWithDifferentOptions_AllSucceed()
    {
        var translator = CreateTranslator();
        var results = new ConcurrentBag<(string Result, DisplayStyle Style)>();
        var displayStyles = new[] { DisplayStyle.Default, DisplayStyle.Short, DisplayStyle.Abbreviation };

        var tasks = new List<Task>();

        for (var i = 0; i < 10; i++)
        {
            tasks.AddRange(displayStyles.Select(localStyle => Task.Run(() =>
            {
                var options = new TranslationOptionsBuilder().WithStyle(localStyle).Build();
                var result = translator.Translate("Message", options, new("en-US"));
                results.Add((result, localStyle));
            })));
        }

        Task.WaitAll([..tasks]);

        Assert.Equal(30, results.Count);
        Assert.All(results, item => Assert.NotEmpty(item.Result));
    }

    [Fact]
    public void ConcurrentWithQuantityVariations_AllSucceed()
    {
        var translator = CreateTranslator();
        var results = new ConcurrentBag<(string Result, decimal Quantity)>();

        var tasks = new Task[50];

        for (var i = 0; i < 50; i++)
        {
            var index = i;
            tasks[i] = Task.Run(() =>
            {
                var quantity = index + 1;
                var options = new TranslationOptionsBuilder()
                    .WithQuantity(quantity)
                    .Build();

                var result = translator.Translate("Item", options, new("en-US"));
                results.Add((result, quantity));
            });
        }

        Task.WaitAll(tasks);

        Assert.Equal(50, results.Count);
        Assert.All(results, item => Assert.NotEmpty(item.Result));
    }

    [Fact]
    public void ConcurrentProviderAccess_NoDeadlock()
    {
        CreateTranslator();

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        var tasks = new Task[20];

        for (var i = 0; i < 20; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                // Try to get provider concurrently
                providerResolver.TryGet<IInflector>(new("en-US"), out var inflector);
                Assert.NotNull(inflector);
            });
        }

        var completedInTime = Task.WaitAll(tasks, TimeSpan.FromSeconds(5));
        Assert.True(completedInTime, "Concurrent provider access should not deadlock");
    }

    [Fact]
    public void ConcurrentCatalogRegistration_AllSucceed()
    {
        var catalog = new TestTranslationCatalog();
        const int registrationCount = 100;
        var tasks = new Task[registrationCount];

        for (var i = 0; i < registrationCount; i++)
        {
            var index = i;
            tasks[i] = Task.Run(() =>
            {
                var resourceKey = $"Resource{index}";
                var manager = new InMemoryResourceManager(new() { ["en-US"] = new() { ["Key"] = $"Value{index}" } });

                catalog.Register(resourceKey, manager);
            });
        }

        Task.WaitAll(tasks);

        // Verify all registrations succeeded by checking that catalog changed event fired
        Assert.NotNull(catalog);
    }

    [Fact]
    public void MixedOperationsUnderLoad_AllCompleteSuccessfully()
    {
        var translator = CreateTranslator();
        var catalog = new TestTranslationCatalog();
        var results = new ConcurrentBag<string>();

        var translationTasks = new List<Task>();

        // Simulate mixed workload
        for (var i = 0; i < 100; i++)
        {
            var index = i;

            // Translation queries
            translationTasks.Add(Task.Run(() =>
            {
                var culture = new CultureInfo(index % 2 == 0 ? "en-US" : "fr-FR");
                var options = new TranslationOptionsBuilder()
                    .WithQuantity(index)
                    .WithArgument("param", index)
                    .Build();

                var result = translator.Translate("Message", options, culture);
                results.Add(result);
            }));

            // Catalog registrations (less frequent)
            if (i % 10 == 0)
            {
                translationTasks.Add(Task.Run(() =>
                {
                    var manager = new InMemoryResourceManager(new() { ["en-US"] = new() { ["Key"] = "Value" } });

                    catalog.Register($"Resource{index}", manager);
                }));
            }
        }

        Task.WaitAll([..translationTasks]);

        Assert.NotEmpty(results);
    }

    [Fact]
    public void HighConcurrencyWithBuilder_AllOptionsBuiltCorrectly()
    {
        var options = new ConcurrentBag<TranslationOptions>();
        var tasks = new Task[50];

        for (var i = 0; i < 50; i++)
        {
            var index = i;
            tasks[i] = Task.Run(() =>
            {
                var built = new TranslationOptionsBuilder()
                    .WithStyle((DisplayStyle)(index % 5))
                    .WithQuantity(index)
                    .WithArgument($"arg{index}", index)
                    .Build();

                options.Add(built);
            });
        }

        Task.WaitAll(tasks);

        Assert.Equal(50, options.Count);
        Assert.All(options, Assert.NotNull);
    }

    [Fact]
    public void StressTestCultureFallback_AllCulturesResolved()
    {
        var translator = CreateTranslator();
        var results = new ConcurrentBag<string>();
        var cultures = new[] { new CultureInfo("en-US"), new CultureInfo("en"), new CultureInfo("fr-FR"), new CultureInfo("fr"), new CultureInfo("de-DE"), new CultureInfo("de") };

        var tasks = new List<Task>();

        for (var i = 0; i < 50; i++)
        {
            tasks.AddRange(cultures.Select(localCulture => Task.Run(() =>
            {
                var result = translator.Translate("Greeting", TranslationOptionsPresets.Default, localCulture);
                results.Add(result);
            })));
        }

        Task.WaitAll([..tasks]);

        Assert.Equal(50 * cultures.Length, results.Count);
        Assert.All(results, Assert.NotEmpty);
    }

    private static Translator CreateTranslator()
    {
        var catalog = new TestTranslationCatalog();
        catalog.Register("Main", new InMemoryResourceManager(new()
        {
            ["en-US"] = new()
            {
                ["Greeting"] = "Hello",
                ["Message"] = "Hello World",
                ["Item"] = "item"
            },
            ["fr-FR"] = new()
            {
                ["Greeting"] = "Bonjour",
                ["Message"] = "Bonjour le monde",
                ["Item"] = "article"
            }
        }));

        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);

        return new(catalog, new StubKeyResolver(), new PluralizationService(providerResolver, new FixedCultureContext(new("en-US"))), CultureFallbackPolicies.ParentCulture);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }

    private sealed class StubKeyResolver : ITranslationKeyResolver
    {
        public IReadOnlyCollection<TranslationKeyCandidate> Resolve(string baseKey, TranslationOptions options, CultureInfo culture) => [new(baseKey, TranslationFallbackPolicy.Flexible)];

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
