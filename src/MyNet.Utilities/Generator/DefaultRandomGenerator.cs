// -----------------------------------------------------------------------
// <copyright file="DefaultRandomGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyNet.Utilities.Helpers;

namespace MyNet.Utilities.Generator;

/// <summary>
/// Current implementation of <see cref="IRandomGenerator"/>.
/// </summary>
public sealed class DefaultRandomGenerator(IRandomSource source) : IRandomGenerator
{
    private readonly IRandomSource _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <inheritdoc />
    public bool Bool() => Weighted(0.5f);

    /// <inheritdoc />
    public bool Weighted(float probability)
        => probability is < 0.0f or > 1.0f
            ? throw new ArgumentOutOfRangeException(nameof(probability), probability, "Probability must be between 0.0 and 1.0.")
            : _source.NextDouble() < probability;

    /// <inheritdoc />
    public byte[] Bytes(int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");

        var buffer = new byte[count];
        _source.NextBytes(buffer);
        return buffer;
    }

    /// <inheritdoc />
    public T Item<T>(IReadOnlyCollection<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        var finalList = list as IReadOnlyList<T> ?? [.. list];
        return finalList.Count == 0 ? throw new ArgumentException("The list cannot be empty.", nameof(list)) : finalList[_source.NextInt32(0, list.Count)];
    }

    /// <inheritdoc />
    public IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var buffer = source.ToList();

        for (var i = buffer.Count - 1; i > 0; i--)
        {
            var j = _source.NextInt32(0, i + 1);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }

        return buffer;
    }

    /// <inheritdoc />
    public IReadOnlyList<T> Subset<T>(IReadOnlyCollection<T> list, int count)
    {
        ArgumentNullException.ThrowIfNull(list);

        return count < 0 || count > list.Count
            ? throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be between 0 and the list size ({list.Count}).")
            : [.. Shuffle(list).Take(count)];
    }

    /// <inheritdoc />
    public DateTime Date(DateTime min, DateTime max)
    {
        if (min > max)
            throw new ArgumentOutOfRangeException(nameof(min), "min must be less than or equal to max.");

        var range = max.Ticks - min.Ticks;
        if (range == 0)
            return min;

        var offset = (long)(_source.NextDouble() * range);
        return new(min.Ticks + offset, min.Kind);
    }

    /// <inheritdoc />
    public T Enum<T>(params T[] exclude)
        where T : struct, Enum
    {
        var values = System.Enum.GetValues<T>();

        if (exclude is { Length: > 0 })
            values = [.. values.Where(x => !exclude.Contains(x))];

        return values.Length == 0 ? throw new ArgumentException($"All values of enum '{typeof(T).Name}' have been excluded.", nameof(exclude)) : values[_source.NextInt32(0, values.Length)];
    }

    /// <inheritdoc />
    public int Int(int minInclusive = int.MinValue, int maxExclusive = int.MaxValue)
    {
        ValidateExclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return _source.NextInt32(minInclusive, maxExclusive);
    }

    /// <inheritdoc />
    public int Even(int minInclusive = int.MinValue, int maxExclusive = int.MaxValue)
    {
        ValidateExclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));

        var start = (minInclusive & 1) == 0 ? minInclusive : minInclusive + 1L;
        var end = (long)maxExclusive - 1;

        if (start > end)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), "The range does not contain any even number.");

        var count = checked((int)(((end - start) / 2) + 1));
        var offset = _source.NextInt32(0, count);

        return checked((int)(start + (2L * offset)));
    }

    /// <inheritdoc />
    public int Odd(int minInclusive = int.MinValue, int maxExclusive = int.MaxValue)
    {
        ValidateExclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));

        var start = (minInclusive & 1) == 1 ? minInclusive : minInclusive + 1L;
        var end = (long)maxExclusive - 1;

        if (start > end)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), "The range does not contain any odd number.");

        var count = checked((int)(((end - start) / 2) + 1));
        var offset = _source.NextInt32(0, count);

        return checked((int)(start + (2L * offset)));
    }

    /// <inheritdoc />
    public double Double(double minInclusive = int.MinValue, double maxExclusive = int.MaxValue)
    {
        ValidateExclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return (_source.NextDouble() * (maxExclusive - minInclusive)) + minInclusive;
    }

    /// <inheritdoc />
    public decimal Decimal(decimal minInclusive = 0.0m, decimal maxExclusive = 1.0m)
    {
        ValidateExclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return (Convert.ToDecimal(_source.NextDouble()) * (maxExclusive - minInclusive)) + minInclusive;
    }

    /// <inheritdoc />
    public float Float(float minInclusive = 0.0f, float maxExclusive = 1.0f)
    {
        ValidateExclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return (Convert.ToSingle(_source.NextDouble()) * (maxExclusive - minInclusive)) + minInclusive;
    }

    /// <inheritdoc />
    public byte Byte(byte minInclusive = byte.MinValue, byte maxExclusive = byte.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return Convert.ToByte(_source.NextInt32(minInclusive, maxExclusive + 1));
    }

    /// <inheritdoc />
    public sbyte SByte(sbyte minInclusive = sbyte.MinValue, sbyte maxExclusive = sbyte.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return Convert.ToSByte(_source.NextInt32(minInclusive, maxExclusive + 1));
    }

    /// <inheritdoc />
    public uint UInt(uint minInclusive = uint.MinValue, uint maxExclusive = uint.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        if (minInclusive == maxExclusive)
            return minInclusive;

        var value = (_source.NextDouble() * ((double)maxExclusive - minInclusive + 1)) + minInclusive;
        return (uint)Math.Floor(value);
    }

    /// <inheritdoc />
    public ulong ULong(ulong minInclusive = ulong.MinValue, ulong maxExclusive = ulong.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        if (minInclusive == maxExclusive)
            return minInclusive;

        var value = (_source.NextDouble() * ((double)maxExclusive - minInclusive + 1)) + minInclusive;
        return (ulong)Math.Floor(value);
    }

    /// <inheritdoc />
    public long Long(long minInclusive = long.MinValue, long maxExclusive = long.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        if (minInclusive == maxExclusive)
            return minInclusive;

        var range = ((decimal)maxExclusive - minInclusive) + 1;
        var value = ((decimal)_source.NextDouble() * range) + minInclusive;
        return decimal.ToInt64(decimal.Floor(value));
    }

    /// <inheritdoc />
    public short Short(short minInclusive = short.MinValue, short maxExclusive = short.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return minInclusive == maxExclusive ? minInclusive : Convert.ToInt16(_source.NextInt32(minInclusive, maxExclusive + 1));
    }

    /// <inheritdoc />
    public ushort UShort(ushort minInclusive = ushort.MinValue, ushort maxExclusive = ushort.MaxValue)
    {
        ValidateInclusiveRange(minInclusive, maxExclusive, nameof(minInclusive), nameof(maxExclusive));
        return minInclusive == maxExclusive ? minInclusive : Convert.ToUInt16(_source.NextInt32(minInclusive, maxExclusive + 1));
    }

    /// <inheritdoc />
    public string String(int length, ReadOnlySpan<char> chars = default)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Length cannot be negative.");

        if (chars.IsEmpty)
            chars = CharHelper.Alphabet;

        var buffer = new char[length];

        for (var i = 0; i < length; i++)
            buffer[i] = chars[_source.NextInt32(0, chars.Length)];

        return new(buffer);
    }

    /// <inheritdoc />
    public char Char(char minInclusive = char.MinValue, char maxExclusive = char.MaxValue) => minInclusive >= maxExclusive ? throw new ArgumentOutOfRangeException(nameof(minInclusive), "minInclusive must be less than maxExclusive.") : Convert.ToChar(_source.NextInt32(minInclusive, maxExclusive));

    /// <inheritdoc />
    public char Letter(bool uppercase = true) => (char)((uppercase ? 'A' : 'a') + _source.NextInt32(0, 26));

    /// <inheritdoc />
    public char Digit() => (char)('0' + _source.NextInt32(0, 10));

    /// <inheritdoc />
    public char[] Chars(char minInclusive = char.MinValue, char maxExclusive = char.MaxValue, int count = 5)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");

        var arr = new char[count];
        for (var i = 0; i < count; i++)
            arr[i] = Char(minInclusive, maxExclusive);

        return arr;
    }

    /// <summary>
    /// Validates that min is less than max. Throws an ArgumentOutOfRangeException if the condition is not met.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="minName">The name of the minimum value parameter.</param>
    /// <param name="maxName">The name of the maximum value parameter.</param>
    /// <typeparam name="T">The type of the values being compared.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if min is not less than max.</exception>
    private static void ValidateExclusiveRange<T>(T min, T max, string minName, string maxName)
        where T : IComparable<T>
    {
        if (min.CompareTo(max) >= 0)
            throw new ArgumentOutOfRangeException(minName, $"{minName} must be less than {maxName}.");
    }

    /// <summary>
    /// Validates that min is less than or equal to max. Throws an ArgumentOutOfRangeException if the condition is not met.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="minName">The name of the minimum value parameter.</param>
    /// <param name="maxName">The name of the maximum value parameter.</param>
    /// <typeparam name="T">The type of the values being compared.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if min is not less than or equal to max.</exception>
    private static void ValidateInclusiveRange<T>(T min, T max, string minName, string maxName)
        where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(minName, $"{minName} must be less than or equal to {maxName}.");
    }
}
