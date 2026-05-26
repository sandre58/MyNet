// -----------------------------------------------------------------------
// <copyright file="BoundedValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Numerics;
using MyNet.Globalization.Facade;
using MyNet.Observable.Behaviors;
using MyNet.Observable.Localization;
using MyNet.Observable.Validation.Validators;
using MyNet.Primitives;
using MyNet.Primitives.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Observable numeric value with optional min/max bounds, reset support, and localized validation.
/// </summary>
/// <typeparam name="T">The numeric type.</typeparam>
public class BoundedValue<T> : ObservableObject, IBoundedValue<T>
    where T : struct, INumber<T>
{
    private static readonly Interval<T> UnboundedInterval = new(start: null, to: null);

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedValue{T}"/> class without bounds.
    /// </summary>
    /// <param name="propertyDisplayKey">Translation key used as the field name in validation messages.</param>
    /// <param name="defaultValue">Optional default value used by <see cref="Reset"/>.</param>
    public BoundedValue(string propertyDisplayKey = nameof(Value), T? defaultValue = null)
        : this(UnboundedInterval, propertyDisplayKey, defaultValue) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedValue{T}"/> class with inclusive bounds.
    /// </summary>
    /// <param name="min">Inclusive minimum, if any.</param>
    /// <param name="max">Inclusive maximum, if any.</param>
    /// <param name="propertyDisplayKey">Translation key used as the field name in validation messages.</param>
    /// <param name="defaultValue">Optional default value used by <see cref="Reset"/>.</param>
    public BoundedValue(T? min, T? max, string propertyDisplayKey = nameof(Value), T? defaultValue = null)
        : this(new(min, max), propertyDisplayKey, defaultValue) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedValue{T}"/> class.
    /// </summary>
    /// <param name="range">Inclusive bounds.</param>
    /// <param name="propertyDisplayKey">Translation key used as the field name in validation messages.</param>
    /// <param name="defaultValue">Optional default value used by <see cref="Reset"/>.</param>
    public BoundedValue(Interval<T> range, string propertyDisplayKey = nameof(Value), T? defaultValue = null)
    {
        Range = range;
        DefaultValue = defaultValue;
        this.UseValidation(new BoundedValueValidator<T>(propertyDisplayKey));
    }

    /// <summary>
    /// Gets the configured inclusive bounds.
    /// </summary>
    public Interval<T> Range { get; private set; }

    /// <inheritdoc />
    public T? Value
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                Behaviors.GetOrDefault<ValidationBehavior<BoundedValue<T>>>()?.ValidateProperty(nameof(Value));
        }
    }

    /// <inheritdoc />
    public T? Min
    {
        get => GetBound(Range.Start);
        set
        {
            if (Equals(value, Min))
                return;

            var before = Min;
            Range = new(value, Max);
            NotifyPropertyChanged(nameof(Min), before, value);
            Behaviors.GetOrDefault<ValidationBehavior<BoundedValue<T>>>()?.ValidateProperty(nameof(Value));
        }
    }

    /// <inheritdoc />
    public T? Max
    {
        get => GetBound(Range.End);
        set
        {
            if (Equals(value, Max))
                return;

            var before = Max;
            Range = new(Min, value);
            NotifyPropertyChanged(nameof(Max), before, value);
            Behaviors.GetOrDefault<ValidationBehavior<BoundedValue<T>>>()?.ValidateProperty(nameof(Value));
        }
    }

    /// <inheritdoc />
    public T? DefaultValue { get; }

    /// <inheritdoc />
    public bool CanReset() => DefaultValue.HasValue && !Equals(Value, DefaultValue);

    /// <inheritdoc />
    public void Reset() => Value = DefaultValue;

    /// <inheritdoc />
    public override string? ToString() => Value?.ToString();

    internal string BuildRangeValidationMessage(string propertyDisplayKey)
    {
        var label = propertyDisplayKey.Translate();

        return Min is { } min && Max is { } max
            ? ValidationResources.FieldMustBeBetween.FormatWith(CultureInfo.CurrentCulture, label, min, max)
            : Min is { } minOnly
            ? ValidationResources.FieldMustBeGreaterThanOrEqualTo.FormatWith(CultureInfo.CurrentCulture, label, minOnly)
            : Max is { } maxOnly
            ? ValidationResources.FieldMustBeLessThanOrEqualTo.FormatWith(CultureInfo.CurrentCulture, label, maxOnly)
            : string.Empty;
    }

    private static T? GetBound(IntervalBoundary<T>? boundary) =>
        boundary is { IsInclusive: true } ? boundary.Value.Value : null;

    /// <summary>
    /// Implicit conversion to the current value.
    /// </summary>
    public static implicit operator T?(BoundedValue<T> value) => value.Value;

    /// <summary>
    /// Returns the current value as a nullable type, which is useful when the value may be null and you want to avoid nullability warnings.
    /// </summary>
    public T? ToNullable() => Value;
}
