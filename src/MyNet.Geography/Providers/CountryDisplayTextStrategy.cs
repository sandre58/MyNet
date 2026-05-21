// -----------------------------------------------------------------------
// <copyright file="CountryDisplayTextStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Geography.Localization;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Humanizer.Display.Strategies;
using MyNet.Utilities.Geography;

namespace MyNet.Geography.Providers;

/// <summary>
/// Provides localized display names for Country values, using a resource manager to retrieve the appropriate display name based on the country's name and the specified culture. If a display name cannot be found for a given country, it falls back to using the country's name as the display name.
/// </summary>
/// <param name="translationService">The translation service used to retrieve localized strings.</param>
/// <param name="keyProvider">The key provider used to generate translation keys.</param>
public sealed class CountryDisplayTextStrategy(ITranslationService translationService, ITranslationKeyProvider keyProvider) : DisplayTextStrategyBase<Country>(translationService, keyProvider)
{
    /// <inheritdoc />
    protected override string? ProvideSymbol(Country value, CultureInfo culture) => null;

    /// <inheritdoc />
    protected override string? ProvideDisplayText(Country value, CultureInfo culture) => CountryResources.ResourceManager.GetString(value.Name, culture);

    /// <inheritdoc />
    protected override string ProvideFallback(Country value, CultureInfo culture) => value.Name;
}
