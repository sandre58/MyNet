// -----------------------------------------------------------------------
// <copyright file="NavigationParameters.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace MyNet.UI.Navigation.Models;

/// <summary>
/// Represents a lightweight, mutable bag of navigation parameters.
/// </summary>
[SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "NavigationParameters is the established public terminology for this bag-based payload.")]
public sealed class NavigationParameters : INavigationParameters, IReadOnlyDictionary<string, object?>
{
    private readonly Dictionary<string, object?> _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    public NavigationParameters()
        : this(Enumerable.Empty<KeyValuePair<string, object?>>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    /// <param name="values">Initial values.</param>
    public NavigationParameters(IEnumerable<KeyValuePair<string, object?>> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = new(values, StringComparer.Ordinal);
    }

    /// <summary>
    /// Gets an empty parameter bag.
    /// </summary>
    public static NavigationParameters Empty { get; } = new();

    /// <inheritdoc />
    public IEnumerable<string> Keys => _values.Keys;

    /// <inheritdoc />
    public IEnumerable<object?> Values => _values.Values;

    /// <inheritdoc />
    public int Count => _values.Count;

    /// <inheritdoc />
    public object? this[string key] => _values[key];

    /// <summary>
    /// Creates a parameter bag from an existing object.
    /// </summary>
    /// <param name="values">Anonymous object, dictionary, or parameter bag.</param>
    /// <returns>A navigation parameter bag.</returns>
    public static NavigationParameters From(object? values)
    {
        switch (values)
        {
            case null:
                return new();
            case NavigationParameters parameters:
                return new(parameters);
            case IReadOnlyDictionary<string, object?> readOnlyDictionary:
                return new(readOnlyDictionary);
            case IEnumerable<KeyValuePair<string, object?>> pairs:
                return new(pairs);
            case IDictionary dictionary:
                {
                    var entries = new List<KeyValuePair<string, object?>>();
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        if (entry.Key is string key)
                            entries.Add(new(key, entry.Value));
                    }

                    return new(entries);
                }

            default:
                return new(values.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
                    .Select(property => new KeyValuePair<string, object?>(property.Name, property.GetValue(values))));
        }
    }

    /// <summary>
    /// Adds or replaces a parameter value.
    /// </summary>
    /// <param name="key">Parameter key.</param>
    /// <param name="value">Parameter value.</param>
    /// <returns>The same parameter bag for chaining.</returns>
    public NavigationParameters Set(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _values[key] = value;
        return this;
    }

    /// <summary>
    /// Gets a parameter value with type conversion support when possible.
    /// </summary>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <param name="key">Parameter key.</param>
    /// <param name="defaultValue">Default value when the parameter is absent or incompatible.</param>
    /// <returns>The converted value or <paramref name="defaultValue"/>.</returns>
    public T? Get<T>(string key, T? defaultValue = default)
        => TryGetValue(key, out T? value) ? value : defaultValue;

    /// <summary>
    /// Tries to retrieve a typed parameter value.
    /// </summary>
    /// <typeparam name="T">Expected value type.</typeparam>
    /// <param name="key">Parameter key.</param>
    /// <param name="value">Resolved value when found.</param>
    /// <returns><see langword="true"/> when the value exists and can be read.</returns>
    public bool TryGetValue<T>(string key, out T? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        if (!_values.TryGetValue(key, out var rawValue))
        {
            value = default;
            return false;
        }

        return NavigationParameterConversions.TryConvert(rawValue, out value);
    }

    /// <inheritdoc />
    public bool ContainsKey(string key) => _values.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, out object? value) => _values.TryGetValue(key, out value);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
