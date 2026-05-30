// -----------------------------------------------------------------------
// <copyright file="ObservableLocalizationFixture.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using MyNet.Globalization;
using MyNet.Humanizer;
using Xunit;

namespace MyNet.Observable.Tests.Fixtures;

/// <summary>
/// Initializes the DI-based localization/humanizer stack for observable tests.
/// </summary>
[SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit collection fixture discovery relies on public visibility.")]
public sealed class ObservableLocalizationFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public ObservableLocalizationFixture()
    {
        var services = new ServiceCollection();
        services.AddLocalization();
        services.AddHumanizer();

        _serviceProvider = services.BuildServiceProvider();
        _serviceProvider.UseLocalization();
        _serviceProvider.UseDisplayText();
    }

    public void Dispose() => _serviceProvider.Dispose();
}

[CollectionDefinition("UseCultureSequential", DisableParallelization = true)]
[SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit collection definition is discovered by reflection.")]
public sealed class UseCultureSequentialFixtureSet : ICollectionFixture<ObservableLocalizationFixture>;
