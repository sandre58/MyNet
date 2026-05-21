// -----------------------------------------------------------------------
// <copyright file="ComparableExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Utilities.Comparers;
using MyNet.Utilities.Intervals;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Extension methods to compare values using custom comparison operators.
/// </summary>
public static class ComparableExtensions
{
    extension(IComparable? x)
    {
        /// <summary>
        /// Compares two IComparable values using the supplied operator.
        /// </summary>
        public bool Compare(IComparable? y, ComparableOperator sign)
        {
            if (x == null || y == null)
                return false;

            var compare = x.CompareTo(y);

            return sign switch
            {
                ComparableOperator.EqualsTo => compare == 0,
                ComparableOperator.NotEqualsTo => compare != 0,
                ComparableOperator.LessThan => compare < 0,
                ComparableOperator.GreaterThan => compare > 0,
                ComparableOperator.LessEqualThan => compare <= 0,
                ComparableOperator.GreaterEqualThan => compare >= 0,
                _ => throw new ArgumentException(null, nameof(sign))
            };
        }

        /// <summary>
        /// Compares a value against a range using a complex operator.
        /// </summary>
        public bool Compare(IComparable? from, IComparable? to, ComplexComparableOperator sign)
        {
            if (x == null || from == null || to == null)
                return false;

            var compareFrom = x.CompareTo(from);
            var compareTo = x.CompareTo(to);

            var result = compareFrom >= 0 && compareTo <= 0;

            return sign switch
            {
                ComplexComparableOperator.IsBetween => result,
                ComplexComparableOperator.IsNotBetween => !result,
                ComplexComparableOperator.EqualsTo => compareFrom == 0,
                ComplexComparableOperator.NotEqualsTo => compareFrom != 0,
                ComplexComparableOperator.LessThan => compareTo < 0,
                ComplexComparableOperator.GreaterThan => compareFrom > 0,
                ComplexComparableOperator.LessEqualThan => compareTo <= 0,
                ComplexComparableOperator.GreaterEqualThan => compareFrom >= 0,
                _ => throw new ArgumentException(null, nameof(sign))
            };
        }
    }

    /// <summary>
    /// Compares a generic comparable against a range using a complex operator.
    /// </summary>
    public static bool Compare<T>(this IComparable<T> x, T? from, T? to, ComplexComparableOperator sign)
        where T : struct, IComparable<T>
    {
        var compareFrom = x.CompareNullableTo(from);
        var compareTo = x.CompareNullableTo(to);

        var result = compareFrom >= 0 && compareTo <= 0;

        return sign switch
        {
            ComplexComparableOperator.IsBetween => result,
            ComplexComparableOperator.IsNotBetween => !result,
            ComplexComparableOperator.EqualsTo => compareFrom == 0,
            ComplexComparableOperator.NotEqualsTo => compareFrom != 0,
            ComplexComparableOperator.LessThan => compareTo < 0,
            ComplexComparableOperator.GreaterThan => compareFrom > 0,
            ComplexComparableOperator.LessEqualThan => compareTo <= 0,
            ComplexComparableOperator.GreaterEqualThan => compareFrom >= 0,
            _ => throw new ArgumentException(null, nameof(sign))
        };
    }

    extension<T>(IComparable<T>? value)
        where T : struct, IComparable<T>
    {
        /// <summary>
        /// Determines if the current value is between two other values, handling nulls safely. The range is exclusive, meaning it returns <c>true</c> if the current value is strictly greater than the lower bound and strictly less than the upper bound. If either bound is null, the method will return <c>false</c> to indicate that the comparison cannot be performed.
        /// </summary>
        /// <param name="from">The lower bound value.</param>
        /// <param name="to">The upper bound value.</param>
        /// <returns><c>true</c> if the current value is between the specified values; otherwise, <c>false</c>.</returns>
        public bool IsBetween(T? from, T? to) => value is T t && new Interval<T>(from, to, false).Contains(t);

        /// <summary>
        /// Returns the maximum of two IComparable values.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>The maximum of the two values.</returns>
        public T Max(T other) => value is T t && t.CompareTo(other) > 0 ? t : other;

        /// <summary>
        /// Returns the minimum of two IComparable values.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>The minimum of the two values.</returns>
        public T Min(T other) => value is T t && t.CompareTo(other) < 0 ? t : other;

        /// <summary>
        /// Returns a MinMax struct containing the minimum and maximum of the current value and another value. The method first checks if the current value is of type T and then uses the CompareTo method to compare it with the other value. If the current value is greater than or equal to the other value, it returns a MinMax struct with the other value as the minimum and the current value as the maximum; otherwise, it returns a MinMax struct with the current value as the minimum and the other value as the maximum. If the current value is null or not of type T, it throws an InvalidCastException.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>A MinMax struct containing the minimum and maximum values.</returns>
        /// <exception cref="InvalidCastException">Thrown if the current value is null or not of type T.</exception>
        public MinMax<T> MinMax(T other) => value is T t ? (t.CompareTo(other) >= 0 ? new(other, t) : new(t, other)) : throw new InvalidCastException();

        /// <summary>
        /// Returns a MinMax struct containing the minimum and maximum of the current value and another value. The method first checks if the current value is of type T and then uses the CompareTo method to compare it with the other value. If the current value is greater than or equal to the other value, it returns a MinMax struct with the other value as the minimum and the current value as the maximum; otherwise, it returns a MinMax struct with the current value as the minimum and the other value as the maximum. If the current value is null or not of type T, it throws an InvalidCastException.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns>A MinMax struct containing the minimum and maximum values.</returns>
        /// <exception cref="InvalidCastException">Thrown if the current value is null or not of type T.</exception>
        public OptionalMinMax<T> MinMax(T? other) => new NullableComparer<T>().Compare(value, other) >= 0 ? new(other, value as T?) : new(value as T?, other);

        /// <summary>
        ///     Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="min"> The minimum value. </param>
        /// <param name="max"> The maximum value. </param>
        public T SafeClamp(T min, T max)
        {
            var minMax = min.MinMax(max);

            return value is not T t
                ? throw new InvalidCastException()
                : t.CompareTo(minMax.Min) < 0 ? minMax.Min : t.CompareTo(minMax.Max) > 0 ? minMax.Max : t;
        }

        /// <summary>
        /// Safely compares a nullable comparable to a value.
        /// </summary>
        public int CompareNullableTo(T other) => new NullableComparer<T>().Compare(value, other);

        /// <summary>
        /// Safely compares a nullable comparable to a nullable value.
        /// </summary>
        public int CompareNullableTo(T? other) => new NullableComparer<T>().Compare(value, other);

        /// <summary>
        /// Determines if the current value is in a range defined by two other values, handling nulls safely. The range is inclusive, meaning it returns <c>true</c> if the current value is greater than or equal to the lower bound and less than or equal to the upper bound. If either bound is null, the method will return <c>false</c> to indicate that the comparison cannot be performed.
        /// </summary>
        /// <param name="from">The lower bound value.</param>
        /// <param name="to">The upper bound value.</param>
        /// <returns><c>true</c> if the current value is in the specified range; otherwise, <c>false</c>.</returns>
        public bool InRange(T? from, T? to) => value is T t && new Interval<T>(from, to).Contains(t);
    }
}

/// <summary>
/// Represents a minimum and maximum value of a comparable type. This struct is immutable and provides a convenient way to store and compare a range of values. The Min and Max properties hold the minimum and maximum values, respectively, and the struct can be used in various comparison operations or to define ranges for validation purposes.
/// </summary>
/// <param name="Min">The minimum value.</param>
/// <param name="Max">The maximum value.</param>
/// <typeparam name="T">The type of the comparable values.</typeparam>
public readonly record struct MinMax<T>(T Min, T Max)
    where T : struct, IComparable<T>;

/// <summary>
/// Represents an optional minimum and maximum value of a comparable type. This struct is immutable and provides a convenient way to store and compare a range of values where either the minimum or maximum (or both) can be null, indicating that the range is unbounded on that side. The Min and Max properties hold the minimum and maximum values, respectively, and the struct can be used in various comparison operations or to define ranges for validation purposes, allowing for flexible handling of cases where one or both bounds may not be defined.
/// </summary>
/// <param name="Min">The minimum value, or null if unbounded.</param>
/// <param name="Max">The maximum value, or null if unbounded.</param>
/// <typeparam name="T">The type of the comparable values.</typeparam>
public readonly record struct OptionalMinMax<T>(T? Min, T? Max)
    where T : struct, IComparable<T>;
