// -----------------------------------------------------------------------
// <copyright file="DisplayTextStrategyBase.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Resources;
using MyNet.Globalization.Localization.Translation;
using MyNet.Globalization.Localization.Translation.KeyGeneration;
using MyNet.Primitives;

namespace MyNet.Humanizer.Display.Strategies;

/// <summary>
/// Base class for display text providers, implementing the common logic for retrieving display texts from attributes, translations, and fallbacks, while allowing derived classes to specify the type of values they support and any additional logic as needed.
/// </summary>
/// <param name="translationService">The translator to use for translating resource keys.</param>
/// <param name="keyProvider">The key provider to use for generating translation keys.</param>
/// <typeparam name="T">The type of values for which to provide display texts.</typeparam>
public abstract class DisplayTextStrategyBase<T>(ITranslationService translationService, ITranslationKeyProvider keyProvider) : IDisplayTextStrategy<T>
    where T : notnull
{
    /// <inheritdoc/>
    public virtual string GetDisplayText(T value, DisplayTextOptions options, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        // 1. Symbol
        if (options.Style == DisplayStyle.Symbol)
        {
            var symbol = ProvideSymbol(value, culture);

            if (!string.IsNullOrWhiteSpace(symbol))
            {
                return symbol;
            }
        }

        // 2. Display name
        var displayText = ProvideDisplayText(value, culture);

        if (!string.IsNullOrWhiteSpace(displayText))
        {
            return displayText;
        }

        // 3. Translation
        var resourceKey = BuildResourceKey(value);
        var translation = translationService.Translate(resourceKey, BuildTranslationOptions(options), culture);

        return !string.IsNullOrWhiteSpace(translation)
            ? translation
            :

            // 4. Fallback
            ProvideFallback(value, culture).OrEmpty();
    }

    /// <summary>
    /// Explicit implementation of the non-generic IDisplayTextStrategy interface method, which casts the input value to the specific type T and delegates to the generic GetDisplayText method. This allows the strategy to be used in contexts where the value type is not known at compile time, while still providing type-specific logic in the generic method.
    /// </summary>
    /// <param name="value">The value for which to get the display text.</param>
    /// <param name="options">The options to use when generating the display text.</param>
    /// <param name="culture">The culture to use for determining the display text.</param>
    /// <returns>The display text for the given value.</returns>
    string IDisplayTextStrategy.GetDisplayText(object value, DisplayTextOptions options, CultureInfo culture) => GetDisplayText((T)value, options, culture);

    /// <summary>
    /// Provides the symbol for the specified value and culture, if available. This method should be implemented by derived classes to return the appropriate symbol based on the value and culture, or null/empty if no symbol is defined.
    /// </summary>
    /// <param name="value">The value for which to provide the symbol.</param>
    /// <param name="culture">The culture to use for symbol retrieval.</param>
    /// <returns>The symbol for the specified value and culture, or null/empty if not defined.</returns>
    protected abstract string? ProvideSymbol(T value, CultureInfo culture);

    /// <summary>
    /// Provides the display name for the specified value and culture, if available. This method should be implemented by derived classes to return the appropriate display name based on the value and culture, or null/empty if no display name is defined.
    /// </summary>
    /// <param name="value">The value for which to provide the display name.</param>
    /// <param name="culture">The culture to use for display name retrieval.</param>
    /// <returns>The display name for the specified value and culture, or null/empty if not defined.</returns>
    protected abstract string? ProvideDisplayText(T value, CultureInfo culture);

    /// <summary>
    /// Provides a fallback display name for the specified value and culture, if available. This method can be implemented by derived classes to return an additional fallback display name based on the value and culture, or null/empty if no fallback is defined. This can be used to provide an additional layer of fallback before resorting to humanization of the value's string representation.
    /// </summary>
    /// <param name="value">The value for which to provide the fallback display name.</param>
    /// <param name="culture">The culture to use for fallback display name retrieval.</param>
    /// <returns>The fallback display name for the specified value and culture, or null/empty if not defined.</returns>
    protected virtual string? ProvideFallback(T value, CultureInfo culture) => value.ToString();

    /// <summary>
    /// Resolves the display name from the specified DisplayAttribute, using the ResourceType if specified, or falling back to the Name or Description properties as needed.
    /// </summary>
    /// <param name="displayAttribute">The DisplayAttribute to resolve.</param>
    /// <param name="culture">The culture to use for resource lookup.</param>
    /// <returns>The resolved display name.</returns>
    protected virtual string ResolveDisplayAttribute(DisplayAttribute displayAttribute, CultureInfo culture)
    {
        // 1.1 Display attribute with ResourceType specified
        if (displayAttribute.ResourceType is not null)
        {
            // 1.1.1 Display attribute with ResourceType specified (Name)
            // 1.1.2 Display attribute with ResourceType specified (Description)
            return new ResourceManager(displayAttribute.ResourceType).GetString(displayAttribute.Name ?? displayAttribute.Description ?? string.Empty, culture).OrEmpty();
        }

        // 1.2 Display attribute with ResourceType specified (Name)
        if (!string.IsNullOrWhiteSpace(displayAttribute.Name))
        {
            return displayAttribute.Name;
        }

        // 1.3 Display attribute without ResourceType specified (Description)
        return !string.IsNullOrWhiteSpace(displayAttribute.Description) ? displayAttribute.Description : string.Empty;
    }

    /// <summary>
    /// Builds the resource key for the specified enum value, following the convention of "{EnumType}{EnumStringValue}".
    /// </summary>
    /// <param name="value">The enum value for which to build the resource key.</param>
    /// <returns>The resource key for the specified enum value.</returns>
    protected virtual string BuildResourceKey(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return keyProvider.GetKey(value);
    }

    /// <summary>
    /// Builds the translation options based on the specified display name options, including style and quantity information that can be used for translation key resolution and inflection during translation.
    /// </summary>
    /// <param name="options">The display name options to use for building the translation options.</param>
    /// <returns>The built translation options.</returns>
    protected virtual TranslationOptions BuildTranslationOptions(DisplayTextOptions options) => new TranslationOptionsBuilder().WithStyle(options.Style).WithoutKeyFallback().Build();
}
