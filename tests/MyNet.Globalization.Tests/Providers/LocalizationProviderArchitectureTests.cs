// -----------------------------------------------------------------------
// <copyright file="LocalizationProviderArchitectureTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using MyNet.Globalization.Localization.Providers;
using MyNet.Globalization.Localization.Providers.Factories;
using MyNet.Globalization.Localization.Translation.Catalog;
using MyNet.Globalization.Tests.Translation;
using Xunit;

namespace MyNet.Globalization.Tests.Providers;

public sealed class LocalizationProviderArchitectureTests
{
    [Fact]
    public void RegisterCulture_WithThrowIfExists_Throws()
    {
        var builder = new LocalizationServiceFactoryBuilder<ITestProvider>(_ => new TestProvider("default", CultureInfo.InvariantCulture));

        builder.RegisterCulture("fr", () => new TestProvider("fr-v1", new("fr")));

        Assert.Throws<InvalidOperationException>(() =>
            builder.RegisterCulture("fr", () => new TestProvider("fr-v2", new("fr")), CultureRegistrationBehavior.ThrowIfExists));
    }

    [Fact]
    public void RegisterCulture_WithSkipIfExists_KeepsFirstRegistration()
    {
        var factory = new LocalizationServiceFactoryBuilder<ITestProvider>(_ => new TestProvider("default", CultureInfo.InvariantCulture))
            .RegisterCulture("fr", () => new TestProvider("fr-v1", new("fr")))
            .RegisterCulture("fr", () => new TestProvider("fr-v2", new("fr")), CultureRegistrationBehavior.SkipIfExists)
            .Build();

        var provider = factory.Create(new("fr-FR"));

        Assert.Equal("fr-v1", provider.Name);
    }

    [Fact]
    public void Resolver_WhenProvidersAreCircular_ThrowsExplicitCycleException()
    {
        var factoryA = new CircularFactory<IAProvider>();
        var factoryB = new CircularFactory<IBProvider>();
        var registry = TestLocalizationProviderRegistryFactory.Create(factoryA, factoryB);
        var resolver = new LocalizationServiceResolver(registry);

        factoryA.CreateAction = culture => new AProvider(culture, resolver.GetRequired<IBProvider>(culture));
        factoryB.CreateAction = culture => new BProvider(culture, resolver.GetRequired<IAProvider>(culture));

        var exception = Assert.Throws<InvalidOperationException>(() => resolver.GetRequired<IAProvider>(new("fr-FR")));

        Assert.Contains("Circular localization provider dependency detected", exception.Message, StringComparison.Ordinal);
        Assert.Contains("IAProvider(fr-FR)", exception.Message, StringComparison.Ordinal);
        Assert.Contains("IBProvider(fr-FR)", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void TranslationCatalogContribution_RegistersResourceIntoCatalog()
    {
        var registry = new TestTranslationCatalog();
        var contribution = new TranslationCatalogContribution(x => x.Register("TestModule", new InMemoryResourceManager()));

        contribution.Apply(registry);

        Assert.True(registry.Resources.ContainsKey("TestModule"));
    }

    private interface ITestProvider : ICultureScoped
    {
        string Name { get; }
    }

    private sealed class TestProvider(string name, CultureInfo culture) : ITestProvider
    {
        public string Name { get; } = name;

        public CultureInfo Culture { get; } = culture;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named to reflect their role in testing circular dependencies.")]
    private interface IAProvider : ICultureScoped;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Named to reflect their role in testing circular dependencies.")]
    private interface IBProvider : ICultureScoped;

    private sealed class AProvider : IAProvider
    {
        public AProvider(CultureInfo culture, IBProvider dependency)
        {
            Culture = culture;
            _ = dependency;
        }

        public CultureInfo Culture { get; }
    }

    private sealed class BProvider : IBProvider
    {
        public BProvider(CultureInfo culture, IAProvider dependency)
        {
            Culture = culture;
            _ = dependency;
        }

        public CultureInfo Culture { get; }
    }

    private sealed class CircularFactory<TProvider> : ILocalizationServiceFactory<TProvider>
        where TProvider : class, ICultureScoped
    {
        public Type TargetType => typeof(TProvider);

        public Func<CultureInfo, TProvider>? CreateAction { get; set; }

        public TProvider Create(CultureInfo culture)
            => CreateAction is null
                ? throw new InvalidOperationException("CreateAction must be configured before use.")
                : CreateAction(culture);
    }

    private sealed class InMemoryResourceManager : ResourceManager;
}
