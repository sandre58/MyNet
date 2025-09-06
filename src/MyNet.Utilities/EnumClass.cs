// -----------------------------------------------------------------------
// <copyright file="EnumClass.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MyNet.Utilities;

/// <summary>
/// Provides helper methods for working with smart enum classes derived from <see cref="EnumClass{TEnum, TValue}"/>.
/// </summary>
public static class EnumClass
{
    /// <summary>
    /// Gets all defined instances of the specified smart enum type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The smart enum type.</typeparam>
    /// <returns>An array containing all instances of the smart enum type.</returns>
    public static T[] GetAll<T>()
        where T : EnumClass<T>
    {
        var baseType = typeof(T);
        return
        [
            .. Assembly.GetAssembly(baseType)!
                .GetTypes()
                .Where(baseType.IsAssignableFrom)
                .SelectMany(GetFieldsOfType<T>)
                .OrderBy(t => t.Name)
        ];
    }

    /// <summary>
    /// Gets all defined instances of the specified smart enum type <typeparamref name="T"/> with an inner value of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="T">The smart enum type.</typeparam>
    /// <typeparam name="TValue">The inner value type.</typeparam>
    /// <returns>An array containing all instances of the smart enum type.</returns>
    public static T[] GetAll<T, TValue>()
        where T : EnumClass<T, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        var baseType = typeof(T);
        return
        [
            .. Assembly.GetAssembly(baseType)!
                .GetTypes()
                .Where(baseType.IsAssignableFrom)
                .SelectMany(GetFieldsOfType<T>)
                .OrderBy(t => t.Name)
        ];
    }

    /// <summary>
    /// Gets all defined instances of the smart enum type represented by <paramref name="baseType"/>.
    /// </summary>
    /// <param name="baseType">The base smart enum type.</param>
    /// <returns>An array of objects representing the enum instances.</returns>
    public static object[] GetAll(Type baseType)
        => [.. Assembly.GetAssembly(baseType)!
            .GetTypes()
            .Where(baseType.IsAssignableFrom)
            .SelectMany(GetFieldsOfType<object>)];

    private static List<TFieldType> GetFieldsOfType<TFieldType>(Type type)
        => [.. type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(p => type.IsAssignableFrom(p.FieldType))
            .Select(pi => (TFieldType?)pi.GetValue(null))
            .NotNull()];
}

/// <summary>
/// A base type to use for creating smart enums with inner value of type <see cref="int"/>.
/// </summary>
/// <typeparam name="TEnum">The type that is inheriting from this class.</typeparam>
public abstract class EnumClass<TEnum>(string name, int value, string? resourceKey = null) :
    EnumClass<TEnum, int>(name, value, resourceKey)
    where TEnum : EnumClass<TEnum, int>;

/// <summary>
/// A base type to use for creating smart enums.
/// </summary>
/// <typeparam name="TEnum">The type that is inheriting from this class.</typeparam>
/// <typeparam name="TValue">The type of the inner value.</typeparam>
public abstract class EnumClass<TEnum, TValue> :
    IEnumeration,
    IEquatable<EnumClass<TEnum, TValue>>,
    IComparable<EnumClass<TEnum, TValue>>,
    IComparable,
    IConvertible
    where TEnum : EnumClass<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    private static readonly Lazy<TEnum[]> EnumOptions = new(EnumClass.GetAll<TEnum, TValue>, LazyThreadSafetyMode.ExecutionAndPublication);
    private static readonly Lazy<Dictionary<string, TEnum>> FromNameValues = new(() => EnumOptions.Value.ToDictionary(item => item.Name));
    private static readonly Lazy<Dictionary<string, TEnum>> FromNameIgnoreCase = new(() => EnumOptions.Value.ToDictionary(item => item.Name, StringComparer.OrdinalIgnoreCase));
    private static readonly Lazy<Dictionary<TValue, TEnum>> FromValueNames =
        new(() =>
        {
            // multiple enums with same value are allowed but store only one per value
            var dictionary = new Dictionary<TValue, TEnum>();
            foreach (var item in EnumOptions.Value)
                _ = dictionary.TryAdd(item.Value, item);
            return dictionary;
        });

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumClass{TEnum, TValue}"/> class.
    /// </summary>
    /// <param name="name">The name of the enum option.</param>
    /// <param name="value">The underlying value of the option.</param>
    /// <param name="resourceKey">An optional resource key used for localization; when null the <paramref name="name"/> is used.</param>
    protected EnumClass(string name, TValue value, string? resourceKey = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(value);

        Name = name;
        Value = value;
        ResourceKey = resourceKey ?? Name;
    }

    /// <summary>
    /// Gets a collection containing all the instances of <see cref="EnumClass{TEnum, TValue}"/>.
    /// </summary>
    /// <value>A <see cref="IReadOnlyCollection{TEnum}"/> containing all the instances of <see cref="EnumClass{TEnum, TValue}"/>.</value>
    /// <remarks>Retrieves all the instances of <see cref="EnumClass{TEnum, TValue}"/> referenced by public static read-only fields in the current class or its bases.</remarks>
    public static IReadOnlyCollection<TEnum> List =>
        FromNameValues.Value.Values
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>A <see cref="string"/> that is the name of the <see cref="EnumClass{TEnum, TValue}"/>.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the resource key used for localization lookups.
    /// </summary>
    public string ResourceKey { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>A <typeparamref name="TValue"/> that is the value of the <see cref="EnumClass{TEnum, TValue}"/>.</value>
    public TValue Value { get; }

    object IEnumeration.Value => Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TValue(EnumClass<TEnum, TValue> smartEnum) => smartEnum.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator EnumClass<TEnum, TValue>(TValue value) => FromTValue(value);

    public static bool operator ==(EnumClass<TEnum, TValue>? left, EnumClass<TEnum, TValue>? right)
    {
        // Handle null on left side
        if (left is null)
            return right is null; // null == null = true

        // Equals handles null on right side
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(EnumClass<TEnum, TValue> left, EnumClass<TEnum, TValue> right) =>
        !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(EnumClass<TEnum, TValue> left, EnumClass<TEnum, TValue> right) =>
        left.CompareTo(right) < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(EnumClass<TEnum, TValue> left, EnumClass<TEnum, TValue> right) =>
        left.CompareTo(right) <= 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(EnumClass<TEnum, TValue> left, EnumClass<TEnum, TValue> right) =>
        left.CompareTo(right) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(EnumClass<TEnum, TValue> left, EnumClass<TEnum, TValue> right) =>
        left.CompareTo(right) >= 0;

    /// <summary>
    /// Gets the item associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the item to get.</param>
    /// <param name="ignoreCase"><c>true</c> to ignore case during the comparison; otherwise, <c>false</c>.</param>
    /// <returns>
    /// The item associated with the specified name.
    /// If the specified name is not found, throws a <see cref="KeyNotFoundException"/>.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <c>null</c>.</exception>
    /// <seealso cref="EnumClass{TEnum, TValue}.TryFromName(string, out TEnum)"/>
    /// <seealso cref="EnumClass{TEnum, TValue}.TryFromName(string, bool, out TEnum)"/>
    public static TEnum? FromName(string name, bool ignoreCase = false)
    {
        return string.IsNullOrEmpty(name)
            ? throw new ArgumentException(null, nameof(name))
            : ignoreCase ? fromName(FromNameIgnoreCase.Value) : fromName(FromNameValues.Value);

        TEnum? fromName(Dictionary<string, TEnum> dictionary)
            => dictionary.GetValueOrDefault(name);
    }

    /// <summary>
    /// Gets the item associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified name, if the key is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="EnumClass{TEnum, TValue}"/> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <c>null</c>.</exception>
    /// <seealso cref="EnumClass{TEnum, TValue}.FromName(string, bool)"/>
    /// <seealso cref="EnumClass{TEnum, TValue}.TryFromName(string, bool, out TEnum)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFromName(string name, out TEnum? result) =>
        TryFromName(name, false, out result);

    /// <summary>
    /// Gets the item associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the item to get.</param>
    /// <param name="ignoreCase"><c>true</c> to ignore case during the comparison; otherwise, <c>false</c>.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified name, if the name is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="EnumClass{TEnum, TValue}"/> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <c>null</c>.</exception>
    /// <seealso cref="EnumClass{TEnum, TValue}.FromName(string, bool)"/>
    /// <seealso cref="EnumClass{TEnum, TValue}.TryFromName(string, out TEnum)"/>
    public static bool TryFromName(string name, bool ignoreCase, out TEnum? result)
    {
        if (!string.IsNullOrEmpty(name))
            return ignoreCase ? FromNameIgnoreCase.Value.TryGetValue(name, out result) : FromNameValues.Value.TryGetValue(name, out result);

        result = null;
        return false;
    }

    /// <summary>
    /// Gets an item associated with the specified value.
    /// </summary>
    /// <param name="value">The value of the item to get.</param>
    /// <returns>
    /// The first item found that is associated with the specified value.
    /// If the specified value is not found, throws a <see cref="KeyNotFoundException"/>.
    /// </returns>
    /// <seealso cref="EnumClass{TEnum, TValue}.FromValue(TValue, TEnum)"/>
    /// <seealso cref="EnumClass{TEnum, TValue}.TryFromValue(TValue, out TEnum)"/>
    public static TEnum FromValue(TValue value)
        => value is null
            ? throw new ArgumentException(null, nameof(value))
            : !FromValueNames.Value.TryGetValue(value, out var result)
                ? throw new InvalidOperationException($"{value} not found in enumeration")
                : result;

    /// <summary>
    /// Gets an item associated with the specified value.
    /// </summary>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="defaultValue">The value to return when item not found.</param>
    /// <returns>
    /// The first item found that is associated with the specified value.
    /// If the specified value is not found, returns <paramref name="defaultValue"/>.
    /// </returns>
    /// <seealso cref="EnumClass{TEnum, TValue}.FromValue(TValue)"/>
    /// <seealso cref="EnumClass{TEnum, TValue}.TryFromValue(TValue, out TEnum)"/>
    public static TEnum FromValue(TValue value, TEnum defaultValue)
        => FromValueNames.Value.GetValueOrDefault(value, defaultValue);

    /// <summary>
    /// Gets an item associated with the specified value.
    /// </summary>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="EnumClass{TEnum, TValue}"/> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    /// <seealso cref="EnumClass{TEnum, TValue}.FromValue(TValue)"/>
    /// <seealso cref="EnumClass{TEnum, TValue}.FromValue(TValue, TEnum)"/>
    public static bool TryFromValue(TValue? value, out TEnum? result)
    {
        if (value is not null) return FromValueNames.Value.TryGetValue(value, out result);
        result = null;
        return false;
    }

    /// <summary>
    /// Returns the enum instance associated with the specified underlying value.
    /// </summary>
    public static EnumClass<TEnum, TValue> FromTValue(TValue value) => FromValue(value);

    /// <summary>
    /// Converts this instance to its underlying value type.
    /// </summary>
    public TValue ToTValue() => Value;

    public override string ToString() =>
        Name;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() =>
        Value.GetHashCode();

    public override bool Equals(object? obj) =>
        obj is EnumClass<TEnum, TValue> other && Equals(other);

    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified <see cref="EnumClass{TEnum, TValue}"/> value.
    /// </summary>
    /// <param name="other">An <see cref="EnumClass{TEnum, TValue}"/> value to compare to this instance.</param>
    /// <returns><c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
    public virtual bool Equals(EnumClass<TEnum, TValue>? other)
    {
        // check if same instance
        if (ReferenceEquals(this, other))
            return true;

        // it's not same instance so
        // check if it's not null and is same value
        return other is not null && Value.Equals(other.Value);
    }

    /// <summary>
    /// Compares this instance to a specified <see cref="EnumClass{TEnum, TValue}"/> and returns an indication of their relative values.
    /// </summary>
    /// <param name="other">An <see cref="EnumClass{TEnum, TValue}"/> value to compare to this instance.</param>
    /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual int CompareTo(EnumClass<TEnum, TValue>? other) => other is { } enumeration ? Value.CompareTo(enumeration.Value) : 1;

    public int CompareTo(object? obj) => obj is EnumClass<TEnum, TValue> other ? Value.CompareTo(other.Value) : 1;

    public TypeCode GetTypeCode() => Convert.GetTypeCode(Value);

    public bool ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Value, provider);

    public byte ToByte(IFormatProvider? provider) => Convert.ToByte(Value, provider);

    public char ToChar(IFormatProvider? provider) => Convert.ToChar(Value, provider);

    public DateTime ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Value, provider);

    public decimal ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(Value, provider);

    public double ToDouble(IFormatProvider? provider) => Convert.ToDouble(Value, provider);

    public short ToInt16(IFormatProvider? provider) => Convert.ToInt16(Value, provider);

    public int ToInt32(IFormatProvider? provider) => Convert.ToInt32(Value, provider);

    public long ToInt64(IFormatProvider? provider) => Convert.ToInt64(Value, provider);

    public sbyte ToSByte(IFormatProvider? provider) => Convert.ToSByte(Value, provider);

    public float ToSingle(IFormatProvider? provider) => Convert.ToSingle(Value, provider);

    public object ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(Value, conversionType, provider);

    public ushort ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Value, provider);

    public uint ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Value, provider);

    public ulong ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Value, provider);

    public string ToString(IFormatProvider? provider) => Convert.ToString(Value, provider).OrEmpty();
}
