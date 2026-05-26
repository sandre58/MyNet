// -----------------------------------------------------------------------
// <copyright file="LocalizedString.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Culture;
using MyNet.Globalization.Facade;
using MyNet.Text;
using MyNet.Text.TextCasing;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Culture-bound string resolved from a translation resource key.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LocalizedString"/> class with an explicit culture service.
/// </remarks>
/// <param name="key">The translation resource key.</param>
/// <param name="cultureService">Culture service used to detect culture changes.</param>
/// <param name="casing">Optional letter casing applied after translation.</param>
/// <param name="filename">Optional resource file name.</param>
public class LocalizedString(string key, ICultureService cultureService, LetterCasing casing = LetterCasing.Normal, string? filename = "") : CultureBoundValue<string>(() => Resolve(key, casing, filename), cultureService)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedString"/> class using <see cref="GlobalizationServices.Current"/>.
    /// </summary>
    /// <param name="key">The translation resource key.</param>
    /// <param name="casing">Optional letter casing applied after translation.</param>
    /// <param name="filename">Optional resource file name.</param>
    public LocalizedString(string key, LetterCasing casing = LetterCasing.Normal, string? filename = "")
        : this(key, GlobalizationServices.Current, casing, filename) { }

    /// <summary>
    /// Gets the translation resource key.
    /// </summary>
    public string Key { get; } = key;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LocalizedString o && Key.Equals(o.Key, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override int GetHashCode() => Key.GetHashCode(StringComparison.CurrentCultureIgnoreCase);

    private static string Resolve(string key, LetterCasing casing, string? filename)
        => string.IsNullOrEmpty(key)
            ? string.Empty
            : string.IsNullOrEmpty(filename)
                ? key.Translate().ApplyCase(casing)
                : key.Translate(filename).ApplyCase(casing);
}
