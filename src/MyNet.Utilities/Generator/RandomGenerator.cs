// -----------------------------------------------------------------------
// <copyright file="RandomGenerator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MyNet.Utilities.Geography;

#pragma warning disable CA5394
namespace MyNet.Utilities.Generator;

public static class RandomGenerator
{
    /// <summary>
    /// Set the random number generator manually with a seed to get reproducible results.
    /// </summary>
    public static readonly Random Seed = new();

    private static readonly Lock Locker = new();

    /// <summary>
    /// Get an int from min to max.
    /// </summary>
    /// <param name="min">Lower bound, inclusive.</param>
    /// <param name="max">Upper bound, inclusive. Only int.MaxValue is exclusive.</param>
    public static int Number(int min = 0, int max = 1)
    {
        // lock any seed access, for thread safety.
        lock (Locker)
        {
            // Clamp max value, Issue #30.
            max = max == int.MaxValue ? max : max + 1;
            var checkMin = Math.Min(min, max);
            var checkMax = Math.Max(min, max);
            return Seed.Next(checkMin, checkMax);
        }
    }

    /// <summary>
    /// Get an int from min to max.
    /// </summary>
    /// <param name="startDate">Lower bound, inclusive.</param>
    /// <param name="endDate">Upper bound, inclusive. Only int.MaxValue is exclusive.</param>
    public static DateTime Date(DateTime startDate, DateTime endDate)
    {
        // lock any seed access, for thread safety.
        lock (Locker)
        {
            var range = endDate.Ticks - startDate.Ticks;
            var randomTicks = startDate.Ticks + (long)(Seed.NextDouble() * range);
            return new DateTime(randomTicks, startDate.Kind);
        }
    }

    /// <summary>
    /// Get a random sequence of digits.
    /// </summary>
    /// <param name="count">How many.</param>
    /// <param name="minDigit">minimum digit, inclusive.</param>
    /// <param name="maxDigit">maximum digit, inclusive.</param>
    public static int[] Digits(int count, int minDigit = 0, int maxDigit = 9)
    {
        var digits = new int[count];
        for (var i = 0; i < count; i++)
        {
            digits[i] = Number(minDigit, maxDigit);
        }

        return digits;
    }

    /// <summary>
    /// Returns a random even number.
    /// </summary>
    /// <param name="min">Lower bound, inclusive.</param>
    /// <param name="max">Upper bound, inclusive.</param>
    public static int Even(int min = 0, int max = 1)
    {
        int result;
        do
        {
            result = Number(min, max);
        }
        while ((result & 1) == 1);
        return result;
    }

    /// <summary>
    /// Returns a random odd number.
    /// </summary>
    /// <param name="min">Lower bound, inclusive.</param>
    /// <param name="max">Upper bound, inclusive.</param>
    public static int Odd(int min = 0, int max = 1)
    {
        int result;
        do
        {
            result = Number(min, max);
        }
        while ((result & 1) == 0);
        return result;
    }

    /// <summary>
    /// Get a random double, between 0.0 and 1.0.
    /// </summary>
    /// <param name="min">Minimum, default 0.0.</param>
    /// <param name="max">Maximum, default 1.0.</param>
    public static double Double(double min = 0.0d, double max = 1.0d)
    {
        lock (Locker)
        {
            return min == 0.0d && max == 1.0d ? Seed.NextDouble() : (Seed.NextDouble() * (max - min)) + min;
        }
    }

    /// <summary>
    /// Get a random decimal, between 0.0 and 1.0.
    /// </summary>
    /// <param name="min">Minimum, default 0.0.</param>
    /// <param name="max">Maximum, default 1.0.</param>
    public static decimal Decimal(decimal min = 0.0m, decimal max = 1.0m) => (Convert.ToDecimal(Double()) * (max - min)) + min;

    /// <summary>
    /// Get a random float, between 0.0 and 1.0.
    /// </summary>
    /// <param name="min">Minimum, default 0.0.</param>
    /// <param name="max">Maximum, default 1.0.</param>
    public static float Float(float min = 0.0f, float max = 1.0f) => (Convert.ToSingle(Double()) * (max - min)) + min;

    /// <summary>
    /// Generate a random byte between 0 and 255.
    /// </summary>
    /// <param name="min">Min value, default 0.</param>
    /// <param name="max">Max value, default 255.</param>
    public static byte Byte(byte min = byte.MinValue, byte max = byte.MaxValue) => Convert.ToByte(Number(min, max));

    /// <summary>
    /// Get a random sequence of bytes.
    /// </summary>
    /// <param name="count">The size of the byte array.</param>
    public static byte[] Bytes(int count)
    {
        var arr = new byte[count];
        lock (Locker)
        {
            Seed.NextBytes(arr);
        }

        return arr;
    }

    /// <summary>
    /// Generate a random sbyte between -128 and 127.
    /// </summary>
    /// <param name="min">Min value, default -128.</param>
    /// <param name="max">Max value, default 127.</param>
    public static sbyte SByte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue) => Convert.ToSByte(Number(min, max));

    /// <summary>
    /// Generate a random int between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    public static int Int(int min = int.MinValue, int max = int.MaxValue) => Number(min, max);

    /// <summary>
    /// Generate a random uint between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    public static uint UInt(uint min = uint.MinValue, uint max = uint.MaxValue) => Convert.ToUInt32((Double() * (max - min)) + min);

    /// <summary>
    /// Generate a random ulong between -128 and 127.
    /// </summary>
    /// <param name="min">Min value, default -128.</param>
    /// <param name="max">Max value, default 127.</param>
    public static ulong ULong(ulong min = ulong.MinValue, ulong max = ulong.MaxValue) => Convert.ToUInt64((Double() * (max - min)) + min);

    /// <summary>
    /// Generate a random long between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    public static long Long(long min = long.MinValue, long max = long.MaxValue)
    {
        var range = (decimal)max - min; // use more bits?
        return Convert.ToInt64(((decimal)Double() * range) + min);
    }

    /// <summary>
    /// Generate a random short between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    public static short Short(short min = short.MinValue, short max = short.MaxValue) => Convert.ToInt16((Double() * (max - min)) + min);

    /// <summary>
    /// Generate a random ushort between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    public static ushort UShort(ushort min = ushort.MinValue, ushort max = ushort.MaxValue) => Convert.ToUInt16((Double() * (max - min)) + min);

    /// <summary>
    /// Generate a random char between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    public static char Char(char min = char.MinValue, char max = char.MaxValue) => Convert.ToChar(Number(min, max));

    /// <summary>
    /// Generate a random chars between MinValue and MaxValue.
    /// </summary>
    /// <param name="min">Min value, default MinValue.</param>
    /// <param name="max">Max value, default MaxValue.</param>
    /// <param name="count">The length of chars to return.</param>
    public static char[] Chars(char min = char.MinValue, char max = char.MaxValue, int count = 5)
    {
        var arr = new char[count];
        for (var i = 0; i < count; i++)
        {
            arr[i] = Char(min, max);
        }

        return arr;
    }

    /// <summary>
    /// Get a string of characters of a specific length.
    /// Uses <seealso cref="Chars"/>.
    /// </summary>
    /// <param name="length">The exact length of the result string. If null, a random length is chosen between 40 and 80.</param>
    /// <param name="minChar">Min character value, default char.MinValue.</param>
    /// <param name="maxChar">Max character value, default char.MaxValue.</param>
    public static string String(int? length = null, char minChar = char.MinValue, char maxChar = char.MaxValue)
    {
        var l = length ?? Number(40, 80);

        return new string(Chars(minChar, maxChar, l));
    }

    /// <summary>
    /// Get a string of characters between <paramref name="minLength" /> and <paramref name="maxLength"/>. Uses <seealso cref="Chars"/>.
    /// </summary>
    /// <param name="minLength">Lower-bound string length. Inclusive.</param>
    /// <param name="maxLength">Upper-bound string length. Inclusive.</param>
    /// <param name="minChar">Min character value, default char.MinValue.</param>
    /// <param name="maxChar">Max character value, default char.MaxValue.</param>
    public static string String(int minLength, int maxLength, char minChar = char.MinValue, char maxChar = char.MaxValue)
    {
        var length = Number(minLength, maxLength);
        return String(length, minChar, maxChar);
    }

    /// <summary>
    /// Get a string of characters with a specific length drawing characters from <paramref name="chars"/>.
    /// The returned string may contain repeating characters from the <paramref name="chars"/> string.
    /// </summary>
    /// <param name="length">The length of the string to return.</param>
    /// <param name="chars">The pool of characters to draw from. The returned string may contain repeat characters from the pool.</param>
    public static string String2(int length, string chars = "abcdefghijklmnopqrstuvwxyz")
    {
        var target = new char[length];

        for (var i = 0; i < length; i++)
        {
            var idx = Number(0, chars.Length - 1);
            target[i] = chars[idx];
        }

        return new string(target);
    }

    /// <summary>
    /// Get a string of characters with a specific length drawing characters from <paramref name="chars"/>.
    /// The returned string may contain repeating characters from the <paramref name="chars"/> string.
    /// </summary>
    /// <param name="minLength">The minimum length of the string to return.</param>
    /// <param name="maxLength">The maximum length of the string to return.</param>
    /// <param name="chars">The pool of characters to draw from. The returned string may contain repeat characters from the pool.</param>
    public static string String2(int minLength, int maxLength, string chars = "abcdefghijklmnopqrstuvwxyz")
    {
        var length = Number(minLength, maxLength);
        return String2(length, chars);
    }

    /// <summary>
    /// Get a random boolean.
    /// </summary>
    public static bool Bool() => Number() == 0;

    /// <summary>
    /// Get a random boolean.
    /// </summary>
    /// <param name="weight">The probability of true. Ranges from 0 to 1.</param>
    public static bool Bool(float weight) => Float() < weight;

    /// <summary>
    /// Get a phone number.
    /// </summary>
    public static string PhoneNumber()
    {
        var str = new StringBuilder();

        _ = str.Append('0');

        _ = str.AppendJoin(string.Empty, Digits(9));

        return str.ToString();
    }

    /// <summary>
    /// Get a random array element.
    /// </summary>
    public static T ArrayElement<T>(T[] array)
    {
        var r = Number(max: array.Length - 1);
        return array[r];
    }

    /// <summary>
    /// Get a random array element.
    /// </summary>
    public static string? ArrayElement(Array array)
    {
        var r = Number(max: array.Length - 1);

        return array.GetValue(r)?.ToString();
    }

    /// <summary>
    /// Get a random subset of an array.
    /// </summary>
    /// <param name="array">The source of items to pick from.</param>
    /// <param name="count">The number of elements to pick; otherwise, a random amount is picked.</param>
    public static T[] ArrayElements<T>(T[] array, int? count = null)
    {
        if (count > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        count ??= Number(0, array.Length - 1);

        return [.. Shuffle(array).Take(count.Value)];
    }

    /// <summary>
    /// Get a random list item.
    /// </summary>
    public static T ListItem<T>(IList<T> list)
    {
        var r = Number(max: list.Count - 1);
        return list[r];
    }

    /// <summary>
    /// Get a random subset of a List.
    /// </summary>
    /// <param name="items">The source of items to pick from.</param>
    /// <param name="count">The number of items to pick; otherwise, a random amount is picked.</param>
    public static IList<T> ListItems<T>(IList<T> items, int? count = null)
    {
        if (count > items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        count ??= Number(0, items.Count - 1);

        return [.. Shuffle(items).Take(count.Value)];
    }

    /// <summary>
    /// Get a random collection item.
    /// </summary>
    public static T CollectionItem<T>(ICollection<T> collection)
    {
        var r = Number(max: collection.Count - 1);
        return collection.Skip(r).First();
    }

    /// <summary>
    /// Get a random collection item.
    /// </summary>
    public static T CollectionItem<T>(IReadOnlyCollection<T> collection)
    {
        var r = Number(max: collection.Count - 1);
        return collection.Skip(r).First();
    }

    /// <summary>
    /// Replaces symbols with numbers.
    /// IE: ### -> 283.
    /// </summary>
    /// <param name="format">The string format.</param>
    /// <param name="symbols">The symbol to search for in format that will be replaced with a number.</param>
    public static string ReplaceByNumbers(string format, params char[] symbols) => ReplaceSymbols(format, symbols, _ => Convert.ToChar('0' + Number(9)));

    /// <summary>
    /// Replaces each character instance in a string.
    /// Func is called each time a symbol is encountered.
    /// </summary>
    /// <param name="format">The string with symbols to replace.</param>
    /// <param name="symbols">The symbol to search for in the string.</param>
    /// <param name="func">The function that produces a character for replacement. Invoked each time the replacement symbol is encountered.</param>
    public static string ReplaceSymbols(string format, char[] symbols, Func<char, char> func)
    {
        var chars = format.Select(x => symbols.Contains(x) ? func(x) : x).ToArray();
        return new string(chars);
    }

    /// <summary>
    /// Replaces symbols with numbers and letters. # = number, ? = letter, * = number or letter.
    /// IE: ###???* -> 283QED4. Letters are uppercase.
    /// </summary>
    public static string ReplaceFormat(string format) => ReplaceSymbols(format, ['*', '#', '?'], x =>
    {
        if (x == '*')
        {
            x = Bool() ? '#' : '?';
        }

        return x switch
        {
            '#' => Convert.ToChar('0' + Number(9)),
            '?' => Convert.ToChar('A' + Number(25)),
            _ => x
        };
    });

    /// <summary>
    /// Clamps the length of a string between min and max characters.
    /// If the string is below the minimum, the string is appended with random characters up to the minimum length.
    /// If the string is over the maximum, the string is truncated at maximum characters; additionally, if the result string ends with
    /// whitespace, it is replaced with a random characters.
    /// </summary>
    public static string ClampString(string str, int? min = null, int? max = null)
    {
        if (max != null && str.Length > max)
        {
            str = str[..max.Value].Trim();
        }

        if (min == null || !(min > str.Length)) return str;
        var missingChars = min - str.Length;
        var fillerChars = ReplaceFormat(string.Empty.PadRight(missingChars.Value, '?'));
        return str + fillerChars;
    }

    /// <summary>
    /// Picks a random Enum of T. Works only with Enums.
    /// </summary>
    /// <typeparam name="T">Must be an Enum.</typeparam>
    /// <param name="exclude">Exclude enum values from being returned.</param>
    public static T Enum<T>(params T[] exclude)
        where T : struct
    {
        var e = typeof(T);

        var selection = System.Enum.GetNames(e);

        if (exclude.Length > 0)
        {
            var excluded = exclude.Select(ex => System.Enum.GetName(e, ex) ?? string.Empty);
            selection = [.. selection.Except(excluded)];
        }

        var val = ArrayElement(selection);

        _ = System.Enum.TryParse(val, out T picked);
        return picked;
    }

    /// <summary>
    /// Shuffles an IEnumerable source.
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
    {
        var buffer = source.ToList();
        for (var i = 0; i < buffer.Count; i++)
        {
            int j;

            // lock any seed access, for thread safety.
            lock (Locker)
            {
                j = Seed.Next(i, buffer.Count);
            }

            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    public static string Color()
    {
        // lock any seed access, for thread safety.
        lock (Locker)
        {
            return $"#{Seed.Next(0x1000000):X6}";
        }
    }

    public static Country Country() => ListItem(EnumClass.GetAll<Country>());
}
