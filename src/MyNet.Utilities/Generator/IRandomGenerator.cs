// -----------------------------------------------------------------------
// <copyright file="IRandomGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Generator;

/// <summary>
/// Injectable service exposing all random generation helpers.
/// </summary>
public interface IRandomGenerator
{
    /// <summary>
    /// Picks a random boolean value.
    /// </summary>
    /// <returns>A random boolean value.</returns>
    bool Bool();

    /// <summary>
    /// Picks a random boolean value based on the specified probability.
    /// </summary>
    /// <param name="probability">The probability of returning true.</param>
    /// <returns>A random boolean value based on the specified probability.</returns>
    bool Weighted(float probability);

    /// <summary>
    /// Generates an array of random bytes.
    /// </summary>
    /// <param name="count">The number of bytes to generate.</param>
    /// <returns>An array of random bytes.</returns>
    byte[] Bytes(int count);

    /// <summary>
    /// Selects a random item from the provided list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list from which to select a random item.</param>
    /// <returns>A random item from the list.</returns>
    T Item<T>(IReadOnlyCollection<T> list);

    /// <summary>
    /// Shuffles the elements of the provided collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to shuffle.</param>
    /// <returns>A new collection with the elements shuffled.</returns>
    IEnumerable<T> Shuffle<T>(IEnumerable<T> source);

    /// <summary>
    /// Creates a random subset of the provided list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list from which to create a random subset.</param>
    /// <param name="count">The number of items to include in the subset.</param>
    /// <returns>A random subset of the list.</returns>
    IReadOnlyList<T> Subset<T>(IReadOnlyCollection<T> list, int count);

    /// <summary>
    /// Generates a random date within the specified range.
    /// </summary>
    /// <param name="min">The minimum date.</param>
    /// <param name="max">The maximum date.</param>
    /// <returns>A random date within the specified range.</returns>
    DateTime Date(DateTime min, DateTime max);

    /// <summary>
    /// Picks a random value from the specified enum type, excluding the specified values.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="exclude">The values to exclude.</param>
    /// <returns>A random value from the enum type, excluding the specified values.</returns>
    T Enum<T>(params T[] exclude)
        where T : struct, Enum;

    /// <summary>
    /// Generates a random integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random integer within the specified range.</returns>
    int Int(int minInclusive = int.MinValue, int maxExclusive = int.MaxValue);

    /// <summary>
    /// Generates a random even integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random even integer within the specified range.</returns>
    int Even(int minInclusive = int.MinValue, int maxExclusive = int.MaxValue);

    /// <summary>
    /// Generates a random odd integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random odd integer within the specified range.</returns>
    int Odd(int minInclusive = int.MinValue, int maxExclusive = int.MaxValue);

    /// <summary>
    /// Generates a random double within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random double within the specified range.</returns>
    double Double(double minInclusive = int.MinValue, double maxExclusive = int.MaxValue);

    /// <summary>
    /// Generates a random decimal within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random decimal within the specified range.</returns>
    decimal Decimal(decimal minInclusive = 0.0m, decimal maxExclusive = 1.0m);

    /// <summary>
    /// Generates a random float within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random float within the specified range.</returns>
    float Float(float minInclusive = 0.0f, float maxExclusive = 1.0f);

    /// <summary>
    /// Generates a random byte within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random byte within the specified range.</returns>
    byte Byte(byte minInclusive = byte.MinValue, byte maxExclusive = byte.MaxValue);

    /// <summary>
    /// Generates a random sbyte within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random sbyte within the specified range.</returns>
    sbyte SByte(sbyte minInclusive = sbyte.MinValue, sbyte maxExclusive = sbyte.MaxValue);

    /// <summary>
    /// Generates a random unsigned integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random unsigned integer within the specified range.</returns>
    uint UInt(uint minInclusive = uint.MinValue, uint maxExclusive = uint.MaxValue);

    /// <summary>
    /// Generates a random unsigned long integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random unsigned long integer within the specified range.</returns>
    ulong ULong(ulong minInclusive = ulong.MinValue, ulong maxExclusive = ulong.MaxValue);

    /// <summary>
    /// Generates a random long integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random long integer within the specified range.</returns>
    long Long(long minInclusive = long.MinValue, long maxExclusive = long.MaxValue);

    /// <summary>
    /// Generates a random short integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random short integer within the specified range.</returns>
    short Short(short minInclusive = short.MinValue, short maxExclusive = short.MaxValue);

    /// <summary>
    /// Generates a random unsigned short integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random unsigned short integer within the specified range.</returns>
    ushort UShort(ushort minInclusive = ushort.MinValue, ushort maxExclusive = ushort.MaxValue);

    /// <summary>
    /// Generates a random string of the specified length using the specified characters.
    /// </summary>
    /// <param name="length">The length of the string.</param>
    /// <param name="chars">The characters to use.</param>
    /// <returns>A random string.</returns>
    string String(int length, ReadOnlySpan<char> chars = default);

    /// <summary>
    /// Generates a random character within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <returns>A random character within the specified range.</returns>
    char Char(char minInclusive = char.MinValue, char maxExclusive = char.MaxValue);

    /// <summary>
    /// Generates a random letter character, optionally uppercase. By default, it generates an uppercase letter.
    /// </summary>
    /// <param name="uppercase">If true, generates an uppercase letter; otherwise, generates a lowercase letter.</param>
    /// <returns>A random letter character.</returns>
    char Letter(bool uppercase = true);

    /// <summary>
    /// Generates a random digit character (0-9).
    /// </summary>
    /// <returns>A random digit character.</returns>
    char Digit();

    /// <summary>
    /// Generates an array of random characters within the specified range.
    /// </summary>
    /// <param name="minInclusive">The minimum value. The upper bound is inclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="maxExclusive">The maximum value. The upper bound is exclusive, following the same convention as <see cref="Random.Next(int, int)"/>.</param>
    /// <param name="count">The number of characters to generate.</param>
    /// <returns>An array of random characters within the specified range.</returns>
    char[] Chars(char minInclusive = char.MinValue, char maxExclusive = char.MaxValue, int count = 5);
}
