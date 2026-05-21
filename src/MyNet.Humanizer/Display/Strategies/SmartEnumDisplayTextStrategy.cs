// -----------------------------------------------------------------------
// <copyright file="SmartEnumDisplayTextStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Utilities;

namespace MyNet.Humanizer.Display.Strategies;

/// <summary>
/// Provides display names for SmartEnum values, using a translation service to support localization. It first attempts to find a translation using a key composed of the SmartEnum type and name, then falls back to using just the name, and finally returns the name if no translation is found.
/// </summary>
/// <param name="translationService">The translation service used to localize the display names.</param>
/// <param name="keyProvider">The key provider to use for generating translation keys.</param>
public sealed class SmartEnumDisplayTextStrategy(ITranslationService translationService, ITranslationKeyProvider keyProvider) : DisplayTextStrategyBase<ISmartEnum>(translationService, keyProvider)
{
    /// <inheritdoc/>
    protected override string? ProvideSymbol(ISmartEnum value, CultureInfo culture) => null;

    /// <inheritdoc/>
    protected override string? ProvideDisplayText(ISmartEnum value, CultureInfo culture) => null;

    /// <inheritdoc/>
    protected override string ProvideFallback(ISmartEnum value, CultureInfo culture) => value.Name;
}
