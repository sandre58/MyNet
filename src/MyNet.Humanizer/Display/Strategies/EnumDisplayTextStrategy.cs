// -----------------------------------------------------------------------
// <copyright file="EnumDisplayTextStrategy.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Utilities;

namespace MyNet.Humanizer.Display.Strategies;

/// <summary>
/// Provides display names for enum values, using DisplayAttribute, DescriptionAttribute, or translation resources as needed.
/// </summary>
/// <param name="translationService">The translation service to use for translating resource keys.</param>
/// <param name="keyProvider">The key provider to use for generating translation keys.</param>
public sealed class EnumDisplayTextStrategy(ITranslationService translationService, ITranslationKeyProvider keyProvider) : DisplayTextStrategyBase<Enum>(translationService, keyProvider)
{
    /// <inheritdoc/>
    protected override string? ProvideSymbol(Enum value, CultureInfo culture) => value.GetSymbol();

    /// <inheritdoc/>
    protected override string? ProvideDisplayText(Enum value, CultureInfo culture)
    {
        var displayAttribute = value.GetAttribute<DisplayAttribute>();

        return displayAttribute is not null ? ResolveDisplayAttribute(displayAttribute, culture) : null;
    }
}
