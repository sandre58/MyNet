// -----------------------------------------------------------------------
// <copyright file="PatternFakerBaseTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using FluentAssertions;
using Moq;
using MyNet.Generator;
using MyNet.Globalization.Localization.Providers;
using MyNet.Text.Randomize;
using Xunit;

namespace MyNet.Fakers.Tests;

public sealed class PatternFakerBaseTests
{
    [Fact]
    public void Randomize_SelectsPatternFromCultureScopedProvider()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var patterns = new[] { "AAA-###", "BBB-###" };
        var provider = new TestPatternProvider(culture, patterns);
        var random = new Mock<IRandomGenerator>();
        var source = new StubCultureScopedSource(provider);
        random.Setup(r => r.Item(patterns)).Returns("FIXED");
        var patternGenerator = new TextRandomGenerator(random.Object);

        var sut = new TestPatternFaker(patternGenerator, random.Object, source);

        var result = sut.Pick(culture);

        result.Should().Be("FIXED");
        source.LastRequestedCulture.Should().Be(culture);
    }
}

internal sealed class TestPatternFaker(
    ITextRandomGenerator patternGenerator,
    IRandomGenerator random,
    ICultureScopedServiceSource<ITestPatternProvider> source)
    : PatternFakerBase<ITestPatternProvider>(patternGenerator, random, source)
{
    public string Pick(CultureInfo culture) => Randomize(p => p.Patterns, culture);
}

internal sealed class StubCultureScopedSource(ITestPatternProvider provider) : ICultureScopedServiceSource<ITestPatternProvider>
{
    public CultureInfo? LastRequestedCulture { get; private set; }

    public ITestPatternProvider Get(CultureInfo? culture = null)
    {
        LastRequestedCulture = culture;
        return provider;
    }

    public bool TryGet(CultureInfo? culture, [NotNullWhen(true)] out ITestPatternProvider? service)
    {
        service = Get(culture);
        return true;
    }
}

internal interface ITestPatternProvider : ICultureScoped
{
    IReadOnlyList<string> Patterns { get; }
}

internal sealed class TestPatternProvider(CultureInfo culture, IReadOnlyList<string> patterns) : ITestPatternProvider
{
    public CultureInfo Culture { get; } = culture;

    public IReadOnlyList<string> Patterns { get; } = patterns;
}
