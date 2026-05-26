// -----------------------------------------------------------------------
// <copyright file="LocalizedSmartEnum.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MyNet.Humanizer.Facade;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Observable.Metadata;
using MyNet.Primitives;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Culture-bound display text for a <see cref="ISmartEnum"/> value.
/// </summary>
/// <param name="enumValue">The smart enum value to present.</param>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "LocalizedSmartEnum is more intuitive than LocalizedSmartEnum<TEnum> for generic usage.")]
public class LocalizedSmartEnum(ISmartEnum enumValue) : LocalizedSmartEnum<ISmartEnum>(enumValue);

/// <summary>
/// Culture-bound display text for a smart enum value.
/// </summary>
/// <typeparam name="TEnum">The smart enum type.</typeparam>
/// <param name="enumValue">The smart enum value to present.</param>
[ExemptFromGeneratedMetadata]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "LocalizedSmartEnum is more intuitive than LocalizedSmartEnum<TEnum> for generic usage.")]
public class LocalizedSmartEnum<TEnum>(TEnum enumValue) : CultureBoundValue<TEnum>(() => enumValue)
    where TEnum : ISmartEnum
{
    /// <summary>
    /// Gets the localized display text for the smart enum value.
    /// </summary>
    public string Display => Value?.Humanize() ?? string.Empty;

    /// <inheritdoc />
    public override string ToString() => Display;

    /// <inheritdoc />
    public override void OnEvent(CultureChangedEvent e)
    {
        base.OnEvent(e);
        NotifyPropertyChanged(nameof(Display));
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LocalizedSmartEnum<TEnum> result && (result.Value?.Equals(Value) ?? false);

    /// <inheritdoc />
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
}
