// -----------------------------------------------------------------------
// <copyright file="SmartEnumExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using MyNet.Humanizer.Display;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Contains extension methods for humanizing Enums.
/// </summary>
public static class SmartEnumExtensions
{
    private static readonly ConcurrentDictionary<CacheKey, FrozenDictionary<string, ISmartEnum>> Cache = new();

    extension(ISmartEnum value)
    {
        /// <summary>
        /// Returns the humanized display name of the SmartEnum value using the default options. If no display name is found, the enum value's name is returned.
        /// </summary>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The humanized display name of the SmartEnum value.</returns>
        public string Humanize(CultureInfo? culture = null) => value.Humanize(DisplayTextOptions.Default, culture);

        /// <summary>
        /// Returns the humanized display name of the SmartEnum value using the specified options. If no display name is found, the enum value's name is returned.
        /// Uses the DI-registered <see cref="IDisplayTextStrategy{T}"/>.
        /// </summary>
        /// <param name="options">The options to use when retrieving the display name.</param>
        /// <param name="culture">The culture to use when retrieving the display name.</param>
        /// <returns>The humanized display name of the SmartEnum value.</returns>
        public string Humanize(DisplayTextOptions options, CultureInfo? culture = null) => TextHumanizer.Humanize(value, options, culture);
    }

    extension(string? input)
    {
        /// <summary>
        /// Dehumanizes the input string to the specified target enum type. The method first checks if the target enum type implements ISmartEnum, and then it retrieves a lookup dictionary for the target enum type and culture from the cache. The lookup dictionary maps both the enum member names and their humanized versions to the corresponding enum members. The method then attempts to find a match for the input string in the lookup dictionary, and if a match is found, it returns the corresponding enum member. If no match is found, it throws a KeyNotFoundException indicating that no matching enum member could be found for the input string.
        /// </summary>
        /// <param name="culture">The culture to use for humanization.</param>
        /// <typeparam name="TSmartEnum">The type of the target enum.</typeparam>
        /// <returns>The corresponding enum member.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the targetEnum is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the targetEnum does not implement ISmartEnum.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no matching enum member is found.</exception>
        public TSmartEnum DehumanizeTo<TSmartEnum>(CultureInfo? culture = null)
            where TSmartEnum : class, ISmartEnum
            => (TSmartEnum)DehumanizeCore(input, typeof(TSmartEnum), culture);

        /// <summary>
        /// Dehumanizes the input string to the specified target enum type. The method first checks if the target enum type implements ISmartEnum, and then it retrieves a lookup dictionary for the target enum type and culture from the cache. The lookup dictionary maps both the enum member names and their humanized versions to the corresponding enum members. The method then attempts to find a match for the input string in the lookup dictionary, and if a match is found, it returns the corresponding enum member. If no match is found, it throws a KeyNotFoundException indicating that no matching enum member could be found for the input string.
        /// </summary>
        /// <param name="targetEnum">The target enum type.</param>
        /// <param name="culture">The culture to use for humanization.</param>
        /// <returns>The corresponding enum member.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the targetEnum is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the targetEnum does not implement ISmartEnum.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no matching enum member is found.</exception>
        public ISmartEnum DehumanizeTo(Type targetEnum, CultureInfo? culture = null) => (ISmartEnum)DehumanizeCore(input, targetEnum, culture);

        /// <summary>
        /// Tries to dehumanize the input string to the specified target enum type. The method first checks if the target enum type implements ISmartEnum, and then it retrieves a lookup dictionary for the target enum type and culture from the cache. The lookup dictionary maps both the enum member names and their humanized versions to the corresponding enum members. The method then attempts to find a match for the input string in the lookup dictionary, and if a match is found, it returns true and sets the result to the corresponding enum member. If no match is found, it returns false and sets the result to null.
        /// </summary>
        /// <param name="result">The resulting enum member if a match is found; otherwise, null.</param>
        /// <param name="culture">The culture to use for humanization.</param>
        /// <typeparam name="TSmartEnum">The type of the target enum.</typeparam>
        /// <returns>True if a match is found; otherwise, false.</returns>
        public bool TryDehumanizeTo<TSmartEnum>(out TSmartEnum? result, CultureInfo? culture = null)
            where TSmartEnum : class, ISmartEnum
        {
            try
            {
                result = input.DehumanizeTo<TSmartEnum>(culture);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Dehumanizes the input string to the specified target enum type. The method first checks if the target enum type implements ISmartEnum, and then it retrieves a lookup dictionary for the target enum type and culture from the cache. The lookup dictionary maps both the enum member names and their humanized versions to the corresponding enum members. The method then attempts to find a match for the input string in the lookup dictionary, and if a match is found, it returns the corresponding enum member. If no match is found, it throws a KeyNotFoundException indicating that no matching enum member could be found for the input string.
    /// </summary>
    /// <param name="input">The input string to dehumanize.</param>
    /// <param name="targetEnum">The target enum type.</param>
    /// <param name="culture">The culture to use for humanization.</param>
    /// <returns>The corresponding enum member.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the targetEnum is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the targetEnum does not implement ISmartEnum.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if no matching enum member is found.</exception>
    private static object DehumanizeCore(string? input, Type targetEnum, CultureInfo? culture)
    {
        ArgumentNullException.ThrowIfNull(targetEnum);

        if (!typeof(ISmartEnum).IsAssignableFrom(targetEnum))
        {
            throw new ArgumentException($"Type '{targetEnum}' does not implement ISmartEnum.", nameof(targetEnum));
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new KeyNotFoundException($"Input string is null or whitespace, cannot dehumanize to enum type '{targetEnum.Name}'.");
        }

        var lookup = GetLookup(targetEnum, culture);

        return lookup.TryGetValue(input, out var result) ? result : throw new KeyNotFoundException($"Couldn't find any enum member of type '{targetEnum.Name}' matching '{input}'.");
    }

    /// <summary>
    /// Retrieves a lookup dictionary for the specified target enum type and culture from the cache. If the lookup does not exist in the cache, it is created using the CreateLookup method and added to the cache before being returned. The lookup dictionary maps both the enum member names and their humanized versions to the corresponding enum members, allowing for efficient dehumanization of input strings to their corresponding enum members based on either their original names or their humanized forms.
    /// </summary>
    /// <param name="targetEnum">The target enum type.</param>
    /// <param name="culture">The culture to use for humanization.</param>
    /// <returns>A frozen dictionary mapping strings to enum members.</returns>
    private static FrozenDictionary<string, ISmartEnum> GetLookup(Type targetEnum, CultureInfo? culture)
    {
        var key = new CacheKey(targetEnum, culture?.Name);

        return Cache.GetOrAdd(key, static key => CreateLookup(key.Type, key.Culture));
    }

    /// <summary>
    /// Creates a lookup dictionary for the specified target enum type and culture. The dictionary maps both the enum member names and their humanized versions to the corresponding enum members. The humanized versions are generated using the Humanize extension method, which takes into account the specified culture for localization. This allows for efficient dehumanization of input strings to their corresponding enum members based on either their original names or their humanized forms.
    /// </summary>
    /// <param name="targetEnum">The target enum type.</param>
    /// <param name="cultureName">The name of the culture to use for humanization.</param>
    /// <returns>A frozen dictionary mapping strings to enum members.</returns>
    private static FrozenDictionary<string, ISmartEnum> CreateLookup(Type targetEnum, string? cultureName)
    {
        var culture = cultureName is null ? null : CultureInfo.GetCultureInfo(cultureName);

        var dictionary = new Dictionary<string, ISmartEnum>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in targetEnum.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
        {
            if (field.GetValue(null) is not ISmartEnum value)
            {
                continue;
            }

            dictionary.TryAdd(value.ToString()!, value);

            var humanized = value.Humanize(culture: culture);

            dictionary.TryAdd(humanized, value);
        }

        return dictionary.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// A struct used as a key for caching the lookup dictionaries for dehumanization. It combines the target enum type and the culture name to ensure that lookups are cached separately for different types and cultures.
    /// </summary>
    /// <param name="Type">The target enum type.</param>
    /// <param name="Culture">The culture name.</param>
    private readonly record struct CacheKey(Type Type, string? Culture);
}
