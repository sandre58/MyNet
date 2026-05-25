// -----------------------------------------------------------------------
// <copyright file="LocalizedEnum.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using MyNet.Humanizer;
using MyNet.Humanizer.Static;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Observable.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Culture-bound display text for a <see cref="Enum"/> value.
/// </summary>
/// <param name="enumValue">The enum value to present.</param>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "LocalizedEnum is more intuitive than LocalizedEnum<Enum> for non-generic usage.")]
public class LocalizedEnum(Enum enumValue) : LocalizedEnum<Enum>(enumValue);

/// <summary>
/// Culture-bound display text for an enum value.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
/// <param name="enumValue">The enum value to present.</param>
[ExemptFromGeneratedMetadata]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "LocalizedEnum is more intuitive than LocalizedEnum<TEnum> for generic usage.")]
public class LocalizedEnum<TEnum>(TEnum enumValue) : CultureBoundValue<TEnum>(() => enumValue)
    where TEnum : Enum
{
    /// <summary>
    /// Gets the description from <see cref="System.ComponentModel.DescriptionAttribute"/> when present.
    /// </summary>
    public string Description => Value is null ? string.Empty : Value.GetDescription() ?? string.Empty;

    /// <summary>
    /// Gets the localized display text for the enum value.
    /// </summary>
    public string Display => Value?.Humanize() ?? string.Empty;

    /// <inheritdoc />
    public override string ToString() => Display;

    /// <inheritdoc />
    public override void OnEvent(CultureChangedEvent e)
    {
        base.OnEvent(e);
        NotifyPropertyChanged(nameof(Display));
        NotifyPropertyChanged(nameof(Description));
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LocalizedEnum<TEnum> result && (result.Value?.Equals(Value) ?? false);

    /// <inheritdoc />
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
}
