// -----------------------------------------------------------------------
// <copyright file="TranslationKeyResolverTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Linq;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Inflection;
using MyNet.Globalization.Inflection.Cultures;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Providers.Factories;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyResolving;
using MyNet.Globalization.Tests.Providers;
using Xunit;

namespace MyNet.Globalization.Tests.Translation;

public class TranslationKeyResolverTests
{
    [Fact]
    public void ResolvePluralizedKey_WithPluralQuantity_ReturnsPluralSuffix()
    {
        var resolver = CreateResolver();

        var key = resolver.ResolvePluralizedKey("DurationDay", 2, new("en-US"));

        Assert.Equal("DurationDayPlural", key);
    }

    [Fact]
    public void ResolvePluralizedKey_WithZeroQuantity_ReturnsZeroSuffix()
    {
        var resolver = CreateResolver();

        var key = resolver.ResolvePluralizedKey("DurationDay", 0, new("en-US"));

        Assert.Equal("DurationDayZero", key);
    }

    [Fact]
    public void ResolveStyledKey_WithAbbreviation_ReturnsAbbrSuffix()
    {
        var resolver = CreateResolver();

        var key = resolver.ResolveStyledKey("TimeUnitMinute", DisplayStyle.Abbreviation, new("en-US"));

        Assert.Equal("TimeUnitMinuteAbbr", key);
    }

    [Fact]
    public void Resolve_WithStyleAndQuantity_ContainsStrictAndFlexibleCandidates()
    {
        var resolver = CreateResolver();
        var options = new TranslationOptionsBuilder()
            .WithStyle(DisplayStyle.Abbreviation)
            .WithQuantity(2)
            .Build();

        var candidates = resolver.Resolve("TimeUnitMinute", options, new("en-US"));

        Assert.Contains(candidates, c => c is { Key: "TimeUnitMinuteAbbrPlural", Policy.AllowInflectionFallback: false });
        Assert.Contains(candidates, c => c is { Key: "TimeUnitMinuteAbbr", Policy.AllowInflectionFallback: false });
        Assert.Contains(candidates, c => c is { Key: "TimeUnitMinute", Policy.AllowInflectionFallback: true });
    }

    [Fact]
    public void Resolve_DoesNotDuplicateCandidates()
    {
        var resolver = CreateResolver();
        var options = new TranslationOptionsBuilder().WithStyle(DisplayStyle.Short).Build();

        var candidates = resolver.Resolve("Duration", options, CultureInfo.InvariantCulture).ToList();

        Assert.Equal(candidates.Count, candidates.Select(x => x.Key).Distinct().Count());
    }

    private static TranslationKeyResolver CreateResolver()
    {
        var factory = new LocalizationServiceFactoryBuilder<IInflector>(_ => Inflectors.Invariant)
            .RegisterCulture("en", () => Inflectors.English)
            .RegisterCulture("fr", () => Inflectors.French)
            .Build();
        var registry = TestLocalizationProviderRegistryFactory.Create(factory);

        var providerResolver = new LocalizationServiceResolver(registry);
        var pluralizationService = new PluralizationService(providerResolver, new FixedCultureContext(new("en-US")));
        return new(pluralizationService);
    }

    private sealed class FixedCultureContext(CultureInfo culture) : ICultureContext
    {
        public CultureInfo CurrentCulture => culture;
    }
}
