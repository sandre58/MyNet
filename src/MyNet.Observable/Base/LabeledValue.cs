// -----------------------------------------------------------------------
// <copyright file="LabeledValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Pairs a value with a culture-bound display label (without wrapper inheritance).
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
/// <param name="Value">The underlying value.</param>
/// <param name="DisplayName">The localized label provider.</param>
public sealed record LabeledValue<T>(T Value, IObservableValue<string> DisplayName)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabeledValue{T}"/> class with a resource key label.
    /// </summary>
    /// <param name="value">The underlying value.</param>
    /// <param name="resourceKey">The translation resource key.</param>
    public LabeledValue(T value, string resourceKey)
        : this(value, new LocalizedString(resourceKey)) { }

    /// <inheritdoc />
    public override string ToString() => DisplayName.Value ?? string.Empty;
}
