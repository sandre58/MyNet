// -----------------------------------------------------------------------
// <copyright file="UtilitiesLocalizationFixture.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization.Tests.Data;
using Xunit;

namespace MyNet.Globalization.Tests;

[SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit fixture discovery relies on public visibility.")]
public sealed class UtilitiesLocalizationFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public UtilitiesLocalizationFixture()
    {
        var services = new ServiceCollection();
        services.AddGlobalization();
        services.AddLocalization();
        services.AddTranslationResource(nameof(DataResources), DataResources.ResourceManager);

        _serviceProvider = services.BuildServiceProvider();
        _serviceProvider.UseGlobalization();
        _serviceProvider.UseLocalization();
    }

    public void Dispose() => _serviceProvider.Dispose();
}

[CollectionDefinition("UseCultureSequential", DisableParallelization = true)]
[SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit collection definition is discovered by reflection.")]
public sealed class UseCultureSequentialFixtureSet : ICollectionFixture<UtilitiesLocalizationFixture>;
