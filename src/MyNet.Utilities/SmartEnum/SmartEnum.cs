// -----------------------------------------------------------------------
// <copyright file="SmartEnum.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides a strongly-typed enumeration pattern with support for any comparable, equatable value type.
/// </summary>
/// <remarks>
/// <para>
/// SmartEnum is a pattern for creating type-safe enumerations with custom logic and properties.
/// It automatically registers all static instances and provides efficient lookup by value.
/// </para>
/// <para>
/// Design principles:
/// - Single Responsibility: SmartEnum only handles enumeration logic
/// - Localization/humanization is handled by extension methods (Humanizer)
/// - This class keeps concerns separated and does not mix framework-specific features.
/// </para>
/// </remarks>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "SmartEnum is a well-known pattern and the name is appropriate for its purpose.")]
public abstract class SmartEnum<TEnum, TValue> : IComparable, IComparable<SmartEnum<TEnum, TValue>>, ISmartEnum
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    private static readonly Lazy<IReadOnlyCollection<TEnum>> LazyValues = new(Discover, isThreadSafe: true);
    private static readonly ConcurrentDictionary<TValue, TEnum> ByValue = new();

    /// <summary>
    /// Gets the underlying value.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Gets the display name (optional for UI layer, override if needed).
    /// </summary>
    public virtual string Name => Value.ToString() ?? string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartEnum{TEnum,TValue}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value of the smart enum.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a SmartEnum instance with the same value already exists.</exception>
    protected SmartEnum(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;

        if (!ByValue.TryAdd(value, (TEnum)this))
        {
            throw new InvalidOperationException(
                $"A {typeof(TEnum).Name} instance with value '{value}' already exists.");
        }
    }

    /// <summary>
    /// Gets all defined instances of this smart enum.
    /// </summary>
    public static IReadOnlyCollection<TEnum> All => LazyValues.Value;

    /// <summary>
    /// Returns all instances declared as static fields.
    /// </summary>
    private static TEnum[] Discover()
    {
        var type = typeof(TEnum);

        var fields = type
            .GetFields(BindingFlags.Public |
                       BindingFlags.Static |
                       BindingFlags.DeclaredOnly);

        var values = fields
            .Where(f => f.FieldType == typeof(TEnum))
            .Select(f => (TEnum)f.GetValue(null)!)
            .ToArray();

        foreach (var v in values)
            ByValue.TryAdd(v.Value, v);

        return values;
    }

    /// <summary>
    /// Try to get an enum instance from value.
    /// </summary>
    public static bool TryFromValue(TValue value, out TEnum? result)
    {
        if (ByValue.TryGetValue(value, out var match))
        {
            result = match;
            return true;
        }

        result = null;
        return false;
    }

    /// <summary>
    /// Get an enum instance from value.
    /// </summary>
    public static TEnum FromValue(TValue value) =>
        TryFromValue(value, out var result) && result is not null
            ? result
            : throw new KeyNotFoundException(
                $"No {typeof(TEnum).Name} found for value '{value}'.");

    #region Equality

    /// <summary>
    /// Determines whether the specified object is equal to the current instance. Two smart enum instances are considered equal if they are of the same type and have the same underlying value. This method overrides the default implementation to provide value-based equality semantics, allowing smart enum instances to be compared based on their values rather than their references. By implementing this method, you can ensure that smart enum instances behave as expected when used in collections, comparisons, or any scenario where equality checks are performed.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is SmartEnum<TEnum, TValue> other && Value.Equals(other.Value);

    /// <summary>
    /// Returns a hash code for the current instance. The hash code is based on the underlying value of the smart enum instance, ensuring that instances with the same value will have the same hash code. This method overrides the default implementation to provide a consistent hash code generation strategy that aligns with the equality semantics defined in the Equals method. By implementing this method, you can ensure that smart enum instances can be used effectively in hash-based collections, such as dictionaries or hash sets, where proper hash code generation is essential for performance and correctness.
    /// </summary>
    /// <returns>A hash code for the current instance.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Determines whether two smart enum instances are equal. This operator overload allows you to compare two smart enum instances using the equality operator (==). It checks if the left instance is not null and then calls the Equals method to determine if the two instances are equal based on their underlying values. If the left instance is null, it checks if the right instance is also null, in which case they are considered equal. By implementing this operator overload, you can provide a more intuitive and convenient way to compare smart enum instances directly using the equality operator, enhancing the readability and usability of your code when working with smart enums.
    /// </summary>
    /// <param name="left">The left smart enum instance to compare.</param>
    /// <param name="right">The right smart enum instance to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right) => left?.Equals(right) ?? right is null;

    /// <summary>
    /// Determines whether two smart enum instances are not equal. This operator overload allows you to compare two smart enum instances using the inequality operator (!=). It negates the result of the equality operator (==) to determine if the two instances are not equal based on their underlying values. By implementing this operator overload, you can provide a more intuitive and convenient way to compare smart enum instances directly using the inequality operator, enhancing the readability and usability of your code when working with smart enums.
    /// </summary>
    /// <param name="left">The left smart enum instance to compare.</param>
    /// <param name="right">The right smart enum instance to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right) => !(left == right);

    #endregion

    #region Comparison

    /// <summary>
    /// Compares the current instance with another smart enum instance of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other instance. The comparison is based on the underlying value of the smart enum instances, allowing you to sort or order them according to their values. If the other instance is null, the current instance is considered greater and will return a positive integer. By implementing this method, you can enable sorting and ordering of smart enum instances in collections or any scenario where comparison is needed, providing a consistent way to compare and sort smart enums based on their values.
    /// </summary>
    /// <param name="other">The other smart enum instance to compare.</param>
    /// <returns>A signed integer that indicates the relative order of the instances being compared.</returns>
    public int CompareTo(SmartEnum<TEnum, TValue>? other) => other is null ? 1 : Value.CompareTo(other.Value);

    /// <summary>
    /// Compares the current instance with another object and returns an integer that indicates the relative order.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>A signed integer that indicates the relative order of the instances being compared.</returns>
    /// <exception cref="ArgumentException">Thrown when obj is not a SmartEnum instance or null.</exception>
    int IComparable.CompareTo(object? obj) => obj is SmartEnum<TEnum, TValue> other
        ? CompareTo(other)
        : throw new ArgumentException($"Object must be of type {typeof(TEnum).Name}.", nameof(obj));

    /// <summary>
    /// Determines whether one smart enum instance is less than another. This operator overload allows you to compare two smart enum instances using the less-than operator. It calls the CompareTo method to determine if the left instance precedes the right instance in the sort order based on their underlying values. By implementing this operator overload, you can provide a more intuitive and convenient way to compare smart enum instances directly using the less-than operator, enhancing the readability and usability of your code when working with smart enums.
    /// </summary>
    /// <param name="left">The left smart enum instance to compare.</param>
    /// <param name="right">The right smart enum instance to compare.</param>
    /// <returns>True if the left instance is less than the right instance; otherwise, false.</returns>
    public static bool operator <(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Determines whether one smart enum instance is less than or equal to another. This operator overload allows you to compare two smart enum instances using the less-than-or-equal-to operator. It calls the CompareTo method to determine if the left instance precedes or is equal to the right instance in the sort order based on their underlying values. By implementing this operator overload, you can provide a more intuitive and convenient way to compare smart enum instances directly using the less-than-or-equal-to operator, enhancing the readability and usability of your code when working with smart enums.
    /// </summary>
    /// <param name="left">The left smart enum instance to compare.</param>
    /// <param name="right">The right smart enum instance to compare.</param>
    /// <returns>True if the left instance is less than or equal to the right instance; otherwise, false.</returns>
    public static bool operator <=(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Determines whether one smart enum instance is greater than another. This operator overload allows you to compare two smart enum instances using the greater-than operator. It calls the CompareTo method to determine if the left instance follows the right instance in the sort order based on their underlying values. By implementing this operator overload, you can provide a more intuitive and convenient way to compare smart enum instances directly using the greater-than operator, enhancing the readability and usability of your code when working with smart enums.
    /// </summary>
    /// <param name="left">The left smart enum instance to compare.</param>
    /// <param name="right">The right smart enum instance to compare.</param>
    /// <returns>True if the left instance is greater than the right instance; otherwise, false.</returns>
    public static bool operator >(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Determines whether one smart enum instance is greater than or equal to another. This operator overload allows you to compare two smart enum instances using the greater-than-or-equal-to operator. It calls the CompareTo method to determine if the left instance follows or is equal to the right instance in the sort order based on their underlying values. By implementing this operator overload, you can provide a more intuitive and convenient way to compare smart enum instances directly using the greater-than-or-equal-to operator, enhancing the readability and usability of your code when working with smart enums.
    /// </summary>
    /// <param name="left">The left smart enum instance to compare.</param>
    /// <param name="right">The right smart enum instance to compare.</param>
    /// <returns>True if the left instance is greater than or equal to the right instance; otherwise, false.</returns>
    public static bool operator >=(SmartEnum<TEnum, TValue> left, SmartEnum<TEnum, TValue> right) => left.CompareTo(right) >= 0;

    #endregion

    /// <summary>
    /// Returns a string that represents the current instance. By default, this method returns the Name property of the smart enum instance, which is typically a user-friendly display name. You can override this method in derived classes to provide a custom string representation if needed. By implementing this method, you can ensure that smart enum instances have a meaningful and readable string representation when used in logging, debugging, or any scenario where a string representation of the instance is required.
    /// </summary>
    /// <returns>A string that represents the current instance.</returns>
    public override string ToString() => Name;
}
