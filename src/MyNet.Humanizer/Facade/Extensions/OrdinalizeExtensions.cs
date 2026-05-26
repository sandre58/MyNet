// -----------------------------------------------------------------------
// <copyright file="OrdinalizeExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MyNet.Globalization.Facade;
using MyNet.Humanizer.Ordinalizing;
using MyNet.Text;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Humanizer.Facade;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Ordinalize extensions.
/// </summary>
public static class OrdinalizeExtensions
{
    extension(string numberString)
    {
        /// <summary>
        /// Turns a number into an ordinal string used to denote the position in an ordered sequence such as 1st, 2nd, 3rd, 4th.
        /// </summary>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(long.Parse(numberString, culture));

        /// <summary>
        /// Turns a number into an ordinal string used to denote the position in an ordered sequence such as 1st, 2nd, 3rd, 4th.
        /// Gender for Brazilian Portuguese locale
        /// "1".Ordinalize(GrammaticalGender.Masculine) -> "1�"
        /// "1".Ordinalize(GrammaticalGender.Feminine) -> "1�".
        /// </summary>
        /// <param name="gender">The grammatical gender to use for output words.</param>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(GrammaticalGender gender, CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(long.Parse(numberString, culture), new() { Gender = gender });
    }

    extension(int number)
    {
        /// <summary>
        /// Turns a number into an ordinal number used to denote the position in an ordered sequence such as 1st, 2nd, 3rd, 4th.
        /// </summary>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(number);

        /// <summary>
        /// Turns a number into an ordinal number used to denote the position in an ordered sequence such as 1st, 2nd, 3rd, 4th.
        /// Gender for Brazilian Portuguese locale
        /// 1.Ordinalize(GrammaticalGender.Masculine) -> "1�"
        /// 1.Ordinalize(GrammaticalGender.Feminine) -> "1�".
        /// </summary>
        /// <param name="gender">The grammatical gender to use for output words.</param>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(GrammaticalGender gender, CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(number, new() { Gender = gender });
    }

    extension(long number)
    {
        /// <summary>
        /// Turns a long number into an ordinal number used to denote the position in an ordered sequence such as 1st, 2nd, 3rd, 4th.
        /// </summary>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(number);

        /// <summary>
        /// Turns a long number into an ordinal number, taking grammatical gender into account.
        /// </summary>
        /// <param name="gender">The grammatical gender to use for output words.</param>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(GrammaticalGender gender, CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(number, new() { Gender = gender });
    }

    extension(uint number)
    {
        /// <summary>
        /// Turns an unsigned integer into an ordinal number used to denote the position in an ordered sequence.
        /// </summary>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(number);

        /// <summary>
        /// Turns an unsigned integer into an ordinal number, taking grammatical gender into account.
        /// </summary>
        /// <param name="gender">The grammatical gender to use for output words.</param>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(GrammaticalGender gender, CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize(number, new() { Gender = gender });
    }

    extension(ulong number)
    {
        /// <summary>
        /// Turns an unsigned long into an ordinal number used to denote the position in an ordered sequence.
        /// </summary>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize((long)number);

        /// <summary>
        /// Turns an unsigned long into an ordinal number, taking grammatical gender into account.
        /// </summary>
        /// <param name="gender">The grammatical gender to use for output words.</param>
        /// <param name="culture">Culture to use. If null, current thread's UI culture is used.</param>
        public string Ordinalize(GrammaticalGender gender, CultureInfo? culture = null) => Localizer.ForCulture(culture).GetRequired<IOrdinalizer>().Ordinalize((long)number, new() { Gender = gender });
    }
}
