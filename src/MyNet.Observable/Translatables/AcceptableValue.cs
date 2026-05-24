// -----------------------------------------------------------------------
// <copyright file="AcceptableValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Observable.Attributes;
using MyNet.Observable.Resources;
using MyNet.Utilities;
using MyNet.Utilities.Sequences;

namespace MyNet.Observable.Translatables;

public class AcceptableValue<T> : EditableObject, IAcceptableValue<T>
    where T : struct, IComparable<T>, IComparable, IConvertible, IEquatable<T>
{
    private AcceptableValueRange<T> _acceptableRange;

    public T? Value { get; set; }

    public T? DefaultValue { get; }

    public bool HasValue => Value.HasValue;

    [ValidateProperty(nameof(Value))]
    public T? Min
    {
        get => _acceptableRange.Min;
        set
        {
            if (Equals(value, Min)) return;

            _acceptableRange = new(value, Max);
            OnPropertyChanged(nameof(Min));
        }
    }

    [ValidateProperty(nameof(Value))]
    public T? Max
    {
        get => _acceptableRange.Max;
        set
        {
            if (Equals(value, Max)) return;

            _acceptableRange = new(Min, value);
            OnPropertyChanged(nameof(Max));
        }
    }

    T IProvideValue<T>.Value => Value.GetValueOrDefault();

    T IResetable<T>.DefaultValue => DefaultValue.GetValueOrDefault();

    public AcceptableValue()
        : this(new(null, null)) { }

    public AcceptableValue(T? min, T? max)
        : this(new(min, max)) { }

    public AcceptableValue(AcceptableValueRange<T> acceptableValueRange, T? defaultValue = null)
    {
        _acceptableRange = acceptableValueRange;
        DefaultValue = defaultValue;

        ValidationRules.Add<IAcceptableValue<T>, T?>(_ => Value,
            () => Min.HasValue && Max.HasValue
            ? ValidationResources.FieldMustBeBetween.FormatWith(CultureInfo.CurrentCulture, nameof(Value).Translate(), Min.Value, Max.Value)
            : Min.HasValue
                ? ValidationResources.FieldMustBeGreaterThanOrEqualTo.FormatWith(CultureInfo.CurrentCulture, nameof(Value).Translate(), Min.Value)
                : Max.HasValue ? ValidationResources.FieldMustBeLessThanOrEqualTo.FormatWith(CultureInfo.CurrentCulture, nameof(Value).Translate(), Max.Value) : string.Empty,
            _acceptableRange.IsValid);
        DefaultValue = defaultValue;
    }

    public void Reset() => Value = DefaultValue;

    public override string? ToString() => Value?.ToString();

    public TypeCode GetTypeCode() => Value?.GetTypeCode() ?? TypeCode.Empty;

    public T? ToNullable() => Value;

    public bool ToBoolean(IFormatProvider? provider) => Value?.ToBoolean(provider) ?? false;

    public byte ToByte(IFormatProvider? provider) => Value?.ToByte(provider) ?? 0;

    public char ToChar(IFormatProvider? provider) => Value?.ToChar(provider) ?? '\0';

    public DateTime ToDateTime(IFormatProvider? provider) => Value?.ToDateTime(provider) ?? default;

    public decimal ToDecimal(IFormatProvider? provider) => Value?.ToDecimal(provider) ?? 0;

    public double ToDouble(IFormatProvider? provider) => Value?.ToDouble(provider) ?? 0;

    public short ToInt16(IFormatProvider? provider) => Value?.ToInt16(provider) ?? 0;

    public int ToInt32(IFormatProvider? provider) => Value?.ToInt32(provider) ?? 0;

    public long ToInt64(IFormatProvider? provider) => Value?.ToInt64(provider) ?? 0;

    public sbyte ToSByte(IFormatProvider? provider) => Value?.ToSByte(provider) ?? 0;

    public float ToSingle(IFormatProvider? provider) => Value?.ToSingle(provider) ?? 0;

    public string ToString(IFormatProvider? provider) => Value?.ToString(provider) ?? string.Empty;

    public object ToType(Type conversionType, IFormatProvider? provider) => Value?.ToBoolean(provider) ?? false;

    public ushort ToUInt16(IFormatProvider? provider) => Value?.ToUInt16(provider) ?? 0;

    public uint ToUInt32(IFormatProvider? provider) => Value?.ToUInt32(provider) ?? 0;

    public ulong ToUInt64(IFormatProvider? provider) => Value?.ToUInt64(provider) ?? 0;

    public override bool Equals(object? obj) => (obj is IAcceptableValue<T> val && Value.Equals(val.Value)) || Value.Equals(obj);

    public virtual bool Equals(T other) => Value.Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public int CompareTo(T? other) => Value.CompareTo(other);

    public int CompareTo(T other) => Value.CompareTo(other);

    public int CompareTo(object? obj)
        => obj switch
        {
            T obj1 => Value.CompareTo(obj1),
            IAcceptableValue<T> obj2 => Value.CompareTo(obj2.Value),
            _ => Value?.CompareTo(obj) ?? 1
        };

    public int CompareTo(IAcceptableValue<T>? other) => CompareTo(other?.Value);

    public bool Equals(T x, T y) => x.Equals(y);

    public int GetHashCode(T obj) => obj.GetHashCode();

    public bool Equals(IAcceptableValue<T>? x, IAcceptableValue<T>? y) => x?.Equals(y) == true;

    public int GetHashCode(IAcceptableValue<T> obj) => obj.GetHashCode();

    public static implicit operator T?(AcceptableValue<T> value) => value.Value;

    public static bool operator ==(AcceptableValue<T> left, AcceptableValue<T> right) => left.Value.Equals(right.Value);

    public static bool operator !=(AcceptableValue<T> left, AcceptableValue<T> right) => !(left == right);

    public static bool operator <(AcceptableValue<T> left, AcceptableValue<T> right) => left.Value.CompareTo(right.Value) < 0;

    public static bool operator <=(AcceptableValue<T> left, AcceptableValue<T> right) => left.Value.CompareTo(right.Value) <= 0;

    public static bool operator >(AcceptableValue<T> left, AcceptableValue<T> right) => left.Value.CompareTo(right.Value) > 0;

    public static bool operator >=(AcceptableValue<T> left, AcceptableValue<T> right) => left.Value.CompareTo(right.Value) >= 0;
}
