// -----------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods for dictionary-like collections.
/// </summary>
public static class DictionaryExtensions
{
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        /// <summary>
        /// Gets the value for the specified key or adds the provided default value if missing.
        /// </summary>
        public TValue GetOrCreate(TKey key, Func<TValue> factory)
        {
            ArgumentNullException.ThrowIfNull(dictionary);

            if (!dictionary.TryGetValue(key, out var value))
            {
                value = factory();
                dictionary.Add(key, value);
            }

            return value;
        }

        /// <summary>
        /// Gets the value for the specified key or returns a default when missing.
        /// </summary>
        public TValue? GetOrDefault(TKey key, TValue? defaultValue = default)
        {
            ArgumentNullException.ThrowIfNull(dictionary);

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Merges two dictionaries into a new dictionary. Keys are expected to be unique.
        /// </summary>
        public Dictionary<TKey, TValue> Merge(IDictionary<TKey, TValue> dictionary2)
        {
            ArgumentNullException.ThrowIfNull(dictionary);

            return new[] { dictionary, dictionary2 }.Merge();
        }
    }

    /// <summary>
    /// Merges multiple dictionaries into a new dictionary. Keys are expected to be unique.
    /// </summary>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<IDictionary<TKey, TValue>> other)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(other);

        var result = new Dictionary<TKey, TValue>();

        foreach (var dictionary in other)
        {
            foreach (var pair in dictionary)
                result[pair.Key] = pair.Value;
        }

        return result;
    }
}
