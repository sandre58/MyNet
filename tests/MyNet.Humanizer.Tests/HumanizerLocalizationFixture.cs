// -----------------------------------------------------------------------
// <copyright file="HumanizerLocalizationFixture.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization;
using Xunit;

namespace MyNet.Humanizer.Tests;

/// <summary>
/// Initializes the DI-based localization/humanizer stack for tests using the UseCultureSequential collection.
/// </summary>
[SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit collection fixture discovery relies on public visibility.")]
public sealed class HumanizerLocalizationFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public HumanizerLocalizationFixture()
    {
        var services = new ServiceCollection();
        services.AddLocalization();
        services.AddHumanizer();

        _serviceProvider = services.BuildServiceProvider();
        _serviceProvider.UseLocalization();
        _serviceProvider.UseDisplayText();
    }

    public IServiceProvider Services => _serviceProvider;

    public void Dispose() => _serviceProvider.Dispose();
}

[CollectionDefinition("UseCultureSequential", DisableParallelization = true)]
[SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit collection definition is discovered by reflection.")]
public sealed class UseCultureSequentialFixtureSet : ICollectionFixture<HumanizerLocalizationFixture>;
