// -----------------------------------------------------------------------
// <copyright file="NavigationParametersExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

#pragma warning disable IDE0130
namespace MyNet.UI.Navigation.Models;
#pragma warning restore IDE0130

/// <summary>
/// Provides convenience helpers for working with <see cref="INavigationParameters"/>.
/// </summary>
public static class NavigationParametersExtensions
{
    /// <summary>
    /// Converts arbitrary values to a <see cref="NavigationParameters"/> bag.
    /// </summary>
    /// <param name="parameters">Parameter source.</param>
    /// <returns>A normalized parameter bag.</returns>
    public static NavigationParameters ToNavigationParameters(this object? parameters)
        => parameters switch
        {
            null => new(),
            NavigationParameters navigationParameters => new(navigationParameters),
            INavigationParameters typedParameters => typedParameters as NavigationParameters ?? NavigationParameters.From(typedParameters),
            IEnumerable<KeyValuePair<string, object?>> pairs => new(pairs),
            _ => NavigationParameters.From(parameters)
        };

    extension(INavigationParameters? parameters)
    {
        /// <summary>
        /// Gets a typed value from a parameter bag when it is backed by <see cref="NavigationParameters"/>.
        /// </summary>
        /// <typeparam name="T">Expected value type.</typeparam>
        /// <param name="key">Parameter key.</param>
        /// <param name="defaultValue">Fallback value.</param>
        /// <returns>The typed value or <paramref name="defaultValue"/>.</returns>
        public T? Get<T>(string key, T? defaultValue = default)
            => parameters is NavigationParameters bag ? bag.Get(key, defaultValue) : defaultValue;

        /// <summary>
        /// Tries to get a typed value from a parameter bag when it is backed by <see cref="NavigationParameters"/>.
        /// </summary>
        /// <typeparam name="T">Expected value type.</typeparam>
        /// <param name="key">Parameter key.</param>
        /// <param name="value">Resolved value.</param>
        /// <returns><see langword="true"/> when the parameter exists.</returns>
        public bool TryGetValue<T>(string key, out T? value)
        {
            if (parameters is NavigationParameters bag)
                return bag.TryGetValue(key, out value);

            value = default;
            return false;
        }
    }
}
