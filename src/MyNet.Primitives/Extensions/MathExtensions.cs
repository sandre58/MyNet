// -----------------------------------------------------------------------
// <copyright file="MathExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Primitives;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MathExtensions
{
    /// <summary>
    /// The smallest positive double such that 1.0 + DblEpsilon != 1.0.
    /// </summary>
    public const double DblEpsilon = 2.2204460492503131e-016;

    /// <summary>
    /// The smallest positive float such that 1.0f + FloatEpsilon != 1.0f.
    /// </summary>
    public const float FloatEpsilon = 1.1920929E-07f;

    extension(int value)
    {
        /// <summary>
        /// Returns whether the integer is even.
        /// </summary>
        /// <returns>True if the integer is even, otherwise false.</returns>
        public bool IsEven() => value % 2 == 0;

        /// <summary>
        /// Returns whether the integer is odd.
        /// </summary>
        /// <returns>True if the integer is odd, otherwise false.</returns>
        public bool IsOdd() => value % 2 != 0;

        /// <summary>
        /// Iterates a specified action a given number of times, passing the current iteration index to the action.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        public void Repeat(Action<int> action)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentNullException.ThrowIfNull(action);

            for (var i = 0; i < value; i++)
                action(i);
        }

        /// <summary>
        /// Generates a sequence of integers starting from a specified minimum value, incrementing by a specified step, and ending at the value of the integer on which the method is called.
        /// </summary>
        /// <param name="min">The starting value of the sequence.</param>
        /// <param name="step">The increment value for each step in the sequence.</param>
        /// <returns>An enumerable sequence of integers.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the step is zero.</exception>
        public IEnumerable<int> Range(int min = 0, int step = 1)
        {
            ArgumentOutOfRangeException.ThrowIfZero(step);

            if (step > 0)
            {
                for (var i = min; i <= value; i += step)
                    yield return i;
            }
            else
            {
                for (var i = min; i >= value; i += step)
                    yield return i;
            }
        }
    }

    extension(object? value)
    {
        /// <summary>
        /// Extracts a double value from an object, returning NaN if the object is not a double or if the value is infinity.
        /// </summary>
        /// <returns>The extracted double value, or NaN if the extraction fails.</returns>
        public double ExtractDouble()
            => value switch
            {
                null => double.NaN,
                double d when !double.IsInfinity(d) => d,
                float f when !float.IsInfinity(f) => f,
                IConvertible c => Convert.ToDouble(c, CultureInfo.InvariantCulture),
                _ => double.NaN
            };
    }

    extension(IEnumerable<double> vals)
    {
        /// <summary>
        /// Returns whether any of the double values in the enumerable are NaN.
        /// </summary>
        /// <returns>True if any of the values are NaN, otherwise false.</returns>
        public bool AnyNan() => vals.Any(double.IsNaN);

        /// <summary>
        /// Returns whether any of the double values in the enumerable are infinity (either positive or negative).
        /// </summary>
        /// <returns>True if any of the values are infinity, otherwise false.</returns>
        public bool AnyInfinity() => vals.Any(double.IsInfinity);
    }

    extension(double value)
    {
        /// <summary>
        /// Returns whether two doubles are "close".
        /// </summary>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "NaN and infinity are handled separately, and for other values we want to check if they are close enough to be considered equal.")]
        public bool IsCloseTo(double value2)
        {
            if (double.IsNaN(value) || double.IsNaN(value2))
                return false;

            if (value == value2)
                return true;

            if (double.IsInfinity(value) || double.IsInfinity(value2))
                return value == value2;

            var eps = DblEpsilon * Math.Max(1.0, Math.Max(Math.Abs(value), Math.Abs(value2)));
            var delta = value - value2;

            return Math.Abs(delta) <= eps;
        }

        /// <summary>
        /// IsGreaterThan - Returns whether the first double is strictly greater than the second double.
        /// </summary>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns> True if the first double is strictly greater than the second double, otherwise false. </returns>
        public bool IsGreaterThan(double value2) => value > value2 + DblEpsilon;

        /// <summary>
        /// IsLessThan - Returns whether the first double is strictly less than the second double.
        /// </summary>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns> True if the first double is strictly less than the second double, otherwise false. </returns>
        public bool IsLessThan(double value2) => value < value2 - DblEpsilon;

        /// <summary>
        /// IsLessThanOrClose - Returns whether the first double is less than or close to
        /// the second double.  That is, whether the first is strictly less than or within
        /// epsilon of the other number.
        /// </summary>
        /// <param name="value2"> The second double to compare. </param>
        public bool IsLessThanOrClose(double value2) => value <= value2 || value.IsCloseTo(value2);

        /// <summary>
        /// IsGreaterThanOrClose - Returns whether the first double is greater than or close to
        /// the second double.  That is, whether the first is strictly greater than or within
        /// epsilon of the other number.
        /// </summary>
        /// <param name="value2"> The second double to compare. </param>
        public bool IsGreaterThanOrClose(double value2) => value >= value2 || value.IsCloseTo(value2);

        /// <summary>
        /// IsOne - Returns whether the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        public bool IsOne() => Math.Abs(value - 1.0) < 10.0 * DblEpsilon;

        /// <summary>
        /// IsZero - Returns whether the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        public bool IsZero() => Math.Abs(value) < 10.0 * DblEpsilon;

        /// <summary>
        /// Generates a sequence of doubles starting from the value of the double on which the method is called, incrementing by a specified step, and ending at a specified maximum value.
        /// </summary>
        /// <param name="max">The maximum value of the sequence.</param>
        /// <param name="step">The increment between each value in the sequence.</param>
        /// <returns>An enumerable sequence of doubles.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the step is zero.</exception>
        public IEnumerable<double> Range(double max, double step = 1)
        {
            ArgumentOutOfRangeException.ThrowIfZero(step);

            if (step > 0)
            {
                for (var i = value; i <= max + DblEpsilon; i += step)
                    yield return i;
            }
            else
            {
                for (var i = value; i >= max - DblEpsilon; i += step)
                    yield return i;
            }
        }
    }

    extension(double? value)
    {
        /// <summary>
        /// Returns whether two nullable doubles are "close".  If both values are null, they are considered close.
        /// </summary>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>True if the values are considered close, otherwise false.</returns>
        public bool IsCloseTo(double? value2) => (!value.HasValue && !value2.HasValue) || (value.HasValue && value2.HasValue && value.Value.IsCloseTo(value2.Value));
    }

    extension(float value)
    {
        /// <summary>
        /// IsGreaterThan - Returns whether the first float is strictly greater than the second float.
        /// </summary>
        /// <param name="value2"> The second float to compare. </param>
        /// <returns> True if the first float is strictly greater than the second float, otherwise false. </returns>
        public bool IsGreaterThan(float value2) => value > value2 + FloatEpsilon;

        /// <summary>
        /// IsLessThan - Returns whether the first float is strictly less than the second float.
        /// </summary>
        /// <param name="value2"> The second float to compare. </param>
        /// <returns> True if the first float is strictly less than the second float, otherwise false. </returns>
        public bool IsLessThan(float value2) => value < value2 - FloatEpsilon;

        /// <summary>
        /// IsLessThanOrClose - Returns whether the first float is less than or close to
        /// the second float.  That is, whether the first is strictly less than or within
        /// epsilon of the other number.
        /// </summary>
        /// <param name="value2"> The second float to compare. </param>
        public bool IsLessThanOrClose(float value2) => value <= value2 || value.IsCloseTo(value2);

        /// <summary>
        /// IsGreaterThanOrClose - Returns whether the first float is greater than or close to
        /// the second float.  That is, whether the first is strictly greater than or within
        /// epsilon of the other number.
        /// </summary>
        /// <param name="value2"> The second float to compare. </param>
        public bool IsGreaterThanOrClose(float value2) => value >= value2 || value.IsCloseTo(value2);

        /// <summary>
        /// IsOne - Returns whether the float is "close" to 1.  Same as AreClose(float, 1),
        /// but this is faster.
        /// </summary>
        public bool IsOne() => Math.Abs(value - 1.0f) < 10.0f * FloatEpsilon;

        /// <summary>
        /// IsZero - Returns whether the float is "close" to 0.  Same as AreClose(float, 0),
        /// but this is faster.
        /// </summary>
        public bool IsZero() => Math.Abs(value) < 10.0f * FloatEpsilon;

        /// <summary>
        /// Returns whether two float are "close".
        /// </summary>
        /// <param name="value2"> The second float to compare. </param>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "NaN and infinity are handled separately, and for other values we want to check if they are close enough to be considered equal.")]
        public bool IsCloseTo(float value2)
        {
            if (float.IsNaN(value) || float.IsNaN(value2))
                return false;

            if (float.IsInfinity(value) || float.IsInfinity(value2))
                return value == value2;

            var eps = (Math.Abs(value) + Math.Abs(value2) + 10.0) * DblEpsilon;
            var delta = value - value2;

            return -eps < delta && delta < eps;
        }
    }

    extension(float? value)
    {
        /// <summary>
        /// Returns whether two nullable floats are "close".  If both values are null, they are considered close.
        /// </summary>
        /// <param name="value2">The second float to compare.</param>
        /// <returns>True if the values are considered close, otherwise false.</returns>
        public bool IsCloseTo(float? value2) => (!value.HasValue && !value2.HasValue) || (value.HasValue && value2.HasValue && value.Value.IsCloseTo(value2.Value));
    }
}
