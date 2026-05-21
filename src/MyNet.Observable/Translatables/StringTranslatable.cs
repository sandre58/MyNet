// -----------------------------------------------------------------------
// <copyright file="StringTranslatable.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Humanizer;
using MyNet.Utilities;
using MyNet.Utilities.Text.TextCasing;

namespace MyNet.Observable.Translatables;

public class StringTranslatable(string key, LetterCasing casing = LetterCasing.Normal, string? filename = "") : Translatable<string>(() => string.IsNullOrEmpty(key) ? string.Empty : string.IsNullOrEmpty(filename) ? key.Translate().ApplyCase(casing) : key.Translate(filename).ApplyCase(casing))
{
    public string Key { get; } = key;

    public override bool Equals(object? obj) => obj is StringTranslatable o && Key.Equals(o.Key, System.StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => Key.GetHashCode(System.StringComparison.CurrentCultureIgnoreCase);
}
